using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace ColorizationControls
{
    /// <summary>
    /// <c>InitNLog</c> contains the static code that must be executed when the application starts and when it terminates, 
    /// in order to configure NLog correctly.
    /// </summary>
    public static class InitNLog
    {
        private static DConsoleTarget dc;

        /// <summary>
        /// To be called at the very beginning of the existence of the add-in, beofre any logging takes place.
        /// </summary>
        public static void StartNLog()
        {
            NLog.LogManager.Setup().SetupExtensions(s => s.AutoLoadAssemblies(false));
            // Faster and more secure startup by not loading random assemblies with NLog as filename-prefix.
            // I wonder whether this place is early enough for calling it. I have no idea when NLog loads assemblies...

#if DEBUG
            NLog.LogManager.Setup().SetupExtensions(s => s.RegisterTarget<ColorizationControls.DConsoleTarget>("DConsole"));
            var nLogConfig = LogManager.Configuration;
            dc = new DConsoleTarget()
            {
                Layout = "${longdate} ${uppercase:${level}} ${logger} ${message}"
            };
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, dc);  // everything
            nLogConfig.AddRule(LogLevel.Info, LogLevel.Fatal, dc);  // everything equal or higher than Info

            //// nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, dc, "ColorizationControls.*");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, dc, "ColorizationControls.ConfigPane");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, dc, "ColorizationControls.InitNLog");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, dc, "ColorizationControls.CharFormatForm");
            ////nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, dc, "ColorizationControls.ConfigControl");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, dc, "ColorizationControls.DConsoleTarget");
            //// nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, dc, "ColorizationControls.FormatButtonHandler2");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, dc, "ColorizationControls.HilightForm");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, dc, "ColorizationControls.LetterFormatForm");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, dc, "ColorizationControls.LicenseForm");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, dc, "ColorizationControls.MyColorDialog");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, dc, "ColorizationControls.StaticColorizControls");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, dc, "ColorizationControls.SylFormatForm");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, dc, "ColorLib.*");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, dc, "ColorizationWord.*");
            LogManager.Configuration = nLogConfig;
#endif
        }

        /// <summary>
        /// To be called when the add in terminates
        /// </summary>
        public static void CloseNLog()
        {
            dc.Dispose();
            NLog.LogManager.Shutdown();
        }
    }
}
