# Finances
App (first an API, then maybe I'll add a frontend) to keep track of expenses. 
Logic is based on the spreadsheet I've made that me and my partner have been using to keep track of our expenses and to split them accordingly.
The API should be able to calculate total monthly expenses by category and sucategory, as well as split expenses between users in different ways.
This split of expenses between household users is called Repartition. It states how much each user has paid in common expenses and how much the user should pay in common expenses, based on the expense split type chosen by the user.

## Approach
ASP.NET API, using a SQL database. Code first approach using Entity Framework Core. Mainly test-driven development for API, particularly for logically complex parts (Repartition). Controller-Service-Repository-Database Architecture.

## TODO
- set up database
- refine database objects
- set up dependency injection
- implement service & repo methods
- create API project

## Current limits
- Repartition only works with households of 1 or 2 people (even though arguably it does not make sense with a single person household)
- A person might not be added to an existing household? Hasn't been tested, and no plans on doing do.