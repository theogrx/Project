
# Project Documentation: Country Information API

## Overview
The **Country Information API** is a RESTful service that provides country-related information, including data about country names, borders, and capitals. The API fetches data either from an external source (via an API) or from a local database and caches the results for performance improvements. The service supports fetching country details, storing them in a local database (SQL Server), and caching the results to minimize redundant API calls.

## Key Components
- **Controllers**: Handle incoming HTTP requests and interact with the `CountryHelper` service to fetch and return country data.
- **CountryHelper**: Contains the logic for fetching country data either from the external API or local database. It also handles caching.
- **ApplicationDbContext**: Entity Framework DbContext used to interact with the SQL Server database.
- **CustomMemoryCache**: A custom caching mechanism that stores country data in memory.
- **Models**: Defines the structure of the country data and DTOs used in API responses.

## Technologies
- **.NET 8**: The application is built using the .NET framework, with dependency injection and middleware to handle requests and responses.
- **Entity Framework Core**: ORM (Object Relational Mapper) to interact with a SQL Server database.
- **SQL Server 2022**: The database used for storing country data.
- **External REST API (e.g., RestCountries API)**: Data is fetched from an external API for country information when necessary.
- **Custom Memory Cache**: Used to store and retrieve country data to optimize API calls.
- **Postman/Swagger**: Used for testing and interacting with the API.

---

## Project Structure

### Controllers

#### CountryController
- **Route**: `/api/country`
- **Methods**:
    - `GET`: Fetches the list of countries. Returns cached data if available, or fetches fresh data from the API and stores it in the database and cache.

#### Example:
**GET /api/country**

```json
[
    {
        "name": "United States",
        "borders": ["Canada", "Mexico"],
        "capital": ["Washington, D.C."]
    },
    {
        "name": "Canada",
        "borders": ["United States"],
        "capital": ["Ottawa"]
    }
]
```

### Services

#### CountryHelper
- Responsible for interacting with the external API, caching the data, and interacting with the database.
- **Methods**:
  - `FetchCountriesAsync()`: Fetches country data either from the cache, the database, or the external API and saves it to the database and cache.

#### CustomMemoryCache
- A simple custom caching service used to store data in memory for quick access.
- **Methods**:
  - `TryGetValue<T>(string key, out T value)`: Checks if a cache entry exists for a given key.
  - `Set(string key, object value)`: Stores data in the cache with the specified key.
  - `Remove(string key)`: Removes an item from the cache.

#### ApplicationDbContext
- Represents the connection to the database and is used for interacting with the `Countries` table.
- **Tables**:
  - `Countries`: Stores country data with information like country name, borders, and capital.

---

## Database Schema

### Countries Table
- **Columns**:
  - `Id` (int): Primary key, unique identifier for each country.
  - `Name` (string): The name of the country.
  - `Borders` (List<string>): A list of country names that the country shares borders with.
  - `Capital` (List<string>): A list of capitals for the country.

---

## AppSettings Configuration

### appsettings.json
The application relies on the `appsettings.json` file for configuration, particularly for the API settings and database connection string.

```json
{
  "ApiSettings": {
    "RestCountriesUrl": "https://restcountries.com/v3.1/all"
  },
  "ConnectionStrings": {
    "Project": "Data Source=(local);Initial Catalog=Project;Integrated Security=True;Encrypt=False"
  }
}
```

### Key Settings:
- **RestCountriesUrl**: URL of the external API to fetch country data.
- **ConnectionStrings.Project**: SQL Server connection string used by the application.

---

## API Endpoints

### GET /api/country
Fetches the list of countries. This endpoint will first check the cache for the requested data. If the data isn't available, it will fetch the data from the external API and store it in the database and cache.

#### Request:
```http
GET /api/country
```

#### Response:
- **200 OK**: A JSON array containing the country data, including names, borders, and capitals.
- **400 BadRequest**: In case of an error or no countries found.
  
Example response:
```json
[
    {
        "name": "United States",
        "borders": ["Canada", "Mexico"],
        "capital": ["Washington, D.C."]
    },
    {
        "name": "Canada",
        "borders": ["United States"],
        "capital": ["Ottawa"]
    }
]
```

---

## Dependencies

1. **HttpClient**:
   - Used to make requests to the external RestCountries API to fetch country data.

2. **DbContext (ApplicationDbContext)**:
   - Manages interaction with the SQL Server database, including storing and retrieving country data.

3. **IMemoryCache / CustomMemoryCache**:
   - Caches country data to improve performance and reduce redundant calls to the external API.

4. **ApiSettings**:
   - Contains the configuration for the external API URL.

---

## Usage & Workflow

1. **Initialization**:
   - When the application starts, the necessary services (`DbContext`, `CustomMemoryCache`, and `CountryHelper`) are registered in the dependency injection container.
   - The API controller (`CountryController`) is ready to handle incoming HTTP requests.

2. **Fetching Country Data**:
   - On a request to `GET /api/country`, the `CountryController` will check the cache for the data.
   - If the cache is empty, it will call `CountryHelper.FetchCountriesAsync()`, which will fetch data from the external API, store it in the database and cache, and return the result.

3. **Caching**:
   - If data is already in the cache, the cached data is returned immediately, reducing the need for API calls and improving performance.

4. **Database**:
   - Data fetched from the API is saved to the database for later use. The database acts as the persistent storage for the country data.

---

## Testing the API with Postman

To test the API using Postman, follow these steps:

1. Open Postman and set the request type to `GET`.
2. Set the URL to `http://localhost:5000/api/country` (or the appropriate URL based on your server configuration).
3. Send the request and inspect the response. You should receive a list of countries with their names, borders, and capitals.

---

## Conclusion

This API provides a simple and efficient way to fetch and cache country information. By leveraging external APIs, a custom caching mechanism, and a local SQL Server database, the API ensures that requests are handled efficiently and consistently.
