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
using NinjaTrader.Strategy;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{


	[Description("Enter the description of your new custom indicator here")]
	public class ZeroLagMacIndi : Indicator
	{
		#region Variables
				// atr multiplier trail stop
		protected double _mmAtrMultiplier = 3.0;
		protected int _mmAtrPeriod = 10;
		protected int _mmAtrEMAperiod = 5;


		private TrendDirection _trendDirection = TrendDirection.Neutral;
		private ExitType _exitType = ExitType.InitialToBreakevenToTrailingATR;
		private TradeState _tradeState = TradeState.InitialStop;

		protected double _lossLevel;
		protected int _mmProfitTicksBeforeBreakeven = 25;
		protected int _mmInitialSL = 100;
		protected int _mmBreakevenTicks = 5;

		private double _positionPrice;

		private MACD_ZeroLag_Colors _macd;
		private RSI _rsi;

		private int _signal = 0;

		#endregion

	/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Green, PlotStyle.Line, "PositionPrice"));
			Add(new Plot(Color.Red, PlotStyle.Line, "StopLoss"));
			BarsRequired = 20;

			Overlay				= true;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar <= BarsRequired) return;

			_macd = MACD_ZeroLag_Colors(1, MACDFast, MACDSlow, MACDSmooth, 0);
			_rsi = RSI(RSIPeriod, RSISmooth);

			// P(_macd.MacdUp + " " + _macd.MacdDn);
			if (IsFlat)
			{
				LookForEntry();
			}
			else
			{
				LookForStop();
                LookForExit();
			}



			if (!IsFlat)
			{
				ManageTradeByTrailingATR();
				PositionPrice.Set(_positionPrice);
				StopLoss.Set(_lossLevel);
			   
			}

		}

		protected  void LookForEntry()
		{
			if (_macd.MacdUp[0] == 0 && Rising(_rsi) && Rising(_macd.Avg))
			{
				_tradeState = TradeState.InitialStop;
				_lossLevel = Low[0] - TickSize * _mmInitialSL;
				_positionPrice = Close[0];
				PositionPrice.Set(_positionPrice);
				StopLoss.Set(_lossLevel);
				SetSignal(1, "going long");
				return;
			}

			if (_macd.MacdDn[0] == 0 && Falling(_rsi) && Falling(_macd.Avg))
			{
				_tradeState = TradeState.InitialStop;
				_lossLevel = High[0] + TickSize * _mmInitialSL;
				_positionPrice = Close[0];
				PositionPrice.Set(_positionPrice);
				StopLoss.Set(_lossLevel);
				SetSignal(-1, "going short");
				return;
			}


		}

		protected void LookForStop()
		{
			if (IsLong)
			{
				if (Close[0] < _lossLevel)
				{
					SetSignal(0, "stoploss");
				}
			}

			if (IsShort)
			{
				if (Close[0] > _lossLevel)
				{
					SetSignal(0, "stoploss");
				}
			}
		}

		protected void LookForExit()
		{
			if (_macd.MacdNeutral[0] == 0)
			{
				SetSignal(0 , "exit nautral");
				return;
			}

			if (_trendDirection == TrendDirection.Long)
			{
				if (CrossBelow(RSIUpper, _rsi, 1))
				{
					SetSignal(0, "exit rsi");
					return;
				}

				if (CrossBelow(40, ADX(14), 1))
				{
					SetSignal(0, "exit adx");
					return;
				}
			}


			if (_trendDirection == TrendDirection.Short)
			{
				if (CrossAbove(RSILower, _rsi, 1))
				{
					SetSignal(0, "exit rsi");
					return;
				}

				if (CrossBelow(40, ADX(14), 1))
				{
					SetSignal(0, "exit adx");
					return;
				}
			}
		}


		protected void ManageTradeByTrailingATR()
		{

			double risk = EMA(ATR(MMAtrPeriod), MMAtrEMAPeriod)[0] * MMAtrMultiplier;

			if (IsLong)
			{
				switch (_tradeState)
				{
					case TradeState.InitialStop:
						// switch to breakeven if possible and start trailing
						if (Close[0] > _positionPrice + (TickSize * ProfitTicksBeforeBreakeven))
						{
							_lossLevel = _positionPrice + (TickSize * BreakevenTicks);
							StopLoss.Set(_lossLevel);
						//    SetStopLoss("long", CalculationMode.Price, _lossLevel, true);
							_tradeState = TradeState.Trailing;
						}
						break;

					case TradeState.Trailing:
						if (Low[0] - risk > _lossLevel)
						{
							_lossLevel = Low[0] - risk;
							StopLoss.Set(_lossLevel);
					 //       SetStopLoss("long", CalculationMode.Price, _lossLevel, true);
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
						if (Close[0] < _positionPrice - (TickSize * ProfitTicksBeforeBreakeven))
						{
							_lossLevel = _positionPrice - (TickSize * BreakevenTicks);
							StopLoss.Set(_lossLevel);
				   //         SetStopLoss("short", CalculationMode.Price, _lossLevel, true);
							_tradeState = TradeState.Trailing;
						}
						break;

					case TradeState.Trailing:
						if (High[0] + risk < _lossLevel)
						{
							_lossLevel = High[0] + risk;
							StopLoss.Set(_lossLevel);
			  //              SetStopLoss("short", CalculationMode.Price, _lossLevel, true);
						}
						break;
				}
			}
		  StopLoss.Set(_lossLevel);
		}



		private void SetSignal(int signal, string message)
		{
			if (message != null)
			{
				P(message);
			}
			_signal = signal;
			switch (_signal)
			{
				case 1:
					_trendDirection = TrendDirection.Long;
					BackColor = Color.LightGreen;
					break;
				case -1:
					_trendDirection = TrendDirection.Short;
					BackColor = Color.Pink;
				   
					 break;
				case 0:
					_trendDirection = TrendDirection.Neutral;
					BackColor = Color.Yellow;
					break;
				default:
					break;

			}
		}

		private void P(string message)
		{
			Print(Time[0] + ": " + message);
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries PositionPrice
		{
			get { return Values[0]; }

		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries StopLoss
		{
			get { return Values[1]; }
	 
		}
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
	public int Signal
		{
			get { return _signal; }
		}

		#region Properties

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


		[GridCategory("Money management")]
		public int BreakevenTicks
		{
			get { return _mmBreakevenTicks; }
			set { _mmBreakevenTicks = Math.Max(1, value); }
		}



		protected int _rsiPeriod = 10; // Default setting for RSIPeriod
		[Description("Period for RSI")]
		[GridCategory("ParametersBase")]
		public int RSIPeriod
		{
			get { return _rsiPeriod; }
			set { _rsiPeriod = Math.Max(1, value); }
		}

		protected int _rsiSmooth = 3; // Default setting for RSIPeriod
		[Description("RSI smoothing")]
		[GridCategory("ParametersBase")]
		public int RSISmooth
		{
			get { return _rsiSmooth; }
			set { _rsiSmooth = Math.Max(1, value); }
		}


		protected int _rsiLower = 45;
		[Description("Period for RSI lower")]
		[GridCategory("ParametersBase")]
		public int RSILower
		{
			get { return _rsiLower; }
			set { _rsiLower = Math.Max(1, value); }
		}

		protected int _rsiUpper = 55;
		[Description("Period for RSI upper")]
		[GridCategory("ParametersBase")]
		public int RSIUpper
		{
			get { return _rsiUpper; }
			set { _rsiUpper = Math.Max(1, value); }
		}


		protected int _macdFast = 12;
		[Description("Period for MACD Fast")]
		[GridCategory("ParametersBase")]
		public int MACDFast
		{
			get { return _macdFast; }
			set { _macdFast = Math.Max(1, value); }
		}
		protected int _macdSlow = 26;
		[Description("Period for MACD Slow")]
		[GridCategory("ParametersBase")]
		public int MACDSlow
		{
			get { return _macdSlow; }
			set { _macdSlow = Math.Max(1, value); }
		}
		protected int _macdSmooth = 9;
		[Description("Period for MACD smoothing")]
		[GridCategory("ParametersBase")]
		public int MACDSmooth
		{
			get { return _macdSmooth; }
			set { _macdSmooth = Math.Max(1, value); }
		}

		protected int _adxPeriod = 10; // Default setting for ADXPeriod
		[Description("Period for ADX")]
		[GridCategory("ParametersBase")]
		public int ADXPeriod
		{
			get { return _adxPeriod; }
			set { _adxPeriod = Math.Max(1, value); }
		}

		protected int _adxMin = 20;
		[Description("Minimum for ADX")]
		[GridCategory("ParametersBase")]
		public int ADXMinimum
		{
			get { return _adxMin; }
			set { _adxMin = Math.Max(1, value); }
		}


		protected int _atrPeriod = 10;
		[Description("Period for ATR")]
		[GridCategory("ParametersBase")]
		public int ATRPeriod
		{
			get { return _atrPeriod; }
			set { _atrPeriod = Math.Max(1, value); }
		}

				[Description("Period for trailing ATR")]
		[GridCategory("Money management")]
		public int MMAtrPeriod
		{
			get { return _mmAtrPeriod; }
			set { _mmAtrPeriod = Math.Max(1, value); }
		}

		[Description("Multiplier for trailing ATR")]
		[GridCategory("Money management")]
		public double MMAtrMultiplier
		{
			get { return _mmAtrMultiplier; }
			set { _mmAtrMultiplier = Math.Max(0.1 , value); }
		}

		[Description("Multiplier for trailing ATR")]
		[GridCategory("Money management")]
		public int MMAtrEMAPeriod
		{
			get { return _mmAtrEMAperiod; }
			set { _mmAtrEMAperiod = Math.Max(1, value); }
		}
		#endregion




		public bool IsLong { get { return _signal == 1; } }
		public bool IsShort { get { return _signal == -1; } }
		public bool IsFlat { get { return _signal == 0; } }

 
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	public partial class Indicator : IndicatorBase
	{
		private ZeroLagMacIndi[] cacheZeroLagMacIndi = null;

		private static ZeroLagMacIndi checkZeroLagMacIndi = new ZeroLagMacIndi();

		/// <summary>
		/// Enter the description of your new custom indicator here
		/// </summary>
		/// <returns></returns>
		public ZeroLagMacIndi ZeroLagMacIndi(int aDXMinimum, int aDXPeriod, int aTRPeriod, int breakevenTicks, int initialStoploss, int mACDFast, int mACDSlow, int mACDSmooth, int mMAtrEMAPeriod, double mMAtrMultiplier, int mMAtrPeriod, int profitTicksBeforeBreakeven, int rSILower, int rSIPeriod, int rSISmooth, int rSIUpper)
		{
			return ZeroLagMacIndi(Input, aDXMinimum, aDXPeriod, aTRPeriod, breakevenTicks, initialStoploss, mACDFast, mACDSlow, mACDSmooth, mMAtrEMAPeriod, mMAtrMultiplier, mMAtrPeriod, profitTicksBeforeBreakeven, rSILower, rSIPeriod, rSISmooth, rSIUpper);
		}

		/// <summary>
		/// Enter the description of your new custom indicator here
		/// </summary>
		/// <returns></returns>
		public ZeroLagMacIndi ZeroLagMacIndi(Data.IDataSeries input, int aDXMinimum, int aDXPeriod, int aTRPeriod, int breakevenTicks, int initialStoploss, int mACDFast, int mACDSlow, int mACDSmooth, int mMAtrEMAPeriod, double mMAtrMultiplier, int mMAtrPeriod, int profitTicksBeforeBreakeven, int rSILower, int rSIPeriod, int rSISmooth, int rSIUpper)
		{
			if (cacheZeroLagMacIndi != null)
				for (int idx = 0; idx < cacheZeroLagMacIndi.Length; idx++)
					if (cacheZeroLagMacIndi[idx].ADXMinimum == aDXMinimum && cacheZeroLagMacIndi[idx].ADXPeriod == aDXPeriod && cacheZeroLagMacIndi[idx].ATRPeriod == aTRPeriod && cacheZeroLagMacIndi[idx].BreakevenTicks == breakevenTicks && cacheZeroLagMacIndi[idx].InitialStoploss == initialStoploss && cacheZeroLagMacIndi[idx].MACDFast == mACDFast && cacheZeroLagMacIndi[idx].MACDSlow == mACDSlow && cacheZeroLagMacIndi[idx].MACDSmooth == mACDSmooth && cacheZeroLagMacIndi[idx].MMAtrEMAPeriod == mMAtrEMAPeriod && Math.Abs(cacheZeroLagMacIndi[idx].MMAtrMultiplier - mMAtrMultiplier) <= double.Epsilon && cacheZeroLagMacIndi[idx].MMAtrPeriod == mMAtrPeriod && cacheZeroLagMacIndi[idx].ProfitTicksBeforeBreakeven == profitTicksBeforeBreakeven && cacheZeroLagMacIndi[idx].RSILower == rSILower && cacheZeroLagMacIndi[idx].RSIPeriod == rSIPeriod && cacheZeroLagMacIndi[idx].RSISmooth == rSISmooth && cacheZeroLagMacIndi[idx].RSIUpper == rSIUpper && cacheZeroLagMacIndi[idx].EqualsInput(input))
						return cacheZeroLagMacIndi[idx];

			lock (checkZeroLagMacIndi)
			{
				checkZeroLagMacIndi.ADXMinimum = aDXMinimum;
				aDXMinimum = checkZeroLagMacIndi.ADXMinimum;
				checkZeroLagMacIndi.ADXPeriod = aDXPeriod;
				aDXPeriod = checkZeroLagMacIndi.ADXPeriod;
				checkZeroLagMacIndi.ATRPeriod = aTRPeriod;
				aTRPeriod = checkZeroLagMacIndi.ATRPeriod;
				checkZeroLagMacIndi.BreakevenTicks = breakevenTicks;
				breakevenTicks = checkZeroLagMacIndi.BreakevenTicks;
				checkZeroLagMacIndi.InitialStoploss = initialStoploss;
				initialStoploss = checkZeroLagMacIndi.InitialStoploss;
				checkZeroLagMacIndi.MACDFast = mACDFast;
				mACDFast = checkZeroLagMacIndi.MACDFast;
				checkZeroLagMacIndi.MACDSlow = mACDSlow;
				mACDSlow = checkZeroLagMacIndi.MACDSlow;
				checkZeroLagMacIndi.MACDSmooth = mACDSmooth;
				mACDSmooth = checkZeroLagMacIndi.MACDSmooth;
				checkZeroLagMacIndi.MMAtrEMAPeriod = mMAtrEMAPeriod;
				mMAtrEMAPeriod = checkZeroLagMacIndi.MMAtrEMAPeriod;
				checkZeroLagMacIndi.MMAtrMultiplier = mMAtrMultiplier;
				mMAtrMultiplier = checkZeroLagMacIndi.MMAtrMultiplier;
				checkZeroLagMacIndi.MMAtrPeriod = mMAtrPeriod;
				mMAtrPeriod = checkZeroLagMacIndi.MMAtrPeriod;
				checkZeroLagMacIndi.ProfitTicksBeforeBreakeven = profitTicksBeforeBreakeven;
				profitTicksBeforeBreakeven = checkZeroLagMacIndi.ProfitTicksBeforeBreakeven;
				checkZeroLagMacIndi.RSILower = rSILower;
				rSILower = checkZeroLagMacIndi.RSILower;
				checkZeroLagMacIndi.RSIPeriod = rSIPeriod;
				rSIPeriod = checkZeroLagMacIndi.RSIPeriod;
				checkZeroLagMacIndi.RSISmooth = rSISmooth;
				rSISmooth = checkZeroLagMacIndi.RSISmooth;
				checkZeroLagMacIndi.RSIUpper = rSIUpper;
				rSIUpper = checkZeroLagMacIndi.RSIUpper;

				if (cacheZeroLagMacIndi != null)
					for (int idx = 0; idx < cacheZeroLagMacIndi.Length; idx++)
						if (cacheZeroLagMacIndi[idx].ADXMinimum == aDXMinimum && cacheZeroLagMacIndi[idx].ADXPeriod == aDXPeriod && cacheZeroLagMacIndi[idx].ATRPeriod == aTRPeriod && cacheZeroLagMacIndi[idx].BreakevenTicks == breakevenTicks && cacheZeroLagMacIndi[idx].InitialStoploss == initialStoploss && cacheZeroLagMacIndi[idx].MACDFast == mACDFast && cacheZeroLagMacIndi[idx].MACDSlow == mACDSlow && cacheZeroLagMacIndi[idx].MACDSmooth == mACDSmooth && cacheZeroLagMacIndi[idx].MMAtrEMAPeriod == mMAtrEMAPeriod && Math.Abs(cacheZeroLagMacIndi[idx].MMAtrMultiplier - mMAtrMultiplier) <= double.Epsilon && cacheZeroLagMacIndi[idx].MMAtrPeriod == mMAtrPeriod && cacheZeroLagMacIndi[idx].ProfitTicksBeforeBreakeven == profitTicksBeforeBreakeven && cacheZeroLagMacIndi[idx].RSILower == rSILower && cacheZeroLagMacIndi[idx].RSIPeriod == rSIPeriod && cacheZeroLagMacIndi[idx].RSISmooth == rSISmooth && cacheZeroLagMacIndi[idx].RSIUpper == rSIUpper && cacheZeroLagMacIndi[idx].EqualsInput(input))
							return cacheZeroLagMacIndi[idx];

				ZeroLagMacIndi indicator = new ZeroLagMacIndi();
				indicator.BarsRequired = BarsRequired;
				indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
				indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
				indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
				indicator.Input = input;
				indicator.ADXMinimum = aDXMinimum;
				indicator.ADXPeriod = aDXPeriod;
				indicator.ATRPeriod = aTRPeriod;
				indicator.BreakevenTicks = breakevenTicks;
				indicator.InitialStoploss = initialStoploss;
				indicator.MACDFast = mACDFast;
				indicator.MACDSlow = mACDSlow;
				indicator.MACDSmooth = mACDSmooth;
				indicator.MMAtrEMAPeriod = mMAtrEMAPeriod;
				indicator.MMAtrMultiplier = mMAtrMultiplier;
				indicator.MMAtrPeriod = mMAtrPeriod;
				indicator.ProfitTicksBeforeBreakeven = profitTicksBeforeBreakeven;
				indicator.RSILower = rSILower;
				indicator.RSIPeriod = rSIPeriod;
				indicator.RSISmooth = rSISmooth;
				indicator.RSIUpper = rSIUpper;
				Indicators.Add(indicator);
				indicator.SetUp();

				ZeroLagMacIndi[] tmp = new ZeroLagMacIndi[cacheZeroLagMacIndi == null ? 1 : cacheZeroLagMacIndi.Length + 1];
				if (cacheZeroLagMacIndi != null)
					cacheZeroLagMacIndi.CopyTo(tmp, 0);
				tmp[tmp.Length - 1] = indicator;
				cacheZeroLagMacIndi = tmp;
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
		public Indicator.ZeroLagMacIndi ZeroLagMacIndi(int aDXMinimum, int aDXPeriod, int aTRPeriod, int breakevenTicks, int initialStoploss, int mACDFast, int mACDSlow, int mACDSmooth, int mMAtrEMAPeriod, double mMAtrMultiplier, int mMAtrPeriod, int profitTicksBeforeBreakeven, int rSILower, int rSIPeriod, int rSISmooth, int rSIUpper)
		{
			return _indicator.ZeroLagMacIndi(Input, aDXMinimum, aDXPeriod, aTRPeriod, breakevenTicks, initialStoploss, mACDFast, mACDSlow, mACDSmooth, mMAtrEMAPeriod, mMAtrMultiplier, mMAtrPeriod, profitTicksBeforeBreakeven, rSILower, rSIPeriod, rSISmooth, rSIUpper);
		}

		/// <summary>
		/// Enter the description of your new custom indicator here
		/// </summary>
		/// <returns></returns>
		public Indicator.ZeroLagMacIndi ZeroLagMacIndi(Data.IDataSeries input, int aDXMinimum, int aDXPeriod, int aTRPeriod, int breakevenTicks, int initialStoploss, int mACDFast, int mACDSlow, int mACDSmooth, int mMAtrEMAPeriod, double mMAtrMultiplier, int mMAtrPeriod, int profitTicksBeforeBreakeven, int rSILower, int rSIPeriod, int rSISmooth, int rSIUpper)
		{
			return _indicator.ZeroLagMacIndi(input, aDXMinimum, aDXPeriod, aTRPeriod, breakevenTicks, initialStoploss, mACDFast, mACDSlow, mACDSmooth, mMAtrEMAPeriod, mMAtrMultiplier, mMAtrPeriod, profitTicksBeforeBreakeven, rSILower, rSIPeriod, rSISmooth, rSIUpper);
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
		public Indicator.ZeroLagMacIndi ZeroLagMacIndi(int aDXMinimum, int aDXPeriod, int aTRPeriod, int breakevenTicks, int initialStoploss, int mACDFast, int mACDSlow, int mACDSmooth, int mMAtrEMAPeriod, double mMAtrMultiplier, int mMAtrPeriod, int profitTicksBeforeBreakeven, int rSILower, int rSIPeriod, int rSISmooth, int rSIUpper)
		{
			return _indicator.ZeroLagMacIndi(Input, aDXMinimum, aDXPeriod, aTRPeriod, breakevenTicks, initialStoploss, mACDFast, mACDSlow, mACDSmooth, mMAtrEMAPeriod, mMAtrMultiplier, mMAtrPeriod, profitTicksBeforeBreakeven, rSILower, rSIPeriod, rSISmooth, rSIUpper);
		}

		/// <summary>
		/// Enter the description of your new custom indicator here
		/// </summary>
		/// <returns></returns>
		public Indicator.ZeroLagMacIndi ZeroLagMacIndi(Data.IDataSeries input, int aDXMinimum, int aDXPeriod, int aTRPeriod, int breakevenTicks, int initialStoploss, int mACDFast, int mACDSlow, int mACDSmooth, int mMAtrEMAPeriod, double mMAtrMultiplier, int mMAtrPeriod, int profitTicksBeforeBreakeven, int rSILower, int rSIPeriod, int rSISmooth, int rSIUpper)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return _indicator.ZeroLagMacIndi(input, aDXMinimum, aDXPeriod, aTRPeriod, breakevenTicks, initialStoploss, mACDFast, mACDSlow, mACDSmooth, mMAtrEMAPeriod, mMAtrMultiplier, mMAtrPeriod, profitTicksBeforeBreakeven, rSILower, rSIPeriod, rSISmooth, rSIUpper);
		}
	}
}
#endregion
