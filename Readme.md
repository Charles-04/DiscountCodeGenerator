# Discount Code Generation and Usage System
This project is a discount code generation and usage system developed using gRPC for server-side communication and ASP.NET MVC for the client-side interface.

## Features
* Discount Code Generation: Randomly generates unique discount codes of 7-8 characters.
* Discount Code Usage: Allows users to manually enter generated discount codes for usage.
* Pagination: Implements pagination for displaying a limited number of discount codes per page.
* Parallel Processing: Supports processing multiple requests in parallel for improved performance.
* Error Handling: Provides error handling for invalid requests and exceptions.
## Requirements
* .NET Core SDK version 8
* Visual Studio or Visual Studio Code (for development)
* Installation and Setup
* Clone the repository to your local machine.
* Open the solution in Visual Studio.
* Build the solution to restore dependencies.
* Update the connection string in appsettings.json for the database configuration.
* Run the database migrations to create the necessary tables (dotnet ef database update).
* Start the server-side application to launch the gRPC server.
* Start the client-side application to access the discount code generation and usage interface.
## Usage
* Navigate to the client-side application in your web browser.
* Use the provided interface to generate discount codes or manually enter existing codes for usage.
* Navigate between pages using pagination controls to view more discount codes.
* Handle errors and exceptions as needed based on the defined error messages.
