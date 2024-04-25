

#include <WiFi.h>
#include <PubSubClient.h>
#include <iostream>
#include <chrono>
#include <ctime>


const char* ssid = "Ane"; 
const char* password = "qwertyuiop"; 
const char* mqttServer = "mqtt.flespi.io"; 
const int mqttPort = 1883; 
const char* mqttUser = "FlespiToken iNAXKnfDnzPMgOqTfJkgYGOiYFdBDxhSdvH67RZK7r488rKRNG3EdqgFX9NYSW4T"; 
const char* mqttPassword = ""; 

WiFiClient espClient;
PubSubClient client(espClient);

void setup() {
  Serial.begin(115200);
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
  client.publish("esp/Startmesage", "ESP32 is up and running");
}

void loop() {
  ParticleReading particleReader;
  particleReader.deviceId = 32;
  particleReader.timestamp = std::chrono::system_clock::now();
  particleReader.sizeTen = 4;
  particleReader.sizeTen = 5;
  delay(2000);
  sendData(particleReader);

}

class ParticleReading {       // The class
  public:             // Access specifier
    int deviceId;
    auto timestamp;
    int sizeTen;        // Attribute (int variable)
    int sizeTwoAndHalf;  // Attribute (string variable)
};

void sendData(ParticleReading particleReading){
  String reading= particleReading.sizeTwoAndHalf + "";
  client.publish("esp/Climate", (char*)reading.c_str());
}
