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
	/// Enter the description of your new custom indicator here
	/// </summary>
	[Description("Enter the description of your new custom indicator here")]
	public class EMAofATR : Indicator
	{
		#region Variables
		// Wizard generated variables
			private int period = 10; // Default setting for Period
		private double multiplier = 3;
		// User defined variables (add any user defined variables below)
		#endregion

		private ATR _atr = null;
		private SMA _ema = null;
		
		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Line, "ATRPlotHigh"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "EMAPlotHigh"));

			Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Line, "ATRPlotLow"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "EMAPlotLow"));

			Overlay				= true;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			 
			if (CurrentBar <= period) return;
			
			if (_atr == null)
				_atr  = ATR(period);

			if (_ema == null) 
				_ema = SMA(_atr, period);

			if (_atr == null & _ema == null) 
				return;

			ATRPlotHigh.Set(High[0] + _atr[0] * multiplier);
            ATRPlotLow.Set(Low[0] - _atr[0] * multiplier);


            EMAPlotHigh.Set(High[0] + _ema[0] * multiplier);
            EMAPlotLow.Set(Low[0] - _ema[0] * multiplier);

		}

		#region Properties
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries ATRPlotHigh
		{
			get { return Values[0]; }
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries EMAPlotHigh
		{
			get { return Values[1]; }
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries ATRPlotLow
		{
			get { return Values[2]; }
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries EMAPlotLow
		{
			get { return Values[3]; }
		}

		[Description("Perio of atr and ema")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
		}

		[Description("Multiplier of atr")]
		[GridCategory("Parameters")]
		public double Multiplier
		{
			get { return multiplier; }
			set { multiplier = Math.Max(1, value); }
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
		private EMAofATR[] cacheEMAofATR = null;

		private static EMAofATR checkEMAofATR = new EMAofATR();

		/// <summary>
		/// Enter the description of your new custom indicator here
		/// </summary>
		/// <returns></returns>
		public EMAofATR EMAofATR(double multiplier, int period)
		{
			return EMAofATR(Input, multiplier, period);
		}

		/// <summary>
		/// Enter the description of your new custom indicator here
		/// </summary>
		/// <returns></returns>
		public EMAofATR EMAofATR(Data.IDataSeries input, double multiplier, int period)
		{
			if (cacheEMAofATR != null)
				for (int idx = 0; idx < cacheEMAofATR.Length; idx++)
					if (Math.Abs(cacheEMAofATR[idx].Multiplier - multiplier) <= double.Epsilon && cacheEMAofATR[idx].Period == period && cacheEMAofATR[idx].EqualsInput(input))
						return cacheEMAofATR[idx];

			lock (checkEMAofATR)
			{
				checkEMAofATR.Multiplier = multiplier;
				multiplier = checkEMAofATR.Multiplier;
				checkEMAofATR.Period = period;
				period = checkEMAofATR.Period;

				if (cacheEMAofATR != null)
					for (int idx = 0; idx < cacheEMAofATR.Length; idx++)
						if (Math.Abs(cacheEMAofATR[idx].Multiplier - multiplier) <= double.Epsilon && cacheEMAofATR[idx].Period == period && cacheEMAofATR[idx].EqualsInput(input))
							return cacheEMAofATR[idx];

				EMAofATR indicator = new EMAofATR();
				indicator.BarsRequired = BarsRequired;
				indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
				indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
				indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
				indicator.Input = input;
				indicator.Multiplier = multiplier;
				indicator.Period = period;
				Indicators.Add(indicator);
				indicator.SetUp();

				EMAofATR[] tmp = new EMAofATR[cacheEMAofATR == null ? 1 : cacheEMAofATR.Length + 1];
				if (cacheEMAofATR != null)
					cacheEMAofATR.CopyTo(tmp, 0);
				tmp[tmp.Length - 1] = indicator;
				cacheEMAofATR = tmp;
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
		/// Enter the description of your new custom indicator here
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.EMAofATR EMAofATR(double multiplier, int period)
		{
			return _indicator.EMAofATR(Input, multiplier, period);
		}

		/// <summary>
		/// Enter the description of your new custom indicator here
		/// </summary>
		/// <returns></returns>
		public Indicator.EMAofATR EMAofATR(Data.IDataSeries input, double multiplier, int period)
		{
			return _indicator.EMAofATR(input, multiplier, period);
		}
	}
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
	public partial class Strategy : StrategyBase
	{
		/// <summary>
		/// Enter the description of your new custom indicator here
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.EMAofATR EMAofATR(double multiplier, int period)
		{
			return _indicator.EMAofATR(Input, multiplier, period);
		}

		/// <summary>
		/// Enter the description of your new custom indicator here
		/// </summary>
		/// <returns></returns>
		public Indicator.EMAofATR EMAofATR(Data.IDataSeries input, double multiplier, int period)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return _indicator.EMAofATR(input, multiplier, period);
		}
	}
}
#endregion
