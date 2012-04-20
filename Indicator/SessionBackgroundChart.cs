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
    /// Colors the chart background depending on sessions
    /// </summary>
    [Description("Colors the chart background depending on sessions")]
    public class SessionBackgroundChart : Indicator
    {
        #region Variables
        // Wizard generated variables
        private int asianStartHour = 23; // Default setting for AsianStart
        private int asianEndHour = 10; // Default setting for AsianEnd
        private int londonStartHour = 9; // Default setting for LondonStart
        private int londonEndHour = 18; // Default setting for LondonEnd
        private int newyorkStartHour = 14; // Default setting for LondonStart
        private int newyorkEndHour = 23; // Default setting for LondonEnd

        private int asianStartMinute = 0; // Default setting for AsianStart
        private int asianEndMinute = 0; // Default setting for AsianEnd
        private int londonStartMinute = 0; // Default setting for LondonStart
        private int londonEndMinute = 0; // Default setting for LondonEnd
        private int newyorkStartMinute = 0; // Default setting for LondonStart
        private int newyorkEndMinute = 0; // Default setting for LondonEnd

        private Color asianColor = Color.FromArgb(225, 225, 225);
        private Color londonColor = Color.FromArgb(238, 238, 238);
        private Color newyorkColor = Color.FromArgb(238, 238, 238);

        private Color overlapAsianLondonColor = Color.FromArgb(242, 242, 242);
        private Color overlapLondonNewyorkColor = Color.FromArgb(255, 255, 255);


        private TimeSpan midnightAfter = new TimeSpan(0, 0, 0);
        private TimeSpan midnightBefore = new TimeSpan(23, 59, 59);
        private TimeSpan spanAsianStart;
        private TimeSpan spanAsianEnd;
        private TimeSpan spanLondonStart;
        private TimeSpan spanLondonEnd;
        private TimeSpan spanNewyorkStart;
        private TimeSpan spanNewyorkEnd;

      

        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Overlay				= true;
			CalculateOnBarClose = true;

           spanAsianStart = new TimeSpan(asianStartHour, asianStartMinute, 00);
           spanAsianEnd = new TimeSpan(asianEndHour, asianEndMinute, 00);
           spanLondonStart = new TimeSpan(londonStartHour, londonStartMinute, 00);
           spanLondonEnd = new TimeSpan(londonEndHour, londonEndMinute, 00);
           spanNewyorkStart = new TimeSpan(newyorkStartHour, newyorkStartMinute, 00);
           spanNewyorkEnd = new TimeSpan(newyorkEndHour, newyorkEndMinute, 00);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {

            if (BarsPeriod.Id == PeriodType.Minute)
            {
                bool sessionAsian = false, sessionLondon = false, sessionNewyork = false;
                TimeSpan clock = new TimeSpan(Time[0].Hour, Time[0].Minute, 00);
              
                if ( (clock >= spanAsianStart && clock < midnightBefore)   || (clock >= midnightAfter&& clock < spanAsianEnd)) sessionAsian = true;
                if (clock >= spanLondonStart && clock < spanLondonEnd) sessionLondon = true;
                if (clock >= spanNewyorkStart && clock < spanNewyorkEnd) sessionNewyork = true;

                if (sessionAsian && sessionLondon)
                {
                    BackColor = overlapAsianLondonColor;
                }
                else if (sessionLondon && sessionNewyork)
                {
                    BackColor = overlapLondonNewyorkColor;
                } 
                else  if (sessionAsian)
                {
                    BackColor = asianColor;
                } 

                else if (sessionLondon)
                {
                    BackColor = londonColor;
                }
                else if (sessionNewyork)
                {
                    BackColor = newyorkColor;
                }

                
            } 

        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot0
        {
            get { return Values[0]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public Color AsianColor
        {
            get { return asianColor; }
            set { asianColor =value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public Color LondonColor
        {
            get { return londonColor; }
            set { londonColor = value; }
        }
        [Description("")]
        [GridCategory("Parameters")]
        public Color NewyorkColor
        {
            get { return newyorkColor; }
            set { newyorkColor = value; }
        }



        [Description("")]
        [GridCategory("Parameters")]
        public int AsianStartMinute
        {
            get { return asianStartMinute; }
            set { asianStartMinute = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int AsianEndMinute
        {
            get { return asianEndMinute; }
            set { asianEndMinute = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int LondonStartMinute
        {
            get { return londonStartMinute; }
            set { londonStartMinute = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int LondonEndMinute
        {
            get { return londonEndMinute; }
            set { londonEndMinute = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int NewyorkStartMinute
        {
            get { return newyorkStartMinute; }
            set { newyorkStartMinute = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int NewyorkEndMinute
        {
            get { return newyorkEndMinute; }
            set { newyorkEndMinute = value; }
        }
        [Description("")]
        [GridCategory("Parameters")]
        public int AsianStartHour
        {
            get { return asianStartHour; }
            set { asianStartHour = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int AsianEndHour
        {
            get { return asianEndHour; }
            set { asianEndHour = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int LondonStartHour
        {
            get { return londonStartHour; }
            set { londonStartHour = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int LondonEndHour
        {
            get { return londonEndHour; }
            set { londonEndHour = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int NewyorkStartHour
        {
            get { return newyorkStartHour; }
            set { newyorkStartHour = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int NewyorkEndHour
        {
            get { return newyorkEndHour; }
            set { newyorkEndHour = value; }
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
        private SessionBackgroundChart[] cacheSessionBackgroundChart = null;

        private static SessionBackgroundChart checkSessionBackgroundChart = new SessionBackgroundChart();

        /// <summary>
        /// Colors the chart background depending on sessions
        /// </summary>
        /// <returns></returns>
        public SessionBackgroundChart SessionBackgroundChart(Color asianColor, int asianEndHour, int asianEndMinute, int asianStartHour, int asianStartMinute, Color londonColor, int londonEndHour, int londonEndMinute, int londonStartHour, int londonStartMinute, Color newyorkColor, int newyorkEndHour, int newyorkEndMinute, int newyorkStartHour, int newyorkStartMinute)
        {
            return SessionBackgroundChart(Input, asianColor, asianEndHour, asianEndMinute, asianStartHour, asianStartMinute, londonColor, londonEndHour, londonEndMinute, londonStartHour, londonStartMinute, newyorkColor, newyorkEndHour, newyorkEndMinute, newyorkStartHour, newyorkStartMinute);
        }

        /// <summary>
        /// Colors the chart background depending on sessions
        /// </summary>
        /// <returns></returns>
        public SessionBackgroundChart SessionBackgroundChart(Data.IDataSeries input, Color asianColor, int asianEndHour, int asianEndMinute, int asianStartHour, int asianStartMinute, Color londonColor, int londonEndHour, int londonEndMinute, int londonStartHour, int londonStartMinute, Color newyorkColor, int newyorkEndHour, int newyorkEndMinute, int newyorkStartHour, int newyorkStartMinute)
        {
            if (cacheSessionBackgroundChart != null)
                for (int idx = 0; idx < cacheSessionBackgroundChart.Length; idx++)
                    if (cacheSessionBackgroundChart[idx].AsianColor == asianColor && cacheSessionBackgroundChart[idx].AsianEndHour == asianEndHour && cacheSessionBackgroundChart[idx].AsianEndMinute == asianEndMinute && cacheSessionBackgroundChart[idx].AsianStartHour == asianStartHour && cacheSessionBackgroundChart[idx].AsianStartMinute == asianStartMinute && cacheSessionBackgroundChart[idx].LondonColor == londonColor && cacheSessionBackgroundChart[idx].LondonEndHour == londonEndHour && cacheSessionBackgroundChart[idx].LondonEndMinute == londonEndMinute && cacheSessionBackgroundChart[idx].LondonStartHour == londonStartHour && cacheSessionBackgroundChart[idx].LondonStartMinute == londonStartMinute && cacheSessionBackgroundChart[idx].NewyorkColor == newyorkColor && cacheSessionBackgroundChart[idx].NewyorkEndHour == newyorkEndHour && cacheSessionBackgroundChart[idx].NewyorkEndMinute == newyorkEndMinute && cacheSessionBackgroundChart[idx].NewyorkStartHour == newyorkStartHour && cacheSessionBackgroundChart[idx].NewyorkStartMinute == newyorkStartMinute && cacheSessionBackgroundChart[idx].EqualsInput(input))
                        return cacheSessionBackgroundChart[idx];

            lock (checkSessionBackgroundChart)
            {
                checkSessionBackgroundChart.AsianColor = asianColor;
                asianColor = checkSessionBackgroundChart.AsianColor;
                checkSessionBackgroundChart.AsianEndHour = asianEndHour;
                asianEndHour = checkSessionBackgroundChart.AsianEndHour;
                checkSessionBackgroundChart.AsianEndMinute = asianEndMinute;
                asianEndMinute = checkSessionBackgroundChart.AsianEndMinute;
                checkSessionBackgroundChart.AsianStartHour = asianStartHour;
                asianStartHour = checkSessionBackgroundChart.AsianStartHour;
                checkSessionBackgroundChart.AsianStartMinute = asianStartMinute;
                asianStartMinute = checkSessionBackgroundChart.AsianStartMinute;
                checkSessionBackgroundChart.LondonColor = londonColor;
                londonColor = checkSessionBackgroundChart.LondonColor;
                checkSessionBackgroundChart.LondonEndHour = londonEndHour;
                londonEndHour = checkSessionBackgroundChart.LondonEndHour;
                checkSessionBackgroundChart.LondonEndMinute = londonEndMinute;
                londonEndMinute = checkSessionBackgroundChart.LondonEndMinute;
                checkSessionBackgroundChart.LondonStartHour = londonStartHour;
                londonStartHour = checkSessionBackgroundChart.LondonStartHour;
                checkSessionBackgroundChart.LondonStartMinute = londonStartMinute;
                londonStartMinute = checkSessionBackgroundChart.LondonStartMinute;
                checkSessionBackgroundChart.NewyorkColor = newyorkColor;
                newyorkColor = checkSessionBackgroundChart.NewyorkColor;
                checkSessionBackgroundChart.NewyorkEndHour = newyorkEndHour;
                newyorkEndHour = checkSessionBackgroundChart.NewyorkEndHour;
                checkSessionBackgroundChart.NewyorkEndMinute = newyorkEndMinute;
                newyorkEndMinute = checkSessionBackgroundChart.NewyorkEndMinute;
                checkSessionBackgroundChart.NewyorkStartHour = newyorkStartHour;
                newyorkStartHour = checkSessionBackgroundChart.NewyorkStartHour;
                checkSessionBackgroundChart.NewyorkStartMinute = newyorkStartMinute;
                newyorkStartMinute = checkSessionBackgroundChart.NewyorkStartMinute;

                if (cacheSessionBackgroundChart != null)
                    for (int idx = 0; idx < cacheSessionBackgroundChart.Length; idx++)
                        if (cacheSessionBackgroundChart[idx].AsianColor == asianColor && cacheSessionBackgroundChart[idx].AsianEndHour == asianEndHour && cacheSessionBackgroundChart[idx].AsianEndMinute == asianEndMinute && cacheSessionBackgroundChart[idx].AsianStartHour == asianStartHour && cacheSessionBackgroundChart[idx].AsianStartMinute == asianStartMinute && cacheSessionBackgroundChart[idx].LondonColor == londonColor && cacheSessionBackgroundChart[idx].LondonEndHour == londonEndHour && cacheSessionBackgroundChart[idx].LondonEndMinute == londonEndMinute && cacheSessionBackgroundChart[idx].LondonStartHour == londonStartHour && cacheSessionBackgroundChart[idx].LondonStartMinute == londonStartMinute && cacheSessionBackgroundChart[idx].NewyorkColor == newyorkColor && cacheSessionBackgroundChart[idx].NewyorkEndHour == newyorkEndHour && cacheSessionBackgroundChart[idx].NewyorkEndMinute == newyorkEndMinute && cacheSessionBackgroundChart[idx].NewyorkStartHour == newyorkStartHour && cacheSessionBackgroundChart[idx].NewyorkStartMinute == newyorkStartMinute && cacheSessionBackgroundChart[idx].EqualsInput(input))
                            return cacheSessionBackgroundChart[idx];

                SessionBackgroundChart indicator = new SessionBackgroundChart();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AsianColor = asianColor;
                indicator.AsianEndHour = asianEndHour;
                indicator.AsianEndMinute = asianEndMinute;
                indicator.AsianStartHour = asianStartHour;
                indicator.AsianStartMinute = asianStartMinute;
                indicator.LondonColor = londonColor;
                indicator.LondonEndHour = londonEndHour;
                indicator.LondonEndMinute = londonEndMinute;
                indicator.LondonStartHour = londonStartHour;
                indicator.LondonStartMinute = londonStartMinute;
                indicator.NewyorkColor = newyorkColor;
                indicator.NewyorkEndHour = newyorkEndHour;
                indicator.NewyorkEndMinute = newyorkEndMinute;
                indicator.NewyorkStartHour = newyorkStartHour;
                indicator.NewyorkStartMinute = newyorkStartMinute;
                Indicators.Add(indicator);
                indicator.SetUp();

                SessionBackgroundChart[] tmp = new SessionBackgroundChart[cacheSessionBackgroundChart == null ? 1 : cacheSessionBackgroundChart.Length + 1];
                if (cacheSessionBackgroundChart != null)
                    cacheSessionBackgroundChart.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSessionBackgroundChart = tmp;
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
        /// Colors the chart background depending on sessions
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SessionBackgroundChart SessionBackgroundChart(Color asianColor, int asianEndHour, int asianEndMinute, int asianStartHour, int asianStartMinute, Color londonColor, int londonEndHour, int londonEndMinute, int londonStartHour, int londonStartMinute, Color newyorkColor, int newyorkEndHour, int newyorkEndMinute, int newyorkStartHour, int newyorkStartMinute)
        {
            return _indicator.SessionBackgroundChart(Input, asianColor, asianEndHour, asianEndMinute, asianStartHour, asianStartMinute, londonColor, londonEndHour, londonEndMinute, londonStartHour, londonStartMinute, newyorkColor, newyorkEndHour, newyorkEndMinute, newyorkStartHour, newyorkStartMinute);
        }

        /// <summary>
        /// Colors the chart background depending on sessions
        /// </summary>
        /// <returns></returns>
        public Indicator.SessionBackgroundChart SessionBackgroundChart(Data.IDataSeries input, Color asianColor, int asianEndHour, int asianEndMinute, int asianStartHour, int asianStartMinute, Color londonColor, int londonEndHour, int londonEndMinute, int londonStartHour, int londonStartMinute, Color newyorkColor, int newyorkEndHour, int newyorkEndMinute, int newyorkStartHour, int newyorkStartMinute)
        {
            return _indicator.SessionBackgroundChart(input, asianColor, asianEndHour, asianEndMinute, asianStartHour, asianStartMinute, londonColor, londonEndHour, londonEndMinute, londonStartHour, londonStartMinute, newyorkColor, newyorkEndHour, newyorkEndMinute, newyorkStartHour, newyorkStartMinute);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Colors the chart background depending on sessions
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SessionBackgroundChart SessionBackgroundChart(Color asianColor, int asianEndHour, int asianEndMinute, int asianStartHour, int asianStartMinute, Color londonColor, int londonEndHour, int londonEndMinute, int londonStartHour, int londonStartMinute, Color newyorkColor, int newyorkEndHour, int newyorkEndMinute, int newyorkStartHour, int newyorkStartMinute)
        {
            return _indicator.SessionBackgroundChart(Input, asianColor, asianEndHour, asianEndMinute, asianStartHour, asianStartMinute, londonColor, londonEndHour, londonEndMinute, londonStartHour, londonStartMinute, newyorkColor, newyorkEndHour, newyorkEndMinute, newyorkStartHour, newyorkStartMinute);
        }

        /// <summary>
        /// Colors the chart background depending on sessions
        /// </summary>
        /// <returns></returns>
        public Indicator.SessionBackgroundChart SessionBackgroundChart(Data.IDataSeries input, Color asianColor, int asianEndHour, int asianEndMinute, int asianStartHour, int asianStartMinute, Color londonColor, int londonEndHour, int londonEndMinute, int londonStartHour, int londonStartMinute, Color newyorkColor, int newyorkEndHour, int newyorkEndMinute, int newyorkStartHour, int newyorkStartMinute)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SessionBackgroundChart(input, asianColor, asianEndHour, asianEndMinute, asianStartHour, asianStartMinute, londonColor, londonEndHour, londonEndMinute, londonStartHour, londonStartMinute, newyorkColor, newyorkEndHour, newyorkEndMinute, newyorkStartHour, newyorkStartMinute);
        }
    }
}
#endregion
