using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatAutoMapperGUI
{
    /// <summary>
    /// Contiene le impostazioni del programma.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Tempo di attesa per la sincronizzazione.
        /// </summary>
        public static TimeSpan SyncingWaitTime { get; set; }
        /// <summary>
        /// Indica se riprovare la sincronizzazione se fallita.
        /// </summary>
        public static bool RetrySyncing { get; set; }
        /// <summary>
        /// Numero di tentativi di sincronizzazione.
        /// </summary>
        public static byte SyncingRetries { get; set; }
        /// <summary>
        /// Numero di tentativi di riavvio.
        /// </summary>
        public static byte RestartRetries { get; set; }
    }
}