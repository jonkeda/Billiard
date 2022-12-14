using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Billiard.UI.Controls
{
    public enum CtrlFlag : byte
    {
        AllowMain       = 0x0001,
        AllowFractions  = 0x0002,
        AllowNegatives  = 0x0004,
        AllowScientific = 0x0008,
        AllowNegSci     = 0x0010,
        LoadScientific  = 0x0020
    }

    public enum DecimalSeparatorType : byte 
    {
        System_Defined,
        Point,
        Comma      
    }

    public enum NegativeSignType : byte
    {
        System_Defined,
        Minus        
    }

    public enum NegativeSignSide : byte
    {
        System_Defined,
        Prefix,
        Suffix      
    }

    public enum NegativePatternType : byte
    {
        System_Defined,
        Symbol_NoSpace,
        Symbol_Space,
        Parentheses
    }

    public enum GroupSeparatorType : byte
    {
        System_Defined,       
        Comma,
        Point,
        Space,
        HalfSpace
    }

    public enum ScientificDisplayType : byte
    {
        CapitalE,
        SmallE,
        Pow10,
        ExpandOnExit
    }

    public struct NumberValue
    {
        public decimal Main;
        public long Numerator, Denominator;
    }

    /// <summary>
    /// Interaction logic for NumericEditCtrl.xaml
    /// </summary>
    public partial class NumberBox : UserControl
    {
        static NumberBox()
        {
            try
            {
                FrameworkPropertyMetadata BorderThicknessMetaData = new FrameworkPropertyMetadata();
                BorderThicknessMetaData.CoerceValueCallback = new CoerceValueCallback(CoerceBorderThickness);
                BorderThicknessProperty.OverrideMetadata(typeof(NumberBox), BorderThicknessMetaData);

                // For Background, do not do in XAML part something like:
                // Background="{Binding Background, ElementName=Root}" in FramePlaceHolder settings.
                // Reason: although this will indeed set the Background values as expected, problems arise when user
                // of control does not explicitly not set a value.
                // In this case, Background of FramePlaceHolder get defaulted to values in UserControl, which is null
                // and not what we want.
                // We want to keep the default values of a standard TextBox, which may differ according to themes.
                // Have to treat similarly as with BorderThickness...

                FrameworkPropertyMetadata BackgroundMetaData = new FrameworkPropertyMetadata();
                BackgroundMetaData.CoerceValueCallback = new CoerceValueCallback(CoerceBackground);
                BackgroundProperty.OverrideMetadata(typeof(NumberBox), BackgroundMetaData);
            }
            catch (Exception)
            {
            }
        }

        private static object CoerceBorderThickness(DependencyObject d, object value)
        {
            NumberBox NumEditCtrl = d as NumberBox;
           
            if (NumEditCtrl != null && value is Thickness)
            {
                Thickness NewBorderThickNess = (Thickness)value;
                NumEditCtrl.NumTextBox.BorderThickness = NewBorderThickNess;
            }
            return new Thickness(0.0);
        }

        private static object CoerceBackground(DependencyObject d, object value)
        {
            NumberBox NumEditCtrl = d as NumberBox;

            if (NumEditCtrl != null && value is Brush)
                NumEditCtrl.FramePlaceHolder.Background = (Brush)value;

            return value;
        }

        private static readonly DependencyProperty TextAlignmentProperty =
                DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(NumberBox), new PropertyMetadata(TextAlignment.Right));
        
        private static readonly DependencyProperty CtrlFlagsProperty =
                DependencyProperty.Register("CtrlFlags", typeof(byte), typeof(NumberBox), new PropertyMetadata((byte)((byte)CtrlFlag.AllowMain | (byte)CtrlFlag.AllowNegatives)));

        private static readonly DependencyProperty DecimalPlacesProperty =
                DependencyProperty.Register("DecimalPlaces", typeof(short), typeof(NumberBox), new PropertyMetadata((short)2));

        private static readonly DependencyProperty DecimalSeparatorTypeProperty =
                DependencyProperty.Register("DecimalSeparatorType", typeof(DecimalSeparatorType), typeof(NumberBox), new PropertyMetadata(DecimalSeparatorType.System_Defined));

        private static readonly DependencyProperty NegativeSignTypeProperty =
                DependencyProperty.Register("NegativeSignType", typeof(NegativeSignType), typeof(NumberBox), new PropertyMetadata(NegativeSignType.System_Defined));

        private static readonly DependencyProperty NegativeSignSideProperty =
                DependencyProperty.Register("NegativeSignSide", typeof(NegativeSignSide), typeof(NumberBox), new PropertyMetadata(NegativeSignSide.System_Defined));

        private static readonly DependencyProperty NegativePatternTypeProperty =
                DependencyProperty.Register("NegativePatternType", typeof(NegativePatternType), typeof(NumberBox), new PropertyMetadata(NegativePatternType.System_Defined));

        private static readonly DependencyProperty GroupSeparatorTypeProperty =
                DependencyProperty.Register("GroupSeparatorType", typeof(GroupSeparatorType), typeof(NumberBox), new PropertyMetadata(GroupSeparatorType.HalfSpace));

        private static readonly DependencyProperty GroupSizeProperty =
                DependencyProperty.Register("GroupSize", typeof(short), typeof(NumberBox), new PropertyMetadata((short)3));

        private static readonly DependencyProperty NegativeTextBrushProperty =
                DependencyProperty.Register("NegativeTextBrush", typeof(Brush), typeof(NumberBox));
        
        private static readonly DependencyProperty ScientificDisplayTypeProperty =
                DependencyProperty.Register("ScientificDisplayType", typeof(ScientificDisplayType), typeof(NumberBox), new PropertyMetadata(ScientificDisplayType.ExpandOnExit));

        private static readonly DependencyProperty DecValueProperty =
                DependencyProperty.Register("DecValue", typeof(decimal?), typeof(NumberBox), new FrameworkPropertyMetadata(DecValueChangedCallback) { BindsTwoWayByDefault = true });

        private static readonly DependencyProperty DoubleValueProperty =
            DependencyProperty.Register("DoubleValue", typeof(double?), typeof(NumberBox), new FrameworkPropertyMetadata(double.MinValue, DoubleValueChangedCallback) { BindsTwoWayByDefault = true});

        private static readonly DependencyProperty NumValueProperty =
                DependencyProperty.Register("NumValue", typeof(NumberValue?), typeof(NumberBox), new FrameworkPropertyMetadata(NumValueChangedCallback) { BindsTwoWayByDefault = true });

        private static readonly DependencyProperty SignificantDigitsProperty =
                DependencyProperty.Register("SignificantDigits", typeof(byte), typeof(NumberBox), new PropertyMetadata((byte)5));

        private static void DecValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox NumBox = d as NumberBox;

            if (NumBox == null || !(e.NewValue is decimal?) || NumBox.InternalUpdateValues)
                return;

            if (e.NewValue == null)
                NumBox.NumTextBox.Text = "";
            else
            {
                decimal value = e.NewValue as decimal? ?? 0;

                bool IsNegative = (value < 0);

                string str = NumBox.GetMainFromDecimalExt(value);

                if (IsNegative)
                    str = NumBox.Display.IsNegativePrefix() ? NumBox.Display.GetNegativeSign() + str : str + NumBox.Display.GetNegativeSign();

                if (NumBox.IsTextBoxVisible)
                    NumBox.NumTextBox.Foreground = ((IsNegative && NumBox.NegativeTextBrush != null) ? NumBox.NegativeTextBrush : NumBox.Foreground);

                NumBox.NumTextBox.Text = str;
            }
            NumBox.Display.InvalidateVisual();
        }

        private static void DoubleValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox NumBox = d as NumberBox;

            if (NumBox == null || !(e.NewValue is double?) || NumBox.InternalUpdateValues)
                return;
       
            if (e.NewValue == null)
                NumBox.NumTextBox.Text = "";
            else
            {
                double value = e.NewValue as double? ?? 0;

                bool IsNegative = (value < 0);

                string str = NumBox.GetMainFromDecimalExt((decimal)value);

                if (IsNegative)
                    str = NumBox.Display.IsNegativePrefix() ? NumBox.Display.GetNegativeSign() + str : str + NumBox.Display.GetNegativeSign();

                if (NumBox.IsTextBoxVisible)
                    NumBox.NumTextBox.Foreground = ((IsNegative && NumBox.NegativeTextBrush != null) ? NumBox.NegativeTextBrush : NumBox.Foreground);

                NumBox.NumTextBox.Text = str;
            }
            NumBox.Display.InvalidateVisual();
        }

        private static void NumValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox NumBox = d as NumberBox;

            if (NumBox == null || !(e.NewValue is NumberValue?) || NumBox.InternalUpdateValues)
                return;

            if (e.NewValue == null)
                NumBox.NumTextBox.Text = "";
            else
            {
                NumberValue value = (NumberValue?)e.NewValue ?? new NumberValue();

                bool IsNegative = (value.Main < 0 || (value.Main == 0.0m && value.Numerator < 0));

                string str = NumBox.GetMainFromDecimalExt(value.Main);

                if (value.Numerator != 0 && value.Denominator != 0 && (NumBox.CtrlFlags & (short)CtrlFlag.AllowFractions) != 0)
                {
                    string Num, Denom;

                    if (value.Numerator < 0)
                        value.Numerator = -value.Numerator;

                    if (value.Denominator < 0)
                        value.Denominator = -value.Denominator;

                    Num = string.Format("{0:G}", value.Numerator);
                    Denom = string.Format("{0:G}", value.Denominator);

                    str = str + " " + Num + "/" + Denom;
                }

                if (IsNegative)
                    str = NumBox.Display.IsNegativePrefix() ? NumBox.Display.GetNegativeSign() + str : str + NumBox.Display.GetNegativeSign();

                if (NumBox.IsTextBoxVisible)
                    NumBox.NumTextBox.Foreground = ((IsNegative && NumBox.NegativeTextBrush != null) ? NumBox.NegativeTextBrush : NumBox.Foreground);

                NumBox.NumTextBox.Text = str;
                NumBox.Display.InvalidateVisual();
            }
        }

        private bool IsTextBoxVisible = false;
        private bool InternalUpdateValues = false;

        public NumberBox()
        {
            InitializeComponent();
        }

        public decimal? DecValue
        {
            get { return (decimal?)GetValue(DecValueProperty); }
            set { SetValue(DecValueProperty, value); }
        }

        public double? DoubleValue
        {
            get { return (double?)GetValue(DoubleValueProperty); }
            set { SetValue(DoubleValueProperty, value); }
        }

        public NumberValue? NumValue
        {
            get { return (NumberValue?)GetValue(NumValueProperty); }
            set { SetValue(NumValueProperty, value); }
        }

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set 
            { 
                SetValue(TextAlignmentProperty, value);
                Display.InvalidateVisual(); // Does not get executed in XAML, which is fine here. Only required in code.
            }
        }

        public byte CtrlFlags
        {
            get { return (byte)GetValue(CtrlFlagsProperty); }
            set { SetValue(CtrlFlagsProperty, value); }
        }

        public short DecimalPlaces
        {
            get { return (short)GetValue(DecimalPlacesProperty); }
            set { SetValue(DecimalPlacesProperty, value); }
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

        public short GroupSize
        {
            get { return (short)GetValue(GroupSizeProperty); }
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

        public byte SignificantDigits
        {
            get { return (byte)GetValue(SignificantDigitsProperty); }
            set { SetValue(SignificantDigitsProperty, value); }
        }
        
        private decimal? GetMainFromString(string Main, string Scientific)
        {
            decimal Res = 0.0m;
            CultureInfo info = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            NumberFormatInfo nfi = info.NumberFormat;
            nfi.NumberDecimalSeparator = Display.GetDecimalSeparator();

            if (string.IsNullOrEmpty(Main) && string.IsNullOrEmpty(Scientific))
                return null;

            if (Main != null && Main.Length > 0)
                decimal.TryParse(Main, NumberStyles.AllowDecimalPoint, info, out Res);

            if (Scientific != null && Scientific.Length > 0)
            {
                try
                {
                    long Pow;
                    decimal temp = (Res == 0.0m)? 1.0m : Res;
                    Pow = long.Parse(Scientific);
                    temp = temp * (decimal)Math.Pow(10.0, (double)Pow);
                    Res = temp;
                }
                catch (Exception)
                {
                    // Do nothing. If an exception is thrown, Res is not assigned to temp, and the value in e
                    // is ignored altogether, which is intention.
                    // Chances are that an exception is thrown because value of e is too large (easily done).
                }
            }
            return Res;
        }

        private string GetMainFromDecimal(decimal value, short decplaces = -1)
        {
            if (value < 0)
                value = -value;

            string str = "{0:F" + Convert.ToString((decplaces == -1) ? DecimalPlaces : decplaces) + "}";

            CultureInfo info = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            NumberFormatInfo nfi = info.NumberFormat;
            nfi.NumberDecimalSeparator = Display.GetDecimalSeparator();

            return string.Format(info, str, value);
        }

        private string GetMainFromDecimalExt(decimal value)
        {
            if ((CtrlFlags & (short)CtrlFlag.LoadScientific) != 0 && (CtrlFlags & (short)CtrlFlag.AllowScientific) != 0)
            {
                byte DecPlaces = (byte)((DecimalPlaces < 0) ? SignificantDigits - 1 : DecimalPlaces);
                short Pow = 0;

                if (value < 0)
                    value = -value;

                while (value > 10)
                {
                    Pow++;
                    value /= 10;
                }
                if ((CtrlFlags & (short)CtrlFlag.AllowNegSci) != 0)
                {
                    while (value < 1)
                    {
                       Pow--;
                       value *= 10;
                    }
                }
                CultureInfo info = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                NumberFormatInfo nfi = info.NumberFormat;
                nfi.NumberDecimalSeparator = Display.GetDecimalSeparator();

                string str = "{0:F" + DecPlaces.ToString() + "}";
                str += (ScientificDisplayType == ScientificDisplayType.CapitalE)? "E" : "e";
                str += "{1:D}";

                return string.Format(info, str, value, Pow);
            }
            if (DecimalPlaces < 0)
            {
                short decplaces = 0;

                if (value != 0.0m)
                {
                    decimal NewVal = value;
                    int IntVal = (int)value;

                    while (IntVal == 0)
                    {
                        NewVal *= 10;
                        IntVal = unchecked((int)NewVal); // If lose data because NewVal is too large, don't care, as long as IntVal is not 0
                        decplaces++;
                    }
                    byte count = 1;
                    // check if next 4 decimals are 0:
                    while (count < SignificantDigits)
                    {
                        NewVal -= decimal.Floor(NewVal);
                        NewVal *= 10;
                        IntVal = unchecked((int)NewVal); // If lose data because NewVal is too large, don't care, as long as IntVal is not 0

                        if (IntVal == 0)
                            break;
                            
                        decplaces++;
                        count++;
                    }
                }
                return GetMainFromDecimal(value, decplaces);
            }
            else
                return GetMainFromDecimal(value);
        }

        private void UpdateDependencyValues()
        {
            InternalUpdateValues = true;
            string Main, Scientific, Numerator, Denominator, Sign;
            Display.GetWholeNumeratorDenominator(NumTextBox.Text, out Main, out Scientific, out Numerator, out Denominator, out Sign);

            decimal? DecVal = GetMainFromString(Main, Scientific);

            if (DecVal == null && string.IsNullOrEmpty(Numerator) && string.IsNullOrEmpty(Denominator))
                NumValue = null;
            else
            {
                NumberValue NewNumValue;
                NewNumValue.Main = DecVal ?? 0.0M;
                long.TryParse(Numerator, out NewNumValue.Numerator);
                long.TryParse(Denominator, out NewNumValue.Denominator);

                if (!string.IsNullOrEmpty(Sign))
                {
                    NewNumValue.Main = -NewNumValue.Main;
                    NewNumValue.Numerator = -NewNumValue.Numerator;
                }
                NumValue = NewNumValue;  
            }

            if (!string.IsNullOrEmpty(Sign) && DecVal != null)
                DecVal = -DecVal;

            DecValue = DecVal;
            DoubleValue = (double?) DecVal;
            InternalUpdateValues = false;
        }

        private void NumTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDependencyValues();
        }

        private void HideTextBox()
        {
            IsTextBoxVisible = false;
            NumTextBox.Background = Brushes.Transparent;
            NumTextBox.Foreground = Brushes.Transparent;
        }

        private void ShowTextBox()
        {
            IsTextBoxVisible = true;
            NumTextBox.Background = FramePlaceHolder.Background;
            bool IsNegativeValue = (NumTextBox.Text.Length > 0 && ((Display.IsNegativePrefix() && NumTextBox.Text[0] == Display.GetNegativeSign()[0]) ||
                                           (!Display.IsNegativePrefix() && NumTextBox.Text[NumTextBox.Text.Length - 1] == Display.GetNegativeSign()[0])));  
            NumTextBox.Foreground = ((IsNegativeValue && NegativeTextBrush != null) ? NegativeTextBrush : Foreground);
        }

        private void Root_GotFocus(object sender, RoutedEventArgs e)
        {
            ShowTextBox();
            NumTextBox.Focus();
        }

        private void Root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsEnabled)
            {
                ShowTextBox();
                NumTextBox.Focus();
                NumTextBox.SelectionStart = NumTextBox.GetCharacterIndexFromPoint(e.GetPosition(this), true);
            }
        }

        private void NumTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string Whole, Scientific, Numerator, Denominator, Sign;
            Display.GetWholeNumeratorDenominator(NumTextBox.Text, out Whole, out Scientific, out Numerator, out Denominator, out Sign);
            int CurPos = NumTextBox.SelectionStart;

            char chrEore = NumTextBox.Text.FirstOrDefault(delegate(char ch) { return ch == 'E' || ch == 'e'; });

            NumTextBox.Text = (Display.IsNegativePrefix())? Sign : "";

            if ((CtrlFlags & (short)CtrlFlag.AllowMain) != 0)
            {
                bool bFill = false;

                if (Whole != null && Whole.Length > 0)
                {
                    if ( ScientificDisplayType != ScientificDisplayType.ExpandOnExit || Scientific == null || Scientific.Length == 0)
                        NumTextBox.Text += Whole;
                    bFill = true;
                }
                if ((CtrlFlags & (short)CtrlFlag.AllowScientific) != 0 && Scientific != null && Scientific.Length > 0)
                {
                    if (ScientificDisplayType != ScientificDisplayType.ExpandOnExit)
                        NumTextBox.Text += chrEore + Scientific;
                    bFill = true;
                }

                if (bFill && ScientificDisplayType == ScientificDisplayType.ExpandOnExit && Scientific != null && Scientific.Length > 0)
                {
                    decimal Main = GetMainFromString(Whole, Scientific) ?? 0.0M;
                    if (Main > 0.0m)
                    {
                        if (DecimalPlaces < 0)
                        {
                            short sci;
                            if (short.TryParse(Scientific, out sci))
                            {
                                short decplaces = (short)-sci;  

                                if (Whole != null && Whole.Length > 0)
                                {
                                    int DecPos = Whole.IndexOf(Display.GetDecimalSeparator()[0]);

                                    if (DecPos != -1)
                                        decplaces += (short)(Whole.Length - DecPos - 1);
                                }
                                if (decplaces < 0)
                                    decplaces = 0;

                                NumTextBox.Text += GetMainFromDecimal(Main, decplaces);
                            }
                        }
                        else
                            NumTextBox.Text += GetMainFromDecimal(Main);
                    }
                }

                if ((Numerator != null || Denominator != null) && bFill)
                    NumTextBox.Text += " ";
            }
            if ((CtrlFlags & (short)CtrlFlag.AllowFractions) != 0)
            {
                if ((CtrlFlags & (short)CtrlFlag.AllowMain) == 0 && Whole != null && Whole.Length > 0)
                {
                    Numerator = Whole;
                    Denominator = "1";
                }
                if (Numerator != null && Denominator != null)
                    NumTextBox.Text += Numerator + "/" + Denominator;
            }

            if (!Display.IsNegativePrefix())
                NumTextBox.Text += Sign;

            NumTextBox.SelectionStart = CurPos;

            HideTextBox();
            Display.InvalidateVisual();
        }

        private void NumTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                int CurPos = NumTextBox.SelectionStart;
                string Txt = NumTextBox.Text.Remove(CurPos, NumTextBox.SelectionLength);
                e.Handled = !ValidateInput(Txt.Insert(CurPos, " "));
            }
            else if (e.Key == Key.Left)
            {
                // Prevent overlapping over an existing negative sign.
                if (Display.IsNegativePrefix() && NumTextBox.Text != null && NumTextBox.Text.Length > 0
                                       && NumTextBox.Text[0] == Display.GetNegativeSign()[0])
                    e.Handled = (NumTextBox.SelectionStart <= 1);
            }
            else if (e.Key == Key.Right)
            {
                if (!Display.IsNegativePrefix() && NumTextBox.Text != null && NumTextBox.Text.Length > 0
                       && NumTextBox.Text[NumTextBox.Text.Length - 1] == Display.GetNegativeSign()[0])
                    e.Handled = (NumTextBox.SelectionStart == NumTextBox.Text.Length - 1);
            }
            else if (e.Key == Key.Home || e.Key == Key.PageUp)
            {
                if (Display.IsNegativePrefix() && NumTextBox.Text != null && NumTextBox.Text.Length > 0 
                                                        && NumTextBox.Text[0] == Display.GetNegativeSign()[0])
                {
                    NumTextBox.SelectionStart = 1;
                    NumTextBox.SelectionLength = 0;
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.End || e.Key == Key.PageDown)
            {
                if (!Display.IsNegativePrefix() && NumTextBox.Text != null && NumTextBox.Text.Length > 0
                                        && NumTextBox.Text[NumTextBox.Text.Length - 1] == Display.GetNegativeSign()[0])
                {
                    NumTextBox.SelectionStart = NumTextBox.Text.Length - 1;
                    NumTextBox.SelectionLength = 0;
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                int CurPos = NumTextBox.SelectionStart;
                string Txt = NumTextBox.Text;

                if (NumTextBox.SelectionLength > 0)
                    Txt = Txt.Remove(CurPos, NumTextBox.SelectionLength);
                else if (e.Key == Key.Delete && CurPos < Txt.Length - 1)
                    Txt = Txt.Remove(CurPos, 1);
                else if (CurPos > 0)
                    Txt = Txt.Remove(CurPos - 1, 1);

                e.Handled = !ValidateInput(Txt);
            }
        }
        
        private void Root_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    FrameworkElement FWE = sender as FrameworkElement;

                    if (FWE != null)
                    {
                        FWE.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                        e.Handled = true;
                    }
                }
                else
                    e.Handled = false;
            }
        }

        private void Root_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            NumTextBox.Visibility = (IsEnabled)? Visibility.Visible : Visibility.Hidden;
            Display.InvalidateVisual();
        }

        private void Root_Loaded(object sender, RoutedEventArgs e)
        {
            Display.InvalidateVisual();
        }

        private void ProcessSciDets(List<Char> lstValidChars, ref string Input, ref bool scientific_entered, ref int Idx, ref int Length)
        {
            lstValidChars.Remove('e');
            lstValidChars.Remove('E');

            scientific_entered = true;

            if ((CtrlFlags & (short)CtrlFlag.AllowNegSci) != 0
                                    && Idx + 1 < Length && Input[Idx + 1] == Display.GetNegativeSign()[0])
                Idx++;
        }

        private bool ValidateInput(string Input)
        {
            int Idx = 0;
            int Length = Input.Length;
            bool scientific_entered = false;
            
            if ((CtrlFlags & (short)CtrlFlag.AllowNegatives) != 0)
            {
                if (Display.IsNegativePrefix())
                {
                    if (Length > 0 && Input[Idx] == Display.GetNegativeSign()[0])
                        Idx++;
                }
                else
                {
                    if (Length > 0 && Input[Length - 1] == Display.GetNegativeSign()[0])
                        Length--;
                }
            }

            if (((CtrlFlags & (short)CtrlFlag.AllowMain) != 0 || (CtrlFlags & (short)CtrlFlag.AllowFractions) != 0))
            {
                List<Char> lstValidChars = new List<Char>();
                lstValidChars.AddRange("0123456789");
                short PosAfterDecSep = -2;

                if ((CtrlFlags & (short)CtrlFlag.AllowScientific) != 0 && (CtrlFlags & (short)CtrlFlag.AllowMain) != 0)
                    lstValidChars.AddRange("eE");             
                
                if ((CtrlFlags & (short)CtrlFlag.AllowFractions) != 0)
                {
                    if ((CtrlFlags & (short)CtrlFlag.AllowMain) != 0)
                        lstValidChars.AddRange(" /");
                    else
                        lstValidChars.Add('/');
                }

                if ((CtrlFlags & (short)CtrlFlag.AllowMain) != 0 && DecimalPlaces != 0)
                    lstValidChars.Add(Display.GetDecimalSeparator()[0]);

                while (Idx < Length)
                {
                    char chr = lstValidChars.Find(delegate(char ch) { return ch == Input[Idx]; });

                    if (chr == '\0')
                        return false;
                    else if (PosAfterDecSep == DecimalPlaces)
                    {
                        if (chr == ' ' && (CtrlFlags & (short)CtrlFlag.AllowFractions) != 0)
                        {
                            lstValidChars.Remove(' ');
                            lstValidChars.Add('/');
                            lstValidChars.Remove('e');
                            lstValidChars.Remove('E');
                        }
                        else if (chr == 'E' || chr == 'e')
                        {
                            ProcessSciDets(lstValidChars, ref Input, ref scientific_entered, ref Idx, ref Length);
                        }
                        else
                            return false;
                    }
                    else
                    {
                        if (chr == ' ')
                        {
                            lstValidChars.Remove(Display.GetDecimalSeparator()[0]);
                            lstValidChars.Remove(' ');
                            lstValidChars.Remove('e');
                            lstValidChars.Remove('E');

                            if (PosAfterDecSep != -2 || scientific_entered)
                            {
                                PosAfterDecSep = -2;
                                lstValidChars.Add('/');
                            }
                        }
                        else if (chr == '/')
                        {
                            lstValidChars.Remove(Display.GetDecimalSeparator()[0]);
                            lstValidChars.Remove(' ');
                            lstValidChars.Remove('/');
                            lstValidChars.Remove('e');
                            lstValidChars.Remove('E');
                        }
                        else if (chr == Display.GetDecimalSeparator()[0])
                        {
                            if (DecimalPlaces > 0)
                                PosAfterDecSep = -1;

                            lstValidChars.Remove(Display.GetDecimalSeparator()[0]);
                            lstValidChars.Remove('/');
                        }
                        else if (chr == 'E' || chr == 'e')
                        {
                            lstValidChars.Remove(Display.GetDecimalSeparator()[0]);
                            lstValidChars.Remove('/');

                            ProcessSciDets(lstValidChars, ref Input, ref scientific_entered, ref Idx, ref Length);

                            if (PosAfterDecSep != -2)
                                PosAfterDecSep = -2;
                        }
                    }
                   
                    Idx++;

                    if (PosAfterDecSep > -2)
                        PosAfterDecSep++;
                }
            }
            return true;
        }

        private void NumTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length == 0)
                return;

            int CurPos = NumTextBox.SelectionStart;
            string Txt = NumTextBox.Text.Remove(CurPos, NumTextBox.SelectionLength);

            if (e.Text == Display.GetNegativeSign())
            {
                bool SciNoteUsed = false;

                if (e.Text.Length == 1 && (CtrlFlags & (short)CtrlFlag.AllowNegSci) != 0)
                {
                    char[] eE = { 'e', 'E' };
                    int StartPos = Txt.IndexOfAny(eE);
                    int EndPos = Txt.IndexOf(' ');

                    if (StartPos >= 0 && CurPos > StartPos && (EndPos == -1 || CurPos <= EndPos))
                    {
                        SciNoteUsed = true;

                        if (Txt.Length > StartPos + 1 && Txt[StartPos + 1] == Display.GetNegativeSign()[0])
                        {
                            Txt = Txt.Remove(StartPos + 1, 1);
                            CurPos--; 
                        }
                        else
                        {
                            Txt = Txt.Insert(StartPos + 1, Display.GetNegativeSign());
                            CurPos++;
                        }
                        NumTextBox.Text = Txt;
                    }
                }

                if (!SciNoteUsed && (CtrlFlags & (short)CtrlFlag.AllowNegatives) != 0)
                {
                    bool IsNegativeValue = false;

                    if (Display.IsNegativePrefix())
                    {
                        if (Txt.Length > 0 && Txt[0] == e.Text[0])
                        {
                            NumTextBox.Text = Txt.Remove(0, 1);
                            if (CurPos > 0) CurPos--;
                        }
                        else
                        {
                            NumTextBox.Text = e.Text + Txt;
                            CurPos++;
                            IsNegativeValue = true;
                        }
                    }
                    else
                    {
                        if (Txt.Length > 0 && Txt[Txt.Length - 1] == e.Text[0])
                        {
                            NumTextBox.Text = Txt.Remove(Txt.Length - 1, 1);

                            if (CurPos > NumTextBox.Text.Length)
                                CurPos = NumTextBox.Text.Length;
                        }
                        else
                        {
                            NumTextBox.Text = Txt + e.Text;
                            IsNegativeValue = true;
                        }
                    }
                    NumTextBox.Foreground = ((IsNegativeValue && NegativeTextBrush != null) ? NegativeTextBrush : Foreground);
                }
                NumTextBox.SelectionStart = CurPos;
                e.Handled = true;
            }
            else
            {
                if (Keyboard.GetKeyStates(Key.Insert) == KeyStates.Toggled)
                {
                    if (CurPos < Txt.Length)
                        Txt = Txt.Remove(CurPos, 1);

                    Txt = Txt.Insert(CurPos, e.Text);

                    if (ValidateInput(Txt))
                    {
                        NumTextBox.Text = Txt;
                        CurPos++;
                    }
                    e.Handled = true;
                    NumTextBox.SelectionStart = CurPos;
                }
                else
                {
                    e.Handled = !ValidateInput(Txt.Insert(CurPos, e.Text));
                    NumTextBox.SelectionStart = CurPos;
                }
            }
        }

        private void Command_Cut_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = NumTextBox.SelectionLength > 0 &&
                ValidateInput(NumTextBox.Text.Remove(NumTextBox.SelectionStart, NumTextBox.SelectionLength));
            e.Handled = true;
        }

        private void Command_Paste_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            string ClipTxt = (string)Clipboard.GetData("Text");
            int CurPos = NumTextBox.SelectionStart;
            string Txt = NumTextBox.Text.Remove(CurPos, NumTextBox.SelectionLength);

            e.CanExecute = ValidateInput(Txt.Insert(CurPos, ClipTxt));
            e.Handled = true;
        }

        private void NumTextBox_PreviewDragEnter(object sender, DragEventArgs e)
        {
            NumTextBox.Focus();
            ShowTextBox();
            e.Handled = true;
        }

        private void NumTextBox_PreviewDrop(object sender, DragEventArgs e)
        {
            string DragTxt = (string)e.Data.GetData("Text");
            int CurPos = NumTextBox.SelectionStart;
            string Txt = NumTextBox.Text.Insert(CurPos, DragTxt);

            if (ValidateInput(Txt))
                NumTextBox.Text = Txt;

            e.Handled = true;
        }
    }
}
