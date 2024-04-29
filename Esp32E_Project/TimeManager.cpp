#include "TimeManager.h"
#include <RTClib.h>

TimeManager::TimeManager(const char* ssid, const char* password) : _ssid(ssid), _password(password), _timeClient(_ntpUDP, "pool.ntp.org", GMT_OFFSET_SECONDS) {}

void TimeManager::begin() {
  // Opret forbindelse til Wi-Fi
  Serial.println("Connecting to WiFi...");
  WiFi.begin(_ssid, _password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nConnected to WiFi network");

  // Hent tid fra NTP-serveren
  _timeClient.begin();
  _timeClient.update();

  // Afbryd Wi-Fi-forbindelsen
  WiFi.disconnect(true);
  Serial.println("Disconnected from WiFi");
}

std::string TimeManager::getCurrentTime() {
    // Få det aktuelle tidspunkt
    time_t now = _timeClient.getEpochTime();

    // Konverter tidspunktet til en struct tm
    struct tm *timeinfo;
    timeinfo = localtime(&now);

    // Opret en buffer til at holde den formaterede timestamp
    char buffer[40]; // Antallet af tegn skal være tilstrækkeligt til at rumme den formaterede timestamp

    // Formater tidspunktet som en timestamp med både dato og år
    strftime(buffer, sizeof(buffer), "%Y-%m-%d %H:%M:%S", timeinfo);

    // Konverter bufferen til en std::string og returner den
    return std::string(buffer);
}
