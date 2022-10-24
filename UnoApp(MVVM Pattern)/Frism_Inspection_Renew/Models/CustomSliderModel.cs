using System;
using System.Collections.Generic;
using System.ComponentModel;
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


namespace Frism_Inspection_Renew.Models
{
    public class CustomSliderModel: INotifyPropertyChanged
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        private IParameter _parameter = null;
        public IParameter Parameter
        {
            get { return _parameter; }
            set
            {
                // Unsubscribe from the previous parameter.
                if (_parameter != null)
                {
                    _parameter.ParameterChanged -= ParameterChanged;
                }

                // Set the new parameter and subscribe to it.
                _parameter = value;
                if (_parameter != null)
                {
                    _parameter.ParameterChanged += ParameterChanged;
                    UpdateValues();
                }
                else
                {
                    Reset();
                }
            }
        }

        private bool _sliderMoving = false;
        public bool SliderMoving { 
            get => _sliderMoving;
            set
            {
                _sliderMoving = value;
                OnPropertyChanged("SliderMoving");
            } 
        }

        private bool _logarithmic = false;
        public bool Logarithmic
        {
            get { return _logarithmic; }
            set { _logarithmic = value; }
        }

        private TextBox _valueLabel = null;
        public TextBox ValueLabel { 
            get => _valueLabel;
            set
            {
                _valueLabel = value;
                OnPropertyChanged("SliderValueLabelMoving");
            }
        }

        private double _minVal = 0;
        public double MinVal { get => _minVal; set => _minVal = value; }

        private double _maxVal = 0;
        public double MaxVal { get => _maxVal; set => _maxVal = value; }

        private double _sliderVal = 0;
        public double SliderVal { get => _sliderVal; set => _sliderVal = value; }
        
        private double _percentVal = 0;
        public double PercentVal { get => _percentVal; set => _percentVal = value; }

        private double _smallChangeVal;
        public double SmallChangeVal { get => _smallChangeVal; set => _smallChangeVal = value; }
        
        private double _sliderValue;
        public double SliderValue { 
            get => _sliderValue;
            set
            {
                SliderValue = value;
                OnPropertyChanged("SliderValue");
            }
        }
        
        private double _sliderMaximum;
        public double SliderMaximum { get => _sliderMaximum; set => _sliderMaximum = value; }
        

        private double _sliderMinimum;
        public double SliderMinimum { get => _sliderMinimum; set => _sliderMinimum = value; }
        

        private double _sliderTickFrequency;
        public double SliderTickFrequency { get => _sliderTickFrequency; set => _sliderTickFrequency = value; }



        private readonly int SLIDER_CONSTANT_FACTOR = 10000;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int PercentToSliderValue(double percent)
        {
            int result = 0;
            try
            {
                
                if (Logarithmic)
                {
                    result = (int)(Math.Log(percent + 1, 2) * SLIDER_CONSTANT_FACTOR);
                }
                else
                {
                    result = (int)(((MaxVal - MinVal) / 100.0) * percent);
                }
                
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " PercentToSliderValue");
            }
            return result;

        }
        private double SliderToPercentValue(int sliderValue)
        {
            double result;
            if (Logarithmic)
            {
                // Due to rounding in PercentToSliderValue, we can't reach the maximum.
                // If the slider maximum has been reached, return 100 %.
                if (SliderMaximum == sliderValue)
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
                result = (((double)sliderValue) / (MaxVal - MinVal)) * 100.0;
            }
            return result;
        }

        public void SetLabel(TextBox label)
        {
            ValueLabel = label;
            ValueLabel.Text = "0";
        }

        private void Reset()
        {
            //slider.IsEnabled = false;
            //valueLabel.Text = "0";
        }

        private void ParameterChanged(Object sender, EventArgs e)
        {

            //if (!Dispatcher.CheckAccess())
            //{
            //    Dispatcher.BeginInvoke(new EventHandler<EventArgs>(ParameterChanged), sender, e);
            //    return;
            //}

            try
            {
                // Update accessibility and parameter values.
                UpdateValues();
            }
            catch (Exception ex)
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
                if (Parameter != null && Parameter.IsReadable)
                {
                    if (Parameter.IsWritable && !SliderMoving)
                    {
                        if (this.Parameter is IFloatParameter)
                        {
                            IFloatParameter floatParameter = this.Parameter as IFloatParameter;
                            // Break any recursion if the value does not exactly match the slider value.
                            SliderMoving = true;
                            // Get the values.
                            MinVal = floatParameter.GetMinimum();
                            MaxVal = floatParameter.GetMaximum();
                            SliderVal = floatParameter.GetValue();
                            PercentVal = floatParameter.GetValuePercentOfRange();
                            SmallChangeVal = 1;
                        }
                        else
                        {
                            IIntegerParameter intParameter = this.Parameter as IIntegerParameter;
                            // Break any recursion if the value does not exactly match the slider value.
                            SliderMoving = true;
                            // Get the values.
                            MinVal = intParameter.GetMinimum();
                            MaxVal = intParameter.GetMaximum();
                            SliderVal = intParameter.GetValue();
                            PercentVal = intParameter.GetValuePercentOfRange();
                            // Configure the SmallChange property of the parameter increment value to prevent invalid values.
                            SmallChangeVal = (int)intParameter.GetIncrement();
                        }
                        // Update the slider. Scale values by scaling factor.
                        SliderMinimum = PercentToSliderValue(0);
                        SliderMaximum = PercentToSliderValue(100);
                        SliderValue = PercentToSliderValue(PercentVal);
                        SliderTickFrequency = (SliderMaximum - SliderMinimum) / 10;

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
            catch (Exception ex)
            {
                // If errors occurred, disable the control.
                //Reset();
                Logger.Error(ex.Message);
            }
            finally
            {
                SliderMoving = false;

            }
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.Parameter != null)
            {
                try
                {
                    if (this.Parameter is IFloatParameter)
                    {
                        IFloatParameter parameter = this.Parameter as IFloatParameter;
                        if (parameter.IsWritable && !SliderMoving)
                        {
                            // Break any recursion if the value does not exactly match the slider value.
                            SliderMoving = true;

                            // Set the value. Scale by scaling factor.
                            parameter.SetValuePercentOfRange(SliderToPercentValue((int)SliderValue));
                            double val = parameter.GetValue();
                            ValueLabel.Text = string.Format("{0:0}", val);

                        }
                    }
                    else
                    {
                        IIntegerParameter parameter = this.Parameter as IIntegerParameter;
                        if (parameter.IsWritable && !SliderMoving)
                        {
                            // Break any recursion if the value does not exactly match the slider value.
                            SliderMoving = true;

                            // Set the value. Scale by scaling factor.
                            parameter.SetValuePercentOfRange(SliderToPercentValue((int)SliderValue));
                            long val = parameter.GetValue();
                            ValueLabel.Text = val.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Ignore any errors here.
                    Console.WriteLine(ex.Message + " slider_ValueChanged");
                    Logger.Error(ex.Message);
                }
                finally
                {
                    SliderMoving = false;
                }
            }
        }
    }


}

