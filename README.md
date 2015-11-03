# SmartSolarDevice
A project for the CoDevelop/ team

## Function of the Hardware
This hardware has the have the ability to:
* determine the temperature of the water on the roof (solar panels)
* determine the tempurature of the hot water cylinder at both the inlet and outlet (top and bottom)
* switch on the water pump to cycle hot water from the roof panels to the cylinder
* switch on the power to the hot water cylinder element (*not* directly power the element)
* have onboard visual feedback when the pump or element is on
* allow for direct access to basic functions of the device

## Hardware
OK, the following is a list of hardware I'm using (so far).
* [Raspberry Pi 2](https://www.raspberrypi.org/products/raspberry-pi-2-model-b/)
* [2 Module Relay](http://www.surplustronics.co.nz/products/7145-relay-2-channel-module-expansion-board-5v-low-level-triggered)
* Analogue to Digital Converter [MCP3008](http://nz.element14.com/microchip/mcp3008-i-p/analog-to-digital-converter-adc/dp/1627174) or [MCP3208](http://nz.element14.com/microchip/mcp3208-ci-p/ic-adc-12-bit-100ksps-spi-dip/dp/1084269)
* [LEDs Red](http://www.surplustronics.co.nz/products/2785-led-30mm-normal-red) and [Green](http://www.surplustronics.co.nz/products/2787-led-30mm-normal-green)
* [2 x 120 ohm Resistors](http://www.surplustronics.co.nz/products/1586-120-ohm-resistor-pk-10) for the above LEDs
* Power cable (either make one or snip the cord off an old appliance/computer lead)
* [Terminal Block](http://www.surplustronics.co.nz/products/7288-terminal-block-white) for prototyping.  May need something better for final solution.

Optionally, If you want the whole unit self contained with only one mains power input (as opposed to one mains and one 5V DC @ 2amps input), you'll need:
* [230v Transformer to 5v DC @ 2 amps](http://www.prodctodc.com/dc-switching-adapter-ac-90240v-to-5v-2a-10w-step-down-voltage-regulator-led-power-module-p-233.html#.VjhClPkrK70)
* [Micro USB male connector](https://www.adafruit.com/products/1390)


If you don't have a solar system to hook this up to (or want to have a working prototype without installing it....re: test it first!) you'll also need:
* [3 x 10K Thermistors](http://www.surplustronics.co.nz/products/2140-ntc-thermistor-10k-ohms) 
* [3 x 10K ohm Resistors](http://www.surplustronics.co.nz/products/1629-10k-ohm-resistor-pk-10) for the above Theristors
* Something powered by mains to prove the water pump and cylinder element relays are working. Eg, 2 x 230v lights etc

You'll also need a breadboard and appropriate wires.

Ultimately, I want to make this as compact and robust as possible without going to the extent of get a custom PCB manufactured (yet!), so I've also gone one better than a breadboard (now I have the breadboard based prototype working) and got a [PermaProto HAT from Adafruit](http://www.adafruit.com/products/2310) that sits nicely on top of the Pi and an [extra-long GPIO Stacking Header](https://www.adafruit.com/products/2223) for the proto board just in case I want to mount a display [like this](http://www.adafruit.com/products/1115) one on top.

## Basic Prototype Layout
![alt tag](http://www.webconception.co.nz/media/smartsolar/breadboard.png "Early breadboard design")

Coming soon...photos of the early breadboard prototype, my current prototype and soon the permaproto all soldered up.