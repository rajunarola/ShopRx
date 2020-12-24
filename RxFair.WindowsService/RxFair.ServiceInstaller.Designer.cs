namespace RxFair.WindowsService
{
    partial class RxFairServiceInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.RxFairProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.RxFairInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // RxFairProcessInstaller
            // 
            this.RxFairProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.RxFairProcessInstaller.Password = null;
            this.RxFairProcessInstaller.Username = null;
            // 
            // RxFairInstaller
            // 
            this.RxFairInstaller.ServiceName = "RxFairService";
            this.RxFairInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // RxFairServiceInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.RxFairProcessInstaller,
            this.RxFairInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller RxFairProcessInstaller;
        private System.ServiceProcess.ServiceInstaller RxFairInstaller;
    }
}