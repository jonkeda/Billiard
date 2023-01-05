﻿using Billiard.Models;

namespace Billiard.viewModels;

public class HsvChannelsFilterSet : FilterSet
{
    public HsvChannelsFilterSet() : base("HSV Channels")
    {
        Original();
        var hsv = CvtColorBgr2Hsv();
        ExtractChannel(hsv, 0);
        ExtractChannel(hsv, 1);
        ExtractChannel(hsv, 2);
    }
}