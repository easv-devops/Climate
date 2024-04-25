#include <ArduinoJson.h>
#include <vector>
#include <Wire.h>
#include <RTClib.h> // RTC library for ESP32

RTC_DS3231 rtc; // Initialize RTC object

// Define the structures
struct TemperatureDto {
    double Temperature;
    unsigned long TimeStamp; // Using unsigned long for timestamp (milliseconds)
};

struct HumidityDto {
    double Humidity;
    unsigned long TimeStamp; // Using unsigned long for timestamp (milliseconds)
};

struct Particle25Dto {
    int Particle25;
    unsigned long TimeStamp; // Using unsigned long for timestamp (milliseconds)
};

struct Particle100Dto {
    int Particle100;
    unsigned long TimeStamp; // Using unsigned long for timestamp (milliseconds)
};

struct DeviceReadingsDto {
    int DeviceId;
    std::vector<TemperatureDto> Temperatures;
    std::vector<HumidityDto> Humidities;
    std::vector<Particle25Dto> Particles25;
    std::vector<Particle100Dto> Particles100;
};

// Function to generate mock temperature data
TemperatureDto generateMockTemperature() {
    TemperatureDto temp;
    temp.Temperature = 25.5; // Replace with actual temperature reading logic
    temp.TimeStamp = getTimestamp();
    return temp;
}

// Function to generate mock humidity data
HumidityDto generateMockHumidity() {
    HumidityDto hum;
    hum.Humidity = 60.0; // Replace with actual humidity reading logic
    hum.TimeStamp = getTimestamp();
    return hum;
}

// Function to generate mock particle 2.5 data
Particle25Dto generateMockParticle25() {
    Particle25Dto particle;
    particle.Particle25 = 50; // Replace with actual particle 2.5 reading logic
    particle.TimeStamp = getTimestamp();
    return particle;
}

// Function to generate mock particle 10 data
Particle100Dto generateMockParticle100() {
    Particle100Dto particle;
    particle.Particle100 = 20; // Replace with actual particle 10 reading logic
    particle.TimeStamp = getTimestamp();
    return particle;
}

// Function to get current timestamp from RTC
unsigned long getTimestamp() {
    DateTime now = rtc.now();
    return now.unixtime() * 1000; // Convert Unix timestamp to milliseconds
}

void setup() {
    Serial.begin(9600);

    // Initialize RTC
    if (!rtc.begin()) {
        Serial.println("Couldn't find RTC");
        while (1);
    }

    // Uncomment the following line to set the RTC to the date & time this sketch was compiled
    // rtc.adjust(DateTime(F(__DATE__), F(__TIME__)));

    // Set up sample data
    DeviceReadingsDto readings;
    readings.DeviceId = 1;

    // Generate mock data and add to DeviceReadingsDto
    readings.Temperatures.push_back(generateMockTemperature());
    readings.Humidities.push_back(generateMockHumidity());
    readings.Particles25.push_back(generateMockParticle25());
    readings.Particles100.push_back(generateMockParticle100());

    // Create a JSON document
    DynamicJsonDocument doc(1024);

    // Serialize DeviceReadingsDto to JSON
    serializeJson(doc, readings);

    // Serialize JSON to a string
    String jsonString;
    serializeJson(doc, jsonString);

    // Print JSON string
    Serial.println(jsonString);
}













/**
#include <Arduino.h>

// Interval mellem læsninger (i sekunder)
unsigned long readingInterval = 300; // 5 minutter

// Næste læsetider
unsigned long tempNextReadingTime = 0;
unsigned long humiNextReadingTime = 0;
unsigned long airNextReadingTime = 0;
unsigned long partiNextReadingTime = 0;
unsigned long mqttNextSendingTime = 0;

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
    // Indstil næste læsetid
    tempNextReadingTime = now + readingInterval;
  }

  if (now > humiNextReadingTime) {
    // Læs luftfugtighed og gem i liste
    // Indstil næste læsetid
    humiNextReadingTime = now + readingInterval;
  }

  if (now > airNextReadingTime) {
    // Læs lufttryk og gem i liste
    // Indstil næste læsetid
    airNextReadingTime = now + readingInterval;
  }

  if (now > partiNextReadingTime) {
    // Læs partikelsensor og gem i liste
    // Indstil næste læsetid
    partiNextReadingTime = now + readingInterval;
  }

  if (now > mqttNextSendingTime) {
    // Send information til server
    // Slet gamle data fra lister
    // Indstil næste afsendelsestid
    // Implementer din logik her
    mqttNextSendingTime = now + readingInterval;
  }

  // Beregn tid til næste læsning
  unsigned long nextReadingTime = min(tempNextReadingTime, min(humiNextReadingTime, min(airNextReadingTime, partiNextReadingTime)));

  // Beregn tid til næste deep sleep
  unsigned long sleepSeconds = nextReadingTime - now;

  // Hvis der er tid tilbage før næste læsning, gå i deep sleep
  if (sleepSeconds > 0) {
    setupTimerForSleep(sleepSeconds);
  }
}
*/


















