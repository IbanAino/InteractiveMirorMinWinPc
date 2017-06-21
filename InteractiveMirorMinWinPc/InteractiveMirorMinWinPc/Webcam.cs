using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;


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


        public Webcam()
        {

        }
   
        public Image TakePicture()
        {
            
            _capture = new VideoCapture();

            Image<Bgr, Byte> image = _capture.QueryFrame().ToImage<Bgr, Byte>();

            //image.ToBitmap().Save("filename.png");

            _capture.Dispose();

            return image.ToBitmap();
        }
    }
}
