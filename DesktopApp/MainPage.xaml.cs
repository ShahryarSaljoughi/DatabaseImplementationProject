using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DesktopApp.Data.Context;
using DesktopApp.Data.Models;
using AppContext = DesktopApp.Data.Context.AppContext;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public async void AddUser(object sender, EventArgs args)
        {
            var firstName = FirstName.Text;
            var LastName = this.LastName.Text;
            using (var db = new AppContext())
            {
                var newMember = new Member()
                {
                    FirstName = firstName,
                    LastName = LastName,
                    Id = Guid.NewGuid()
                };
                db.Members.Add(newMember);
                await db.SaveChangesAsync();
            }
        }
    }
}
