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
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("Enter the description of your new custom indicator here")]
    public class AmazingCrossoverIndi : Indicator
    {
        #region Variables

        private int eMASlowPeriod = 10; // Default setting for EMASlowPeriod
        private int eMAFastPeriod = 5; // Default setting for EMAFastPeriod
        private int rSIPeriod = 10; // Default setting for RSIPeriod
        private int aDXPeriod = 10; // Default setting for ADXPeriod
        private int _atrPeriod = 10;
        private double _atrExclusionMultiplier = 1;

        private int _adxMin = 20;
        private int _rsiLower = 45;
        private int _rsiUpper = 55;

        private int _crossoverLookbackPeriod = 1; 

        private EMA _emaSlow, _emaFast;
        private RSI _rsi;
        private ADX _adx;
        private ATR _atr;

        private int _signal;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "EMASlowPlot"));
            Add(new Plot(Color.FromKnownColor(KnownColor.OrangeRed), PlotStyle.Line, "EMAFastPlot"));
            Overlay = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {

            SetupObjects();

           _signal =  GetSignalState();
            

            EMAFastPlot.Set(_emaFast[0]);
            EMASlowPlot.Set(_emaSlow[0]);
        }

        private int GetSignalState()
        {
            BackColor = Color.White;
            int retSignal = 0;
           
            // is ADX ok
            if (_adx[0] <= _adxMin) return 0;  
            
            // is previous bar height smaller than ATR exlusion rule?
            //if (VerticalGridLines)
            //{
                
            //}


            // long
            if (CrossAbove(_rsi, _rsiUpper, _crossoverLookbackPeriod) &&
                CrossAbove(_emaFast, _emaSlow, _crossoverLookbackPeriod))
            {
                BackColor = Color.LightGreen;
                return 1;
            }

            // short
            if (CrossBelow(_rsi, _rsiLower, _crossoverLookbackPeriod) &&
                 CrossBelow(_emaFast, _emaSlow, _crossoverLookbackPeriod))
            {
                 BackColor = Color.Pink;
                return -1;
            }

            
            return retSignal;
        }


        private void SetupObjects()
        {
            if (_emaSlow == null)
                _emaSlow = EMA(eMASlowPeriod);
            if (_emaFast == null)
                _emaFast = EMA(eMAFastPeriod);
            if (_rsi == null)
                _rsi = RSI(Median, rSIPeriod, 1);
            if (_adx == null)
                _adx = ADX(aDXPeriod);
            if (_atr == null)
                _atr = ATR(_atrPeriod);
        }

        #region plots
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries EMASlowPlot
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries EMAFastPlot
        {
            get { return Values[1]; }
        }

        #endregion

        #region dataobj
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public RSI RSIseries
        {
            get { return _rsi; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public ADX ADXseries
        {
            get { return _adx; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public ATR ATRseries
        {
            get { return _atr; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public int Signal
        {
            get { return _signal; }
        }

        #endregion

        #region Properties
        [Description("Period for the slow EMA")]
        [GridCategory("Parameters")]
        public int EMASlowPeriod
        {
            get { return eMASlowPeriod; }
            set { eMASlowPeriod = Math.Max(1, value); }
        }

        [Description("Period for the fast EMA")]
        [GridCategory("Parameters")]
        public int EMAFastPeriod
        {
            get { return eMAFastPeriod; }
            set { eMAFastPeriod = Math.Max(1, value); }
        }

        [Description("Period for RSI, applied to median")]
        [GridCategory("Parameters")]
        public int RSIPeriod
        {
            get { return rSIPeriod; }
            set { rSIPeriod = Math.Max(1, value); }
        }




        [Description("Period for RSI lower")]
        [GridCategory("Parameters")]
        public int RSILower
        {
            get { return _rsiLower; }
            set { _rsiLower = Math.Max(1, value); }
        }
        [Description("Period for RSI upper")]
        [GridCategory("Parameters")]
        public int RSIUpper
        {
            get { return _rsiUpper; }
            set { _rsiUpper = Math.Max(1, value); }
        }






        [Description("Period for ADX")]
        [GridCategory("Parameters")]
        public int ADXPeriod
        {
            get { return aDXPeriod; }
            set { aDXPeriod = Math.Max(1, value); }
        }
        [Description("Minimum for ADX")]
        [GridCategory("Parameters")]
        public int ADXMinimum
        {
            get { return _adxMin; }
            set { _adxMin = Math.Max(1, value); }
        }

        [Description("Period for ATR")]
        [GridCategory("Parameters")]
        public int ATRPeriod
        {
            get { return _atrPeriod; }
            set { _atrPeriod = Math.Max(1, value); }
        }

        [Description("ATR multiplier for exluding trades")]
        [GridCategory("Parameters")]
        public double ATRExclusionMultiplier
        {
            get { return _atrExclusionMultiplier; }
            set { _atrExclusionMultiplier = Math.Max(1, value); }
        }


        [Description("Lookback period for crossover convergence")]
        [GridCategory("Parameters")]
        public int CrossoverLookbackPeriod
        {
            get { return _crossoverLookbackPeriod; }
            set { _crossoverLookbackPeriod = Math.Max(1, value); }
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
        private AmazingCrossoverIndi[] cacheAmazingCrossoverIndi = null;

        private static AmazingCrossoverIndi checkAmazingCrossoverIndi = new AmazingCrossoverIndi();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public AmazingCrossoverIndi AmazingCrossoverIndi(int aDXMinimum, int aDXPeriod, double aTRExclusionMultiplier, int aTRPeriod, int crossoverLookbackPeriod, int eMAFastPeriod, int eMASlowPeriod, int rSILower, int rSIPeriod, int rSIUpper)
        {
            return AmazingCrossoverIndi(Input, aDXMinimum, aDXPeriod, aTRExclusionMultiplier, aTRPeriod, crossoverLookbackPeriod, eMAFastPeriod, eMASlowPeriod, rSILower, rSIPeriod, rSIUpper);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public AmazingCrossoverIndi AmazingCrossoverIndi(Data.IDataSeries input, int aDXMinimum, int aDXPeriod, double aTRExclusionMultiplier, int aTRPeriod, int crossoverLookbackPeriod, int eMAFastPeriod, int eMASlowPeriod, int rSILower, int rSIPeriod, int rSIUpper)
        {
            if (cacheAmazingCrossoverIndi != null)
                for (int idx = 0; idx < cacheAmazingCrossoverIndi.Length; idx++)
                    if (cacheAmazingCrossoverIndi[idx].ADXMinimum == aDXMinimum && cacheAmazingCrossoverIndi[idx].ADXPeriod == aDXPeriod && Math.Abs(cacheAmazingCrossoverIndi[idx].ATRExclusionMultiplier - aTRExclusionMultiplier) <= double.Epsilon && cacheAmazingCrossoverIndi[idx].ATRPeriod == aTRPeriod && cacheAmazingCrossoverIndi[idx].CrossoverLookbackPeriod == crossoverLookbackPeriod && cacheAmazingCrossoverIndi[idx].EMAFastPeriod == eMAFastPeriod && cacheAmazingCrossoverIndi[idx].EMASlowPeriod == eMASlowPeriod && cacheAmazingCrossoverIndi[idx].RSILower == rSILower && cacheAmazingCrossoverIndi[idx].RSIPeriod == rSIPeriod && cacheAmazingCrossoverIndi[idx].RSIUpper == rSIUpper && cacheAmazingCrossoverIndi[idx].EqualsInput(input))
                        return cacheAmazingCrossoverIndi[idx];

            lock (checkAmazingCrossoverIndi)
            {
                checkAmazingCrossoverIndi.ADXMinimum = aDXMinimum;
                aDXMinimum = checkAmazingCrossoverIndi.ADXMinimum;
                checkAmazingCrossoverIndi.ADXPeriod = aDXPeriod;
                aDXPeriod = checkAmazingCrossoverIndi.ADXPeriod;
                checkAmazingCrossoverIndi.ATRExclusionMultiplier = aTRExclusionMultiplier;
                aTRExclusionMultiplier = checkAmazingCrossoverIndi.ATRExclusionMultiplier;
                checkAmazingCrossoverIndi.ATRPeriod = aTRPeriod;
                aTRPeriod = checkAmazingCrossoverIndi.ATRPeriod;
                checkAmazingCrossoverIndi.CrossoverLookbackPeriod = crossoverLookbackPeriod;
                crossoverLookbackPeriod = checkAmazingCrossoverIndi.CrossoverLookbackPeriod;
                checkAmazingCrossoverIndi.EMAFastPeriod = eMAFastPeriod;
                eMAFastPeriod = checkAmazingCrossoverIndi.EMAFastPeriod;
                checkAmazingCrossoverIndi.EMASlowPeriod = eMASlowPeriod;
                eMASlowPeriod = checkAmazingCrossoverIndi.EMASlowPeriod;
                checkAmazingCrossoverIndi.RSILower = rSILower;
                rSILower = checkAmazingCrossoverIndi.RSILower;
                checkAmazingCrossoverIndi.RSIPeriod = rSIPeriod;
                rSIPeriod = checkAmazingCrossoverIndi.RSIPeriod;
                checkAmazingCrossoverIndi.RSIUpper = rSIUpper;
                rSIUpper = checkAmazingCrossoverIndi.RSIUpper;

                if (cacheAmazingCrossoverIndi != null)
                    for (int idx = 0; idx < cacheAmazingCrossoverIndi.Length; idx++)
                        if (cacheAmazingCrossoverIndi[idx].ADXMinimum == aDXMinimum && cacheAmazingCrossoverIndi[idx].ADXPeriod == aDXPeriod && Math.Abs(cacheAmazingCrossoverIndi[idx].ATRExclusionMultiplier - aTRExclusionMultiplier) <= double.Epsilon && cacheAmazingCrossoverIndi[idx].ATRPeriod == aTRPeriod && cacheAmazingCrossoverIndi[idx].CrossoverLookbackPeriod == crossoverLookbackPeriod && cacheAmazingCrossoverIndi[idx].EMAFastPeriod == eMAFastPeriod && cacheAmazingCrossoverIndi[idx].EMASlowPeriod == eMASlowPeriod && cacheAmazingCrossoverIndi[idx].RSILower == rSILower && cacheAmazingCrossoverIndi[idx].RSIPeriod == rSIPeriod && cacheAmazingCrossoverIndi[idx].RSIUpper == rSIUpper && cacheAmazingCrossoverIndi[idx].EqualsInput(input))
                            return cacheAmazingCrossoverIndi[idx];

                AmazingCrossoverIndi indicator = new AmazingCrossoverIndi();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ADXMinimum = aDXMinimum;
                indicator.ADXPeriod = aDXPeriod;
                indicator.ATRExclusionMultiplier = aTRExclusionMultiplier;
                indicator.ATRPeriod = aTRPeriod;
                indicator.CrossoverLookbackPeriod = crossoverLookbackPeriod;
                indicator.EMAFastPeriod = eMAFastPeriod;
                indicator.EMASlowPeriod = eMASlowPeriod;
                indicator.RSILower = rSILower;
                indicator.RSIPeriod = rSIPeriod;
                indicator.RSIUpper = rSIUpper;
                Indicators.Add(indicator);
                indicator.SetUp();

                AmazingCrossoverIndi[] tmp = new AmazingCrossoverIndi[cacheAmazingCrossoverIndi == null ? 1 : cacheAmazingCrossoverIndi.Length + 1];
                if (cacheAmazingCrossoverIndi != null)
                    cacheAmazingCrossoverIndi.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheAmazingCrossoverIndi = tmp;
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
        public Indicator.AmazingCrossoverIndi AmazingCrossoverIndi(int aDXMinimum, int aDXPeriod, double aTRExclusionMultiplier, int aTRPeriod, int crossoverLookbackPeriod, int eMAFastPeriod, int eMASlowPeriod, int rSILower, int rSIPeriod, int rSIUpper)
        {
            return _indicator.AmazingCrossoverIndi(Input, aDXMinimum, aDXPeriod, aTRExclusionMultiplier, aTRPeriod, crossoverLookbackPeriod, eMAFastPeriod, eMASlowPeriod, rSILower, rSIPeriod, rSIUpper);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.AmazingCrossoverIndi AmazingCrossoverIndi(Data.IDataSeries input, int aDXMinimum, int aDXPeriod, double aTRExclusionMultiplier, int aTRPeriod, int crossoverLookbackPeriod, int eMAFastPeriod, int eMASlowPeriod, int rSILower, int rSIPeriod, int rSIUpper)
        {
            return _indicator.AmazingCrossoverIndi(input, aDXMinimum, aDXPeriod, aTRExclusionMultiplier, aTRPeriod, crossoverLookbackPeriod, eMAFastPeriod, eMASlowPeriod, rSILower, rSIPeriod, rSIUpper);
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
        public Indicator.AmazingCrossoverIndi AmazingCrossoverIndi(int aDXMinimum, int aDXPeriod, double aTRExclusionMultiplier, int aTRPeriod, int crossoverLookbackPeriod, int eMAFastPeriod, int eMASlowPeriod, int rSILower, int rSIPeriod, int rSIUpper)
        {
            return _indicator.AmazingCrossoverIndi(Input, aDXMinimum, aDXPeriod, aTRExclusionMultiplier, aTRPeriod, crossoverLookbackPeriod, eMAFastPeriod, eMASlowPeriod, rSILower, rSIPeriod, rSIUpper);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.AmazingCrossoverIndi AmazingCrossoverIndi(Data.IDataSeries input, int aDXMinimum, int aDXPeriod, double aTRExclusionMultiplier, int aTRPeriod, int crossoverLookbackPeriod, int eMAFastPeriod, int eMASlowPeriod, int rSILower, int rSIPeriod, int rSIUpper)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.AmazingCrossoverIndi(input, aDXMinimum, aDXPeriod, aTRExclusionMultiplier, aTRPeriod, crossoverLookbackPeriod, eMAFastPeriod, eMASlowPeriod, rSILower, rSIPeriod, rSIUpper);
        }
    }
}
#endregion
