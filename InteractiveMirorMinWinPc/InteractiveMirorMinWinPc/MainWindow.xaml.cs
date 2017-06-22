using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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

        int count = 0;

        System.Drawing.Image image;

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
            checkBoxStateButton.SetValue(CheckBox.IsCheckedProperty, true);
            //Task.Delay(1000).Wait();
            //checkBoxStateButton.SetValue(CheckBox.IsCheckedProperty, false);

            System.Drawing.Image image;
            
            // take a picture
            image = webcam.TakePicture();

            // look for a face in the picture
            bool aFaceIsDetected = faceDetection.DetectFace(image);

            //checkBoxStateButton.SetValue(CheckBox.IsCheckedProperty, aFaceIsDetected);

            // if there is a face in the picture, analyse the face
            if (aFaceIsDetected)
            {
                string response = await emotionApi.MakeRequestBitmap2(image);
                textBlockEmotionApi.Text = "microsoft says :\n";
                textBlockEmotionApi.Text += response;
            }
            else
            {
                textBlockEmotionApi.Text = "openCV hasn't detected any face";
            }
        }

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            bool nextLoop = true;

            while (nextLoop)
            {
                count++;
                //nextLoop = false;
                textBlockEmotionApi.Text += "\n \n loop begining, loop number " + count.ToString() + "\n";

                checkBoxStateButton.SetValue(CheckBox.IsCheckedProperty, true);

                //System.Drawing.Image image;



                // take a picture
                textBlockEmotionApi.Text += "1 - picture soonly taken \n";

                Task<System.Drawing.Image> task2 = Task.Run(new Func<System.Drawing.Image>(webcam.TakePicture));
                image = task2.Result;
                
                Debug.Print("1 - picture taken");
                textBlockEmotionApi.Text += "1 - picture taken \n";


                
                // look for a face in the picture
                textBlockEmotionApi.Text += "2 - picture soonly analyzed \n";

                Task <bool> task3 = Task<bool>.Factory.StartNew(() =>
                {
                    bool x = faceDetection.DetectFace(image);
                    return x;
                });
                bool aFaceIsDetected = task3.Result;
                Debug.Print("2 - picture analyzed");

                textBlockEmotionApi.Text += "2 - picture analyzed \n";
                
                //bool aFaceIsDetected = true;


                // if there is a face in the picture, analyse the face
                textBlockEmotionApi.Text += "3 - picture soonly submitted (or not) \n";
                if (aFaceIsDetected)
                {
                    string response = await emotionApi.MakeRequestBitmap2(image);
                    textBlockEmotionApi.Text = response;
                    Debug.Print("3.1 - emotion Api called");
                    textBlockEmotionApi.Text += "\n 3.1 - API Emotion called \n";
                }
                else
                {
                    textBlockEmotionApi.Text = "openCV hasn't detected any face";
                    Debug.Print("3.2 - picture no analyzed - no face detected");
                }
                //Task.Delay(500).Wait();
            }
        }

        private async void button2_Click(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                count++;
                
                // take a photo
                Task<System.Drawing.Image> task1 = Task.Run(new Func<System.Drawing.Image>(webcam.TakePicture));
                image = task1.Result;

                // look for a face in the photo
                bool aFaceIsDetected = await Task.Run(() => faceDetection.DetectFace(image));

                if (aFaceIsDetected)
                {
                    string response = await Task.Run(() => emotionApi.MakeRequestBitmap2(image));
                    textBlockEmotionApi.Text = response;
                }
                else
                {
                    textBlockEmotionApi.Text = "no face detected by OpenCV";
                }

                textBlockEmotionApi.Text += "\n iteration number " + count.ToString();
            }

        }
    } 
}
