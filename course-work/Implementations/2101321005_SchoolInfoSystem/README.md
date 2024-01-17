# School Information System
This project is my first attempt at creating a distributed application. It helped me learn more about separation of concerns, creating REST API and implementing authentication using JWT. The application is separated into five projects:
* [Data](https://github.com/omin29/distributed-applications-se/tree/master/course-work/Implementations/2101321005_SchoolInfoSystem/README.md#Data)
* [Repository](https://github.com/omin29/distributed-applications-se/tree/master/course-work/Implementations/2101321005_SchoolInfoSystem/README.md#Repository)
* [ApplicationService](https://github.com/omin29/distributed-applications-se/tree/master/course-work/Implementations/2101321005_SchoolInfoSystem/README.md#ApplicationService)
* [SchoolInfoAPI](https://github.com/omin29/distributed-applications-se/tree/master/course-work/Implementations/2101321005_SchoolInfoSystem/README.md#SchoolInfoAPI)
* [SchoolInfoMVC](https://github.com/omin29/distributed-applications-se/tree/master/course-work/Implementations/2101321005_SchoolInfoSystem/README.md#SchoolInfoMVC)

## Data
This project contains the entities, the database context and the migrations which are used to create the database. I chose to use the Code First approach where I create the data classes first and with the help of the ORM [Entity Framework Core](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore) I am able to create the database using migrations. The database context is configured to work with Microsoft SQL Server.

## Repository
I implemented the Generic Repository and Unit of Work design patterns in this project. The Generic Repository provides an extra layer of abstraction and allows the management of database entities from one place. The Unit of Work allows us to make transactions which help us maintain the consistency of the data.

## ApplicationService
This project consists of implementations of services for the entities in the database and DTOs which are needed for transferring data for said services. These services include CRUD operations, pagination, filtering and registration/login with password hashing for extra security. For hashing and verifying the passwords I used [BCrypt.Net-Next](https://www.nuget.org/packages/BCrypt.Net-Next).

## SchoolInfoAPI
This is the REST API which consumes the services from [ApplicationService](https://github.com/omin29/distributed-applications-se/tree/master/course-work/Implementations/2101321005_SchoolInfoSystem/README.md#ApplicationService). It is created using [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-8.0). The API has [documentation](https://github.com/omin29/distributed-applications-se/blob/master/course-work/Implementations/2101321005_SchoolInfoSystem/SchoolInfoAPI/SchoolInfoApiDocumentation.xml) generated by Swagger. With the help of [Swagger UI](https://github.com/swagger-api/swagger-ui), the endpoints can be tested directly in the browser. This eliminates the need for testing tools like Postman. The API can issue a bearer token using the JWT encoding standard after the user successfully logs into the system. This bearer token is valid for a certain period of time and it can be used to gain access to the other endpoints of the API which are for managing the students, teachers and classes.

## SchoolInfoMVC
This is a MVC project which consumes the [SchoolInfoAPI](https://github.com/omin29/distributed-applications-se/tree/master/course-work/Implementations/2101321005_SchoolInfoSystem/README.md#SchoolInfoAPI). It is also made using [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-8.0). This web app communicates with the API using [RestSharp](https://restsharp.dev/) and allows the user to utilize the API functionalities. The user can log in and start viewing and managing information about the students, teachers and classes. After successful login, the returned JWT bearer token is stored temporarily as a cookie and it is attached to the header of every request made to the API. The API tries to verify if the bearer token is signed by it. If the verification is successful, it knows that the request is made by an authenticated user and returns a successful response.