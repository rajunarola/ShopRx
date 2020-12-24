/*
 * Created by SharpDevelop.
 * User: jason
 * Date:  9.4.2009
 * Time: 9.16 AM
 * 
 */
using System;
using System.IO;
using System.Threading;

namespace RxFair.WindowsService
{
    /// <summary>
    /// Singleton Logger that can open the file and keep logging as needed...
    /// </summary>
    public sealed class Logger
    {
        private static Logger _instance = new Logger();
        private String _log = String.Empty;
        private bool _logToConsole = false;
        private static StreamWriter _logStream;
        private String _logSep = " | ";
        private static LogState _state = LogState.NeedsInitialized;

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                }
                return _instance;
            }
        }

        private Logger()
        {
            // do something?
        }

        public bool Init(String logFile, bool alwaysLogToConsole)
        {
            bool bRet = false;
            _logToConsole = alwaysLogToConsole;
            _log = logFile;
            try
            {
                //setup file
                _logStream = new StreamWriter(_log, true, System.Text.Encoding.Unicode);
                bRet = true;
                _state = LogState.Ready;
            }
            catch (System.IO.DirectoryNotFoundException dnfE)
            {
                WriteLogException(dnfE);
            }
            catch (System.IO.PathTooLongException ptlE)
            {
                WriteLogException(ptlE);
            }
            catch (System.IO.IOException ioE)
            {
                WriteLogException(ioE);
            }
            return bRet;
        }

        /// <summary>
        /// Logs information and error in the application
        /// </summary>
        /// <param name="message"></param>
        /// <param name="baseDir"></param>
        /// <param name="newLine"></param>
        public void WriteLog(string message, string baseDir = "", string newLine = "")
        {
            if (!_state.Equals(LogState.Ready))
            {
                Exception e = new Exception($"Logger has NOT been initialized, or logfile '{_log}' is in a bad state. ERR: {_state.ToString()}");
                throw (e);
            }
            String logTime = LogTime();
            if (_logToConsole)
            {
                Console.WriteLine(String.Concat(logTime, _logSep, message));
            }
            try
            {
                _logStream.WriteLine(String.Concat(logTime, _logSep, message));
                _logStream.Flush();
            }
            catch (System.ObjectDisposedException ode)
            {
                if (_logToConsole)
                {
                    Console.WriteLine("EXCEPTION TRYING TO WRITE TO LOGFILE!");
                }
                WriteLogException(ode);
            }
            catch (System.IO.IOException ioe)
            {
                if (_logToConsole)
                {
                    Console.WriteLine("EXCEPTION TRYING TO WRITE TO LOGFILE!");
                }
                WriteLogException(ioe);
            }
        }

        /// <summary>
        /// Logs an exception to the logfile
        /// </summary>
        /// <param name="ex">Exeption to log</param>
        public void WriteLogException(Exception ex)
        {
            if (!_state.Equals(LogState.Ready))
            {
                Exception e = new Exception(String.Format("Logger has NOT been initialized, or logfile '{0}' is in a bad state. ERR: {1}", _log, _state.ToString()));
                throw (e);
            }
            String errTime = LogTime();
            if (_logToConsole)
            {
                Console.WriteLine(String.Concat(errTime, _logSep, ex.Message, Environment.NewLine, ex.StackTrace));
            }
            try
            {
                _logStream.WriteLine(String.Concat(errTime, _logSep, "*****EXCEPTION*****"));
                _logStream.WriteLine(String.Concat(String.Format("{0,22}{1}", _logSep, "Message : "), ex.Message));
                _logStream.WriteLine(String.Concat(String.Format("{0,22}{1}", _logSep, "Stack Trace : "), ex.StackTrace));
                _logStream.Flush();
            }
            catch (System.ObjectDisposedException ode)
            {
                _state = LogState.ERR;
                if (_logToConsole)
                {
                    Console.WriteLine("EXCEPTION TRYING TO WRITE TO LOGFILE!");
                    Console.WriteLine(ode.Message);
                    Console.WriteLine(ode.StackTrace);
                }
                else
                {
                    throw (ode);
                }
            }
            catch (System.IO.IOException ioe)
            {
                _state = LogState.ERR;
                if (_logToConsole)
                {
                    Console.WriteLine("EXCEPTION TRYING TO WRITE TO LOGFILE!");
                    Console.WriteLine(ioe.Message);
                    Console.WriteLine(ioe.StackTrace);
                }
                else
                {
                    throw (ioe);
                }
            }
        }

        private String LogTime()
        {
            return DateTime.Now.ToString("yyyy:MM:dd:HH:mm:ss");
        }

        ~Logger() //destructor
        {
            // make sure we close our files and flush the log.
            if (_logStream != null && _logStream.BaseStream.CanWrite)
            {
                _logStream.Flush();
                _logStream.Close();
                _logStream.Dispose();
            }
        }

        public enum LogState
        {
            ERR = -1,
            NeedsInitialized = 0,
            Ready = 1
        }

        #region accessors
        public LogState State
        {
            get
            {
                return _state;
            }
        }
        #endregion
    }
}
