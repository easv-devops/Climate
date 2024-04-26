#ifndef RESPONSE_DTO_H
#define RESPONSE_DTO_H
#include <Arduino.h>
#include "response_dto.h"
#include <string>
#include <vector>
#include <sstream>

struct SensorDto {
    double Value;
    std::string TimeStamp;  
};
struct DeviceReadingsDto {
    std::vector<SensorDto> Temperatures;
    std::vector<SensorDto> Humidities;
    std::vector<SensorDto> Particles25;
    std::vector<SensorDto> Particles100;
};

class DeviceData {
public:
    DeviceData(int deviceId, const DeviceReadingsDto& data) : deviceId(deviceId), data(data) {}

    std::string toJsonString() const {
        std::stringstream ss;
        ss << "{\"DeviceId\":" << deviceId << ",";
        ss << "\"Data\":{";
        ss << "\"Temperatures\":[";
        serializeSensorArray(ss, data.Temperatures);
        ss << "],\"Humidities\":[";
        serializeSensorArray(ss, data.Humidities);
        ss << "],\"Particles25\":[";
        serializeSensorArray(ss, data.Particles25);
        ss << "],\"Particles100\":[";
        serializeSensorArray(ss, data.Particles100);
        ss << "]}";
        return ss.str();
    }

private:
    int deviceId;
    DeviceReadingsDto data;
    void serializeSensorArray(std::stringstream& ss, const std::vector<SensorDto>& sensors) const {
        bool isFirst = true;
        for (const auto& sensor : sensors) {
            if (!isFirst) {
                ss << ",";
            }
            isFirst = false;
            ss << "{\"Value\":" << sensor.Value << ",\"TimeStamp\":\"" << sensor.TimeStamp << "\"}";
        }
    }
};

#endif
