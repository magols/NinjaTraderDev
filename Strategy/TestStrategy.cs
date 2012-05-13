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
    public sealed class TestStrategy : BaseForexStrategy
    {
        private AmazingCrossoverIndi _indi;

        public TestStrategy()
        {
            SetupIndicatorProperties();
            ExitStrategy = ExitType.IntialToBreakevenToTrailing;
        }

        protected override void SetupIndicatorProperties()
        {
            PropertiesExposed.Add("EMASlowPeriod");
            PropertiesExposed.Add("EMAFastPeriod");
            PropertiesExposed.Add("RSIPeriod");
            PropertiesExposed.Add("RSILower");
            PropertiesExposed.Add("RSIUpper");
            PropertiesExposed.Add("ADXPeriod");
            PropertiesExposed.Add("ADXMinimum");
            PropertiesExposed.Add("CrossoverLookbackPeriod");
        }

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void MyInitialize()
        {

            if (_indi == null)
            {
                _indi = AmazingCrossoverIndi(_adxMin, _adxPeriod, _atrExclusionMultiplier, _atrPeriod,
                             _crossoverLookbackPeriod, _emaFastPeriod, _emaSlowPeriod, _rsiLower, _rsiPeriod,
                             _rsiUpper);
                _indi.SetupObjects();
                Add(_indi);
            }
            CalculateOnBarClose = true;
        }




        protected  override void MyOnBarUpdate()
        {
            var a = _indi[0];
        }

        protected override void LookForTrade()
        {
            if (_indi.Signal == 0) return;

            double risk = TickSize * _mmInitialSL;
            if (_indi.Signal == 1)
            {
                _lossLevel = Close[0] - risk;
                _entry = EnterLong(ComputeQty(risk), GetType().Name + " long" );
            }
            else if (_indi.Signal == -1)
            {
                _lossLevel = Close[0] + risk;
                _entry = EnterShort(ComputeQty(risk), GetType().Name + " short");
            }
            _tradeState = TradeState.InitialStop;
        }

        protected override void MyManagePosition()
        {
            throw new NotImplementedException();
        }
    }
}
