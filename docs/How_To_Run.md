# How to Run the SystemController Integration

## Prerequisites

1. **.NET 9.0 SDK** installed
2. **Visual Studio 2022** or later (for WPF support)
3. **Windows OS** (WPF is Windows-only)
4. **Simulation CSV files** in `data/simulations/` directory:
   - `pHSensor_simulation.csv`
   - `IntakePump_simulation.csv`
   - `TurbiditySensor_simulation.csv`

## Quick Start

### Option 1: Visual Studio

1. Open `CSCN72020-Sec3-Group3.sln` in Visual Studio
2. Set `GUI` project as the startup project (right-click on GUI project → Set as Startup Project)
3. Press **F5** to build and run
4. The application window should open with three monitoring panels

### Option 2: Command Line

```bash
# Navigate to project root
cd C:\Users\plats\CSCN72020-Sec3-Group3--1

# Build the entire solution
dotnet build

# Run the GUI application
cd src/GUI
dotnet run
```

## Verification Steps

### 1. Check Console Output

When the application starts, you should see console output like:

```
[2025-01-15 14:23:45] [Info] [SystemController] Initializing system...
[2025-01-15 14:23:45] [Info] [Sensors] All sensors initialized
[2025-01-15 14:23:45] [Info] [Devices] All devices initialized
[2025-01-15 14:23:45] [Info] [DeviceLinking] Chemical doser linked to pH sensor
[2025-01-15 14:23:45] [Info] [DeviceRepository] All devices registered
[2025-01-15 14:23:45] [Info] [EventSubscription] All device events subscribed
[2025-01-15 14:23:45] [Info] [SystemController] System initialized successfully
All devices initialized
All devices started
[2025-01-15 14:23:45] [Info] [SystemController] System STARTED
```

### 2. Verify Real-Time Updates

Once running, you should see:

- **pH Monitoring Panel**: pH values updating in real-time (every 1 second)
- **Intake Pump Panel**: Flow rate values updating (if pump is on)
- **Filtration Sensor Panel**: Turbidity values updating in real-time

### 3. Test User Actions

#### Test Intake Pump Control:
1. Click **"TURN ON"** button in Intake Pump Panel
   - Button should change to **"TURN OFF"** (red background)
   - Flow rate slider should become enabled
   - Console should show: `[UserAction] Intake pump turned ON`
2. Move the **Flow Rate slider** (0-100%)
   - Flow rate display should update
   - Console should show: `[UserAction] Intake pump flow rate set to X.X%`
3. Click **"TURN OFF"** button
   - Button should change back to **"TURN ON"** (blue background)
   - Flow rate slider should become disabled
   - Console should show: `[UserAction] Intake pump turned OFF`

### 4. Test Automatic Actions

#### Test Chemical Doser Auto-Activation:
1. Watch the pH value in pH Monitoring Panel
2. When pH goes **below 6.5** or **above 8.5**:
   - Chemical Doser should automatically activate
   - Status should change to **"ACTIVE"** (red background)
   - Console should show: `[ChemicalDoser] State changed: ACTIVE`
   - Console should show: `[pHSensor] ⚠️ pH OUT OF RANGE: X.XX (safe range: 6.5-8.5)`
3. When pH returns to safe range (6.5 - 8.5):
   - Chemical Doser should automatically deactivate
   - Status should change to **"INACTIVE"** (gray background)
   - Console should show: `[ChemicalDoser] State changed: INACTIVE`

#### Test Filtration Alerts:
1. Watch the turbidity value in Filtration Sensor Panel
2. When turbidity exceeds **5.0 NTU**:
   - Alert banner should appear with warning
   - Background should change to red
   - Console should show: `[FiltrationSensor] 🚨 ALERT: Turbidity threshold exceeded! Value: X.XX NTU`
3. When turbidity returns below 5.0 NTU:
   - Alert banner should disappear
   - Console should show: `[FiltrationSensor] ✅ ALERT CLEARED: Turbidity back to normal. Value: X.XX NTU`

