using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace bayoen
{
    public class TrayIcon
    {
        public NotifyIcon NotifyIcon { get; set; }

        private string _iconText;
        public string IconText
        {
            get => this._iconText;
            set
            {
                this.NotifyIcon.Text = value;
                this._iconText = value;
            }
        }

        public TrayIcon()
        {
            this.NotifyIcon = new NotifyIcon
            {
                Visible = true,
                Icon = bayoen.Properties.Resources.ico_kirby_star_allies,
                ContextMenu = new ContextMenu(),
                //Text = Config.ProjectName,
            };

            this.SetNotifyExitMenu();
        }

        private void SetNotifyExitMenu()
        {
            MenuItem ExitMenuItem = new MenuItem("Exit")
            {

            };
            ExitMenuItem.Click += ExitMenuItem_Click;
            this.NotifyIcon.ContextMenu.MenuItems.Add(ExitMenuItem);

            void ExitMenuItem_Click(object sender, EventArgs e)
            {
                this.DisposeNotifyIcon();
                Environment.Exit(0);
            }
        }

        private void DisposeNotifyIcon()
        {
            this.NotifyIcon.Visible = false;
            this.NotifyIcon.Dispose();
        }
    }
}
