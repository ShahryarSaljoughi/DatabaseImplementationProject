using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using DesktopApp.Data.Models;
using AppContext = DesktopApp.Data.Context.AppContext;

namespace DesktopApp.Services
{
    class LoginService
    {
        public static Member CurrentUser;
        
        public bool IsValid(string username = "shahryar",string passwor = "my pass")
        {

            // todo: really implement it!
            using (var db = new AppContext())
            {
                var me = db.Members.FirstOrDefault(m => m.FirstName == username);
                if (me is null)
                    return false;
            }
            return true;
        }
    }
}
