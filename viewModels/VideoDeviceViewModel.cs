using Billiard.Camera.Devices;
using Billiard.UI;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Emgu.CV.CvEnum;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Billiard.Threading;

namespace Billiard.viewModels
{
    public class VideoDeviceViewModel : ViewModel
    {
        private VideoDevice selectedVideoDevice;
        private VideoCapture camera;

        public IReadOnlyList<VideoDevice> VideoDevices
        {
            get
            {
                using SystemDevices systemDevice = new SystemDevices();
                IReadOnlyList<VideoDevice> devices = systemDevice.VideoDevices();
                SelectedVideoDevice = devices.LastOrDefault();
                return devices;
            }
        }

        public VideoDevice SelectedVideoDevice
        {
            get { return selectedVideoDevice; }
            set
            {
                if (SetProperty(ref selectedVideoDevice, value))
                {
                    camera?.Dispose();
                    camera = null;
                }
            }
        }

        public VideoCapture Camera
        {
            get
            {
                if (camera == null && SelectedVideoDevice != null)
                {
                    camera = new VideoCapture(SelectedVideoDevice.Index);
                }

                return camera;
            }
        }

        public ICommand CaptureCommand
        {
            get { return new TargetCommand(Captures); }
        }

        private void Captures()
        {
            if (Camera == null
                || !Camera.Grab())
            {
                return;
            }

            Mat image = Camera.QueryFrame();
            if (image != null)
            {
                OnCaptureImage(image);
            }
        }

        public bool Calculate
        {
            get { return calculate; }
            set { SetProperty(ref calculate, value); }
        }

        private FileSystemWatcher FolderWatcher;

        public bool LoadFromFolder
        {
            get { return loadFromFolder; }
            set
            {
                if (SetProperty(ref loadFromFolder, value))
                {
                    FolderWatcher?.Dispose();
                    if (!String.IsNullOrEmpty(SelectedFolder))
                    {
                        FolderWatcher = new FileSystemWatcher(SelectedFolder, "*.jpg");
                        FolderWatcher.IncludeSubdirectories = true;
                        FolderWatcher.EnableRaisingEvents = true;
                        FolderWatcher.Created += FolderWatcher_Created;

                    }
                }
            }
        }

        private void FolderWatcher_Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                Mat image = CvInvoke.Imread(e.FullPath, ImreadModes.Color);
                OnCaptureImage(image);
                pathName = e.FullPath;
            }
            catch
            {
                //
            }

        }

        public ICommand SelectFolderCommand
        {
            get { return new TargetCommand(SelectFolder); }
        }

        public string SelectedFolder
        {
            get { return selectedFolder; }
            set
            {
                if (SetProperty(ref selectedFolder, value))
                {
                    LoadFromFolder = false;
                    LoadFromFolder = true;
                }
            }
        }

        private void SelectFolder()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                SelectedFolder = Path.GetDirectoryName(ofd.FileName);
            }
        }

        public ICommand LoadCommand
        {
            get { return new TargetCommand(Load); }
        }

        private string pathName;
        private string selectedFolder;
        private bool loadFromFolder;
        private bool calculate;

        private void Load()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    Mat image = CvInvoke.Imread(ofd.FileName, ImreadModes.Color);
                    OnCaptureImage(image);
                    pathName = ofd.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public ICommand NextCommand
        {
            get { return new TargetCommand(Next); }
        }

        private void Next()
        {
            if (pathName == null)
            {
                return;
            }
            try
            {
                string folder = Path.GetDirectoryName(pathName);
                if (folder == null)
                {
                    return;
                }

                string name = Directory.EnumerateFiles(folder, "*.jpg")
                    .OrderBy(n => n)
                    .FirstOrDefault(n => String.CompareOrdinal(n, pathName) > 0);
                if (name == null)
                {
                    return;
                }
                pathName = name;

                Mat image = CvInvoke.Imread(name, ImreadModes.Color);
                OnCaptureImage(image);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public ICommand PreviousCommand
        {
            get { return new TargetCommand(Previous); }
        }

        private void Previous()
        {
            if (pathName == null)
            {
                return;
            }
            try
            {
                string folder = Path.GetDirectoryName(pathName);
                if (folder == null)
                {
                    return;
                }

                string name = Directory.EnumerateFiles(folder, "*.jpg")
                    .OrderByDescending(n => n)
                    .FirstOrDefault(n => String.CompareOrdinal(n, pathName) < 0);
                if (name == null)
                {
                    return;
                }
                pathName = name;

                Mat image = CvInvoke.Imread(name, ImreadModes.Color);
                OnCaptureImage(image);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public ICommand DeleteCommand
        {
            get { return new TargetCommand(Delete); }
        }

        private void Delete()
        {
            if (pathName == null)
            {
                return;
            }
            File.Delete(pathName);
            Next();
        }

        public ICommand OkCommand
        {
            get { return new TargetCommand(Ok); }
        }

        private void Ok()
        {
            if (pathName == null)
            {
                return;
            }
            string newName = Path.Combine(Path.GetDirectoryName(pathName), "Ok", Path.GetFileName(pathName));
            File.Move(pathName, newName);
            Next();
        }

        public ICommand NotOkCommand
        {
            get { return new TargetCommand(NotOk); }
        }

        private void NotOk()
        {
            if (pathName == null)
            {
                return;
            }
            string newName = Path.Combine(Path.GetDirectoryName(pathName), "..", Path.GetFileName(pathName));
            File.Move(pathName, newName);
            Next();
        }

        public event EventHandler<CaptureEvent> CaptureImage;

        protected void OnCaptureImage(Mat image)
        {
            ThreadDispatcher.Invoke(() =>
            {
                if (image.Height > image.Width)
                {
                    CvInvoke.ResizeForFrame(image, image, new System.Drawing.Size(500, 1000));
                }
                else
                {
                    CvInvoke.ResizeForFrame(image, image, new System.Drawing.Size(1000, 500));
                }

                CaptureImage?.Invoke(this, new CaptureEvent(image));
            });
        }


        public event EventHandler<CaptureEvent> StreamImage;
        protected void OnStreamImage(Mat image)
        {
            if (image.Height > image.Width)
            {
                CvInvoke.ResizeForFrame(image, image, new System.Drawing.Size(500, 1000));
            }
            else
            {
                CvInvoke.ResizeForFrame(image, image, new System.Drawing.Size(1000, 500));
            }
            StreamImage?.Invoke(this, new CaptureEvent(image));
        }


        public void Stop()
        {
            if (Camera != null)
            {
                Camera.Stop();

                Camera.ImageGrabbed -= Capturer_ImageGrabbed;
            }

        }

        public void Start()
        {
            if (Camera != null)
            {
                Camera.ImageGrabbed -= Capturer_ImageGrabbed;
                Camera.ImageGrabbed += Capturer_ImageGrabbed;

                Camera.Start();
            }
        }

        private void Capturer_ImageGrabbed(object sender, EventArgs e)
        {
            OnStreamImage(Camera.QueryFrame());
        }
    }

    public class CaptureEvent : EventArgs
    {
        public CaptureEvent(Mat image)
        {
            Image = image;
        }

        public Mat Image { get; }
    }

}
