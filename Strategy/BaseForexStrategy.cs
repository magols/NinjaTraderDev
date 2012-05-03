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
    public abstract  class BaseForexStrategy : Strategy, ICustomTypeDescriptor
    {
        protected TradeState _tradeState = TradeState.InitialStop;
       

        #region Account variables
        private int _counter = 1; // Default for Counter
        protected double _equity = 20000; // Default for Equity

        protected IOrder _entry = null, _exit = null;
        

        #endregion
    
        #region Account properties

        [Description("Initial Equity of the account")]
        [GridCategory("Account")]
        public double Equity
        {
            get { return _equity; }
            set { _equity = Math.Max(1, value); }
        }



        [Description("A counter used for optimization runs")]
        [GridCategory("Account")]
        public int RunCounter
        {
            get { return _counter; }
            set { _counter = Math.Max(1, value); }
        }
        #endregion

        
        #region Enums
        public enum TradeState
        {
            InitialStop,
            Breakeven,
            Trailing
        }

        public enum ExitType
        {
            None,
            IntialToBreakevenToTrailing,
            TrailingATR
        }

        #endregion


        #region Abstract Methods
        // to keep unique strategy stuff 
        protected abstract void MyOnBarUpdate();
        // when we are flat, we are looking for new opportunities to entry
        protected abstract void LookForTrade();
   
        // once we have entried, we have to manage the positions until exit, and here, 
        //protected abstract void ManagePositions();
       
        // the inherited strategy initializes here
        protected abstract void MyInitialize();

       

        // this is called by an exit event from the positions, so we can tell the child strategy that the position was closed, and it can reset the signals
        // protected abstract void OnExit(object sender, int positionID);
        // we have always to update signals, even when we have no positions, we are always calculating them, to entry or to exit
        //   protected abstract void UpdateSignals();

        #endregion

        
        #region methods

        

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
        
        //    ClearOutputWindow();
            
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

        protected void ManagePositions()
        {
            switch (_exitType)
            {
                case ExitType.IntialToBreakevenToTrailing:
                    ManageTradeByBreakevenTrail();
                    break;

                case ExitType.TrailingATR:
                    ManageTradeByTrailingATR();
                    break;
                default:
                    break;

            }
        }

        private void ManageTradeByTrailingATR()
        {
            double risk = _risk;
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


        #region Money management variables

        protected ExitType _exitType = ExitType.None;
        protected double _risk;
        protected double _percentRisk = 1.0; // Default for PercentRisk
        protected double _lossLevel;

        // breakeven trailstop
        protected int _mmProfitTicksBeforeBreakeven = 25;
        protected int _mmInitialSL = 30;
        protected int _mmBreakevenTicks = 2;
        protected int _mmTrailTicks = 25;



        // atr multiplier trail stop
        private double _mmAtrMultiplier = 3;
        #endregion

        #region Money management properties

        [Description("A counter used for optimization runs")]
        [GridCategory("Money management")]
        [RefreshProperties(RefreshProperties.All)]
        public ExitType ExitStrategy
        {
            get { return _exitType; }
            set { _exitType = value; }
        }

        [Description("Risk per trade in %")]
        [GridCategory("Money management")]
        public double PercentRisk
        {
            get { return _percentRisk; }
            set { _percentRisk = Math.Max(0.001, value); }
        }

        [Description("Ticks in profit before moving stoploss to breakeven(-ish)")]
        [GridCategory("Money management")]
        public int ProfitTicksBeforeBreakeven
        {
            get { return _mmProfitTicksBeforeBreakeven; }
            set { _mmProfitTicksBeforeBreakeven = Math.Max(1, value); }
        }

        [Description("Initial stoploss in ticks")]
        [GridCategory("Money management")]
        public int InitialStoploss
        {
            get { return _mmInitialSL; }
            set { _mmInitialSL = Math.Max(1, value); }
        }


        [Description("Ticksbeyond breakeven to move from initial stop")]
        [GridCategory("Money management")]
        public int BreakevenTicks
        {
            get { return _mmBreakevenTicks; }
            set { _mmBreakevenTicks = Math.Max(1, value); }
        }

        [Description("Trailing stop ticks, when starting to trail from breakeven")]
        [GridCategory("Money management")]
        public int TrailTicks
        {
            get { return _mmTrailTicks; }
            set { _mmTrailTicks = Math.Max(1, value); }
        }


        [Description("Lookback period for crossover convergence")]
        [GridCategory("Money management")]
        public double MMAtrMultiplier
        {
            get { return _mmAtrMultiplier; }
            set { _mmAtrMultiplier = Math.Max(1, value); }
        }

        #endregion

        
        #region Custom Property Manipulation

        private void ModifyProperties(PropertyDescriptorCollection col)
        {
            //PercentRisk
            //ProfitTicksBeforeBreakeven
            //InitialStoploss
            //BreakevenTicks
            //TrailTicks
            //MMAtrMultiplier

            
            switch (ExitStrategy)
            {
                case ExitType.IntialToBreakevenToTrailing:
                    col.Remove(col.Find("PercentRisk", true));
                    col.Remove(col.Find("MMAtrMultiplier", true));   
                    break;

                case ExitType.TrailingATR:
                    col.Remove(col.Find("PercentRisk", true));    
                    col.Remove(col.Find("ProfitTicksBeforeBreakeven", true));    
                    col.Remove(col.Find("InitialStoploss", true));    
                    col.Remove(col.Find("BreakevenTicks", true));    
                    col.Remove(col.Find("TrailTicks", true));    
                    break;

                case ExitType.None:
                    col.Remove(col.Find("PercentRisk", true));    
                    col.Remove(col.Find("ProfitTicksBeforeBreakeven", true));    
                    col.Remove(col.Find("InitialStoploss", true));    
                    col.Remove(col.Find("BreakevenTicks", true));    
                    col.Remove(col.Find("TrailTicks", true));
                    col.Remove(col.Find("MMAtrMultiplier", true));
                    break;
                

            }
        }

        #endregion

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(GetType());
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(GetType());
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(GetType());
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(GetType());
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(GetType());
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(GetType());
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(GetType(), editorBaseType);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(GetType(), attributes);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(GetType());
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection orig = TypeDescriptor.GetProperties(GetType(), attributes);
            PropertyDescriptor[] arr = new PropertyDescriptor[orig.Count];
            orig.CopyTo(arr, 0);
            PropertyDescriptorCollection col = new PropertyDescriptorCollection(arr);

            ModifyProperties(col);
            return col;

        }

        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(GetType());
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }
}
