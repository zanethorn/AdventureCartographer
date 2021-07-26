using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MapMaker.Models;

namespace MapMaker.Controllers
{
    public class SettingsController : SmartObject
    {
        public SettingsController()
        {
            Settings = new Settings();
        }

        public Settings Settings { get; private set; }

        public Task Init()
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location);
            Debug.Assert(versionInfo.CompanyName != null, "versionInfo.CompanyName != null");
            Debug.Assert(versionInfo.ProductName != null, "versionInfo.ProductName != null");

            var settingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                versionInfo.CompanyName,
                versionInfo.ProductName,
                "settings.xml"
            );

            var directory = Path.GetDirectoryName(settingsPath);
            Debug.Assert(directory != null, nameof(directory) + " != null");
            Directory.CreateDirectory(directory);

            var serializer = new XmlSerializer(typeof(Settings));
            if (File.Exists(settingsPath))
                Settings = (Settings) (serializer.Deserialize(File.OpenRead(settingsPath)) ?? new Settings());
            else
                serializer.Serialize(File.OpenWrite(settingsPath), Settings);

            return Task.CompletedTask;
        }
    }
}