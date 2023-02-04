using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Billiards.Base.FilterSets;
using Billiards.Base.Validations;
using Billiards.Wpf.UI;
using OpenCvSharp;

namespace Billiards.Wpf.ViewModels;

public class ValidationViewModel : ViewModel
{
    private ValidatedFolder? folder = new();
    private string? path;
    private ValidatedFile? selectedValidatedFile;

    public ValidationViewModel(VideoDeviceViewModel videoDevice)
    {
        VideoDevice = videoDevice;
    }

    public VideoDeviceViewModel VideoDevice { get; }

    public string? Path
    {
        get { return path; }
        set { SetProperty(ref path, value); }
    }

    public ValidatedFolder? Folder
    {
        get { return folder; }
        set { SetProperty(ref folder, value); }
    }

    public ICommand LoadValidationsCommand
    {
        get { return new TargetCommand(LoadValidations); }
    }

    private void LoadValidations()
    {
        FolderBrowserDialog ofd = new();
        if (ofd.ShowDialog() == DialogResult.OK)
        {
            Path = ofd.SelectedPath;

            string filename = System.IO.Path.Combine(Path, "ValidatedFiles.json");

            if (File.Exists(filename))
            {
                Folder = ValidatedFolder.Load(filename);
            }
            else
            {
                Folder = new ValidatedFolder();
            }
        }
    }

    public ICommand SaveValidationsCommand
    {
        get { return new TargetCommand(SaveValidations); }
    }

    private void SaveValidations()
    {
        if (Path != null
            && Folder != null)
        {
            foreach (ValidatedFile file in Folder.ValidatedFiles)
            {
                foreach (ValidatedBall ball in file.Balls)
                {
                    ball.X = ball.ResultX;
                    ball.Y = ball.ResultY;
                }
            }
            Folder.Save(System.IO.Path.Combine(Path, "ValidatedFiles.json"));
        }
    }

    public ICommand ValidateCommand
    {
        get { return new TargetCommand(Validate); }
    }

    public ValidatedFile? SelectedValidatedFile
    {
        get { return selectedValidatedFile; }
        set { SetProperty(ref selectedValidatedFile, value); }
    }

    private void Validate()
    {
        if (Path == null
            || Folder == null)
        {
            return;
        }
        ValidatedFolder? folder = Folder;
        Folder = null;
        CaramboleDetector detector = new(false);
        foreach (var file in Directory.EnumerateFiles(Path, "*.jpg"))
        {
            Mat image = Cv2.ImRead(file, ImreadModes.Color);
            var result = detector.ApplyFilters(image);

            string filename = System.IO.Path.GetFileName(file);

            ValidatedFile? validatedFile = folder.ValidatedFiles.FirstOrDefault(f => f.FileName == filename);
            if (validatedFile == null)
            {
                validatedFile = new ValidatedFile();
                folder.ValidatedFiles.Add(validatedFile);
                validatedFile.FileName = filename;
            }
            validatedFile.Found = result.Found;
            validatedFile.SetBalls(result.Balls);
        }
        Folder = folder;
        NotifyPropertyChanged(nameof(Folder));
    }

    public ICommand SaveImagesCommand
    {
        get { return new TargetCommand(SaveImages); }
    }

    private void SaveImages()
    {
        if (Path == null
            || Folder == null)
        {
            return;
        }
        CaramboleDetector detector = new(false);
        foreach (var file in Directory.EnumerateFiles(Path, "*.jpg"))
        {
            Mat image = Cv2.ImRead(file, ImreadModes.Color);
            var result = detector.ApplyFilters(image);

        }
    }
}