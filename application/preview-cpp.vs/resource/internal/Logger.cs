using System;
//using EnvDTE;
//using EnvDTE80;

namespace resource.preview
{
    public static class Logger
    {
        //private static OutputWindowPane _loggerPane;

        public static void Initialize()
        {
            // ThreadHelper.ThrowIfNotOnUIThread();
            //try
            //{
            //    DTE2 dte = Package.GetGlobalService(typeof(SDTE)) as DTE2;
            //    if (dte != null)
            //    {
            //        _loggerPane = dte.ToolWindows.OutputWindow.OutputWindowPanes.Add("Log");
            //        _loggerPane.Activate();
            //        _loggerPane.Clear();
            //    }
            //}
            //catch
            //{
            //    // TODO: Add catch
            //}
        }

        public static void Log(string message)
        {
            try
            {
                //if (_loggerPane == null)
                //    Logger.Initialize();
                if (string.IsNullOrEmpty(message))
                    return;
                OutputString(DateTime.Now.ToString() + ": " + message + Environment.NewLine);
            }
            catch
            {
                // TODO: Add catch
            }
        }

        public static void Log(string format, params object[] args)
        {
            try
            {
                //if (_loggerPane == null)
                //    Logger.Initialize();
                if (string.IsNullOrEmpty(format))
                    return;
                OutputString(string.Format(format, args) + Environment.NewLine);
            }
            catch
            {
                // TODO: Add catch
            }
        }

        private static void OutputString(string text)
        {
            // TODO add timer System.Timer
            // ThreadHelper.ThrowIfNotOnUIThread("VSoutput.Logger.OutputString");
            //if (_loggerPane != null)
            //{
            //    try
            //    {
            //        _loggerPane.Activate();
            //        _loggerPane.OutputString(text);
            //    }
            //    catch (Exception)
            //    {
            //        //TODO: log Exception
            //    }
            //}
            atom.Trace.GetInstance().
                Send(atom.Trace.NAME.SOURCE.DIAGNOSTIC, atom.Trace.NAME.EVENT.INFO, 0, text);
        }
    }
}
