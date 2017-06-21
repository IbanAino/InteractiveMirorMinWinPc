using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        // instanciations of classes
        Webcam webcam = new Webcam();
        EmotionApi emotionApi = new EmotionApi();
        FacesDetection faceDetection = new FacesDetection();

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
            System.Drawing.Image image;
            
            // take a picture
            image = webcam.TakePicture();

            // look for a face in the picture
            bool aFaceIsDetected = faceDetection.DetectFace(image);

            checkBoxStateButton.SetValue(CheckBox.IsCheckedProperty, aFaceIsDetected);

            // if there is a face in the picture, analyse the face
            if (aFaceIsDetected)
            {
                string response = await emotionApi.MakeRequestBitmap2(image);
                textBlockEmotionApi.Text = response;
            }
            else
            {
                textBlockEmotionApi.Text = "openCV hasn't detected any face";
            }
        }

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                System.Drawing.Image image;

                // take a picture
                image = webcam.TakePicture();

                // look for a face in the picture
                bool aFaceIsDetected = faceDetection.DetectFace(image);

                checkBoxStateButton.SetValue(CheckBox.IsCheckedProperty, aFaceIsDetected);

                // if there is a face in the picture, analyse the face
                if (aFaceIsDetected)
                {
                    string response = await emotionApi.MakeRequestBitmap2(image);
                    textBlockEmotionApi.Text = response;
                }
                else
                {
                    textBlockEmotionApi.Text = "openCV hasn't detected any face";
                }
            }
        }
    }
}
