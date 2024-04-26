#include <time.h>
#include "TimeManager.h"
#include "response_dto.h"
#include "pms_functions.h"
#include "mock_data_generator.h"
#include <Arduino.h>
#include <WiFi.h>
#include <PubSubClient.h>

// Wi-Fi- og MQTT-oplysninger
const char* ssid = "Jep";
const char* password = "damkier1";
const char* mqttServer = "mqtt.flespi.io";
const int mqttPort = 1883;
const char* mqttUser = "FlespiToken iNAXKnfDnzPMgOqTfJkgYGOiYFdBDxhSdvH67RZK7r488rKRNG3EdqgFX9NYSW4T";
const char* mqttPassword = "";

// WiFi- og MQTT-klienter
WiFiClient espClient;
PubSubClient client(espClient);

TimeManager timeManager(ssid, password);


DeviceReadingsDto readings;

const int deviceId = 1;

const int minDeepSleepTime = 60;
unsigned long readingInterval = 15;//todo should have for each sensor

unsigned long tempNextReadingTime = 0;
unsigned long humiNextReadingTime = 0;
unsigned long airNextReadingTime = 0;
unsigned long partiNextReadingTime = 0;
unsigned long mqttNextSendingTime = 0;

void setupTimerForSleep(unsigned long sleepSeconds) {
  esp_sleep_enable_timer_wakeup(sleepSeconds * 1000000);
  esp_deep_sleep_start();
}

void setup() {
  Serial.begin(115200);
  timeManager.begin();
}

void loop() {
  std::string currentTime = timeManager.getCurrentTime();
  // Udskriv den aktuelle tid

  unsigned long now = millis() / 1000;

if (now > tempNextReadingTime) {
    SensorDto temperatureReading = generateMockTemperature();
    temperatureReading.TimeStamp = currentTime; // Overskriv timestamp

    readings.Temperatures.push_back(temperatureReading);

    Serial.println(temperatureReading.TimeStamp.c_str());
    tempNextReadingTime = now + readingInterval;
}

 if (now > humiNextReadingTime) {
    SensorDto humidityReading = generateMockHumidity();
    humidityReading.TimeStamp = currentTime; // Overskriv timestamp
    readings.Humidities.push_back(humidityReading);
    humiNextReadingTime = now + readingInterval;
}

if (now > airNextReadingTime) {
    SensorDto particle25Reading = generateMockParticle25();
    particle25Reading.TimeStamp = currentTime; // Overskriv timestamp
    readings.Particles25.push_back(particle25Reading);
    airNextReadingTime = now + readingInterval;
}

if (now > partiNextReadingTime) {
    SensorDto particle100Reading = generateMockParticle100();
    particle100Reading.TimeStamp = currentTime; // Overskriv timestamp
    readings.Particles100.push_back(particle100Reading);
    partiNextReadingTime = now + readingInterval;
}

  if (now > mqttNextSendingTime) {

    // Opret et DeviceData-objekt
    DeviceData deviceData(deviceId, readings);
    std::string jsonString = deviceData.toJsonString();
    sendDataToBroker("Climate", jsonString.c_str());


    readings.Temperatures.clear();
    readings.Humidities.clear();
    readings.Particles25.clear();
    readings.Particles100.clear();
    mqttNextSendingTime = now + readingInterval;
  }

  unsigned long nextReadingTime = min(tempNextReadingTime, min(humiNextReadingTime, min(airNextReadingTime, partiNextReadingTime)));
  unsigned long sleepSeconds = nextReadingTime - now;

  if (sleepSeconds > minDeepSleepTime || (mqttNextSendingTime - now) > minDeepSleepTime) {
    setupTimerForSleep(sleepSeconds);
  }
}

// Funktion til at oprette forbindelse til Wi-Fi
void connectToWifi() {
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.println("Connecting to WiFi..");
  }
  Serial.println("Connected to the WiFi network");
}

// Funktion til at oprette forbindelse til MQTT-mægleren
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
}

void sendDataToBroker(const char* topic, const char* payload) {
  connectToWifi(); // Opkald til at oprette forbindelse til Wi-Fi
  client.setServer(mqttServer, mqttPort); // Initialisering af MQTT-server og port
  
  if (!client.connected()) {
    connectToBroker();
  }

  client.publish(topic, payload);//set max size in lib if needed
}

