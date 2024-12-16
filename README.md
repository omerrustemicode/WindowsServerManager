# Computer Management App

## Overview
The **Computer Management App** is a Windows application built using C# and WPF. It allows you to monitor and manage the performance of remote computers via WMI (Windows Management Instrumentation). The app connects to remote computers over a network, retrieves system performance data such as CPU usage, RAM usage, and disk usage, and displays this information in real-time. 

## Features
- **Connect to remote computers**: Connect to remote machines using their IP address, username, and password.
- **Monitor system performance**: Track the CPU, RAM, and disk usage of the connected computer.
- **Network scanning**: Discover active IP addresses in the local network.
- **Automatic refresh**: The performance data is updated every 10 seconds for real-time monitoring.
- **Save and load credentials**: Store and load credentials for easy connection to multiple remote machines.

## Technologies Used
- **C#**: Programming language for the application.
- **WPF (Windows Presentation Foundation)**: Framework for building the user interface.
- **WMI (Windows Management Instrumentation)**: Used for querying system performance data from remote machines.
- **JSON**: Used for storing saved credentials.
- **DispatcherTimer**: Used for updating the system performance data every 10 seconds.

## Installation

### Prerequisites
- **.NET Framework**: Ensure that you have the .NET Framework 4.7.2 or later installed on your system.
- **Visual Studio**: You'll need Visual Studio (or any C# compatible IDE) to build and run this project.

![MainWindow of Project](ServerManager.png)

### Buy Me a Coffee
![BMC](bmc_qr.png)
