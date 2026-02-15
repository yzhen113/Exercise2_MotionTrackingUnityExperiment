# Setup guide: Unity game → ESP32 → LCD display

Follow these steps in order.

---

## Part 1: Hardware

### What you need
- **ESP32** (Nano, DevKit, or any ESP32 with WiFi)
- **I2C LCD** (e.g. 16×2 with PCF8574 backpack — the kind with 4 pins: VCC, GND, SDA, SCL)
- 4 jumper wires

### Wiring

| LCD pin | Connect to ESP32        |
|---------|-------------------------|
| **VCC** | 5V or 3.3V (match your LCD) |
| **GND** | GND                    |
| **SDA** | GPIO **21** (ESP32 Dev) or default **SDA** (Nano ESP32) |
| **SCL** | GPIO **22** (ESP32 Dev) or default **SCL** (Nano ESP32) |

Plug the ESP32 into your computer via USB for power and programming.

---

## Part 2: Arduino IDE / ESP32

### 2.1 Install ESP32 board support
1. Open **Arduino IDE**.
2. **File → Preferences** → in "Additional Board Manager URLs" add:
   ```text
   https://raw.githubusercontent.com/espressif/arduino-esp32/gh-pages/package_esp32_index.json
   ```
3. **Tools → Board → Boards Manager** → search **esp32** → install **"esp32" by Espressif**.
4. **Tools → Board** → select your board (e.g. **ESP32 Dev Module** or **Arduino Nano ESP32**).

### 2.2 Install LCD library
1. **Sketch → Include Library → Manage Libraries**.
2. Search **LiquidCrystal I2C**.
3. Install **"LiquidCrystal I2C"** (e.g. by Frank de Brabander).

### 2.3 Configure the sketch
1. Open **`ESP32_LCD_GameDisplay.ino`** from the `ESP32_LCD_GameDisplay` folder.
2. Set your WiFi (around lines 20–21):
   ```cpp
   const char* ssid     = "YourNetworkName";
   const char* password = "YourWiFiPassword";
   ```
3. If your LCD doesn’t work with the default address, change line 29:
   - Try `0x3F` instead of `0x27` (or use an I2C scanner sketch to find the address).

### 2.4 Upload and get the ESP32 IP
1. **Tools → Port** → select the port of your ESP32 (e.g. COM3 or /dev/cu.usb…).
2. Click **Upload** (right-arrow).
3. When it’s done, open **Tools → Serial Monitor**.
4. Set baud rate to **9600**.
5. Press the **RESET** button on the ESP32 if needed. You should see something like:
   ```text
   ....
   IP: 192.168.1.105
   ```
6. **Write down this IP** — you’ll use it in Unity.

---

## Part 3: Unity

### 3.1 Add the sender to your scene
1. Open your game scene in Unity (the one with the **Person** script on the player).
2. Select the **GameObject that has the Person component** (usually the player).
3. In the Inspector, click **Add Component**.
4. Search for **Game Status Sender** and add it.

### 3.2 Set the ESP32 IP
1. With that same GameObject selected, find the **Game Status Sender** component.
2. Set **Esp32 Ip** to the IP you wrote down (e.g. `192.168.1.105`).
3. Leave **Udp Port** as **8888** (must match the ESP32 sketch).

### 3.3 Run the game
1. Put your **PC and ESP32 on the same WiFi network**.
2. Press **Play** in Unity.
3. The LCD should show:
   - **Line 1:** `Gems: 0` (updates as you collect gems)
   - **Line 2:** `Status: ALIVE` or `Status: DEAD` when you hit a chicken

---

## Quick checklist

- [ ] LCD wired to ESP32 (VCC, GND, SDA, SCL)
- [ ] ESP32 board and LiquidCrystal I2C library installed in Arduino IDE
- [ ] WiFi SSID and password set in the .ino file
- [ ] Sketch uploaded; Serial Monitor shows the ESP32 IP
- [ ] GameStatusSender added to the player (same GameObject as Person) in Unity
- [ ] Esp32 Ip set in the Game Status Sender component to that IP
- [ ] PC and ESP32 on the same WiFi; Unity game running

---

## If the LCD stays blank or shows garbage

- **Wrong I2C address:** Change `LCD_I2C_ADDR` in the sketch to `0x3F` (or run an I2C scanner to find the address).
- **Wrong wiring:** Double-check SDA/SCL and VCC/GND; try swapping SDA and SCL once.
- **No data on LCD:** Confirm the ESP32 IP in Serial Monitor, then confirm that exact IP is in Unity’s **Esp32 Ip** and that you’re in Play mode.

## If Unity can’t send (firewall)

- On Windows, allow Unity (or the editor) through the firewall when prompted, or allow UDP outbound on port 8888.
