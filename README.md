# Movie Store Application

The Movie Store is a university project developed for the .NET course. It is a web application built using ASP.NET Core (.NET 6) that provides users with detailed information about various movies. The application uses SQL Server as its database and follows the MVC (Model-View-Controller) architecture.

Users can register, browse movies, and view details such as the release year, director, cast, genre, broadcast information, and a brief plot summary.

User Roles
The application now includes three user roles:

Normal User – Can register, browse the movie catalog, and view movie details.
Admin – Has additional permissions to manage movies and genres.
Owner – Has full control over admin profiles and is the only one allowed to edit or delete admin users.

Technologies
ASP.NET Core (.NET 6)
Entity Framework Core (EF Core) for database access
SQL Server for data storage
Razor Pages for dynamic UI rendering
MVC (Model-View-Controller) architecture

## Features
- **User Registration and Authentication**: Users can create accounts and log in to browse the movie collection.
- **Movie Catalog**: Displays detailed information about movies, including release year, director, cast, genre, broadcast details, and a short plot summary.
- **Admin Features**: Admins can:
  - Add new movies and genres.
  - Edit existing movie information.
  - Delete movies from the catalog.
  - Manage user accounts (delete or modify user information).
- **Role-based Access Control**: Regular users and admins have different access rights.
- Owner – Has full control over admin profiles and is the only one allowed to edit or delete admin users.

## Installation
Prerequisites
To run the project, you need the following dependencies installed:

.NET 6 SDK
SQL Server
Entity Framework Core
ASP.NET Core MVC
Razor Pages
1. Clone the repository:
   ```bash
   git clone https://github.com/subzero0008/MovieStore.git
   Navigate to the project directory and restore the required packages:
cd MovieStoreMvc
dotnet restore
Set up the database:
Update the connection string in appsettings.json with your SQL Server details.
Run the migrations to set up the database: dotnet ef database update
dotnet build
dotnet run
Usage
Register/Login: Create an account or log in to browse movies.
Browse Movies: Users can view details about the available movies in the store.
Admin Panel: Admins can access additional functionality to manage the movie database.
Admin User Features
Add/Edit Movies: Admins can add new movies or edit existing ones.
Manage Genres: Admins can add, edit, or delete genres.
User Management: Admins can delete regular users or update their information.
Build and Deployment
Credits
This application was developed by Yulian Yuriev as part of a web development project.

css

