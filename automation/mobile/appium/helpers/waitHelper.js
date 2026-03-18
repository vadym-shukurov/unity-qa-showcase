/**
 * Explicit wait helpers for device tests. Replaces fixed browser.pause().
 */
async function waitForAppReady(browser, timeoutMs = 10000) {
  const start = Date.now();
  while (Date.now() - start < timeoutMs) {
    const source = await browser.getPageSource();
    if (source && source.length > 50) {
      return;
    }
    await browser.pause(500);
  }
  throw new Error(`App not ready within ${timeoutMs}ms`);
}

async function waitForElement(browser, selector, timeoutMs = 15000) {
  const el = await browser.$(selector);
  await el.waitForExist({ timeout: timeoutMs, timeoutMsg: `Element ${selector} not found within ${timeoutMs}ms` });
  return el;
}

async function waitForTransition(browser, ms = 800) {
  await browser.pause(ms);
}

module.exports = {
  waitForAppReady,
  waitForElement,
  waitForTransition,
};
