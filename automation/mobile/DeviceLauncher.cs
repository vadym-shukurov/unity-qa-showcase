// Placeholder for device install/launch abstraction.
// Abstracts Android (adb) vs iOS (xcrun simctl / idevice) differences.
//
// Example usage:
//   var launcher = DeviceLauncher.ForPlatform(Platform.Android);
//   launcher.Install("path/to/app.apk");
//   launcher.Launch("com.company.game");
//   launcher.WaitForLaunch(timeout);

namespace UnityMobileQA.Automation.Mobile
{
    public enum Platform { Android, iOS }

    public class DeviceLauncher
    {
        public static DeviceLauncher ForPlatform(Platform platform) => new DeviceLauncher(platform);
        private DeviceLauncher(Platform platform) { }
    }
}
