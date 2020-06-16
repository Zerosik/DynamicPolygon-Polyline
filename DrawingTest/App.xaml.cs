using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace DrawingTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {


            MainWindow mainWindow = new MainWindow();

            if (Screen.AllScreens.Length > 1)
            {
                Screen s2 = Screen.AllScreens[1];
                Rectangle r2 = s2.WorkingArea;
                mainWindow.Top = r2.Top;
                mainWindow.Left = r2.Left;
                //mainWindow.WindowState = WindowState.Maximized;
                mainWindow.Show();
            }

            else
            {
                Screen s1 = Screen.AllScreens[0];
                Rectangle r1 = s1.WorkingArea;
                mainWindow.Top = r1.Top;
                mainWindow.Left = r1.Left;
                //mainWindow.WindowState = WindowState.Maximized;
                mainWindow.Show();
            }
            mainWindow.WindowState = WindowState.Maximized;

        }
    }
}
