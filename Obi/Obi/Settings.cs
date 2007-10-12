using System;
using System.Collections;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Obi
{
    /// <summary>
    /// Persistent application settings.
    /// </summary>
    /// <remarks>It also seems that making a change in the class resets the existing settings.</remarks>
    [Serializable()]
    public class Settings
    {
        public int AudioChannels;         // number of channels for recording
        public int BitDepth;              // sample bit depth
        public bool CreateTitleSection;   // defaulf for "create title section" in new project
        public string DefaultExportPath;  // default path for DAISY export
        public string DefaultPath;        // default location
        public bool EnableTooltips;       // enable or disable tooltips
        public float FontSize;            // global font size (all font sizes must be relative to this one)
        public string IdTemplate;         // identifier template
        public string LastInputDevice;    // the name of the last input device selected by the user
        public string LastOpenProject;    // path to the last open project
        public string LastOutputDevice;   // the name of the last output device selected by the user
        public Size ObiFormSize;          // size of the form (for future sessions) 
        public bool OpenLastProject;      // open the last open project at startup
        public bool SynchronizeViews;     // keep views synchronized
        public ArrayList RecentProjects;  // paths to projects recently opened
        public int SampleRate;            // sample rate in Hertz
        public UserProfile UserProfile;   // the user profile

        private static readonly string SETTINGS_FILE_NAME = "obi_settings.xml";

        /// <summary>
        /// An ID generated from the pattern in the settings.
        /// </summary>
        public string GeneratedID
        {
            get
            {
                string id = IdTemplate;
                Random rand = new Random();
                Regex regex = new Regex("#");
                while (id.Contains("#"))
                {
                    id = regex.Replace(id, String.Format("{0}", rand.Next(0, 10)), 1);
                }
                return id;
            }
        }

        /// <summary>
        /// Read the settings from the settings file; missing values are replaced with defaults.
        /// </summary>
        /// <remarks>Errors are silently ignored and default settings are returned.</remarks>
        public static Settings GetSettings()
        {
            Settings settings = new Settings();
            settings.RecentProjects = new ArrayList();
            settings.UserProfile = new UserProfile();
            settings.IdTemplate = "obi_####";
            settings.DefaultPath = Environment.CurrentDirectory;
            settings.DefaultExportPath = Environment.CurrentDirectory;
            settings.LastOpenProject = "";
            settings.LastOutputDevice = "";
            settings.LastInputDevice = "";
            settings.AudioChannels = 1;
            settings.SampleRate = 44100;
            settings.BitDepth = 16;
            settings.FontSize = 10.0f;
            settings.EnableTooltips = true;
            settings.OpenLastProject = false;
            settings.SynchronizeViews = true;
            settings.ObiFormSize = new Size(0, 0);
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
            try
            {
                IsolatedStorageFileStream stream =
                    new IsolatedStorageFileStream(SETTINGS_FILE_NAME, FileMode.Open, FileAccess.Read, file);
                SoapFormatter soap = new SoapFormatter();
                settings = (Settings)soap.Deserialize(stream);
                stream.Close();
            }
            catch (Exception) { }
            return settings;
        }

        /// <summary>
        /// Save the settings when closing.
        /// </summary>
        public void SaveSettings()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
            IsolatedStorageFileStream stream =
                new IsolatedStorageFileStream(SETTINGS_FILE_NAME, FileMode.Create, FileAccess.Write, file);
            SoapFormatter soap = new SoapFormatter();
            soap.Serialize(stream, this);
            stream.Close();
        }
    }
}
