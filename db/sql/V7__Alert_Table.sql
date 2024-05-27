CREATE TABLE Alert (
    Id              INT AUTO_INCREMENT PRIMARY KEY,
    DeviceId        INT,
    Timestamp       TIMESTAMP,
    Description     VARCHAR(200),
    IsRead          BOOLEAN,
    FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);