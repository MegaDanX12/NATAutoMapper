using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Globalization;

namespace NatAutoMapperGUI
{
    /// <summary>
    /// Informazioni su un regola.
    /// </summary>
    public struct RuleInfo
    {
        /// <summary>
        /// Nome.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Protocollo.
        /// </summary>
        public string Protocol { get; set; }
        /// <summary>
        /// Indirizzo IP.
        /// </summary>
        public IPAddress Address { get; set; }
        /// <summary>
        /// Porta interna.
        /// </summary>
        public int PrivatePort { get; set; }
        /// <summary>
        /// Porta pubblica.
        /// </summary>
        public int PublicPort { get; set; }
        /// <summary>
        /// Tempo di vita rimanente.
        /// </summary>
        public int RemainingLifetime { get; set; }
    }

    /// <summary>
    /// Gestisce la comunicazione con lo strumento.
    /// </summary>
    public static class ToolCommunication
    {
        /// <summary>
        /// Pipe di entrata.
        /// </summary>
        private static AnonymousPipeServerStream InputPipe;
        /// <summary>
        /// Pipe di uscita.
        /// </summary>
        private static AnonymousPipeServerStream OutputPipe;
        /// <summary>
        /// Oggetto per la lettura della pipe di entrata
        /// </summary>
        private static StreamReader ToolReader;
        /// <summary>
        /// Oggetto per la scrittura della pipe di uscita.
        /// </summary>
        private static StreamWriter ToolWriter;
        /// <summary>
        /// Prepara la comunicazione tra l'interfaccia utente e lo strumento.
        /// </summary>
        public static void Initialize()
        {
            InputPipe = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
            OutputPipe = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
            ToolReader = new StreamReader(InputPipe);
            ToolWriter = new StreamWriter(OutputPipe)
            {
                AutoFlush = true
            };
        }
        /// <summary>
        /// Recupera gli handle dei pipe.
        /// </summary>
        /// <returns></returns>
        public static string[] GetPipeHandles()
        {
            string[] Handles = new string[2];
            Handles[0] = InputPipe.GetClientHandleAsString();
            Handles[1] = OutputPipe.GetClientHandleAsString();
            return Handles;
        }
        /// <summary>
        /// Resetta la comunicazione tra l'interfaccia utente e lo strumento.
        /// </summary>
        public static void Reinitialize()
        {
            ToolReader.Dispose();
            ToolWriter.Dispose();
            Initialize();
        }
        /// <summary>
        /// Sincronizza i processi dell'interfaccia utente e dello strumento.
        /// </summary>
        /// <returns></returns>
        public static bool SyncProcesses()
        {
            Task<string> SyncTask = Task.Run(ToolReader.ReadLine);
            string Message = string.Empty;
            if (SyncTask.Wait(Settings.SyncingWaitTime))
            {
                Message = SyncTask.Result;
            }
            if (Message == "ready")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Recupera l'indirizzo IP locale del computer.
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {
            ToolWriter.WriteLine("localip");
            string LocalIP = ToolReader.ReadLine();
            if (LocalIP == "error")
            {
                return string.Empty;
            }
            else
            {
                return LocalIP;
            }
        }
        /// <summary>
        /// Controlla se l'IP fornito è valido.
        /// </summary>
        /// <returns></returns>
        public static bool IsLocalIP(string IPAddress)
        {
            ToolWriter.WriteLine("checkip");
            string Response = ToolReader.ReadLine();
            if (Response == "ready")
            {
                ToolWriter.WriteLine(IPAddress);
            }
            Response = ToolReader.ReadLine();
            if (Response == "valid")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Recupera le regole attualmente impostate.
        /// </summary>
        /// <returns></returns>
        public static RuleInfo[] RetrieveCurrentRulesList()
        {
            List<RuleInfo> CurrentRules = new List<RuleInfo>();
            ToolWriter.WriteLine("rules");
            string[] Response = ToolReader.ReadLine().Split(';');
            foreach (string rule in Response)
            {
                RuleInfo Info = new RuleInfo();
                string[] RuleParts = rule.Split(',');
                foreach (string part in RuleParts)
                {
                    string[] RuleData = part.Split(':');
                    switch (RuleData[0])
                    {
                        case "Name":
                            Info.Name = RuleData[1];
                            break;
                        case "Protocol":
                            Info.Protocol = RuleData[1];
                            break;
                        case "Address":
                            Info.Address = IPAddress.Parse(RuleData[1]);
                            break;
                        case "PrivatePort":
                            Info.PrivatePort = int.Parse(RuleData[1], CultureInfo.InvariantCulture);
                            break;
                        case "PublicPort":
                            Info.PublicPort = int.Parse(RuleData[1], CultureInfo.InvariantCulture);
                            break;
                        case "RemainingLifetime":
                            Info.RemainingLifetime = int.Parse(RuleData[1], CultureInfo.InvariantCulture);
                            break;
                    }
                }
                CurrentRules.Add(Info);
            }
            return CurrentRules.ToArray();
        }
        /// <summary>
        /// Apre una porta.
        /// </summary>
        /// <param name="Name">Nome della regola.</param>
        /// <param name="Protocol">Protocollo</param>
        /// <param name="Address">Indirizzo IP.</param>
        /// <param name="PrivatePort">Porta interna.</param>
        /// <param name="PublicPort">Porta pubblica.</param>
        /// <param name="Lifetime">Tempo di vita.</param>
        public static void OpenPort(string Name, string Protocol, string Address, int PrivatePort, int PublicPort, int Lifetime)
        {
            ToolWriter.WriteLine("open");
            string Response = ToolReader.ReadLine();
            if (Response == "ready")
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Name:" + Name + ",");
                sb.Append("Protocol:" + Protocol + ",");
                sb.Append("Address:" + Address + ",");
                sb.Append("PrivatePort:" + PrivatePort + ",");
                sb.Append("PublicPort:" + PublicPort + ",");
                sb.Append("RemainingLifetime:" + Lifetime);
                ToolWriter.WriteLine(sb.ToString());
            }
        }
        /// <summary>
        /// Apre una serie di porte.
        /// </summary>
        /// <param name="Name">Nome della regola.</param>
        /// <param name="Protocol">Protocollo.</param>
        /// <param name="Address">Indirizzo IP.</param>
        /// <param name="PrivatePortRange">Raggio di porte private.</param>
        /// <param name="PublicPortRange">Raggio di porte pubbliche</param>
        /// <param name="Lifetime">Tempo di vita.</param>
        public static void OpenPortRange(string Name, string Protocol, string Address, string PrivatePortRange, string PublicPortRange, int Lifetime)
        {
            ToolWriter.WriteLine("openrange");
            string Response = ToolReader.ReadLine();
            if (Response == "ready")
            {
                string[] PrivateRange = PrivatePortRange.Split('-');
                string[] PublicRange = PublicPortRange.Split('-');
                StringBuilder sb = new StringBuilder();
                sb.Append("Name:" + Name + ",");
                sb.Append("Protocol:" + Protocol + ",");
                sb.Append("Address:" + Address + ",");
                sb.Append("PrivatePortStart:" + PrivateRange[0] + ",");
                sb.Append("PublicPortStart:" + PublicRange[0] + ",");
                sb.Append("PrivatePortEnd:" + PrivateRange[1] + ",");
                sb.Append("PublicPortEnd:" + PublicRange[1] + ",");
                sb.Append("RemainingLifetime:" + Lifetime);
                ToolWriter.WriteLine(sb.ToString());
            }
        }
        /// <summary>
        /// Recupera la lista di servizi noti.
        /// </summary>
        /// <returns></returns>
        public static string[] RetrieveKnownPortsList()
        {
            ToolWriter.WriteLine("knownrules");
            string Response = ToolReader.ReadLine();
            return Response.Split(',');
        }
        /// <summary>
        /// Chiude una porta.
        /// </summary>
        /// <param name="Name">Nome della regola.</param>
        public static void ClosePort(string Name)
        {
            ToolWriter.WriteLine("close");
            string Response = ToolReader.ReadLine();
            if (Response == "ready")
            {
                ToolWriter.WriteLine(Name);
            }
        }
        /// <summary>
        /// Esegue uno script.
        /// </summary>
        /// <param name="FilePath">Percorso del file di script da eseguire.</param>
        public static void RunScript(string FilePath)
        {
            ToolWriter.WriteLine("runscript");
            string Response = ToolReader.ReadLine();
            if (Response == "ready")
            {
                ToolWriter.WriteLine(FilePath);
            }
        }
        /// <summary>
        /// Chiude lo strumento di mappatura porte.
        /// </summary>
        public static void TerminateTool()
        {
            ToolWriter.WriteLine("exit");
        }
    }
}