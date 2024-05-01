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
        ss << "{\n";
        ss << "  \"DeviceId\": " << deviceId << ",\n";
        ss << "  \"Data\": {\n";
        ss << "    \"Temperatures\": [\n";
        serializeSensorArray(ss, data.Temperatures);
        ss << "    ],\n";
        ss << "    \"Humidities\": [\n";
        serializeSensorArray(ss, data.Humidities);
        ss << "    ],\n";
        ss << "    \"Particles25\": [\n";
        serializeSensorArray(ss, data.Particles25);
        ss << "    ],\n";
        ss << "    \"Particles100\": [\n";
        serializeSensorArray(ss, data.Particles100);
        ss << "    ]\n";
        ss << "  }\n";
        ss << "}\n";
        return ss.str();
    }

private:
    int deviceId;
    DeviceReadingsDto data;
void serializeSensorArray(std::stringstream& ss, const std::vector<SensorDto>& sensors) const {
        bool isFirst = true;
        for (const auto& sensor : sensors) {
            if (!isFirst) {
                ss << ",\n";
            }
            isFirst = false;
            ss << "      {\n";
            ss << "        \"Value\": " << sensor.Value << ",\n";
            ss << "        \"TimeStamp\": \"" << sensor.TimeStamp << "\"\n";
            ss << "      }";
        }
        ss << "\n";
        
    }
};

#endif
