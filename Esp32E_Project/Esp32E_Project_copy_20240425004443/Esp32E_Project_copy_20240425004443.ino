#include "pms_functions.h"
#include "mock_data_generator.h" // inkluder mock data generator header filen skal slettes når alt kører
#include <WiFi.h>
#include "mqtt_handler.h"


#include <WiFi.h>
#include <Arduino.h>
#include <PubSubClient.h>
#include <iostream>
#include <chrono>
#include <ctime>
#include <vector> // inkluder vector biblioteket

struct DeviceReadingsDto {
    std::vector<TemperatureDto> Temperatures;
    std::vector<HumidityDto> Humidities;
    std::vector<Particle25Dto> Particles25;
    std::vector<Particle100Dto> Particles100;
};

//todo should be loaded from eeprom later
const char* ssid = "Ane"; 
const char* password = "qwertyuiop"; 
const char* mqttServer = "mqtt.flespi.io"; 
const int mqttPort = 1883; 
const char* mqttUser = "FlespiToken iNAXKnfDnzPMgOqTfJkgYGOiYFdBDxhSdvH67RZK7r488rKRNG3EdqgFX9NYSW4T"; 
const char* mqttPassword = ""; 

//handler for creating wifi and mqtt connection + sending objects
MqttHandler mqttHandler(ssid, password, mqttServer, mqttPort, mqttUser, mqttPassword);

const int minDeepSleepTime = 60; //minimum sleep time for at esp går i deep sleep i sekunder

// Interval mellem læsninger (i sekunder) burde laves om så der er en for hver reading
unsigned long readingInterval = 300; // 5 minutter

// Næste læsetider
unsigned long tempNextReadingTime = 0;
unsigned long humiNextReadingTime = 0;
unsigned long airNextReadingTime = 0;
unsigned long partiNextReadingTime = 0;
unsigned long mqttNextSendingTime = 0;

DeviceReadingsDto readings; // Opret et globalt DeviceReadingsDto objekt

// Funktion til at indstille timeren til deep sleep
void setupTimerForSleep(unsigned long sleepSeconds) {
  esp_sleep_enable_timer_wakeup(sleepSeconds * 1000000);
  esp_deep_sleep_start();
}

void setup() {
  Serial.begin(115200);
}

void loop() {
  unsigned long now = millis() / 1000; // Konverter millisekunder til sekunder

  if (now > tempNextReadingTime) {
    // Læs temperatur og gem i liste
    TemperatureDto tempData = generateMockTemperature();
    readings.Temperatures.push_back(tempData); // Tilføj temperatur til DeviceReadingsDto objekt
    // Indstil næste læsetid
    tempNextReadingTime = now + readingInterval;
  }

  if (now > humiNextReadingTime) {
    // Læs luftfugtighed og gem i liste
    HumidityDto humData = generateMockHumidity();
    readings.Humidities.push_back(humData); // Tilføj luftfugtighed til DeviceReadingsDto objekt
    // Indstil næste læsetid
    humiNextReadingTime = now + readingInterval;
  }

  if (now > airNextReadingTime) {
    // Læs lufttryk og gem i liste
    Particle25Dto airData = generateMockParticle25();
    readings.Particles25.push_back(airData); // Tilføj lufttryk til DeviceReadingsDto objekt
    // Indstil næste læsetid
    airNextReadingTime = now + readingInterval;
  }

  if (now > partiNextReadingTime) {
    // Læs partikelsensor og gem i liste
    Particle100Dto partiData = generateMockParticle100();
    readings.Particles100.push_back(partiData); // Tilføj partikelsensor til DeviceReadingsDto objekt
    // Indstil næste læsetid
    partiNextReadingTime = now + readingInterval;
  }

  if (now > mqttNextSendingTime) {
    // Send DeviceReadingsDto object to server
    mqttHandler.sendDataToBroker("Climate/test", readings);
    
    // Clear old data from lists
    readings.Temperatures.clear();
    readings.Humidities.clear();
    readings.Particles25.clear();
    readings.Particles100.clear();

    //reset timer
    mqttNextSendingTime = now + readingInterval;
  }

  // Beregn tid til næste læsning
  unsigned long nextReadingTime = min(tempNextReadingTime, min(humiNextReadingTime, min(airNextReadingTime, partiNextReadingTime)));

  // Beregn tid til næste deep sleep
  unsigned long sleepSeconds = nextReadingTime - now;

  if (sleepSeconds > minDeepSleepTime || (mqttNextSendingTime - now) > minDeepSleepTime) {
    setupTimerForSleep(sleepSeconds);
  }
}


/**
WiFiClient espClient;
PubSubClient client(espClient);
int sendDataWhen10 = 0;



void setup() {
  Serial.begin(9600);  // Initialize serial communication for debugging
  connectToWifi();
  connectToBroker();
  delay(5000);
  setupPMS5003Sensor(26, 25);
}

void loop() {

  //ParticleData part_data = readParticels();
  //part_data.printData();

  delay(2000);
  printCounterAndCount();
  //When to send
  if (sendDataWhen10 % 10 == 0) {
    sendDataWhen10 = 0;
    TemperaturData temperaturdata(sendDataWhen10);
    ParticleData testParticleData(1, 2, 3);
    HumidityData testHumidityData(sendDataWhen10);
    //sendDataToBroker(temperaturdata.toJSON());
    sendDataListToBroker(temperaturdata.toJSON(), testParticleData.toJSON25(), testHumidityData.toJSON());
  }
}

void connectToWifi() {
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.println("Connecting to WiFi..");
  }
  Serial.println("Connected to the WiFi network");
  client.setServer(mqttServer, mqttPort);
}
void connectToBroker() {
  while (!client.connected()) {
    Serial.println("Connecting to MQTT...");

    if (client.connect("ESP32Client", mqttUser, mqttPassword)) {
      Serial.println("connected");
    } else {

      Serial.print("failed with state ");
      Serial.print(client.state());
      delay(2000);
    }
  }
  client.publish("Climate/Startmesage", "ESP32 is up and running");//todo should be deleted when live
}

void sendDataToBroker(String jsonObject) {
  client.publish("Climate/test", (char*)jsonObject.c_str());
}


void sendDataListToBroker(String jsonObject1, String jsonObject2, String jsonObject3) {

  // Create a JSON document
  StaticJsonDocument<256> doc;
  //creating a root object
  JsonObject tempRoot = doc.to<JsonObject>();
  tempRoot["allTemperatures"] = "living room";

  // Create JSON objects to be included in the main JSON object
  JsonObject obj1 = tempRoot.createNestedObject(jsonObject1);
  JsonObject obj2 = tempRoot.createNestedObject(jsonObject2);
  JsonObject obj3 = tempRoot.createNestedObject(jsonObject3);

  String Serial;
  serializeJsonPretty(tempRoot, Serial);
  client.publish("Climate/test", (char*)Serial.c_str());
}

void printCounterAndCount() {
  sendDataWhen10++;  //counter
  Serial.print("RandomNumber:  ");
  Serial.print(sendDataWhen10);
}
*/