#region Using declarations

using System;
using System.Drawing;
using NinjaTrader.Indicator;
using NinjaTrader.Strategy;
#endregion
namespace NinjaTrader.Custom.Strategy
{
    public class ZeroLagMac : BaseForexStrategy
    {
        private MACD_ZeroLag_Colors _macd;
        private RSI _rsi;

        protected override void MyInitialize()
        {
            _macd = MACD_ZeroLag_Colors(1, MACDFast, MACDSlow, MACDSmooth, 0);
             Add(_macd);

            _rsi = RSI(RSIPeriod, RSISmooth);
            Add(_rsi);
            TraceOrders = true;
        }

        protected override void MyManagePosition()
        {
             
        }

        protected override void SetupIndicatorProperties()
        {
            PropertiesExposed.Add("RSIPeriod");
            PropertiesExposed.Add("RSIUpper");
            PropertiesExposed.Add("RSILower");
            PropertiesExposed.Add("RSISmooth");

            PropertiesExposed.Add("MACDFast");
            PropertiesExposed.Add("MACDSlow");
            PropertiesExposed.Add("MACDSmooth");
            
        }

        protected override void MyOnBarUpdate()
        {


        }

        protected override void LookForEntry()
        {
            if (_macd.MacdUp[0] == 0 && Rising(_rsi) && Rising(_macd.Avg))
            {
                _tradeState = TradeState.InitialStop;
                BackColor = Color.LightGreen;
                EnterLong("long");
                if (_exitType == ExitType.TrailingATR)
                {
                    _lossLevel = Low[0] - EMA(ATR(MMAtrPeriod), MMAtrEMAPeriod)[0] * MMAtrMultiplier;
                    SetStopLoss("long", CalculationMode.Price, _lossLevel, true);
                }
                else if (_exitType == ExitType.InitialToBreakevenToTrailingATR)
                {
                    _lossLevel = Low[0] - TickSize * _mmInitialSL;
                    SetStopLoss("long", CalculationMode.Price, _lossLevel, true);
                }
                else
                {
                    _lossLevel = Low[0] - TickSize * _mmInitialSL;
                }
            }

            if (_macd.MacdDn[0] == 0 && Falling(_rsi) && Falling(_macd.Avg))
             {
                 _tradeState = TradeState.InitialStop;
                 BackColor = Color.Pink;
                 EnterShort("short");
                 if (_exitType == ExitType.TrailingATR)
                 {
                     _lossLevel = High[0] + EMA(ATR(MMAtrPeriod), MMAtrEMAPeriod)[0] * MMAtrMultiplier;
                     SetStopLoss("short", CalculationMode.Price, _lossLevel, true);
                 }
                 else if (_exitType == ExitType.InitialToBreakevenToTrailingATR)
                 {
                     _lossLevel = High[0] + TickSize * _mmInitialSL;
                     SetStopLoss("short", CalculationMode.Price, _lossLevel, true);
                 }
                 else
                 {
                     _lossLevel = High[0] + TickSize * _mmInitialSL;
                 }
             }


        }
        
        protected override void LookForExit()
        {


            if (IsLong)
            {
                //if (_macd.MacdNeutral[0] == 0)
                //{
                //    ExitLong("macd neutral", "long");
                //    BackColor = Color.LightYellow;
                //    return;
                //}
                if (CrossBelow(RSIUpper, _rsi, 1))
                {
                    ExitLong("rsi stop", "long");
                    BackColor = Color.LightYellow;
                    return;
                }

                if (CrossBelow(40, ADX(14), 1))
                {
                    ExitLong("exit bishop", "long");
                    return;
                }
            }


            if (IsShort)
            {
                //if (_macd.MacdNeutral[0] == 0)
                //{
                //    ExitShort("macd neutral", "short");
                //    BackColor = Color.LightYellow;
                //    return;
                //}

                if (CrossAbove(RSILower, _rsi, 1))
                {
                    ExitShort("rsi stop", "short");
                    BackColor = Color.LightYellow;
                    return;
                }

                if (CrossBelow(40, ADX(14), 1))
                {
                    ExitShort("exit bishop", "short");
                    return;
                }
            }
        }
    }
}
