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
    /// A basic example of how to remove unused properties
    /// </summary>
    [Description("A basic example of how to remove unused properties")]
    public class TestProperties : Indicator, ICustomTypeDescriptor
    {
        #region Variables
        // Wizard generated variables
            private int inputA = 1; // Default setting for InputA
            private int inputB = 1; // Default setting for InputB
			private bool showA = true;
			private bool showB = false;
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

        [Description("This is input A")]
        [GridCategory("Parameters")]
        public int InputA
        {
            get { return inputA; }
            set { inputA = Math.Max(1, value); }
        }

        [Description("This is input B")]
        [GridCategory("Parameters")]
        public int InputB
        {
            get { return inputB; }
            set { inputB = Math.Max(1, value); }
        }
		
		[Description("Show or hide A")]
        [GridCategory("Parameters")]
        [RefreshProperties(RefreshProperties.All)]
        public bool ShowA
        {
            get { return showA; }
            set { showA = value; }
        }
		
		[Description("This is input B")]
        [GridCategory("Parameters")]
        [RefreshProperties(RefreshProperties.All)]
        public bool ShowB
        {
            get { return showB; }
            set { showB = value; }
        }
        #endregion

        #region Custom Property Manipulation

        private void ModifyProperties(PropertyDescriptorCollection col)
        {
            if (!ShowB)
            {
                col.Remove(col.Find("InputB", true));
            }
            if (!ShowA)
            {
                col.Remove(col.Find("InputA", true));
            }
        }

        #endregion


        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(GetType());
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(GetType());
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(GetType());
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(GetType());
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(GetType());
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(GetType());
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(GetType(), editorBaseType);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(GetType(), attributes);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(GetType());
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection orig = TypeDescriptor.GetProperties(GetType(), attributes);
            PropertyDescriptor[] arr = new PropertyDescriptor[orig.Count];
            orig.CopyTo(arr, 0);
            PropertyDescriptorCollection col = new PropertyDescriptorCollection(arr);

            ModifyProperties(col);
            return col;
        
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(GetType());
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
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
        private TestProperties[] cacheTestProperties = null;

        private static TestProperties checkTestProperties = new TestProperties();

        /// <summary>
        /// A basic example of how to remove unused properties
        /// </summary>
        /// <returns></returns>
        public TestProperties TestProperties(int inputA, int inputB, bool showA, bool showB)
        {
            return TestProperties(Input, inputA, inputB, showA, showB);
        }

        /// <summary>
        /// A basic example of how to remove unused properties
        /// </summary>
        /// <returns></returns>
        public TestProperties TestProperties(Data.IDataSeries input, int inputA, int inputB, bool showA, bool showB)
        {
            if (cacheTestProperties != null)
                for (int idx = 0; idx < cacheTestProperties.Length; idx++)
                    if (cacheTestProperties[idx].InputA == inputA && cacheTestProperties[idx].InputB == inputB && cacheTestProperties[idx].ShowA == showA && cacheTestProperties[idx].ShowB == showB && cacheTestProperties[idx].EqualsInput(input))
                        return cacheTestProperties[idx];

            lock (checkTestProperties)
            {
                checkTestProperties.InputA = inputA;
                inputA = checkTestProperties.InputA;
                checkTestProperties.InputB = inputB;
                inputB = checkTestProperties.InputB;
                checkTestProperties.ShowA = showA;
                showA = checkTestProperties.ShowA;
                checkTestProperties.ShowB = showB;
                showB = checkTestProperties.ShowB;

                if (cacheTestProperties != null)
                    for (int idx = 0; idx < cacheTestProperties.Length; idx++)
                        if (cacheTestProperties[idx].InputA == inputA && cacheTestProperties[idx].InputB == inputB && cacheTestProperties[idx].ShowA == showA && cacheTestProperties[idx].ShowB == showB && cacheTestProperties[idx].EqualsInput(input))
                            return cacheTestProperties[idx];

                TestProperties indicator = new TestProperties();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.InputA = inputA;
                indicator.InputB = inputB;
                indicator.ShowA = showA;
                indicator.ShowB = showB;
                Indicators.Add(indicator);
                indicator.SetUp();

                TestProperties[] tmp = new TestProperties[cacheTestProperties == null ? 1 : cacheTestProperties.Length + 1];
                if (cacheTestProperties != null)
                    cacheTestProperties.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTestProperties = tmp;
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
        /// A basic example of how to remove unused properties
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TestProperties TestProperties(int inputA, int inputB, bool showA, bool showB)
        {
            return _indicator.TestProperties(Input, inputA, inputB, showA, showB);
        }

        /// <summary>
        /// A basic example of how to remove unused properties
        /// </summary>
        /// <returns></returns>
        public Indicator.TestProperties TestProperties(Data.IDataSeries input, int inputA, int inputB, bool showA, bool showB)
        {
            return _indicator.TestProperties(input, inputA, inputB, showA, showB);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// A basic example of how to remove unused properties
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TestProperties TestProperties(int inputA, int inputB, bool showA, bool showB)
        {
            return _indicator.TestProperties(Input, inputA, inputB, showA, showB);
        }

        /// <summary>
        /// A basic example of how to remove unused properties
        /// </summary>
        /// <returns></returns>
        public Indicator.TestProperties TestProperties(Data.IDataSeries input, int inputA, int inputB, bool showA, bool showB)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TestProperties(input, inputA, inputB, showA, showB);
        }
    }
}
#endregion
