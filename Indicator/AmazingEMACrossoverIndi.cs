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
	public class AmazingEMACrossoverIndi : Indicator
	{
		#region Variables
		// Wizard generated variables
			private int fastEMAPeriod = 5; // Default setting for FastEMAPeriod
			private int slowEMAPeriod = 10; // Default setting for SlowEMAPeriod
		// User defined variables (add any user defined variables below)
		
		private EMA _emaSlow = null;
		private EMA _emaFast = null;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Gold), PlotStyle.Line, "SlowEMAPlot"));
			Add(new Plot(Color.FromKnownColor(KnownColor.OrangeRed), PlotStyle.Line, "FastEMAPlot"));
			Overlay				= true;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (_emaSlow == null) 
				_emaSlow = EMA(slowEMAPeriod);
			if (_emaFast == null) 
				_emaFast = EMA(fastEMAPeriod);
			
			FastEMAPlot.Set(_emaFast[0]);
			SlowEMAPlot.Set(_emaSlow[0]);
		}

		#region Properties
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries SlowEMAPlot
		{
			get { return Values[0]; }
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries FastEMAPlot
		{
			get { return Values[1]; }
		}

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
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	public partial class Indicator : IndicatorBase
	{
		private AmazingEMACrossoverIndi[] cacheAmazingEMACrossoverIndi = null;

		private static AmazingEMACrossoverIndi checkAmazingEMACrossoverIndi = new AmazingEMACrossoverIndi();

		/// <summary>
		/// Enter the description of your new custom indicator here
		/// </summary>
		/// <returns></returns>
		public AmazingEMACrossoverIndi AmazingEMACrossoverIndi(int fastEMAPeriod, int slowEMAPeriod)
		{
			return AmazingEMACrossoverIndi(Input, fastEMAPeriod, slowEMAPeriod);
		}

		/// <summary>
		/// Enter the description of your new custom indicator here
		/// </summary>
		/// <returns></returns>
		public AmazingEMACrossoverIndi AmazingEMACrossoverIndi(Data.IDataSeries input, int fastEMAPeriod, int slowEMAPeriod)
		{
			if (cacheAmazingEMACrossoverIndi != null)
				for (int idx = 0; idx < cacheAmazingEMACrossoverIndi.Length; idx++)
					if (cacheAmazingEMACrossoverIndi[idx].FastEMAPeriod == fastEMAPeriod && cacheAmazingEMACrossoverIndi[idx].SlowEMAPeriod == slowEMAPeriod && cacheAmazingEMACrossoverIndi[idx].EqualsInput(input))
						return cacheAmazingEMACrossoverIndi[idx];

			lock (checkAmazingEMACrossoverIndi)
			{
				checkAmazingEMACrossoverIndi.FastEMAPeriod = fastEMAPeriod;
				fastEMAPeriod = checkAmazingEMACrossoverIndi.FastEMAPeriod;
				checkAmazingEMACrossoverIndi.SlowEMAPeriod = slowEMAPeriod;
				slowEMAPeriod = checkAmazingEMACrossoverIndi.SlowEMAPeriod;

				if (cacheAmazingEMACrossoverIndi != null)
					for (int idx = 0; idx < cacheAmazingEMACrossoverIndi.Length; idx++)
						if (cacheAmazingEMACrossoverIndi[idx].FastEMAPeriod == fastEMAPeriod && cacheAmazingEMACrossoverIndi[idx].SlowEMAPeriod == slowEMAPeriod && cacheAmazingEMACrossoverIndi[idx].EqualsInput(input))
							return cacheAmazingEMACrossoverIndi[idx];

				AmazingEMACrossoverIndi indicator = new AmazingEMACrossoverIndi();
				indicator.BarsRequired = BarsRequired;
				indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
				indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
				indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
				indicator.Input = input;
				indicator.FastEMAPeriod = fastEMAPeriod;
				indicator.SlowEMAPeriod = slowEMAPeriod;
				Indicators.Add(indicator);
				indicator.SetUp();

				AmazingEMACrossoverIndi[] tmp = new AmazingEMACrossoverIndi[cacheAmazingEMACrossoverIndi == null ? 1 : cacheAmazingEMACrossoverIndi.Length + 1];
				if (cacheAmazingEMACrossoverIndi != null)
					cacheAmazingEMACrossoverIndi.CopyTo(tmp, 0);
				tmp[tmp.Length - 1] = indicator;
				cacheAmazingEMACrossoverIndi = tmp;
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
		public Indicator.AmazingEMACrossoverIndi AmazingEMACrossoverIndi(int fastEMAPeriod, int slowEMAPeriod)
		{
			return _indicator.AmazingEMACrossoverIndi(Input, fastEMAPeriod, slowEMAPeriod);
		}

		/// <summary>
		/// Enter the description of your new custom indicator here
		/// </summary>
		/// <returns></returns>
		public Indicator.AmazingEMACrossoverIndi AmazingEMACrossoverIndi(Data.IDataSeries input, int fastEMAPeriod, int slowEMAPeriod)
		{
			return _indicator.AmazingEMACrossoverIndi(input, fastEMAPeriod, slowEMAPeriod);
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
		public Indicator.AmazingEMACrossoverIndi AmazingEMACrossoverIndi(int fastEMAPeriod, int slowEMAPeriod)
		{
			return _indicator.AmazingEMACrossoverIndi(Input, fastEMAPeriod, slowEMAPeriod);
		}

		/// <summary>
		/// Enter the description of your new custom indicator here
		/// </summary>
		/// <returns></returns>
		public Indicator.AmazingEMACrossoverIndi AmazingEMACrossoverIndi(Data.IDataSeries input, int fastEMAPeriod, int slowEMAPeriod)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return _indicator.AmazingEMACrossoverIndi(input, fastEMAPeriod, slowEMAPeriod);
		}
	}
}
#endregion
