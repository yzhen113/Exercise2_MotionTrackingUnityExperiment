# ESP32 + LCD game status display

Shows **gems collected** and **game status (Alive/Dead)** from the Unity Chicken & Gem game on an I2C LCD, via WiFi UDP.

## Hardware

- **ESP32** (Nano, DevKit, or any ESP32 with WiFi)
- **I2C LCD** (e.g. 16x2 with PCF8574 backpack)

### Wiring (I2C LCD)

| LCD   | ESP32 (typical) |
|-------|------------------|
| VCC   | 5V or 3.3V       |
| GND   | GND              |
| SDA   | GPIO 21 (ESP32) / default SDA (Nano ESP32) |
| SCL   | GPIO 22 (ESP32) / default SCL (Nano ESP32) |

If your I2C address is not `0x27`, change `LCD_I2C_ADDR` in the sketch (e.g. `0x3F`). Use an I2C scanner sketch if unsure.

## Arduino setup

1. Install boards: **ESP32** by Espressif (Board Manager).
2. Install libraries (Library Manager):
   - **LiquidCrystal I2C** (e.g. by Frank de Brabander)
   - **WiFi** and **WiFiUdp** are included with ESP32 core.
3. Open `ESP32_LCD_GameDisplay.ino`.
4. Set your WiFi in the sketch:
   - `ssid` = your network name  
   - `password` = your WiFi password
5. Upload to the ESP32. Note the **IP address** printed in Serial Monitor (e.g. `192.168.1.100`).

## Unity setup

1. Add **GameStatusSender** to a GameObject in your game scene (e.g. the same object that has **Person**).
2. Set **Esp32 Ip** to the ESP32’s IP (e.g. `192.168.1.100`).
3. **Udp Port** must be `8888` (same as in the ESP32 sketch).
4. Run the game. The LCD should show gems and “ALIVE” or “DEAD”.

## Protocol (UDP)

Unity sends one UDP packet per update, for example:

- `G:0,S:1` — 0 gems, alive  
- `G:2,S:1` — 2 gems, alive  
- `G:4,S:0` — 4 gems, dead (game over)

Port: **8888**.
