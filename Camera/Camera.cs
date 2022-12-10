using System.Windows.Media.Imaging;
using Emgu.CV;


namespace Billiard.Camera
{
    internal class Camera
    {
        public static BitmapSource capture()
        {
            VideoCapture capture = new VideoCapture();
            using (var frame = capture.QueryFrame())
            {
                return frame.ToBitmapSource();
            }
        }
        /*
                    ImageViewer viewer = new ImageViewer(); //create an image viewer
                    Capture capture = new Capture(); //create a camera captue
                    Application.Idle += new EventHandler(delegate (object sender, EventArgs e)
                    {  //run this until application closed (close button click on image viewer)
                        viewer.Image = capture.QueryFrame(); //draw the image obtained from camera
                    });
                    viewer.ShowDialog();
        */
    }
}
