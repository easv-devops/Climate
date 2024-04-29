#ifndef MOCK_DATA_GENERATOR_H
#define MOCK_DATA_GENERATOR_H

#include <Arduino.h>

struct TemperatureDto {
    double Temperature;
    unsigned long TimeStamp; // Using unsigned long for timestamp (milliseconds)
};

struct HumidityDto {
    double Humidity;
    unsigned long TimeStamp; // Using unsigned long for timestamp (milliseconds)
};

struct Particle25Dto {
    int Particle25;
    unsigned long TimeStamp; // Using unsigned long for timestamp (milliseconds)
};

struct Particle100Dto {
    int Particle100;
    unsigned long TimeStamp; // Using unsigned long for timestamp (milliseconds)
};

TemperatureDto generateMockTemperature();
HumidityDto generateMockHumidity();
Particle25Dto generateMockParticle25();
Particle100Dto generateMockParticle100();

#endif