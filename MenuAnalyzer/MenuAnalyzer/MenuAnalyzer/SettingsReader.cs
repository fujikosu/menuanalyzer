using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace MenuAnalyzer
{
    static class SettingsReader
    {
        internal static string GetKey(string key)
        {
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;

#if __ANDROID__
            var streamLocation = "MenuAnalyzer.Droid.App.config";
#else
#if __IOS__
            var streamLocation = "MenuAnalyzer.iOS.App.config";
#else
            // UWP
            var streamLocation = "MenuAnalyzer.UWP.App.config";
#endif
#endif

            Stream stream = assembly.GetManifestResourceStream(streamLocation);

            using (var reader = new System.IO.StreamReader(stream))
            {
                var doc = XDocument.Parse(reader.ReadToEnd());
                return doc.Element("config").Element(key).Value;
            }
        }
    }
}
