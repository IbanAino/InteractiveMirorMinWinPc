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
using System.Windows.Threading;



namespace InteractiveMirorMinWinPc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // ATTRIBUTS

        // DispatcherTimer setup
        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        Webcam webcam = new Webcam();

        // CONSTRUCTOR
        public MainWindow()
        {
            InitializeComponent();

            // display the date and the time every second
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();            
        }

        // DISPLAY THE TIME AND THE DATE
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Updating the Label which displays the current second
            textBlockTime.Text = DateTime.Now.ToString();
            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }

        // DISPLAY BUTTON STATE
        private async void button_Click(object sender, RoutedEventArgs e)
        {
            
            // physical button pressed
            bool a = webcam.DetectFace();
            //string a = webcam.TakePicture();
            /*
            checkBoxStateButton.SetValue(CheckBox.IsCheckedProperty, true);
            await Task.Delay(500);
            checkBoxStateButton.SetValue(CheckBox.IsCheckedProperty, false);
            */
            checkBoxStateButton.SetValue(CheckBox.IsCheckedProperty, a);
        }
    }
}
