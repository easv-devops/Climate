#include <EEPROM.h>

const int EEPROM_SIZE = 4096;  // Assuming EEPROM size is 4096 bytes

// Struct for key-value pair
struct KeyValuePair {
  char key[20];   // Max length of the key (adjust according to your needs)
  int value;      // The value associated with the key
};

// Funktion til at gemme et key-value pair
void saveKeyValuePair(const char* key, int value) {
  // Beregn adresse baseret på nøglen
  int address = 0;  // Startadresse
  for (int i = 0; key[i] != '\0'; ++i) {
    address += key[i];
  }
  
  // Gem key-value par (overskriv eksisterende værdi, hvis nøglen allerede findes)
  KeyValuePair kvp;
  EEPROM.get(address, kvp);
  strcpy(kvp.key, key);
  kvp.value = value;
  EEPROM.put(address, kvp);
  EEPROM.commit();
}

// Funktion til at indlæse værdien ud fra en nøgle
int loadValueByKey(const char* key) {
  // Beregn adresse baseret på nøglen
  int address = 0;  // Startadresse
  for (int i = 0; key[i] != '\0'; ++i) {
    address += key[i];
  }
  
  // Indlæs værdi der svarer til nøglen
  KeyValuePair kvp;
  EEPROM.get(address, kvp);
  return kvp.value;
}

// Funktion til at gemme SSID og adgangskode
void saveSSIDAndPassword(const char* ssid, const char* password) {
  // Skriv SSID til EEPROM
  int address = EEPROM_SIZE - 64; // Startadresse til SSID 
  for (int i = 0; ssid[i] != '\0'; ++i) {
    EEPROM.write(address++, ssid[i]);
  }
  EEPROM.write(address++, '\0');  // Null-terminator

  // Skriv adgangskode til EEPROM
  for (int i = 0; password[i] != '\0'; ++i) {
    EEPROM.write(address++, password[i]);
  }
  EEPROM.write(address++, '\0');  // Null-terminator

  EEPROM.commit();
}

// Funktion til at indlæse SSID og adgangskode
void loadSSIDAndPassword(char* ssid, char* password) {
  // Indlæs SSID fra EEPROM
  int address = EEPROM_SIZE - 64; // Startadresse til SSID 
  char ch;
  do {
    ch = EEPROM.read(address++);
    *ssid++ = ch;
  } while (ch != '\0');

  // Indlæs adgangskode fra EEPROM
  do {
    ch = EEPROM.read(address++);
    *password++ = ch;
  } while (ch != '\0');
}
