using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Basler.Pylon;


namespace Frism
{

    public partial class UnoSlider : UserControl
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private IParameter parameter = null;
        private bool sliderMoving = false;
        private bool logarithmic = false;
        private TextBox valueLabel = null;
        double min = 0;
        double max = 0;
        double val = 0;
        double percent = 0;
        private readonly int SLIDER_CONSTANT_FACTOR = 10000;

        public UnoSlider()
        {
            InitializeComponent();
        }



        public IParameter Parameter
        {
            set
            {
                // Unsubscribe from the previous parameter.
                if (parameter != null)
                {
                    parameter.ParameterChanged -= ParameterChanged;
                }

                // Set the new parameter and subscribe to it.
                parameter = value;
                if (parameter != null)
                {
                    parameter.ParameterChanged += ParameterChanged;
                    UpdateValues();
                }
                else
                {
                    Reset();
                }
            }
        }

        

        public bool Logarithmic
        {
            get { return logarithmic; }
            set { this.logarithmic = value; }
        }

        private int PercentToSliderValue(double percent)
        {
            int result;
            if (logarithmic)
            {
                result = (int)(Math.Log(percent + 1, 2) * SLIDER_CONSTANT_FACTOR);
            }
            else
            {
                result = (int)(((max - min) / 100.0) * percent);
            }
            return result;
        }
        private double SliderToPercentValue(int sliderValue)
        {
            double result;
            if (logarithmic)
            {
                // Due to rounding in PercentToSliderValue, we can't reach the maximum.
                // If the slider maximum has been reached, return 100 %.
                if (slider.Maximum == sliderValue)
                {
                    result = 100.0;
                }
                else
                {
                    result = Math.Pow(2, (double)sliderValue / SLIDER_CONSTANT_FACTOR) - 1;
                }
            }
            else
            {
                result = (((double)sliderValue) / (max - min)) * 100.0;
            }
            return result;
        }

        public void SetLabel(TextBox label)
        {
            valueLabel = label;
            valueLabel.Text = "0";
        }

        private void Reset()
        {
            //slider.IsEnabled = false;
            //valueLabel.Text = "0";
        }

        private void ParameterChanged(Object sender, EventArgs e)
        {

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(ParameterChanged), sender, e);
                return;
            }
           
            try
            {
                // Update accessibility and parameter values.
                
                UpdateValues();
            }
            catch(Exception ex)
            {
                // If errors occurred, disable the control.
                //Reset();
                Logger.Error(ex.Message);
            }
        }

        // Gets the current values from the parameter and displays them.
        private void UpdateValues()
        {
            try
            {
                if (parameter != null && parameter.IsReadable)
                {
                    if (parameter.IsWritable && !sliderMoving)
                    {
                        if (this.parameter is IFloatParameter)
                        {
                            IFloatParameter floatParameter = this.parameter as IFloatParameter;
                            // Break any recursion if the value does not exactly match the slider value.
                            sliderMoving = true;
                            // Get the values.
                            min = floatParameter.GetMinimum();
                            max = floatParameter.GetMaximum();
                            val = floatParameter.GetValue();
                            percent = floatParameter.GetValuePercentOfRange();
                            slider.SmallChange = 1;
                        }
                        else
                        {
                            IIntegerParameter intParameter = this.parameter as IIntegerParameter;
                            // Break any recursion if the value does not exactly match the slider value.
                            sliderMoving = true;
                            // Get the values.
                            min = intParameter.GetMinimum();
                            max = intParameter.GetMaximum();
                            val = intParameter.GetValue();
                            percent = intParameter.GetValuePercentOfRange();
                            // Configure the SmallChange property of the parameter increment value to prevent invalid values.
                            slider.SmallChange = (int)intParameter.GetIncrement();
                        }
                        // Update the slider. Scale values by scaling factor.
                        slider.Minimum = PercentToSliderValue(0);
                        slider.Maximum = PercentToSliderValue(100);
                        slider.Value = PercentToSliderValue(percent);
                        slider.TickFrequency = (slider.Maximum - slider.Minimum) / 10;
                        

                        // Update the access status.
                        //slider.IsEnabled = parameter.IsWritable;

                        return;
                    }
                }
                else
                {
                    //slider.IsEnabled = false;
                    
                }
            }
            catch(Exception ex)
            {
                // If errors occurred, disable the control.
                //Reset();
                Logger.Error(ex.Message);
            }
            finally
            {
                sliderMoving = false;
                
            }
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.parameter != null)
            {
                try
                {
                    if (this.parameter is IFloatParameter)
                    {
                        IFloatParameter parameter = this.parameter as IFloatParameter;
                        if (parameter.IsWritable && !sliderMoving)
                        {
                            // Break any recursion if the value does not exactly match the slider value.
                            sliderMoving = true;

                            // Set the value. Scale by scaling factor.
                            parameter.SetValuePercentOfRange(SliderToPercentValue((int)slider.Value));
                            double val = parameter.GetValue();
                            valueLabel.Text = string.Format("{0:0}", val);
                            
                        }
                    }
                    else
                    {
                        IIntegerParameter parameter = this.parameter as IIntegerParameter;
                        if (parameter.IsWritable && !sliderMoving)
                        {
                            // Break any recursion if the value does not exactly match the slider value.
                            sliderMoving = true;

                            // Set the value. Scale by scaling factor.
                            parameter.SetValuePercentOfRange(SliderToPercentValue((int)slider.Value));
                            long val = parameter.GetValue();
                            valueLabel.Text= val.ToString();
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Ignore any errors here.
                    Logger.Error(ex.Message);
                }
                finally
                {
                    sliderMoving = false;
                }
            }
        }
    }
}
