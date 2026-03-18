// Device smoke: install, launch, verify app opens and is interactive.
// Uses explicit waits instead of fixed pause. Run after APK build.

const { waitForAppReady, waitForTransition } = require('../helpers/waitHelper');

describe('Unity Mobile QA - Android Smoke', () => {
  before(async function () {
    this.timeout(15000);
    await waitForAppReady(browser, 10000);
  });

  it('should launch app without crash', async () => {
    const source = await browser.getPageSource();
    expect(source).toBeDefined();
    expect(source.length).toBeGreaterThan(0);
  });

  it('should have valid package/activity', async () => {
    const activity = await browser.getCurrentActivity();
    expect(activity).toBeDefined();
    expect(activity).toContain('unity');
  });

  it('should have interactive window', async () => {
    const { width, height } = await browser.getWindowSize();
    expect(width).toBeGreaterThan(0);
    expect(height).toBeGreaterThan(0);
  });

  it('should respond to back button without crash', async () => {
    await browser.pressKeyCode(4); // Android BACK
    await waitForTransition(browser, 500);
    const source = await browser.getPageSource();
    expect(source).toBeDefined();
    expect(source.length).toBeGreaterThan(0);
  });

  it('should survive multiple back presses (resilience)', async () => {
    for (let i = 0; i < 3; i++) {
      await browser.pressKeyCode(4); // Android BACK
      await waitForTransition(browser, 300);
    }
    const source = await browser.getPageSource();
    expect(source).toBeDefined();
    expect(source.length).toBeGreaterThan(0);
  });

  it('should remain responsive after idle', async () => {
    await waitForTransition(browser, 5000);
    const source = await browser.getPageSource();
    expect(source).toBeDefined();
    expect(source.length).toBeGreaterThan(0);
  });
});
