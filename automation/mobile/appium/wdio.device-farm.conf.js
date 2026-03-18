// Device farm config — parallel execution on multiple device configs.
// Run: npx wdio wdio.device-farm.conf.js
// Use with BrowserStack, Sauce Labs, or local emulator matrix.

const base = require('./wdio.android.conf').config;

const baseCap = base.capabilities[0];

exports.config = {
  ...base,
  capabilities: [
    { ...baseCap, 'appium:deviceName': 'Pixel 6', 'appium:platformVersion': '13' },
    { ...baseCap, 'appium:deviceName': 'Pixel 4', 'appium:platformVersion': '11' },
    { ...baseCap, 'appium:deviceName': 'Samsung Galaxy S21', 'appium:platformVersion': '12' },
  ],
  maxInstances: 3,
  // BrowserStack: add 'browserstack.user', 'browserstack.key' to capabilities
  // Sauce Labs: add 'sauce:options' with username, accessKey
};
