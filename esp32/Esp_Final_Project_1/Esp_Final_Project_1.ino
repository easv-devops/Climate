#include <time.h>
#include "TimeManager.h"
#include "response_dto.h"
#include "pms_functions.h"
#include <Arduino.h>
#include "EEPROMManager.h"
#include <WiFi.h>
#include <PubSubClient.h>
#include <Adafruit_BME280.h>
#include <ArduinoJson.h>

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

// BME280 sensor
Adafruit_BME280 bme;

// EEPROM manager
EEPROMManager eepromManager(4096);

// Time manager
TimeManager timeManager(ssid, password);

DeviceReadingsDto readings;

const int deviceId = 1;

const int minDeepSleepTime = 10;
unsigned long readingInterval = 120; // Tidsinterval for læsninger i sekunder

unsigned long tempNextReadingTime = 0;
unsigned long humiNextReadingTime = 0;
unsigned long airNextReadingTime = 0;
unsigned long mqttNextSendingTime = 0;

void setupTimerForSleep(unsigned long sleepSeconds) {
    Serial.print("Going to sleep for ");
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
        Serial.println("Started BME280 sensor");
    }
}

// Denne funktion kaldes, når ESP32 vågner fra dyb søvn
void IRAM_ATTR esp32_wake_up() {
    Serial.println("Waking up, and running setup");
    setupESP();
}

void setup() {
    setupESP();

    eepromManager.begin();

    // Indlæs gemte threshold-værdier fra EEPROM
    int loadedThreshold1, loadedThreshold2, loadedThreshold3;
    eepromManager.loadThresholds(loadedThreshold1, loadedThreshold2, loadedThreshold3);

    tempNextReadingTime = loadedThreshold1;
    humiNextReadingTime = loadedThreshold1;
    airNextReadingTime = loadedThreshold2;
    mqttNextSendingTime = loadedThreshold3;

    // Print de indlæste værdier fra EEPROM
    Serial.print("temp interval : ");
    Serial.println(loadedThreshold1);
    Serial.print("part interval : ");
    Serial.println(loadedThreshold2);
    Serial.print("mqtt interval: ");
    Serial.println(loadedThreshold3);

    connectToWifi();
    client.setServer(mqttServer, mqttPort);
    client.setCallback(mqttCallback);

    connectToBroker();
}

void loop() {
    client.loop();  // Process incoming messages

    std::string currentTime = timeManager.getCurrentTime(); // Gets current real time
    unsigned long now = millis() / 1000; // Tidsmærke i sekunder

    // Læs temperatur fra BME280-sensor
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

        // Tjek efter nye indstillinger
        checkForSettingsUpdate();
    }

    unsigned long nextReadingTime = min(tempNextReadingTime, min(humiNextReadingTime, airNextReadingTime));
    unsigned long sleepSeconds = nextReadingTime - now;

    if (sleepSeconds > minDeepSleepTime || (mqttNextSendingTime - now) > minDeepSleepTime) {
        setupTimerForSleep(sleepSeconds);
    }
}

// MQTT callback function to handle incoming messages
void mqttCallback(char* topic, byte* payload, unsigned int length) {
    // Convert payload to string
    String message;
    for (unsigned int i = 0; i < length; i++) {
        message += (char)payload[i];
    }
    Serial.print("Message arrived [");
    Serial.print(topic);
    Serial.print("] ");
    Serial.println(message);

    // Parse the message to extract thresholds
    // Assuming the message is in JSON format like: {"tempInterval": 100, "humiInterval": 200, "partInterval": 300}
    DynamicJsonDocument doc(256);
    DeserializationError error = deserializeJson(doc, message);
    if (error) {
        Serial.print("deserializeJson() failed: ");
        Serial.println(error.f_str());
        return;
    }

    int tempInterval = doc["tempInterval"];
    int humiInterval = doc["humiInterval"];
    int partInterval = doc["partInterval"];

    // Save the new thresholds to EEPROM
    eepromManager.saveThresholds(tempInterval, humiInterval, partInterval);

    // Update the intervals
    tempNextReadingTime = tempInterval;
    humiNextReadingTime = humiInterval;
    airNextReadingTime = partInterval;
    mqttNextSendingTime = partInterval;

    Serial.println("Updated intervals from MQTT message:");
    Serial.print("temp interval : ");
    Serial.println(tempInterval);
    Serial.print("humi interval : ");
    Serial.println(humiInterval);
    Serial.print("part interval : ");
    Serial.println(partInterval);
}

// Tjek efter nye indstillinger på MQTT-topic "Climate/1/settings"
void checkForSettingsUpdate() {
    client.subscribe("Climate/1/settings");
    delay(500); // Vent kort for at sikre, at beskeder modtages

    client.loop();  // Process incoming messages

    client.unsubscribe("Climate/1/settings");
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
    connectToWifi(); // Opret forbindelse til Wi-Fi
    client.setServer(mqttServer, mqttPort); // Initialisering af MQTT-server og port
    
    if (!client.connected()) {
        connectToBroker();
    }

    client.publish(topic, payload); // Publiser data til MQTT-mægleren
    Serial.println("Data sent to broker");
    Serial.println("");

    WiFi.disconnect(true);
    Serial.println("Disconnected from WiFi");
}

