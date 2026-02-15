/*
 * ESP32 (Nano or Dev) + I2C LCD — Game status display
 * Receives UDP from Unity: gems collected and alive/dead.
 * Protocol: "G:2,S:1" = gems 2, S:1=alive S:0=dead
 *
 * Wiring (I2C LCD, e.g. 16x2 with PCF8574):
 *   LCD VCC -> 5V (or 3.3V if your LCD is 3.3V)
 *   LCD GND -> GND
 *   LCD SDA -> ESP32 SDA (e.g. GPIO 21 on ESP32 Dev, or default I2C on Nano ESP32)
 *   LCD SCL -> ESP32 SCL (e.g. GPIO 22 on ESP32 Dev)
 *
 * Libraries: WiFi, WiFiUdp, LiquidCrystal_I2C (install via Arduino Library Manager)
 */

#include <WiFi.h>
#include <WiFiUdp.h>
#include <Wire.h>
#include <LiquidCrystal_I2C.h>

// ----- WiFi (same network as your PC running Unity) -----
const char* ssid     = "YOUR_WIFI_SSID";
const char* password = "YOUR_WIFI_PASSWORD";

// ----- UDP -----
WiFiUDP udp;
const int udpPort = 8888;
char packetBuffer[64];

// ----- I2C LCD (0x27 is common for 16x2; use 0x3F if yours doesn't work) -----
#define LCD_I2C_ADDR 0x27
#define LCD_COLS     16
#define LCD_ROWS     2
LiquidCrystal_I2C lcd(LCD_I2C_ADDR, LCD_COLS, LCD_ROWS);

// ----- Game state (from Unity) -----
int   gems  = 0;
bool  alive = true;
unsigned long lastPacketTime = 0;
bool  lcdReady = false;

void setup() {
  Serial.begin(9600);

  // LCD init
  Wire.begin();  // default SDA/SCL on your board
  lcd.init();
  lcd.backlight();
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Waiting...");
  lcdReady = true;

  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println();
  Serial.print("IP: ");
  Serial.println(WiFi.localIP());

  udp.begin(udpPort);
  lcd.setCursor(0, 0);
  lcd.print("IP:");
  lcd.setCursor(0, 1);
  lcd.print(WiFi.localIP());
  delay(2000);
  lcd.clear();
  updateDisplay();
}

void loop() {
  int len = udp.parsePacket();
  if (len > 0) {
    int n = udp.read(packetBuffer, sizeof(packetBuffer) - 1);
    if (n > 0) {
      packetBuffer[n] = '\0';
      lastPacketTime = millis();
      parsePacket(packetBuffer);
      updateDisplay();
    }
  }

  // Optional: show "no signal" if no packet for a while
  if (lastPacketTime > 0 && (millis() - lastPacketTime) > 5000) {
    lcd.setCursor(0, 1);
    lcd.print("-- no signal --");
  }
}

// Parse "G:2,S:1" or "G:0,S:0"
void parsePacket(char* buf) {
  int g = 0;
  int s = 1;
  if (sscanf(buf, "G:%d,S:%d", &g, &s) >= 1) {
    gems  = g;
    alive = (s != 0);
  }
}

void updateDisplay() {
  if (!lcdReady) return;

  // Line 0: Gems
  lcd.setCursor(0, 0);
  lcd.print("Gems: ");
  lcd.print(gems);
  for (int i = 6 + (gems >= 10 ? 2 : 1); i < LCD_COLS; i++) lcd.print(" ");

  // Line 1: Status
  lcd.setCursor(0, 1);
  if (alive) {
    lcd.print("Status: ALIVE ");
  } else {
    lcd.print("Status: DEAD  ");
  }
}
