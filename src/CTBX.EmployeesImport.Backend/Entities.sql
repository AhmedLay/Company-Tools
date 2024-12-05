-- 1. Create Schema
CREATE SCHEMA IF NOT EXISTS employee;

-- 2. Create Table within the 'employee' schema
CREATE TABLE IF NOT EXISTS employee.employees (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    path VARCHAR(255),
    status VARCHAR(50)
);

