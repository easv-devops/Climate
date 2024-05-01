#include <time.h>
#include "TimeManager.h"
#include "response_dto.h"
#include "pms_functions.h"
#include <Arduino.h>
#include <WiFi.h>
#include <PubSubClient.h>
#include <Adafruit_BME280.h>


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

//bme sensor 
Adafruit_BME280 bme;

TimeManager timeManager(ssid, password);

DeviceReadingsDto readings;

const int deviceId = 1;

const int minDeepSleepTime = 10;
unsigned long readingInterval = 120;//todo should have for each sensor

unsigned long tempNextReadingTime = 0;
unsigned long humiNextReadingTime = 0;
unsigned long airNextReadingTime = 0;
unsigned long partiNextReadingTime = 0;
unsigned long mqttNextSendingTime = 0;

void setupTimerForSleep(unsigned long sleepSeconds) {
  Serial.print("gpoing to sleep for ");
  Serial.println(sleepSeconds);

  esp_sleep_enable_timer_wakeup(sleepSeconds * 1000000);
  esp_deep_sleep_start();
}


void setupESP() {
  Serial.begin(115200);
  timeManager.begin();

  if (!bme.begin(0x76)) {
    Serial.println("Could not find a valid BME280 sensor, check wiring!");
    while (1);
  } else {
    Serial.println("started BME280 sensor");
  }
}

// Denne funktion kaldes, når ESP32 vågner fra dyb søvn
void IRAM_ATTR esp32_wake_up() {
  Serial.println("Waking up, And running setup");
  setupESP();
}

void setup() {
  setupESP();
}

void loop() {
  std::string currentTime = timeManager.getCurrentTime();//gets current real time
  unsigned long now = millis() / 1000; //times when next reading in seconds

  // Læs temperaturfra BME280-sensor
if (now > tempNextReadingTime) {
  Serial.println("Start reading temp");

  float temperature = bme.readTemperature();
    // Gem målingerne i readings-objektet
  SensorDto temperatureReading;
  temperatureReading.Value = temperature;
  temperatureReading.TimeStamp = currentTime;
  readings.Temperatures.push_back(temperatureReading);
  tempNextReadingTime = now + readingInterval;
  Serial.println("End reading temp");

}

 if (now > humiNextReadingTime) {
  Serial.println("Start reading Humidity");
  
  float humidity = bme.readHumidity();

  SensorDto humidityReading;
  humidityReading.Value = humidity;
  humidityReading.TimeStamp = currentTime;
  readings.Humidities.push_back(humidityReading);
    humiNextReadingTime = now + readingInterval;

  Serial.println("End reading Humidity");

}

if (now > airNextReadingTime) {

  Serial.println("Start reading Particles");

  setupPMS5003Sensor(25, 26); // Start PMS5003-sensoren
  delay(5000); // Vent i 5 sekunder for at lade sensoren stabilisere

  ParticleData avgData = getAverageParticleData(10); // Tag 10 aflæsninger og beregn gennemsnittet samt slukker sensor
  // Gem partikelmålinger
  SensorDto particle25Reading;
  particle25Reading.Value = avgData.getPM25();
  particle25Reading.TimeStamp = currentTime;
  readings.Particles25.push_back(particle25Reading);

  SensorDto particle100Reading;
  particle100Reading.Value = avgData.getPM100();
  particle100Reading.TimeStamp = currentTime;
  readings.Particles100.push_back(particle100Reading);

  airNextReadingTime = now + readingInterval;

  Serial.println("End reading Particles");
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

unsigned long nextReadingTime = min(tempNextReadingTime, min(humiNextReadingTime, airNextReadingTime));
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
  Serial.println("Dto Send");
  Serial.println("");

  WiFi.disconnect(true);
  Serial.println("Disconnected from WiFi");
}

