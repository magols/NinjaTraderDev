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
    public class GeekoParaTrend : Indicator
    {

        private bool DEBUG = false;

        #region Variables

        private double acceleration = 0.02;
        private double accelerationStep = 0.02;
        private double accelerationMax = 0.2;

        private int periodMinutesSlow = 120; // Default setting for MyInput0
        private int periodMinutesFast = 15; // Default setting for MyInput0

        private TrendType currTrendGekko = TrendType.NEUTRAL;
        private TrendType currTrendDaily = TrendType.NEUTRAL;
        private TrendType currTrendSLow = TrendType.NEUTRAL;
        private TrendType currTrendFast = TrendType.NEUTRAL;

        private bool testprint = false;
        #endregion

        #region enums
        public enum TrendType
        {
            DOWN = -1,
            NEUTRAL = 0,
            UP = 1
        } ;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(PeriodType.Day, 1);
            Add(PeriodType.Minute, periodMinutesSlow);
            Add(PeriodType.Minute, periodMinutesFast);

            Add(new Plot(Color.Brown, PlotStyle.Dot, "gekkotrend"));
            Add(new Plot(Color.LightGreen, PlotStyle.Dot, "psardaily"));
            Add(new Plot(Color.Blue, PlotStyle.Dot, "psarslow"));
            Add(new Plot(Color.Red, PlotStyle.Dot, "psarfast"));

            BarsRequired = 5;
            Overlay = false;

        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // check for enough data to start rocking!
            if (CurrentBars[0] <= BarsRequired || CurrentBars[1] <= BarsRequired || CurrentBars[2] <= BarsRequired) return;


            switch (BarsInProgress)
            {
                case 1:
                    ProcessDaily();
                    break;
                case 2:
                   ProcessSlow();
                    break;
                case 3:
                    ProcessFast();
                    break;
                default:
                    
                    DoPlot();
                    //if (!testprint && currTrendDaily == TrendType.UP && currTrendSLow == TrendType.UP && currTrendFast == TrendType.UP)
                    //{
                    //    Print("UP! " + Time[0]);
                    //    testprint = true;
                    //}
                    //else
                    //{
                    //    testprint = false;
                    //}
                    //if (!testprint && currTrendDaily == TrendType.DOWN && currTrendSLow == TrendType.DOWN  && currTrendFast == TrendType.DOWN)
                    //{
                    //    Print("DOWN! " + Time[0]);
                    //    testprint = true;
                    //}
                    //else
                    //{
                    //    testprint = false;
                    //}

                    break;
            }


        }

        private void DoPlot()
        {
            PlotDaily.Set((double)currTrendDaily);
            PlotSlow.Set((double)currTrendSLow*0.75);
            PlotFast.Set((double)currTrendFast*0.5);
            PlotGekkoTrend.Set(PlotDaily[0] + PlotSlow[0] + PlotFast[0]);
        }

        private void ProcessDaily()
        {
            Log("DAILY, bar " + CurrentBar);
            ParabolicSAR p = ParabolicSAR(BarsArray[1], Acceleration, AccelerationMax, AccelerationStep);
            currTrendDaily = p[0] > Closes[1][0] ? TrendType.UP : TrendType.DOWN;
            
        }

        private void ProcessSlow()
        {
            Log("SLOW, bar " + CurrentBar);

            ParabolicSAR p = ParabolicSAR(BarsArray[2], Acceleration, AccelerationMax, AccelerationStep);
            currTrendSLow = p[0] > Closes[2][0] ? TrendType.UP : TrendType.DOWN;
            
        }

        private void ProcessFast()
        {
            Log("bar " + CurrentBar);

            ParabolicSAR p = ParabolicSAR(BarsArray[3], Acceleration, AccelerationMax, AccelerationStep);
            currTrendFast = p[0] > Closes[3][0] ? TrendType.UP : TrendType.DOWN;
            
        }

        private void Log(string msg)
        {
            if (DEBUG)
                Print(msg);
        }

        #region Properties

        /// <summary>
        /// The initial acceleration factor
        /// </summary>
        [Description("The initial acceleration factor")]
        [GridCategory("Parameters")]
        public double Acceleration
        {
            get { return acceleration; }
            set { acceleration = Math.Max(0.00, value); }
        }

        /// <summary>
        /// The acceleration step factor
        /// </summary>
        [Description("The acceleration step factor")]
        [GridCategory("Parameters")]
        [Gui.Design.DisplayNameAttribute("Acceleration step")]
        public double AccelerationStep
        {
            get { return accelerationStep; }
            set { accelerationStep = Math.Max(0.02, value); }
        }

        /// <summary>
        /// The maximum acceleration
        /// </summary>
        [Description("The maximum acceleration")]
        [GridCategory("Parameters")]
        [Gui.Design.DisplayNameAttribute("Acceleration max")]
        public double AccelerationMax
        {
            get { return accelerationMax; }
            set { accelerationMax = Math.Max(0.02, value); }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PlotGekkoTrend
        {
            get { return Values[0]; }
           
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PlotDaily
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PlotSlow
        {
            get { return Values[2]; }

        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PlotFast
        {
            get { return Values[3]; }

        }

        [Description("Minutes for slow PSAR")]
        [GridCategory("Parameters")]
        public int PeriodSlow
        {
            get { return periodMinutesSlow; }
            set
            {
                periodMinutesSlow = Math.Max(15, value);
            }
        }
        [Description("Minutes for fast PSAR")]
        [GridCategory("Parameters")]
        public int PeriodFast
        {
            get { return periodMinutesFast; }
            set
            {
                
                periodMinutesFast = Math.Max(15, value);
            }
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
        private GeekoParaTrend[] cacheGeekoParaTrend = null;

        private static GeekoParaTrend checkGeekoParaTrend = new GeekoParaTrend();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public GeekoParaTrend GeekoParaTrend(double acceleration, double accelerationMax, double accelerationStep, int periodFast, int periodSlow)
        {
            return GeekoParaTrend(Input, acceleration, accelerationMax, accelerationStep, periodFast, periodSlow);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public GeekoParaTrend GeekoParaTrend(Data.IDataSeries input, double acceleration, double accelerationMax, double accelerationStep, int periodFast, int periodSlow)
        {
            if (cacheGeekoParaTrend != null)
                for (int idx = 0; idx < cacheGeekoParaTrend.Length; idx++)
                    if (Math.Abs(cacheGeekoParaTrend[idx].Acceleration - acceleration) <= double.Epsilon && Math.Abs(cacheGeekoParaTrend[idx].AccelerationMax - accelerationMax) <= double.Epsilon && Math.Abs(cacheGeekoParaTrend[idx].AccelerationStep - accelerationStep) <= double.Epsilon && cacheGeekoParaTrend[idx].PeriodFast == periodFast && cacheGeekoParaTrend[idx].PeriodSlow == periodSlow && cacheGeekoParaTrend[idx].EqualsInput(input))
                        return cacheGeekoParaTrend[idx];

            lock (checkGeekoParaTrend)
            {
                checkGeekoParaTrend.Acceleration = acceleration;
                acceleration = checkGeekoParaTrend.Acceleration;
                checkGeekoParaTrend.AccelerationMax = accelerationMax;
                accelerationMax = checkGeekoParaTrend.AccelerationMax;
                checkGeekoParaTrend.AccelerationStep = accelerationStep;
                accelerationStep = checkGeekoParaTrend.AccelerationStep;
                checkGeekoParaTrend.PeriodFast = periodFast;
                periodFast = checkGeekoParaTrend.PeriodFast;
                checkGeekoParaTrend.PeriodSlow = periodSlow;
                periodSlow = checkGeekoParaTrend.PeriodSlow;

                if (cacheGeekoParaTrend != null)
                    for (int idx = 0; idx < cacheGeekoParaTrend.Length; idx++)
                        if (Math.Abs(cacheGeekoParaTrend[idx].Acceleration - acceleration) <= double.Epsilon && Math.Abs(cacheGeekoParaTrend[idx].AccelerationMax - accelerationMax) <= double.Epsilon && Math.Abs(cacheGeekoParaTrend[idx].AccelerationStep - accelerationStep) <= double.Epsilon && cacheGeekoParaTrend[idx].PeriodFast == periodFast && cacheGeekoParaTrend[idx].PeriodSlow == periodSlow && cacheGeekoParaTrend[idx].EqualsInput(input))
                            return cacheGeekoParaTrend[idx];

                GeekoParaTrend indicator = new GeekoParaTrend();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Acceleration = acceleration;
                indicator.AccelerationMax = accelerationMax;
                indicator.AccelerationStep = accelerationStep;
                indicator.PeriodFast = periodFast;
                indicator.PeriodSlow = periodSlow;
                Indicators.Add(indicator);
                indicator.SetUp();

                GeekoParaTrend[] tmp = new GeekoParaTrend[cacheGeekoParaTrend == null ? 1 : cacheGeekoParaTrend.Length + 1];
                if (cacheGeekoParaTrend != null)
                    cacheGeekoParaTrend.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheGeekoParaTrend = tmp;
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
        public Indicator.GeekoParaTrend GeekoParaTrend(double acceleration, double accelerationMax, double accelerationStep, int periodFast, int periodSlow)
        {
            return _indicator.GeekoParaTrend(Input, acceleration, accelerationMax, accelerationStep, periodFast, periodSlow);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.GeekoParaTrend GeekoParaTrend(Data.IDataSeries input, double acceleration, double accelerationMax, double accelerationStep, int periodFast, int periodSlow)
        {
            return _indicator.GeekoParaTrend(input, acceleration, accelerationMax, accelerationStep, periodFast, periodSlow);
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
        public Indicator.GeekoParaTrend GeekoParaTrend(double acceleration, double accelerationMax, double accelerationStep, int periodFast, int periodSlow)
        {
            return _indicator.GeekoParaTrend(Input, acceleration, accelerationMax, accelerationStep, periodFast, periodSlow);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.GeekoParaTrend GeekoParaTrend(Data.IDataSeries input, double acceleration, double accelerationMax, double accelerationStep, int periodFast, int periodSlow)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.GeekoParaTrend(input, acceleration, accelerationMax, accelerationStep, periodFast, periodSlow);
        }
    }
}
#endregion
