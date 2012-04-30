#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// Two EMA  line crossover indicator
	/// </summary>
	[Description("Two EMA  line crossover indicator")]
	public class GekkoEMAcrossover : Indicator
	{
		#region Variables

		private int fastEMAPeriod = 5; // Default setting for FastEMAPeriod
		private int slowEMAPeriod = 8; // Default setting for SlowEMAPeriod
		private int _adxPeriod = 10; // Default setting for SlowEMAPeriod
		private int _adxMin = 20;

		private EMA _emaSlow = null;
		private EMA _emaFast = null;
		private ADX _adx = null;

		private int _signal = 0;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.SeaGreen), PlotStyle.Line, "FastEMAPlot"));
			Add(new Plot(Color.FromKnownColor(KnownColor.GreenYellow), PlotStyle.Line, "SlowEMAPlot"));
			//Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "ADXPlot"));
			
			Overlay	= true;
	
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			 

			if (CurrentBar <= BarsRequired) return;

			if (_emaSlow == null) 
				_emaSlow = EMA(Open, slowEMAPeriod);
			if (_emaFast == null) 
				_emaFast = EMA(fastEMAPeriod);
			if (_adx == null) 
				_adx = ADX(_adxPeriod);	
			
				
			if (_adx[0] >= _adxMin) 
			{
				PlotColors[0][0] = Color.Blue;
			} else 
			{
				PlotColors[0][0] = Color.LightBlue;
			}
			
			FastEMAPlot.Set(_emaFast[0]);
			SlowEMAPlot.Set(_emaSlow[0]);
			
			if (CrossAbove(_emaFast, _emaSlow, 1)&& _adx[0] >= _adxMin) 
			{

				_signal = 1;
				BackColor = Color.LightGreen;
			}
			else if (CrossBelow(_emaFast, _emaSlow, 1) && _adx[0] >= _adxMin)
			{		
				_signal = -1;
				BackColor = Color.Pink;
				
			
			}
            else
            {
                _signal = 0;
                BackColor = Color.White;
            }
			
		}

		#region Properties
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries FastEMAPlot
		{
			get { return Values[0]; }
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries SlowEMAPlot
		{
			get { return Values[1]; }
		}
		
//        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
//        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
//        public DataSeries ADXPlot
//        {
//            get { return Values[2]; }
//        }
		[Description("")]
		[GridCategory("Parameters")]
		public int FastEMAPeriod
		{
			get { return fastEMAPeriod; }
			set { fastEMAPeriod = Math.Max(1, value); }
		}

		[Description("")]
		[GridCategory("Parameters")]
		public int SlowEMAPeriod
		{
			get { return slowEMAPeriod; }
			set { slowEMAPeriod = Math.Max(1, value); }
		}
		
		[Description("")]
		[GridCategory("Parameters")]
		public int ADXMinimum
		{
			get { return _adxMin; }
			set { _adxMin = Math.Max(1, value); }
		}
		
				[Description("")]
		[GridCategory("Parameters")]
		public int ADXPeriod
		{
			get { return _adxPeriod; }
			set { _adxPeriod = Math.Max(1, value); }
		}
		
		[Description("Trade signal, -1, 0, 1 = short, neutral, long")]
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]
		public int Signal
		{
			get { return _signal; }
		}
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	public partial class Indicator : IndicatorBase
	{
		private GekkoEMAcrossover[] cacheGekkoEMAcrossover = null;

		private static GekkoEMAcrossover checkGekkoEMAcrossover = new GekkoEMAcrossover();

		/// <summary>
		/// Two EMA  line crossover indicator
		/// </summary>
		/// <returns></returns>
		public GekkoEMAcrossover GekkoEMAcrossover(int aDXMinimum, int aDXPeriod, int fastEMAPeriod, int slowEMAPeriod)
		{
			return GekkoEMAcrossover(Input, aDXMinimum, aDXPeriod, fastEMAPeriod, slowEMAPeriod);
		}

		/// <summary>
		/// Two EMA  line crossover indicator
		/// </summary>
		/// <returns></returns>
		public GekkoEMAcrossover GekkoEMAcrossover(Data.IDataSeries input, int aDXMinimum, int aDXPeriod, int fastEMAPeriod, int slowEMAPeriod)
		{
			if (cacheGekkoEMAcrossover != null)
				for (int idx = 0; idx < cacheGekkoEMAcrossover.Length; idx++)
					if (cacheGekkoEMAcrossover[idx].ADXMinimum == aDXMinimum && cacheGekkoEMAcrossover[idx].ADXPeriod == aDXPeriod && cacheGekkoEMAcrossover[idx].FastEMAPeriod == fastEMAPeriod && cacheGekkoEMAcrossover[idx].SlowEMAPeriod == slowEMAPeriod && cacheGekkoEMAcrossover[idx].EqualsInput(input))
						return cacheGekkoEMAcrossover[idx];

			lock (checkGekkoEMAcrossover)
			{
				checkGekkoEMAcrossover.ADXMinimum = aDXMinimum;
				aDXMinimum = checkGekkoEMAcrossover.ADXMinimum;
				checkGekkoEMAcrossover.ADXPeriod = aDXPeriod;
				aDXPeriod = checkGekkoEMAcrossover.ADXPeriod;
				checkGekkoEMAcrossover.FastEMAPeriod = fastEMAPeriod;
				fastEMAPeriod = checkGekkoEMAcrossover.FastEMAPeriod;
				checkGekkoEMAcrossover.SlowEMAPeriod = slowEMAPeriod;
				slowEMAPeriod = checkGekkoEMAcrossover.SlowEMAPeriod;

				if (cacheGekkoEMAcrossover != null)
					for (int idx = 0; idx < cacheGekkoEMAcrossover.Length; idx++)
						if (cacheGekkoEMAcrossover[idx].ADXMinimum == aDXMinimum && cacheGekkoEMAcrossover[idx].ADXPeriod == aDXPeriod && cacheGekkoEMAcrossover[idx].FastEMAPeriod == fastEMAPeriod && cacheGekkoEMAcrossover[idx].SlowEMAPeriod == slowEMAPeriod && cacheGekkoEMAcrossover[idx].EqualsInput(input))
							return cacheGekkoEMAcrossover[idx];

				GekkoEMAcrossover indicator = new GekkoEMAcrossover();
				indicator.BarsRequired = BarsRequired;
				indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
				indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
				indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
				indicator.Input = input;
				indicator.ADXMinimum = aDXMinimum;
				indicator.ADXPeriod = aDXPeriod;
				indicator.FastEMAPeriod = fastEMAPeriod;
				indicator.SlowEMAPeriod = slowEMAPeriod;
				Indicators.Add(indicator);
				indicator.SetUp();

				GekkoEMAcrossover[] tmp = new GekkoEMAcrossover[cacheGekkoEMAcrossover == null ? 1 : cacheGekkoEMAcrossover.Length + 1];
				if (cacheGekkoEMAcrossover != null)
					cacheGekkoEMAcrossover.CopyTo(tmp, 0);
				tmp[tmp.Length - 1] = indicator;
				cacheGekkoEMAcrossover = tmp;
				return indicator;
			}
		}
	}
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
	public partial class Column : ColumnBase
	{
		/// <summary>
		/// Two EMA  line crossover indicator
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.GekkoEMAcrossover GekkoEMAcrossover(int aDXMinimum, int aDXPeriod, int fastEMAPeriod, int slowEMAPeriod)
		{
			return _indicator.GekkoEMAcrossover(Input, aDXMinimum, aDXPeriod, fastEMAPeriod, slowEMAPeriod);
		}

		/// <summary>
		/// Two EMA  line crossover indicator
		/// </summary>
		/// <returns></returns>
		public Indicator.GekkoEMAcrossover GekkoEMAcrossover(Data.IDataSeries input, int aDXMinimum, int aDXPeriod, int fastEMAPeriod, int slowEMAPeriod)
		{
			return _indicator.GekkoEMAcrossover(input, aDXMinimum, aDXPeriod, fastEMAPeriod, slowEMAPeriod);
		}
	}
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
	public partial class Strategy : StrategyBase
	{
		/// <summary>
		/// Two EMA  line crossover indicator
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.GekkoEMAcrossover GekkoEMAcrossover(int aDXMinimum, int aDXPeriod, int fastEMAPeriod, int slowEMAPeriod)
		{
			return _indicator.GekkoEMAcrossover(Input, aDXMinimum, aDXPeriod, fastEMAPeriod, slowEMAPeriod);
		}

		/// <summary>
		/// Two EMA  line crossover indicator
		/// </summary>
		/// <returns></returns>
		public Indicator.GekkoEMAcrossover GekkoEMAcrossover(Data.IDataSeries input, int aDXMinimum, int aDXPeriod, int fastEMAPeriod, int slowEMAPeriod)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return _indicator.GekkoEMAcrossover(input, aDXMinimum, aDXPeriod, fastEMAPeriod, slowEMAPeriod);
		}
	}
}
#endregion
