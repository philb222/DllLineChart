using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DllLineChart
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// 
	/// This custom control provides a 'Line Chart' where:
	/// 
	/// 1. The user can set several dependency properties to customize the look and behavior. They all
	///    start with 'Cc' (custom control) so they are grouped together making them easy to find when coding xaml.
	/// 
	/// 2. The 'CcYaxisValues' property (array of doubles) defaults to 0d to 900d. To set other values the user must
	///    set CcYaxisValues via user code, not user xaml. These represent
	///    values to show going up the Y axis, to the left of the chart area.
	///    
	/// 3. The 'CcDataValues' property (array of doubles) must be set via user code, not user xaml. These are used to
	///    plot a line in the chart. This property can be set dynamically at time intervals, making the chart 
	///    re-draw itself with a new charted line to reflect the newly given values.
	///    
	/// 4. Once an application is running and shows a plotted line, the line vertices include ellipses (small circles).
	///    If the user hovers the mouse over one:
	///    
	///    - the Y-axis value displays as a tooltip
	///    - guidelines appear horizontally and vertically to help show the related X and Y axis values.
	/// 
	/// 5. Helpful error messages will display at run time in case of invalid property settings and errors.
	/// 
	/// </summary>
	public partial class LineChart : UserControl, INotifyPropertyChanged
	{
		#region Constructor
		public LineChart()
		{
			InitializeComponent();

			labChartName.Content = _defaultName;
		}
		#endregion

		#region Private fields

		// Next are default properties and constants that affects code here. Many match values in the xaml file.
		private const string _defaultName = "CcChartName not set";
		private const double _defaultControlHeight = 410d;      // Height of entire user control
		private const double _defaultControlWidth = 590d;       // Width of entire user control
		private const double _defaultChartHeight = 300d;        // Height of the plotted canvas area
		private const double _defaultChartWidth = 500d;         // Width of the plotted canvas area 
																//		private static string _defaultXaxisValues = "jqpWRTe 111111e 222222e 333333e 444444e 555555e 666666e 777777e 888888e 999999e";
		private static string _defaultXaxisValues = "0 1 2 3 4 5 6 7 8 9";
		private static string _defaultYaxisDescription = "Values";
		private static double[] _defaultYaxisValues = { 0d, 100d, 200d, 300d, 400d, 500d, 600d, 700d, 800d, 900d };
		private static decimal[] _defaultDataValues = { 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m };
		private int _dataPointsPerXaxisValue;					// Plotted line points from 1 X-axis value to the next
		private const int _defaultYaxisTickMarks = 2;           // # of Y-axis tick marks from one Y-axis value to the next
		private const double _defaultCanvasHorPadding = 10;     // Space to left and right of main canvas so X-axis labels at beginning and end can be seen

		private const double _borderThicknessAndPad = 7d;
		private const double _chartFrameLeft = 30d;
		private const double _ellipseRadius = 6d;
		private const double _plottedValuesInterval = 25d;      // Default interval between plotted values horizontally
		private double _xBump = 0d;                             // Horizontal bump amount for objects on the main chart
		private bool? _usedChartWidth = null;
		#endregion

		#region Internal Properties

		private double _ChartHeight = _defaultChartHeight;         // Height of the plotted canvas area
		public double ChartHeight
		{
			get { return _ChartHeight; }
			private set
			{
				if (_ChartHeight != value)
				{
					_ChartHeight = value;
					NotifyPropertyChanged();
				}
			}
		}

		private double _ChartWidth = _defaultChartWidth;         // Width of the plotted canvas area
		public double ChartWidth
		{
			get { return _ChartWidth; }
			private set
			{
				if (_ChartWidth != value)
				{
					_ChartWidth = value;
					NotifyPropertyChanged();
				}
			}
		}

		private double _ScrollViewerWidth = _defaultChartWidth + (_borderThicknessAndPad * 2d);
		public double ScrollViewerWidth
		{
			get { return _ScrollViewerWidth; }
			private set
			{
				if (_ScrollViewerWidth != value)
				{
					_ScrollViewerWidth = value;
					NotifyPropertyChanged();
				}
			}
		}

		#endregion

		#region Implement INotifyPropertyChanged
		/// <summary>
		/// Raised when a property on this object has a new value.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raises this object's PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">The property that has a new value.</param>
		protected virtual void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion

		#region CcChartName Chart Name Property (Caption at top of chart)

		public static readonly DependencyProperty CcChartNameProperty =
			DependencyProperty.Register("CcChartName", typeof(string), typeof(LineChart),
				new FrameworkPropertyMetadata(_defaultName, FrameworkPropertyMetadataOptions.AffectsRender,
				new PropertyChangedCallback(CcChartNamePropertyChanged)));

		private static void CcChartNamePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as LineChart).OnCcChartNamePropertyChanged(e);
		}

		private void OnCcChartNamePropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			labChartName.Content = (String)e.NewValue;
		}

		// Allow access via code.
		public static string GetCcChartNameProperty(UIElement e)
		{
			return (string)e.GetValue(CcChartNameProperty);
		}

		public static void SetCcChartNameProperty(UIElement e, string value)
		{
			e.SetValue(CcChartNameProperty, value);
		}

		// Allow access via XAML.
		public String CcChartName
		{
			get { return (String)GetValue(CcChartNameProperty); }

			set { SetValue(CcChartNameProperty, value); }
		}
		#endregion

		#region CcBackground1 Property

		private static readonly DependencyProperty CcBackground1Property =
			DependencyProperty.Register("CcBackground1", typeof(Color), typeof(LineChart),
			new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.AffectsRender,
			new PropertyChangedCallback(CcBackground1PropertyChanged)));

		private static void CcBackground1PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as LineChart).OnCcBackground1PropertyChanged(e);
		}

		private void OnCcBackground1PropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			SolidColorBrush aBrush = (SolidColorBrush)this.Resources["Background1"];
			aBrush.Color = (Color)e.NewValue;
		}

		// Allow access via XAML.
		public Color CcBackground1
		{
			get { return (Color)GetValue(CcBackground1Property); }

			set { SetValue(CcBackground1Property, value); }
		}
		#endregion

		#region CcBackground2 Property

		private static readonly DependencyProperty CcBackground2Property =
			DependencyProperty.Register("CcBackground2", typeof(Color), typeof(LineChart),
			new FrameworkPropertyMetadata(Colors.Blue, FrameworkPropertyMetadataOptions.AffectsRender,
			new PropertyChangedCallback(CcBackground2PropertyChanged)));

		private static void CcBackground2PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as LineChart).OnCcBackground2PropertyChanged(e);
		}

		private void OnCcBackground2PropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			SolidColorBrush aBrush = (SolidColorBrush)this.Resources["Background2"];
			aBrush.Color = (Color)e.NewValue;
		}

		// Allow access via XAML.
		public Color CcBackground2
		{
			get { return (Color)GetValue(CcBackground2Property); }

			set { SetValue(CcBackground2Property, value); }
		}
		#endregion

		#region CcForeground1 Property

		private static readonly DependencyProperty CcForeground1Property =
			DependencyProperty.Register("CcForeground1", typeof(Color), typeof(LineChart),
			new FrameworkPropertyMetadata(Colors.Yellow, FrameworkPropertyMetadataOptions.AffectsRender,
			new PropertyChangedCallback(CcForeground1PropertyChanged)));

		private static void CcForeground1PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as LineChart).OnCcForeground1PropertyChanged(e);
		}

		private void OnCcForeground1PropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			SolidColorBrush aBrush = (SolidColorBrush)this.Resources["Foreground1"];
			aBrush.Color = (Color)e.NewValue;
		}

		// Allow access via XAML.
		public Color CcForeground1
		{
			get { return (Color)GetValue(CcForeground1Property); }

			set { SetValue(CcForeground1Property, value); }
		}
		#endregion

		#region CcForeground2 Property

		private static readonly DependencyProperty CcForeground2Property =
			DependencyProperty.Register("CcForeground2", typeof(Color), typeof(LineChart),
			new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.AffectsRender,
			new PropertyChangedCallback(CcForeground2PropertyChanged)));

		private static void CcForeground2PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as LineChart).OnCcForeground2PropertyChanged(e);
		}

		private void OnCcForeground2PropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			SolidColorBrush aBrush = (SolidColorBrush)this.Resources["Foreground2"];
			aBrush.Color = (Color)e.NewValue;
		}

		// Allow access via XAML.
		public Color CcForeground2
		{
			get { return (Color)GetValue(CcForeground2Property); }

			set { SetValue(CcForeground2Property, value); }
		}
		#endregion

		#region CcGraphicsColor Property (for graphics in the main chart area)

		private static readonly DependencyProperty CcGraphicsColorProperty =
			DependencyProperty.Register("CcGraphicsColor", typeof(Color), typeof(LineChart),
			new FrameworkPropertyMetadata(Colors.Lime, FrameworkPropertyMetadataOptions.AffectsRender,
			new PropertyChangedCallback(CcGraphicsColorPropertyChanged)));

		private static void CcGraphicsColorPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as LineChart).OnCcGraphicsColorPropertyChanged(e);
		}

		private void OnCcGraphicsColorPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			SolidColorBrush aBrush = (SolidColorBrush)this.Resources["GraphicsBush"];
			aBrush.Color = (Color)e.NewValue;
		}

		// Allow access via XAML.
		public Color CcGraphicsColor
		{
			get { return (Color)GetValue(CcGraphicsColorProperty); }

			set { SetValue(CcGraphicsColorProperty, value); }
		}
		#endregion

		#region CcHeight Property - for height of entire control

		private static readonly DependencyProperty CcHeightProperty =
			DependencyProperty.Register("CcHeight", typeof(double), typeof(LineChart),
			new FrameworkPropertyMetadata(_defaultControlHeight, FrameworkPropertyMetadataOptions.AffectsRender |
				FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure |
				FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure,
			new PropertyChangedCallback(CcHeightPropertyChanged)));

		private static void CcHeightPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as LineChart).OnCcHeightPropertyChanged(e);
		}

		private void OnCcHeightPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			double diff;
			// If larger value...
			if ((double)e.NewValue > (double)e.OldValue)
			{
				diff = (double)e.NewValue - (double)e.OldValue;
				ControlArea.Height += diff;
				ChartHeight += diff;
			}
			// Else smaller value...
			else
			{
				diff = (double)e.OldValue - (double)e.NewValue;
				ControlArea.Height -= diff;
				ChartHeight -= diff;
			}
		}

		// Allow access via XAML.
		public double CcHeight
		{
			get { return (double)GetValue(CcHeightProperty); }

			set { SetValue(CcHeightProperty, value); }
		}
		#endregion

		#region CcWidth Property - for width of entire control

		private static readonly DependencyProperty CcWidthProperty =
			DependencyProperty.Register("CcWidth", typeof(double), typeof(LineChart),
			new FrameworkPropertyMetadata(_defaultControlWidth, FrameworkPropertyMetadataOptions.AffectsRender |
				FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure |
				FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure,
			new PropertyChangedCallback(CcWidthPropertyChanged)));

		private static void CcWidthPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as LineChart).OnCcWidthPropertyChanged(e);
		}

		private void OnCcWidthPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			double diff;

			if ((double)e.NewValue > (double)e.OldValue)
			{
				diff = (double)e.NewValue - (double)e.OldValue;
				ControlArea.Width += diff;
				ScrollViewerWidth += diff;
				ChartWidth += diff;
			}
			// Else smaller value...
			else
			{
				diff = (double)e.OldValue - (double)e.NewValue;
				ControlArea.Width -= diff;
				ScrollViewerWidth -= diff;
				ChartWidth -= diff;
			}
		}

		// Allow access via XAML.
		public double CcWidth
		{
			get { return (double)GetValue(CcWidthProperty); }

			set { SetValue(CcWidthProperty, value); }
		}
		#endregion

		#region CcCanvasHorPadding Property - space to left and right of plotted line area

		private static readonly DependencyProperty CcCanvasHorPaddingProperty =
			DependencyProperty.Register("CcCanvasHorPadding", typeof(double), typeof(LineChart),
			new FrameworkPropertyMetadata(_defaultCanvasHorPadding, FrameworkPropertyMetadataOptions.AffectsRender));

		// Allow access via XAML.
		public double CcCanvasHorPadding
		{
			get { return (double)GetValue(CcCanvasHorPaddingProperty); }

			set { SetValue(CcCanvasHorPaddingProperty, value); }
		}
		#endregion

		#region CcXaxisValues Property - to show along the bottom of the chart.

		// These values are setup during the 'Load' event so by then, any height, width, etc. property
		// has been set. That way we can set Xaxis values properly.
		private static readonly DependencyProperty CcXaxisValuesProperty =
			DependencyProperty.Register("CcXaxisValues", typeof(string), typeof(LineChart),
			new FrameworkPropertyMetadata(_defaultXaxisValues, FrameworkPropertyMetadataOptions.AffectsRender));

		// Allow access via XAML.
		public string CcXaxisValues
		{
			get { return (string)GetValue(CcXaxisValuesProperty); }

			set { SetValue(CcXaxisValuesProperty, value); }
		}
		#endregion

		#region CcXaxisDescription Property - text to show below the CcXaxisValues strings.

		private static readonly DependencyProperty CcXaxisDescriptionProperty =
			DependencyProperty.Register("CcXaxisDescription", typeof(string), typeof(LineChart),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender,
			new PropertyChangedCallback(CcXaxisDescriptionChanged)));

		private static void CcXaxisDescriptionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as LineChart).OnCcXaxisDescriptionPropertyChanged(e);
		}

		private void OnCcXaxisDescriptionPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			labXaxisDescription.Content = e.NewValue;
		}

		// Allow access via XAML.
		public String CcXaxisDescription
		{
			get { return (String)GetValue(CcXaxisDescriptionProperty); }

			set { SetValue(CcXaxisDescriptionProperty, value); }
		}
		#endregion

		#region CcYaxisValues Property - values to show vertically to the left of the chart.

		// These values are setup during the 'Load' event so by then, any height, width, etc. property
		// has been set. That way we can set Yaxis values properly.
		private static readonly DependencyProperty CcYaxisValuesProperty =
			DependencyProperty.Register("CcYaxisValues", typeof(double[]), typeof(LineChart),
			new FrameworkPropertyMetadata(_defaultYaxisValues, FrameworkPropertyMetadataOptions.AffectsRender));

		// Allow access via XAML.
		public double[] CcYaxisValues
		{
			get { return (double[])GetValue(CcYaxisValuesProperty); }

			set { SetValue(CcYaxisValuesProperty, value); }
		}
		#endregion

		#region CcYaxisTickMarks Property - # of plotted line points from 1 Yaxis value to the next.

		private static readonly DependencyProperty CcYaxisTickMarksProperty =
			DependencyProperty.Register("CcYaxisTickMarks", typeof(int), typeof(LineChart),
			new FrameworkPropertyMetadata(_defaultYaxisTickMarks, FrameworkPropertyMetadataOptions.AffectsRender));

		// Allow access via XAML.
		public int CcYaxisTickMarks
		{
			get { return (int)GetValue(CcYaxisTickMarksProperty); }

			set { SetValue(CcYaxisTickMarksProperty, value); }
		}
		#endregion

		#region CcYaxisDescription Property - text to show to the left of the CcYaxisValues.

		private static readonly DependencyProperty CcYaxisDescriptionProperty =
			DependencyProperty.Register("CcYaxisDescription", typeof(string), typeof(LineChart),
			new FrameworkPropertyMetadata(_defaultYaxisDescription, FrameworkPropertyMetadataOptions.AffectsRender,
			new PropertyChangedCallback(CcYaxisDescriptionPropertyChanged)));

		private static void CcYaxisDescriptionPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as LineChart).OnCcYaxisDescriptionPropertyChanged(e);
		}

		private void OnCcYaxisDescriptionPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			labYaxis.Content = e.NewValue;
		}

		// Allow access via XAML.
		public String CcYaxisDescription
		{
			get { return (String)GetValue(CcYaxisDescriptionProperty); }

			set { SetValue(CcYaxisDescriptionProperty, value); }
		}
		#endregion

		#region CcShowTickMarks Property - to show small guidelines along the X and Y axis.

		private static readonly DependencyProperty CcShowTickMarksProperty =
			DependencyProperty.Register("CcShowTickMarks", typeof(bool), typeof(LineChart),
			new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

		// Allow access via XAML.
		public bool CcShowTickMarks
		{
			get { return (bool)GetValue(CcShowTickMarksProperty); }

			set { SetValue(CcShowTickMarksProperty, value); }
		}
		#endregion

		#region CcShowBlips Property - to show small circles in the charted line's vertices.

		private static readonly DependencyProperty CcShowBlipsProperty =
			DependencyProperty.Register("CcShowBlips", typeof(bool), typeof(LineChart),
			new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

		// Allow access via XAML.
		public bool CcShowBlips
		{
			get { return (bool)GetValue(CcShowBlipsProperty); }

			set { SetValue(CcShowBlipsProperty, value); }
		}
		#endregion

		#region CcScale Property - to scale the entire custom control bigger or smaller.

		private static readonly DependencyProperty CcScaleProperty =
			DependencyProperty.Register("CcScale", typeof(double), typeof(LineChart),
			new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));

		// Allow access via XAML.
		public double CcScale
		{
			get { return (double)GetValue(CcScaleProperty); }

			set { SetValue(CcScaleProperty, value); }
		}
		#endregion

		#region CcThresholdHigh Property - to show optional faint rectangle where high values deserve attention.

		private static readonly DependencyProperty CcThresholdHighProperty =
			DependencyProperty.Register("CcThresholdHigh", typeof(double), typeof(LineChart),
			new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

		// Allow access via XAML.
		public double CcThresholdHigh
		{
			get { return (double)GetValue(CcThresholdHighProperty); }

			set { SetValue(CcThresholdHighProperty, value); }
		}
		#endregion

		#region CcThresholdLow Property - to show optional faint rectangle where low values deserve attention.

		private static readonly DependencyProperty CcThresholdLowProperty =
			DependencyProperty.Register("CcThresholdLow", typeof(double), typeof(LineChart),
			new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

		// Allow access via XAML.
		public double CcThresholdLow
		{
			get { return (double)GetValue(CcThresholdLowProperty); }

			set { SetValue(CcThresholdLowProperty, value); }
		}
		#endregion

		#region CcDataValues Property - main values used to draw a line on the chart

		private static readonly DependencyProperty CcDataValuesProperty =
			DependencyProperty.Register("CcDataValues", typeof(decimal[]), typeof(LineChart),
			new FrameworkPropertyMetadata(_defaultDataValues, FrameworkPropertyMetadataOptions.AffectsRender,
			new PropertyChangedCallback(CcDataValuesPropertyChanged)));

		private static void CcDataValuesPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as LineChart).OnCcDataValuesPropertyChanged(e);
		}

		private void OnCcDataValuesPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			try
			{
				CreateLineChart();

				CanvasScroller.ScrollToLeftEnd();
			}
			catch (Exception ex)
			{
				ShowErrorMessage(ex.Message);
			}
		}


		// Allow access via XAML.
		public decimal[] CcDataValues
		{
			get { return (decimal[])GetValue(CcDataValuesProperty); }

			set { SetValue(CcDataValuesProperty, value); }
		}

		private void CreateLineChart()
		{
			// First, remove the old graphics if there. Don't clear all. Need to leave CcThresholdxxx graphics.
			UIElementCollection eCollection = MainCanvas.Children;

			for (int x = 0; x < eCollection.Count; ++x)
			{
				if (eCollection[x] is Ellipse)
				{
					Ellipse anElement = (Ellipse)eCollection[x];
					if ((string)anElement.Tag == "ChartDot")
					{
						anElement.MouseEnter -= AnEllipse_MouseEnter;
						anElement.MouseLeave -= AnEllipse_MouseLeave;
						eCollection.RemoveAt(x);
						--x;
					}
				}
				else if (eCollection[x] is Polyline)
				{
					Polyline anElement = (Polyline)eCollection[x];
					if ((string)anElement.Tag == "ChartLine")
					{
						eCollection.RemoveAt(x);
						--x;
					}
				}
			}
			// Calculate width to hold all values
			double horDataLength = (((double)CcDataValues.Length - 1d) * _plottedValuesInterval) + (CcCanvasHorPadding * 2d);

			// If data exceeds the chart's width...
			if (horDataLength > ChartWidth)
			{
				ChartWidth = horDataLength;			// Increase chart width so scroll bar appears.
				_usedChartWidth = false;
			}
			// Else data does not span the chart width...
			else
			{
				// If we had not previously used chart width, then do so now.
				// Otherwise, we had used chart width before, but now these CcDataValues are fewer so instead of spreading out 
				// the plotted line across the entire chart width, we would plot a line across part of the chart width.
				if (_usedChartWidth != false)
				{
					horDataLength = ChartWidth;
				}
			}
			horDataLength -= (CcCanvasHorPadding * 2d);

			// Prepare to create a new charted 'line' with points that span the chart horizontally.
			Polyline aLine = new Polyline();
			aLine.Tag = "ChartLine";

			PointCollection ptCollection = new PointCollection();

			// Start 'X' at far left of the canvas.
			double ptX = CcCanvasHorPadding;

			// Amount to bump 'x' of each point to the right evenly.
			_xBump = horDataLength / ((double)CcDataValues.Length - 1d);

			// Calc ratio so given CcDataValues work with ChartHeight
			double maxYvalue = CcYaxisValues.Max();
			double minYvalue = CcYaxisValues.Min();
			double chartHeightToMaxYRatio = ChartHeight / (maxYvalue - minYvalue);

			// For number of CcDataValuess, create points for the Polyline. ==================================================
			for (int x = 0; x < CcDataValues.Length; ++x)
			{
				Point aPoint = new Point();
				aPoint.X = ptX;
				aPoint.Y = ChartHeight - (((double)CcDataValues[x] - minYvalue) * chartHeightToMaxYRatio);
				ptCollection.Add(aPoint);

				ptX += _xBump;
			};
			aLine.Points = ptCollection;
			MainCanvas.Children.Add(aLine);

			if (CcShowBlips)
			{
				// Now create Ellipses with Tooltips for each point in the above Polyline ==========================================
				Ellipse anEllipse = null;
				ToolTip aToolTip = null;

				Thickness aMargin = new Thickness((CcCanvasHorPadding - _ellipseRadius), 0d, 0d, 0d);

				// Amount to bump margins to the right evenly.
				double marginBump = horDataLength / (CcDataValues.Length - 1d);

				// For the given CcDataValues, add ellipses and tooltips to the canvas.
				for (int x = 0; x < CcDataValues.Length; ++x)
				{
					anEllipse = new Ellipse();
					anEllipse.Opacity = 1d;
					anEllipse.Tag = "ChartDot";
					anEllipse.MouseEnter += AnEllipse_MouseEnter;
					anEllipse.MouseLeave += AnEllipse_MouseLeave;
					aMargin.Top = ChartHeight - ((((double)CcDataValues[x] - minYvalue) * chartHeightToMaxYRatio)) - _ellipseRadius;

					aToolTip = new ToolTip();
					aToolTip.Content = Convert.ToString(CcDataValues[x]);

					anEllipse.Margin = aMargin;
					anEllipse.ToolTip = aToolTip;

					MainCanvas.Children.Add(anEllipse);
					aMargin = new Thickness(aMargin.Left + marginBump, aMargin.Top, aMargin.Right, aMargin.Bottom);
				}
			}
		}

		/// <summary>
		/// When the mouse is over a dot on the canvas, add temporary
		/// guidelines to the canvas:
		/// 1. Add a horizontal line to help show the y-axis value for the dot.
		/// 2. Add a vertical line to help show the x-axis value for the dot.
		/// </summary>
		private void AnEllipse_MouseEnter(object sender, MouseEventArgs e)
		{
			SolidColorBrush graphicsBrush = (SolidColorBrush)this.Resources["GraphicsBush"];
			Ellipse anEllispe = sender as Ellipse;

			double x = anEllispe.Margin.Left + _ellipseRadius;
			double y = anEllispe.Margin.Top + _ellipseRadius;

			// Create a horizontal guideline for the canvas
			Line horizontalLine = new Line();
			horizontalLine.Stroke = graphicsBrush;
			horizontalLine.Tag = "TempGuideLine";
			horizontalLine.Opacity = 0.5d;
			horizontalLine.X1 = 0d;
			horizontalLine.Y1 = y;
			horizontalLine.X2 = x;
			horizontalLine.Y2 = y;

			// Create a vertical guideline for the canvas
			Line verticalLine = new Line();
			verticalLine.Stroke = graphicsBrush;
			verticalLine.Tag = "TempGuideLine";
			verticalLine.Opacity = 0.5d;
			verticalLine.X1 = x;
			verticalLine.Y1 = y;
			verticalLine.X2 = x;
			verticalLine.Y2 = ChartHeight;

			MainCanvas.Children.Add(horizontalLine);
			MainCanvas.Children.Add(verticalLine);
		}

		// Now remove the temporary guidelines from the canvas.
		private void AnEllipse_MouseLeave(object sender, MouseEventArgs e)
		{
			Ellipse anEllispe = sender as Ellipse;
			UIElementCollection eCollection = MainCanvas.Children;

			for (int x = 0; x < eCollection.Count; ++x)
			{
				if (eCollection[x] is Line)
				{
					Line anElement = (Line)eCollection[x];
					if ((string)anElement.Tag == "TempGuideLine")
					{
						eCollection.RemoveAt(x);
						--x;
					}
				}
			}
		}
		#endregion

		#region Loaded Event
		/// <summary>
		/// Here, the user's properties have been set on this control.
		/// So now we can setup things like x-axis info, etc.
		/// </summary>
		private void ControlArea_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				// Validate user provided properties.
				string msg = ValidateProperties();
				if (!String.IsNullOrEmpty(msg))
				{
					ShowErrorMessage(msg);
					return;
				}

				// If data was given via CcDataValues being set by the user...
				if (_xBump > 0d)
				{
					SetupXaxisValues();
					SetupYaxisValues();

					if (CcThresholdHigh > 0d)
					{
						SetupThresholdHigh();
					}
					if (CcThresholdLow > 0d)
					{
						SetupThresholdLow();
					}
				}
			}
			catch (Exception ex)
			{
				ShowErrorMessage(ex.Message);
			}
		}

		private void SetupXaxisValues()
		{
			string[] XaxisValues = CcXaxisValues.Split();

			// Prepare to create labels from the user-given X-axis information.
			Label aLabel = null;
			Thickness aPadding = new Thickness(0d, 0d, 0d, 0d);
			Style labelStyle = (Style)this.Resources["LabelStyle1"];
			FontFamily aFont = new FontFamily("Courier New");

			// Calculate the width of the largest X-axis value for setting labelWidth.
			int maxXaxisLength = (XaxisValues.Select(s => s.Length)).Max();
			string testWord = new string('W', maxXaxisLength);
			aLabel = new Label()
			{
				Content = testWord,
				FontFamily = aFont,
				FontSize = 10d,
				MinWidth = 5,
				Padding = aPadding,
				Style = labelStyle
			};
			double labelWidth = (MeasureString(testWord, aLabel)).Width;

			// Calculate number of data points (points to plot)
			_dataPointsPerXaxisValue = (CcDataValues.Length - 1) / (XaxisValues.Length - 1);

			double firstMarginleft = CcCanvasHorPadding - (labelWidth / 2d);
			Thickness aMargin = new Thickness(firstMarginleft, 0, 0, 0);

			// For number of Xaxis values, create labels and add them to the canvas along the bottom.
			for (int x = 0; x < XaxisValues.Length; ++x)
			{
				// Safety check if bumping too far. 
				if (aMargin.Left > ChartWidth - CcCanvasHorPadding)
					aMargin.Left = ChartWidth - CcCanvasHorPadding;

				aLabel = new Label()
				{
					Content = XaxisValues[x],
					FontFamily = aFont,
					FontSize = 10d,
					Margin = aMargin,
					Style = labelStyle,
					Tag = "ccXaxisLabel",
					Width = labelWidth
				};
				CanvasXaxis.Children.Add(aLabel);
				aMargin = new Thickness(aMargin.Left + (_xBump * _dataPointsPerXaxisValue), aMargin.Top, aMargin.Right, aMargin.Bottom);
			}
			int numLines = XaxisValues.Length * _dataPointsPerXaxisValue;

			if (CcShowTickMarks)
			{
				ShowXaxisTickMarks(numLines);
			}
		}

		/// <summary>
		/// Add 'n' tick marks (vertical lines) per X axis 'value' across the bottom Xaxis area
		/// </summary>
		private void ShowXaxisTickMarks(int numLines)
		{
			Line aLine = null;

			double lineX = CcCanvasHorPadding;

			// Create the tick-mark lines.
			for (int x = 0; x < numLines; ++x)
			{
				// Safety check if bumping too far. 
				if (lineX > ChartWidth - CcCanvasHorPadding)
					lineX = ChartWidth - CcCanvasHorPadding;

				aLine = new Line()
				{
					Opacity = 0.5d,
					X1 = lineX,
					X2 = lineX,
					Y1 = -7d,
					Y2 = 3d
				};
				CanvasXaxis.Children.Add(aLine);
				lineX += _xBump;
			}
		}

		/// <summary>
		/// Setup the main Y axis values up the left side of the chart area.
		/// </summary>
		private void SetupYaxisValues()
		{
			if (CcShowTickMarks)
			{
				ShowYaxisTickMarks();
			}

			Label aLabel = null;
			Thickness aPadding = new Thickness(0, 0, 0, 0);
			Style labelStyle = (Style)this.Resources["LabelStyle1"];

			// Calculate the width of the largest Y-axis value for setting labelWidth.
			double maxYaxisValue = CcYaxisValues.Max();
			int maxYwidth = (int)Math.Ceiling(maxYaxisValue);
			string testWord = maxYwidth.ToString();

			aLabel = new Label()
			{
				Content = testWord,
				MinWidth = 5,
				Padding = aPadding,
				Style = labelStyle
			};
			Size labelSize = MeasureString(testWord, aLabel);

			double labelHeight = labelSize.Height;
			double labelWidth = labelSize.Width;

			double firstMarginTop = 0d - (labelHeight / 2d) + _borderThicknessAndPad;

			Thickness aMargin = new Thickness(_chartFrameLeft - labelWidth, firstMarginTop, 0, 0);

			// Amount to bump label margins down the column evenly. 
			double marginBump = ChartHeight / ((double)CcYaxisValues.Length - 1d);

			// For number of Yaxis values, create labels and add them to the canvas to the left of the chart area.
			for (int x = CcYaxisValues.Length - 1; x >= 0; --x)
			{
				aLabel = new Label()
				{
					Content = CcYaxisValues[x],
					Margin = aMargin,
					Style = labelStyle,
					Tag = "ccYaxisLabel"
				};
				YaxisPanel.Children.Add(aLabel);
				aMargin = new Thickness(aMargin.Left, aMargin.Top + marginBump, aMargin.Right, aMargin.Bottom);
			}
		}

		/// <summary>
		/// Add 'n' (per CcYaxisTickMarks) tick marks (horizontal lines) per Y axis 'value' in the left Yaxis area.
		/// </summary>
		private void ShowYaxisTickMarks()
		{
			Line aLine = null;
			double lineY = _borderThicknessAndPad;

			int numLines = (CcYaxisValues.Length -1) * CcYaxisTickMarks;
			double xBump = ChartHeight / numLines;

			// Create the tick-mark lines.
			for (int x = 0; x <= numLines; ++x)
			{
				aLine = new Line()
				{
					Opacity = 0.5d,
					X1 = _chartFrameLeft + 7d,
					X2 = _chartFrameLeft - 3d,
					Y1 = lineY,
					Y2 = lineY
				};
				YaxisPanel.Children.Add(aLine);
				lineY += xBump;
			}
		}

		/// <summary>
		/// Measure the given text
		/// </summary>
		/// <param name="candidate">Text to measure</param>
		/// <param name="anElement">Label with text to measure</param>
		/// <returns>Size of the Text</returns>
		private Size MeasureString(string candidate, Label anElement)
		{
			var formattedText = new FormattedText(
				candidate,
				CultureInfo.CurrentUICulture,
				FlowDirection.LeftToRight,
				new Typeface(anElement.FontFamily, anElement.FontStyle, anElement.FontWeight, anElement.FontStretch),
				anElement.FontSize,
				Brushes.Black);

			return new Size(formattedText.Width, formattedText.Height);
		}

		/// <summary>
		/// Add a faint rectangle (via a polygon) to the main canvas that represents the area where
		/// 'high values' deserve attention.
		/// </summary>
		private void SetupThresholdHigh()
		{
			double maxYvalue = CcYaxisValues.Max();
			double minYvalue = CcYaxisValues.Min();

			// If valid threshold value...
			if (CcThresholdHigh < maxYvalue && CcThresholdHigh > 0d)
			{
				// Calculate ratio so CcThresholdHigh works with ChartHeight
				double chartHeightToMaxYRatio = ChartHeight / (maxYvalue - minYvalue);

				Polygon thresholdArea = new Polygon();
				PointCollection points = new PointCollection();

				double pointY = ChartHeight - ((CcThresholdHigh - minYvalue) * chartHeightToMaxYRatio);

				Point point1 = new Point(CcCanvasHorPadding, pointY);
				Point point2 = new Point(ChartWidth - CcCanvasHorPadding, pointY);
				Point point3 = new Point(ChartWidth - CcCanvasHorPadding, 0d);
				Point point4 = new Point(CcCanvasHorPadding, 0d);

				points.Add(point1);
				points.Add(point2);
				points.Add(point3);
				points.Add(point4);
				thresholdArea.Points = points;
				thresholdArea.Tag = "ThresholdHigh";

				MainCanvas.Children.Add(thresholdArea);

				// Make the threshold area be background so Mouse Enter / Mouse Leave
				// events still work on the ellipses / dots on the canvas.
				Canvas.SetZIndex(thresholdArea, -1);
			}
		}

		/// <summary>
		/// Add a faint rectangle (via a polygon) to the main canvas that represents the area where
		/// 'low values' deserve attention.
		/// </summary>
		private void SetupThresholdLow()
		{
			double maxYvalue = CcYaxisValues.Max();
			double minYvalue = CcYaxisValues.Min();

			// If valid threshold value...
			if (CcThresholdLow < maxYvalue)
			{
				// Calculate ratio so CcThresholdLow works with ChartHeight
				double chartHeightToMaxYRatio = ChartHeight / (maxYvalue - minYvalue);

				Polygon thresholdArea = new Polygon();
				PointCollection points = new PointCollection();

				double pointY = ChartHeight - ((CcThresholdLow - minYvalue) * chartHeightToMaxYRatio);

				Point point1 = new Point(CcCanvasHorPadding, pointY);
				Point point2 = new Point(ChartWidth - CcCanvasHorPadding, pointY);
				Point point3 = new Point(ChartWidth - CcCanvasHorPadding, ChartHeight);
				Point point4 = new Point(CcCanvasHorPadding, ChartHeight);

				points.Add(point1);
				points.Add(point2);
				points.Add(point3);
				points.Add(point4);
				thresholdArea.Points = points;
				thresholdArea.Tag = "ThresholdLow";

				MainCanvas.Children.Add(thresholdArea);

				// Make the threshold area be background so Mouse Enter / Mouse Leave
				// events still work on the ellipses / dots on the canvas.
				Canvas.SetZIndex(thresholdArea, -1);
			}
		}
		#endregion

		#region Unloaded Event

		/// <summary>
		/// Control has unloaded - release our many resources.
		/// </summary>
		private void ControlArea_Unloaded(object sender, RoutedEventArgs e)
		{
			DeleteChartObjects();

			// Collect garbage now.
			GC.Collect();
		}

		/// <summary>
		/// Remove all chart objects. This is used to free up memory.
		/// </summary>
		private void DeleteChartObjects()
		{
			UIElementCollection eCollection = MainCanvas.Children;

			for (int x = 0; x < eCollection.Count; ++x)
			{
				if (eCollection[x] is Ellipse)
				{
					Ellipse anElement = (Ellipse)eCollection[x];
					if ((string)anElement.Tag == "ChartDot")
					{
						// Remove event handlers first.
						anElement.MouseEnter -= AnEllipse_MouseEnter;
						anElement.MouseLeave -= AnEllipse_MouseLeave;
					}
				}
			}
			// Now we can remove all chart objects.
			MainCanvas.Children.Clear();

			CanvasXaxis.Children.Clear();
			YaxisPanel.Children.Clear();

			// Clear the arrays of data.
			this.CcYaxisValues = null;
			this.CcDataValues = null;
		}

		#endregion

		#region Error Handling
			/// <summary>
			/// Ensure the user has set valid property values.
			/// </summary>
			/// <returns>Empty string if OK, else error message.</returns>
		private string ValidateProperties()
		{
			string errorMsg = String.Empty;
			string[] XaxisValues = CcXaxisValues.Split();
			StringBuilder sb = new StringBuilder();

			if (CcDataValues.Length < XaxisValues.Length)
			{
				errorMsg = "Number of CcDataValues must be greater than or equal to number of CcXaxisValues.";
				return errorMsg;
			}

			if ((CcDataValues.Length) % (XaxisValues.Length) != 0)
			{
				sb.AppendLine("Invalid combination of CcDataValues and CcXaxisValues.");
				sb.AppendFormat("Number of CcDataValues given:  {0}", CcDataValues.Length);
				sb.AppendLine();
				sb.AppendFormat("Number of CcXaxisValues given: {0}", XaxisValues.Length);
				sb.AppendLine();
				sb.AppendLine("(Number of CcXaxisValues) must divide evenly into (number of CcDataValues). For example:");
				sb.AppendLine("\tNumber of CcXaxisValues =  4, number of CcDataValues = 4");
				sb.AppendLine("\tNumber of CcXaxisValues =  4, number of CcDataValues = 8");
				sb.AppendLine("\tNumber of CcXaxisValues =  4, number of CcDataValues = 12");
				sb.AppendLine("\tNumber of CcXaxisValues = 13, number of CcDataValues = 26");
				sb.AppendLine("\tNumber of CcXaxisValues = 13, number of CcDataValues = 39");

				errorMsg = sb.ToString();
				return errorMsg;
			}
			return errorMsg;
		}

		/// <summary>
		/// Show an error message to the user. Use the pre-defined
		/// TextBlock called 'txtError' which is on the main canvas.
		/// </summary>
		/// <param name="inMessage">Message to show</param>
		private void ShowErrorMessage(string inMessage)
		{
			txtError.Text = inMessage;
			txtError.Opacity = 1d;

			// Ensure error msg shows over any plotted line if there.
			Canvas.SetZIndex(txtError, 10);
		}
		#endregion


	}
}
