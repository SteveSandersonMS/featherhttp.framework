import * as fs from 'fs';
import * as express from 'express';
import * as bodyParser from 'body-parser';
import { Validator } from 'express-json-validator-middleware';
import * as db from './todo-db';
import { Todo } from './todo';

const app = express();
const port = process.env.PORT || 3000;
const jsonValidator = new Validator({ allErrors: true });
const todoSchema = JSON.parse(fs.readFileSync('./schema/todo.json', { encoding: 'utf-8' }));

app.use(bodyParser.json());

app.get('/api/todos', (req, res) => {
    const todos = db.todos.values();
    res.json(Array.from(todos));
});

app.get('/api/todos/:id(\\d+)', (req, res) => {
    const todo = db.todos.get(parseInt(req.params['id']));
    if (todo) {
      res.json(todo);
    } else {
      res.status(404);
      res.end();
    }
});

app.post('/api/todos', jsonValidator.validate({ body: todoSchema }), (req, res) => {
    const data = req.body as Todo;
    data.id = db.todos.size + 1;
    db.todos.set(data.id, data);
    res.status(204);
    res.end();
});

app.post('/api/todos/:id(\\d+)', (req, res) => {
    const existingTodo = db.todos.get(parseInt(req.params['id']));

    if (existingTodo) {
        existingTodo.isComplete = (req.body as Todo).isComplete;
    } else {
        res.status(404);
    }

    res.end();
});

app.delete('/api/todos/:id(\\d+)', (req, res) => {
    const id = parseInt(req.params['id']);

    if (db.todos.has(id)) {
        db.todos.delete(id);
        res.status(204);
    } else {
        res.status(404);
    }

    res.end();
});

app.listen(port, () => {
    console.log(`Example app listening at http://localhost:${port}`)
});
