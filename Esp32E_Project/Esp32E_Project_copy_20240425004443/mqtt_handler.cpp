#include "mqtt_handler.h"

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

template<typename T>
bool MqttHandler::sendDataToBroker(const char* topic, const T& data) {
    connectToWifi();
    connectToBroker();

    // Create a JSON document
    DynamicJsonDocument doc(1024);

    // Serialize object to JSON
    serializeJson(doc, data);

    // Serialize JSON to a string
    String jsonString;
    serializeJson(doc, jsonString);

    bool result = client.publish(topic, jsonString.c_str());
    disconnect();
    return result;
}

void MqttHandler::disconnect() {
    client.disconnect();
    WiFi.disconnect(true);
    delay(500);
}
