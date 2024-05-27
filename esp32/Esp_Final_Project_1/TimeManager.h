#ifndef TIME_MANAGER_H
#define TIME_MANAGER_H

#include <WiFi.h>
#include <NTPClient.h>
#include <WiFiUdp.h>

const long GMT_OFFSET_SECONDS = 7200; // GMT offset (in seconds)

class TimeManager {
public:
    TimeManager(const char* ssid, const char* password);
    void begin();
    std::string getCurrentTime();

private:
    const char* _ssid;
    const char* _password;
    WiFiUDP _ntpUDP;
    NTPClient _timeClient;
};

#endif

