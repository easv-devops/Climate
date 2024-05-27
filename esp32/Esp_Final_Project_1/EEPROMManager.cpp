#include "EEPROMManager.h"

EEPROMManager::EEPROMManager(int eepromSize) : EEPROM_SIZE(eepromSize) {}

void EEPROMManager::begin() {
    EEPROM.begin(EEPROM_SIZE);
}

void EEPROMManager::saveThresholds(int threshold1, int threshold2, int threshold3) {
    EEPROM.put(THRESHOLD1_ADDRESS, threshold1);
    EEPROM.put(THRESHOLD2_ADDRESS, threshold2);
    EEPROM.put(THRESHOLD3_ADDRESS, threshold3);
    commit();
}

void EEPROMManager::loadThresholds(int &threshold1, int &threshold2, int &threshold3) {
    EEPROM.get(THRESHOLD1_ADDRESS, threshold1);
    EEPROM.get(THRESHOLD2_ADDRESS, threshold2);
    EEPROM.get(THRESHOLD3_ADDRESS, threshold3);
}

void EEPROMManager::commit() {
    EEPROM.commit();
}
