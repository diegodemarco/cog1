# cog1
![cog1 front and back as built](docs/images/picture_1.jpg)

## What is cog1?
cog1 is an open-source industrial IoT gateway (hardware and software) built as a motherboard hosting an [Orange Pi Zero 3](http://www.orangepi.org/html/hardWare/computerAndMicrocontrollers/details/Orange-Pi-Zero-3.html) linux SBC (Wiki [here](http://www.orangepi.org/orangepiwiki/index.php/Orange_Pi_Zero_3)).
- The motherboard was designed using [KiCad](https://www.kicad.org/).
- Its software uses the [.net core framework](https://dotnet.microsoft.com/) (currently at version 8) to provide the services running in the hardware, as well as the back-end web services for the UI.
- It also hosts the user interface (console), which is written  in [Angular](https://angular.dev/), based on the [Core-UI admin template](https://coreui.io/product/free-angular-admin-template/).
- In essence, it's a fully self-contained IoT gateway capable of interfacing with sensors and actuators directly, designed to process data, make local decisions, and communicate with external cloud services if needed.

### Features

#### Hardware
The cog1 platform currently supports:
- Powered by any DC supply from 12 to 24 VDC.
- Four digital inputs (isolated, using two [ISO 1212](https://www.ti.com/lit/ds/symlink/iso1212.pdf) chips from TI).
- Four digital outputs (relays, varistor-protected).
- Four analog voltage inputs (0-10 V, non-isolated), protected up to 48 VDC. The ADC is an [ADC128D818](https://www.ti.com/lit/ds/symlink/adc128d818.pdf) chip from TI.
- Four analog current inputs (0-20 mA, non isolated), protected up to 100 mA. Uses the same [ADC128D818](https://www.ti.com/lit/ds/symlink/adc128d818.pdf) chip as the voltage inputs.
- One RS-485 port (tested from 9600 to 115200 bauds). Support for Modbus RTU is already built on top of this.
- Battery-backed RTC.
- A simple but extensible user interface with a small OLED display and a rotary encoder + pushbutton.
- All connectivity inherited from the Orange Pi Zero 3 SBC
  - 1 Gb Ethernet
  - 2.4 + 5.8 GHz WiFi
  - Bluetooth 5
  - A front USB port (USB 2.0)

#### Software
![cog1 admin console](docs/images/console-1.png)
The current software is rather basic, but includes everything needed to fully test the hardware. Additionally, the software already has the correct "architecture", i.e. it is designed as a linux service file that can be started, stopped, etc., and communicates with the hardware through a low-level i/o library created with native code (plain old C).

The front-end (console) is written in Angular, based on the [Core-UI admin template](https://coreui.io/product/free-angular-admin-template/), and it communicates with the back-end via web-services, which are already integrated in such a way that creating new controllers/endpoints and consuming them from the front-end is really simple. 

The following features are already implemented:
- Internal variables are fully supported. These match the actual hardware of the cog1.
- External variables can be defined as well, based on Modbus registers.
- Both Modbus RTU and Modbus TCP are supported, allowing reading and writing the most common types of registers in a variety of data types.
- Variables can be defined to be "polled" automatically. For variables bound to Modbus registers, this means that the respective registers are polled via RTU or TCP as needed.
- Writeable variables can be controlled/modified from the dashboard.
- Writeable variables bound to Modbus registers also trigger the respective write operations through RTU or TCP, which can be done from the dashboard.

The plan is to continue developing the software so that it can:
- Provide data-extraction web services.
- Communicate with external cloud services via MQTT or any other protocol that could be relevant.
- Allow running javascript code using [Jint](https://github.com/sebastienros/jint) to make it possible to compute complex variables, do edge data-processing, etc.
- Update the console UI to allow further management of all avaible functionality, as it grows over time.

### Hardware
#### PCB
![cog1 PCB as rendered by KiCad](docs/images/picture_2.jpg)

The PCB has been designed using KiCad. It contains everything needed, including a 3D render. All components can be hand-soldered, but a few (essentially the AOZ regulator, the ADC, and the digital input isolators) require a bit of skills, as they are SSOP components. 

All capacitors, resistors, etc. are 0805 or bigger, so they can be hand-soldered with little effort. The PCB contains all necessary headers to allow the Orange Pi Zero 3 to be plugged in, inverted.

All components can be found in the usual marketplaces (DigiKey, Mouser, etc.). I strongly advice not using unreliable (i.e. unrealistically cheap) sources for the components, especially the chips, as there is a large market of fake chips that either don't work, or even worse, work erratically.

#### Enclosure
The enclosure for the cog1 is a rather standard extruded-aluminum case, which can be found online for a few bucks. However, as a 3D printing enthusiast, I ended up designing and printing everything necessary to complete the enclosure:
- Front cover
- Back cover
- Knob

All designs, done in SolidWorks, are included in this project. I have printed everything with an Elegoo Mars 3 Pro (resin), and expect that all parts would print well on any resin 3D printer. I have not tried to print this on a filament printer, as I believe the quality would not be good enough. When printed in resin, all parts really look production-grade if the proper resin is used.

### Software
#### Main back-end code
The main code of the cog1 platform is a .net core application that contains all the necessary components (web services and general logic) to keep the cog1 working. The current version is relatively basic, and only contains those features needed to test the hardware. A web service is in place to provide basic status of the cog1, which is currently used to test the overall reliability of the hardware. All hardware functions have been tested with this software.

This is by far the area that needs the most effort, so that the cog1 can be turned into a fully-functional gateway.

#### Native back-end code
There are a few small portions of the software written in plan old C, and compiled directly in the SBC:
- A low-level i/o library that deals with digital and analog i/o, display, etc.
- A small "logo" application (configured as a service that starts early in the Linux boot process) to display the cog1 logo while the cog1 is booting.

Everything else is c#, so there's no low-level really in the c# code. 

#### Front-end code
The front-end (console) is written in Angular, based on the [Core-UI admin template](https://coreui.io/product/free-angular-admin-template/), and it communicates with the back-end via web-services, which are already integrated in such a way that creating new controllers/endpoints and consuming them from the front-end is really simple. While the functionality is relatively limited at the moment, the front-end contains all the necessary building-blocks to be extended easily:
- It communicates with the back-end via web-services, with proxies created using [swagger-typescript-api](https://github.com/acacode/swagger-typescript-api). This makes it very simple to create new controllers and endpoints in the back-end andthen use them in the front-end with full type-checking and code insight.
- It handles authentication with the back-end.
- It fully supports loading literals (in the language of the user) from the back-end, before the application starts.

## Getting started
### Building the hardware
The KiCad project included in the pcb folder contains everything needed to manufacture the pcb. For all prototypes, the pcb was ordered to PCBWay, and then all components were soldered manually. When soldering, I strongly advise mounting the switching power supply first, veryfying the 5V are steady and noiseless, and then mount everything else. This is to prevent blowing up anything if the switching power supply was not mounted correctly.

### Installing the software
I'll write a tutorial for this ASAP, but it's pretty straightforward.

## Current status
### What's done
### What's missing
