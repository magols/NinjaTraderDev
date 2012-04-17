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
    /// My specialized indicator
    /// </summary>
    [Description("My specialized indicator")]
    public class GekkoIndicator : Indicator
    {
        #region Variables
        // Wizard generated variables
        private int sMAPeriod = 240; // Default setting for SMAPeriod
        private int aTRPeriod = 5; // Default setting for ATRPeriod
        // User defined variables (add any user defined variables below)


        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "PlotSMA"));
            Overlay = true;
            CalculateOnBarClose = true;
            BarsRequired = 24;

            Add(PeriodType.Minute, 60);


        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {

            // OnBarUpdate() will be called on incoming tick events on all Bars objects added to the strategy
            // We only want to process events on our primary Bars object (index = 0) which is set when adding
            // the strategy to a chart
            //if (BarsInProgress != 0)
            //    return;

            if (CurrentBars[0] <= BarsRequired && CurrentBars[1] <= BarsRequired) return;
      //      if (CurrentBars[1] < 1) return;


            // Print("BarsinProgress: " + BarsInProgress + ", CurrentBar: " + CurrentBar + ", Closes[0]: "+ Closes[0][0]);

            double sma = SMA(BarsArray[1], SMAPeriod*24)[0];
            PlotSMA.Set(sma);







        }


        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PlotSMA
        {
            get { return Values[0]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int SMAPeriod
        {
            get { return sMAPeriod; }
            set { sMAPeriod = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int ATRPeriod
        {
            get { return aTRPeriod; }
            set { aTRPeriod = Math.Max(1, value); }
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
        private GekkoIndicator[] cacheGekkoIndicator = null;

        private static GekkoIndicator checkGekkoIndicator = new GekkoIndicator();

        /// <summary>
        /// My specialized indicator
        /// </summary>
        /// <returns></returns>
        public GekkoIndicator GekkoIndicator(int aTRPeriod, int sMAPeriod)
        {
            return GekkoIndicator(Input, aTRPeriod, sMAPeriod);
        }

        /// <summary>
        /// My specialized indicator
        /// </summary>
        /// <returns></returns>
        public GekkoIndicator GekkoIndicator(Data.IDataSeries input, int aTRPeriod, int sMAPeriod)
        {
            if (cacheGekkoIndicator != null)
                for (int idx = 0; idx < cacheGekkoIndicator.Length; idx++)
                    if (cacheGekkoIndicator[idx].ATRPeriod == aTRPeriod && cacheGekkoIndicator[idx].SMAPeriod == sMAPeriod && cacheGekkoIndicator[idx].EqualsInput(input))
                        return cacheGekkoIndicator[idx];

            lock (checkGekkoIndicator)
            {
                checkGekkoIndicator.ATRPeriod = aTRPeriod;
                aTRPeriod = checkGekkoIndicator.ATRPeriod;
                checkGekkoIndicator.SMAPeriod = sMAPeriod;
                sMAPeriod = checkGekkoIndicator.SMAPeriod;

                if (cacheGekkoIndicator != null)
                    for (int idx = 0; idx < cacheGekkoIndicator.Length; idx++)
                        if (cacheGekkoIndicator[idx].ATRPeriod == aTRPeriod && cacheGekkoIndicator[idx].SMAPeriod == sMAPeriod && cacheGekkoIndicator[idx].EqualsInput(input))
                            return cacheGekkoIndicator[idx];

                GekkoIndicator indicator = new GekkoIndicator();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ATRPeriod = aTRPeriod;
                indicator.SMAPeriod = sMAPeriod;
                Indicators.Add(indicator);
                indicator.SetUp();

                GekkoIndicator[] tmp = new GekkoIndicator[cacheGekkoIndicator == null ? 1 : cacheGekkoIndicator.Length + 1];
                if (cacheGekkoIndicator != null)
                    cacheGekkoIndicator.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheGekkoIndicator = tmp;
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
        /// My specialized indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.GekkoIndicator GekkoIndicator(int aTRPeriod, int sMAPeriod)
        {
            return _indicator.GekkoIndicator(Input, aTRPeriod, sMAPeriod);
        }

        /// <summary>
        /// My specialized indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.GekkoIndicator GekkoIndicator(Data.IDataSeries input, int aTRPeriod, int sMAPeriod)
        {
            return _indicator.GekkoIndicator(input, aTRPeriod, sMAPeriod);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// My specialized indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.GekkoIndicator GekkoIndicator(int aTRPeriod, int sMAPeriod)
        {
            return _indicator.GekkoIndicator(Input, aTRPeriod, sMAPeriod);
        }

        /// <summary>
        /// My specialized indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.GekkoIndicator GekkoIndicator(Data.IDataSeries input, int aTRPeriod, int sMAPeriod)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.GekkoIndicator(input, aTRPeriod, sMAPeriod);
        }
    }
}
#endregion
