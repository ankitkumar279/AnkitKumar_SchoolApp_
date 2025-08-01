# AnkitKumar_SchoolApp

Welcome to the **AnkitKumar_SchoolApp**! This is a simple ASP.NET Core MVC web application designed to manage teacher records using a MySQL database.

---

## Features

- View a list of teachers with their details
- Add new teachers
- Edit existing teacher records
- Delete teachers
- View detailed info of each teacher

---

## Technologies Used

- ASP.NET Core MVC
- C#
- MySQL (via MySqlConnector)
- Entity Framework Core (optional, commented out in this version)
- Razor views for UI
- Bootstrap 5 for styling

---

## Project Structure

- **Controllers**: Handles HTTP requests and CRUD operations
- **Models**: Teacher class represents teacher data, and SchoolDbContext for EF Core (optional)
- **Views**: Razor pages for displaying forms, lists, and details
- **wwwroot/css**: Contains custom styles and Bootstrap CSS

---

## How to Run

1. **Setup MySQL database**  
   Create a MySQL database with a `teachers` table that has columns matching the `Teacher` model:
   - `teacherid` (int, primary key, auto-increment)
   - `teacherfname` (varchar)
   - `teacherlname` (varchar)
   - `employeenumber` (varchar)
   - `hiredate` (datetime)
   - `salary` (decimal)

2. **Configure Connection String**  
   In your `appsettings.json`, add your MySQL connection string as:  
   ```json
   "ConnectionStrings": {
       "SchoolDbConnection": "server=YOUR_SERVER;user=YOUR_USER;password=YOUR_PASS;database=YOUR_DB"
   }
