#include "pms_functions.h"
#include <WiFi.h>
#include <PubSubClient.h>
#include <iostream>
#include <chrono>
#include <ctime>
#include <ArduinoJson.h> // Include the ArduinoJson library



const char* ssid = "Ane"; 
const char* password = "qwertyuiop"; 
const char* mqttServer = "mqtt.flespi.io"; 
const int mqttPort = 1883; 
const char* mqttUser = "FlespiToken iNAXKnfDnzPMgOqTfJkgYGOiYFdBDxhSdvH67RZK7r488rKRNG3EdqgFX9NYSW4T"; 
const char* mqttPassword = ""; 

WiFiClient espClient;
PubSubClient client(espClient);
int sendDataWhen10 = 0;


void setup() {
  Serial.begin(9600); // Initialize serial communication for debugging
  connectToWifi();
  connectToBroker();
  delay(5000);
  setupPMS5003Sensor(26, 25);
}

void loop() {
 
  ParticleData part_data = readParticels();
  part_data.printData();
  delay(500);
  sendDataWhen10 = sendDataWhen10 + 1;
  Serial.print("RandomNumber:  ");
  Serial.print(sendDataWhen10);
  if(sendDataWhen10 % 10 == 0){
    TemperaturData temperaturdata(sendDataWhen10);
    sendDataWhen10 = 0;
    sendDataToBroker(temperaturdata.toJSON());
    sendDataListToBroker(temperaturdata.toJSON());
  }

}

void connectToWifi(){  
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.println("Connecting to WiFi..");
  }
  Serial.println("Connected to the WiFi network"); 
  client.setServer(mqttServer, mqttPort);
}
void connectToBroker(){
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
  client.publish("Climate/Startmesage", "ESP32 is up and running");
}

void sendDataToBroker(String jsonObject){
  client.publish("Climate/test", (char*)jsonObject.c_str());
}

void sendDataListToBroker(String jsonObject){
  //create a new JSON object within a parent JSON object.

  // Create a JSON document
  StaticJsonDocument<256> doc;
  //creating a root object
  JsonObject tempRoot = doc.to<JsonObject>();
  tempRoot["allTemperatures"] = "living room";

  // Create JSON objects to be included in the main JSON object
  JsonObject obj1 = tempRoot.createNestedObject(jsonObject);

  String Serial;
  serializeJson(tempRoot, Serial);

  client.publish("Climate/test", (char*)Serial.c_str());
}

