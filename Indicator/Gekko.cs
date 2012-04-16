#region Using declarations
using System;
using System.ComponentModel;
using System.Collections.Generic;
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
	/// My collective strategy
	/// </summary>
	[Description("My collective strategy")]
	public class Gekko : Indicator
	{
		#region Variables

		private double candlestickWeight = 5;
		private int candlestickTrendStrength = 3; // Default setting for CandlestickTrendStrength
		private int candlestickBarsAgo = 0; // Default setting for CandlestickBarsAgo
		private int candlesticksLookback = 3; // lookback period for candlesticks
		private BopaCandlesticksRules csrules;  // place holder for the selected CS patterns to be recognized
	
		private double adx;
		private double adxWeight = 5;
		private int aDXPeriod = 14; // Default setting for ADXPeriod
		private int aDXLow = 20; // Default setting for ADXLow
		private int aDXHigh = 40; // Default setting for ADXHigh

		private double rsi;
		private double rsiWeight = 5;
		private int _RSIPeriod = 14; // Default setting for RSIPeriod
		private int _RSILow = 35; // Default setting for RSILow
		private int _RSIHigh = 65; // Default setting for RSIHigh
		private int _RSIsmooth = 3;
		private int _rsiLookback = 1; // lookback period for crossovers
		
		private double macdWeight = 5;
		private int macdSlow = 26;
		private int macdFast = 12;
		private int macdSignal = 9;
		
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Black), PlotStyle.Line, "Total"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "PlotCS"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "PlotADX"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "PlotRSI"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Yellow), PlotStyle.Line, "PlotMACD"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "PlotMACDavg"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Bar, "PlotMACDdioff"));
			
			Overlay				= false;
			CalculateOnBarClose = true;
			
			csrules = new BopaCandlesticksRules(candlestickTrendStrength, candlestickBarsAgo);
			
			
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			double totalweight = 0;

			int csweight = DrawCandlesticks();
			totalweight += csweight;
			
			int adxweight = GetADXval();
			totalweight += adxweight;
			
			// int rsiweight = GetRSIval();
			int rsiweight = DidRSICrossBackAbove();
			rsiweight += DidRSICrossBackBelow();
			totalweight += rsiweight;
			
//		   	PlotTotal.Set(totalweight);
//		  	PlotCS.Set(csweight);
//		  	PlotADX.Set(adxweight);
//	 	  	PlotRSI.Set(rsiweight);
//		
			DoMCAD();
			
