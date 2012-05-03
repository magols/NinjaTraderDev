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
    public class TestStrategy : BaseForexStrategy
    {
        #region Variables
        private int _emaSlowPeriod = 10; // Default setting for EMASlowPeriod
        private int _emaFastPeriod = 5; // Default setting for EMAFastPeriod
        private int _rsiPeriod = 10; // Default setting for RSIPeriod
        private int _adxPeriod = 10; // Default setting for ADXPeriod
        private int _atrPeriod = 10;
        private double _atrExclusionMultiplier = 1;
        private int _adxMin = 20;
        private int _rsiLower = 45;
        private int _rsiUpper = 55;
        private int _crossoverLookbackPeriod = 1;

        private AmazingCrossoverIndi _indi;

       
        #endregion

 
        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void MyInitialize()
        {

            if (_indi == null)
            {
                _indi = AmazingCrossoverIndi(_adxMin, _adxPeriod, _atrExclusionMultiplier, _atrPeriod,
                             _crossoverLookbackPeriod, _emaFastPeriod, _emaSlowPeriod, _rsiLower, _rsiPeriod,
                             _rsiUpper);
                _indi.SetupObjects();
                Add(_indi);
            }
            CalculateOnBarClose = true;
        }



        protected  override void MyOnBarUpdate()
        {
            var a = _indi[0];
        }

        protected override void LookForTrade()
        {
            if (_indi.Signal == 0) return;

            double risk = TickSize * _mmInitialSL;
            if (_indi.Signal == 1)
            {
                _lossLevel = Close[0] - risk;
                _entry = EnterLong(ComputeQty(risk));
            }
            else if (_indi.Signal == -1)
            {
                _lossLevel = Close[0] + risk;
                _entry = EnterShort(ComputeQty(risk));
            }
            _tradeState = TradeState.InitialStop;
        }






        #region Properties
        [Description("Period for the slow EMA")]
        [GridCategory("Indicator")]
        public int EMASlowPeriod
        {
            get { return _emaSlowPeriod; }
            set { _emaSlowPeriod = Math.Max(1, value); }
        }

        [Description("Period for the fast EMA")]
        [GridCategory("Indicator")]
        public int EMAFastPeriod
        {
            get { return _emaFastPeriod; }
            set { _emaFastPeriod = Math.Max(1, value); }
        }

        [Description("Period for RSI, applied to median")]
        [GridCategory("Indicator")]
        public int RSIPeriod
        {
            get { return _rsiPeriod; }
            set { _rsiPeriod = Math.Max(1, value); }
        }




        [Description("Period for RSI lower")]
        [GridCategory("Indicator")]
        public int RSILower
        {
            get { return _rsiLower; }
            set { _rsiLower = Math.Max(1, value); }
        }
        [Description("Period for RSI upper")]
        [GridCategory("Indicator")]
        public int RSIUpper
        {
            get { return _rsiUpper; }
            set { _rsiUpper = Math.Max(1, value); }
        }


        [Description("Period for ADX")]
        [GridCategory("Indicator")]
        public int ADXPeriod
        {
            get { return _adxPeriod; }
            set { _adxPeriod = Math.Max(1, value); }
        }
        [Description("Minimum for ADX")]
        [GridCategory("Indicator")]
        public int ADXMinimum
        {
            get { return _adxMin; }
            set { _adxMin = Math.Max(1, value); }
        }

        [Description("Period for ATR")]
        [GridCategory("Indicator")]
        public int ATRPeriod
        {
            get { return _atrPeriod; }
            set { _atrPeriod = Math.Max(1, value); }
        }

        [Description("ATR multiplier for exluding trades")]
        [GridCategory("Indicator")]
        public double ATRExclusionMultiplier
        {
            get { return _atrExclusionMultiplier; }
            set { _atrExclusionMultiplier = Math.Max(1, value); }
        }


        [Description("Lookback period for crossover convergence")]
        [GridCategory("Indicator")]
        public int CrossoverLookbackPeriod
        {
            get { return _crossoverLookbackPeriod; }
            set { _crossoverLookbackPeriod = Math.Max(1, value); }
        }


   

        #endregion


        //#region properties MM

        //[Description("Ticks in profit before moving stoploss to breakeven(-ish)")]
        //[GridCategory("Money management")]
        //public int ProfitTicksBeforeBreakeven
        //{
        //    get { return _mmProfitTicksBeforeBreakeven; }
        //    set { _mmProfitTicksBeforeBreakeven = Math.Max(1, value); }
        //}

        //[Description("Initial stoploss in ticks")]
        //[GridCategory("Money management")]
        //public int InitialStoploss
        //{
        //    get { return _mmInitialSL; }
        //    set { _mmInitialSL = Math.Max(1, value); }
        //}


        //[Description("Ticksbeyond breakeven to move from initial stop")]
        //[GridCategory("Money management")]
        //public int BreakevenTicks
        //{
        //    get { return _mmBreakevenTicks; }
        //    set { _mmBreakevenTicks = Math.Max(1, value); }
        //}

        //[Description("Trailing stop ticks, when starting to trail from breakeven")]
        //[GridCategory("Money management")]
        //public int TrailTicks
        //{
        //    get { return _mmTrailTicks; }
        //    set { _mmTrailTicks = Math.Max(1, value); }
        //}

        //#endregion

    }
}
