// Device smoke: install, launch, verify app opens and is interactive.
// Uses explicit waits instead of fixed pause. Run after IPA/.app build.

const { waitForAppReady, waitForTransition } = require('../helpers/waitHelper');

describe('Unity Mobile QA - iOS Smoke', () => {
  before(async function () {
    this.timeout(15000);
    await waitForAppReady(browser, 10000);
  });

  it('should launch app without crash', async () => {
    const source = await browser.getPageSource();
    expect(source).toBeDefined();
    expect(source.length).toBeGreaterThan(0);
  });

  it('should have valid bundle', async () => {
    const session = await browser.getSession();
    expect(session).toBeDefined();
    expect(session.capabilities).toBeDefined();
  });

  it('should have interactive window', async () => {
    const { width, height } = await browser.getWindowSize();
    expect(width).toBeGreaterThan(0);
    expect(height).toBeGreaterThan(0);
  });

  it('should respond to back gesture without crash', async () => {
    await browser.back();
    await waitForTransition(browser, 500);
    const source = await browser.getPageSource();
    expect(source).toBeDefined();
    expect(source.length).toBeGreaterThan(0);
  });

  it('should survive multiple back gestures (resilience)', async () => {
    for (let i = 0; i < 3; i++) {
      await browser.back();
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
