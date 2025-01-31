using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTester
{
	class Utilities
	{
        internal static string ByteArrayToString(byte[] array)
        {
            string ret = "";

            foreach (byte b in array)
                ret += $"{b.ToString("x2")} ";
            return ret;
        }

        internal static float ConvertTemperature(byte[] data)
		{
            return (float)((data[3] << 8) + data[4]) / 10;
        }
    }
    public static class AppSettings
    {
        /// <summary>
        /// Return a app settings from environment variable (first try) or
        /// app_settings.config file.
        /// </summary>
        /// <param name="key">Key of the setting from app_settings file</param>
        /// <param name="defaultValue">Default valut to be returned in case "key" is not found</param>
        /// <returns></returns>
        public static T Get<T>(string key, T defaultValue)
        {
            try
            {
                string settingValue = System.Configuration.ConfigurationManager.AppSettings[key];

                if (settingValue != null)
                {
                    // check if exist on environment
                    string envSettingValue = System.Environment.GetEnvironmentVariable(key);
                    if (envSettingValue != null) settingValue = envSettingValue;
                }

                // if both are empty use defaultValue
                if (settingValue == null)
                    return defaultValue;

                T retValue = (T)Convert.ChangeType(settingValue, typeof(T));
                return retValue;
            }
            catch
            {
                return default;
            }
        }
    }
}
