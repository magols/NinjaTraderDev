//###
//### Calculate Risk Reward Ratio from User-Drawn price levels
//### Draws 3 Horizontal lines, Named 'Entry', 'Stop', 'Target' respectively
//### Risk/Reward and Position Sizing will be calculated based on these lines
//### and User entries in the AcctSize, AcctRisk, and FixedShares user variables
//###
//### User		Date 		Description
//### ------	-------- 	-------------
//### Gaston	Feb 2011	Created
//### Gaston	Feb 2012	Added Position sizing
//### CKKOH     Mar 2012    1. Enhanced Position Sizing for FX with Lots type support.
//###                       2. Added a toolstrip button to toggle the Risk/Reward analysis.
//###
#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Text.RegularExpressions;	//Required for Regular Expressions
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using a1RiskReward_v2.Utility;
#endregion

namespace NinjaTrader.Indicator
{
    [Description("Calculate Trade Risk/Reward Ratio and Position Sizing")]
    public class a1RiskReward_v2 : Indicator
    {
        #region Variables
		double entryPrice  = 0;
		double stopPrice   = 0;
		double targetPrice = 0;
		double risk        = 0;
		double reward      = 0;
		double riskReward  = 0;
		double dollarRisk  = 0;			//### Amount in dollars to risk for a trade
		double userDollarRisk =0;		//### User specified Amount in dollars to risk for a trade
		double userAcctSize   =0;		//### User Account size i dollars
		double userAcctRisk   =2;		//### Percent of user account to risk
		double dollarsPerTick =.01;		//### How much each tick is worth in dollars
		int    fixedShares = 1;			//### User fixed share sizing
		int    tickStop    = 0;
		int    tickTarget  = 0;
		int    tradeSide   = 0;
		int    shares      = 0;

        // 28Mar12 CKKOH
        bool   rewardRiskMgtEnabled = false;
        int    sharesMultiplier = 1;
        FXLotsType fxLotsType   = FXLotsType.Standard;

        // 28Mar12 CKKOH
        System.Windows.Forms.ToolStrip toolStrip                    = null;
        System.Windows.Forms.ToolStripButton toolStripButton        = null;
        System.Windows.Forms.ToolStripSeparator toolStripSeparator  = null;
        System.Drawing.Font btnBoldFont = null;
        System.Drawing.Font btnDefFont  = null;
		
        IHorizontalLine entry  = null;
        IHorizontalLine stop   = null;
        IHorizontalLine target = null;
		
		System.Drawing.Font boldFont 	= new Font("Courier", 9,System.Drawing.FontStyle.Bold);
		StringFormat format	= new StringFormat();
		string priceFormat = "0.00";
			
        #endregion

        protected override void Initialize() {
            Overlay	= true;
			format.Alignment = StringAlignment.Far;
        }

        protected override void OnStartUp()
        {
            // 28Mar12 CKKOH
            if ( Instrument.MasterInstrument.InstrumentType == InstrumentType.Currency )
            {
                switch (fxLotsType)
                {
                    case FXLotsType.Mini:
                        sharesMultiplier = 10;
                        break;
                    case FXLotsType.Micro:
                        sharesMultiplier = 100;
                        break;
                    default:
                        break;
                }
            }
            if ( ChartControl != null )
            {
                toolStrip = (ToolStrip)ChartControl.Controls["tsrTool"];

                // Add a separator
                toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
                toolStripSeparator.Name = "separator";
                toolStrip.Items.Add(toolStripSeparator);

                // Add a toggle button
                toolStripButton = new System.Windows.Forms.ToolStripButton("EnableRR");
                toolStripButton.Name = "EnableRRButton";
                toolStripButton.Text = "$$";
                toolStripButton.Click += rewardRiskButtonClick;
                toolStripButton.Enabled = true;
                toolStripButton.CheckState = CheckState.Unchecked;
                toolStripButton.CheckOnClick = true;
                toolStripButton.ForeColor = Color.Black;
                toolStrip.Items.Add(toolStripButton);

                btnBoldFont = new Font(toolStripButton.Font, toolStripButton.Font.Style | FontStyle.Bold);
                btnDefFont = new Font(toolStripButton.Font, toolStripButton.Font.Style);
            }

        }

