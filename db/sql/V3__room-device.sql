-- Create Room table
CREATE TABLE Room (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId                  INT,
    RoomName                VARCHAR(50) NOT NULL
    FOREIGN KEY (UserId) REFERENCES User(Id)
);

-- Create Device table
CREATE TABLE Device (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    RoomId                  INT,
    DeviceName              VARCHAR(50) NOT NULL
    FOREIGN KEY (RoomId) REFERENCES Room(Id)
);