// Appium/WebdriverIO config for iOS device smoke tests.
// Run: npx wdio wdio.ios.conf.js
// Requires: Appium server, iOS simulator/device, IPA path.
// Note: iOS simulator requires macOS. Use device-farm-ios workflow when IPA is available.

exports.config = {
  port: 4723,
  path: '/wd/hub',
  specs: ['./specs/smoke.ios.js'],
  capabilities: [{
    'appium:platformName': 'iOS',
    'appium:automationName': 'XCUITest',
    'appium:deviceName': 'iPhone 14',
    'appium:platformVersion': '16.0',
    'appium:app': process.env.IPA_PATH || process.env.APP_PATH || './build/app.app',
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