        protected override void OnTermination()
        {
            if ( toolStrip != null )
            {
                if ( toolStripButton != null )
                    toolStrip.Items.RemoveByKey("EnableRRButton");
                if ( toolStripSeparator != null )
                    toolStrip.Items.RemoveByKey("separator");
            }
            toolStrip = null;
            toolStripButton = null;
            toolStripSeparator = null;
        }

        protected override void OnBarUpdate() {
			if ( CurrentBar == Bars.Count-2 ) {
					//### init
				priceFormat = Regex.Replace(((decimal)TickSize).ToString(),"([\\d])","0");
				dollarsPerTick = Instrument.MasterInstrument.PointValue * TickSize;
					//### Draw trade target lines
#if REMOVED
				if ( stopPrice <= 0 ) {
					stopPrice   = High[LowestBar(Low,ChartControl.BarsPainted)];
					targetPrice = Low[HighestBar(High,ChartControl.BarsPainted)];
					entryPrice  = stopPrice+((targetPrice-stopPrice)/2);
				}
#endif
                if ( rewardRiskMgtEnabled )
                {
                    DrawHorizontalLine("Target", true, targetPrice, Color.Blue, DashStyle.Solid, 3);
                    DrawHorizontalLine("Entry", true, entryPrice, Color.Sienna, DashStyle.Solid, 3);
                    DrawHorizontalLine("Stop", true, stopPrice, Color.Red, DashStyle.Solid, 3);
                }
			}
		}
		
		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max) {
			base.Plot(graphics, bounds, min, max);
			int y=0;

            // 28Mar12 CKKOH
            if ( rewardRiskMgtEnabled == false )
                return;

			try {
			if ( entry == null || stop == null || target == null ) {
			  foreach (IDrawObject draw in DrawObjects) {
    			if (draw.DrawType == DrawType.HorizontalLine) {
					draw.Locked = false;
			        switch (draw.Tag.ToUpper()) {
					case "ENTRY":
			        	entry = (IHorizontalLine) draw;
	            		break;
					case "STOP":
			        	stop = (IHorizontalLine) draw;
						break;         
					case "TARGET":
			        	target = (IHorizontalLine) draw;
						break;
					default:
						break;
			        }		
			    }
			  }
			}
			
			if ( entry != null ) {
				entryPrice = Instrument.MasterInstrument.Round2TickSize(entry.Y);
				y = ChartControl.GetYByValue(Bars, entryPrice);
				tradeSide  = ( stopPrice <= entryPrice ) ? 1 : -1;
				risk       = (entryPrice-stopPrice)*tradeSide;
				tickStop   = (int)Math.Round(risk/TickSize);
				reward     = (targetPrice-entryPrice)*tradeSide;
				tickTarget = (int)Math.Round(reward/TickSize);
				riskReward = ( risk != 0 ) ? reward/risk : 0;
				
					//### Risk Acct Percentage Specified
				if ( userAcctSize > 0 && userAcctRisk > 0 ) dollarRisk = userAcctSize * (userAcctRisk/100);
					//### Risk Explicit Dollar Amount Specified
				if ( userDollarRisk > 0 ) dollarRisk = userDollarRisk;
				
					//### Fixed Shares Specified
				if ( fixedShares > 0 ) {
					shares     = fixedShares;
					dollarRisk = shares * tickStop * dollarsPerTick;
				}
					//### Calculated Shares
				else {
					if ( tickStop > 0 && dollarsPerTick > 0 )
                        // Modified by CKKOH
						//shares = (int)(dollarRisk/(tickStop*dollarsPerTick));
                        shares = (int)((dollarRisk * sharesMultiplier)/ (tickStop * dollarsPerTick));
					else shares = 0;
				}
				graphics.DrawString( ((tradeSide>0)?"Long":"Short") +":  " +shares +" @ " +entryPrice.ToString(priceFormat) +"\n"  +tickStop +"T   [$"+dollarRisk.ToString("0") +"]", boldFont, entry.Pen.Brush, ChartControl.CanvasRight, y, format );
			}
			if ( stop != null ) {
				stopPrice = Instrument.MasterInstrument.Round2TickSize(stop.Y);
				y = ChartControl.GetYByValue(Bars, stopPrice);
                // Modified by CKKOH
				//graphics.DrawString("Stp:  " +shares +" @ " +((decimal)stopPrice).ToString(priceFormat) +"\n" +tickStop +"T   $"+(shares*tickStop*dollarsPerTick).ToString("0"), boldFont, stop.Pen.Brush, ChartControl.CanvasRight, y, format);
                graphics.DrawString("Stp:  " + shares + " @ " + ((decimal)stopPrice).ToString(priceFormat) + "\n" + tickStop + "T   $" + ((shares * tickStop * dollarsPerTick) / sharesMultiplier).ToString("0"), boldFont, stop.Pen.Brush, ChartControl.CanvasRight, y, format);
            }
			if ( target != null ) {
				targetPrice = Instrument.MasterInstrument.Round2TickSize(target.Y);
				y = ChartControl.GetYByValue(Bars, targetPrice);
                // Modified by CKKOH
                //graphics.DrawString("Tgt:  " +shares +" @ " +targetPrice.ToString(priceFormat) +"\n" +riskReward.ToString("0.##") +"R    " +tickTarget +"T   $" +(shares*tickTarget*dollarsPerTick).ToString("0"), boldFont, target.Pen.Brush, ChartControl.CanvasRight, y, format);
                graphics.DrawString("Tgt:  " + shares + " @ " + targetPrice.ToString(priceFormat) + "\n" + riskReward.ToString("0.##") + "R    " + tickTarget + "T   $" + ((shares * tickTarget * dollarsPerTick) / sharesMultiplier).ToString("0"), boldFont, target.Pen.Brush, ChartControl.CanvasRight, y, format);

			}
			} catch (Exception ex){
				//Print(ex.ToString());
				Print("(RISK_REWARD) Need Entry, Stop and Target lines");
				entry = null; stop = null; target = null;
				entryPrice = 0; stopPrice = 0; targetPrice = 0;
			}			
		}