### 5. Check Event Logging

Monitor the console for system events:

- **DataUpdate**: Sensor readings changing
  ```
  [2025-01-15 14:23:46] [DataUpdate] [pHSensor] pH reading changed: 7.25
  [2025-01-15 14:23:46] [DataUpdate] [IntakePump] Flow rate changed: 50.0%
  [2025-01-15 14:23:46] [DataUpdate] [FiltrationSensor] Turbidity changed: 2.50 NTU
  ```

- **StateChange**: Device state changes
  ```
  [2025-01-15 14:23:47] [StateChange] [IntakePump] State changed: ON
  [2025-01-15 14:23:47] [StateChange] [ChemicalDoser] State changed: ACTIVE
  ```

- **Warning**: Threshold warnings
  ```
  [2025-01-15 14:23:47] [Warning] [pHSensor] ⚠️ pH OUT OF RANGE: 6.20 (safe range: 6.5-8.5)
  [2025-01-15 14:23:47] [Warning] [FiltrationSensor] ⚠️ HIGH TURBIDITY: 5.50 NTU (threshold: 5 NTU)
  ```

- **Alert**: Critical alerts
  ```
  [2025-01-15 14:23:48] [Alert] [FiltrationSensor] 🚨 ALERT: Turbidity threshold exceeded! Value: 5.50 NTU
  ```

- **UserAction**: User control actions
  ```
  [2025-01-15 14:23:49] [UserAction] Intake pump turned ON
  [2025-01-15 14:23:50] [UserAction] Intake pump flow rate set to 75.0%
  ```

## Troubleshooting

### Issue: "File not found" errors

**Solution:**
- Ensure simulation CSV files exist in `data/simulations/` directory
- Check file paths in console output
- Verify you're running from the correct directory

### Issue: UI not updating

**Solution:**
- Check console output for system events
- Verify devices are started (`System STARTED` message)
- Check that events are being raised (console shows data updates)
- Ensure simulation files contain valid data

### Issue: Build errors

**Solution:**
- Ensure .NET 9.0 SDK is installed: `dotnet --version`
- Restore packages: `dotnet restore`
- Clean and rebuild: `dotnet clean && dotnet build`

### Issue: Chemical doser not activating

**Solution:**
- Check that pH sensor is linked to chemical doser (console should show `DeviceLinking` message)
- Verify pH values are outside safe range (6.5 - 8.5)
- Check console for pH reading changes

### Issue: No console output

**Solution:**
- Ensure you're running from command line or Visual Studio
- Check that SystemController events are being logged
- Verify `OnSystemEvent` event handler is connected

## Expected Behavior Summary

✅ **System Initialization:**
- All devices instantiated
- Events subscribed
- System started

✅ **Real-Time Updates:**
- pH readings update every 1 second
- Turbidity readings update every 1 second
- Flow rate updates when pump is on
- UI panels update automatically

✅ **Automatic Actions:**
- Chemical doser activates when pH out of range
- Chemical doser deactivates when pH returns to safe range
- Alerts trigger when turbidity exceeds threshold

✅ **User Actions:**
- Intake pump can be turned on/off
- Flow rate can be adjusted (0-100%)
- All actions are logged

✅ **Event Logging:**
- All events logged to console
- Events broadcast to GUI components
- System events available for subscription

## Next Steps

Once the system is running:

1. **Monitor** real-time sensor data
2. **Test** user control actions
3. **Observe** automatic device responses
4. **Review** event logs in console
5. **Verify** GUI panels update correctly

## Architecture Verification

To verify the integration layer is working:

1. **SystemController** initializes all devices ✓
2. **Event Subscription** connects all device events ✓
3. **Event Broadcasting** sends updates to GUI ✓
4. **Automatic Actions** trigger based on sensor readings ✓
5. **User Actions** control devices via SystemController ✓
6. **Logging** captures all system events ✓

All of these should be visible in the console output and GUI behavior.
