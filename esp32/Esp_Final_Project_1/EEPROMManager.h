#ifndef EEPROM_MANAGER_H
#define EEPROM_MANAGER_H

#include <EEPROM.h>

class EEPROMManager {
public:
    EEPROMManager(int eepromSize);
    void begin();

    void saveThresholds(int threshold1, int threshold2, int threshold3);
    void loadThresholds(int &threshold1, int &threshold2, int &threshold3);

private:
    const int EEPROM_SIZE;
    const int THRESHOLD1_ADDRESS = 0;
    const int THRESHOLD2_ADDRESS = sizeof(int);
    const int THRESHOLD3_ADDRESS = sizeof(int) * 2;

    void commit();
};

#endif // EEPROM_MANAGER_H
