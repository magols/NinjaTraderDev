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
    public class AmazingCrossoverStrat : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int eMASlowPeriod = 5; // Default setting for EMASlowPeriod
        private int eMAFastPeriod = 10; // Default setting for EMAFastPeriod
        private int rSIPeriod = 10; // Default setting for RSIPeriod
        private int aDXPeriod = 10; // Default setting for ADXPeriod
        private int mMInitialSL = 50; // Default setting for MMInitialSL
        private int mMProfitBeforeBE = 1; // Default setting for MMProfitBeforeBE
        private int aTRPeriod = 8; // Default setting for ATRPeriod
        private double aTRMultiplier = 3; // Default setting for ATRMultiplier
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
        }

        #region Properties
        [Description("Period for the slow EMA")]
        [GridCategory("Parameters")]
        public int EMASlowPeriod
        {
            get { return eMASlowPeriod; }
            set { eMASlowPeriod = Math.Max(1, value); }
        }

        [Description("Period for the fast EMA")]
        [GridCategory("Parameters")]
        public int EMAFastPeriod
        {
            get { return eMAFastPeriod; }
            set { eMAFastPeriod = Math.Max(1, value); }
        }

        [Description("Period for RSI, applied to median")]
        [GridCategory("Parameters")]
        public int RSIPeriod
        {
            get { return rSIPeriod; }
            set { rSIPeriod = Math.Max(1, value); }
        }

        [Description("Period for ADX")]
        [GridCategory("Parameters")]
        public int ADXPeriod
        {
            get { return aDXPeriod; }
            set { aDXPeriod = Math.Max(1, value); }
        }

        [Description("Initial stoploss for trade")]
        [GridCategory("Parameters")]
        public int MMInitialSL
        {
            get { return mMInitialSL; }
            set { mMInitialSL = Math.Max(1, value); }
        }

        [Description("Profit in ticks before moving SL to breakeven")]
        [GridCategory("Parameters")]
        public int MMProfitBeforeBE
        {
            get { return mMProfitBeforeBE; }
            set { mMProfitBeforeBE = Math.Max(1, value); }
        }

        [Description("Period for ATR, deciding moving stop loss levels")]
        [GridCategory("Parameters")]
        public int ATRPeriod
        {
            get { return aTRPeriod; }
            set { aTRPeriod = Math.Max(1, value); }
        }

        [Description("Multiplier for ATR, deciding moving stop loss levels")]
        [GridCategory("Parameters")]
        public double ATRMultiplier
        {
            get { return aTRMultiplier; }
            set { aTRMultiplier = Math.Max(1, value); }
        }
        #endregion
    }
}
