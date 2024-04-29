#include "WString.h"
#include "HardwareSerial.h"
#include "pms_functions.h"

/**
* setup serial connection to sensor, and sets the datatype
* starts the serial connection to the PMS5003 through serial 2
* tx = pin 26 on esp32E
* rx = pin 25 on esp32E
*/
void setupPMS5003Sensor(int tx, int rx) {
  Serial2.begin(9600, SERIAL_8N1, tx, rx);  // Initialize Serial2 for PMS5003 (TX = GPIO25, RX = GPIO26)
  // Start PMS5003
  Serial2.write(0x42);
  Serial2.write(0x4D);
}

ParticleData getAverageParticleData(int numReadings) {
  int totalPM10 = 0;
  int totalPM25 = 0;
  int totalPM100 = 0;

  // Read the particle data 'numReadings' times
  for (int i = 0; i < numReadings; i++) {
    ParticleData data = readParticels();
    totalPM10 += data.getPM10();
    totalPM25 += data.getPM25();
    totalPM100 += data.getPM100();
    delay(1000);  // Delay between readings (adjust as needed)
  }

  // Calculate the average values
  int avgPM10 = totalPM10 / numReadings;
  int avgPM25 = totalPM25 / numReadings;
  int avgPM100 = totalPM100 / numReadings;

  // Create a new ParticleData object with the average values
  ParticleData avgData(avgPM10, avgPM25, avgPM100);

  // Send kommando til at slukke for sensoren
  Serial2.write(0x42);
  Serial2.write(0x4D);

  return avgData;
}


// Function to read particle data
ParticleData readParticels() {
  ParticleData particleData(0, 0, 0);

  if (Serial2.available() >= 32) {
    // Read data frame
    unsigned char data[32];
    for (int i = 0; i < 32; i++) {
      data[i] = Serial2.read();
    }

    // Check if the data starts with the correct header bytes
    if (data[0] == 0x42 && data[1] == 0x4D) {
      // Calculate PM1.0, PM2.5, and PM10 values
      int pm10 = (data[4] << 8) | data[5];
      int pm25 = (data[6] << 8) | data[7];
      int pm100 = (data[8] << 8) | data[9];

      // Update particle data object
      particleData = ParticleData(pm10, pm25, pm100);
    }
  }

  return particleData;
}

// Method to print particle data implementation
void ParticleData::printData() {
  Serial.print("\t\tPM1.0: ");
  Serial.print(pm10);
  Serial.print("\t\tPM2.5: ");
  Serial.print(pm25);
  Serial.print("\t\tPM10: ");
  Serial.println(pm100);
}