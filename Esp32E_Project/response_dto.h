#ifndef RESPONSE_DTO_H
#define RESPONSE_DTO_H

#include <Arduino_JSON.h> // Inkluder Arduino_JSON headerfilen
#include <vector>

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
    std::vector<TemperatureDto> Temperatures;
    std::vector<HumidityDto> Humidities;
    std::vector<Particle25Dto> Particles25;
    std::vector<Particle100Dto> Particles100;

   void serializeJson(JSONVar& jsonData) const {
    // Serialize Temperatures
    JSONVar temperaturesArray;
    for(const auto& temperature : Temperatures) {
        JSONVar tempObj;
        tempObj["Temperature"] = temperature.Temperature;
        tempObj["TimeStamp"] = temperature.TimeStamp;
        temperaturesArray[temperaturesArray.length()] = tempObj;
    }

    jsonData["Temperatures"] = temperaturesArray;

    // Serialize Humidities
    JSONVar humiditiesArray;
    for(const auto& humidity : Humidities) {
        JSONVar humObj;
        humObj["Humidity"] = humidity.Humidity;
        humObj["TimeStamp"] = humidity.TimeStamp;
        humiditiesArray[humiditiesArray.length()] = humObj;
    }

    jsonData["Humidities"] = humiditiesArray;

    // Serialize Particles25
    JSONVar particles25Array;
    for(const auto& particle : Particles25) {
        JSONVar particleObj;
        particleObj["Particle25"] = particle.Particle25;
        particleObj["TimeStamp"] = particle.TimeStamp;
        particles25Array[particles25Array.length()] = particleObj;
    }

    jsonData["Particles25"] = particles25Array;

    // Serialize Particles100
    JSONVar particles100Array;
    for(const auto& particle : Particles100) {
        JSONVar particleObj;
        particleObj["Particle100"] = particle.Particle100;
        particleObj["TimeStamp"] = particle.TimeStamp;
        particles100Array[particles100Array.length()] = particleObj;
    }

    jsonData["Particles100"] = particles100Array;
}

};

class DeviceData {
public:
    DeviceData(int deviceId, const DeviceReadingsDto& data) : deviceId(deviceId), data(data) {}

    int getDeviceId() const {
        return deviceId;
    }

    const DeviceReadingsDto& getData() const {
        return data;
    }

    void setDeviceId(int newDeviceId) {
        deviceId = newDeviceId;
    }

    void setData(const DeviceReadingsDto& newData) {
        data = newData;
    }

    void serializeJson(JSONVar& jsonData) const {
        // Set DeviceId
        jsonData["DeviceId"] = deviceId;

        // Serialize Data
        JSONVar dataObj;
        data.serializeJson(dataObj);

        jsonData["Data"] = dataObj;
    }

private:
    int deviceId;
    DeviceReadingsDto data;
};

#endif


