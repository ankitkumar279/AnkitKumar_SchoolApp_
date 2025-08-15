# Ankit Kumar School Management App

This is a **School Management Application** built with ASP.NET Core MVC and MySQL.  
It allows managing **Teachers, Courses, Students**, and their **enrollments**, with both web pages and API endpoints.

---

## **Project Features**

### 1. Web Application (MVC)
- **Teachers**
  - List all teachers
  - Add new teacher
  - Edit teacher
  - Delete teacher
  - View teacher details
- **Courses**
  - List all courses
  - Add new course
  - Edit course
  - Delete course
  - View course details
- **Students**
  - List all students
  - Add new student
  - Edit student
  - Delete student
  - View student details
- **StudentXCourse**
  - Manage course enrollments for students
  - Add/Edit/Delete enrollment

### 2. API Endpoints
- **TeacherAPI**
  - `GET /api/TeacherAPI` ? Get all teachers
  - `GET /api/TeacherAPI/{id}` ? Get teacher by ID
  - `POST /api/TeacherAPI` ? Add new teacher
  - `PUT /api/TeacherAPI/{id}` ? Update teacher
  - `DELETE /api/TeacherAPI/{id}` ? Delete teacher
- **CourseAPI**
  - `GET /api/CourseAPI` ? Get all courses
  - `GET /api/CourseAPI/{id}` ? Get course by ID
  - `POST /api/CourseAPI` ? Add new course
  - `PUT /api/CourseAPI/{id}` ? Update course
  - `DELETE /api/CourseAPI/{id}` ? Delete course

---

## **Features Explained**

### **Teachers Management**
- View a list of all teachers in the school.
- Add a new teacher with details such as name, employee number, hire date, and salary.
- Edit teacher information if updates are needed.
- Delete a teacher from the system when they leave.
- View detailed information about each teacher.

### **Courses Management**
- List all courses currently offered.
- Add new courses and assign a teacher to them.
- Edit course details like code, name, start/finish dates, and assigned teacher.
- Delete courses if they are no longer needed.
- View detailed course information.

### **Students Management**
- View all students in the school.
- Add new students with their personal and enrollment details.
- Edit student information when required.
- Delete students from the system if necessary.
- View full details of each student.

### **StudentXCourse (Enrollment Management)**
- Manage which students are enrolled in which courses.
- Add a student to a course, edit enrollment, or remove enrollment.
- Keeps track of all student-course relationships efficiently.

### **API Features**
- Access data programmatically using REST APIs.
- TeacherAPI: Get all teachers, get a single teacher, add/update/delete teachers.
- CourseAPI: Get all courses, get a single course, add/update/delete courses.
- Enables integration with other systems or front-end apps.

---

## **Prerequisites**
- .NET 6 SDK or later
- MySQL Server
- Visual Studio 2022 / VS Code (recommended)

---

## **Database Setup**

1. Create a database named `school`.
2. You can either run SQL commands manually (below) or **import the included `school.sql` file**.

```sql
CREATE TABLE teachers (
    teacherid INT AUTO_INCREMENT PRIMARY KEY,
    teacherfname VARCHAR(50),
    teacherlname VARCHAR(50),
    employeenumber VARCHAR(20),
    hiredate DATE,
    salary DECIMAL(10,2)
);

CREATE TABLE courses (
    courseid INT AUTO_INCREMENT PRIMARY KEY,
    coursecode VARCHAR(20),
    coursename VARCHAR(50),
    teacherid INT,
    startdate DATE,
    finishdate DATE,
    FOREIGN KEY (teacherid) REFERENCES teachers(teacherid)
);

CREATE TABLE students (
    studentid INT AUTO_INCREMENT PRIMARY KEY,
    studentnumber VARCHAR(20),
    studentfname VARCHAR(50),
    studentlname VARCHAR(50),
    enrolldate DATE
);

CREATE TABLE studentxcourse (
    id INT AUTO_INCREMENT PRIMARY KEY,
    studentid INT,
    courseid INT,
    FOREIGN KEY (studentid) REFERENCES students(studentid),
    FOREIGN KEY (courseid) REFERENCES courses(courseid)
);

## **Update appsettings.json with Your MySQL Connection**


Before running the project, make sure to update your `appsettings.json` file with your MySQL database connection details:

```json
{
  "ConnectionStrings": {
    "SchoolDbConnection": "server=127.0.0.1;port=3306;database=school;user=root;password=;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}

