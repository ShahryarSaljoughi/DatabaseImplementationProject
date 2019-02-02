using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace testDB
{
    class Program
    {
        static void Main(string[] args)
        {
            var connStringBuilder = new NpgsqlConnectionStringBuilder();
            connStringBuilder.Host = "localhost";
            connStringBuilder.Port = 26257;
            connStringBuilder.SslMode = SslMode.Require;
            connStringBuilder.Username = "shahryar";
            connStringBuilder.Database = "tmdb";
            Simple(connStringBuilder.ConnectionString);
        }
        static void Simple(string connString)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.ProvideClientCertificatesCallback += ProvideClientCertificatesCallback;
                conn.UserCertificateValidationCallback += UserCertificateValidationCallback;
                conn.Open();

                // Create the "accounts" table.
                new NpgsqlCommand("CREATE TABLE IF NOT EXISTS accounts (id INT PRIMARY KEY, balance INT)", conn).ExecuteNonQuery();

                // Insert two rows into the "accounts" table.
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "UPSERT INTO accounts(id, balance) VALUES(@id1, @val1), (@id2, @val2)";
                    cmd.Parameters.AddWithValue("id1", 1);
                    cmd.Parameters.AddWithValue("val1", 1000);
                    cmd.Parameters.AddWithValue("id2", 2);
                    cmd.Parameters.AddWithValue("val2", 250);
                    cmd.ExecuteNonQuery();
                }

                // Print out the balances.
                System.Console.WriteLine("Initial balances:");
                using (var cmd = new NpgsqlCommand("SELECT id, balance FROM accounts", conn))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        Console.Write("\taccount {0}: {1}\n", reader.GetValue(0), reader.GetValue(1));
            }
        }

        static void ProvideClientCertificatesCallback(X509CertificateCollection clientCerts)
        {
            // To be able to add a certificate with a private key included, we must convert it to
            // a PKCS #12 format. The following openssl command does this:
            // openssl pkcs12 -password pass: -inkey client.maxroach.key -in client.maxroach.crt -export -out client.maxroach.pfx
            // As of 2018-12-10, you need to provide a password for this to work on macOS.
            // See https://github.com/dotnet/corefx/issues/24225

            // Note that the password used during X509 cert creation below
            // must match the password used in the openssl command above.
            clientCerts.Add(new X509Certificate2("I:/cockroachDBExamples/certs/client.shahryar.pfx", ""));
        }

        // By default, .Net does all of its certificate verification using the system certificate store.
        // This callback is necessary to validate the server certificate against a CA certificate file.
        static bool UserCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain defaultChain, SslPolicyErrors defaultErrors)
        {
            X509Certificate2 caCert = new X509Certificate2("I:/cockroachDBExamples/certs/ca.crt");
            X509Chain caCertChain = new X509Chain();
            caCertChain.ChainPolicy = new X509ChainPolicy()
            {
                RevocationMode = X509RevocationMode.NoCheck,
                RevocationFlag = X509RevocationFlag.EntireChain
            };
            caCertChain.ChainPolicy.ExtraStore.Add(caCert);

            X509Certificate2 serverCert = new X509Certificate2(certificate);

            caCertChain.Build(serverCert);
            if (caCertChain.ChainStatus.Length == 0)
            {
                // No errors
                return true;
            }

            foreach (X509ChainStatus status in caCertChain.ChainStatus)
            {
                // Check if we got any errors other than UntrustedRoot (which we will always get if we don't install the CA cert to the system store)
                if (status.Status != X509ChainStatusFlags.UntrustedRoot)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
