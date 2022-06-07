# ![VJK Solutions](https://vjk.solutions/images/logo-64.png) JimCO Retailers Demonstration App ![JimCo Logo](https://vjk.solutions/images/jimcologo-64.png)

The **JimCo Retailers** demonstration application is a group of four related web applications. It consists of the following parts:

1. A back end API written in C# using ASP.Net Core.
2. A Retail web site aimed at online shoppers written using React
3. An Employee Portal web site aimed at internal employees written using React
4. A Vendor Portal web site aimed at Vendors who distribute products to the store written using React

All four web sites are hosted on Microsoft Azure. The code repositories are hosted on [GitHub](https://github.com) (see below for links to the individual repositories, or click [here](https://github.com/vjkrammes) to see them all), and CI/CD is handled by Microsoft Azure DevOps.

## The API (This Project)

Repository [here](https://github.com/vjkrammes/JimCo.api)

The API is the back end for the three front end web sites. It is written in C# version 10 using ASP.Net Core / .Net Core version 6. It is architected in a multi-layer approach as shown below:

|Layer Name|Function|Type|
|----------|--------|----|
|Data Access|The repository layer that handles access to the database|Class Library|
|Services|The business logic layer that sits between the API and Data Access Layers|Class Library|
|API|The web-facing API using minimal endpoints|Web API|
|Models|The DTO models shared between the API and Services Layers|Class Library|
|Common|Common classes, Attributes, Enumerations, extension methods, etc|Class Library|

In addition to the above, the API uses [Dapper](https://github.com/DapperLib/Dapper) to access a Microsoft SQL / Azure SQL database.

## The Retail Site

Repository [here](https://github.com/vjkrammes/jimco.retailsite)

The Retail site is written in **TypeScript** using **React** functions / hooks, *React version 18* and *React Router version 6.3*. It is responsive down to 375 pixels. It does not currently use any authorization or authentication (though this may change).

## The Employee Portal

Repository [here](https://github.com/vjkramems/jimco.employeeportal)

The Employee Portal is written in **TypeScript** using **React** functions / hooks, *React version 18* and *React Router version 6.3*. It is designed specifically for in-store use on tablets or desktop computers and is therefore designed for a minimum screen size of 1024 x 768 pixels. [Auth0](https://auth0.com) is used for authorization, and the internal database is used for role-based authentication. 

## The Vendor Portal

Repository [here](https://github.com/vjkrammes/jimco.vendorportal)

The vendor portal is written in **TypeScript** using **React** functions / hooks, *React version 18* and *React Router version 6.3*. It is designed specifically for desktop computers and therefore is designed for a minimum screen resolution of 1024 x 768 pixels. [Auth0](https://auth0.com) is used for Authentication, and the internal database is used for role-based authorization.

## External Dependencies

For the API:

1. [AspNetCoreRateLimit](https://github.com/stefanprodan/AspNetCoreRateLimit) Used to rate limit requests
2. [Hashids.net](https://hashids.org/net/) Used to obfuscate Ids sent to end users
3. [Dapper](https://github.com/DapperLib/Dapper) Used to access the database
4. [Json.NET](https://www.json.org/json-en.html) a JSON library for .Net

For the Web Sites, the following NPM packages:

1. *react, react-dom, react-router-dom, react-icons*
2. *@mui/material*, used for alerts
3. *@auth0/auth0-react*, used to interact with Auth0
4. *uuid*, used to generate uuids
