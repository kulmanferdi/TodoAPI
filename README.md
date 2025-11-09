# Todo API

Simple Todo API implementation.

## Features
- Create, Read, Update, Delete (CRUD) operations for todo items
- SQLite database for data storage

## Requirements
- dotnet 9.0 SDK or later
- Entity Framework Core
- SQLite
- xUnit for testing

## Endpoints
- `GET /todos`: Retrieve all todo items
- `GET /todos/<id>`: Retrieve a specific todo item by ID
- `POST /todos`: Create a new todo item
- `PUT /todos/<id>`: Update an existing todo item by ID
- `DELETE /todos/<id>`: Delete a todo item by ID


## Run
```sh
dotnet run
```
App will start on localhost, use [Postman](https://www.postman.com/), [Yaak](https://yaak.app/), [Insomnia](https://insomnia.rest/) or Curl to interact with the API.

## Test
```sh
dotnet test
```

## Contributing
Open for contributions!

## License
MIT License