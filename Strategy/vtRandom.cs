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
	/// <summary>
	/// Van Tharp's random test
	/// </summary>
	[Description("Van Tharp's random test")]
	public class vtRandom : Strategy
	{
		#region Variables
		private int _period = 10; // Default setting for Period
		private double _multiplier = 3; // Default setting for Multiplier

		private double _commission = 4.4;
		
		#endregion

		private Random _rand = null;
		private EMAofATR _atr = null;
		
		private const int GO_LONG = 1;
		private const int GO_SHORT = 0;
		
		
	

		#region Ninjascript Events
		protected override void Initialize()
		{
			_rand = new Random(DateTime.Now.Millisecond);
			_atr = EMAofATR(_period);
			Add(_atr);
			
			this.ClearOutputWindow();
			
			CalculateOnBarClose = true;

			
		}
		
		protected override void OnStart()
		{
			switch (Instrument.MasterInstrument.Name)
			{
				case "ES":
				case "YM":
				case "NQ":
				case "TF":
				case "DX":
				case "ZB":
				case "ZN":
					_commission = 4.4;
					break;
				case "CL":
				case "NG":
					_commission = 5.02;
					break;
				case "6E":
				case "6A":
				case "6B":
				case "6S":
				case "6C":
				case "6M":
					_commission = 5.32;
					break;
				default:
					_commission = 4.4;
					break;
			}
		}

		protected override void OnBarUpdate()
		{
			if (StillHaveMoney)
			{
				double risk = _atr.EMAPlot[0] * _multiplier;
				
				if (IsFlat) 
					EnterTrade(risk);

				ManageTrade(risk);
			}
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

				double profit = ((diff * this.PointValue) - _commission) * _entry.Quantity;
				_equity += profit;
				
//				P("Profit=" + profit.ToString("C2") + ", Equity=" + _equity.ToString("C2"));
			}
		}
		#endregion
		
		private void EnterTrade(double risk)
		{
			int quantity = ComputeQty(risk);
			int toss = _rand.Next(2);
			
			if (toss == GO_LONG)
			{
				GoFlat();
				_lossLevel = Close[0] - risk;
				SetStopLoss(CalculationMode.Price, _lossLevel);
				_entry = EnterLong(quantity);
			}
			else if (toss == GO_SHORT)
			{
				GoFlat();
				_lossLevel = Close[0] + risk;
				SetStopLoss(CalculationMode.Price, _lossLevel);
				_entry = EnterShort(quantity);
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
		



		
		#region Properties
		[Description("Period for EMA/ATR")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return _period; }
			set { _period = Math.Max(1, value); }
		}

		[Description("ATR multiplier")]
		[GridCategory("Parameters")]
		public double Multiplier
		{
			get { return _multiplier; }
			set { _multiplier = Math.Max(0, value); }
		}
		
	
		

		#endregion
	}
}
