﻿-- Create User table
CREATE TABLE User (
    Id INT AUTO_INCREMENT       PRIMARY KEY,
    Email VARCHAR(255)          NOT NULL UNIQUE
);

-- Create UserInformation table
CREATE TABLE UserInformation (
    UserId                      INT,
    FirstName                   VARCHAR(100),
    LastName                    VARCHAR(100),
    FOREIGN KEY (UserId) REFERENCES User(Id)
);

-- Create ContactInformation table
CREATE TABLE ContactInformation (
    UserId                      INT,
    CountryCode                 VARCHAR(5),
    Number                      VARCHAR(20),
    FOREIGN KEY (UserId) REFERENCES User(Id)
);

-- Create UserStatus table
CREATE TABLE UserStatus (
    UserId                      INT,
    BanTimestamp                TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES User(Id)
);