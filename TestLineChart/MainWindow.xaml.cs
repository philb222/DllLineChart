using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Threading;

namespace TestLineChart
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private decimal[] _dataValues1a = { 15m, 75m, 203m, 336m, 499m, 586m, 683m, 774m, 843m, 900m, 743m, 600m, 
											275m, 303m, 536m, 799m, 486m, 883m, 600m, 990m, 200m, 500m, 0m, 10m,
											275m, 303m, 536m, 799m, 486m, 883m, 600m, 990m, 200m, 500m, 20m, 100m };
		private decimal[] _dataValues1b = { 65m, 275m, 303m, 536m, 799m, 486m, 883m, 600m, 990m, 200m, 500m, 10m };
		//private double[] _dataValues2 = { 65d, 275d, 303d, 536d, 799d, 486d, 883d, 600d, 990d, 200d, 500d, 0d, 10d, 100d, 101.02d, 111.005d };
		//private double[] _dataValues2 = { 65d, 275d, 303d, 536d };
		//private double[] _dataValues2 = { 65d, 275d, 303d, 536d, 799d, 65d, 275d };
		private decimal[] _dataValues2 = { 65m, 275m, 303m, 536m, 799m, 65m, 275m, 400m, 0m, 500m };
		//private double[] _dataValues2 = { 65d, 275d, 303d, 500d, 510d };
		//private double[] _dataValues2 = { 65d, 275d, 303d, 536d, 799d, 65d, 275d, 300d, 411.003d };

		//private string _XaxisValues = "AAA BBB CCC DDD EEE FFF GGG HHH";
		//private double[] _YaxisValues3 = { 0d, 50d, 100d, 150d, 200d, 250d, 300d, 350d, 400d, 450d };
		//private double[] _dataValues3 = { 100d, 0d, 150d, 225d, 450d, 400d, 320d, 200d, 125d, 150d, 225d, 450d, 449d, 420d, 300d };
		//private double[] _YaxisValues3 = { 450d, 500d, 550d, 600d, 650d, 700d, 750d, 800d, 850d, 900d };
		//private double[] _dataValues3 = { 500d, 450d, 550d, 625d, 650d, 700d, 720d, 800d, 825d, 850d, 900d, 899d, 650d, 600d, 450d };
		private double[] _YaxisValues3 = { 200d, 250d, 300d, 350d, 400d, 450d, 500d, 550d, 600d };
		private double[] _dataValues3 = { 250d, 250d, 350d, 325d, 450d, 500d, 520d, 600d, 620d, 550d, 400d, 499d, 350d, 200d, 250d };

		DispatcherTimer _timer;
		public MainWindow()
		{
			InitializeComponent();

			this.DataContext = this;
			TestChartName = "Testing chart name via binding";

			_timer = new DispatcherTimer();
			_timer.Interval = new TimeSpan(0, 0, 0, 0, 2000);
			_timer.Tick += _timer_Tick;
			_timer.Start();
		}

		//public string TestChartName { get; set; }

		private static readonly DependencyProperty TestChartNameProperty =
			DependencyProperty.Register("TestChartName", typeof(string), typeof(MainWindow),
			new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender | 
				FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | 
				FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

		// Allow access via code.
		public static string GetTestChartNameProperty(UIElement e)
		{
			return (string)e.GetValue(TestChartNameProperty);
		}

		public static void SetTestChartNameProperty(UIElement e, string value)
		{
			e.SetValue(TestChartNameProperty, value);
		}

		// Allow access via XAML.
		public String TestChartName
		{
			get { return (String)GetValue(TestChartNameProperty); }

			set { SetValue(TestChartNameProperty, value); }
		}


		private void _timer_Tick(object sender, EventArgs e)
		{
			_timer.Stop();
			Chart1.CcDataValues = _dataValues1b;
		}

		private void btnQuit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Chart1.CcDataValues = _dataValues1a;
			Chart2.CcDataValues = _dataValues2;
			//Chart3.CcYaxisValues = _YaxisValues3;
			//Chart3.CcXaxisValues = _XaxisValues;
			//Chart3.CcDataValues = _dataValues3;
		}
	}
}
