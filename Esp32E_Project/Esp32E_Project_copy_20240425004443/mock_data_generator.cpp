#include "mock_data_generator.h"

TemperatureDto generateMockTemperature() {
    TemperatureDto temp;
    temp.Temperature = 25.5; // Replace with actual temperature reading logic
    temp.TimeStamp = millis(); // Using millis() for simplicity
    return temp;
}

HumidityDto generateMockHumidity() {
    HumidityDto hum;
    hum.Humidity = 60.0; // Replace with actual humidity reading logic
    hum.TimeStamp = millis(); // Using millis() for simplicity
    return hum;
}

Particle25Dto generateMockParticle25() {
    Particle25Dto particle;
    particle.Particle25 = 50; // Replace with actual particle 2.5 reading logic
    particle.TimeStamp = millis(); // Using millis() for simplicity
    return particle;
}

Particle100Dto generateMockParticle100() {
    Particle100Dto particle;
    particle.Particle100 = 20; // Replace with actual particle 10 reading logic
    particle.TimeStamp = millis(); // Using millis() for simplicity
    return particle;
}