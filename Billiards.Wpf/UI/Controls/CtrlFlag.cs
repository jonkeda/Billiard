namespace Billiards.Wpf.UI.Controls;

public enum CtrlFlag : byte
{
    AllowMain       = 0x0001,
    AllowFractions  = 0x0002,
    AllowNegatives  = 0x0004,
    AllowScientific = 0x0008,
    AllowNegSci     = 0x0010,
    LoadScientific  = 0x0020
}