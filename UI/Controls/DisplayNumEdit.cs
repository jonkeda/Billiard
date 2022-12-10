using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VouwwandImages.UI.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:NumEditCtrl"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:NumEditCtrl;assembly=NumEditCtrl"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:DisplayNumEdit/>
    /// This control is intended to be exclusively used with NumericEditCtrl, and therefore is not public,
    /// so as not to be visible outside the assembly.
    /// </summary>
    class DisplayNumEdit : Control
    {
        static class SystemNumberInfo
        {
            static private NumberFormatInfo nfi;

            static SystemNumberInfo()
            {
                CultureInfo ci = CultureInfo.CurrentCulture;
                nfi = ci.NumberFormat;
            }
            public static string DecimalSeparator
            {
                get { return nfi.NumberDecimalSeparator; }
            }
            public static string GroupSeparator
            {
                get { return nfi.NumberGroupSeparator; }
            }
            public static string NegativeSign
            {
                get { return nfi.NegativeSign; }
            }
            public static bool IsNegativePrefix
            {
                // for values, see: http://msdn.microsoft.com/en-us/library/system.globalization.numberformatinfo.numbernegativepattern.aspx
                // Assume if negative number format is (xxx), number is prefixed.
                get
                {
                    return nfi.NumberNegativePattern < 3;
                }
            }
            public static bool IsNegativeParentheses
            {
                get
                {
                    return nfi.NumberNegativePattern == 0;
                }
            }
            public static bool IsNegativeSpaceSeparated
            {
                get
                {
                    return nfi.NumberNegativePattern == 2 || nfi.NumberNegativePattern == 4;
                }
            }
        }
        private static readonly DependencyProperty TextProperty =
                DependencyProperty.Register("Text", typeof(string), typeof(DisplayNumEdit));

        private static readonly DependencyProperty TextAlignmentProperty =
                DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(DisplayNumEdit));

        private static readonly DependencyProperty DecimalSeparatorTypeProperty =
                DependencyProperty.Register("DecimalSeparatorType", typeof(DecimalSeparatorType), typeof(DisplayNumEdit));

        private static readonly DependencyProperty NegativeSignTypeProperty =
                DependencyProperty.Register("NegativeSignType", typeof(NegativeSignType), typeof(DisplayNumEdit));

        private static readonly DependencyProperty NegativeSignSideProperty =
                DependencyProperty.Register("NegativeSignSide", typeof(NegativeSignSide), typeof(DisplayNumEdit));

        private static readonly DependencyProperty NegativePatternTypeProperty =
                DependencyProperty.Register("NegativePatternType", typeof(NegativePatternType), typeof(DisplayNumEdit));

        private static readonly DependencyProperty GroupSeparatorTypeProperty =
                DependencyProperty.Register("GroupSeparatorType", typeof(GroupSeparatorType), typeof(DisplayNumEdit));

        private static readonly DependencyProperty GroupSizeProperty =
                DependencyProperty.Register("GroupSize", typeof(int), typeof(DisplayNumEdit));

        private static readonly DependencyProperty NegativeTextBrushProperty =
                DependencyProperty.Register("NegativeTextBrush", typeof(Brush), typeof(DisplayNumEdit));

        private static readonly DependencyProperty ScientificDisplayTypeProperty =
                DependencyProperty.Register("ScientificDisplayType", typeof(ScientificDisplayType), typeof(DisplayNumEdit), new PropertyMetadata(ScientificDisplayType.Pow10));
        
        private bool IsNegativeValue = false;

        static DisplayNumEdit()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DisplayNumEdit), new FrameworkPropertyMetadata(typeof(DisplayNumEdit)));
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public DecimalSeparatorType DecimalSeparatorType
        {
            get { return (DecimalSeparatorType)GetValue(DecimalSeparatorTypeProperty); }
            set { SetValue(DecimalSeparatorTypeProperty, value); }
        }

        public NegativeSignType NegativeSignType
        {
            get { return (NegativeSignType)GetValue(NegativeSignTypeProperty); }
            set { SetValue(NegativeSignTypeProperty, value); }
        }

        public NegativeSignSide NegativeSignSide
        {
            get { return (NegativeSignSide)GetValue(NegativeSignSideProperty); }
            set { SetValue(NegativeSignSideProperty, value); }
        }

        public NegativePatternType NegativePatternType
        {
            get { return (NegativePatternType)GetValue(NegativePatternTypeProperty); }
            set { SetValue(NegativePatternTypeProperty, value); }
        }

        public GroupSeparatorType GroupSeparatorType
        {
            get { return (GroupSeparatorType)GetValue(GroupSeparatorTypeProperty); }
            set { SetValue(GroupSeparatorTypeProperty, value); }
        }

        public int GroupSize
        {
            get { return (int)GetValue(GroupSizeProperty); }
            set { SetValue(GroupSizeProperty, value); }
        }

        public Brush NegativeTextBrush
        {
            get { return (Brush)GetValue(NegativeTextBrushProperty); }
            set { SetValue(NegativeTextBrushProperty, value); }
        }

        public ScientificDisplayType ScientificDisplayType
        {
            get { return (ScientificDisplayType)GetValue(ScientificDisplayTypeProperty); }
            set { SetValue(ScientificDisplayTypeProperty, value); }
        }

        public string GetDecimalSeparator()
        {
            switch (DecimalSeparatorType)
            {
                case DecimalSeparatorType.Point:
                    return ".";
                case DecimalSeparatorType.Comma:
                    return ",";
                case DecimalSeparatorType.System_Defined:
                default:
                    return SystemNumberInfo.DecimalSeparator;
            }
        }

        public string GetNegativeSign()
        {
            switch (NegativeSignType)
            {
                case NegativeSignType.Minus:
                    return "-";
                case NegativeSignType.System_Defined:
                default:
                    return SystemNumberInfo.NegativeSign;
            }
        }

        public bool IsNegativePrefix()
        {
            switch (NegativeSignSide)
            {
                case NegativeSignSide.Prefix:
                    return true;
                case NegativeSignSide.Suffix:
                    return false;
                case NegativeSignSide.System_Defined:
                default:
                    return SystemNumberInfo.IsNegativePrefix;
            }
        }

        private string GetNegativePrefix()
        {
            if (NegativePatternType == NegativePatternType.Parentheses ||
                (NegativePatternType == NegativePatternType.System_Defined && SystemNumberInfo.IsNegativeParentheses))
                return "(";

            if (IsNegativePrefix())
            {
                if (NegativePatternType == NegativePatternType.Symbol_Space ||
                    (NegativePatternType == NegativePatternType.System_Defined && SystemNumberInfo.IsNegativeSpaceSeparated))
                    return GetNegativeSign() + " ";
                else
                    return GetNegativeSign();
            }
            return "";
        }

        private string GetNegativeSuffix()
        {
            if (NegativePatternType == NegativePatternType.Parentheses ||
                (NegativePatternType == NegativePatternType.System_Defined && SystemNumberInfo.IsNegativeParentheses))
                return ")";

            if (!IsNegativePrefix())
            {
                if (NegativePatternType == NegativePatternType.Symbol_Space ||
                    (NegativePatternType == NegativePatternType.System_Defined && SystemNumberInfo.IsNegativeSpaceSeparated))
                    return " " + GetNegativeSign();
                else
                    return GetNegativeSign();
            }
            return "";
        }

        private string GetGroupSeparator()
        {
            switch (GroupSeparatorType)
            {
                case GroupSeparatorType.System_Defined:
                    return SystemNumberInfo.GroupSeparator;

                case GroupSeparatorType.Comma:
                    return ",";

                case GroupSeparatorType.Point:
                    return ".";

                case GroupSeparatorType.Space:
                case GroupSeparatorType.HalfSpace:
                default:
                    return " ";
            }   
        }

        public void GetWholeNumeratorDenominator(string value, out string Whole, out string Scientific, out string Numerator, out string Denominator, out string Sign)
        {
            int i = 0, j = 0;
            string[] str = new string[3];
            bool HasFraction = false;
            Scientific = "";
            bool IsScientific = false, HasScientific = false;

            Sign = (value.Length > 0 && ((IsNegativePrefix() && value[0] == GetNegativeSign()[0]) ||
                 (!IsNegativePrefix() && value[value.Length - 1] == GetNegativeSign()[0]))) ? GetNegativeSign() : "";

            if (Sign == GetNegativeSign())
                value = value.Trim(GetNegativeSign()[0]);
                       
            while (i != value.Length && j < 3)
            {
                if (value[i] == 'e' || value[i] == 'E')
                {
                    IsScientific = true;
                    HasScientific = true;
                }
                else if (value[i] == ' ' || value[i] == '/')
                {
                    j++;
                    IsScientific = false;
                }
                else if (IsScientific)
                    Scientific += value[i];
                else
                    str[j] += value[i];

                if (value[i] == '/')
                    HasFraction = true;

                i++;
            }
            if (j == 1) 
            {
                if (HasFraction) // case where no space: no whole. Only a numerator and denominator
                {
                    str[2] = (string.IsNullOrEmpty(str[1])) ? "1" : str[1];
                    str[1] = (string.IsNullOrEmpty(str[0])) ? "1" : str[0];
                    str[0] = "";
                }
                // case when user enters something like " 3456"
                // in this case, assume user meant to enter "3456" and not "3456/1"
                else if (string.IsNullOrEmpty(str[0]) && !string.IsNullOrEmpty(str[1]))
                {
                    str[0] = str[1];
                    str[1] = "";
                }
                else if (!string.IsNullOrEmpty(str[1])) // No fraction explicitly entered. assume it is /1
                {
                    str[2] = "1";
                }
            }
            if (j == 2)
            {
                if (string.IsNullOrEmpty(str[1]))
                    str[1] = "1";
                if (string.IsNullOrEmpty(str[2]))
                    str[2] = "1";
            }
            if (HasScientific && Scientific == "" && string.IsNullOrEmpty(str[0]))
                str[0] = "1"; // Assume e <=> e0 <=> 1

            Whole = str[0];
            Numerator = str[1];
            Denominator = str[2];
        }

        private Brush GetTextBrush()
        {
            // Tried default, Aero, Royale, Classic and Luna, and with these, always gets a default disabled text color
            // of #FF6D6D6D on my Windows 7 computer. What might happen on other machines?
            // I want to obtain a color garanteed correct on all machines for all themes.
            // Unfortunately, the closest I have got to this is SystemColors.InactiveCaptionTextBrush, which returns 
            // wrong color (slightly too dark).
            return IsEnabled ? ((IsNegativeValue && NegativeTextBrush != null)? NegativeTextBrush : Foreground) 
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6D6D6D"));
        }

        private FormattedText GetFormatedText(string strTxt, double dFontSize)
        {
            FormattedText formattedText = new FormattedText(
                strTxt,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface(FontFamily.ToString()),
                dFontSize,
                GetTextBrush());

            formattedText.SetFontWeight(FontWeight);
            formattedText.SetFontStretch(FontStretch);
            formattedText.SetFontStyle(FontStyle);
            formattedText.SetFontFamily(FontFamily);

            return formattedText;
        }

        private string FormatNumber(string strNumber)
        {
            if (strNumber != null)
            {
                int EndIndex = strNumber.IndexOf(GetDecimalSeparator()[0]);

                if (EndIndex == -1)
                    EndIndex = strNumber.Length;

                int Index = EndIndex - 1;
                int counter = -1;

                while (Index >= 0 && strNumber[Index] >= '0' && strNumber[Index] <= '9')
                {
                    counter++;

                    if (counter == GroupSize)
                    {
                        counter = 0;
                        strNumber = strNumber.Insert(Index + 1, GetGroupSeparator());
                    }
                    Index--;
                }
            }
            return strNumber;
        }

        private FormattedText GetMainFormattedText(string strMain)
        {
            FormattedText formattedText = GetFormatedText(strMain, FontSize);

            for (int i = 0; i < strMain.Length; i++)
            {
                if (strMain[i] == ' ' && GroupSeparatorType == GroupSeparatorType.HalfSpace)
                    formattedText.SetFontSize(FontSize / 2.0, i, 1);
            }
            return formattedText;
        }

        private double GetSumFormattedTextWidth(params FormattedText[] FormTxtArray)
        {
            double sumWidth = 0.0;

            foreach (FormattedText FormTxt in FormTxtArray)
            {
                if (FormTxt != null)
                    sumWidth += FormTxt.WidthIncludingTrailingWhitespace;
            }
            return sumWidth;
        }

        private double DrawText(DrawingContext drawingContext, FormattedText formatTxt, Point StartPt)
        {
            drawingContext.DrawText(formatTxt, StartPt);
            return formatTxt.WidthIncludingTrailingWhitespace;
        }
  
        private void SetTextBoundary(DrawingContext drawingContext, FormattedText formatTxt,  double RemWidth, ref bool spaceAvailable)
        {
            if (formatTxt.WidthIncludingTrailingWhitespace >= RemWidth)
            {
                // Set a maximum width and height. If the text overflows these values, an ellipsis "..." appears.
                // Useful, so aware of any non displayed numbers.
                formatTxt.MaxTextWidth = RemWidth;
                formatTxt.MaxTextHeight = ActualHeight;
                formatTxt.Trimming = TextTrimming.CharacterEllipsis;
                spaceAvailable = false;
            }
        }

        private Point DrawScientific(DrawingContext drawingContext, Point InitOffset, FormattedText formatScientific)
        {
            switch (ScientificDisplayType)
            {
                case ScientificDisplayType.CapitalE:
                case ScientificDisplayType.SmallE:
                    InitOffset = new Point(InitOffset.X + DrawText(drawingContext, formatScientific, InitOffset), InitOffset.Y);                     
                break;

                case ScientificDisplayType.Pow10:
                default:
                    InitOffset = new Point(InitOffset.X + DrawText(drawingContext, formatScientific, new Point(InitOffset.X, InitOffset.Y - (FontSize / 12.0))), InitOffset.Y);
                break;
            }
            return InitOffset;
        }

        private Point DrawFraction(DrawingContext drawingContext, Point InitOffset, FormattedText formatNumerator,
                                                        FormattedText formatDenominator, FormattedText formatSlash, double SlashExtraWidth)
        {
            double NumVOffset = -(FontSize / 12.0), DenomVOffset = (FontSize / 3.0);
            InitOffset = new Point(InitOffset.X + DrawText(drawingContext, formatNumerator, new Point(InitOffset.X, InitOffset.Y + NumVOffset)), InitOffset.Y);
            InitOffset = new Point(InitOffset.X + DrawText(drawingContext, formatSlash, InitOffset) + SlashExtraWidth, InitOffset.Y);
            InitOffset = new Point(InitOffset.X + DrawText(drawingContext, formatDenominator, new Point(InitOffset.X, InitOffset.Y + DenomVOffset)), InitOffset.Y);
            return InitOffset;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect rect = new Rect(0.0, 0.0, ActualWidth, ActualHeight);

            // I have been unable to obtain the system disabled background color.
            // The closest I have got to is with: SystemColors.InactiveCaptionBrush.InactiveCaptionTextBrush
            // Value returned is not accurate.
            // Fortunately, a trick is not to paint the background at all as the disabled TextBox behind has
            // correct color in all circumstances.
            // drawingContext.DrawRectangle(IsEnabled ? Background :
            // new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF4F4F4")), null, rect);
        
            RectangleGeometry Bound = new RectangleGeometry(rect);
            drawingContext.PushClip(Bound);

            if (Text == null)
                return;

            string Whole, Scientific, Numerator, Denominator, Sign;
            FormattedText formatMain = null, formatPrefixSign = null, formatSuffixSign = null, formatPreScien = null, formatScientific = null,
                                                       formatNumerator = null, formatDenominator = null, formatSlash = null;
                       
            Point InitOffset = new Point(4.0, 2.0);

            if (VerticalContentAlignment == VerticalAlignment.Center)
                InitOffset = new Point(InitOffset.X, (ActualHeight - GetFormatedText("T", FontSize).Height)/2.0);
            else if (VerticalContentAlignment == VerticalAlignment.Bottom)
                InitOffset = new Point(InitOffset.X, ActualHeight - InitOffset.Y - GetFormatedText("T", FontSize).Height);
             
            double TotalWidth, MainFracOffset = FontSize / 3.0, // MainFracOffset is distance separating main number from start of fraction
                    SlashExtraWidth = FontSize / 10.0;          // Extra width for slash for better look
            GetWholeNumeratorDenominator(Text, out Whole, out Scientific, out Numerator, out Denominator, out Sign);

            if (Sign != null && Sign != "")    
            {
                IsNegativeValue = true;
                string strNegPref = GetNegativePrefix(), strNegSuf = GetNegativeSuffix();

                if (strNegPref != "")
                    formatPrefixSign = GetFormatedText(strNegPref, FontSize);

                if (strNegSuf != "")
                    formatSuffixSign = GetFormatedText(strNegSuf, FontSize);
            }
            else
                IsNegativeValue = false;

            // Create the initial formatted text string.
            if (!string.IsNullOrEmpty(Whole))
            {
                Whole = FormatNumber(Whole);
                formatMain = GetMainFormattedText(Whole);
            }

            if (!string.IsNullOrEmpty(Scientific))
            {
                string strScienSym;

                switch (ScientificDisplayType)
                {
                    case ScientificDisplayType.CapitalE:
                        strScienSym = "E";
                        formatScientific = GetFormatedText(Scientific, 0.9 * FontSize);
                        break;

                    case ScientificDisplayType.SmallE:
                        strScienSym = "e";
                        formatScientific = GetFormatedText(Scientific, 0.9 * FontSize);
                        break;

                    case ScientificDisplayType.Pow10:
                    default:
                        strScienSym = (formatMain == null)? "10" : "x10";
                        formatScientific = GetFormatedText(Scientific, 0.7 * FontSize);
                        break;
                }
                formatPreScien = GetFormatedText(strScienSym, FontSize);
            }

            if (!string.IsNullOrEmpty(Numerator) && !string.IsNullOrEmpty(Denominator))
            {
                formatNumerator = GetFormatedText(Numerator, FontSize / 1.3);
                formatDenominator = GetFormatedText(Denominator, FontSize / 1.3);

                string slash = "";
                slash += (char)164;
                formatSlash = new FormattedText(slash,
                                                CultureInfo.GetCultureInfo("en-us"),
                                                FlowDirection.LeftToRight,
                                                new Typeface("Symbol"),
                                                FontSize,
                                                GetTextBrush());

                formatSlash.SetFontWeight(FontWeight);
                formatSlash.SetFontStretch(FontStretch);
                formatSlash.SetFontStyle(FontStyle);
            }

            TotalWidth = GetSumFormattedTextWidth(formatMain, formatPreScien, formatScientific, formatPrefixSign, formatSuffixSign, 
                                                                                formatNumerator, formatDenominator, formatSlash);

            if (formatMain != null && formatNumerator != null && formatDenominator != null)
                TotalWidth += MainFracOffset + SlashExtraWidth;

            if (TotalWidth + 2*InitOffset.X < ActualWidth)
            {
                if (TextAlignment == TextAlignment.Right)
                    InitOffset = new Point(ActualWidth - TotalWidth - InitOffset.X, InitOffset.Y);
                else if (TextAlignment == TextAlignment.Center)
                    InitOffset = new Point((ActualWidth - TotalWidth)/2.0, InitOffset.Y);

                if (formatPrefixSign != null)
                    InitOffset = new Point(InitOffset.X + DrawText(drawingContext, formatPrefixSign, InitOffset), InitOffset.Y);

                if (formatMain != null)
                    InitOffset = new Point(InitOffset.X + DrawText(drawingContext, formatMain, InitOffset), InitOffset.Y);

                if (formatPreScien != null && formatScientific != null)
                {
                    InitOffset = new Point(InitOffset.X + DrawText(drawingContext, formatPreScien, InitOffset), InitOffset.Y);
                    InitOffset = DrawScientific(drawingContext, InitOffset, formatScientific);
                }
                
                if (formatNumerator != null && formatSlash != null && formatDenominator != null)
                {
                    if (formatMain != null)
                        InitOffset = new Point(InitOffset.X + MainFracOffset, InitOffset.Y);

                    InitOffset = DrawFraction(drawingContext, InitOffset, formatNumerator, formatDenominator, formatSlash, SlashExtraWidth);
                }
                if (formatSuffixSign != null)
                    DrawText(drawingContext, formatSuffixSign, InitOffset);   
            }
            else
            {
                double RemWidth = ActualWidth - 2 * InitOffset.X, offset = 0.0;
                bool spaceAvailable = true;

                if (formatPrefixSign != null && formatPrefixSign.WidthIncludingTrailingWhitespace < RemWidth)
                {
                    RemWidth -= DrawText(drawingContext, formatPrefixSign, InitOffset);
                    offset += formatPrefixSign.WidthIncludingTrailingWhitespace;
                }

                if (formatSuffixSign != null && formatSuffixSign.WidthIncludingTrailingWhitespace < RemWidth)
                    RemWidth -= DrawText(drawingContext, formatSuffixSign, new Point(ActualWidth - formatSuffixSign.WidthIncludingTrailingWhitespace - InitOffset.X, InitOffset.Y));

                if (formatMain != null)
                {
                    SetTextBoundary(drawingContext, formatMain, RemWidth, ref spaceAvailable);
                    RemWidth -= DrawText(drawingContext, formatMain, new Point(InitOffset.X + offset, InitOffset.Y));
                    offset += formatMain.Width;
                }
                if (spaceAvailable && formatPreScien != null && formatScientific != null)
                {
                    SetTextBoundary(drawingContext, formatPreScien, RemWidth, ref spaceAvailable);
                    RemWidth -= DrawText(drawingContext, formatPreScien, new Point(InitOffset.X + offset, InitOffset.Y));
                    offset += formatPreScien.Width;

                    if (spaceAvailable)
                    {
                        SetTextBoundary(drawingContext, formatScientific, RemWidth, ref spaceAvailable);
                        DrawScientific(drawingContext, new Point(InitOffset.X + offset, InitOffset.Y), formatScientific);
                        offset += formatScientific.Width;
                        RemWidth -= formatScientific.Width;
                    }
                }
                if (spaceAvailable && formatNumerator != null && formatDenominator != null )
                {
                    InitOffset = new Point(InitOffset.X + offset, InitOffset.Y);

                    if (FontSize >= RemWidth)
                    {
                        // Case nearly reached end anyway - no way we could reasonably display any part of fraction.
                        // Just add '...' at end. 
                        FormattedText Ellipsis = GetFormatedText("...", FontSize);
                        drawingContext.DrawText(Ellipsis, InitOffset);
                    }
                    else
                    {
                        if (formatMain != null)
                        {
                            InitOffset = new Point(InitOffset.X + MainFracOffset, InitOffset.Y);
                            RemWidth -= MainFracOffset;
                        }

                        RemWidth = RemWidth - formatSlash.Width - SlashExtraWidth;

                        if (formatNumerator.Width >= RemWidth / 2.0 && formatDenominator.Width >= RemWidth / 2.0)
                        {
                            formatNumerator.MaxTextWidth = RemWidth / 2.0;
                            formatDenominator.MaxTextWidth = RemWidth / 2.0;
                        }
                        else if (formatNumerator.Width > formatDenominator.Width
                            && (formatNumerator.Width >= (RemWidth - formatDenominator.Width)))
                            formatNumerator.MaxTextWidth = RemWidth - formatDenominator.Width;
                        else if (formatDenominator.Width > formatNumerator.Width
                            && (formatDenominator.Width >= (RemWidth - formatNumerator.Width)))
                            formatDenominator.MaxTextWidth = RemWidth - formatNumerator.Width;

                        DrawFraction(drawingContext, InitOffset, formatNumerator, formatDenominator, formatSlash, SlashExtraWidth);             
                    }
                }
            }
        }
    }
}
