-- Create RangeSettings table
CREATE TABLE RangeSettings (
    DeviceId                            INT PRIMARY KEY,
    TemperatureMin                      DECIMAL,
    TemperatureMax                      DECIMAL,
    HumidityMin                         DECIMAL,
    HumidityMax                         DECIMAL,
    Particle25Max                       INT,
    Particle100Max                      INT,
    FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);

-- Create DeviceSettings table
CREATE TABLE DeviceSettings (
    DeviceId                            INT PRIMARY KEY,
    BMP280ReadingInterval               INT,
    PMSReadingInterval                  INT,
    UpdateInterval                      INT,
    FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);