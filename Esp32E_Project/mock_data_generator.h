#ifndef MOCK_DATA_GENERATOR_H
#define MOCK_DATA_GENERATOR_H
#include "mock_data_generator.h"
#include <Arduino.h>

SensorDto generateMockTemperature();
SensorDto generateMockHumidity();
SensorDto generateMockParticle25();
SensorDto generateMockParticle100();

#endif