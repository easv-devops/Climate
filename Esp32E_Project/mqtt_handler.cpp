#include "mqtt_handler.h"
#include "response_dto.h"
#include <Arduino_JSON.h> // Inkluder Arduino_JSON headerfilen

MqttHandler::MqttHandler(const char* ssid, const char* password, const char* mqttServer, int mqttPort, const char* mqttUser, const char* mqttPassword)
    : ssid(ssid), password(password), mqttServer(mqttServer), mqttPort(mqttPort), mqttUser(mqttUser), mqttPassword(mqttPassword), client(espClient) {}

void MqttHandler::connectToWifi() {
    WiFi.begin(ssid, password);
    while (WiFi.status() != WL_CONNECTED) {
        delay(500);
        Serial.println("Connecting to WiFi..");
    }
    Serial.println("Connected to the WiFi network");
}

void MqttHandler::connectToBroker() {
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
    client.publish("Climate/Startmesage", "ESP32 is up and running"); //todo should be deleted when live
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
