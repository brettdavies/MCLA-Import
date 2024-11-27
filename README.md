# KSU Lacrosse: MCLA Data Importer

This repository contains a C# application designed to import data from the [Men’s Collegiate Lacrosse Association (MCLA)](http://mcla.us) website. It facilitates the extraction and processing of lacrosse-related information, enabling seamless integration into various applications or databases.

## Features
- **Data Extraction**: Retrieves comprehensive lacrosse data from the MCLA website.
- **Data Processing**: Transforms and organizes the extracted data for easy integration.
- **Automation**: Supports scheduled data imports to ensure up-to-date information.

## Prerequisites
- **.NET Framework**: Version 4.7.2 or higher.
- **Development Environment**: Visual Studio 2015 or later.

## Getting Started
1. Clone the Repository:
    ```
    git clone https://github.com/brettdavies/MCLA-Import.git
    cd MCLA-Import
    ```
2. Open the Solution:
   - Launch `MCLAImport.sln` in Visual Studio.
3. Restore NuGet Packages:
   - In Visual Studio, navigate to `Tools > NuGet Package Manager > Manage NuGet Packages for Solution`, and restore the required packages.
4. Build the Solution:
   - Press `Ctrl+Shift+B` to build the project.
5. Configure Settings:
   - Update `App.config` with the necessary configuration settings, such as database connection strings and API endpoints.
6. Run the Application:
   - Press F5 or click the Start button in Visual Studio to execute the application.

## Usage
- **Initiate Data Import**: The application will automatically start importing data from the MCLA website upon execution.
- **Monitor Progress**: View console output or logs to monitor the import process and identify any issues.
- **Access Imported Data**: Once the import is complete, the data will be available in the configured database.

## Contributing
Contributions are not welcome. Development on this project has ended.

## Acknowledgments
- [Men’s Collegiate Lacrosse Association (MCLA)](http://mcla.us) for providing the data.

#
*Note: Ensure that you have the necessary permissions to access and use the data from the MCLA website in compliance with their terms of service.*
