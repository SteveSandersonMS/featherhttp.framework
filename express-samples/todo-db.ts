import { Todo } from './todo';

export const todos = new Map<number, Todo>();

todos.set(123, new Todo(123, 'Bert', false));
todos.set(456, new Todo(456, 'Blurt', true));
