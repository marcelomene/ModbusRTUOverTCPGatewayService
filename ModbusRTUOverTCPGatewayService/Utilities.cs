using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRTUOverTCPGatewayService
{
	internal static class Utilities
	{
		internal static string ByteArrayToString(byte[] array)
		{
			string ret = "";

			foreach (byte b in array)
				ret += $"{b.ToString("x2")} ";
			return ret;
		}
	}

	internal static class Extensions 
	{
		public static bool IsConnected(this Socket client)
		{
			// Detect if client disconnected
			if (client.Poll(0, SelectMode.SelectRead))
			{
				byte[] buff = new byte[1];
				if (client.Receive(buff, SocketFlags.Peek) == 0)
				{
					// Client disconnected
					return false;
				}
				else
					return true;
			}
			else
				return true;
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
