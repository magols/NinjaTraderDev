// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Collections;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// Pivot Points.
	/// </summary>
	[Description("Pivot Points.")]
	public class Pivots : Indicator
	{
		#region Variables
		private	SolidBrush[]		brushes				= { new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black), new SolidBrush(Color.Black) };
		private DateTime			cacheSessionBegin	= Cbi.Globals.MinDate;
		private DateTime			cacheSessionEnd		= Cbi.Globals.MinDate;
		private DateTime			cacheSessionDate	= Cbi.Globals.MinDate;
		private DateTime			cacheWeeklyEndDate	= Cbi.Globals.MinDate;
		private DateTime			cacheMonthlyEndDate	= Cbi.Globals.MinDate;
		private	double				currentClose		= 0;
		private DateTime			currentDate			= Cbi.Globals.MinDate;
		private DateTime			currentMonth		= Cbi.Globals.MinDate;
		private DateTime			currentWeek			= Cbi.Globals.MinDate;
		private	double				currentHigh			= double.MinValue;
		private	double				currentLow			= double.MaxValue;
		private Bars				dailyBars			= null;
		private	string				errorData			= "Insufficient historical data to calculate pivots. Increase chart look back period (days back).";
        private float				errorTextWidth		= 0;
        private float				errorTextHeight		= 0;
		private bool				existsHistDailyData	= false;
		private bool				isDailyDataLoaded	= false;
		private ArrayList			newSessionBarIdxArr	= new ArrayList();
		private Data.PivotRange		pivotRangeType		= PivotRange.Daily;
		private HLCCalculationMode	priorDayHLC			= HLCCalculationMode.DailyBars;
		private	double				pp					= 0;
		private	double				r1					= 0;
		private	double				r2					= 0;
		private	double				r3					= 0;
		private	double				s1					= 0;
		private	double				s2					= 0;
		private	double				s3					= 0;
		private DateTime			sessionDateTmp		= Cbi.Globals.MinDate;
		private DateTime			sessionDateDaily	= Cbi.Globals.MinDate;
		private	StringFormat		stringFormatFar		= new StringFormat();
		private	StringFormat		stringFormatNear	= new StringFormat();
		private SolidBrush			textBrush			= new SolidBrush(Color.Black);
		private Font				textFont			= new Font("Arial", 30);
		private double				userDefinedClose	= 0;
		private double				userDefinedHigh		= 0;
		private double				userDefinedLow		= 0;
		private int					width				= 20;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Black,	"PP"));
			Add(new Plot(Color.Blue,	"R1"));
			Add(new Plot(Color.Red,		"S1"));
			Add(new Plot(Color.Blue,	"R2"));
			Add(new Plot(Color.Red,		"S2"));
			Add(new Plot(Color.Blue,	"R3"));
			Add(new Plot(Color.Red,		"S3"));
			
			AutoScale					= false;
			Overlay						= true;
			stringFormatFar.Alignment	= StringAlignment.Far;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (Bars == null)
				return; 
			if (!Bars.BarsType.IsIntraday && Bars.Period.Id != PeriodType.Day)
				return;
			if (Bars.Period.Id == PeriodType.Day && pivotRangeType == PivotRange.Daily)
				return;
			if (Bars.Period.Id == PeriodType.Day && Bars.Period.Value > 1)
				return;

			// pivots only work for 
			// - intraday
			// - 1 day chart with PivotRange Weekly or Monthly

			if (!isDailyDataLoaded)
			{
				if (priorDayHLC == HLCCalculationMode.DailyBars && Bars.BarsType.IsIntraday) 
				{
					Enabled = false;
					System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(GetBarsNow));
					return;
				}

				existsHistDailyData = false;
				isDailyDataLoaded	= true;
			}

			IBar dailyBar;
			if (existsHistDailyData) 
			{
				sessionDateDaily = GetLastBarSessionDate(Time[0], Bars, PivotRange.Daily);
				dailyBar = dailyBars.Get(dailyBars.GetBar(sessionDateDaily));

				if (dailyBar.Time.Date > sessionDateDaily.Date)
				{
					for (DateTime i = sessionDateDaily; i >= dailyBars.GetTime(0); i = i.AddDays(-1))
					{
						dailyBar = dailyBars.Get(dailyBars.GetBar(i));
						if (dailyBar.Time.Date == i.Date)
							break;
					}
				}
			} 
			else 
				dailyBar = null;

			double	high		= existsHistDailyData ? dailyBar.High : High[0];
			double	low			= existsHistDailyData ? dailyBar.Low : Low[0];
			double	close		= existsHistDailyData ? dailyBar.Close : Close[0];

			DateTime lastBarTimeStamp = GetLastBarSessionDate(Time[0], Bars, pivotRangeType);
			if ((currentDate != Cbi.Globals.MinDate && pivotRangeType == PivotRange.Daily && lastBarTimeStamp != currentDate)
				|| (currentWeek != Cbi.Globals.MinDate && pivotRangeType == PivotRange.Weekly && lastBarTimeStamp != currentWeek) 
				|| (currentMonth != Cbi.Globals.MinDate && pivotRangeType == PivotRange.Monthly && lastBarTimeStamp != currentMonth)) 
			{
				pp				= (currentHigh + currentLow + currentClose) / 3;
				s1				= 2 * pp - currentHigh;
				r1				= 2 * pp - currentLow;
				s2				= pp - (currentHigh - currentLow);
				r2				= pp + (currentHigh - currentLow);
				s3				= pp - 2 * (currentHigh - currentLow);
				r3				= pp + 2 * (currentHigh - currentLow);
				currentClose	= (priorDayHLC == HLCCalculationMode.UserDefinedValues) ? userDefinedClose : close;
				currentHigh		= (priorDayHLC == HLCCalculationMode.UserDefinedValues) ? userDefinedHigh : high;
				currentLow		= (priorDayHLC == HLCCalculationMode.UserDefinedValues) ? userDefinedLow : low;
			}
			else
			{
				currentClose	= (priorDayHLC == HLCCalculationMode.UserDefinedValues) ? userDefinedClose : close;
				currentHigh		= (priorDayHLC == HLCCalculationMode.UserDefinedValues) ? userDefinedHigh : Math.Max(currentHigh, high);
				currentLow		= (priorDayHLC == HLCCalculationMode.UserDefinedValues) ? userDefinedLow : Math.Min(currentLow, low);
			}

			if (pivotRangeType == PivotRange.Daily)
				currentDate = lastBarTimeStamp;
			if (pivotRangeType == PivotRange.Weekly)
				currentWeek = lastBarTimeStamp;
			if (pivotRangeType == PivotRange.Monthly)
				currentMonth = lastBarTimeStamp;

			if ((pivotRangeType == PivotRange.Daily && currentDate != Cbi.Globals.MinDate)
				|| (pivotRangeType == PivotRange.Weekly && currentWeek != Cbi.Globals.MinDate)
				|| (pivotRangeType == PivotRange.Monthly && currentMonth != Cbi.Globals.MinDate))
			{
				PP.Set(pp);
				R1.Set(r1);
				S1.Set(s1);
				R2.Set(r2);
				S2.Set(s2);
				R3.Set(r3);
				S3.Set(s3);
			}
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Type of period for pivot points.")]
		[GridCategory("Parameters")]
		public Data.PivotRange PivotRangeType 
		{
			get { return pivotRangeType; }
			set { pivotRangeType = value; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries PP
		{
			get { return Values[0]; }
		}

		/// <summary>
		/// </summary>
		[Description("Approach for calculation the prior day HLC values.")]
		[GridCategory("Parameters")]
		public Data.HLCCalculationMode PriorDayHLC
		{
			get { return priorDayHLC; }
			set { priorDayHLC = value; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries R1
		{
			get { return Values[1]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries R2
		{
			get { return Values[3]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries R3
		{
			get { return Values[5]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries S1
		{
			get { return Values[2]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries S2
		{
			get { return Values[4]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries S3
		{
			get { return Values[6]; }
		}

		/// <summary>
		/// close value for user defined pivots calculation
		/// </summary>
		[Description("User defined prior session close value used for pivots calculation.")]
		[GridCategory("\rUser defined values")]
		public double UserDefinedClose
		{
			get { return userDefinedClose; }
			set { userDefinedClose = value; }
		}

		/// <summary>
		/// high value for user defined pivots calculation
		/// </summary>
		[Description("User defined prior session high value used for pivots calculation.")]
		[GridCategory("\rUser defined values")]
		public double UserDefinedHigh
		{
			get { return userDefinedHigh; }
			set { userDefinedHigh = value; }
		}

		/// <summary>
		/// low value for user defined pivots calculation
		/// </summary>
		[Description("User defined prior session low value used for pivots calculation.")]
		[GridCategory("\rUser defined values")]
		public double UserDefinedLow
		{
			get { return userDefinedLow; }
			set { userDefinedLow = value; }
		}

		/// <summary>
		/// </summary>
		[Description("Width of the pivot lines as # of bars.")]
		[GridCategory("Parameters")]
		public int Width
		{
			get { return width; }
			set { width = Math.Max(1, value); }
		}
		#endregion

		#region Miscellaneous

		/// <summary>
		/// Have the .GetBars() call in a seperate thread. Reason: NT internal pool locking would not work if called in main thread.
		/// </summary>
		/// <param name="state"></param>
		private void GetBarsNow(object state)
		{
			if (Disposed)
				return;

			dailyBars			= Data.Bars.GetBars(Bars.Instrument, new Period(PeriodType.Day, 1, Bars.Period.MarketDataType), Bars.From.AddDays(-7), Bars.To, (Session) Bars.Session.Clone(), Data.Bars.SplitAdjust, Data.Bars.DividendAdjust);
			existsHistDailyData	= (dailyBars.Count <= 1) ? false : true;
			isDailyDataLoaded	= true;
			Enabled				= true;

			Cbi.Globals.SynchronizeInvoke.AsyncInvoke(new System.Windows.Forms.MethodInvoker(InvalidateNow), null);
		}

		private DateTime GetLastBarSessionDate(DateTime time, Data.Bars bars, PivotRange pivotRange)
		{
			if (time > cacheSessionEnd)
			{
				if (Bars.BarsType.IsIntraday)
				{
					Bars.Session.GetNextBeginEnd(time, out cacheSessionBegin, out cacheSessionEnd);
					sessionDateTmp = System.TimeZoneInfo.ConvertTime(cacheSessionEnd.AddSeconds(-1), System.TimeZoneInfo.Local, Bars.Session.TimeZoneInfo).Date;
				}
				else
					sessionDateTmp = time.Date;
			}	
			
			if (pivotRange == PivotRange.Daily)
			{	
				if(sessionDateTmp != cacheSessionDate)
				{
					if (newSessionBarIdxArr.Count == 0 || (newSessionBarIdxArr.Count > 0 && CurrentBar > (int) newSessionBarIdxArr[newSessionBarIdxArr.Count - 1]))
						newSessionBarIdxArr.Add(CurrentBar);
					cacheSessionDate = sessionDateTmp;
				}
				return sessionDateTmp;
			}
			else if (pivotRange == PivotRange.Weekly) 
			{
				DateTime tmpWeeklyEndDate = RoundUpTimeToPeriodTime(sessionDateTmp, PivotRange.Weekly);
				if (tmpWeeklyEndDate != cacheWeeklyEndDate)
				{
					if (newSessionBarIdxArr.Count == 0 || (newSessionBarIdxArr.Count > 0 && CurrentBar > (int) newSessionBarIdxArr[newSessionBarIdxArr.Count - 1]))
						newSessionBarIdxArr.Add(CurrentBar);
					cacheWeeklyEndDate = tmpWeeklyEndDate;
				}
				return tmpWeeklyEndDate;
			}
			else // pivotRange == PivotRange.Monthly
			{
				DateTime tmpMonthlyEndDate = RoundUpTimeToPeriodTime(sessionDateTmp, PivotRange.Monthly);
				if (tmpMonthlyEndDate != cacheMonthlyEndDate)
				{
					if (newSessionBarIdxArr.Count == 0 || (newSessionBarIdxArr.Count > 0 && CurrentBar > (int) newSessionBarIdxArr[newSessionBarIdxArr.Count - 1]))
						newSessionBarIdxArr.Add(CurrentBar);
					cacheMonthlyEndDate = tmpMonthlyEndDate;
				}
				return tmpMonthlyEndDate;
			}
		}

		internal void InvalidateNow()
		{
			if (Disposed || ChartControl == null)
				return;

			ChartControl.Invalidate(true);
		}

		/// <summary>
        /// Overload this method to handle the termination of an indicator. Use this method to dispose of any resources vs overloading the Dispose() method.
		/// </summary>
		protected override void OnTermination()
		{
			if (dailyBars != null)
				dailyBars.Dispose();

			textBrush.Dispose();
			foreach (SolidBrush solidBrush in brushes)
				solidBrush.Dispose();
			stringFormatFar.Dispose();	
			stringFormatNear.Dispose();
		}
	
		/// <summary>
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="bounds"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			if (Bars == null || ChartControl == null)
				return;

			// plot error if data not complete
			if (textBrush.Color != ChartControl.AxisColor || textFont != ChartControl.Font)
			{
				textBrush.Color = ChartControl.AxisColor;
                textFont = ChartControl.Font;

				SizeF errorSize	= graphics.MeasureString(errorData, textFont);
				errorTextWidth	= errorSize.Width + 5;
				errorTextHeight	= errorSize.Height + 5;
			}

			if (priorDayHLC == HLCCalculationMode.CalcFromIntradayData) 
			{
				DateTime lastBarDate = (Bars.Count == 0) ? Cbi.Globals.MaxDate : ChartControl.EquidistantBars ?
										Bars.GetTime(Math.Min(Bars.Count - 1, ChartControl.LastBarPainted)).Date 
										: Bars.GetTime(Math.Min(Bars.Count - 1, Bars.GetBar(ChartControl.LastBarTimePainted))).Date;
				if ((lastBarDate == Cbi.Globals.MaxDate)
					|| (Bars.BarsType.BuiltFrom == PeriodType.Minute
						&& ((pivotRangeType == PivotRange.Monthly && Bars.Count > 0 && Bars.GetTime(0).AddSeconds(-1).Date >= new DateTime(lastBarDate.Year, lastBarDate.Month, 1).AddMonths(-1)) 
							|| (pivotRangeType == PivotRange.Weekly && Bars.Count > 0 && Bars.GetTime(0).AddSeconds(-1).Date >= lastBarDate.AddDays(- (((int) lastBarDate.DayOfWeek) + 1) % 7).AddDays(-7))
							|| (pivotRangeType == PivotRange.Daily && Bars.Count > 0 && Bars.GetTime(0).AddSeconds(-1).Date >= lastBarDate.AddDays(-1))))
					|| (Bars.BarsType.BuiltFrom != PeriodType.Minute
						&& ((pivotRangeType == PivotRange.Monthly && Bars.Count > 0 && Bars.GetTime(0).Date >= new DateTime(lastBarDate.Year, lastBarDate.Month, 1).AddMonths(-1)) 
							|| (pivotRangeType == PivotRange.Weekly && Bars.Count > 0 && Bars.GetTime(0).Date >= lastBarDate.AddDays(-(((int) lastBarDate.DayOfWeek) + 1) % 7).AddDays(-7))
							|| (pivotRangeType == PivotRange.Daily && Bars.Count > 0 && Bars.GetTime(0).Date >= lastBarDate.AddDays(-1)))))
				{
					graphics.DrawString(errorData, ChartControl.Font, textBrush, bounds.X + bounds.Width - errorTextWidth, bounds.Y + bounds.Height - errorTextHeight, stringFormatNear);
				}
			}
			else if (priorDayHLC == HLCCalculationMode.DailyBars && existsHistDailyData)
			{
				DateTime lastBarDate = (dailyBars.Count == 0) ? Cbi.Globals.MaxDate : ChartControl.EquidistantBars ? 
					dailyBars.GetTime(Math.Min(dailyBars.Count - 1, ChartControl.LastBarPainted)).Date 
					: dailyBars.GetTime(Math.Min(dailyBars.Count - 1, dailyBars.GetBar(ChartControl.LastBarTimePainted))).Date;
				if ((lastBarDate == Cbi.Globals.MaxDate)
					|| (pivotRangeType == PivotRange.Monthly && dailyBars.GetTime(0).Date >= new DateTime(lastBarDate.Year, lastBarDate.Month, 1).AddMonths(-1)) 
					|| (pivotRangeType == PivotRange.Weekly && dailyBars.GetTime(0).Date >= lastBarDate.AddDays(-(((int) lastBarDate.DayOfWeek) + 1) % 7).AddDays(-7))
					|| (pivotRangeType == PivotRange.Daily && dailyBars.GetTime(0).Date >= lastBarDate.AddDays(-1)))
				{
					graphics.DrawString(errorData, ChartControl.Font, textBrush, bounds.X + bounds.Width - errorTextWidth, bounds.Y + bounds.Height - errorTextHeight, stringFormatNear);
				}
			} 

			float textHeight = ChartControl.Font.GetHeight();
			for (int seriesCount = 0; seriesCount < Values.Length; seriesCount++)
			{
				SolidBrush		brush				= brushes[seriesCount];
				int				firstBarIdxToPaint	= -1;
				int				lastX				= -1;
				int				lastY				= -1;
				SmoothingMode	oldSmoothingMode	= graphics.SmoothingMode;
				Gui.Chart.Plot	plot				= Plots[seriesCount];
				DataSeries		series				= (DataSeries) Values[seriesCount];

				for (int i = newSessionBarIdxArr.Count - 1; i >= 0; i--)
				{
					int prevSessionBreakIdx = (int) newSessionBarIdxArr[i];
					if (prevSessionBreakIdx <= this.LastBarIndexPainted)
					{
						firstBarIdxToPaint = prevSessionBreakIdx;
						break;
					}
				}

				using (GraphicsPath	path = new GraphicsPath()) 
				{
					if (brush.Color != plot.Pen.Color)	
						brush = new SolidBrush(plot.Pen.Color);

					for (int idx = this.LastBarIndexPainted; idx >= Math.Max(this.FirstBarIndexPainted, this.LastBarIndexPainted - Width); idx--)
					{
						if (idx - Displacement < 0 || idx - Displacement >= Bars.Count || (!ChartControl.ShowBarsRequired && idx - Displacement < BarsRequired))
							continue;
						else if (!series.IsValidPlot(idx))
							continue;

						if (idx < firstBarIdxToPaint)
							break;

						double	val = series.Get(idx);
						int		x	= ChartControl.GetXByBarIdx(BarsArray[0], idx);
						int		y	= ChartControl.GetYByValue(this, val);

						if (lastX >= 0)
						{
							if (y != lastY) // Problem here is, that last bar of old day has date of new day
								y = lastY;
							path.AddLine(lastX - plot.Pen.Width / 2, lastY, x - plot.Pen.Width / 2, y);
						}
						lastX	= x;
						lastY	= y;
					}

					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.DrawPath(plot.Pen, path);
					graphics.SmoothingMode = oldSmoothingMode;
					graphics.DrawString(plot.Name, ChartControl.Font, brush, lastX, lastY - textHeight / 2, stringFormatFar);
				}
			}
		}

		private DateTime RoundUpTimeToPeriodTime(DateTime time, PivotRange pivotRange)
		{
			if (pivotRange == PivotRange.Weekly)
			{
				DateTime periodStart = time.AddDays((6 - (((int) time.DayOfWeek) + 1) % 7));
				return periodStart.Date.AddDays(System.Math.Ceiling(System.Math.Ceiling(time.Date.Subtract(periodStart.Date).TotalDays) / 7) * 7).Date;
			}
			else if (pivotRange == PivotRange.Monthly)
			{
				DateTime result = new DateTime(time.Year, time.Month, 1); 
				return result.AddMonths(1).AddDays(-1);
			}
			else
				return time;
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
        private Pivots[] cachePivots = null;

        private static Pivots checkPivots = new Pivots();

        /// <summary>
        /// Pivot Points.
        /// </summary>
        /// <returns></returns>
        public Pivots Pivots(Data.PivotRange pivotRangeType, Data.HLCCalculationMode priorDayHLC, double userDefinedClose, double userDefinedHigh, double userDefinedLow, int width)
        {
            return Pivots(Input, pivotRangeType, priorDayHLC, userDefinedClose, userDefinedHigh, userDefinedLow, width);
        }

        /// <summary>
        /// Pivot Points.
        /// </summary>
        /// <returns></returns>
        public Pivots Pivots(Data.IDataSeries input, Data.PivotRange pivotRangeType, Data.HLCCalculationMode priorDayHLC, double userDefinedClose, double userDefinedHigh, double userDefinedLow, int width)
        {
            if (cachePivots != null)
                for (int idx = 0; idx < cachePivots.Length; idx++)
                    if (cachePivots[idx].PivotRangeType == pivotRangeType && cachePivots[idx].PriorDayHLC == priorDayHLC && Math.Abs(cachePivots[idx].UserDefinedClose - userDefinedClose) <= double.Epsilon && Math.Abs(cachePivots[idx].UserDefinedHigh - userDefinedHigh) <= double.Epsilon && Math.Abs(cachePivots[idx].UserDefinedLow - userDefinedLow) <= double.Epsilon && cachePivots[idx].Width == width && cachePivots[idx].EqualsInput(input))
                        return cachePivots[idx];

            lock (checkPivots)
            {
                checkPivots.PivotRangeType = pivotRangeType;
                pivotRangeType = checkPivots.PivotRangeType;
                checkPivots.PriorDayHLC = priorDayHLC;
                priorDayHLC = checkPivots.PriorDayHLC;
                checkPivots.UserDefinedClose = userDefinedClose;
                userDefinedClose = checkPivots.UserDefinedClose;
                checkPivots.UserDefinedHigh = userDefinedHigh;
                userDefinedHigh = checkPivots.UserDefinedHigh;
                checkPivots.UserDefinedLow = userDefinedLow;
                userDefinedLow = checkPivots.UserDefinedLow;
                checkPivots.Width = width;
                width = checkPivots.Width;

                if (cachePivots != null)
                    for (int idx = 0; idx < cachePivots.Length; idx++)
                        if (cachePivots[idx].PivotRangeType == pivotRangeType && cachePivots[idx].PriorDayHLC == priorDayHLC && Math.Abs(cachePivots[idx].UserDefinedClose - userDefinedClose) <= double.Epsilon && Math.Abs(cachePivots[idx].UserDefinedHigh - userDefinedHigh) <= double.Epsilon && Math.Abs(cachePivots[idx].UserDefinedLow - userDefinedLow) <= double.Epsilon && cachePivots[idx].Width == width && cachePivots[idx].EqualsInput(input))
                            return cachePivots[idx];

                Pivots indicator = new Pivots();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.PivotRangeType = pivotRangeType;
                indicator.PriorDayHLC = priorDayHLC;
                indicator.UserDefinedClose = userDefinedClose;
                indicator.UserDefinedHigh = userDefinedHigh;
                indicator.UserDefinedLow = userDefinedLow;
                indicator.Width = width;
                Indicators.Add(indicator);
                indicator.SetUp();

                Pivots[] tmp = new Pivots[cachePivots == null ? 1 : cachePivots.Length + 1];
                if (cachePivots != null)
                    cachePivots.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachePivots = tmp;
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
        /// Pivot Points.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Pivots Pivots(Data.PivotRange pivotRangeType, Data.HLCCalculationMode priorDayHLC, double userDefinedClose, double userDefinedHigh, double userDefinedLow, int width)
        {
            return _indicator.Pivots(Input, pivotRangeType, priorDayHLC, userDefinedClose, userDefinedHigh, userDefinedLow, width);
        }

        /// <summary>
        /// Pivot Points.
        /// </summary>
        /// <returns></returns>
        public Indicator.Pivots Pivots(Data.IDataSeries input, Data.PivotRange pivotRangeType, Data.HLCCalculationMode priorDayHLC, double userDefinedClose, double userDefinedHigh, double userDefinedLow, int width)
        {
            return _indicator.Pivots(input, pivotRangeType, priorDayHLC, userDefinedClose, userDefinedHigh, userDefinedLow, width);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Pivot Points.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Pivots Pivots(Data.PivotRange pivotRangeType, Data.HLCCalculationMode priorDayHLC, double userDefinedClose, double userDefinedHigh, double userDefinedLow, int width)
        {
            return _indicator.Pivots(Input, pivotRangeType, priorDayHLC, userDefinedClose, userDefinedHigh, userDefinedLow, width);
        }

        /// <summary>
        /// Pivot Points.
        /// </summary>
        /// <returns></returns>
        public Indicator.Pivots Pivots(Data.IDataSeries input, Data.PivotRange pivotRangeType, Data.HLCCalculationMode priorDayHLC, double userDefinedClose, double userDefinedHigh, double userDefinedLow, int width)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Pivots(input, pivotRangeType, priorDayHLC, userDefinedClose, userDefinedHigh, userDefinedLow, width);
        }
    }
}
#endregion
