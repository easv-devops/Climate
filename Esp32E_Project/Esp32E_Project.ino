#include "pms_functions.h"
#include <WiFi.h>
#include <PubSubClient.h>


const char* ssid = "Jep";
const char* password =  "damkier1";
const char* mqttServer = "mqtt.flespi.io";
const int mqttPort = 1883;
const char* mqttUser = "FlespiToken cOcCuPKsMOfE1tsj5X3bjwsSUOWvGfkIRl8bYHuAjz3aQlhL68Nfu5VcHHyeKGKr";
const char* mqttPassword = "";



void setup() {
  Serial.begin(9600); // Initialize serial communication for debugging
  setupPMS5003Sensor(26, 25);

    
    
// template on setup wifi conn
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.println("Connecting to WiFi..");
  }

  Serial.println("Connected to the WiFi network");

  client.setServer(mqttServer, mqttPort);
 
  while (!client.connected()) {
    Serial.println("Connecting to MQTT...");
 
    if (client.connect("ESP32Client", mqttUser, mqttPassword )) {
 
      Serial.println("connected");
 
    } else {
      Serial.print("failed with state ");
      Serial.print(client.state());
      delay(2000);

    }
  }
}

void loop() {
   ParticleData part_data = readParticels();
   part_data.printData();
  
  delay(1000);
}


