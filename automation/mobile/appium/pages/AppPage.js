/**
 * Page Object for Unity Mobile QA app screens.
 * Centralizes selectors and interactions for Appium specs.
 */
const { BasePage } = require('./BasePage');

class AppPage extends BasePage {
  constructor(driver) {
    super(driver);
  }

  // Selectors (accessibility IDs)
  get selectors() {
    return {
      btnStart: '~btn_start',
      btnPlay: '~btn_play',
      btnSettings: '~btn_settings',
      btnSimulateComplete: '~btn_simulate_complete',
      btnBack: '~btn_back',
      btnBackToMenu: '~btn_back_to_menu',
    };
  }

  async waitForUnityReady(timeoutMs = 10000) {
    return this.waitForAppReady(timeoutMs);
  }

  async tapStart() {
    return this.tapOrFallback(this.selectors.btnStart, () => this.tapCenter());
  }

  async tapPlay() {
    return this.tapOrFallback(this.selectors.btnPlay, () => this.tapCenter());
  }

  async tapSettings() {
    return this.tapOrFallback(this.selectors.btnSettings, () => this.tapCenter());
  }

  async tapSimulateComplete() {
    return this.tapOrFallback(this.selectors.btnSimulateComplete, () => this.tapCenter());
  }

  async tapBack() {
    return this.tapOrFallback(this.selectors.btnBack, () => this.tapCenter());
  }

  async tapBackToMenu() {
    return this.tapOrFallback(this.selectors.btnBackToMenu, () => this.tapCenter());
  }

  async isMainMenuVisible() {
    const el = await this.driver.$(this.selectors.btnPlay);
    return el.isExisting();
  }
}

module.exports = { AppPage };
