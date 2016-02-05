using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Devices.Spi;
using Windows.Devices.Gpio;
using Windows.Devices.Enumeration;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IntelligentSolar_v1
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private bool isElementOn = false;
        private bool isPumpOn = false;
        private bool isBoosting = false;
        private bool isFrost = false;

        private bool boost = false;
        private bool priority = false;
        private bool away = false;

        private int pumpOnTemperature = 5; // these are normally higher....but for testing with just body temp on the thermisters
        private int pumpOffTemperature = 2; // these are normally higher....but for testing with just body temp on the thermisters

        private int electricityTarget = 55;
        private int solarTarget = 70;

        private int holdOffTemperature = 40;
        private int holdOffMinutes = 480;

        private int frostTemperature = 4;

        private int roofTemperature = 0;
        private int inletTemperature = 0;
        private int tankTemperature = 0;

        private int oldInletTemp = 999;

        public MainPage()
        {
            this.InitializeComponent();

//            InitSPI();
//            InitGpio();

            InitSettings();

            DispatcherTimer timer = new DispatcherTimer();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += Timer_Tick;
            timer.Start();

        }

        private void InitSettings()
        {
            // override default settings with stored ones if they exist (or store the defaults)

        }


        private async void InitSPI()
        {
            try
            {
                var settings = new SpiConnectionSettings(SPI_CHIP_SELECT_LINE);
                settings.ClockFrequency = 500000;// 10000000;
                settings.Mode = SpiMode.Mode0; //Mode3;

                string spiAqs = SpiDevice.GetDeviceSelector(SPI_CONTROLLER_NAME);
                var deviceInfo = await DeviceInformation.FindAllAsync(spiAqs);
                SpiDisplay = await SpiDevice.FromIdAsync(deviceInfo[0].Id, settings);
            }

            /* If initialization fails, display the exception and stop running */
            catch (Exception ex)
            {
                throw new Exception("SPI Initialization Failed", ex);
            }
        }

        private void InitGpio()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller  
            if (gpio == null)
            {
                pinElement = null;
                pinPump = null;
                pinElementLED = null;
                pinPumpLED = null;
                GpioStatus.Text = "NONE";
                return;
            }

            pinElement = gpio.OpenPin(Element_PIN);
            pinPump = gpio.OpenPin(Pump_PIN);
            pinElementLED = gpio.OpenPin(ElementLED_PIN);


            // Show an error if the pin wasn't initialized properly  
            if (pinElement == null || pinPump == null || pinElementLED == null) // || pinPumpLED == null)
            {
                GpioStatus.Text = "Error";
                return;
            }

            //init each 
            pinElement.SetDriveMode(GpioPinDriveMode.Output);
            pinElement.Write(GpioPinValue.High);
            pinElement.Write(GpioPinValue.Low);
            pinElement.Write(GpioPinValue.High);

            pinPump.SetDriveMode(GpioPinDriveMode.Output);
            pinPump.Write(GpioPinValue.High);
            pinPump.Write(GpioPinValue.Low);
            pinPump.Write(GpioPinValue.High);


            pinElementLED.SetDriveMode(GpioPinDriveMode.Output);
            pinElementLED.Write(GpioPinValue.High);
            pinElementLED.Write(GpioPinValue.Low);
            pinElementLED.Write(GpioPinValue.High);

            pinPumpLED.SetDriveMode(GpioPinDriveMode.Output);
            pinPumpLED.Write(GpioPinValue.High);
            pinPumpLED.Write(GpioPinValue.Low);
            pinPumpLED.Write(GpioPinValue.High);

            GpioStatus.Text = "Success";
        }


        private void Timer_Tick(object sender, object e)
        {
//            DisplayTextBoxContents();
        }


        private void DisplayTextBoxContents()
        {
 
            writeBuffer[0] = 0x06; // set to channel 0 

            roofTemperature = readTemperature(0x00, TemperatureRoof);
            tankTemperature = readTemperature(0x40, TemperatureTank);
            inletTemperature = readTemperature(0x40, TemperatureTankInlet);


            // if element is on, should we turn it off?
            if (isElementOn && inletTemperature >= electricityTarget)
            {
                //"turn element off"
                pinElement.Write(GpioPinValue.High);
                pinElementLED.Write(GpioPinValue.High);
                LEDStatus.Fill = grayBrush; 
                isElementOn = false;
                boost = false;
                isBoosting = false;
                //log
            }

            // if the pump is on, should we turn if off?
            if (isPumpOn)
            {
                // if currently under frost conditions, only switch off if 2(?) degress higher than
                // frost min
                if (isFrost)
                {
                    if (inletTemperature - frostTemperature >= 2)
                    {
                        //"turn pump off"
                        pinPump.Write(GpioPinValue.High);
                        pinPumpLED.Write(GpioPinValue.High);
                        LEDStatus.Fill = grayBrush;
                        isPumpOn = false;
                        isFrost = false;
                        //log
                    }
                }
                else
                {
                    // if solar target hit or pump off condition met, switch off pump
                    if (inletTemperature >= solarTarget ||
                        roofTemperature - inletTemperature <= pumpOffTemperature)
                    {
                        //"turn pump off"
                        pinPump.Write(GpioPinValue.High);
                        pinPumpLED.Write(GpioPinValue.High);
                        LEDStatus.Fill = grayBrush;
                        isPumpOn = false;
                        boost = false;
                        isBoosting = false;
                        //log
                    }
                }
            }

            // should we turn on the pump?
            // do we have a frost condition
            if (!isFrost && roofTemperature <= frostTemperature)
            {
                //"turn pump on"
                pinPump.Write(GpioPinValue.Low);
                pinPumpLED.Write(GpioPinValue.Low);
                LEDStatus.Fill = greenBrush;
                isPumpOn = true;
                isFrost = true;
                //log
            }
            else
            {
                // if less than solar target and pump on condition met, turn on pump
                // note that we only start pumping if the tank temperature is 2 degress less than the
                // solar target, otherwise the pump cycle is too short before we topout and stop the
                // pump
                if (inletTemperature <= solarTarget - 2 &&
                     roofTemperature - inletTemperature >= pumpOnTemperature)
                {
                    //"turn pump on"
                    pinPump.Write(GpioPinValue.Low);
                    pinPumpLED.Write(GpioPinValue.Low);
                    LEDStatus.Fill = greenBrush;
                    isPumpOn = true;
                    //log
                }
            }

            // should we be switching on the element (using electricity) at this point?
            // a big assumption with putting this logic here is that both the pump and element are 
            // allowed to be on at the same time.  My thinking is that this should be possible in the
            // case of a frost condition or even when 'boosting' and solar is hot and ready to go.
            if (!isElementOn && inletTemperature <= electricityTarget)
            {
                // first, if user has set to boost (get to target temperature NOW!) and not at target
                if (boost && !isBoosting)
                {
                    //"turn element on"
                    pinElement.Write(GpioPinValue.Low);
                    pinElementLED.Write(GpioPinValue.Low);
                    LEDStatus.Fill = redBrush;
                    isElementOn = true;
                    isBoosting = true;
                    //log
                }
                else
                {
                    //// if the user is not away and hasn't set up any usage patterns or they have set to
                    //// priority h/w, then operate the same as the current controller.
                    //if (!away && (usagePatterns.Count == 0 || priority))
                    //{
                    //    // only switch on the element if the inlet temp is less than the electricityTemp
                    //    // and we have exceeded the "hold off minutes" to wait to see if the tank heats 
                    //    // up in time via solar
                    //    // OR
                    //    // switch on if inlet temp is less than the lowerReheatTemperature
                    //    if (
                    //        (inletTemperature <= electricityTarget &&
                    //         holdOffTimer >= holdOffMinutes
                    //        ) ||
                    //        inletTemperature <= holdOffTemperature)
                    //    {
                    //        //"turn element on"
                    //        pinPower.Write(GpioPinValue.Low);
                    //        LEDStatus.Fill = redBrush;
                    //        elementOn = true;
                    //        //log
                    //    }
                    //}
                    //else
                    //{
                    //    // OK....this is the real smart stuff here.  Look at the usagePattern data (and 
                    //    // returning back from holiday data) as entered by the user and find the upcoming
                    //    // day/time of interest.  Using this target day/time, calculate the time it will
                    //    // take to heat the cylinder to the electricityTemperature from now.  If that 
                    //    // duration is >= time difference between now and the target time, then its time
                    //    // to switch on the element!!
                    //    set targetDayTime = min(usagePatterns && returnDateTime)
            
                    //    // OK, now some fancy formula to calculate heating duration
                    //    set heatingDuration = heatingDuration(inletTemperature,
                    //                    electricityTemperature,
                    //                    elementWatts,
                    //                    cyclinderLitres)
            
                    //    if (Now.AddTime(heatingDuration) >= targetDayTime)
                    //                {
                    //                    "turn element on"
                    //        set elementOn to true
                    //        log
                    //    }
                    //}
                }
            }

            // determine the status of the holdOffTimer
            if (inletTemperature < electricityTarget && oldInletTemp >= electricityTarget)
            {
                // start the holdOffTimer
            }
            else
            {
                if (inletTemperature >= electricityTarget &&
                    oldInletTemp < electricityTarget)
                {
                    // stop the holdOffTimer
                }
            }

            // hold a copy of the current inlet temp so we can detect when it crosses the 
            // electricityTemp threshold.
            oldInletTemp = inletTemperature;

        }

        private int readTemperature(byte buffer, TextBox tb)
        {
            writeBuffer[1] = buffer; // set to channel 0
//            SpiDisplay.TransferFullDuplex(writeBuffer, readBuffer);
            int res = convertToInt(readBuffer);
            double temp = convertToCelcuis(res);
            var ohms = ((5.0 / (Convert.ToDouble(res) * (5.0 / 4095.0))) - 1.0) * 10000.0;
            tb.Text = String.Format("{0:0} {1:0}°C", ohms, temp);
            return Convert.ToInt32(temp);
        }

        private double convertToCelcuis(int res)
        {
            if (res > 0)
            {
                //var volts = Convert.ToDecimal(res) * (5M / 4095.0M);
                ////var ohms = ((1.0M / volts) * 5000M) + 10000M;
                ////var ohms = volts / 0.0001M;
                //var ohms = ((5M / volts) - 1M) * 10000M;
                //var lnohm = Math.Log(Convert.ToDouble(ohms), Math.E);

                //// a, b, & c values from http://www.thermistor.com/calculators.php
                //// using curve R (-6.2%/C @ 25C) Mil Ratio X
                //var a = 0.001821776240470;
                //var b = 0.000159566897583;
                //var c = 0.000000080342408;

                //// Steinhart Hart Equation
                //// T = 1/(a + b[ln(ohm)] + c[ln(ohm)]^3)

                //var t1 = (b * lnohm); // b[ln(ohm)]
                //var c2 = c * lnohm; // c[ln(ohm)]
                //var t2 = Math.Pow(c2, 3); // c[ln(ohm)]^3
                //var temp = 1 / (a + t1 + t2); //calcualte temperature
                //var tempc = temp - 273.15 - 4; // K to C
                //return tempc;

                double thermisterNominal = 10000;
                double temperatureNominal = 25;
                double bCoefficient = 3369;
                double steinhart;

                var volts = Convert.ToDouble(res) * (3.3 / 4095.0);
                var ohms = ((3.3 / volts) - 1.0) * 10000.0;
                
                steinhart = ohms / thermisterNominal;     // (R/Ro)
                steinhart = Math.Log(steinhart);                  // ln(R/Ro)
                steinhart /= bCoefficient;                   // 1/B * ln(R/Ro)
                steinhart += 1.0 / (temperatureNominal + 273.15); // + (1/To)
                steinhart = 1.0 / steinhart;                 // Invert
                steinhart -= 273.15;                         // convert to C

                return steinhart;
            }
            else
            {
                return 0;
            }
        }

        /* This is the conversion for MCP3208 which is a 12 bits output; Uncomment this if you are using MCP3208 */
        public int convertToInt(byte[] data)
        {
            int result = data[1] & 0x0F;
            result <<= 8;
            result += data[2];
            return result;
        }


        /* This is the conversion for MCP3002 which is a 10 bits output; Uncomment this if you are using MCP3002 */
        //public int convertToInt(byte[] data)
        //{
        //    int result = data[0] & 0x03;
        //    result <<= 8;
        //    result += data[1];
        //    return result;
        //}

        private GpioPin pinPumpLED;
        private GpioPin pinElementLED;
        private GpioPin pinPump;
        private GpioPin pinElement;

        private const int Pump_PIN = 22;
        private const int Element_PIN = 27;
        private const int PumpLED_PIN = 17;
        private const int ElementLED_PIN = 18;

        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);
        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);

        /*RaspBerry Pi2  Parameters*/
        private const string SPI_CONTROLLER_NAME = "SPI0";  /* For Raspberry Pi 2, use SPI0                             */
        private const Int32 SPI_CHIP_SELECT_LINE = 0;       /* Line 0 maps to physical pin number 24 on the Rpi2        */

        /*Channel configuration for MCP3208, Uncomment this if you are using MCP3208*/

        byte[] readBuffer = new byte[3]; /*this is defined to hold the output data*/
        byte[] writeBuffer = new byte[3] { 0x06, 0x00, 0x00 };//00000110 00; /* It is SPI port serial input pin, and is used to load channel configuration data into the device*/

        /*Channel configuration for MCP3002, Uncomment this if you are using MCP3002*/
        //byte[] readBuffer = new byte[3]; /*this is defined to hold the output data*/
        //byte[] writeBuffer = new byte[3] { 0x68, 0x00, 0x00 };//00001101 00; /* It is SPI port serial input pin, and is used to load channel configuration data into the device*/

        private SpiDevice SpiDisplay;

        // create a timer
        private DispatcherTimer timer;

    }
}
