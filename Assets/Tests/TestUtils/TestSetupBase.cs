using System;
using NUnit.Framework;
using UnityEngine;
using UnityMobileQA.Core;

namespace UnityMobileQA.Tests.TestUtils
{
    // PRESENTATION: "Deterministic tests — clean state per test. No pollution."
    // WHY: PlayerPrefs persist across tests. Reset = each test starts fresh.
    // SCALE: Add more teardown (clear singletons, reset time) as needed.
    // RELEASE: Flaky from shared state = bad. This prevents it.
    /// <summary>
    /// Base class for tests that need consistent setup/teardown.
    /// Cleans PlayerPrefs to avoid test pollution.
    /// </summary>
    public abstract class TestSetupBase
    {
        private string _currentTestName;

        [OneTimeSetUp]
        public virtual void BaseOneTimeSetUp()
        {
            if (Application.isPlaying)
                RuntimeLogCapture.Start();
        }

        [OneTimeTearDown]
        public virtual void BaseOneTimeTearDown()
        {
            if (Application.isPlaying)
                RuntimeLogCapture.Stop();
            TestTelemetry.Flush();
        }

        [SetUp]
        public virtual void BaseSetUp()
        {
            PlayerPrefs.DeleteAll();
            _currentTestName = TestContext.CurrentContext?.Test?.Name ?? "Unknown";
        }

        [TearDown]
        public virtual void BaseTearDown()
        {
            if (Application.isPlaying)
            {
                if (TestContext.CurrentContext?.Result?.Outcome == NUnit.Framework.Interfaces.ResultState.Failure)
                    ScreenshotCapture.CaptureOnFailure(_currentTestName);
                ServiceLocator.ResetForTests();
            }
            PlayerPrefs.DeleteAll();
        }
    }
}
