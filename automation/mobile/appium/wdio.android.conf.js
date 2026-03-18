// Appium/WebdriverIO config for Android device smoke tests.
// Run: npx wdio wdio.android.conf.js
// Requires: Appium server, Android emulator/device, APK path.

exports.config = {
  port: 4723,
  path: '/wd/hub',
  specs: ['./specs/smoke.android.js'],
  // Full flow: --spec specs/full-flow.android.js (requires accessibility IDs, see docs/device-full-flow.md)
  capabilities: [{
    'appium:platformName': 'Android',
    'appium:automationName': 'UiAutomator2',
    'appium:deviceName': 'Android Emulator',
    'appium:app': process.env.APK_PATH || './build/app.apk',
  }],
  framework: 'mocha',
  mochaOpts: {
    timeout: 60000,
  },
  reporters: [
    'spec',
    ['junit', { outputDir: './reports/junit' }],
  ],
  outputDir: './reports',
};
