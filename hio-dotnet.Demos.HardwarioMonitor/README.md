# Hardwario Monitor
This is example of MAUI Windows Desktop Application.

This application offers way for communication with the microcontrollers via J-Link. 
It opens channel for logs and settings and display the results on the screen. 
It has integrated driver for [Power Profiler Kit II (PPK2)](https://www.nordicsemi.com/Products/Development-hardware/Power-Profiler-Kit-2). It means it can power supply the microcontroller.

![HIO-Monitor-new-netsdk](https://github.com/user-attachments/assets/bd5cae44-438c-4fa2-9a4f-ea31187dcb5e)

## Main features
### HW related features
- J-Link RTT communication
- PPK2 control
- Upload firmware via J-Link
- Remote session
- Automatic load of commands from help of ZephyrRTOS device based shell
- Copy buttons for console/log or optional for each line
- Resize of text in the console/log windows
- Autoscrolling of console/log window
- Export of console/log window to text file

### Hardwario Cloud related features
- Connect to the Hardwario cloud
- List spaces, devices and messages
- Display message as json
- Create Hardwario Cloud to ThingsBoard connector for device or multiple devices
- Create Tags for devices

### ThingsBoard related features
- Connect to ThingsBoard
- List devices
- List device data (newest or historical)
- Display device data in graph
- Export device data to MS Excel
- Simulate input data for device