/**
 * Base page for Appium Page Object Model.
 * Provides explicit wait helpers and common interactions.
 */
class BasePage {
  constructor(driver) {
    this.driver = driver;
  }

  /**
   * Wait for element to exist (explicit wait). Replaces fixed browser.pause().
   * @param {string} selector - Appium selector (e.g. '~btn_play')
   * @param {number} timeoutMs - Max wait in ms
   * @returns {Promise<WebdriverIO.Element>}
   */
  async waitForElement(selector, timeoutMs = 15000) {
    const el = await this.driver.$(selector);
    await el.waitForExist({ timeout: timeoutMs, timeoutMsg: `Element ${selector} not found within ${timeoutMs}ms` });
    return el;
  }

  /**
   * Wait for Unity app to be ready (page source non-empty).
   * @param {number} timeoutMs - Max wait in ms
   */
  async waitForAppReady(timeoutMs = 10000) {
    const start = Date.now();
    while (Date.now() - start < timeoutMs) {
      const source = await this.driver.getPageSource();
      if (source && source.length > 50) {
        return;
      }
      await this.driver.pause(500);
    }
    throw new Error(`App not ready within ${timeoutMs}ms`);
  }

  /**
   * Tap element by selector, or fallback to center tap.
   * @param {string} selector - Appium selector
   * @param {Function} fallbackTap - Async fn to call if element not found
   */
  async tapOrFallback(selector, fallbackTap) {
    const el = await this.driver.$(selector);
    if (await el.isExisting()) {
      await el.click();
    } else {
      await fallbackTap();
    }
  }

  async tapCenter() {
    const { width, height } = await this.driver.getWindowSize();
    const x = Math.floor(width * 0.5);
    const y = Math.floor(height * 0.5);
    await this.driver.touchAction({ action: 'tap', x, y });
  }
}

module.exports = { BasePage };
