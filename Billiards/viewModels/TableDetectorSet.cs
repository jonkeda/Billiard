﻿using System.Linq;
using Billiard.Models;

namespace Billiard.viewModels;

public class TableDetectorSet : FilterSet
{
    public OriginalFilter OriginalFilter { get; private set; }
    public FloodFillFilter FloodFilter { get; private set; }

    public TableDetectorSet() : base("Find table")
    {
        var original = Original();
        var hsv = CvtColorBgr2Hsv();
        ExtractChannel(hsv, 0);
        var gaus = GaussianBlur();
        //var findPoint = Filters.AddFilter(new FindPointByColorFilter(gaus));
        //findPoint.Size = 20;
        //findPoint.Step = 10;

        MorphClose();
        MorphOpen();
        var flood = FloodFill(255);
        flood.MinimumArea = 14;
        flood.MaximumArea = 99;

        //flood.PointFilter = findPoint;
        var asMask = Mask();
        //DrawBoundingRect().BoundingRect = flood;

        OriginalFilter = original;
        FloodFilter = flood;
    }

    public AbstractFilter ResultFilter()
    {
        return Filters.LastOrDefault();
    }
}