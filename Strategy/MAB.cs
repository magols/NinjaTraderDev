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
    public class MAB: BaseForexStrategy
    {
        private Bollinger _boll;
        private SMA _smaFast, _smaSlow;

        protected override void SetupIndicatorProperties()
        {
            PropertiesExposed.Add("SMAFastPeriod");
            PropertiesExposed.Add("SMASlowPeriod");

            PropertiesExposed.Add("BollingerPeriod");
            PropertiesExposed.Add("BollingerStdDev");
        }

        protected override void MyInitialize()
        {
            _boll = Bollinger(BollingerStdDev, BollingerPeriod);
            Add(_boll);

            _smaFast = SMA(SMAFastPeriod);
            _smaFast.Plots[0].Pen.Color = Color.Blue;
            Add(_smaFast);

            _smaSlow = SMA(SMASlowPeriod);
            _smaSlow.Plots[0].Pen.Color = Color.Purple;
            Add(_smaSlow);

            InitialStoploss = 100;
            ProfitTicks = 300;
            SetProfitTarget(CalculationMode.Ticks, 300);
            //  SetStopLoss(CalculationMode.Ticks, 200);
        }

        protected override void MyOnBarUpdate()
        {
            if (Close[0] > _boll.Upper[0])
            {
                BackColor = Color.Pink;
            }
            if (Close[0] < _boll.Lower[0])
            {
                BackColor = Color.LightGreen;
            }

        }

        protected override void LookForEntry()
        {

            if (Close[0] < _boll.Lower[0] && Close[0] < _smaSlow[0])
            {
                if (_exitType == ExitType.TrailingATR)
                {
                    _tradeState = TradeState.InitialStop;
                    _lossLevel = Close[0] - EMA(ATR(MMAtrPeriod), MMAtrEMAPeriod)[0] * MMAtrMultiplier;
                    SetStopLoss("long", CalculationMode.Price, _lossLevel, true);
                }
                else
                {
                    _lossLevel = Close[0] - TickSize * _mmInitialSL;
                }
                EnterLong("long");
            }

            if (Close[0] > _boll.Upper[0] && Close[0] > _smaSlow[0])
            {
                if (_exitType == ExitType.TrailingATR)
                {
                    _tradeState = TradeState.InitialStop;
                    _lossLevel = Close[0] + EMA(ATR(MMAtrPeriod), MMAtrEMAPeriod)[0] * MMAtrMultiplier;
                    SetStopLoss("short", CalculationMode.Price, _lossLevel, true);
                }
                else
                {
                    _lossLevel = Close[0] + TickSize * _mmInitialSL;
                }
                EnterShort("short");
            }

        }

        protected override void LookForExit()
        {
            throw new NotImplementedException();
        }


        protected override void MyManagePosition()
        {
            if (IsLong)
            {
                if (Close[0] > _boll.Middle[0])
                {
                    ExitLong("median from long","long");
                }
            }

            if (IsShort)
            {
                if (Close[0] < _boll.Middle[0])
                {
                    ExitShort("median from short", "short");
                } 
            }
        }
    }
}
