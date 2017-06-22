using System;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

//using Microsoft.ProjectOxford;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;

using System.IO;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace InteractiveMirorMinWinPc
{
    class EmotionApi
    {
        //ATTRIBUTS
        Emotion[] emotionResult;
        EmotionServiceClient emotionServiceClient;

        //CONSTRUCTOR
        public EmotionApi()
        {
            string apiKey = "1dd1f4e23a5743139399788aa30a7153";
            emotionServiceClient = new EmotionServiceClient(apiKey);
        }


        public async Task<float[]> MakeRequestBitmap2(Image image)
        {

            //FileStream stream3 = new FileStream("portrait.jpeg", FileMode.Open, FileAccess.Read);

            // convert the picture to a stream
            var stream2 = new System.IO.MemoryStream();
            try
            {
                image.Save(stream2, ImageFormat.Png);
                stream2.Position = 0;
            }catch{
                return new float[] { 1 };
            }


            // ask the emotion Api
            try{
                emotionResult = await emotionServiceClient.RecognizeAsync(stream2);
            }catch{
                //return "error asking emotion Api";
                return new float[] { 2 };
            }


            // read the response
            Microsoft.ProjectOxford.Common.Contract.EmotionScores scores;


            try
            {
                scores = emotionResult[0].Scores;
            }
            catch
            {
                //return "Microsoft Emotion Api has no detected any emotion 01";
                return new float[] { 3 };
            }


            string response = "Microsoft Emotion Api has no detected any emotion 02";
            float[] response2 = null;

            if (emotionResult != null){
                response =
                    "Happiness : " + scores.Happiness + "\n" +
                    "Sadness : " + scores.Sadness + "\n" +
                    "Surprise : " + scores.Surprise + "\n" +
                    "Fear : " + scores.Fear + "\n" +
                    "Anger : " + scores.Anger + "\n" +
                    "Comtempt : " + scores.Contempt + "\n" +
                    "Disgust : " + scores.Disgust + "\n" +
                    "Neutral : " + scores.Neutral;

                response2 = new float[] {
                    float.Parse(scores.Happiness.ToString()),
                    float.Parse(scores.Sadness.ToString()),
                    float.Parse(scores.Surprise.ToString()),
                    float.Parse(scores.Fear.ToString()),
                    float.Parse(scores.Anger.ToString()),
                    float.Parse(scores.Contempt.ToString()),
                    float.Parse(scores.Disgust.ToString()),
                    float.Parse(scores.Neutral.ToString())};
            }

            return response2;  
        }
    }
}

