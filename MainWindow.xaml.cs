using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace BayoCosTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private MainWindowViewModel _viewModel;
        private string file_name = "";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Nullable<bool> result;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            dlg.Filter = "Cos file (*.cos)|*.cos|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            result = dlg.ShowDialog();
            // User didn't select a file so don't do anything
            if (result == false)
                return;

            // Load the file the user selected  
            Console.WriteLine(dlg.FileName);

            return;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            return;
        }

        private void CloseCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void SaveAsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}
