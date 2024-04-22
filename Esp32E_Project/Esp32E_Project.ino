#include "pms_functions.h"


void setup() {
  Serial.begin(9600); // Initialize serial communication for debugging

  setupPMS5003Sensor(26, 25);

}

void loop() {
   ParticleData part_data = readParticels();
   part_data.printData();

  delay(1000);
}
