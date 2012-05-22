#region Using declarations

using System;
using System.Drawing;
using NinjaTrader.Indicator;
using NinjaTrader.Strategy;
#endregion
namespace NinjaTrader.Custom.Strategy
{
    public class ZeroLagMacStrat : BaseForexStrategy
    {
        private ZeroLagMacIndi _indi;

        protected override void MyInitialize()
        {
            _indi = ZeroLagMacIndi(ADXMinimum, ADXPeriod, ATRPeriod, BreakevenTicks, InitialStoploss, MACDFast, MACDSlow,
                                   MACDSmooth, MMAtrEMAPeriod, MMAtrMultiplier, MMAtrPeriod, ProfitTicksBeforeBreakeven,
                                   RSILower, RSIPeriod, RSISmooth, RSIUpper);
            Add(_indi);
            TraceOrders = true;

         //   InitialStoploss = 300;
        }

        protected override void MyManagePosition()
        {

        }

        protected override void SetupIndicatorProperties()
        {
            PropertiesExposed.Add("ADXMinimum");
            PropertiesExposed.Add("ADXPeriod");
            PropertiesExposed.Add("ATRPeriod");
 
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
              P(_indi.StopLoss[0] + "");

        }

        protected override void LookForEntry()
        {

            if (_indi.Signal == 1)
            {
                SetStopLoss(CalculationMode.Price, _indi.StopLoss[0]);
                EnterLong("long");
            }
            if (_indi.Signal == -1)
            {
                SetStopLoss(CalculationMode.Price, _indi.StopLoss[0]);
                EnterShort("short");
            }
        }

        protected override void LookForExit()
        {
            if (IsLong && _indi.Signal == 0)
            {
                ExitLong("long");
            }

            if (IsShort && _indi.Signal == 0)
            {
                ExitShort("short");
            }
        }
    }
}
