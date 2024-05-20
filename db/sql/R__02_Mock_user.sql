-- Insert user data
INSERT INTO User (Email)
VALUES ('user@example.com');

-- Insert user information
INSERT INTO UserInformation (UserId, FirstName, LastName)
VALUES (1, 'John', 'Doe');

-- Insert contact information
INSERT INTO ContactInformation (UserId, IsoCode, Number)
VALUES (1, 'DK', '1234567890');

-- Insert user status
INSERT INTO UserStatus (UserId, BanTimestamp)
VALUES (1, NULL);

-- Insert password hash information
INSERT INTO PasswordHash (UserId, Hash, Salt, Algorithm)
VALUES (1, '1EJybmIbon7kimzpBZXA17OxI3/iVLZK8euSAloQgK3W8ibEJ8G/Ql2J4kjtDDMRV5sN71LEgRuL+lXyP9dOHz9IuMXuWjTdFSwkKaDNbiNa9MsWy/dngKWo04jYvG8Tb26UV0Bnxd83V9zQZCPdPSQXENoRvPOhnDZKaayFYuRz4pVkBrooL9Hu9EgrCzE9Z3kExf+w1BwR/hqVip2wj+W3mxBwTWgm5hhsko1TZqr3d+HWPAeaFmaNTmwuG0miPhA8H9C4/V0mUs62V2zJkZEVP3QEipvTvkCyctxq7U89NSLwVIGiEsmFG/sZ1EqXnXpmpbV1PQ7pkDYFad+pzQ==', 'sMQAck67hWo2asVpqlbmmGVFj3jo6i86oZVTQh3c3wOpKd0LO8oxqSYhveceXkLrXlCKIIVFB+IRPXrcE3ZkFdVKmG5A7gOyvWwkOltwOytSDoPHmT3+aWUS0sFjO89RMbJxCncsghBbtF3a9hHtr/7/NcexUj8wJQz48gq6izw=', 'argon2id');