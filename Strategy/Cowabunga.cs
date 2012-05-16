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

namespace NinjaTrader.Custom.Strategy
{
//    4hr. Chart Settings:

//    5 EMA applied to the close
//    10 EMA applied to the close
//    Stochastic (10,3,3) (Use slow and simple settings)
//    RSI (9) (Simple)

//The 15 Minute Chart

//After establishing the main trend , it's time to look for trade entries on the 15 minute chart. The 15 minute chart looks similar to the 4hr. chart, except for the fact that I have added a MACD histogram. The trade entry rules are simple:

//15 Minute Chart Settings:

//    5 EMA applied to the close
//    10 EMA applied to the close
//    Stochastic (10,3,3) (Use slow and simple settings)
//    RSI (9) (Simple)
//    MACD (12,26,9) (Exponential histogram)- Make sure the histogram displays the difference between the 2 lines




    public class Cowabunga: BaseForexStrategy
    {
        protected enum TrendDirection
        {
            Neutral,
            Long,
            Short
        }

        protected override void MyInitialize()
        {
             Add(PeriodType.Minute, 240);
        }


        protected override void SetupIndicatorProperties()
        {
            PropertiesExposed.Add("EMAFastPeriod");
            PropertiesExposed.Add("EMASlowPeriod");

            PropertiesExposed.Add("StochPeriodD");
            PropertiesExposed.Add("StochPeriodK");
            PropertiesExposed.Add("StochSmooth");
            PropertiesExposed.Add("StochUpper");
            PropertiesExposed.Add("StochLower");
            
            PropertiesExposed.Add("RSIPeriod");
            PropertiesExposed.Add("RSIUpper");
            PropertiesExposed.Add("RSISmooth");
            
            PropertiesExposed.Add("MACDFast");
            PropertiesExposed.Add("MACDSlow");
            PropertiesExposed.Add("MACDSmooth");

            PropertiesExposed.Add("CrossoverLookbackPeriod");
            
        }

        protected override void MyOnBarUpdate()
        {
          
           

        }

        protected override void LookForEntry()
        {
            // TrendDirection trendFourHour =  Get4HDirection();

            if ((Time[0].DayOfWeek == DayOfWeek.Sunday) || (Time[0].DayOfWeek == DayOfWeek.Friday))
            {
                if (Get4HDirection() == TrendDirection.Long && Get15MinuteDirection() == TrendDirection.Long)
                {
                    BackColor = Color.LightGreen;
                    Print("Going long");
                    //    SetStopLoss("long", CalculationMode.Price, Swing(5).SwingLow[0], false);
                    EnterLongLimit(Close[0], "long");

                    //    SetProfitTarget(CalculationMode.Price,  Close[0] + (Close[0] - Swing(5).SwingLow[0])) ;
                }

                if (Get4HDirection() == TrendDirection.Short && Get15MinuteDirection() == TrendDirection.Short)
                {
                    BackColor = Color.Pink;
                    Print("Going short");
                    //      SetStopLoss("short", CalculationMode.Price, Swing(5).SwingHigh[0], false);
                    EnterShortLimit(Close[0], "short");
                    //     SetProfitTarget(CalculationMode.Price, Close[0] - (1.5*( Swing(5).SwingHigh[0] - Close[0])));
                }
            }
        }


        private TrendDirection Get15MinuteDirection()
        {
            EMA fastEMA = EMA(EMAFastPeriod);
            EMA slowEMA = EMA(EMASlowPeriod);
            Stochastics stoch = Stochastics(StochPeriodD, StochPeriodK, StochSmooth);
            RSI rsi = RSI(RSIPeriod, RSISmooth);
            MACD macd = MACD(MACDFast, MACDSlow, MACDSmooth);

            // long
            if (CrossAbove(slowEMA, fastEMA[0],  CrossoverLookbackPeriod)
                && rsi[0] > RSIUpper
                && Rising(stoch) 
                && stoch[0] < StochUpper
                && (CrossAbove(0,macd.Diff, CrossoverLookbackPeriod) || (macd.Diff[0] < 0 && Rising(macd.Diff)) )  )  // macd.diff should be just starting to rise
            {
                return TrendDirection.Long;
            }

            // short 
            if (CrossBelow(slowEMA, fastEMA[0], CrossoverLookbackPeriod)
                && rsi[0] < RSIUpper
                && Falling(stoch)
                && stoch[0] > StochLower
                && (CrossBelow(0, macd.Diff, CrossoverLookbackPeriod) || (macd.Diff[0] > 0   && Falling(macd.Diff))))  // macd.diff should be just starting to fall
            {
                return TrendDirection.Short;
            }

            return TrendDirection.Neutral;
        }

        private TrendDirection Get4HDirection()
        {
            // check the 4h chart

            EMA fastEMA = EMA(BarsArray[1], EMAFastPeriod);
            EMA slowEMA = EMA(BarsArray[1], EMASlowPeriod);
            Stochastics stoch = Stochastics(BarsArray[1], StochPeriodD, StochPeriodK, StochSmooth);
            RSI rsi = RSI(BarsArray[1], RSIPeriod, RSISmooth);

            // long
            if (fastEMA[0] > slowEMA[0]  && Rising(fastEMA) && rsi[0] > RSIUpper && Rising(stoch) && stoch[0] < StochUpper)
            {
           //      BackColor = Color.LightGreen;
               return TrendDirection.Long;
            }

            // short 
            if (fastEMA[0] < slowEMA[0] && Falling(fastEMA) && rsi[0] < RSIUpper && Falling(stoch) && stoch[0] > StochLower)
            {
             //     BackColor = Color.Pink;
              return TrendDirection.Short;
            }

            return TrendDirection.Neutral;
        }



        protected override void MyManagePosition()
        {
             
        }
    }
}
