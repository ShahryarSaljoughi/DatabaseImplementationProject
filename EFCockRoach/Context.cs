using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using EFCockRoach.Models;
using Npgsql;

namespace EFCockRoach
{
    //[DbConfigurationType(typeof(AppDbContextConfiguration))]
    class Context: DbContext
    {
        public DbSet<Member> Members { get; set; }
        public Context(): base(GetConnection(), true)
        {
            Database.SetInitializer<Context>(null);
        }
        public static DbConnection GetConnection()
        {
            var connStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = "localhost",
                Port = 26257,
                SslMode = SslMode.Require,
                Username = "shahryar",
                Password = "1234",
                Database = "tmdb",
                TrustServerCertificate = true,
                
            };
            var connectionString =  connStringBuilder.ConnectionString;
            var conn = new NpgsqlConnection(connectionString);
            conn.ProvideClientCertificatesCallback += ProvideClientCertificatesCallback;
            conn.UserCertificateValidationCallback += UserCertificateValidationCallback;
            return conn;
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // needed!
            modelBuilder.Properties().Configure
                (c => c.HasColumnName(c.ClrPropertyInfo.Name.ToLower()));
        }
    }

    

    public class AppDbContextConfiguration : DbConfiguration
    {
        public AppDbContextConfiguration()
        {
            var name = "Npgsql";

            SetProviderFactory(providerInvariantName: name,
                providerFactory: NpgsqlFactory.Instance);

            SetProviderServices(providerInvariantName: name,
                provider: NpgsqlServices.Instance);

            SetDefaultConnectionFactory(connectionFactory: new NpgsqlConnectionFactory());
        }
        
        public DbSet<Member> Members { get; set; }
    }

   
}
