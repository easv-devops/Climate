#include "WString.h"
#ifndef SENSOR_READINGS_H
#define SENSOR_READINGS_H
#include <Arduino.h>

// Class to hold particle data for the PMS5003 sensor 
class ParticleData {
public:
    ParticleData(int pm10, int pm25, int pm100) : pm10(pm10), pm25(pm25), pm100(pm100) {}
    void printData(); // Method declaration inside the class
    String toJSON25();
    String toJSON100();

    int getPM10() const { return pm10; }
    int getPM25() const { return pm25; }
    int getPM100() const { return pm100; }

private:
    int pm10;
    int pm25;
    int pm100;
};

void setupPMS5003Sensor(int tx, int rx);//starts the serial connection to the PMS5003 through serial 2
 
ParticleData readParticels();//reads the sensor value once and returns it (should only be used for testing)

ParticleData getAverageParticleData(int numReadings); //gets the average partical readings x amount of times (takes 1 second pr reading..)

#endif 