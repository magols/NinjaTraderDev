#region Using declarations
using System;
using System.Collections.Generic;
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

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Enter the description of your strategy here
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class AsianAssault : BaseForexStrategy
    {
        #region Variables

 
        private int _maxTicksToTarget = 10;

        private PriorDayOHLC _indiPrior;
        private SMA _sma;
        private IOrder _latestSubmittedOrder;
        #endregion

        protected override void MyInitialize()
        {
          //  TraceOrders = true;

            _exitType = ExitType.TrailingATR;

            _indiPrior = PriorDayOHLC();
            _indiPrior.ShowClose = false;
            _indiPrior.ShowOpen = false;
            Add(_indiPrior);
            
            Add(EMAofATR(MMAtrMultiplier, MMAtrEMAPeriod));

            Add(PeriodType.Day, 1);
           

            //      SetTrailStop(CalculationMode.Ticks, 15);
            //SetStopLoss(CalculationMode.Ticks, 20);
            //SetProfitTarget(CalculationMode.Ticks, 30);

        }

        protected override void SetupIndicatorProperties()
        {

            PropertiesExposed.Add("TradeableTimeStartHour");
            PropertiesExposed.Add("TradeableTimeEndHour");
            PropertiesExposed.Add("SMAFastPeriod");
       
          
        }

 
        protected override void MyOnBarUpdate()
        {
    
            if (_latestSubmittedOrder != null)
            {
                if (Time[0].Hour > _tradeableTimeEndHour)
                {
                    CancelOrder(_latestSubmittedOrder);
                    _latestSubmittedOrder = null;
                }
            }
             
        }


        protected override void OnOrderUpdate(IOrder order)
        {
            if (_latestSubmittedOrder != null && _latestSubmittedOrder == order)
            {
                Print(order.ToString());
                if (order.OrderState == OrderState.Filled)
                    _latestSubmittedOrder = null;
            }
        }


        private List<string> _orderDates = new List<string>();
       

        protected override void LookForEntry()
        {
            _sma = SMA(BarsArray[1], SMAFastPeriod);

            if (!EntryOk()) return;
            if (_latestSubmittedOrder != null) return;

            double priorHigh = _indiPrior.PriorHigh[0];
            double priorLow = _indiPrior.PriorLow[0];

            if (priorLow == 0 || priorHigh == 0) return;


            // go long?
            if (Close[0] > priorLow && Close[0] <= priorLow + (TickSize * _maxTicksToTarget) && Falling(Close) && Rising(_sma))
            {
                 _latestSubmittedOrder = EnterLongLimit(0, true, DefaultQuantity, priorLow, "long");
                _tradeState = TradeState.InitialStop;

                if (_exitType == ExitType.TrailingATR)
                {
                    _lossLevel = priorLow - EMA(ATR(MMAtrPeriod), MMAtrEMAPeriod)[0] * MMAtrMultiplier;
                    SetStopLoss("long", CalculationMode.Price, _lossLevel, true);
                }
                else
                {
                    _lossLevel = priorLow - TickSize * _mmInitialSL;
                }
                _orderDates.Add(Time[0].ToShortDateString());
                
            }

            // go short?
            if (Close[0] < priorHigh && Close[0] >= priorHigh - (TickSize * _maxTicksToTarget) && Rising(Close) && Falling(_sma))
            {
                _latestSubmittedOrder = EnterShortLimit(0, true, DefaultQuantity, priorHigh, "short");
                _tradeState = TradeState.InitialStop;

                if (_exitType == ExitType.TrailingATR)
                {
                    _lossLevel = priorHigh + EMA(ATR(MMAtrPeriod), MMAtrEMAPeriod)[0] * MMAtrMultiplier;
                    SetStopLoss("short", CalculationMode.Price, _lossLevel, true);
                }
                else
                {
                     _lossLevel = priorHigh + TickSize*_mmInitialSL;
                }
               
                _orderDates.Add(Time[0].ToShortDateString());
            
            }




        }



        protected override void MyManagePosition()
        {

        }

        private bool EntryOk()
        {
            if (Time[0].Day == 25  && Time[0].Hour == 1 && Time[0].Minute == 30)
            {
                var s = "";
            }

            if ((Time[0].Hour < _tradeableTimeStartHour) )
            {
                return false;
            }

            if ((Time[0].Hour >= _tradeableTimeEndHour))
            {
                return false;
            }
            if (_orderDates.Contains(Time[0].ToShortDateString()))
            {
                return false;

            }


            //if (_orderDates.Count >0)
            //{
            //    return false;

            //}



            return true;
        }

        #region parameters

        #endregion
    }
}


