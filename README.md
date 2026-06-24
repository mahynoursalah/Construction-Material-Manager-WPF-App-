# Construction Material Manager

## Description

Construction Material Manager is a WPF desktop application developed to streamline the management of construction materials, quantity calculations, cost estimation, and order tracking. The application provides an intuitive interface for managing material inventories, generating material requirements for various construction activities, and maintaining order records.

The project demonstrates practical use of WPF desktop development concepts, including data binding, collection management, validation, JSON data persistence, and object-oriented design principles.
Applied construction-specific quantity takeoff calculations for concrete, steel, paint, and tile estimation.
Implemented cost estimation and order tracking workflows to simulate real-world construction material management processes.


## Technologies

* C#
* WPF (Windows Presentation Foundation)
* .NET Framework 4.8
* XAML
* Object-Oriented Programming (OOP)
* ObservableCollection
* JSON Serialization & Deserialization
* Windows Forms Dialogs
* Data Binding

## Features

### Material Management

* Add, view, and delete construction materials.
* Store material details including name, category, unit, and unit price.
* Prevent duplicate material entries.
* Validate user input before processing operations.
* Display materials using a WPF DataGrid.
* Preload commonly used construction materials such as cement, sand, gravel, rebar, paint, and tiles.

### Material Calculations

* Calculate concrete volume requirements with configurable waste allowance.
* Estimate steel quantities based on bar diameter, length, and quantity.
* Calculate paint requirements using area, coverage rate, and number of coats.
* Estimate tile quantities based on room area, tile dimensions, and waste percentage.
* Automatically generate material requirements from calculation results.

### Order Management

* Create material orders automatically from calculated quantities.
* Track orders with detailed information including order number, material, category, quantity, unit price, total cost, date, and status.
* Search orders by material name.
* Filter orders by category and order status.
* Update order status between Pending and Delivered.
* Delete orders with confirmation prompts.

### Dashboard & Reporting

* Display summary statistics including:

  * Total Materials
  * Total Orders
  * Total Spending
* Provide real-time status updates through a status bar.

### Data Persistence

* Save materials and orders to JSON files.
* Load previously saved project data from JSON files.
* Maintain application data between sessions.
  
