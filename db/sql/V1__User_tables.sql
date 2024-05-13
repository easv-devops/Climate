-- Create User table
CREATE TABLE User (
    Id INT AUTO_INCREMENT       PRIMARY KEY,
    Email VARCHAR(255)          NOT NULL UNIQUE
);

-- Create UserInformation table
CREATE TABLE UserInformation (
    UserId                      INT PRIMARY KEY,
    FirstName                   VARCHAR(100),
    LastName                    VARCHAR(100),
    FOREIGN KEY (UserId) REFERENCES User(Id)
);

-- Create CountryCode table
CREATE TABLE CountryCode (
    Country VARCHAR(255),
    CountryCode VARCHAR(10),
    IsoCode VARCHAR(10) PRIMARY KEY
);

-- Create ContactInformation table
CREATE TABLE ContactInformation (
    UserId                      INT PRIMARY KEY,
    IsoCode                     VARCHAR(5),
    Number                      VARCHAR(20),
    FOREIGN KEY (UserId) REFERENCES User(Id),
    FOREIGN KEY (IsoCode) REFERENCES CountryCode(IsoCode)
);

-- Create UserStatus table
CREATE TABLE UserStatus (
    UserId                      INT PRIMARY KEY,
    BanTimestamp                TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES User(Id)
);
