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
    public class AmazingCrossoverRSI : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int rSIPeriod = 10; // Default setting for RSIPeriod
        // User defined variables (add any user defined variables below)
		private RSI _rsi;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "RSIPlot"));
			Add(new Line(Color.Gray, 50, "Middle"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (_rsi == null) 
				_rsi = RSI(Median, rSIPeriod, 0);	
			
            RSIPlot.Set(_rsi[0]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries RSIPlot
        {
            get { return Values[0]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int RSIPeriod
        {
            get { return rSIPeriod; }
            set { rSIPeriod = Math.Max(1, value); }
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
        private AmazingCrossoverRSI[] cacheAmazingCrossoverRSI = null;

        private static AmazingCrossoverRSI checkAmazingCrossoverRSI = new AmazingCrossoverRSI();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public AmazingCrossoverRSI AmazingCrossoverRSI(int rSIPeriod)
        {
            return AmazingCrossoverRSI(Input, rSIPeriod);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public AmazingCrossoverRSI AmazingCrossoverRSI(Data.IDataSeries input, int rSIPeriod)
        {
            if (cacheAmazingCrossoverRSI != null)
                for (int idx = 0; idx < cacheAmazingCrossoverRSI.Length; idx++)
                    if (cacheAmazingCrossoverRSI[idx].RSIPeriod == rSIPeriod && cacheAmazingCrossoverRSI[idx].EqualsInput(input))
                        return cacheAmazingCrossoverRSI[idx];

            lock (checkAmazingCrossoverRSI)
            {
                checkAmazingCrossoverRSI.RSIPeriod = rSIPeriod;
                rSIPeriod = checkAmazingCrossoverRSI.RSIPeriod;

                if (cacheAmazingCrossoverRSI != null)
                    for (int idx = 0; idx < cacheAmazingCrossoverRSI.Length; idx++)
                        if (cacheAmazingCrossoverRSI[idx].RSIPeriod == rSIPeriod && cacheAmazingCrossoverRSI[idx].EqualsInput(input))
                            return cacheAmazingCrossoverRSI[idx];

                AmazingCrossoverRSI indicator = new AmazingCrossoverRSI();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.RSIPeriod = rSIPeriod;
                Indicators.Add(indicator);
                indicator.SetUp();

                AmazingCrossoverRSI[] tmp = new AmazingCrossoverRSI[cacheAmazingCrossoverRSI == null ? 1 : cacheAmazingCrossoverRSI.Length + 1];
                if (cacheAmazingCrossoverRSI != null)
                    cacheAmazingCrossoverRSI.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheAmazingCrossoverRSI = tmp;
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
        public Indicator.AmazingCrossoverRSI AmazingCrossoverRSI(int rSIPeriod)
        {
            return _indicator.AmazingCrossoverRSI(Input, rSIPeriod);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.AmazingCrossoverRSI AmazingCrossoverRSI(Data.IDataSeries input, int rSIPeriod)
        {
            return _indicator.AmazingCrossoverRSI(input, rSIPeriod);
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
        public Indicator.AmazingCrossoverRSI AmazingCrossoverRSI(int rSIPeriod)
        {
            return _indicator.AmazingCrossoverRSI(Input, rSIPeriod);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.AmazingCrossoverRSI AmazingCrossoverRSI(Data.IDataSeries input, int rSIPeriod)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.AmazingCrossoverRSI(input, rSIPeriod);
        }
    }
}
#endregion
