#ifndef MOCK_DATA_GENERATOR_H
#define MOCK_DATA_GENERATOR_H
#include "mock_data_generator.h"
#include <Arduino.h>

TemperatureDto generateMockTemperature();
HumidityDto generateMockHumidity();
Particle25Dto generateMockParticle25();
Particle100Dto generateMockParticle100();

#endif