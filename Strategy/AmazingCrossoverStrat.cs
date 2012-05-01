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

        #region userdefined

        #region Variables
        private int _counter = 1; // Default for Counter

        protected double _equity = 1000000; // Default for Equity
        protected double _percentRisk = 1.0; // Default for PercentRisk
        protected double _lossLevel;

        protected IOrder _entry = null, _exit = null;

        #endregion

        protected void GoFlat()
        {
            if (IsLong) _exit = ExitLong();
            if (IsShort) _exit = ExitShort();
        }

        protected int ComputeQty(double volatilityRisk)
        {
            double dollarRisk = _equity * (_percentRisk / 100.0);
            double tickRisk = Round2Tick(volatilityRisk / this.TickSize);
            double qty = (dollarRisk / (volatilityRisk * this.PointValue));

            int rounded;

            // round the shares into a lot-friendly number, applies only to stocks
            //			rounded = (int) (Math.Round(qty/100.0, 0) * 100.0);

            rounded = (int)Math.Round(qty, 0);

            //			P("vol risk=" + volatilityRisk.ToString("N2") 
            //				+ ", $ risk=" + dollarRisk.ToString("C2") 
            //				+ ", equity=" + _equity.ToString("C2")
            //				+ ", qty=" + qty.ToString("N0") 
            //				+ ", rounded=" + rounded.ToString("N0")
            //				+ ", price=" + Close[0].ToString());

            return rounded;
        }

        protected void DrawLossLevel()
        {
            if (IsFlat) return;

            Color color = Color.Black;

            if (IsLong)
                color = Color.Magenta;
            else if (IsShort)
                color = Color.Cyan;

            this.DrawDiamond("d" + CurrentBar, true, 0, _lossLevel, color);
        }

        protected bool StillHaveMoney { get { return _equity > 0; } }

        #region Helpers
        protected bool IsFlat { get { return Position.MarketPosition == MarketPosition.Flat; } }
        protected bool IsLong { get { return Position.MarketPosition == MarketPosition.Long; } }
        protected bool IsShort { get { return Position.MarketPosition == MarketPosition.Short; } }

        protected double Round2Tick(double val) { return Instrument.MasterInstrument.Round2TickSize(val); }
        protected double PointValue { get { return this.Instrument.MasterInstrument.PointValue; } }

        protected void P(string msg)
        {
            Print(Time[0].ToShortDateString() + " " + Time[0].ToShortTimeString() + "::" + msg);
        }
        #endregion

        #region Properties

        [Description("Initial Equity of the account")]
        [GridCategory("Account")]
        public double Equity
        {
            get { return _equity; }
            set { _equity = Math.Max(1, value); }
        }

        [Description("Initial Equity of the account")]
        [GridCategory("Account")]
        public double PercentRisk
        {
            get { return _percentRisk; }
            set { _percentRisk = Math.Max(0.001, value); }
        }

        [Description("A counter used for optimization runs")]
        [GridCategory("Account")]
        public int RunCounter
        {
            get { return _counter; }
            set { _counter = Math.Max(1, value); }
        }

        #endregion

        #endregion

        #region IndicatorVariables
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

        #endregion

        #region StrategyVariables

        private AmazingCrossoverIndi _indi;
        private double _mmAtrMultiplier = 3;
        #endregion




        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            if (_indi == null)
            {
                _indi = AmazingCrossoverIndi(_adxMin, _adxPeriod, _atrExclusionMultiplier, _atrPeriod,
                             _crossoverLookbackPeriod, _emaFastPeriod, _emaSlowPeriod, _rsiLower, _rsiPeriod,
                             _rsiUpper);
                _indi.SetupObjects();
                 Add(_indi);
            }

            this.ClearOutputWindow();

            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            //if (_indi.ATRseries == null)
            //{
            //    _indi.SetupObjects();
            //}

            if (CurrentBar <= BarsRequired) return;

            if (StillHaveMoney)
            {
                double risk = _indi.ATRseries[0] * _mmAtrMultiplier;
                var x = _indi.EMAFastPlot[0];

                if (IsFlat)
                    LookForTrade(risk);
                else
                    ManageTrade(risk);
            }
        }

        private void LookForTrade(double risk)
        {

            if (_indi.Signal == 1)
            {
                //	GoFlat();
                _lossLevel = Close[0] - risk;
                SetStopLoss(CalculationMode.Price, _lossLevel);
                _entry = EnterLong(ComputeQty(risk));
            }
            else if (_indi.Signal == -1)
            {
                //	GoFlat();
                _lossLevel = Close[0] + risk;
                SetStopLoss(CalculationMode.Price, _lossLevel);
                _entry = EnterShort(ComputeQty(risk));
            }
        }


        private void ManageTrade(double risk)
        {
            //			if (!Flat)
            //					P("atr = " + _atr[0].ToString("N3")
            //						+ ", risk = " + risk.ToString("N2") 
            //						+ ", lossLevel = " + _lossLevel.ToString("N2") 
            //						+ ", High = " + High[0] 
            //						+ ", Low = " + Low[0]);
            if (IsLong)
            {
                if (High[0] - risk > _lossLevel)
                {
                    _lossLevel = High[0] - risk;
                    //					P("LONG: changing stop loss level to " + _lossLevel.ToString("N2"));
                    SetStopLoss(CalculationMode.Price, _lossLevel);
                }
            }
            else if (IsShort)
            {
                if (Low[0] + risk < _lossLevel)
                {
                    _lossLevel = Low[0] + risk;
                    //					P("SHORT: changing stop loss level to " + _lossLevel.ToString("N2"));
                    SetStopLoss(CalculationMode.Price, _lossLevel);
                }
            }
            DrawLossLevel();
        }

        protected override void OnExecution(IExecution execution)
        {
            if (_entry == null) return;
            if (execution == null) return;
            if (execution.Order == null) return;

            bool isEntry = (_entry.Token == execution.Order.Token);
            bool isExit = !isEntry;

            if (isExit)
            {
                double diff = 0;

                IOrder exit = execution.Order;
                if (_entry.OrderAction == OrderAction.Buy)
                    diff = exit.AvgFillPrice - _entry.AvgFillPrice;
                else if (_entry.OrderAction == OrderAction.SellShort)
                    diff = _entry.AvgFillPrice - exit.AvgFillPrice;

                double profit = ((diff * this.PointValue)) * _entry.Quantity;
                _equity += profit;

                //				P("Profit=" + profit.ToString("C2") + ", Equity=" + _equity.ToString("C2"));
            }
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

        [Description("Lookback period for crossover convergence")]
        [GridCategory("Money management")]
        public double MMAtrMultiplier
        {
            get { return _mmAtrMultiplier; }
            set { _mmAtrMultiplier = Math.Max(1, value); }
        }
        #endregion
    }
}
