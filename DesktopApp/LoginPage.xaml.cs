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
using DesktopApp.Services;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private LoginService LoginService { get; set; }
        public LoginPage()
        {
            InitializeComponent();
            LoginService = new LoginService();
            
        }

        public void OnContinueClicked(object sender, EventArgs args)
        {
            var isValid = LoginService.IsValid(UserName.Text, Password.Text);
            if (isValid)
            {
                this.NavigationService.Navigate(new MainPage());
            }
        }
    }
}
