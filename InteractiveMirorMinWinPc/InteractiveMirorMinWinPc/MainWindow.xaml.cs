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
                    float[] response = await Task.Run(() => emotionApi.MakeRequestBitmap2(image));

                    if(response.Length > 1){
                        /*
                        textBlockEmotionApi.Text =
                            "Happiness : " + response[0].ToString() + "\n" +
                            "Sadness : " + response[1].ToString() + "\n" +
                            "Surprise : " + response[2].ToString() + "\n" +
                            "Fear : " + response[3].ToString() + "\n" +
                            "Anger : " + response[4].ToString() + "\n" +
                            "Comtempt : " + response[5].ToString() + "\n" +
                            "Disgust : " + response[6].ToString() + "\n" +
                            "Neutral : " + response[7].ToString();
                            */

                        happiness.Width = response[0] * 100;
                        sadness.Width = response[1] * 100;
                        surprise.Width = response[2] * 100;
                        fear.Width = response[3] * 100;
                        anger.Width = response[4] * 100;
                        contempt.Width = response[5] * 100;
                        disgust.Width = response[6] * 100;
                        neutral.Width = response[7] * 100;
                    }
                    else
                    {
                        textBlockEmotionApi.Text = "ERROR n°" + response[0];
                    }
                }
                else
                {
                    textBlockEmotionApi.Text = "no face detected by OpenCV";
                }

                textBlockEmotionApi.Text += "\n iteration number " + count.ToString();
            }

        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            System.Drawing.Image image1 = null;
            System.Drawing.Image image2 = null;
            System.Drawing.Image image3 = null;
            bool aFaceIsDetected = false;
            bool aFaceIsDetectedNextLoop = false;
            float[] response = null;

            while (true)
            {
                count++;

                List<Task> tasks = new List<Task>();

                // take a photo
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    image1 = webcam.TakePicture();
                }));

                // look for a face in the photo
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    if(image2 != null)
                        aFaceIsDetected = faceDetection.DetectFace(image2);
                }));

                // ask microsoft emotion Api                
                tasks.Add(Task.Factory.StartNew(async () =>
                {
                    if (aFaceIsDetected && image3 != null)
                    {
                        response = await emotionApi.MakeRequestBitmap2(image3);                       
                    }                    
                }));

                /*
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(3000);
                }));
                */

                await Task.WhenAll(tasks);

                textBlockEmotionApi.Text = "- iteration n°" + count + "\n- threads appelés\n";

                if (!aFaceIsDetectedNextLoop)
                    textBlockEmotionApi.Text += "- no face detected\n";
                else
                {
                    textBlockEmotionApi.Text += "- a face is detected\n";
                    if (response != null)
                    {
                        if (response.Length > 1)
                        {
                            textBlockEmotionApi.Text +=
                                "\n\n\n" +
                                "Happiness : " + response[0].ToString() + "\n" +
                                "Sadness : " + response[1].ToString() + "\n" +
                                "Surprise : " + response[2].ToString() + "\n" +
                                "Fear : " + response[3].ToString() + "\n" +
                                "Anger : " + response[4].ToString() + "\n" +
                                "Comtempt : " + response[5].ToString() + "\n" +
                                "Disgust : " + response[6].ToString() + "\n" +
                                "Neutral : " + response[7].ToString();

                            // visual datas on the screen
                            happiness.Width = response[0] * 100;
                            sadness.Width = response[1] * 100;
                            surprise.Width = response[2] * 100;
                            fear.Width = response[3] * 100;
                            anger.Width = response[4] * 100;
                            contempt.Width = response[5] * 100;
                            disgust.Width = response[6] * 100;
                            neutral.Width = response[7] * 100;
                        }
                        else
                        {
                            textBlockEmotionApi.Text += "- ERROR n°" + response[0];
                        }
                    }
                }

                image3 = image2;
                image2 = image1;

                aFaceIsDetectedNextLoop = aFaceIsDetected;
            }

        }
    } 
}
