using Microsoft.Win32;

namespace LS.Flowchart.Tools
{

    /// <summary>
    /// 自启管理
    /// </summary>
    public class StartupManager
    {
        private const string KeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private readonly string _appName = "VisionApp"; // 唯一应用标识

        /// <summary>
        /// 启用自启动
        /// </summary>
        public void EnableStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(KeyPath, true))
            {
                // 替换 Application.ExecutablePath 为正确的获取可执行文件路径方式
                string exePath = System.Reflection.Assembly.GetEntryAssembly().Location + " /startup"; // 添加参数
                key.SetValue(_appName, exePath);
            }
        }

        /// <summary>
        /// 禁用自启动
        /// </summary>
        public void DisableStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(KeyPath, true))
            {
                if (key.GetValue(_appName) != null)
                {
                    key.DeleteValue(_appName);
                }
            }
        }

        /// <summary>
        /// 检查状态
        /// </summary>
        /// <returns></returns>
        public bool IsStartupEnabled()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(KeyPath, false))
            {
                return key?.GetValue(_appName) != null;
            }
        }
    }
}
