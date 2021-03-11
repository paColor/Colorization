using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace ColorLibTest
{
    /// <summary>
    /// <c>InitNLog</c> contains the static code that must be executed when the application starts and when it terminates, 
    /// in order to configure NLog correctly.
    /// </summary>
    public static class InitNLog
    {
        /// <summary>
        /// To be called at the very beginning of the existence of the add-in, beofre any logging takes place.
        /// </summary>
        public static void StartNLog()
        {
            NLog.LogManager.Setup().SetupExtensions(s => s.AutoLoadAssemblies(false));
            // Faster and more secure startup by not loading random assemblies with NLog as filename-prefix.
            // I wonder whether this place is early enough for calling it. I have no idea when NLog loads assemblies...

#if DEBUG
            var nLogConfig = LogManager.Configuration;
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            logconsole.Layout = "${longdate} ${uppercase:${level}} ${logger} ${message}";


            // ---------------------------------- EVERYTHING --------------------------------------
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, dc);  // everything
            nLogConfig.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);  // everything equal or higher than Debug
            //nLogConfig.AddRule(LogLevel.Info, LogLevel.Fatal, dc);  // everything equal or higher than Info

            // ----------------------------- ColorizationControls ---------------------------------
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole, "ColorizationControls.*");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole, "ColorizationControls.ConfigPane");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole, "ColorizationControls.InitNLog");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole, "ColorizationControls.CharFormatForm");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole, "ColorizationControls.ConfigControl");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole, "ColorizationControls.DConsoleTarget");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole, "ColorizationControls.FormatButtonHandler2");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole, "ColorizationControls.HilightForm");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole, "ColorizationControls.LetterFormatForm");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole, "ColorizationControls.LicenseForm");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole, "ColorizationControls.MyColorDialog");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole, "ColorizationControls.StaticColorizControls");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole, "ColorizationControls.SylFormatForm");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Trace, logconsole, "ColorizationControls.RTBText");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Trace, logconsole, "ColorizationControls.WaitingForm*");

            // ---------------------------------- ColorLib -----------------------------------------
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole, "ColorLib.*");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole, "ColorLib.Config");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Trace, logconsole, "ColorLib.TheText*");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Trace, logconsole, "ColorLib.ProgressNotifier*");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Trace, logconsole, "ColorLib.PhonWord*");
            nLogConfig.AddRule(LogLevel.Trace, LogLevel.Trace, logconsole, "ColorLib.UndoFactory*");

            // ------------------------------------ Office ------------------------------------------
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole, "ColorizationWord.*");
            //nLogConfig.AddRule(LogLevel.Trace, LogLevel.Trace, logconsole, "Colorization.*");





            LogManager.Configuration = nLogConfig;
#endif
        }

        /// <summary>
        /// To be called when the add in terminates
        /// </summary>
        public static void CloseNLog()
        {
            NLog.LogManager.Shutdown();
        }
    }
}
