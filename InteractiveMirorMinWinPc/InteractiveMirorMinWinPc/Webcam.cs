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

using System.Windows.Media.Imaging;
using System.Threading.Tasks;

//using FaceDetection;

namespace InteractiveMirorMinWinPc
{
    class Webcam
    {
        ImageViewer viewer = new ImageViewer(); //create an image viewer

        // for the camera capture
        private VideoCapture _capture = null;

        // 2



        public Webcam()
        {

        }
   
        public Image TakePicture()
        {
            
            _capture = new VideoCapture();
            //_capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, 720 );
            //_capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, 1280);

            Image<Bgr, Byte> image = _capture.QueryFrame().ToImage<Bgr, Byte>();

            image.ToBitmap().Save("filename.png");


            _capture.Dispose();

            return image.ToBitmap();
        }

    }
}
