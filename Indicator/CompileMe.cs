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
    public class CompileMe : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int myInput0 = 1; // Default setting for MyInput0
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Plot0"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            Plot0.Set(Close[0]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot0
        {
            get { return Values[0]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MyInput0
        {
            get { return myInput0; }
            set { myInput0 = Math.Max(1, value); }
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
        private CompileMe[] cacheCompileMe = null;

        private static CompileMe checkCompileMe = new CompileMe();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public CompileMe CompileMe(int myInput0)
        {
            return CompileMe(Input, myInput0);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public CompileMe CompileMe(Data.IDataSeries input, int myInput0)
        {
            if (cacheCompileMe != null)
                for (int idx = 0; idx < cacheCompileMe.Length; idx++)
                    if (cacheCompileMe[idx].MyInput0 == myInput0 && cacheCompileMe[idx].EqualsInput(input))
                        return cacheCompileMe[idx];

            lock (checkCompileMe)
            {
                checkCompileMe.MyInput0 = myInput0;
                myInput0 = checkCompileMe.MyInput0;

                if (cacheCompileMe != null)
                    for (int idx = 0; idx < cacheCompileMe.Length; idx++)
                        if (cacheCompileMe[idx].MyInput0 == myInput0 && cacheCompileMe[idx].EqualsInput(input))
                            return cacheCompileMe[idx];

                CompileMe indicator = new CompileMe();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.MyInput0 = myInput0;
                Indicators.Add(indicator);
                indicator.SetUp();

                CompileMe[] tmp = new CompileMe[cacheCompileMe == null ? 1 : cacheCompileMe.Length + 1];
                if (cacheCompileMe != null)
                    cacheCompileMe.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheCompileMe = tmp;
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
        public Indicator.CompileMe CompileMe(int myInput0)
        {
            return _indicator.CompileMe(Input, myInput0);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.CompileMe CompileMe(Data.IDataSeries input, int myInput0)
        {
            return _indicator.CompileMe(input, myInput0);
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
        public Indicator.CompileMe CompileMe(int myInput0)
        {
            return _indicator.CompileMe(Input, myInput0);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.CompileMe CompileMe(Data.IDataSeries input, int myInput0)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.CompileMe(input, myInput0);
        }
    }
}
#endregion
