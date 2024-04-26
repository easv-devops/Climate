-- Create temperature readings table
CREATE TABLE ReadingTemperature (
    ReadingId           INT PRIMARY KEY AUTO_INCREMENT,
    DeviceId            INT,
    Timestamp           TIMESTAMP,
    Temperature         FLOAT,
    FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);

-- Create humidity readings table
CREATE TABLE ReadingHumidity (
    ReadingId           INT PRIMARY KEY AUTO_INCREMENT,
    DeviceId            INT,
    Timestamp           TIMESTAMP,
    Humidity            FLOAT,
    FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);

-- Create particle 2,5 readings table
CREATE TABLE ReadingParticle2_5 (
    ReadingId           INT PRIMARY KEY AUTO_INCREMENT,
    DeviceId            INT,
    Timestamp           TIMESTAMP,
    P2_5                INT,
    FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);

-- Create particle 10 readings table
CREATE TABLE ReadingParticle10 (
    ReadingId           INT PRIMARY KEY AUTO_INCREMENT,
    DeviceId            INT,
    Timestamp           TIMESTAMP,
    P10                 INT,
    FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);