# Todo API

Simple Todo API implementation.

## Features
- Create, Read, Update, Delete (CRUD) operations for todo items
- SQLite database for data storage

## Requirements
- dotnet 10 SDK
- Entity Framework Core
- SQLite
- xUnit for testing

## API Endpoints

### GET
- `GET /todos`: Retrieve all todo items
- `GET /todos/<id>`: Retrieve a specific todo item by ID
- `GET /todos/completed`: Retrieve all completed todo items
- `GET /todos/incomplete`: Retrieve all not completed todo items
- `GET /todos/count`: Retrieve the count of todo items
- `GET /todos/count/completed`: Retrieve the count of completed todo items
- `GET /todos/count/incomplete`: Retrieve the count of not completed todo items
- `GET /todos/search/<term>`: Search todo items by term
- `GET /todos/ordered/asc`: Retrieve all todo items ordered ascendingly by title
- `GET /todos/ordered/desc`: Retrieve all todo items ordered descendingly by title

### POST
- `POST /todos`: Create a new todo item

### PATCH
- `PATCH /todos/<id>/completed`: Mark a todo item as completed by ID

### PUT
- `PUT /todos/<id>`: Update an existing todo item by ID

### DELETE
- `DELETE /todos/<id>`: Delete a todo item by ID
- `DELETE /todos/completed`: Delete all completed todo items

## Contributing
Feel free to submit issues or pull requests. Please use the templates provided in the repository.

## License
Source code is distributed under MIT license. See LICENSE file for more information.