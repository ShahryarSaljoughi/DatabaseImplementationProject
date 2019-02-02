using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCockRoach.Models;

namespace EFCockRoach
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new Context())
            {
                // todo : certificate validation
                var a = new Member() {
                    Id = Guid.NewGuid()
                    , FirstName = "pegah"
                    , LastName = "fateh"
                };

                db.Members.Add(a);
                var me = db.Members.FirstOrDefault();
                System.Console.Write(me.FirstName);
                db.SaveChanges();
            }
        }
    }
}
