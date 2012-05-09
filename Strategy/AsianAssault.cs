#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Enter the description of your strategy here
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class AsianAssault : BaseForexStrategy
    {
        #region Variables


        private int _timeStartHour = 2;
        private int _timeStartMinute = 0;

        private int _timeStopHour = 4;
        private int _timeStopMinute = 59;

        #endregion

        protected override void MyInitialize()
        {

        }

        protected override void SetupIndicatorProperties()
        {

        }

 
        protected override void MyOnBarUpdate()
        {
            var ok =  TimeForEntryOk();
        }

   

        protected override void LookForTrade()
        {
        }



        protected override void MyManagePosition()
        {

        }

        private bool TimeForEntryOk()
        {
            if ((Time[0].Hour >= TimeStartHour && Time[0].Minute >= _timeStartMinute) && (Time[0].Hour <= TimeStopHour && Time[0].Minute <= _timeStopMinute))
            { return true; }
            return false;
        }

        #region parameters

        [Description("")]
        [GridCategory("Parameters")]
        public int TimeStartHour
        {
            get { return _timeStartHour; }
            set { _timeStartHour = Math.Max(1, value); }
        }
        [Description("")]
        [GridCategory("Parameters")]
        public int TimeStopHour
        {
            get { return _timeStopHour; }
            set { _timeStopHour = Math.Max(1, value); }
        }
        #endregion
    }
}


