namespace Billiards.Wpf.Devices;

public class VideoDevice
{
    public VideoDevice(int index, string name)
    {
        Index = index;
        Name = name;
    }

    public int Index { get; }

    public string Name { get; }

    public override string ToString()
    {
        return Name;
    }
}