# 🏣Hotel Booking System

![License](https://img.shields.io/badge/License-MIT-blue.svg)
![Status](https://img.shields.io/badge/Status-Development%20%7C%20Complete-green.svg)
![Repo Size](https://img.shields.io/github/repo-size/imansafari1991/HotelBooking)

## 📝 Overview
The **Hotel Booking** API is the robust backend service for a modern hotel reservation system. It is implemented as a high-performance RESTful API using .NET 10 Web API and is built upon the principles
of Clean Architecture to ensure maximum decoupling, testability, and maintainability.

The system's design is heavily influenced by **Test-Driven Development (TDD)**, ensuring that every piece of business logic is rigorously tested and validated in isolation.


## 🏛 Clean Architecture Layers

The project structure strictly adheres to the Clean Architecture (sometimes called Onion Architecture), where dependencies flow inward. 
This design isolates the critical business rules (**Domain**) from external concerns like databases (**Infrastructure**) and frameworks (**Presentation**).


The system is logically divided into the following layers:

### 1. Domain (The Core)

**Purpose**: Contains the enterprise-wide business rules. This layer is the heart of the application and holds the most critical logic.

**Contents**: Core Entities (Booking, Room), Value Objects, Domain Services, and Domain Events.

**Key Principle**: Zero Dependencies. This layer is completely independent of all other layers, frameworks, and databases, making it stable and 100% testable via Unit Tests.

### 2. Application

**Purpose**: Contains the application-specific business rules, or use cases. It orchestrates the flow of data to and from the Domain layer and defines the necessary external interfaces.

**Contents**: Use Case Implementations, Commands & Queries (following the CQRS pattern), Data Transfer Objects (DTOs), and application-level interfaces like Repositories (IRoomRepository).

**Key Principle**: Defines what the system does. It depends only on the Domain layer.

### 3. Infrastructure

**Purpose**: Handles all external technical concerns and implements the interfaces defined in the inner layers (Application). This is the "plumbing" layer.

**Contents**: Concrete implementations of Repositories , Database Context (DbContext), Database Migrations.

**Key Principle**: Depends on the Application and Domain layers, but the inner layers never depend on it.


## 4. Presentation (WebAPI)

**Purpose**: The entry point for the API. It handles HTTP requests and responses.

**Contents**: Thin Controllers , Dependency Injection Configuration.

**Key Principle**: The controllers delegate all business logic to the Application layer.
<p align="center">
<img width="600"  alt="clean architecture" src="https://github.com/user-attachments/assets/9faeedae-a995-4435-a935-4d03438869a1" />
</p>



## 🧪 Test-Driven Development (TDD) Approach

Development is driven by the TDD cycle (Red -> Green -> Refactor). This methodology is enforced across all layers:

**Domain & Application**: Covered by fast, isolated Unit Tests using mocked dependencies. This guarantees the integrity of the business logic.

**Infrastructure**: Covered by Integration Tests which validate the data persistence and interaction with the actual database (e.g., using an in-memory SQLite provider for testing).
<p align="center">
<img width="600" height="1034" alt="image" src="https://github.com/user-attachments/assets/654a5d64-1ac5-47db-81e1-5c05e6363b5a" />
</p>


## 🛠 Getting Started

###Prerequisites

** .NET 10.0 SDK**

A preferred IDE (Visual Studio, VS Code with C# Dev Kit)
Installation and Run

#### Clone the Repository:
```
git clone https://github.com/imansafari1991/HotelBooking.git
cd HotelBooking
```
#### Restore Dependencies:
```
dotnet restore
```
#### Apply Database Migrations:

Ensure your connection string is configured in the appsettings.json of the Presentation project.

**Update the database:**
```
dotnet ef database update --project HotelBooking.Infrastructure
```

#### Run the API:
```
dotnet run --project HotelBooking.Api
```




