#region Using declarations
using System;
using System.Collections;
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
    public sealed class TestRandomWithATRTrail : BaseForexStrategy
    {
        private Random _rand = null;
     
        private const int GO_LONG = 1;
        private const int GO_SHORT = 0;

        public TestRandomWithATRTrail()
        {
            SetupIndicatorProperties();
            ExitStrategy = ExitType.IntialToBreakevenToTrailing;
        }

        protected override void SetupIndicatorProperties()
        {
            //IndicatorPropertiesUsed.Add("InitialStoploss");
            //IndicatorPropertiesUsed.Add("BreakevenTicks");
            //IndicatorPropertiesUsed.Add("ProfitTicksBeforeBreakeven");  
            //IndicatorPropertiesUsed.Add("TrailTicks");  

        }

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void MyInitialize()
        {

            _rand = new Random(DateTime.Now.Millisecond);
      
            CalculateOnBarClose = true;
        }




        protected override void MyOnBarUpdate()
        {

        }

        protected override void LookForTrade()
        {
                EnterTrade();
        }

        private void EnterTrade()
        {
            int toss = _rand.Next(2);

            if (toss == GO_LONG)
            {
     //           GoFlat();
         //       _lossLevel = Close[0] - (TickSize* _mmInitialSL);
         //       SetStopLoss(CalculationMode.Price, _lossLevel);
                _entry = EnterLong(DefaultQuantity);
            }
            else if (toss == GO_SHORT)
            {
   //             GoFlat();
           //     _lossLevel = Close[0] + (TickSize * _mmInitialSL);
        //        SetStopLoss(CalculationMode.Price, _lossLevel);
                _entry = EnterShort(DefaultQuantity);
            }

            // immediately start manage the trade by setting initial EMAofATR stop
            ManageTradeByTrailingATR();
        }

        protected override void MyManagePosition()
        {
            throw new NotImplementedException();
        }
    }
}
