#region Using declarations
using System;
using System.Collections.Generic;
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
    public class GekkoTrendByMAIndicator : Indicator
    {
        #region Variables
        // Wizard generated variables
        private int sMAPeriod1 = 10; // Default setting for SMAPeriod
        private int sMAPeriod2 = 20; // Default setting for SMAPeriod
        private int aTRPeriod = 5; // Default setting for ATRPeriod
        // User defined variables (add any user defined variables below)

        Font fontTrend = new Font("Arial", 14);
        private double sma1;
        private double sma2;


        private TrendType currentOverallTrend = TrendType.NEUTRAL;


        #endregion

        #region enums
        public enum TrendType
        {
            DOWN = -1,
            NEUTRAL,
            UP
        } ;
        #endregion


        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkOrange), PlotStyle.Line, "PlotSMA1"));
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkMagenta), PlotStyle.Line, "PlotSMA2"));
            Overlay = true;

            

            // add a 60 minute plot
            Add(PeriodType.Minute, 60);

            
            Print(BarsPeriods[0].ToString());
            Print(BarsPeriods[1].ToString());
            
            BarsRequired = 15;
            CalculateOnBarClose = false;
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

            if (CurrentBars[0] <= BarsRequired || CurrentBars[1] <= BarsRequired) return;
            //if (CurrentBars[0] < 1) return;
            //if (CurrentBars[1] < 1) return;

       //    Print("BarsinProgress: " + BarsInProgress + ", CurrentBar: " + CurrentBar + ", Closes[0]: "+ Closes[0][0]);

        
            //sma2 = SMA(BarsArray[1], SMAPeriod2*24)[0];
            //PlotSMA2.Set(sma2);

            if (BarsInProgress == 1)
            {

                sma1 = SMA(Closes[1], SMAPeriod1 * 24)[0];
                
                double threshold = 0.000;
                //   double threshold = 0.000;

                if (PlotSMA1[0] + threshold < PlotSMA1[1])
                {
                    currentOverallTrend = TrendType.DOWN;
                }
                else if (PlotSMA1[0] > PlotSMA1[1] + threshold)
                {
                    currentOverallTrend = TrendType.UP;
                }
                else
                {
                    currentOverallTrend = TrendType.NEUTRAL;
                }
            }



            // Draws a dotted lime green arrow line
            //   DrawArrowLine("tag1", true, 10, Close[10] * 1.05, 0, Close[0] * 1.05, Color.LimeGreen, DashStyle.Solid, 3);

            // only draw on primary series
            if (BarsInProgress == 0)
            {

                PlotSMA1.Set(sma1);

                switch (currentOverallTrend)
                {
                    case TrendType.DOWN:
                        DrawArrowDown("arrdown" + CurrentBar, false, 1, High[1] * 1.0008, Color.Red);
                        //   BackColor = Color.Pink;
                        break;

                    case TrendType.UP:
                        DrawArrowUp("arrup" + CurrentBar,1, Low[1] * 0.9992, Color.Blue);
                        //     BackColor = Color.LightGreen;
                        break;

                    default:
                        DrawArrowLine("arrneu" + CurrentBar, 2, Close[0], 1, Close[0], Color.Yellow);
                        break;
                }
            }



            // only draw on last bar
            if (Count - 2 == CurrentBar || Count-1 == CurrentBar)
            {
                switch (currentOverallTrend)
                {
                    case TrendType.DOWN:
                        DrawTextFixed("texttrend", "Trend: DOWN", TextPosition.TopRight, Color.Red, fontTrend, Color.Blue, Color.Black, 100);
                        break;
                    case TrendType.UP:
                        DrawTextFixed("texttrend", "Trend: UP", TextPosition.TopRight, Color.Lime, fontTrend, Color.Blue, Color.Black, 100);
                        break;
                    default:
                        break;
                }
            }









        }

        private void PaintDayOfWeekBackground()
        {
            List<DayOfWeek> days = new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday };
            if (days.Contains(Times[1][0].DayOfWeek))
            {
                BackColor = Color.Gray;
            }
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PlotSMA1
        {
            get { return Values[0]; }
        }
        public DataSeries PlotSMA2
        {
            get { return Values[1]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int SMAPeriod1
        {
            get { return sMAPeriod1; }
            set { sMAPeriod1 = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int SMAPeriod2
        {
            get { return sMAPeriod2; }
            set { sMAPeriod2 = Math.Max(1, value); }
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
        private GekkoTrendByMAIndicator[] cacheGekkoTrendByMAIndicator = null;

        private static GekkoTrendByMAIndicator checkGekkoTrendByMAIndicator = new GekkoTrendByMAIndicator();

        /// <summary>
        /// My specialized indicator
        /// </summary>
        /// <returns></returns>
        public GekkoTrendByMAIndicator GekkoTrendByMAIndicator(int aTRPeriod, int sMAPeriod1, int sMAPeriod2)
        {
            return GekkoTrendByMAIndicator(Input, aTRPeriod, sMAPeriod1, sMAPeriod2);
        }

        /// <summary>
        /// My specialized indicator
        /// </summary>
        /// <returns></returns>
        public GekkoTrendByMAIndicator GekkoTrendByMAIndicator(Data.IDataSeries input, int aTRPeriod, int sMAPeriod1, int sMAPeriod2)
        {
            if (cacheGekkoTrendByMAIndicator != null)
                for (int idx = 0; idx < cacheGekkoTrendByMAIndicator.Length; idx++)
                    if (cacheGekkoTrendByMAIndicator[idx].ATRPeriod == aTRPeriod && cacheGekkoTrendByMAIndicator[idx].SMAPeriod1 == sMAPeriod1 && cacheGekkoTrendByMAIndicator[idx].SMAPeriod2 == sMAPeriod2 && cacheGekkoTrendByMAIndicator[idx].EqualsInput(input))
                        return cacheGekkoTrendByMAIndicator[idx];

            lock (checkGekkoTrendByMAIndicator)
            {
                checkGekkoTrendByMAIndicator.ATRPeriod = aTRPeriod;
                aTRPeriod = checkGekkoTrendByMAIndicator.ATRPeriod;
                checkGekkoTrendByMAIndicator.SMAPeriod1 = sMAPeriod1;
                sMAPeriod1 = checkGekkoTrendByMAIndicator.SMAPeriod1;
                checkGekkoTrendByMAIndicator.SMAPeriod2 = sMAPeriod2;
                sMAPeriod2 = checkGekkoTrendByMAIndicator.SMAPeriod2;

                if (cacheGekkoTrendByMAIndicator != null)
                    for (int idx = 0; idx < cacheGekkoTrendByMAIndicator.Length; idx++)
                        if (cacheGekkoTrendByMAIndicator[idx].ATRPeriod == aTRPeriod && cacheGekkoTrendByMAIndicator[idx].SMAPeriod1 == sMAPeriod1 && cacheGekkoTrendByMAIndicator[idx].SMAPeriod2 == sMAPeriod2 && cacheGekkoTrendByMAIndicator[idx].EqualsInput(input))
                            return cacheGekkoTrendByMAIndicator[idx];

                GekkoTrendByMAIndicator indicator = new GekkoTrendByMAIndicator();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ATRPeriod = aTRPeriod;
                indicator.SMAPeriod1 = sMAPeriod1;
                indicator.SMAPeriod2 = sMAPeriod2;
                Indicators.Add(indicator);
                indicator.SetUp();

                GekkoTrendByMAIndicator[] tmp = new GekkoTrendByMAIndicator[cacheGekkoTrendByMAIndicator == null ? 1 : cacheGekkoTrendByMAIndicator.Length + 1];
                if (cacheGekkoTrendByMAIndicator != null)
                    cacheGekkoTrendByMAIndicator.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheGekkoTrendByMAIndicator = tmp;
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
        public Indicator.GekkoTrendByMAIndicator GekkoTrendByMAIndicator(int aTRPeriod, int sMAPeriod1, int sMAPeriod2)
        {
            return _indicator.GekkoTrendByMAIndicator(Input, aTRPeriod, sMAPeriod1, sMAPeriod2);
        }

        /// <summary>
        /// My specialized indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.GekkoTrendByMAIndicator GekkoTrendByMAIndicator(Data.IDataSeries input, int aTRPeriod, int sMAPeriod1, int sMAPeriod2)
        {
            return _indicator.GekkoTrendByMAIndicator(input, aTRPeriod, sMAPeriod1, sMAPeriod2);
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
        public Indicator.GekkoTrendByMAIndicator GekkoTrendByMAIndicator(int aTRPeriod, int sMAPeriod1, int sMAPeriod2)
        {
            return _indicator.GekkoTrendByMAIndicator(Input, aTRPeriod, sMAPeriod1, sMAPeriod2);
        }

        /// <summary>
        /// My specialized indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.GekkoTrendByMAIndicator GekkoTrendByMAIndicator(Data.IDataSeries input, int aTRPeriod, int sMAPeriod1, int sMAPeriod2)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.GekkoTrendByMAIndicator(input, aTRPeriod, sMAPeriod1, sMAPeriod2);
        }
    }
}
#endregion
