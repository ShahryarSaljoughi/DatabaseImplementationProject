using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using DesktopApp.Data.Models;
using AppContext = DesktopApp.Data.Context.AppContext;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _mainFrame.Navigate(new LoginPage());
        }

        public void TestDbConnection(object sender, RoutedEventArgs args)
        {
            using (var db = new AppContext())
            {
                var me = db.Members.FirstOrDefault();
                System.Console.Write(me.FirstName);
            }
        }
    }
}
