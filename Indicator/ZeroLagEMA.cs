#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Zero-Lagging Exponential Moving Average
    /// </summary>
    [Description("Zero-Lagging Exponential Moving Average")]
    public class ZeroLagEMA : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period = 20; // Default setting for Period
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.OrangeRed), PlotStyle.Line, "ZLEMA"));
            //CalculateOnBarClose	= true;
            Overlay				= true;
            PriceTypeSupported	= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			EMA ema1 = EMA(Input, Period);
			double difference = ema1[0] - EMA(ema1, Period)[0];
            ZLEMA.Set(ema1[0] + difference);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ZLEMA
        {
            get { return Values[0]; }
        }

        [Description("")]
        [Category("Parameters")]
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
        private ZeroLagEMA[] cacheZeroLagEMA = null;

        private static ZeroLagEMA checkZeroLagEMA = new ZeroLagEMA();

        /// <summary>
        /// Zero-Lagging Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        public ZeroLagEMA ZeroLagEMA(int period)
        {
            return ZeroLagEMA(Input, period);
        }

        /// <summary>
        /// Zero-Lagging Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        public ZeroLagEMA ZeroLagEMA(Data.IDataSeries input, int period)
        {
            if (cacheZeroLagEMA != null)
                for (int idx = 0; idx < cacheZeroLagEMA.Length; idx++)
                    if (cacheZeroLagEMA[idx].Period == period && cacheZeroLagEMA[idx].EqualsInput(input))
                        return cacheZeroLagEMA[idx];

            lock (checkZeroLagEMA)
            {
                checkZeroLagEMA.Period = period;
                period = checkZeroLagEMA.Period;

                if (cacheZeroLagEMA != null)
                    for (int idx = 0; idx < cacheZeroLagEMA.Length; idx++)
                        if (cacheZeroLagEMA[idx].Period == period && cacheZeroLagEMA[idx].EqualsInput(input))
                            return cacheZeroLagEMA[idx];

                ZeroLagEMA indicator = new ZeroLagEMA();
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

                ZeroLagEMA[] tmp = new ZeroLagEMA[cacheZeroLagEMA == null ? 1 : cacheZeroLagEMA.Length + 1];
                if (cacheZeroLagEMA != null)
                    cacheZeroLagEMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZeroLagEMA = tmp;
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
        /// Zero-Lagging Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZeroLagEMA ZeroLagEMA(int period)
        {
            return _indicator.ZeroLagEMA(Input, period);
        }

        /// <summary>
        /// Zero-Lagging Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.ZeroLagEMA ZeroLagEMA(Data.IDataSeries input, int period)
        {
            return _indicator.ZeroLagEMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Zero-Lagging Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZeroLagEMA ZeroLagEMA(int period)
        {
            return _indicator.ZeroLagEMA(Input, period);
        }

        /// <summary>
        /// Zero-Lagging Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.ZeroLagEMA ZeroLagEMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZeroLagEMA(input, period);
        }
    }
}
#endregion
