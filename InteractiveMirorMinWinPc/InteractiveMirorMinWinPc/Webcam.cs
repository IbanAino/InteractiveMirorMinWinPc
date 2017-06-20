using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
#if !(__IOS__ || NETFX_CORE)
using Emgu.CV.Cuda;
#endif

using System.Drawing.Imaging;
using System.IO;



//using FaceDetection;

namespace InteractiveMirorMinWinPc
{
    class Webcam
    {
        ImageViewer viewer = new ImageViewer(); //create an image viewer

        // for the camera capture
        private VideoCapture _capture = null;
        private bool _captureInProgress;
        private Mat _frame;
        private Mat _grayFrame;
        private Mat _smallGrayFrame;
        private Mat _smoothedGrayFrame;
        private Mat _cannyFrame;


        public Webcam()
        {

        }
 
        //-------------------------------------take picture-----------------       
        public string TakePicture()
        {
            
            _capture = new VideoCapture();
            /*
            //_capture.ImageGrabbed += ProcessFrame;
            viewer.Image = _capture.QueryFrame();
            Image.toBitmap().Save("filename.png");
            */
            Image<Bgr, Byte> image = _capture.QueryFrame().ToImage<Bgr, Byte>();

            image.ToBitmap().Save("filename.png");

            return "Charlotte";
        }

        /*
        private void ProcessFrame(object sender, EventArgs arg)
        {
            if (_capture != null && _capture.Ptr != IntPtr.Zero)
            {
                _capture.Retrieve(_frame, 0);

                CvInvoke.CvtColor(_frame, _grayFrame, ColorConversion.Bgr2Gray);

                CvInvoke.PyrDown(_grayFrame, _smallGrayFrame);

                CvInvoke.PyrUp(_smallGrayFrame, _smoothedGrayFrame);

                CvInvoke.Canny(_smoothedGrayFrame, _cannyFrame, 100, 60);

                captureImageBox.Image = _frame;
                grayscaleImageBox.Image = _grayFrame;
                smoothedGrayscaleImageBox.Image = _smoothedGrayFrame;
                cannyImageBox.Image = _cannyFrame;
            }
        }
        */
        //---------------------------------end take picture---------------------------------


        private void button1_Click(object sender, EventArgs e)
        {

        }

        public bool DetectFace()
        {
            Debug.Print("function TakePicture called");

            bool functionResponse = false;
            long detectionTime;

            IImage image;
            //Read the files as an 8-bit Bgr image  


            image = new UMat("lena.jpg", ImreadModes.Color); //UMat version

            //image = new UMat("landscape.jpg", ImreadModes.Color); //UMat version

            String faceFileName = "haarcascade_frontalface_default.xml";
            List<Rectangle> faces = new List<Rectangle>();

            Stopwatch watch;

            using (InputArray iaImage = image.GetInputArray())
            {
                Debug.Print("InputArray iaImage called");
#if !(__IOS__ || NETFX_CORE)
                if (iaImage.Kind == InputArray.Type.CudaGpuMat && CudaInvoke.HasCuda)
                {
                    using (CudaCascadeClassifier face = new CudaCascadeClassifier(faceFileName))
                    {
                        face.ScaleFactor = 1.1;
                        face.MinNeighbors = 10;
                        face.MinObjectSize = Size.Empty;
                        watch = Stopwatch.StartNew();
                        using (CudaImage<Bgr, Byte> gpuImage = new CudaImage<Bgr, byte>(image))
                        using (CudaImage<Gray, Byte> gpuGray = gpuImage.Convert<Gray, Byte>())
                        using (GpuMat region = new GpuMat())
                        {
                            face.DetectMultiScale(gpuGray, region);
                            Rectangle[] faceRegion = face.Convert(region);
                            faces.AddRange(faceRegion);

                            if (faces[0] != null)
                            {
                                functionResponse = true;
                            }
                        }
                    }
                }
                else
#endif
                {
                    Debug.Print("else called");
                    using (CascadeClassifier face = new CascadeClassifier(faceFileName))
                    {
                        watch = Stopwatch.StartNew();

                        using (UMat ugray = new UMat())
                        {
                            CvInvoke.CvtColor(image, ugray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

                            //normalizes brightness and increases contrast of the image
                            CvInvoke.EqualizeHist(ugray, ugray);

                            //Detect the faces  from the gray scale image and store the locations as rectangle
                            //The first dimensional is the channel
                            //The second dimension is the index of the rectangle in the specific channel                     
                            Rectangle[] facesDetected = face.DetectMultiScale(
                               ugray,
                               1.1,
                               10,
                               new Size(20, 20));

                            faces.AddRange(facesDetected);

                            int facesCount = facesDetected.Length;

                            if (facesCount != 0)
                            {
                                functionResponse = true;
                            }
                        }
                        watch.Stop();
                    }
                }
                detectionTime = watch.ElapsedMilliseconds;
            }            
            return functionResponse;
        }
    }
}
