-- Insert dummy data for Rooms
INSERT INTO Room (RoomName, UserId) VALUES
    ('Living Room', 1),
    ('Bedroom', 1);

-- Insert dummy data for Devices
INSERT INTO Device (DeviceName, RoomId) VALUES
    ('Window', 1),
    ('Door', 1),
    ('Bed', 2),
    ('Closet', 2);

-- Insert dummy data for Temperature Readings
INSERT INTO ReadingTemperature (DeviceId, Timestamp, Temperature) VALUES
    (1, '2024-04-22 08:00:00', 22.5),
    (1, '2024-04-22 09:00:00', 23.0),
    (2, '2024-04-22 08:00:00', 20.0),
    (2, '2024-04-22 09:00:00', 19.5),
    (3, '2024-04-22 08:00:00', 25.0),
    (3, '2024-04-22 09:00:00', 24.5),
    (4, '2024-04-22 08:00:00', 21.5),
    (4, '2024-04-22 09:00:00', 22.0);

-- Insert dummy data for Humidity Readings
INSERT INTO ReadingHumidity (DeviceId, Timestamp, Humidity) VALUES
    (1, '2024-04-22 08:00:00', 40.0),
    (1, '2024-04-22 09:00:00', 42.0),
    (2, '2024-04-22 08:00:00', 55.0),
    (2, '2024-04-22 09:00:00', 50.0),
    (3, '2024-04-22 08:00:00', 35.0),
    (3, '2024-04-22 09:00:00', 38.0),
    (4, '2024-04-22 08:00:00', 60.0),
    (4, '2024-04-22 09:00:00', 58.0);

-- Insert dummy data for Particle 2.5 Readings
INSERT INTO ReadingParticle2_5 (DeviceId, Timestamp, P2_5) VALUES
    (1, '2024-04-22 08:00:00', 15),
    (1, '2024-04-22 09:00:00', 18),
    (2, '2024-04-22 08:00:00', 8),
    (2, '2024-04-22 09:00:00', 9),
    (3, '2024-04-22 08:00:00', 25),
    (3, '2024-04-22 09:00:00', 28),
    (4, '2024-04-22 08:00:00', 12),
    (4, '2024-04-22 09:00:00', 14);

-- Insert dummy data for Particle 10 Readings
INSERT INTO ReadingParticle10 (DeviceId, Timestamp, P10) VALUES
    (1, '2024-04-22 08:00:00', 10),
    (1, '2024-04-22 09:00:00', 12),
    (2, '2024-04-22 08:00:00', 5),
    (2, '2024-04-22 09:00:00', 6),
    (3, '2024-04-22 08:00:00', 20),
    (3, '2024-04-22 09:00:00', 22),
    (4, '2024-04-22 08:00:00', 8),
    (4, '2024-04-22 09:00:00', 10);