        private void rewardRiskButtonClick(object s, EventArgs e)
        {
            if ( toolStripButton.Checked )
            {
                // Change the button text to Bold Green color.
                toolStripButton.Font = btnBoldFont;
                toolStripButton.ForeColor = Color.Green;

                if ( ChartControl != null )
                {
                    stopPrice = High[LowestBar(Low, ChartControl.BarsPainted)];
                    targetPrice = Low[HighestBar(High, ChartControl.BarsPainted)];
                    entryPrice = stopPrice + ((targetPrice - stopPrice) / 2);

                    DrawHorizontalLine("Target", true, targetPrice, Color.Green, DashStyle.Solid, 3);
                    DrawHorizontalLine("Entry", true, entryPrice, Color.Sienna, DashStyle.Solid, 3);
                    DrawHorizontalLine("Stop", true, stopPrice, Color.Red, DashStyle.Solid, 3);

                    ChartControl.Refresh();
                } // if (ChartControl != null)

                rewardRiskMgtEnabled = true;
            }
            else
            {
                // Revert button text to its default.
                toolStripButton.Font = btnDefFont;
                toolStripButton.ForeColor = Color.Black;

                if ( ChartControl != null )
                {
                    RemoveDrawObject("Target");
                    RemoveDrawObject("Entry");
                    RemoveDrawObject("Stop");

                    entry = null;
                    target = null;
                    stop = null;

                    ChartControl.Refresh();
                } // if (ChartControl != null)

                rewardRiskMgtEnabled = false;
            }
        }

