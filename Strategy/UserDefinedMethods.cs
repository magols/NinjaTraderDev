#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
	/// <summary>
	/// This file holds all user defined strategy methods.
	/// </summary>
	public partial class Strategy
    {
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
        public int Counter
        {
            get { return _counter; }
            set { _counter = Math.Max(1, value); }
        }

        #endregion

    }
}
