# GradeBook
GradeBook is a digital implementation of traditional paper grade book that commonly used in Ukrainian higher education system. 

## Main Features
* Flexible layout
* Works on mobiles
* REST API
* Three access levels - Lecturer, Student and Administrator
* Convenient menu grouping of grade books
* Automatic attendance book generation
* Create grade books and view grade book details
* Add tasks and view task details
* Update student grades and attendances

## Project Structure
```
├─ ..
|  ├─ db                    - database sample data
|  ├─ docs                  - project related documents
|  ├─ frontend              - client application
|  ├─ services              - backend services
|  |  ├─ GradeBookService   - main api's application
|  |  ├─ OAuthService       - Custom OAuth server
|  |  ├─ GradeBook.Data		- Components to work with Database
```

## Technology Stack
*Front End*
* HTML/CSS
* Angular Material
* AngularJS

*Back End*
* C#
* .NET 5
* Entity Framework Core
* MySql.EntityFrameworkCore
* RESTful API
* ASP.NET API
* Swagger documentation
* OAuth2
* JSON Web Token (JWT)
* JSON data
* MySQL
