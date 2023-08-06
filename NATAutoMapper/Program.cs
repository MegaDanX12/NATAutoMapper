using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Xml;
using System.Diagnostics;
using System.Resources;
using System.Globalization;
using System.IO.Pipes;
using Open.Nat;

namespace NATAutoMapper
{
    class Program
    {
        /// <summary>
        /// Indica che il programma deve terminare.
        /// </summary>
        private static bool ExitProgram = false;
        /// <summary>
        /// Tempo, in millisecondi, per la ricerca di dispositivi NAT.
        /// </summary>
        private static int DiscoveryTimeout;
        /// <summary>
        /// Indica se selezionare il primo dispositivo NAT trovato.
        /// </summary>
        private static bool SelectFirstDiscoveredDevice;
        /// <summary>
        /// Indica se l'elaborazione di script è abilitata.
        /// </summary>
        private static bool ScriptingEnabled;
        static void Main(string[] args)
        {
            Console.Title = Properties.NatAutoMapper.TitleString;
            Console.WriteLine(Properties.NatAutoMapper.TitleString);
            Console.WriteLine();
            Console.WriteLine(Properties.NatAutoMapper.SettingsLoading);
            Console.WriteLine();
            LoadSettings();
            Console.WriteLine(Properties.NatAutoMapper.SettingsLoaded);
            Console.WriteLine();
            // Il programma inizia cercando dispositivi NAT che supportano UPnP.
            Console.WriteLine(Properties.NatAutoMapper.DeviceDiscovery);
            Console.WriteLine();
            NatDiscoverer Discoverer = new NatDiscoverer();
            CancellationTokenSource NatDeviceDiscoveryToken = new CancellationTokenSource(DiscoveryTimeout); // La ricerca termina dopo un certo tempo definito, in secondi, nelle impostazioni del programma.
            NatDevice[] Devices = Discoverer.DiscoverDevicesAsync(PortMapper.Upnp, NatDeviceDiscoveryToken).Result.ToArray();
            NatDeviceDiscoveryToken.Dispose();
            // Dopo la ricerca li visualizza sulla console permettendo all'utente di selezionare il dispositivo su cui effettuare le operazioni (se più di uno è presente).
            if (Devices.Length > 1)
            {
                Console.WriteLine(Properties.NatAutoMapper.DevicesFound1 + Devices.Length + Properties.NatAutoMapper.DevicesFound2);
                Console.WriteLine();
                for (int i = 0; i < Devices.Length; i++)
                {
                    Console.WriteLine(i + 1 + ")");
                    Console.WriteLine("\t" + Devices[i].ToString());
                    Console.WriteLine();
                }
                Console.WriteLine();
                if (SelectFirstDiscoveredDevice)
                {
                    PortOperationsMethods.SetNatDevice(Devices[0]);
                    Console.WriteLine(Properties.NatAutoMapper.FirstDeviceFoundSelected);
                }
                else
                {
                    Console.Write(Properties.NatAutoMapper.SelectDevice);
                    bool NatDeviceSelected = false;
                    while (!NatDeviceSelected)
                    {
                        ConsoleKeyInfo NatDevice = Console.ReadKey();
                        if (Convert.ToInt32(NatDevice.KeyChar) > Devices.Length)
                        {
                            Console.Write(Properties.NatAutoMapper.InvalidDeviceSelection);
                        }
                        else
                        {
                            PortOperationsMethods.SetNatDevice(Devices[Convert.ToInt32(NatDevice.KeyChar)]);
                            NatDeviceSelected = true;
                        }
                    }
                }
            }
            else if (Devices.Length == 1)
            {
                Console.WriteLine(Properties.NatAutoMapper.DeviceFound);
                Console.WriteLine();
                Console.WriteLine(Devices[0].ToString());
                PortOperationsMethods.SetNatDevice(Devices[0]);
            }
            else
            {
                Console.WriteLine(Properties.NatAutoMapper.NoDeviceFound);
                Console.ReadKey(true);
                Environment.Exit(0);
            }
            Console.WriteLine();
            // Se sono stati forniti parametri alla linea di comando, questi vengono elaborati e le operazioni eseguite in automatico (modalità automatica).
            if (args.Length > 0)
            {
                string Command = args[0].ToLowerInvariant();
                string[] Parameters = new string[args.Length - 1];
                Array.Copy(args, 1, Parameters, 0, Parameters.Length);
                int ExitCode = int.MaxValue;
                if (Command == "-gui")
                {
                    using (GUICommunication GuiCommunicationProcessor = new GUICommunication(Parameters[1], Parameters[3]))
                    {
                        GuiCommunicationProcessor.ProcessCommand();
                    }
                    Environment.Exit(0);
                }
                else if (Command == "-script")
                {
                    if (!ScriptingEnabled)
                    {
                        Console.WriteLine(Properties.NatAutoMapper.ScriptingDisabledWarning);
                        Console.ReadKey(true);
                        Environment.Exit(-1);
                    }
                    else
                    {
                        if (Path.GetExtension(args[1]) != ".nat")
                        {
                            Console.WriteLine(Properties.NatAutoMapper.InvalidScript);
                            ExitCode = int.MinValue;
                        }
                        else
                        {
                            Console.WriteLine(Properties.NatAutoMapper.ScriptRunning);
                            Console.WriteLine();
                            ExitCode = AutomaticProcessing.ProcessScript(args[1]);
                            if (ExitCode != 0)
                            {
                                Console.WriteLine(Properties.NatAutoMapper.ScriptFailed);
                                Console.ReadKey(true);
                            }
                            else
                            {
                                Console.WriteLine(Properties.NatAutoMapper.ScriptSucceded);
                                Console.ReadKey(true);
                            }
                        }
                    }
                    Environment.Exit(ExitCode);
                }
                else
                {
                    ExitCode = AutomaticProcessing.ProcessArguments(Command, Parameters);
                    if (Command == "-list")
                    {
                        switch (ExitCode)
                        {
                            case -1:
                                Console.WriteLine(Properties.NatAutoMapper.EnumerationError);
                                Console.ReadKey(true);
                                break;
                        }
                    }
                    else if (Command == "-add")
                    {
                        switch (ExitCode)
                        {
                            case -1:
                                Console.WriteLine(Properties.NatAutoMapper.InvalidProtocolError);
                                Console.ReadKey(true);
                                break;
                            case -2:
                                Console.WriteLine(Properties.NatAutoMapper.NotLocalIPError);
                                Console.ReadKey(true);
                                break;
                            case -3:
                                Console.WriteLine(Properties.NatAutoMapper.InvalidPrivatePortError);
                                Console.ReadKey(true);
                                break;
                            case -4:
                                Console.WriteLine(Properties.NatAutoMapper.InvalidPublicPortError);
                                Console.ReadKey(true);
                                break;
                            case -5:
                                Console.WriteLine(Properties.NatAutoMapper.InvalidLifetimeError);
                                Console.ReadKey(true);
                                break;
                            case -6:
                                Console.WriteLine(Properties.NatAutoMapper.NoneOrInvalidParameters);
                                Console.ReadKey(true);
                                break;
                            case 1:
                                Console.WriteLine(Properties.NatAutoMapper.EnumerationError);
                                Console.ReadKey(true);
                                break;
                            case 2:
                                Console.WriteLine(Properties.NatAutoMapper.AlreadyExistingError);
                                Console.ReadKey(true);
                                break;
                            case 3:
                                Console.WriteLine(Properties.NatAutoMapper.CreationError);
                                Console.ReadKey(true);
                                break;
                        }
                    }
                    else if (Command == "-remove")
                    {
                        switch (ExitCode)
                        {
                            case 1:
                                Console.WriteLine(Properties.NatAutoMapper.EnumerationError);
                                Console.ReadKey(true);
                                break;
                            case 2:
                                Console.WriteLine(Properties.NatAutoMapper.DeletionError);
                                Console.ReadKey(true);
                                break;
                            case 3:
                                Console.WriteLine(Properties.NatAutoMapper.NoOtherRuleFound);
                                Console.ReadKey(true);
                                break;
                            case 4:
                                Console.WriteLine(Properties.NatAutoMapper.NoRuleFound);
                                Console.ReadKey(true);
                                break;
                            case -1:
                                Console.WriteLine(Properties.NatAutoMapper.NoOperationSelected);
                                Console.ReadKey(true);
                                break;
                            case -6:
                                Console.WriteLine(Properties.NatAutoMapper.NoneOrInvalidParameters);
                                Console.ReadKey(true);
                                break;
                        }
                    }
                    Environment.Exit(ExitCode);
                }
            }
            // Se non sono stati forniti parametri, il programma entra nella modalità manuale.
            ConsoleKeyInfo Operation; // Questa variabile contiene informazioni sul tasto premuto dall'utente per selezionare un'opzione.
            Console.WriteLine(Properties.NatAutoMapper.MainMenuSelection);
            // Il programma continua a visualizzare le opzioni fino a quando non si chiede esplicitamente l'uscita.
            do
            {
                Console.WriteLine();
                Console.WriteLine(Properties.NatAutoMapper.MainMenuCommand0);
                Console.WriteLine(Properties.NatAutoMapper.MainMenuCommand1);
                Console.WriteLine(Properties.NatAutoMapper.MainMenuCommand2);
                Console.WriteLine(Properties.NatAutoMapper.MainMenuCommand3);
                Console.WriteLine(Properties.NatAutoMapper.MainMenuCommand4);
                Console.WriteLine(Properties.NatAutoMapper.MainMenuCommand5);
                Console.WriteLine(Properties.NatAutoMapper.MainMenuCommand6);
                Console.WriteLine();
                Console.Write(Properties.NatAutoMapper.UserSelection);
                Operation = Console.ReadKey(false);
                Console.WriteLine();
                Console.WriteLine();
                switch (Operation.Key)
                {
                    case ConsoleKey.D0:
                    case ConsoleKey.NumPad0:
                        ExitProgram = true;
                        break;
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        PortOperationsMethods.ShowAllPortMappings();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        PortOperationsMethods.OpenPort();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        PortOperationsMethods.OpenKnownPort();
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        PortOperationsMethods.ClosePort();
                        break;
                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        ClearScreen();
                        break;
                    case ConsoleKey.D6:
                    case ConsoleKey.NumPad6:
                        EditSettings();
                        break;
                    default:
                        Console.WriteLine(Properties.NatAutoMapper.InvalidOperationSelection);
                        break;
                }
            }
            while (!ExitProgram);
        }
        /// <summary>
        /// Elenca le impostazioni del programma.
        /// </summary>
        private static void EditSettings()
        {
            bool SettingsEdited = false;
            do
            {
                Console.WriteLine();
                Console.WriteLine(Properties.NatAutoMapper.CurrentSettings);
                Console.WriteLine();
                Console.WriteLine(Properties.NatAutoMapper.SettingsName1 + DiscoveryTimeout / 1000);
                Console.Write(Properties.NatAutoMapper.SettingsName2);
                if (SelectFirstDiscoveredDevice)
                {
                    Console.WriteLine(Properties.NatAutoMapper.SettingsYes);
                }
                else
                {
                    Console.WriteLine(Properties.NatAutoMapper.SettingsNo);
                }
                Console.Write(Properties.NatAutoMapper.SettingsName3);
                if (ScriptingEnabled)
                {
                    Console.WriteLine(Properties.NatAutoMapper.SettingsYes);
                }
                else
                {
                    Console.WriteLine(Properties.NatAutoMapper.SettingsNo);
                }
                Console.WriteLine();
                Console.Write(Properties.NatAutoMapper.SettingsEditSelection);
                ConsoleKeyInfo Key = Console.ReadKey(false);
                Console.WriteLine();
                switch (Key.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        Console.Write(Properties.NatAutoMapper.DiscoveryTimeSettingEdit);
                        string NewSetting = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(NewSetting) || NewSetting == "0")
                        {
                            DiscoveryTimeout = 5000;
                        }
                        else
                        {
                            DiscoveryTimeout = int.Parse(NewSetting, CultureInfo.InvariantCulture);
                        }
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        if (SelectFirstDiscoveredDevice)
                        {
                            Console.WriteLine(Properties.NatAutoMapper.FirstDeviceSelectionSettingDisabled);
                            SelectFirstDiscoveredDevice = false;
                        }
                        else
                        {
                            Console.WriteLine(Properties.NatAutoMapper.FirstDeviceSelectionSettingEnabled);
                            SelectFirstDiscoveredDevice = true;
                        }
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        if (ScriptingEnabled)
                        {
                            Console.WriteLine(Properties.NatAutoMapper.ScriptingSettingDisabled);
                            ScriptingEnabled = false;
                        }
                        else
                        {
                            Console.WriteLine(Properties.NatAutoMapper.ScriptingSettingEnabled);
                            ScriptingEnabled = true;
                        }
                        break;
                    case ConsoleKey.D0:
                    case ConsoleKey.NumPad0:
                        UpdateSettings();
                        SettingsEdited = true;
                        break;
                }
            }
            while (!SettingsEdited);
        }
        /// <summary>
        /// Pulisce la finestra del programma.
        /// </summary>
        private static void ClearScreen()
        {
            Console.Clear();
            Console.WriteLine(Properties.NatAutoMapper.TitleString);
            Console.WriteLine();
        }
        /// <summary>
        /// Aggiorna le impostazioni del programma.
        /// </summary>
        private static void UpdateSettings()
        {
            XmlDocument doc = new XmlDocument()
            {
                XmlResolver = null
            };
            doc.Load("AppSettings.xml");
            XmlNode DiscoveryTimeoutNode = doc.SelectSingleNode("//AppSettings/NatDiscovery/Timeout");
            DiscoveryTimeoutNode.InnerText = DiscoveryTimeout.ToString(CultureInfo.InvariantCulture);
            XmlNode SelectFirstDiscoveredDeviceNode = doc.SelectSingleNode("//AppSettings/NatDiscovery/SelectFirstDeviceFound");
            SelectFirstDiscoveredDeviceNode.InnerText = SelectFirstDiscoveredDevice.ToString(CultureInfo.InvariantCulture);
            XmlNode ScriptingEnabledNode = doc.SelectSingleNode("//AppSettings/Scripting/EnableScripting");
            ScriptingEnabledNode.InnerText = ScriptingEnabled.ToString(CultureInfo.InvariantCulture);
            doc.Save("AppSettings.xml");
        }
        /// <summary>
        /// Carica le impostazioni del programma.
        /// </summary>
        private static void LoadSettings()
        {
            if (!File.Exists("AppSettings.xml"))
            {
                CreateDefaultSettings();
                XmlDocument doc = new XmlDocument()
                {
                    XmlResolver = null
                };
                doc.Load("AppSettings.xml");
                DiscoveryTimeout = int.Parse(doc.SelectSingleNode("//AppSettings/NatDiscovery/Timeout").InnerText, CultureInfo.InvariantCulture);
                SelectFirstDiscoveredDevice = bool.Parse(doc.SelectSingleNode("//AppSettings/NatDiscovery/SelectFirstDeviceFound").InnerText);
                ScriptingEnabled = bool.Parse(doc.SelectSingleNode("//AppSettings/Scripting/EnableScripting").InnerText);
            }
            else
            {
                XmlDocument doc = new XmlDocument()
                {
                    XmlResolver = null
                };
                doc.Load("AppSettings.xml");
                DiscoveryTimeout = int.Parse(doc.SelectSingleNode("//AppSettings/NatDiscovery/Timeout").InnerText, CultureInfo.InvariantCulture);
                SelectFirstDiscoveredDevice = bool.Parse(doc.SelectSingleNode("//AppSettings/NatDiscovery/SelectFirstDeviceFound").InnerText);
                ScriptingEnabled = bool.Parse(doc.SelectSingleNode("//AppSettings/Scripting/EnableScripting").InnerText);
            }
        }
        /// <summary>
        /// Genera il file di impostazioni di default.
        /// </summary>
        private static void CreateDefaultSettings()
        {
            XmlDocument doc = new XmlDocument
            {
                XmlResolver = null
            };
            XmlNode RootNode = doc.CreateElement("AppSettings");
            doc.AppendChild(RootNode);
            XmlNode NATDiscoverySettingsNode = doc.CreateElement("NatDiscovery");
            RootNode.AppendChild(NATDiscoverySettingsNode);
            XmlNode DiscoveryTimeoutNode = doc.CreateElement("Timeout");
            DiscoveryTimeoutNode.InnerText = 5000.ToString(CultureInfo.InvariantCulture);
            NATDiscoverySettingsNode.AppendChild(DiscoveryTimeoutNode);
            XmlNode SelectFirstDeviceFoundNode = doc.CreateElement("SelectFirstDeviceFound");
            SelectFirstDeviceFoundNode.InnerText = false.ToString(CultureInfo.InvariantCulture);
            NATDiscoverySettingsNode.AppendChild(SelectFirstDeviceFoundNode);
            XmlNode ScriptingSettingsNode = doc.CreateElement("Scripting");
            RootNode.AppendChild(ScriptingSettingsNode);
            XmlNode EnableScriptingNode = doc.CreateElement("EnableScripting");
            EnableScriptingNode.InnerText = true.ToString(CultureInfo.InvariantCulture);
            ScriptingSettingsNode.AppendChild(EnableScriptingNode);
            doc.Save("AppSettings.xml");
        }
    }
}