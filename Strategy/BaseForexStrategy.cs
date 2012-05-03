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


namespace NinjaTrader.Strategy
{
    public abstract  class BaseForexStrategy : Strategy
    {
       

        #region Variables
        private int _counter = 1; // Default for Counter

        protected double _equity = 1000000; // Default for Equity
        protected double _percentRisk = 1.0; // Default for PercentRisk
        protected double _lossLevel;

        protected IOrder _entry = null, _exit = null;
        protected TradeState _tradeState = TradeState.InitialStop;

        #endregion

     
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

     


        #region Money management variables

        protected int _mmProfitTicksBeforeBreakeven = 25;
        protected int _mmInitialSL = 30;
        protected int _mmBreakevenTicks = 2;
        protected int _mmTrailTicks = 25;
        #endregion

        #region enum
        public enum TradeState
        {
            InitialStop,
            Breakeven,
            Trailing
        }
        #endregion

        #region Abstract Methods
        // to keep unique strategy stuff 
        protected abstract void MyOnBarUpdate();
        // when we are flat, we are looking for new opportunities to entry
        protected abstract void LookForTrade();
        // once we have entried, we have to manage the positions until exit, and here, 
        protected abstract void ManagePositions();
        // the inherited strategy initializes here
        protected abstract void MyInitialize();

        // this is called by an exit event from the positions, so we can tell the child strategy that the position was closed, and it can reset the signals
        // protected abstract void OnExit(object sender, int positionID);
        // we have always to update signals, even when we have no positions, we are always calculating them, to entry or to exit
        //   protected abstract void UpdateSignals();

        #endregion


        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            if (CurrentBar <= BarsRequired) return;

          

            MyOnBarUpdate();

            if (StillHaveMoney)
            {
                if (IsFlat)
                {
                    LookForTrade();
                }
                else
                {
                    ManagePositions();
                }
            }
          
            
        }

        protected override void Initialize()
        {

            //			System.Diagnostics.EventLog appLog =   new System.Diagnostics.EventLog() ;
    //        Debug("[GENERAL] \t initializing framework", 1);
            CalculateOnBarClose = true;
            //Unmanaged = false;
            MyInitialize();

            //if (logLevel > 1)
            //    TraceOrders = trace;

            // you get an error, dont try to assing the event here
            //positionManager.Position1.Exited += new PositionExited(OnExit);
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



        protected void ManageTradeByBreakevenTrail()
        {

            if (IsLong)
            {
                switch (_tradeState)
                {
                    case TradeState.InitialStop:
                        // switch to breakeven if possible and start trailing
                        if (Close[0] > Position.AvgPrice + (TickSize * _mmProfitTicksBeforeBreakeven))
                        {
                            _lossLevel = Position.AvgPrice + TickSize * _mmBreakevenTicks;
                            SetStopLoss(CalculationMode.Price, _lossLevel);
                            _tradeState = TradeState.Trailing;
                        }
                        break;

                    case TradeState.Trailing:
                        if (Close[0] - TickSize * _mmTrailTicks > _lossLevel)
                        {
                            _lossLevel = Close[0] - TickSize * _mmTrailTicks;
                            SetStopLoss(CalculationMode.Price, _lossLevel);
                        }
                        break;
                }

            }
            else if (IsShort)
            {
                switch (_tradeState)
                {
                    case TradeState.InitialStop:
                        // switch to breakeven if possible and start trailing
                        if (Close[0] < Position.AvgPrice - (TickSize * _mmProfitTicksBeforeBreakeven))
                        {
                            _lossLevel = Position.AvgPrice - TickSize * _mmBreakevenTicks;
                            SetStopLoss(CalculationMode.Price, _lossLevel);
                            _tradeState = TradeState.Trailing;
                        }
                        break;

                    case TradeState.Trailing:
                        if (Close[0] + TickSize * _mmTrailTicks < _lossLevel)
                        {
                            _lossLevel = Close[0] + TickSize * _mmTrailTicks;
                            SetStopLoss(CalculationMode.Price, _lossLevel);
                        }
                        break;
                }
            }
            DrawLossLevel();
        }

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


    }
}
