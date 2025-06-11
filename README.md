# Extractify-Api
A .NET 8 backend for Extractify, a web scraping application designed to extract text and images from websites using a hybrid approach: default mode for quick scraping and advanced mode for custom CSS selectors. The API supports CRUD operations for scraping tasks, executes scraping jobs, and stores results in a SQL Server database.
Table of Contents

# Features
Architecture
Prerequisites
Setup
Running the Application
Usage
Dependencies
Project Structure
Contributing
License

Features

Hybrid Scraping:
Default Mode: Scrapes text (h1,h2,h3,p) and images (img[src]) automatically.
Advanced Mode: Allows custom CSS selectors (e.g., .quote .text, img) for precise data extraction.


RESTful API: Manages scraping tasks via endpoints (e.g., POST /api/ScrapingTasks/{id}/execute).
Data Persistence: Stores tasks and scraped data in SQL Server ((localdb)\MSSQLLocalDB;Database=Extractify).
Logging: Uses Serilog to log errors and info to logs/extractify.log.
Dependency Injection: Clean architecture with services, repositories, and interfaces.

# Architecture

Clean Architecture:
Extractify.Api: ASP.NET Core Web API.
Extractify.Application: Business logic, services, DTOs.
Extractify.Infrastructure: Data access, scraping logic, external services.


# Key Components:
ScraperClient.cs (artifact_id: 477061e0-fb81-433f-821e-214d83b97c12): Handles web scraping with HtmlAgilityPack.
ScrapingService.cs (artifact_id: 43a23269-262d-4a31-af61-b9a49bb68ec6): Orchestrates task execution.
Program.cs (artifact_id: 6188a3a5-6537-4ff3-9eba-abe5051a70a8): Configures API and CORS.



# Prerequisites

.NET 8 SDK
SQL Server LocalDB
Git
Visual Studio 2022 or any C# IDE
Optional: Postman for API testing

Setup

Clone the Repository:git clone https://github.com/<your-username>/Extractify-Api.git
cd Extractify-Api


Restore Dependencies:dotnet restore


Configure Database:
Ensure appsettings.json has the connection string:"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=Extractify;Trusted_Connection=True;"
}


Apply migrations:cd src/Extractify.Infrastructure
dotnet ef database update --startup-project ../Extractify.Api





Running the Application

Run the API:cd src/Extractify.Api
dotnet run


Access at https://localhost:7142.
Swagger UI: https://localhost:7142/swagger.


Verify Logs:
Check logs/extractify.log for runtime details.



Usage

Create a Task:
Endpoint: POST /api/ScrapingTasks
Body:{
  "url": "https://quotes.toscrape.com",
  "selector": "",
  "imageSelector": "",
  "status": "Pending"
}


Response: Task ID (e.g., 22).


Execute Task:
Endpoint: POST /api/ScrapingTasks/22/execute
Response: true if successful.


View Results:
Endpoint: GET /api/ScrapingTasks/22/data
Example Response:[
  { "id": 1, "scrapingTaskId": 22, "content": "The world as we have created it...", "imageUrl": null, "scrapedAt": "2025-06-11T..." }
]




Advanced Mode:
Set selector (e.g., .quote .text) and imageSelector (e.g., img) in the task creation request.



Dependencies

.NET Packages:
Microsoft.EntityFrameworkCore.SqlServer: 8.0.0
AutoMapper: 12.0.1
HtmlAgilityPack: 1.11.46
Serilog.AspNetCore: 8.0.1
Serilog.Sinks.File: 5.0.0


Configuration:
See Extractify.Api.csproj for full list.



Project Structure
Extractify-Api/
├── src/
│   ├── Extractify.Api/           # API controllers, Program.cs
│   ├── Extractify.Application/   # Services, DTOs, interfaces
│   ├── Extractify.Infrastructure/# Repositories, scraping logic
├── Extractify.sln                # Solution file
├── README.md                     # This file
├── .gitignore                    # Git ignore rules

Contributing

Fork the repository.
Create a feature branch: git checkout -b feature/your-feature.
Commit changes: git commit -m "Add your feature".
Push: git push origin feature/your-feature.
Open a pull request on GitHub.

License
MIT License. See LICENSE for details.
