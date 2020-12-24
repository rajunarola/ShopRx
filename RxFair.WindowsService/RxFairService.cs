using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using RxFair.StandardLibrary;

namespace RxFair.WindowsService
{
    public partial class RxFairService : ServiceBase
    {
        private Timer DailySchedular = null;
        private Timer MonthlySchedular = null;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _WatchlistTask; 

        public RxFairService()
        {
            InitializeComponent();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        protected override void OnStart(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();
            try
            {
                // init logging
                string logfile = string.Concat(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["LogFilePrefix"], DateTime.Now.ToString(".ddMMyyyy.HHmm"), ".log");
                Logger.Instance.Init(logfile, true);

                //Schedule Service to start at specified intervals
                this.ScheduleService();

                //Log to file if successful Start
                Logger.Instance.WriteLog($"{this.ServiceName} Service Started {DateTime.Now:dd/MM/yyyy hh:mm:ss tt}");
            }
            catch (Exception ex)
            {
                if (Logger.Instance.State == Logger.LogState.Ready)
                {
                    Logger.Instance.WriteLog(ex.Message);
                }
            }
        }

        protected override void OnStop()
        {
            this.DailySchedular.Dispose();
            this.MonthlySchedular.Dispose();
            //Try to cancel Task started
            _cancellationTokenSource.Cancel();
            try { _WatchlistTask.Wait(); }
            catch (Exception e)
            {
                // handle exeption
                Logger.Instance.WriteLog($"{this.ServiceName} Service Exception message : {e.Message}");
            }
            Logger.Instance.WriteLog($"{this.ServiceName} Service Stopped {DateTime.Now:dd/MM/yyyy hh:mm:ss tt}");
        }

        public void ScheduleService()
        {
            try
            {
                DailySchedular = new Timer(DailySchedularCallback);
                MonthlySchedular = new Timer(MonthlySchedularCallback);

                //Set the Default Time.
                DateTime dailyScheduledTime = DateTime.MinValue;
                DateTime monthlyScheduleTime = DateTime.MinValue;

                //Daily Schedular
                //Get the Scheduled Time from AppSettings.

                dailyScheduledTime = DateTime.Parse(ConfigurationManager.AppSettings["ScheduledDailyTime"]);
                Logger.Instance.WriteLog($"{this.ServiceName} ScheduledDailyTime {dailyScheduledTime}");

                if (DateTime.Now > dailyScheduledTime)
                {
                    //If Scheduled Time is passed set Schedule for the next day.
                    dailyScheduledTime = dailyScheduledTime.AddDays(1);
                }
                TimeSpan dailyTimeSpan = dailyScheduledTime.Subtract(DateTime.Now);
                Logger.Instance.WriteLog($"{this.ServiceName} dailyTimeSpan {dailyTimeSpan}");
                //Get the difference in Minutes between the Scheduled and Current Time.
                Int64 dailyDueTime = Convert.ToInt64(dailyTimeSpan.TotalMilliseconds);
                //Change the Timer's Due Time.
                DailySchedular.Change(dailyDueTime, Timeout.Infinite);

                //Monthly Schedular
                //scheduledTime = DateTime.Parse(System.Configuration.ConfigurationManager.AppSettings["ScheduledTime"]);
                monthlyScheduleTime = DateTime.Parse(DateTime.Now.Year + "/" + DateTime.Now.Month +
                    "/" + ConfigurationManager.AppSettings["ScheduledMonthlyDay"] + " " +
                    ConfigurationManager.AppSettings["ScheduledMonthlyTime"]);

                Logger.Instance.WriteLog($"{this.ServiceName} ScheduledMonthlyTime {monthlyScheduleTime}");

                if (DateTime.Now > monthlyScheduleTime)
                {
                    //If Scheduled Time is passed set Schedule for the next day.
                    monthlyScheduleTime = monthlyScheduleTime.AddMonths(1);
                }
                TimeSpan monthlyTimeSpan = monthlyScheduleTime.Subtract(DateTime.Now);
                //Get the difference in Minutes between the Scheduled and Current Time.
                Int64 monthlyDueTime = Convert.ToInt64(monthlyTimeSpan.TotalMilliseconds);
                //Change the Timer's Due Time.
                MonthlySchedular.Change(monthlyDueTime, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                //WriteToFile("Simple Service Error on: {0} " + ex.Message + ex.StackTrace);
                Logger.Instance.WriteLog($"{this.ServiceName} Service Error on: {DateTime.Now:dd/MM/yyyy hh:mm:ss tt} " + ex.Message + ex.StackTrace, "", "");
                //Stop the Windows Service.
                using (ServiceController serviceController = new ServiceController(this.ServiceName))
                {
                    serviceController.Stop();
                }
            }
        }

        private void DailySchedularCallback(object e)
        {
            try
            {
                DailyServices ds = new DailyServices();
                ds.WatchListServiceAsync(_cancellationTokenSource);
                ds.DealOfTheDayServiceAsync(_cancellationTokenSource);
                Logger.Instance.WriteLog($"{this.ServiceName} Daily Schedular callback {DateTime.Now:dd/MM/yyyy hh:mm:ss tt}");
                //_WatchlistTask = Task.Run(() => ds.WatchListServiceAsync(_cancellationTokenSource));
                
                //call daily task from class 

                //using (var smo = new smoSQLServerClass())
                //{
                //    smoSQLServerClass.serverName = ConfigurationManager.AppSettings["SqlServerName"];
                //    smoSQLServerClass.LoginID = ConfigurationManager.AppSettings["SqlLoginName"];
                //    smoSQLServerClass.password = ConfigurationManager.AppSettings["SqlPassword"];
                //    smo.createourDatabase(ConfigurationManager.AppSettings["SqlDbaseName"]);
                //}
            }
            catch (Exception exCreateDb)
            {
                Logger.Instance.WriteLog(exCreateDb.Message);
            }
            finally
            {
                //reschedule service again
                this.ScheduleService();
            }
        }

        private void MonthlySchedularCallback(object e)
        {
            try
            {
                MonthlyServices ms = new MonthlyServices();
                Logger.Instance.WriteLog($"{this.ServiceName} Monthly Schedular Callback {DateTime.Now:dd/MM/yyyy hh:mm:ss tt}");
                ms.SubscriptionCommissionServiceAsync(_cancellationTokenSource);
                //ms.RewardCalculationServiceAsync(_cancellationTokenSource);
                //call monthly task from class 
            }
            catch (Exception exCreateDb)
            {
                Logger.Instance.WriteLog(exCreateDb.Message);
            }
            finally
            {
                //reschedule service again
                this.ScheduleService();
            }
        }

    }
}
