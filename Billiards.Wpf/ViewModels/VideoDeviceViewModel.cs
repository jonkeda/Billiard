using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows.Input;
using System.Xml.Linq;
using Billiards.Base.Filters;
using Billiards.Base.Threading;
using Billiards.Wpf.Devices;
using Billiards.Wpf.UI;
using OpenCvSharp;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Billiards.Wpf.ViewModels
{
    public class VideoDeviceViewModel : ViewModel
    {
        public VideoDeviceViewModel()
        {
            timer = new(new TimeSpan(0, 0, 0, 0, 1000 / 30));
            timer.Elapsed += TimerOnElapsed;
        }

        private VideoDevice selectedVideoDevice;
        private VideoCapture camera;

        public bool IsWhiteCueBall
        {
            get { return isWhiteCueBall; }
            set
            {
                if (SetProperty(ref isWhiteCueBall, value))
                {
                    NotifyPropertyChanged(nameof(IsYellowCueBall));
                }
            }
        }

        public bool IsYellowCueBall
        {
            get { return !isWhiteCueBall; }
            set
            {
                if (SetProperty(ref isWhiteCueBall, !value))
                {
                    NotifyPropertyChanged(nameof(IsWhiteCueBall));
                }

            }
        }

        public IReadOnlyList<VideoDevice> VideoDevices
        {
            get
            {
                using SystemDevices systemDevice = new();
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
                    camera = new(SelectedVideoDevice.Index, VideoCaptureAPIs.DSHOW );
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

            Mat image = Camera.RetrieveMat();
            if (image != null)
            {
                OnCaptureImage(image, null);
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
                        FolderWatcher = new(SelectedFolder, "*.jpg");
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
                Mat image = Cv2.ImRead(e.FullPath, ImreadModes.Color);
                OnCaptureImage(image, null);
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
            OpenFileDialog ofd = new();
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
            OpenFileDialog ofd = new();
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    Mat image = Cv2.ImRead(ofd.FileName, ImreadModes.Color);
                    OnCaptureImage(image, ofd.FileName);
                    pathName = ofd.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public ICommand CalculateCommand
        {
            get { return new TargetCommand(DoCalculate); }
        }

        private void DoCalculate()
        {
            if (pathName == null)
            {
                return;
            }
            Mat image = Cv2.ImRead(pathName, ImreadModes.Color);
            OnCaptureImage(image, null);
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

                Mat image = Cv2.ImRead(name, ImreadModes.Color);
                OnCaptureImage(image, name);
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

                Mat image = Cv2.ImRead(name, ImreadModes.Color);
                OnCaptureImage(image, name);
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

            Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(pathName), "Ok"));
            string newName = Path.Combine(Path.GetDirectoryName(pathName), "Ok", Path.GetFileName(pathName));
            File.Move(pathName, newName, true);
            Next();
        }

        public ICommand DoubleCommand
        {
            get { return new TargetCommand(Double); }
        }

        private void Double()
        {
            if (pathName == null)
            {
                return;
            }

            Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(pathName), "Double"));
            string newName = Path.Combine(Path.GetDirectoryName(pathName), "Double", Path.GetFileName(pathName));
            File.Move(pathName, newName, true);
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
            File.Move(pathName, newName, true);
            Next();
        }

        public event EventHandler<CaptureEvent> CaptureImage;

        private BallColor CueBall()
        {
            if (IsWhiteCueBall)
            {
                return BallColor.White;
            }

            return BallColor.Yellow;
        }

        protected void OnCaptureImage(Mat image, string? fileName)
        {
            ThreadDispatcher.Invoke(() =>
            {
                CaptureImage?.Invoke(this, new(image, CueBall(), fileName));
            });
        }


        public event EventHandler<CaptureEvent> StreamImage;
        protected void OnStreamImage(Mat image)
        {
            StreamImage?.Invoke(this, new(image, CueBall(), null));
        }


        private readonly Timer timer;
        private bool isWhiteCueBall;

        public void Stop()
        {
            timer.Stop();
        }

        public void Start()
        {
            timer.Start();
        }

        private void Capturer_ImageGrabbed(object sender, EventArgs e)
        {
        }

        private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
        {
            OnStreamImage(Camera.RetrieveMat());
        }


    }
}
