// TradingStudies.com
// info@tradingStudies.com
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TSEcoUtility;

namespace TSEcoUtility
{
    public enum DockingPlace
    {
        Below = 0,
        Above = 1
    }
}

namespace NinjaTrader.Indicator
{
    [Gui.Design.DisplayName(" TS Economic News")]
    [Description(" TS Economic News Add On")]
    public class TSEcoNews : Indicator
    {
        private DockingPlace _dp = DockingPlace.Below;
        private EcoNewsControl.EcoNewsControl _so;
        private Splitter _sp;
        private bool _x;
        ToolStrip _mystrip;
        ToolStripSeparator _myitem0;
        ToolStripButton _myitem1;

        [Description("Docking")] 
        [Category("Parameters")]
        [Gui.Design.DisplayName("Docking")] 
            public DockingPlace Dp
        {
            get { return _dp; }
            set { _dp = value; }
        }
        protected override void Initialize()
        {
            Overlay = true;
            PriceTypeSupported = false;
        }

        protected override void OnBarUpdate()
        {
            if (ChartControl == null || _x )
                return;
            if (!ChartControl.Controls.ContainsKey("TSEco_News"))
            {
                _myitem0 = new ToolStripSeparator();
                _myitem0.Name = "TradingStudiesEcoSeparator";

                _myitem1 = new ToolStripButton("Hide News");
                _myitem1.Text = "Hide News";
                _myitem1.Name = "TradingStudiesEcoNews";
                _myitem1.Click += ToolClick1;
                _myitem1.Enabled = true;
                _myitem1.ForeColor = Color.Black;
                _mystrip = (ToolStrip) ChartControl.Controls["tsrTool"];
                _mystrip.Items.Add(_myitem0);
                _mystrip.Items.Add(_myitem1);

                _sp = new Splitter();
                _sp.Name = "TSEco_Splitter";
                _sp.Dock = _dp == DockingPlace.Below ? DockStyle.Bottom : DockStyle.Top;
                ChartControl.Controls.Add(_sp);

                _so = new EcoNewsControl.EcoNewsControl(Cbi.Core.InstallDir + @"\Sounds", Cbi.Core.UserDataDir + @"bin\Custom\");
                _so.Dock = _dp == DockingPlace.Below ? DockStyle.Bottom : DockStyle.Top;
                _so.Name = "TSEco_News";
                ChartControl.Controls.Add(_so);
            }
            else
                _so = ChartControl.Controls["TSEco_News"] as EcoNewsControl.EcoNewsControl;
            _x = true;
        }

        private void ToolClick1(object sender, EventArgs e)
        {
            if(_so.Visible)
            {
                _so.Hide();
                _myitem1.Text = "Show News";
            }
            else
            {
                _so.Show();
                _myitem1.Text = "Hide News";
            }
        }

        public override void Dispose()
        {
            if (ChartControl != null && _so != null)
            {
                ChartControl.Controls.Remove(_so);
                ChartControl.Controls.Remove(_sp);
                _mystrip.Items.RemoveByKey("TradingStudiesEcoSeparator");
                _mystrip.Items.RemoveByKey("TradingStudiesEcoNews");
            }
            _so = null;
            _sp = null;
            _myitem0 = null;
            _myitem1 = null;
            _mystrip = null;
            base.Dispose();
        }
    }
}
#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private TSEcoNews[] cacheTSEcoNews = null;

        private static TSEcoNews checkTSEcoNews = new TSEcoNews();

        /// <summary>
        ///  TS Economic News Add On
        /// </summary>
        /// <returns></returns>
        public TSEcoNews TSEcoNews(DockingPlace dp)
        {
            return TSEcoNews(Input, dp);
        }

        /// <summary>
        ///  TS Economic News Add On
        /// </summary>
        /// <returns></returns>
        public TSEcoNews TSEcoNews(Data.IDataSeries input, DockingPlace dp)
        {
            checkTSEcoNews.Dp = dp;
            dp = checkTSEcoNews.Dp;

            if (cacheTSEcoNews != null)
                for (int idx = 0; idx < cacheTSEcoNews.Length; idx++)
                    if (cacheTSEcoNews[idx].Dp == dp && cacheTSEcoNews[idx].EqualsInput(input))
                        return cacheTSEcoNews[idx];

            TSEcoNews indicator = new TSEcoNews();
            indicator.BarsRequired = BarsRequired;
            indicator.CalculateOnBarClose = CalculateOnBarClose;
            indicator.Input = input;
            indicator.Dp = dp;
            indicator.SetUp();

            TSEcoNews[] tmp = new TSEcoNews[cacheTSEcoNews == null ? 1 : cacheTSEcoNews.Length + 1];
            if (cacheTSEcoNews != null)
                cacheTSEcoNews.CopyTo(tmp, 0);
            tmp[tmp.Length - 1] = indicator;
            cacheTSEcoNews = tmp;
            Indicators.Add(indicator);

            return indicator;
        }

    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        ///  TS Economic News Add On
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TSEcoNews TSEcoNews(DockingPlace dp)
        {
            return _indicator.TSEcoNews(Input, dp);
        }

        /// <summary>
        ///  TS Economic News Add On
        /// </summary>
        /// <returns></returns>
        public Indicator.TSEcoNews TSEcoNews(Data.IDataSeries input, DockingPlace dp)
        {
            return _indicator.TSEcoNews(input, dp);
        }

    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        ///  TS Economic News Add On
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TSEcoNews TSEcoNews(DockingPlace dp)
        {
            return _indicator.TSEcoNews(Input, dp);
        }

        /// <summary>
        ///  TS Economic News Add On
        /// </summary>
        /// <returns></returns>
        public Indicator.TSEcoNews TSEcoNews(Data.IDataSeries input, DockingPlace dp)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TSEcoNews(input, dp);
        }

    }
}
#endregion