        #region Properties
		[Category("Parameters")]
        [Description("Size of trading account in dollars")]
        [Gui.Design.DisplayNameAttribute("1. UserAcctSize")]
        public double UserAcctSize {
            get { return userAcctSize; }
            set { userAcctSize = Math.Max(0,value); }
        }

		[Category("Parameters")]
        [Description("Percent of Account to Risk per trade Ex: 2 is 2%")]
        [Gui.Design.DisplayNameAttribute("2. UserAcctRisk (%)")]
        public double UserAcctRisk {
            get { return userAcctRisk; }
            set { userAcctRisk = Math.Max(0,value); }
        }

		[Category("Parameters")]
        [Description("Dollar Account to Risk per trade Ex: 100 is $100")]
        [Gui.Design.DisplayNameAttribute("3. DollarRisk")]
        public double UserDollarRisk {
            get { return userDollarRisk; }
            set { userDollarRisk = Math.Max(0,value); }
        }

		[Category("Parameters")]
        [Description("Fixed Share/Contract Size")]
        [Gui.Design.DisplayNameAttribute("4. FixedPositionSize")]
        public int FixedShares {
            get { return fixedShares; }
            set { fixedShares = Math.Max(0,value); }
        }

        [Category("Parameters")] // Use of Category will hide it from parameter list
        [Description("Lots Type (For Forex only).")]
        [Gui.Design.DisplayNameAttribute("5. Lots Type")]
        public FXLotsType LotsType
        {
            get { return fxLotsType; }
            set { fxLotsType = value; }
        }

        public double EntryPrice {
            get { return entryPrice; }
            set { entryPrice = value; }
        }
        public double StopPrice {
            get { return stopPrice; }
            set { stopPrice = value; }
        }
        public double TargetPrice {
            get { return targetPrice; }
            set { targetPrice = value; }
        }
		

		#endregion
    }
}

