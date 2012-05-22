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
// Code provided by 'total3', compiled by 'Elliot Wave'.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Zero lag version of the MACD indicator
	/// Updated by Big Mike (ctrlbrk) 05/23/2009 http://ctrlbrk.blogspot.com
	/// Updated again 6/1/09 to fix some bugs
    /// </summary>
    [Description("ZeroLag MACD with enhanced visual coloring")]
	[Gui.Design.DisplayName("MACD ZeroLag w/Colors")]
    public class MACD_ZeroLag_Colors : Indicator
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below)
		
		private double acceleration = 1;  // typical values might be 0.5, 1, 1.5, or 2.0
		private int fast = 12;
		private int slow = 26;
		private int smooth = 9;
		private double threshold = 0;  // amount past zero line it needs to cross to trigger
		
		private DataSeries fastEma;
		private DataSeries diffArr;
        private DataSeries macdAvg2;
		
		private DataSeries signal;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(new Pen(Color.LightSteelBlue, 2), "Macd"));					// plot 0
			Add(new Plot(new Pen(Color.RoyalBlue, 2), PlotStyle.Hash, "MacdUp"));		// plot 1 
			Add(new Plot(new Pen(Color.DarkRed, 2), PlotStyle.Hash, "MacdDn"));			// plot 2
			Add(new Plot(new Pen(Color.Yellow, 2), PlotStyle.Hash, "MacdNeutral"));		// plot 3
			Add(new Plot(new Pen(Color.DarkViolet, 2), "Avg"));							// plot 4
			Add(new Plot(new Pen(Color.DimGray, 2), PlotStyle.Bar, "Diff"));			// plot 5
			Add(new Plot(new Pen(Color.Teal, 2), PlotStyle.Line, "ADX"));				// plot 6
			
			Plots[0].Pen.Width = 3;
			Plots[1].Pen.Width = 6;
			Plots[2].Pen.Width = 6;
			Plots[3].Pen.Width = 6;
			Plots[4].Pen.Width = 1;
			Plots[5].Pen.Width = 2;
			Plots[6].Pen.Width = 2;
			
			Plots[4].Pen.DashStyle = DashStyle.Dash;
			
			fastEma = new DataSeries(this);
			macdAvg2 = new DataSeries(this);
			diffArr = new DataSeries(this);
			signal = new DataSeries(this);
			
			Overlay				= false;
            PriceTypeSupported	= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            if (CurrentBar == 0)
			{
				fastEma.Set(Input[0]);
				macdAvg2.Set(Input[0]);
				Value.Set(0);
				Avg.Set(0);
				Diff.Set(0);
			}
			else
			{
				fastEma.Set((ZeroLagEMA(Input, (int)(Fast / Acceleration))[0]) - (ZeroLagEMA(Input, (int)(Slow / Acceleration))[0]));
				double macd = fastEma[0] ;
				
				macdAvg2.Set(ZeroLagEMA(fastEma,Smooth)[0]);
				double macdAvg = macdAvg2[0];
				
				Value.Set(macd);
				Avg.Set(macdAvg);
				ADX.Set(ADXVMA(Value, (int)(Fast / Acceleration))[0]);
				
				Diff.Set(macd - macdAvg);
				
				//Print (Time[0] + ": Value = " + Value[0].ToString("0.00") + ", Avg = " + Avg[0].ToString("0.00") + ", ADX = " + ADX[0].ToString("0.00"));
					
				if ((Value[0] > ADX[0]) && (Value[0] > Threshold))
				{ 
					MacdUp.Set(0); 
					MacdDn.Reset();
					MacdNeutral.Reset();
					signal.Set(1); 
				}
				else 
					if ((Value[0] < ADX[0]) && (Value[0] < -Threshold))
					{ 
						MacdDn.Set(0);
						MacdUp.Reset();
						MacdNeutral.Reset();
						signal.Set(-1); 
					}
					else
					{ 
						MacdNeutral.Set(0); 
						MacdDn.Reset();
						MacdUp.Reset();
						signal.Set(0); 
					}
			}
        }

        #region Properties
		/// <summary>
		/// </summary>
		/// 
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries MacdUp
		{
		get { return Values[1]; }
		}
		
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries MacdDn
		{
		get { return Values[2]; }
		}
		
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries MacdNeutral
		{
		get { return Values[3]; }
		}
		
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Avg
		{
		get { return Values[4]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Default
		{
		get { return Values[0]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Diff
		{
		get { return Values[5]; }
		}
		
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries ADX
		{
		get { return Values[6]; }
		}
		
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Signal
		{
		get { return signal; }
		}
		
		/// <summary>
		/// </summary>
		[Description("Number of bars for fast EMA")]
		[Category("Parameters")]
		public int Fast
		{
		get { return fast; }
		set { fast = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("Number of bars for slow EMA")]
		[Category("Parameters")]
		public int Slow
		{
		get { return slow; }
		set { slow = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("Number of bars for smoothing")]
		[Category("Parameters")]
		public int Smooth
		{
		get { return smooth; }
		set { smooth = Math.Max(1, value); }
		}
		
		[Description("Threshold for zero line")]
		[Category("Parameters")]
		public double Threshold
		{
		get { return threshold; }
		set { threshold = value; }
		}
		
		[Description("Acceleration of fast and slow")]
		[Category("Parameters")]
		public double Acceleration
		{
		get { return acceleration; }
		set { acceleration = value; }
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
        private MACD_ZeroLag_Colors[] cacheMACD_ZeroLag_Colors = null;

        private static MACD_ZeroLag_Colors checkMACD_ZeroLag_Colors = new MACD_ZeroLag_Colors();

        /// <summary>
        /// ZeroLag MACD with enhanced visual coloring
        /// </summary>
        /// <returns></returns>
        public MACD_ZeroLag_Colors MACD_ZeroLag_Colors(double acceleration, int fast, int slow, int smooth, double threshold)
        {
            return MACD_ZeroLag_Colors(Input, acceleration, fast, slow, smooth, threshold);
        }

        /// <summary>
        /// ZeroLag MACD with enhanced visual coloring
        /// </summary>
        /// <returns></returns>
        public MACD_ZeroLag_Colors MACD_ZeroLag_Colors(Data.IDataSeries input, double acceleration, int fast, int slow, int smooth, double threshold)
        {
            if (cacheMACD_ZeroLag_Colors != null)
                for (int idx = 0; idx < cacheMACD_ZeroLag_Colors.Length; idx++)
                    if (Math.Abs(cacheMACD_ZeroLag_Colors[idx].Acceleration - acceleration) <= double.Epsilon && cacheMACD_ZeroLag_Colors[idx].Fast == fast && cacheMACD_ZeroLag_Colors[idx].Slow == slow && cacheMACD_ZeroLag_Colors[idx].Smooth == smooth && Math.Abs(cacheMACD_ZeroLag_Colors[idx].Threshold - threshold) <= double.Epsilon && cacheMACD_ZeroLag_Colors[idx].EqualsInput(input))
                        return cacheMACD_ZeroLag_Colors[idx];

            lock (checkMACD_ZeroLag_Colors)
            {
                checkMACD_ZeroLag_Colors.Acceleration = acceleration;
                acceleration = checkMACD_ZeroLag_Colors.Acceleration;
                checkMACD_ZeroLag_Colors.Fast = fast;
                fast = checkMACD_ZeroLag_Colors.Fast;
                checkMACD_ZeroLag_Colors.Slow = slow;
                slow = checkMACD_ZeroLag_Colors.Slow;
                checkMACD_ZeroLag_Colors.Smooth = smooth;
                smooth = checkMACD_ZeroLag_Colors.Smooth;
                checkMACD_ZeroLag_Colors.Threshold = threshold;
                threshold = checkMACD_ZeroLag_Colors.Threshold;

                if (cacheMACD_ZeroLag_Colors != null)
                    for (int idx = 0; idx < cacheMACD_ZeroLag_Colors.Length; idx++)
                        if (Math.Abs(cacheMACD_ZeroLag_Colors[idx].Acceleration - acceleration) <= double.Epsilon && cacheMACD_ZeroLag_Colors[idx].Fast == fast && cacheMACD_ZeroLag_Colors[idx].Slow == slow && cacheMACD_ZeroLag_Colors[idx].Smooth == smooth && Math.Abs(cacheMACD_ZeroLag_Colors[idx].Threshold - threshold) <= double.Epsilon && cacheMACD_ZeroLag_Colors[idx].EqualsInput(input))
                            return cacheMACD_ZeroLag_Colors[idx];

                MACD_ZeroLag_Colors indicator = new MACD_ZeroLag_Colors();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Acceleration = acceleration;
                indicator.Fast = fast;
                indicator.Slow = slow;
                indicator.Smooth = smooth;
                indicator.Threshold = threshold;
                Indicators.Add(indicator);
                indicator.SetUp();

                MACD_ZeroLag_Colors[] tmp = new MACD_ZeroLag_Colors[cacheMACD_ZeroLag_Colors == null ? 1 : cacheMACD_ZeroLag_Colors.Length + 1];
                if (cacheMACD_ZeroLag_Colors != null)
                    cacheMACD_ZeroLag_Colors.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheMACD_ZeroLag_Colors = tmp;
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
        /// ZeroLag MACD with enhanced visual coloring
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MACD_ZeroLag_Colors MACD_ZeroLag_Colors(double acceleration, int fast, int slow, int smooth, double threshold)
        {
            return _indicator.MACD_ZeroLag_Colors(Input, acceleration, fast, slow, smooth, threshold);
        }

        /// <summary>
        /// ZeroLag MACD with enhanced visual coloring
        /// </summary>
        /// <returns></returns>
        public Indicator.MACD_ZeroLag_Colors MACD_ZeroLag_Colors(Data.IDataSeries input, double acceleration, int fast, int slow, int smooth, double threshold)
        {
            return _indicator.MACD_ZeroLag_Colors(input, acceleration, fast, slow, smooth, threshold);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// ZeroLag MACD with enhanced visual coloring
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MACD_ZeroLag_Colors MACD_ZeroLag_Colors(double acceleration, int fast, int slow, int smooth, double threshold)
        {
            return _indicator.MACD_ZeroLag_Colors(Input, acceleration, fast, slow, smooth, threshold);
        }

        /// <summary>
        /// ZeroLag MACD with enhanced visual coloring
        /// </summary>
        /// <returns></returns>
        public Indicator.MACD_ZeroLag_Colors MACD_ZeroLag_Colors(Data.IDataSeries input, double acceleration, int fast, int slow, int smooth, double threshold)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.MACD_ZeroLag_Colors(input, acceleration, fast, slow, smooth, threshold);
        }
    }
}
#endregion
