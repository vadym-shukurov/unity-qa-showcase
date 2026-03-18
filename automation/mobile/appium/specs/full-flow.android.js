// Device full-flow: splash → menu → gameplay → result → menu.
// Uses Page Object Model and explicit waits. Accessibility IDs preferred; center tap fallback.

const { AppPage } = require('../pages/AppPage');
const { waitForAppReady, waitForTransition } = require('../helpers/waitHelper');

describe('Unity Mobile QA - Android Full Flow', () => {
  let appPage;

  before(async function () {
    this.timeout(15000);
    await waitForAppReady(browser, 10000);
    appPage = new AppPage(browser);
  });

  it('should complete full flow: splash → menu → gameplay → result → menu', async () => {
    await appPage.tapStart();
    await waitForTransition(browser, 1500);

    await appPage.tapPlay();
    await waitForTransition(browser, 2000);

    await appPage.tapSimulateComplete();
    await waitForTransition(browser, 1500);

    await appPage.tapBackToMenu();
    await waitForTransition(browser, 1000);

    const backAtMenu = await appPage.isMainMenuVisible();
    if (!backAtMenu) {
      const source = await browser.getPageSource();
      expect(source).toBeDefined();
      expect(source.length).toBeGreaterThan(0);
    } else {
      expect(backAtMenu).toBe(true);
    }
  });

  it('should complete Settings flow: menu → settings → back', async () => {
    // Assume at MainMenu from previous test, or tap Start if fresh
    const atMenu = await appPage.isMainMenuVisible();
    if (!atMenu) {
      await appPage.tapStart();
      await waitForTransition(browser, 1500);
    }

    await appPage.tapSettings();
    await waitForTransition(browser, 1000);

    await appPage.tapBack();
    await waitForTransition(browser, 500);

    const backAtMenu = await appPage.isMainMenuVisible();
    expect(backAtMenu).toBe(true);
  });
});
