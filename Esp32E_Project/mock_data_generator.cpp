#include "response_dto.h"
#include "mock_data_generator.h"
#include <Arduino.h>
SensorDto generateMockTemperature() {
    SensorDto temp;
    temp.Value = 25.5; // Replace with actual temperature reading logic
    temp.TimeStamp = "feee"; // Using millis() for simplicity
    return temp;
}

SensorDto generateMockHumidity() {
    SensorDto hum;
    hum.Value = 60.0; // Replace with actual humidity reading logic
    hum.TimeStamp = "ndew"; // Using millis() for simplicity
    return hum;
}

SensorDto generateMockParticle25() {
    SensorDto particle;
    particle.Value = 50; // Replace with actual particle 2.5 reading logic
    particle.TimeStamp = "fnejkw"; // Using millis() for simplicity
    return particle;
}

SensorDto generateMockParticle100() {
    SensorDto particle;
    particle.Value = 20; // Replace with actual particle 10 reading logic
    particle.TimeStamp = "feopwf"; // Using millis() for simplicity
    return particle;
}
