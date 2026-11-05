-- 000_create_database.sql
-- Creates the CyberpunkDB database. Safe to run when it already exists.
-- Run the scripts in this folder in number order to rebuild the database from scratch.

IF DB_ID(N'CyberpunkDB') IS NULL
    CREATE DATABASE CyberpunkDB;
GO
