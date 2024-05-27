#include "mqtt_handler.h"
#include "response_dto.h"
#include <ArduinoJson.h>

MqttHandler::MqttHandler(const char* ssid, const char* password, const char* mqttServer, int mqttPort, const char* mqttUser, const char* mqttPassword)
    : ssid(ssid), password(password), mqttServer(mqttServer), mqttPort(mqttPort), mqttUser(mqttUser), mqttPassword(mqttPassword), client(espClient) {}

void MqttHandler::connectToWifi() {
    Serial.println("Connecting to WiFi...");
    Serial.print("SSID: ");
    Serial.println(ssid);
    Serial.print("Password: ");
    Serial.println(password);

    WiFi.begin(ssid, password);
    while (WiFi.status() != WL_CONNECTED) {
        delay(500);
        Serial.print(".");
    }
    Serial.println("Connected to the WiFi network");
}

void MqttHandler::connectToBroker() {
    Serial.println("Connecting to MQTT...");
    Serial.print("MQTT Server: ");
    Serial.println(mqttServer);
    Serial.print("MQTT Port: ");
    Serial.println(mqttPort);
    Serial.print("MQTT User: ");
    Serial.println(mqttUser);
    // Serial.print("MQTT Password: ");  // Omitting printing password for security reasons

    while (!client.connected()) {
        if (client.connect("ESP32Client", mqttUser, "")) {
            Serial.println("Connected to MQTT");
        } else {
            Serial.print("Failed to connect to MQTT with state ");
            Serial.println(client.state());
            delay(2000);
        }
    }
    client.publish("Climate", "ESP32 is up and running"); //todo should be deleted when live
}

bool MqttHandler::sendDataToBroker(const char* topic, const DeviceReadingsDto& data) {
    connectToWifi();
    connectToBroker();

    // Create a JSON object
    JSONVar jsonData;

    // Serialize object to JSON
    data.serializeJson(jsonData);

    // Convert JSON to string
    String jsonString = JSON.stringify(jsonData);
    
    bool result = client.publish(topic, jsonString.c_str());

    disconnect();
    return result; // Return the actual result of the publish operation
}

void MqttHandler::disconnect() {
    client.disconnect();
    WiFi.disconnect(true);
    delay(500);
}
