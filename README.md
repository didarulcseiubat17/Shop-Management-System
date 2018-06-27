# Shop-Management-System

## Introduction
The Shop Management System is targeted to automate managers work.
System supports CRUD operations on:
  * Managers
  * Customers
  * Products
  * Orders
  
System allows to create order and calculate order sum according to added products.

You must link your mail to restore access to your account.

MD5 hash + salt are used to store password.


## Usage
* Specify connection string or move database file to folder with exe file
* Compile

## Used packages and libs:
* Entity Framework 6.0
* FluentValidation 7.5.2 - https://github.com/JeremySkinner/FluentValidation
* MahApps.Metro - https://mahapps.com/
* SQLite - https://www.sqlite.org/

Default user:

username : admin

password : admin

## Todo:
* Rework everything related with database. 

## Notes
* System created with database first approuch, that means you need existing database file.

## Special
* C#
* WPF
* MVVM (weak)
* SQLite
* Entity Framework (weak) 
* MahApps 
* FluentValidation