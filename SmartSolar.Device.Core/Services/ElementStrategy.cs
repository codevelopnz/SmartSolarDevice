﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartSolar.Device.Core.Domain;

namespace SmartSolar.Device.Core.Services
{

	/// <summary>
	/// Single responsibility: hold all the factors that influence whether the element should be on or off.
	/// These could be just parameters to the ElementStrategy below - but then you end up with a lot of meaningless
	/// boolean parameters, so better to have a class encapsulating them.
	/// </summary>
	public class ElementStrategyParams
	{
		public bool IsElementOn { get; set; }
		public bool IsBoosting { get; set; }
		public int InletTemperature { get; set; }
		
	}
	/// <summary>
	/// Single responsibility: to make decisions about whether the element should currently be on or off.
	/// </summary>
	public class ElementStrategy
	{
		// should we be switching on the element (using electricity) at this point?
		// a big assumption with putting this logic here is that both the pump and element are 
		// allowed to be on at the same time.  My thinking is that this should be possible in the
		// case of a frost condition or even when 'boosting' and solar is hot and ready to go.
		public bool ShouldElementBeOn(ElementStrategyParams @params, Settings settings)
		{
			if (@params.IsElementOn)
			{
				// Once it's on, it stays on - this can't be right, but it's my reading of the original code
				return true;
			}

			// Not currently on - turn it on if the inlet temp is less than our target
			// (This makes it currently a "dumb" controller)
			return @params.InletTemperature <= settings.ElectricityTarget;

			// Commented code from original MainPage - this is the "smarts" we'll need to implement.
//			else
//			{
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
//			}
		}
			}
		}
