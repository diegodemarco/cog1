# cog1

## What is cog1?
cog1 is an open-source industrial IoT gateway (hardware and software) built as a motherboard hosting an [Orange Pi Zero 3](http://www.orangepi.org/html/hardWare/computerAndMicrocontrollers/details/Orange-Pi-Zero-3.html) linux SBC (Wiki [here](http://www.orangepi.org/orangepiwiki/index.php/Orange_Pi_Zero_3). The motherboard was designed using [KiCad](https://www.kicad.org/). Its software uses the [.net core framework](https://dotnet.microsoft.com/) (currently at version 8) to provide the services running in the hardware, as well as the back-end web services for the user interface (not started yet), which will be developed in [Angular](https://angular.dev/).

### Features

#### Hardware
The cog1 platform currently supports:
- Powered by any DC supply from 12 to 24 VDC.
- Four digital inputs (isolated, using two [ISO 1212](https://www.ti.com/lit/ds/symlink/iso1212.pdf) chips from TI).
- Four digital outputs (relays, varistor-protected).
- Four analog voltage inputs (0-10 V, non-isolated), protected up to 48 VDC. The ADC is an [ADC128D818](https://www.ti.com/lit/ds/symlink/adc128d818.pdf) chip from TI.
- Four analog current inputs (0-20 mA, non isolated), protected up to 100 mA. Uses the same [ADC128D818](https://www.ti.com/lit/ds/symlink/adc128d818.pdf) chip as the voltage inputs.
- One RS-485 port (tested from 9600 to 115200 bauds). Support for Modbus RTU will be built on top of this.
- Battery-backed RTC.
- A simple but extensible user interface with a small OLED display and a rotary encoder + pushbutton.
- All connectivity inherited from the Orange Pi Zero 3 SBC
  - 1 Gb Ethernet
  - 2.4 + 5.8 GHz WiFi
  - Bluetooth 5
  - A front USB port (USB 2.0)

#### Software
The current software is extremely basic, but includes everything needed to fully test the hardware. Additionally, the software already has the correct "architecture", i.e. it is designed as a linux service file that can be started, stopped, etc., and communicates with the hardware through a low-level i/o library created with native code (plain old C).

The plan is to continue developing the software so that it can:
- Communicate with a user interface (to be developed in Angular) to allow configuration and monitoring.
- Enable the definition of "variables" based on the various inputs and outputs as well as on Modbus RTU (through the built-in RS-485 port). This could easily be extended to use Modbus TCP as well, or any other LAN protocol that could be relevant.
- Provide data-extraction web services.
- Communicate with external cloud services via MQTT or any other protocol that could be relevant.
- Allow running javascript code using [Jint](https://github.com/sebastienros/jint) to make it possible to compute complex variables, do edge data-processing, etc.

### Hardware
#### PCB
The PCB has been designed using KiCad. It contains everything needed, including a 3D render. All components can be hand-soldered, but a few (essentially the AOZ regulator, the ADC, and the digital input isolators) require a bit of skills, as they are SSOP components. 

All capacitors, resistors, etc. are 0805 or bigger, so they can be hand-soldered with little effort. The PCB contains all necessary headers to allow the Orange Pi Zero 3 to be plugged in, inverted.

All components can be found in the usual marketplaces (DigiKey, Mouser, etc.). I strongly advice not using cheap / uncertified sources for the components, especially the chips, as there is a large market of fake chips that either don't work, or even worse, work erratically.

#### Enclosure
The enclosure for the cog1 is a rather standard extruded-aluminum case, which can be found online for a few bucks. However, as a 3D printing enthusiast, I ended up designing and printing everything necessary to complete the enclosure:
- Front cover.
- Back cover
- Knob
All designs are included in this project. I have printed everything with an Elegoo Mars 3 Pro (resin), and expect that all parts would print well on any resin 3D printer. I have not tried to print this on a filament printer, as I believe the quality would not be good enough. When printed in resin, all parts really look production-grade if the proper resin is used.

### Software
#### Main code
The main code of the cog1 platform is a .net core application that contains all the necessary components (web services and general logic) to keep the cog1 working. The current version is relatively basic, and only contains those features needed to test the hardware. A web service is in place to provide basic status of the cog1, which is currently used to test the overall reliability of the hardware. All hardware functions have been tested with this software.

This is by far the area that needs the most effort, so that the cog1 can be turned into a fully-functional gateway.

#### Native code
There are a few small portions of the software written in plan old C, and compiled directly in the SBC:
- A low-level i/o library that deals with digital and analog i/o, display, etc.
- A small "logo" application (configured as a service that starts early in the Linux boot process) to display the cog1 logo while the cog1 is booting.

Everything else is c#, so there's no low-level really in the c# code. 

## Getting started
### Building the hardware
### Installing the software
## Current status
### What's done
### What's missing
