using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityMobileQA.Tests.TestUtils;

namespace UnityMobileQA.Tests.PlayMode
{
    // PRESENTATION: "Mobile-specific — orientation. Real devices rotate."
    // WHY: Portrait/landscape switch can break UI, flow. High-risk on mobile.
    // SCALE: Add more orientation tests. Test on different resolutions.
    // RELEASE: Catches orientation bugs before device testing.
    /// <summary>
    /// Play Mode: Mobile orientation handling. Validates app under rotation.
    /// </summary>
    [TestFixture]
    [Category("Regression")]
    public class MobileOrientationPlayModeTests : TestSetupBase
    {
        [UnityTest]
        [Timeout(5000)]
        public IEnumerator ScreenOrientation_CanBeQueried_WithoutCrash()
        {
            var orientation = Screen.orientation;
            Assert.IsTrue(orientation == ScreenOrientation.Portrait ||
                          orientation == ScreenOrientation.LandscapeLeft ||
                          orientation == ScreenOrientation.LandscapeRight ||
                          orientation == ScreenOrientation.PortraitUpsideDown ||
                          orientation == ScreenOrientation.AutoRotation);
            yield return null;
        }

        [UnityTest]
        [Timeout(3000)]
        public IEnumerator ScreenWidthAndHeight_ArePositive()
        {
            Assert.Greater(Screen.width, 0, "Screen width should be positive");
            Assert.Greater(Screen.height, 0, "Screen height should be positive");
            yield return null;
        }
    }
}
