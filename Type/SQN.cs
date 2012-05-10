/* 
 Portions copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.


 Optimizer code copyright (c) 2007, OV Trading, all rights reserved.

 $Header: C:/Documents\040and\040Settings/Peter/My\040Documents/NinjaTrader\0406.5/bin/Custom/Type/RCS/SQN.cs 1.3 2008/03/03 00:01:04 Peter Exp $
 $Log: SQN.cs $
 Revision 1.3  2008/03/03 00:01:04  Peter
 Changes to stay in sync with v1.4 of genetic optimizer.

 Revision 1.2  2008/02/12 03:54:03  Peter
 Updated for GA compatibility.

*/

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
	/// System Quality Number
	/// </summary>
	[Gui.Design.DisplayName("my system quality number")]
	public class SQN : OptimizationType
	{
		public static double sLastSQN;
		public static int sMinTrades = 0;
		
		/// <summary>
		/// Return the performance value of a backtesting result.
		/// </summary>
		/// <param name="systemPerformance"></param>
		/// <returns></returns>
		public override double GetPerformanceValue(SystemPerformance systemPerformance)
		{
			// Allow override of minimum number of trades to return a value
			// Parameter is SQNMinTrades
			int minTrades = 30;
			if (sMinTrades > 0)
			{
				minTrades = sMinTrades;
			}
			else
			{
				int n;
				for (n = 0; n < Strategy.Parameters.Count; n++)
				{
					if ("SQNMinTrades".CompareTo(Strategy.Parameters[n].Name) == 0)
					{
						minTrades = (int)Strategy.Parameters[n].Value;
						break;
					}
				}
			}
		
			double numTrades = systemPerformance.AllTrades.Count;
			
			if (numTrades < minTrades)
				return 0;
			
			// This calc comes from NT standard net profit opt type
			double avgProfit = (systemPerformance.AllTrades.TradesPerformance.GrossProfit +
				systemPerformance.AllTrades.TradesPerformance.GrossLoss) / numTrades;

			double stddev = 0;
			double tradeProf;
			
			// Now figure std dev of profit
			// Note: I forget my statistics & pulled this algorithm from the internet,
			// corrections welcomed.
			foreach (Trade t in systemPerformance.AllTrades)
			{
				tradeProf = (t.ProfitPoints * t.Quantity * t.Entry.Instrument.MasterInstrument.PointValue);
				
				// Uncomment this section for debug output to NT's output window
				/*
				Strategy.Print(t.Entry.Time + "," + t.Quantity + "," + t.Entry.Price + "," + t.Exit.Price + "," +
					t.ProfitPoints.ToString("N2") + "," + t.Quantity + "," +
					t.Entry.Instrument.MasterInstrument.PointValue + "," + tradeProf);
				*/
				
				stddev += Math.Pow(tradeProf - avgProfit, 2);
			}
			
			stddev /= numTrades;
			stddev = Math.Sqrt(stddev);
			
			double sqn = (Math.Sqrt(numTrades) * avgProfit) / stddev;

			// Uncomment this section for debug output to NT's output window
			/*
			Strategy.Print("numTrades: " + numTrades.ToString("N2") + "  avgProfit: " + avgProfit.ToString("N2") +
				"  stddev: " + stddev.ToString("N2") + "  sqn: " + sqn.ToString("N2"));
			*/
			
			// Hoping to access this from my optimizer (note: it works.)
			sLastSQN = sqn;
			return sqn;
		}
	}

}