namespace a1RiskReward_v2.Utility
{
    public enum FXLotsType
    {
        Standard,
        Mini,
        Micro,
    }
}
#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private a1RiskReward_v2[] cachea1RiskReward_v2 = null;

        private static a1RiskReward_v2 checka1RiskReward_v2 = new a1RiskReward_v2();

        /// <summary>
        /// Calculate Trade Risk/Reward Ratio and Position Sizing
        /// </summary>
        /// <returns></returns>
        public a1RiskReward_v2 a1RiskReward_v2(int fixedShares, FXLotsType lotsType, double userAcctRisk, double userAcctSize, double userDollarRisk)
        {
            return a1RiskReward_v2(Input, fixedShares, lotsType, userAcctRisk, userAcctSize, userDollarRisk);
        }

        /// <summary>
        /// Calculate Trade Risk/Reward Ratio and Position Sizing
        /// </summary>
        /// <returns></returns>
        public a1RiskReward_v2 a1RiskReward_v2(Data.IDataSeries input, int fixedShares, FXLotsType lotsType, double userAcctRisk, double userAcctSize, double userDollarRisk)
        {
            if (cachea1RiskReward_v2 != null)
                for (int idx = 0; idx < cachea1RiskReward_v2.Length; idx++)
                    if (cachea1RiskReward_v2[idx].FixedShares == fixedShares && cachea1RiskReward_v2[idx].LotsType == lotsType && Math.Abs(cachea1RiskReward_v2[idx].UserAcctRisk - userAcctRisk) <= double.Epsilon && Math.Abs(cachea1RiskReward_v2[idx].UserAcctSize - userAcctSize) <= double.Epsilon && Math.Abs(cachea1RiskReward_v2[idx].UserDollarRisk - userDollarRisk) <= double.Epsilon && cachea1RiskReward_v2[idx].EqualsInput(input))
                        return cachea1RiskReward_v2[idx];

            lock (checka1RiskReward_v2)
            {
                checka1RiskReward_v2.FixedShares = fixedShares;
                fixedShares = checka1RiskReward_v2.FixedShares;
                checka1RiskReward_v2.LotsType = lotsType;
                lotsType = checka1RiskReward_v2.LotsType;
                checka1RiskReward_v2.UserAcctRisk = userAcctRisk;
                userAcctRisk = checka1RiskReward_v2.UserAcctRisk;
                checka1RiskReward_v2.UserAcctSize = userAcctSize;
                userAcctSize = checka1RiskReward_v2.UserAcctSize;
                checka1RiskReward_v2.UserDollarRisk = userDollarRisk;
                userDollarRisk = checka1RiskReward_v2.UserDollarRisk;

                if (cachea1RiskReward_v2 != null)
                    for (int idx = 0; idx < cachea1RiskReward_v2.Length; idx++)
                        if (cachea1RiskReward_v2[idx].FixedShares == fixedShares && cachea1RiskReward_v2[idx].LotsType == lotsType && Math.Abs(cachea1RiskReward_v2[idx].UserAcctRisk - userAcctRisk) <= double.Epsilon && Math.Abs(cachea1RiskReward_v2[idx].UserAcctSize - userAcctSize) <= double.Epsilon && Math.Abs(cachea1RiskReward_v2[idx].UserDollarRisk - userDollarRisk) <= double.Epsilon && cachea1RiskReward_v2[idx].EqualsInput(input))
                            return cachea1RiskReward_v2[idx];

                a1RiskReward_v2 indicator = new a1RiskReward_v2();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.FixedShares = fixedShares;
                indicator.LotsType = lotsType;
                indicator.UserAcctRisk = userAcctRisk;
                indicator.UserAcctSize = userAcctSize;
                indicator.UserDollarRisk = userDollarRisk;
                Indicators.Add(indicator);
                indicator.SetUp();

                a1RiskReward_v2[] tmp = new a1RiskReward_v2[cachea1RiskReward_v2 == null ? 1 : cachea1RiskReward_v2.Length + 1];
                if (cachea1RiskReward_v2 != null)
                    cachea1RiskReward_v2.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachea1RiskReward_v2 = tmp;
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
        /// Calculate Trade Risk/Reward Ratio and Position Sizing
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.a1RiskReward_v2 a1RiskReward_v2(int fixedShares, FXLotsType lotsType, double userAcctRisk, double userAcctSize, double userDollarRisk)
        {
            return _indicator.a1RiskReward_v2(Input, fixedShares, lotsType, userAcctRisk, userAcctSize, userDollarRisk);
        }

        /// <summary>
        /// Calculate Trade Risk/Reward Ratio and Position Sizing
        /// </summary>
        /// <returns></returns>
        public Indicator.a1RiskReward_v2 a1RiskReward_v2(Data.IDataSeries input, int fixedShares, FXLotsType lotsType, double userAcctRisk, double userAcctSize, double userDollarRisk)
        {
            return _indicator.a1RiskReward_v2(input, fixedShares, lotsType, userAcctRisk, userAcctSize, userDollarRisk);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Calculate Trade Risk/Reward Ratio and Position Sizing
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.a1RiskReward_v2 a1RiskReward_v2(int fixedShares, FXLotsType lotsType, double userAcctRisk, double userAcctSize, double userDollarRisk)
        {
            return _indicator.a1RiskReward_v2(Input, fixedShares, lotsType, userAcctRisk, userAcctSize, userDollarRisk);
        }

        /// <summary>
        /// Calculate Trade Risk/Reward Ratio and Position Sizing
        /// </summary>
        /// <returns></returns>
        public Indicator.a1RiskReward_v2 a1RiskReward_v2(Data.IDataSeries input, int fixedShares, FXLotsType lotsType, double userAcctRisk, double userAcctSize, double userDollarRisk)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.a1RiskReward_v2(input, fixedShares, lotsType, userAcctRisk, userAcctSize, userDollarRisk);
        }
    }
}
#endregion
