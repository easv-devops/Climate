-- Create Room table
CREATE TABLE Room (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId                  INT,
    RoomName                VARCHAR(50) NOT NULL
    FOREIGN KEY (UserId) REFERENCES User(Id)
);