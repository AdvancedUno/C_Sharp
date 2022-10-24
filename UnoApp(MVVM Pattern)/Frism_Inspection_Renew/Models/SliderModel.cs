using Basler.Pylon;
using NLog;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;
using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace Frism_Inspection_Renew.Models
{
    public class SliderModel : INotifyPropertyChanged
    {

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private IParameter parameter = null;
        private bool sliderMoving = false;
        private bool logarithmic = false;

        private bool checkExposureTime = false;

        private TextBox _valueLabel;
        public TextBox ValueLabel
        {
            get { return _valueLabel; }
            set
            {
                _valueLabel = value;
                OnPropertyChanged("ValueLabel");
            }
        }

        private double _min = 0;
        public double Min { get => _min; set => _min = value; }

        

        private double _max = 0;
        public double Max { get => _max; set => _max = value; }


        private double _val = 0;
        public double Val { get => _val; set => _val = value; }

        private double _percent = 0;
        public double Percent { get => _percent; set => _percent = value; }

        private readonly int SLIDER_CONSTANT_FACTOR = 10000;
        private double _sliderMinimum = 0;
        public double SliderMinimum {
            get => _sliderMinimum;
            set
            {
                _sliderMinimum = value;
                OnPropertyChanged("SliderMinimum");
            }
        }

        private double _sliderMaximum = 100;
        public double SliderMaximum { 
            get => _sliderMaximum;
            set { 
                _sliderMaximum = value;
                OnPropertyChanged("SliderMaximum");
            }
        }

        private double _sliderValue = 0;
        public double SliderValue { 
            get => _sliderValue;
            set {
                _sliderValue = value;
                slider_ValueChanged();
                OnPropertyChanged("SliderValue");
            }
        }

        private double _sliderTickFrequency;
        public double SliderTickFrequency { 
            get => _sliderTickFrequency; 
            set {
                _sliderTickFrequency = value;
                OnPropertyChanged("SliderTickFrequency");
            }
        }

        private double _smallChange;

        public SliderModel(bool checkExposureTime)
        {
            ValueLabel = new TextBox();
            ValueLabel.Text = "0";
            this.checkExposureTime = checkExposureTime;
            //slider_ValueChanged();
        }

        public double SmallChange { 
            get => _smallChange;
            set {
                _smallChange = value;
                OnPropertyChanged("SmallChange");
            }
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
                result = (int)(((Max - Min) / 100.0) * percent);
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
                result = (((double)sliderValue) / (Max - Min)) * 100.0;
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
            ValueLabel.Text = "0";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                            Min = floatParameter.GetMinimum();
                            Max = floatParameter.GetMaximum();
                            Val = floatParameter.GetValue();
                            Percent = floatParameter.GetValuePercentOfRange();
                            SmallChange = 1;
                        }
                        else
                        {
                            IIntegerParameter intParameter = this.parameter as IIntegerParameter;
                            // Break any recursion if the value does not exactly match the slider value.
                            sliderMoving = true;
                            // Get the values.
                            Min = intParameter.GetMinimum();
                            Max = intParameter.GetMaximum();
                            Val = intParameter.GetValue();
                            Percent = intParameter.GetValuePercentOfRange();
                            // Configure the SmallChange property of the parameter increment value to prevent invalid values.
                            SmallChange = (int)intParameter.GetIncrement();
                        }
                        // Update the slider. Scale values by scaling factor.
                        SliderMinimum = PercentToSliderValue(0);
                        SliderMaximum = PercentToSliderValue(100);
                        SliderValue = PercentToSliderValue(Percent);
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
                sliderMoving = false;

            }
        }


        public void BindParametersToControls(IVisionCamera camera)
        {
            IParameterCollection parameters = camera.GetParameters();
            if (parameters != null)
            {
                try
                {
                    if (!checkExposureTime)
                    {
                        Console.WriteLine(PLCamera.Gain);
                        if (parameters.Contains(PLCamera.Gain))
                        {
                            Parameter = parameters[PLCamera.Gain];
                        }
                        else
                        {

                            Parameter = parameters[PLCamera.GainRaw];
                        }
                    }
                    else
                    {
                        if (parameters.Contains(PLCamera.ExposureTimeAbs))
                        {
                            Logarithmic = true;
                            Parameter = parameters[PLCamera.ExposureTimeAbs];
                        }
                        else
                        {
                            Logarithmic = true;
                            Parameter = parameters[PLCamera.ExposureTime];
                        }
                    }
                    
                    
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message + " BindParametersToControls_SelectCamWindow");
                    Console.WriteLine(ex.Message + " BindParametersToControls_SelectCamWindow");
                    //Helper.ShowException(e);
                }
            }
        }

        public void slider_ValueChanged()
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
                            parameter.SetValuePercentOfRange(SliderToPercentValue((int)SliderValue));
                            double val = parameter.GetValue();
                            ValueLabel.Text = string.Format("{0:0}", val);

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
                            parameter.SetValuePercentOfRange(SliderToPercentValue((int)SliderValue));
                            long val = parameter.GetValue();
                            ValueLabel.Text = val.ToString();

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
