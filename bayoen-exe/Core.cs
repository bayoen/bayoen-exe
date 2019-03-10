using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using bayoen.Data;
using bayoen.Memory;
using bayoen.Windows;

namespace bayoen
{
    public static class Core
    {
        #region [ Variables ]
        private static TrayIcon _trayIcon;
        public static TrayIcon TrayIcon => _trayIcon ?? (_trayIcon = new TrayIcon());

        private static MainWindow _mainWindow;
        public static MainWindow MainWindow => _mainWindow ?? (_mainWindow = new MainWindow());

        #if DEBUG
        private static DebugWindow _debugWindow;
        public static DebugWindow DebugWindow => _debugWindow ?? (_debugWindow = new DebugWindow());
        #endif

        private static PPTMemory _pptMemory;
        public static PPTMemory PPTMemory => _pptMemory ?? (_pptMemory = new PPTMemory(Config.PPTName));

        private static PPTStatus _pptStatus;
        public static PPTStatus PPTStatus => _pptStatus ?? (_pptStatus = new PPTStatus());        
        public static PPTStatus OldPPTStatus { get; set; }

        private static PPTTimer _pptTimer;
        public static PPTTimer PPTTimer => _pptTimer ?? (_pptTimer = new PPTTimer(Config.PPTTimeSpan));

        private static MatchRecord _currentMatch;
        public static MatchRecord CurrentMatch => _currentMatch ?? (_currentMatch = new MatchRecord());

        private static GameRecord _currentGame;
        public static GameRecord CurrentGame => _currentGame ?? (_currentGame = new GameRecord());

        #endregion

        public static void Initialize()
        {
            TrayIcon.IconText = Config.ProjectName;

            MainWindow.Show();
            #if DEBUG
            DebugWindow.Show();
            #endif

            OldPPTStatus = new PPTStatus();
            OldPPTStatus.Check();

            PPTTimer.Start();
        }
    }
}
