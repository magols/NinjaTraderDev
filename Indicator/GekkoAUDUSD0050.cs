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
    public class GekkoAUDUSD0050 : Indicator
    {
        #region Variables
        // Wizard generated variables
        private int _graceTicks = 5; 
            private int hours = 6; // Default setting for Hours
            private int stoploss = 50; // Default setting for Stoploss
            private int takeProfit = 50; // Default setting for TakeProfit
            private int trailStop = 10; // Default setting for TrailStop
        // User defined variables (add any user defined variables below)

        private double triggerPrice1 = 0.0100;
        private double triggerPrice2 = 0.0050;


        double boundaryLower, boundaryUpper;
        private decimal beginning;
        private decimal ending;

        private DateTime _triggerUpDateTime;
        private DateTime _triggerDownDateTime;
        private DateTime _confirmationUpDateTime, _confirmationDownDateTime;
        private double _triggerUpPrice, _triggerDownPrice;
        private double _confirmationUpPrice, _confirmationDownPrice;
        private decimal _currPrice;

        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {

            _triggerUpDateTime = DateTime.Now.Subtract(new TimeSpan(2, 0, 0)); 
            _triggerDownDateTime = DateTime.Now.Subtract(new TimeSpan(2, 0, 0));
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "PlotConfirmation"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Pink), PlotStyle.Line, "PlotUpper"));
            Add(new Plot(Color.FromKnownColor(KnownColor.LightGreen), PlotStyle.Line, "PlotLower"));
            BarsRequired = 20;
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            PlotConfirmation.Set(0);

            _currPrice = (decimal) (Open[0] + High[0] + Low[0] + Close[0])/4;

            TestForBoundaries();
            TestForTriggers();
            TestForConfirmations();
        }

        private void TestForConfirmations()
        {
            if (_triggerUpPrice == 0 || _triggerDownPrice == 0) return;

            DateTime lookbackTime = Time[0].Subtract(new TimeSpan(Hours, 0, 0));

            // check for highs over upper boundary
            // check for up trigger
            // check for timespan
            if (High[0] >= boundaryUpper && _triggerUpPrice == boundaryUpper-0.0050 && _triggerUpDateTime  >= lookbackTime)
            {
                _confirmationUpDateTime = Time[0];
                _confirmationUpPrice = boundaryUpper;
                BackColor = Color.LightGreen;

                Print("UP " + _triggerUpPrice + " @ " +  _triggerUpDateTime + " -> " + _confirmationUpPrice + " @ " + _confirmationUpDateTime );
                PlotConfirmation.Set(1);
            }



            // check for lows under lower boundary
            // check for low trigger
            // check for timespan
            if (Low[0] <= boundaryLower && _triggerDownPrice == boundaryLower + 0.0050 && _triggerDownDateTime >= lookbackTime)
            {
                _confirmationDownDateTime = Time[0];
                _confirmationDownPrice = boundaryLower;
                BackColor = Color.Pink;

                Print("DOWN " + _triggerDownPrice + " @ " + _triggerDownDateTime + " -> " + _confirmationDownPrice + " @ " + _confirmationDownDateTime);
                PlotConfirmation.Set(-1);
            }   
            

        }

        private void TestForTriggers()
        {
 
            // check for highs over upper boundary
            if (High[0] >= boundaryUpper)
            {
                _triggerDownDateTime = Time[0];
                _triggerDownPrice = boundaryUpper;
            }  // check for lows under lower boundary
            else if (Low[0] <= boundaryLower)
            {
                _triggerUpDateTime = Time[0];
                _triggerUpPrice = boundaryLower;

            }
        }


        private void TestForBoundaries()
        {
            beginning = decimal.Truncate(_currPrice * 100) / 100;
             ending = (_currPrice - beginning);

            if (TestRange(ending, 0, 0.0050m))
            {
                boundaryLower = (double)beginning;
                boundaryUpper = (double)beginning + 0.0050;
            }
            else
            {
                boundaryLower = (double)beginning + 0.0050;
                boundaryUpper = (double)beginning + 0.0100;
            }
            // Print(currPrice + ", " + ending  + " is between " + boundaryLower + " and " + boundaryUpper);
            PlotUpper.Set((double)boundaryUpper);
            PlotLower.Set((double)boundaryLower);  
        }
 
          decimal IsTriggerPoint(decimal d)
        {
            decimal tmp = decimal.Truncate((decimal)Close[0] * 10000) / 100;
            decimal ending = (tmp - Math.Truncate(tmp)) * 100;
            if (ending % 50 == 0)
            {
                return ending;
            }

            return -1;
        }



          static bool TestRange(decimal numberToCheck, decimal bottom, decimal top)
        {
            return (numberToCheck >= bottom && numberToCheck < top);
        }


        #region Properties
          [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
          [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
          public DataSeries PlotConfirmation
          {
              get { return Values[0]; }
          }
          [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
          [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
          public DataSeries PlotUpper
          {
              get { return Values[1]; }
          }
          [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
          [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
          public DataSeries PlotLower
          {
              get { return Values[2]; }
          }

        [Description("")]
        [GridCategory("Parameters")]
        public int Hours
        {
            get { return hours; }
            set { hours = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Stoploss
        {
            get { return stoploss; }
            set { stoploss = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int TakeProfit
        {
            get { return takeProfit; }
            set { takeProfit = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int TrailStop
        {
            get { return trailStop; }
            set { trailStop = Math.Max(1, value); }
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
        private GekkoAUDUSD0050[] cacheGekkoAUDUSD0050 = null;

        private static GekkoAUDUSD0050 checkGekkoAUDUSD0050 = new GekkoAUDUSD0050();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public GekkoAUDUSD0050 GekkoAUDUSD0050(int hours, int stoploss, int takeProfit, int trailStop)
        {
            return GekkoAUDUSD0050(Input, hours, stoploss, takeProfit, trailStop);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public GekkoAUDUSD0050 GekkoAUDUSD0050(Data.IDataSeries input, int hours, int stoploss, int takeProfit, int trailStop)
        {
            if (cacheGekkoAUDUSD0050 != null)
                for (int idx = 0; idx < cacheGekkoAUDUSD0050.Length; idx++)
                    if (cacheGekkoAUDUSD0050[idx].Hours == hours && cacheGekkoAUDUSD0050[idx].Stoploss == stoploss && cacheGekkoAUDUSD0050[idx].TakeProfit == takeProfit && cacheGekkoAUDUSD0050[idx].TrailStop == trailStop && cacheGekkoAUDUSD0050[idx].EqualsInput(input))
                        return cacheGekkoAUDUSD0050[idx];

            lock (checkGekkoAUDUSD0050)
            {
                checkGekkoAUDUSD0050.Hours = hours;
                hours = checkGekkoAUDUSD0050.Hours;
                checkGekkoAUDUSD0050.Stoploss = stoploss;
                stoploss = checkGekkoAUDUSD0050.Stoploss;
                checkGekkoAUDUSD0050.TakeProfit = takeProfit;
                takeProfit = checkGekkoAUDUSD0050.TakeProfit;
                checkGekkoAUDUSD0050.TrailStop = trailStop;
                trailStop = checkGekkoAUDUSD0050.TrailStop;

                if (cacheGekkoAUDUSD0050 != null)
                    for (int idx = 0; idx < cacheGekkoAUDUSD0050.Length; idx++)
                        if (cacheGekkoAUDUSD0050[idx].Hours == hours && cacheGekkoAUDUSD0050[idx].Stoploss == stoploss && cacheGekkoAUDUSD0050[idx].TakeProfit == takeProfit && cacheGekkoAUDUSD0050[idx].TrailStop == trailStop && cacheGekkoAUDUSD0050[idx].EqualsInput(input))
                            return cacheGekkoAUDUSD0050[idx];

                GekkoAUDUSD0050 indicator = new GekkoAUDUSD0050();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Hours = hours;
                indicator.Stoploss = stoploss;
                indicator.TakeProfit = takeProfit;
                indicator.TrailStop = trailStop;
                Indicators.Add(indicator);
                indicator.SetUp();

                GekkoAUDUSD0050[] tmp = new GekkoAUDUSD0050[cacheGekkoAUDUSD0050 == null ? 1 : cacheGekkoAUDUSD0050.Length + 1];
                if (cacheGekkoAUDUSD0050 != null)
                    cacheGekkoAUDUSD0050.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheGekkoAUDUSD0050 = tmp;
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
        public Indicator.GekkoAUDUSD0050 GekkoAUDUSD0050(int hours, int stoploss, int takeProfit, int trailStop)
        {
            return _indicator.GekkoAUDUSD0050(Input, hours, stoploss, takeProfit, trailStop);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.GekkoAUDUSD0050 GekkoAUDUSD0050(Data.IDataSeries input, int hours, int stoploss, int takeProfit, int trailStop)
        {
            return _indicator.GekkoAUDUSD0050(input, hours, stoploss, takeProfit, trailStop);
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
        public Indicator.GekkoAUDUSD0050 GekkoAUDUSD0050(int hours, int stoploss, int takeProfit, int trailStop)
        {
            return _indicator.GekkoAUDUSD0050(Input, hours, stoploss, takeProfit, trailStop);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.GekkoAUDUSD0050 GekkoAUDUSD0050(Data.IDataSeries input, int hours, int stoploss, int takeProfit, int trailStop)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.GekkoAUDUSD0050(input, hours, stoploss, takeProfit, trailStop);
        }
    }
}
#endregion
