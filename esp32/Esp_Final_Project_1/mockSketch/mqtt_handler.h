#ifndef MQTT_HANDLER_H
#define MQTT_HANDLER_H

#include <Arduino.h>
#include <WiFi.h>
#include <PubSubClient.h>
#include "response_dto.h" // Inkluderer response_dto.h her

class MqttHandler {
public:
    MqttHandler(const char* ssid, const char* password, const char* mqttServer, int mqttPort, const char* mqttUser, const char* mqttPassword);

    bool sendDataToBroker(const char* topic, const DeviceReadingsDto& data); // Opdateret signatur

    void disconnect();

private:
    void connectToWifi();
    void connectToBroker();

    WiFiClient espClient;
    PubSubClient client;
    const char* ssid;
    const char* password;
    const char* mqttServer;
    int mqttPort;
    const char* mqttUser;
    const char* mqttPassword;
};

#endif