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
        float refresfScrennDatasCount;

        bool refreshScreenDatas = false;

        //System.Drawing.Image image;

        float[] emotions = new float[8];
        float[] oldEmotions = new float[8];
        float[] displayedEmotions = new float[8];

        // DispatcherTimer setup
        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        DispatcherTimer dispatcherTimer3 = new System.Windows.Threading.DispatcherTimer();

        // dispatcher timer to refresh the screen datas
        System.Timers.Timer dispatcherTimer2 = new System.Timers.Timer();

        // instanciations of classes
        Webcam webcam = new Webcam();
        EmotionApi emotionApi = new EmotionApi();
        FacesDetection faceDetection = new FacesDetection();
        WeatherApi weatherApi = new WeatherApi();

        // CONSTRUCTOR
        public MainWindow()
        {
            InitializeComponent();

            // display the date and the time every second
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            // refresh the data screen 24 time per second
            dispatcherTimer3.Tick += new EventHandler(dispatcherTimer_Tick2);
            dispatcherTimer3.Interval = new TimeSpan(0, 0, 0, 0, 42);
            dispatcherTimer3.Start();
        }

        // DISPLAY THE TIME AND THE DATE
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Updating the Label which displays the current second
            textBlockTime.Text = DateTime.Now.ToString();
            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }


        // REFRESH SCREEN DATAS

        private void dispatcherTimer_Tick2(object sender, EventArgs e)
        {
            refresfScrennDatasCount++;

            if (refreshScreenDatas)
            {
                if (refresfScrennDatasCount < 14)
                {                    
                    for (int i = 0; i < 8; i++)
                    {
                        displayedEmotions[i] = oldEmotions[i] + (emotions[i] - oldEmotions[i]) * refresfScrennDatasCount * 0.0714f;

                        if (displayedEmotions[i] < 0)
                            displayedEmotions[i] = 0;
                    }                    

                    displayDatas(displayedEmotions);

                    refresfScrennDatasCount++;
                    countDisplayed.Text = refresfScrennDatasCount.ToString() + "\n new : " + emotions[0] + "\n old : " + oldEmotions[0] + "\n dis : " + displayedEmotions[0] ;
                }
                
                else
                {
                    refresfScrennDatasCount = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        oldEmotions[i] = emotions[i];
                        //displayedEmotions[i] = 0;
                    }
                    refreshScreenDatas = false;
                    countDisplayed.Text = "finit";
                }
            }            
        }

        private void displayDatas(float[] response)
        {
            happiness.Width = response[0];
            sadness.Width = response[1];
            surprise.Width = response[2];
            fear.Width = response[3];
            anger.Width = response[4];
            contempt.Width = response[5];
            disgust.Width = response[6];
            neutral.Width = response[7];
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            System.Drawing.Image image1 = null;
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
                    // take a photo
                    image1 = webcam.TakePicture();

                    // look for a face in the photo
                    if (image1 != null)
                        aFaceIsDetected = faceDetection.DetectFace(image1);
                }));

                // ask microsoft emotion Api                
                tasks.Add(Task.Factory.StartNew(async () =>
                {
                    if (aFaceIsDetected && image3 != null)
                    {
                        response = await emotionApi.MakeRequestBitmap2(image3);                       
                    }                    
                }));

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

                            for (int i = 0; i < 8; i++)
                            {
                                emotions[i] = response[i] * 100;
                            }

                            if (!refreshScreenDatas)
                            {
                                refresfScrennDatasCount = 0;
                                refreshScreenDatas = true;
                            }
                        }
                        else
                        {
                            textBlockEmotionApi.Text += "- ERROR n°" + response[0];
                        }
                    }
                }

                image3 = image1;

                aFaceIsDetectedNextLoop = aFaceIsDetected;
            }
        }

        // button to ask the weather
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String response = weatherApi.checkWeather(3034473);
                weatherTextBlock.Text = response;
            }
            catch
            {
                weatherTextBlock.Text = "Weather Api ERROR";
            }
        }
    } 
}
