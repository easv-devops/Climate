INSERT INTO RangeSettings (DeviceId, TemperatureMin, TemperatureMax, HumidityMin, HumidityMax, Particle25Max, Particle100Max)
VALUES (
           1,               -- DeviceId
           18.0,             -- TemperatureMin
           22.0,            -- TemperatureMax
           40.0,            -- HumidityMin
           70.0,            -- HumidityMax
           15,              -- Particle25Max
           15              -- Particle100Max
       );

INSERT INTO RangeSettings (DeviceId, TemperatureMin, TemperatureMax, HumidityMin, HumidityMax, Particle25Max, Particle100Max)
VALUES (2, 18.0, 22.0, 40.0, 70.0, 15, 15);
INSERT INTO RangeSettings (DeviceId, TemperatureMin, TemperatureMax, HumidityMin, HumidityMax, Particle25Max, Particle100Max)
VALUES (3, 18.0, 22.0, 40.0, 70.0, 15, 15);
INSERT INTO RangeSettings (DeviceId, TemperatureMin, TemperatureMax, HumidityMin, HumidityMax, Particle25Max, Particle100Max)
VALUES (4, 18.0, 22.0, 40.0, 70.0, 15, 15);

INSERT INTO DeviceSettings (DeviceId, BMP280ReadingInterval, PMSReadingInterval, UpdateInterval)
VALUES (
           1,               -- DeviceId
           1,              -- BMP280ReadingInterval in minutes
           1,              -- PMSReadingInterval in minutes
           5              -- UpdateInterval in minutes
       );

INSERT INTO DeviceSettings (DeviceId, BMP280ReadingInterval, PMSReadingInterval, UpdateInterval) VALUES (2, 1, 1, 5);
INSERT INTO DeviceSettings (DeviceId, BMP280ReadingInterval, PMSReadingInterval, UpdateInterval) VALUES (3, 1, 1, 5);
INSERT INTO DeviceSettings (DeviceId, BMP280ReadingInterval, PMSReadingInterval, UpdateInterval) VALUES (4, 1, 1, 5);