//		  	Value.Set(totalweight);
		}

		protected double DoMCAD()
		{
			MACD _macd = MACD(MACDFastMA, MACDSlowMA, MACDSignalMA);
			
			PlotMACD.Set(_macd[0]);
		
			// 5 avg
			Values[5].Set(_macd.Avg[0]);	
			
			// 6 diff = histogram
			Values[6].Set(_macd.Diff[0]);
			
			
			// cross signal
			
			// cross SMA
			
			return 0;
		}
		
		#region Properties
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries PlotTotal
		{
			get { return Values[0]; }
		}
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries PlotCS
		{
			get { return Values[1]; }
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries PlotADX
		{
			get { return Values[2]; }
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries PlotRSI
		{
			get { return Values[3]; }
		}
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries PlotMACD
		{
			get { return Values[4]; }
		}
		
		[Description("")]
		[GridCategory("Parameters")]
		public double CandlestickWeight
		{
			get { return candlestickWeight; }
			set { candlestickWeight = Math.Max(0, value); }
		}
		
		[Description("")]
		[GridCategory("Parameters")]
		public int CandlestickTrendStrength
		{
			get { return candlestickTrendStrength; }
			set { candlestickTrendStrength = Math.Max(1, value); }
		}

		[Description("")]
		[GridCategory("Parameters")]
		public int CandlestickBarsAgo
		{
			get { return candlestickBarsAgo; }
			set { candlestickBarsAgo = Math.Max(0, value); }
		}
		
		[Description("")]
		[GridCategory("Parameters")]
		public int CandlestickLookback
		{
			get { return candlesticksLookback; }
			set { candlesticksLookback = Math.Max(0, value); }
		}
		
		[Description("Weight for ADX")]
		[GridCategory("Parameters")]
		public double ADXWeight
		{
			get { return adxWeight; }
			set { adxWeight = Math.Max(0, value); }
		}
		
		[Description("Number of bars for ADX")]
		[GridCategory("Parameters")]
		public int ADXPeriod
		{
			get { return aDXPeriod; }
			set { aDXPeriod = Math.Max(1, value); }
		}

		[Description("Low range for ADX")]
		[GridCategory("Parameters")]
		public int ADXLow
		{
			get { return aDXLow; }
			set { aDXLow = Math.Max(1, value); }
		}

		[Description("High range for ADX")]
		[GridCategory("Parameters")]
		public int ADXHigh
		{
			get { return aDXHigh; }
			set { aDXHigh = Math.Max(1, value); }
		}
		
		[Description("RSI high range")]
		[GridCategory("Parameters")]
		public double RSIWeight
		{
			get { return rsiWeight; }
			set { rsiWeight = Math.Max(0, value); }
		}
		
		[Description("RSI high range")]
		[GridCategory("Parameters")]
		public int RSIHigh
		{
			get { return _RSIHigh; }
			set { _RSIHigh = Math.Max(1, value); }
		}
		
		 [Description("RSI Low range")]
		[GridCategory("Parameters")]
		public int RSILow
		{
			get { return _RSILow; }
			set { _RSILow = Math.Max(1, value); }
		}
		
		[Description("RSI Period")]
		[GridCategory("Parameters")]
		public int RSIPeriod
		{
			get { return _RSIPeriod; }
			set { _RSIPeriod = Math.Max(1, value); }
		}
		
		[Description("RSI Smooth")]
		[GridCategory("Parameters")]
		public int RSISmooth
		{
			get { return _RSIsmooth; }
			set { _RSIsmooth = Math.Max(0, value); }
		}
	
		[Description("RSI Crossover lookback")]
		[GridCategory("Parameters")]
		public int RSILookback
		{
			get { return _rsiLookback; }
			set { _rsiLookback = Math.Max(0, value); }
		}
		
		[Description("MACD Weight")]
		[GridCategory("Parameters MACD")]
		public double MACDWeight
		{
			get { return macdWeight; }
			set { macdWeight = Math.Max(0, value); }
		}
		
		[Description("MACD Fast MA")]
		[GridCategory("Parameters MACD")]
		public int MACDFastMA
		{
			get { return macdFast; }
			set { macdFast = Math.Max(1, value); }
		}
		[Description("MACD Slow MA")]
		[GridCategory("Parameters MACD")]
		public int MACDSlowMA
		{
			get { return macdSlow; }
			set { macdSlow = Math.Max(1, value); }
		}
		[Description("MACD Signal")]
		[GridCategory("Parameters MACD")]
		public int MACDSignalMA
		{
			get { return macdSignal; }
			set { macdSignal = Math.Max(1, value); }
		}
		
		
		
		
		#endregion
	
		
		
		#region ADX stuff
		private int GetADXval()
		{
			if (isADXhigh()) return 1;
			if (isADXlow()) return -1;
			return 0;
		}
		
		private bool isADXlow()
		{
			return ADX(aDXPeriod)[0] < ADXLow;
		}
		
		private bool isADXhigh()
		{
			return ADX(aDXPeriod)[0] > ADXHigh;
		}
		

		#endregion
		
		#region rsi stuff
		private int GetRSIval()
		{
			if (IsRSIhigh()) return -2;
			if (IsRSIlow()) return 2;
			return 0;
		}
		private bool IsRSIlow()
		{
			return RSI(_RSIPeriod, _RSIsmooth)[0] < RSILow;
		}
		
		private bool IsRSIhigh()
		{
			return RSI(_RSIPeriod, _RSIsmooth)[0] > RSIHigh;
		}
		
		private int DidRSICrossAbove()
		{
			if(CrossAbove(RSI(RSIPeriod, RSISmooth),RSIHigh, RSILookback)) {return -3;}
			return 0;
		}
		
		private int DidRSICrossBelow()
		{
			if(CrossBelow(RSI(RSIPeriod, RSISmooth),RSILow, RSILookback)) {return 3;}
			return 0;
		}
		private int DidRSICrossBackAbove()
		{
			if(CrossAbove(RSI(RSIPeriod, RSISmooth),RSILow, RSILookback)) {return 3;}
			return 0;
		}
		
		private int DidRSICrossBackBelow()
		{
			if(CrossBelow(RSI(RSIPeriod, RSISmooth),RSIHigh, RSILookback)) {return -3;}
		
			return 0;
		}
		
		private int DidRSICrossAbove50()
		{
			if(CrossAbove(RSI(RSIPeriod, RSISmooth),50, RSILookback)) {return 3;}
			return 0;
		}
		
		private int DidRSICrossBelow50()
		{
			if(CrossBelow(RSI(RSIPeriod, RSISmooth),50, RSILookback)) {return -3;}
			return 0;
		}

	
		#endregion
		
		#region candlestick stuff
		
		protected int DrawCandlesticks()
		{
			int totalweight = 0;
			
			foreach (BopaCandlesticksRule pattern in csrules)	
			{
				
				 if (CrossAbove(CandleStickPattern(pattern.Pattern, pattern.TrendStrength).PatternFound, 0, CandlestickLookback))
		//		if (CandleStickPattern(pattern.Pattern, pattern.TrendStrength)[candlestickBarsAgo] == 1)
				{
					
			//		Print(Time[0] + " - " + pattern.Pattern);
			
					totalweight = totalweight + pattern.Weight;

					if(pattern.Weight < 0)
					{
						DrawText(pattern.Name, true, pattern.Name, 0, High[0], 20, Color.Red, new Font("Arial", 12, FontStyle.Regular), StringAlignment.Center, Color.Black, Color.Wheat, 50);
					} 
					else if(pattern.Weight > 0 ) 
					{
						DrawText(pattern.Name, true, pattern.Name, 0, Low[0], -15, Color.Green, new Font("Arial", 12, FontStyle.Regular), StringAlignment.Center, Color.Black, Color.Wheat, 50);
					} 
					else if (pattern.Weight == 0)
					{
						DrawText(pattern.Name, true, pattern.Name, 0, Low[0], -15, Color.Blue, new Font("Arial", 12, FontStyle.Regular), StringAlignment.Center, Color.Black, Color.White, 10);
					} 
					else 
					{
						Print("wtf gekko cs?");
					}
				}
			}
			return totalweight;
		}
		
	  public class BopaCandlesticksRules : List<BopaCandlesticksRule>
	{
		private int _trendStrength;
		private int _barsAgo;
 
		public BopaCandlesticksRules(int TrendStrength, int barsAgo)
		{
			this._trendStrength = TrendStrength;
			this._barsAgo = barsAgo;
			
		//	Add(new BopaCandlesticksRule(ChartPattern.BearishBeltHold, "BeBH", _trendStrength, _barsAgo, 1));
			Add(new BopaCandlesticksRule(ChartPattern.BearishEngulfing, "BeE", _trendStrength, _barsAgo, -3));
			Add(new BopaCandlesticksRule(ChartPattern.BearishHarami, "BeH", _trendStrength, _barsAgo, -1));
			Add(new BopaCandlesticksRule(ChartPattern.BearishHaramiCross, "BeHC", _trendStrength, _barsAgo, -1));
			Add(new BopaCandlesticksRule(ChartPattern.BullishBeltHold, "BuBH", _trendStrength, _barsAgo, 1));
			Add(new BopaCandlesticksRule(ChartPattern.BullishEngulfing, "BuE", _trendStrength, _barsAgo, 3));
			Add(new BopaCandlesticksRule(ChartPattern.BullishHarami, "BuH", _trendStrength, _barsAgo, 1));
			Add(new BopaCandlesticksRule(ChartPattern.BullishHaramiCross, "BuHC", _trendStrength, _barsAgo, 1));
			Add(new BopaCandlesticksRule(ChartPattern.DarkCloudCover, "DCC", _trendStrength, _barsAgo, -2));
			Add(new BopaCandlesticksRule(ChartPattern.Doji, "Doji", _trendStrength, _barsAgo, 0));
	//		Add(new BopaCandlesticksRule(ChartPattern.DownsideTasukiGap, "DTG", _trendStrength, _barsAgo, 1));
			Add(new BopaCandlesticksRule(ChartPattern.EveningStar, "ES", _trendStrength, _barsAgo, -2));
	//		Add(new BopaCandlesticksRule(ChartPattern.FallingThreeMethods, "FTM", _trendStrength, _barsAgo, 1));
			Add(new BopaCandlesticksRule(ChartPattern.Hammer, "H", _trendStrength, _barsAgo, 1));
			Add(new BopaCandlesticksRule(ChartPattern.HangingMan, "HM", _trendStrength, _barsAgo, -1));
			Add(new BopaCandlesticksRule(ChartPattern.InvertedHammer, "IH", _trendStrength, _barsAgo, -1));
			Add(new BopaCandlesticksRule(ChartPattern.MorningStar, "MS", _trendStrength, _barsAgo, 2));
			Add(new BopaCandlesticksRule(ChartPattern.PiercingLine, "PL", _trendStrength, _barsAgo, 2));
//			Add(new BopaCandlesticksRule(ChartPattern.RisingThreeMethods, "RTM", _trendStrength, _barsAgo, 1));
			Add(new BopaCandlesticksRule(ChartPattern.ShootingStar, "SS", _trendStrength, _barsAgo, 1));
//			Add(new BopaCandlesticksRule(ChartPattern.StickSandwich, "StS", _trendStrength, _barsAgo, 1));
			Add(new BopaCandlesticksRule(ChartPattern.ThreeBlackCrows, "TBC", _trendStrength, _barsAgo, -2));
			Add(new BopaCandlesticksRule(ChartPattern.ThreeWhiteSoldiers, "TWS", _trendStrength, _barsAgo, 2));
//			Add(new BopaCandlesticksRule(ChartPattern.UpsideGapTwoCrows, "UGTC", _trendStrength, _barsAgo, 1));
//			Add(new BopaCandlesticksRule(ChartPattern.UpsideTasukiGap, "UTG", _trendStrength, _barsAgo, 1));
		}

		
	}

	public class BopaCandlesticksRule {
		public ChartPattern Pattern;
		public string Name;
		
		public int TrendStrength;
		public int BarsAgo;
		public int Weight;
		public int Direction;
			
		public BopaCandlesticksRule(ChartPattern Pattern, string Name, int TrendStrength, int BarsAgo, int Weight)
		{
			this.Pattern = Pattern;
			this.Name = Name;
			this.TrendStrength = TrendStrength;
			this.BarsAgo = BarsAgo;
			this.Weight = Weight;
			 
			
		}
	 
		 
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
		private Gekko[] cacheGekko = null;

		private static Gekko checkGekko = new Gekko();

		/// <summary>
		/// My collective strategy
		/// </summary>
		/// <returns></returns>
		public Gekko Gekko(int aDXHigh, int aDXLow, int aDXPeriod, double aDXWeight, int candlestickBarsAgo, int candlestickLookback, int candlestickTrendStrength, double candlestickWeight, int mACDFastMA, int mACDSignalMA, int mACDSlowMA, double mACDWeight, int rSIHigh, int rSILookback, int rSILow, int rSIPeriod, int rSISmooth, double rSIWeight)
		{
			return Gekko(Input, aDXHigh, aDXLow, aDXPeriod, aDXWeight, candlestickBarsAgo, candlestickLookback, candlestickTrendStrength, candlestickWeight, mACDFastMA, mACDSignalMA, mACDSlowMA, mACDWeight, rSIHigh, rSILookback, rSILow, rSIPeriod, rSISmooth, rSIWeight);
		}

		/// <summary>
		/// My collective strategy
		/// </summary>
		/// <returns></returns>
		public Gekko Gekko(Data.IDataSeries input, int aDXHigh, int aDXLow, int aDXPeriod, double aDXWeight, int candlestickBarsAgo, int candlestickLookback, int candlestickTrendStrength, double candlestickWeight, int mACDFastMA, int mACDSignalMA, int mACDSlowMA, double mACDWeight, int rSIHigh, int rSILookback, int rSILow, int rSIPeriod, int rSISmooth, double rSIWeight)
		{
			if (cacheGekko != null)
				for (int idx = 0; idx < cacheGekko.Length; idx++)
					if (cacheGekko[idx].ADXHigh == aDXHigh && cacheGekko[idx].ADXLow == aDXLow && cacheGekko[idx].ADXPeriod == aDXPeriod && Math.Abs(cacheGekko[idx].ADXWeight - aDXWeight) <= double.Epsilon && cacheGekko[idx].CandlestickBarsAgo == candlestickBarsAgo && cacheGekko[idx].CandlestickLookback == candlestickLookback && cacheGekko[idx].CandlestickTrendStrength == candlestickTrendStrength && Math.Abs(cacheGekko[idx].CandlestickWeight - candlestickWeight) <= double.Epsilon && cacheGekko[idx].MACDFastMA == mACDFastMA && cacheGekko[idx].MACDSignalMA == mACDSignalMA && cacheGekko[idx].MACDSlowMA == mACDSlowMA && Math.Abs(cacheGekko[idx].MACDWeight - mACDWeight) <= double.Epsilon && cacheGekko[idx].RSIHigh == rSIHigh && cacheGekko[idx].RSILookback == rSILookback && cacheGekko[idx].RSILow == rSILow && cacheGekko[idx].RSIPeriod == rSIPeriod && cacheGekko[idx].RSISmooth == rSISmooth && Math.Abs(cacheGekko[idx].RSIWeight - rSIWeight) <= double.Epsilon && cacheGekko[idx].EqualsInput(input))
						return cacheGekko[idx];

			lock (checkGekko)
			{
				checkGekko.ADXHigh = aDXHigh;
				aDXHigh = checkGekko.ADXHigh;
				checkGekko.ADXLow = aDXLow;
				aDXLow = checkGekko.ADXLow;
				checkGekko.ADXPeriod = aDXPeriod;
				aDXPeriod = checkGekko.ADXPeriod;
				checkGekko.ADXWeight = aDXWeight;
				aDXWeight = checkGekko.ADXWeight;
				checkGekko.CandlestickBarsAgo = candlestickBarsAgo;
				candlestickBarsAgo = checkGekko.CandlestickBarsAgo;
				checkGekko.CandlestickLookback = candlestickLookback;
				candlestickLookback = checkGekko.CandlestickLookback;
				checkGekko.CandlestickTrendStrength = candlestickTrendStrength;
				candlestickTrendStrength = checkGekko.CandlestickTrendStrength;
				checkGekko.CandlestickWeight = candlestickWeight;
				candlestickWeight = checkGekko.CandlestickWeight;
				checkGekko.MACDFastMA = mACDFastMA;
				mACDFastMA = checkGekko.MACDFastMA;
				checkGekko.MACDSignalMA = mACDSignalMA;
				mACDSignalMA = checkGekko.MACDSignalMA;
				checkGekko.MACDSlowMA = mACDSlowMA;
				mACDSlowMA = checkGekko.MACDSlowMA;
				checkGekko.MACDWeight = mACDWeight;
				mACDWeight = checkGekko.MACDWeight;
				checkGekko.RSIHigh = rSIHigh;
				rSIHigh = checkGekko.RSIHigh;
				checkGekko.RSILookback = rSILookback;
				rSILookback = checkGekko.RSILookback;
				checkGekko.RSILow = rSILow;
				rSILow = checkGekko.RSILow;
				checkGekko.RSIPeriod = rSIPeriod;
				rSIPeriod = checkGekko.RSIPeriod;
				checkGekko.RSISmooth = rSISmooth;
				rSISmooth = checkGekko.RSISmooth;
				checkGekko.RSIWeight = rSIWeight;
				rSIWeight = checkGekko.RSIWeight;

				if (cacheGekko != null)
					for (int idx = 0; idx < cacheGekko.Length; idx++)
						if (cacheGekko[idx].ADXHigh == aDXHigh && cacheGekko[idx].ADXLow == aDXLow && cacheGekko[idx].ADXPeriod == aDXPeriod && Math.Abs(cacheGekko[idx].ADXWeight - aDXWeight) <= double.Epsilon && cacheGekko[idx].CandlestickBarsAgo == candlestickBarsAgo && cacheGekko[idx].CandlestickLookback == candlestickLookback && cacheGekko[idx].CandlestickTrendStrength == candlestickTrendStrength && Math.Abs(cacheGekko[idx].CandlestickWeight - candlestickWeight) <= double.Epsilon && cacheGekko[idx].MACDFastMA == mACDFastMA && cacheGekko[idx].MACDSignalMA == mACDSignalMA && cacheGekko[idx].MACDSlowMA == mACDSlowMA && Math.Abs(cacheGekko[idx].MACDWeight - mACDWeight) <= double.Epsilon && cacheGekko[idx].RSIHigh == rSIHigh && cacheGekko[idx].RSILookback == rSILookback && cacheGekko[idx].RSILow == rSILow && cacheGekko[idx].RSIPeriod == rSIPeriod && cacheGekko[idx].RSISmooth == rSISmooth && Math.Abs(cacheGekko[idx].RSIWeight - rSIWeight) <= double.Epsilon && cacheGekko[idx].EqualsInput(input))
							return cacheGekko[idx];

				Gekko indicator = new Gekko();
				indicator.BarsRequired = BarsRequired;
				indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
				indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
				indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
				indicator.Input = input;
				indicator.ADXHigh = aDXHigh;
				indicator.ADXLow = aDXLow;
				indicator.ADXPeriod = aDXPeriod;
				indicator.ADXWeight = aDXWeight;
				indicator.CandlestickBarsAgo = candlestickBarsAgo;
				indicator.CandlestickLookback = candlestickLookback;
				indicator.CandlestickTrendStrength = candlestickTrendStrength;
				indicator.CandlestickWeight = candlestickWeight;
				indicator.MACDFastMA = mACDFastMA;
				indicator.MACDSignalMA = mACDSignalMA;
				indicator.MACDSlowMA = mACDSlowMA;
				indicator.MACDWeight = mACDWeight;
				indicator.RSIHigh = rSIHigh;
				indicator.RSILookback = rSILookback;
				indicator.RSILow = rSILow;
				indicator.RSIPeriod = rSIPeriod;
				indicator.RSISmooth = rSISmooth;
				indicator.RSIWeight = rSIWeight;
				Indicators.Add(indicator);
				indicator.SetUp();

				Gekko[] tmp = new Gekko[cacheGekko == null ? 1 : cacheGekko.Length + 1];
				if (cacheGekko != null)
					cacheGekko.CopyTo(tmp, 0);
				tmp[tmp.Length - 1] = indicator;
				cacheGekko = tmp;
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
		/// My collective strategy
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.Gekko Gekko(int aDXHigh, int aDXLow, int aDXPeriod, double aDXWeight, int candlestickBarsAgo, int candlestickLookback, int candlestickTrendStrength, double candlestickWeight, int mACDFastMA, int mACDSignalMA, int mACDSlowMA, double mACDWeight, int rSIHigh, int rSILookback, int rSILow, int rSIPeriod, int rSISmooth, double rSIWeight)
		{
			return _indicator.Gekko(Input, aDXHigh, aDXLow, aDXPeriod, aDXWeight, candlestickBarsAgo, candlestickLookback, candlestickTrendStrength, candlestickWeight, mACDFastMA, mACDSignalMA, mACDSlowMA, mACDWeight, rSIHigh, rSILookback, rSILow, rSIPeriod, rSISmooth, rSIWeight);
		}

		/// <summary>
		/// My collective strategy
		/// </summary>
		/// <returns></returns>
		public Indicator.Gekko Gekko(Data.IDataSeries input, int aDXHigh, int aDXLow, int aDXPeriod, double aDXWeight, int candlestickBarsAgo, int candlestickLookback, int candlestickTrendStrength, double candlestickWeight, int mACDFastMA, int mACDSignalMA, int mACDSlowMA, double mACDWeight, int rSIHigh, int rSILookback, int rSILow, int rSIPeriod, int rSISmooth, double rSIWeight)
		{
			return _indicator.Gekko(input, aDXHigh, aDXLow, aDXPeriod, aDXWeight, candlestickBarsAgo, candlestickLookback, candlestickTrendStrength, candlestickWeight, mACDFastMA, mACDSignalMA, mACDSlowMA, mACDWeight, rSIHigh, rSILookback, rSILow, rSIPeriod, rSISmooth, rSIWeight);
		}
	}
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
	public partial class Strategy : StrategyBase
	{
		/// <summary>
		/// My collective strategy
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.Gekko Gekko(int aDXHigh, int aDXLow, int aDXPeriod, double aDXWeight, int candlestickBarsAgo, int candlestickLookback, int candlestickTrendStrength, double candlestickWeight, int mACDFastMA, int mACDSignalMA, int mACDSlowMA, double mACDWeight, int rSIHigh, int rSILookback, int rSILow, int rSIPeriod, int rSISmooth, double rSIWeight)
		{
			return _indicator.Gekko(Input, aDXHigh, aDXLow, aDXPeriod, aDXWeight, candlestickBarsAgo, candlestickLookback, candlestickTrendStrength, candlestickWeight, mACDFastMA, mACDSignalMA, mACDSlowMA, mACDWeight, rSIHigh, rSILookback, rSILow, rSIPeriod, rSISmooth, rSIWeight);
		}

		/// <summary>
		/// My collective strategy
		/// </summary>
		/// <returns></returns>
		public Indicator.Gekko Gekko(Data.IDataSeries input, int aDXHigh, int aDXLow, int aDXPeriod, double aDXWeight, int candlestickBarsAgo, int candlestickLookback, int candlestickTrendStrength, double candlestickWeight, int mACDFastMA, int mACDSignalMA, int mACDSlowMA, double mACDWeight, int rSIHigh, int rSILookback, int rSILow, int rSIPeriod, int rSISmooth, double rSIWeight)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return _indicator.Gekko(input, aDXHigh, aDXLow, aDXPeriod, aDXWeight, candlestickBarsAgo, candlestickLookback, candlestickTrendStrength, candlestickWeight, mACDFastMA, mACDSignalMA, mACDSlowMA, mACDWeight, rSIHigh, rSILookback, rSILow, rSIPeriod, rSISmooth, rSIWeight);
		}
	}
}
#endregion
