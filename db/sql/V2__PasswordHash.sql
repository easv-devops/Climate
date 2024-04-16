-- Create PasswordHash table
CREATE TABLE PasswordHash (
    UserId                  INT PRIMARY KEY,
    Hash                    TEXT NOT NULL,
    Salt                    VARCHAR(255) NOT NULL,
    Algorithm               VARCHAR(50) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES User(Id)
);
