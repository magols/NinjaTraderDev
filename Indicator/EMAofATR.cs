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
		// User defined variables (add any user defined variables below)
		#endregion

		private ATR _atr = null;
		private EMA _ema = null;
		
		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "ATRPlot"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "EMAPlot"));
			Overlay				= false;
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
				_ema = EMA(_atr, period);

			if (_atr == null & _ema == null) 
				return;
			
			
			// Use this method for calculating your indicator values. Assign a value to each
			// plot below by replacing 'Close[0]' with your own formula.
			ATRPlot.Set(_atr[0]);
			EMAPlot.Set(_ema[0]);
		}

		#region Properties
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries ATRPlot
		{
			get { return Values[0]; }
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries EMAPlot
		{
			get { return Values[1]; }
		}

		[Description("Perio of atr and ema")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
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
		public EMAofATR EMAofATR(int period)
		{
			return EMAofATR(Input, period);
		}

		/// <summary>
		/// Enter the description of your new custom indicator here
		/// </summary>
		/// <returns></returns>
		public EMAofATR EMAofATR(Data.IDataSeries input, int period)
		{
			if (cacheEMAofATR != null)
				for (int idx = 0; idx < cacheEMAofATR.Length; idx++)
					if (cacheEMAofATR[idx].Period == period && cacheEMAofATR[idx].EqualsInput(input))
						return cacheEMAofATR[idx];

			lock (checkEMAofATR)
			{
				checkEMAofATR.Period = period;
				period = checkEMAofATR.Period;

				if (cacheEMAofATR != null)
					for (int idx = 0; idx < cacheEMAofATR.Length; idx++)
						if (cacheEMAofATR[idx].Period == period && cacheEMAofATR[idx].EqualsInput(input))
							return cacheEMAofATR[idx];

				EMAofATR indicator = new EMAofATR();
				indicator.BarsRequired = BarsRequired;
				indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
				indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
				indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
				indicator.Input = input;
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
		public Indicator.EMAofATR EMAofATR(int period)
		{
			return _indicator.EMAofATR(Input, period);
		}

		/// <summary>
		/// Enter the description of your new custom indicator here
		/// </summary>
		/// <returns></returns>
		public Indicator.EMAofATR EMAofATR(Data.IDataSeries input, int period)
		{
			return _indicator.EMAofATR(input, period);
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
		public Indicator.EMAofATR EMAofATR(int period)
		{
			return _indicator.EMAofATR(Input, period);
		}

		/// <summary>
		/// Enter the description of your new custom indicator here
		/// </summary>
		/// <returns></returns>
		public Indicator.EMAofATR EMAofATR(Data.IDataSeries input, int period)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return _indicator.EMAofATR(input, period);
		}
	}
}
#endregion
