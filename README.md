# SerialButtonLogger
A background app for tracking work time and presenting it in a SAP friendly way (copy paste).

This app is intended to be used with a hardware push button connected to the serial interface of your PC. If you intend to use an usb adapter, make sure it is one that supports Hardware flow control (CTS and DTR).
The Button circuit is really simple:
![Alt text](./circuit.svg)
<img src="./circuit.svg">

The idea is to set the RTS pin high, thus also having the CTS high. When the push button is pressed, the CTS line is pulled to GND, which can be read as a low state.
You may have to experiment with your resistor value. The higher the value, the lower the power consumption of your switch. However, also the input impedance of the CTS limits you, as the voltage will not drop towards GND sufficiently, if your resistor has a too high value. 10k seemed to work fine for my application with a integrated COM Port of the docking station.