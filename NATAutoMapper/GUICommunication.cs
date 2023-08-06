using System;
using System.Text;
using System.IO;
using System.IO.Pipes;
using Open.Nat;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;

namespace NATAutoMapper
{
    /// <summary>
    /// Gestisce la comunicazione con l'interfaccia utente.
    /// </summary>
    public class GUICommunication : IDisposable
    {
        private bool _disposed = false;

        /// <summary>
        /// Pipe in entrata.
        /// </summary>
        private readonly AnonymousPipeClientStream _inputPipe;

        /// <summary>
        /// Pipe in uscita.
        /// </summary>
        private readonly AnonymousPipeClientStream _outputPipe;

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="GUICommunication"/> con gli handle di pipe forniti.
        /// </summary>
        /// <param name="inputPipe">Handle del pipe per i dati in entrata.</param>
        /// <param name="outputPipe">Handle del pipe per i dati in uscita.</param>
        public GUICommunication(string inputPipe, string outputPipe)
        {
            this._inputPipe = new AnonymousPipeClientStream(PipeDirection.In, inputPipe);
            this._outputPipe = new AnonymousPipeClientStream(PipeDirection.Out, outputPipe);
        }

        /// <summary>
        /// Elabora i comandi in arrivo.
        /// </summary>
        public void ProcessCommand()
        {
            bool exit = false;
            using (StreamWriter sw = new StreamWriter(_outputPipe)
            {
                AutoFlush = true
            })
            {
                using (StreamReader sr = new StreamReader(_inputPipe))
                {
                    string ruleData = null;
                    string[] ruleParts;
                    string name = null;
                    Protocol protocol = Protocol.Tcp;
                    IPAddress address = null;
                    int privatePort = 0;
                    int publicPort = 0;
                    int privateRangeStart = 0;
                    int publicRangeStart = 0;
                    int privateRangeEnd = 0;
                    int publicRangeEnd = 0;
                    int lifetime = 0;
                    StringBuilder sb = null;
                    while (!exit)
                    {
                        sw.WriteLine("ready");
                        string Command = sr.ReadLine();
                        switch (Command)
                        {
                            case "exit":
                                exit = true;
                                break;
                            case "localip":
                                try
                                {
                                    sw.WriteLine(UtilityMethods.GetLocalIP().ToString());
                                }
                                catch (NetworkInformationException)
                                {
                                    sw.WriteLine("error");
                                }
                                break;
                            case "checkip":
                                sw.WriteLine("ready");
                                string ipAddress = sr.ReadLine();
                                try
                                {
                                    bool IsValid = UtilityMethods.IsAddressLocal(IPAddress.Parse(ipAddress));
                                    if (IsValid)
                                    {
                                        sw.WriteLine("valid");
                                    }
                                    else
                                    {
                                        sw.WriteLine("invalid");
                                    }
                                }
                                catch (FormatException)
                                {
                                    sw.WriteLine("invalid");
                                }
                                break;
                            case "rules":
                                Mapping[] mappings = PortOperationsMethods.RetrieveRules();
                                sb = new StringBuilder();
                                if (mappings.Length == 1)
                                {
                                    _ = sb.Append("Name:" + mappings[0].Description + ",");
                                    _ = sb.Append("Protocol:" + mappings[0].Protocol.ToString() + ",");
                                    _ = sb.Append("Address:" + mappings[0].PrivateIP.ToString() + ",");
                                    _ = sb.Append("PrivatePort:" + mappings[0].PrivatePort.ToString(CultureInfo.InvariantCulture) + ",");
                                    _ = sb.Append("PublicPort:" + mappings[0].PublicPort.ToString(CultureInfo.InvariantCulture) + ",");
                                    _ = sb.Append("RemainingLifetime:" + mappings[0].Lifetime.ToString(CultureInfo.InvariantCulture));
                                }
                                else
                                {
                                    for (int i = 0; i < mappings.Length; i++)
                                    {
                                        if (i < mappings.Length - 1)
                                        {
                                            _ = sb.Append("Name:" + mappings[i].Description + ",");
                                            _ = sb.Append("Protocol:" + mappings[i].Protocol.ToString() + ",");
                                            _ = sb.Append("Address:" + mappings[i].PrivateIP.ToString() + ",");
                                            _ = sb.Append("PrivatePort:" + mappings[i].PrivatePort.ToString(CultureInfo.InvariantCulture) + ",");
                                            _ = sb.Append("PublicPort:" + mappings[i].PublicPort.ToString(CultureInfo.InvariantCulture) + ",");
                                            _ = sb.Append("RemainingLifetime:" + mappings[i].Lifetime.ToString(CultureInfo.InvariantCulture));
                                            sb.Append(';');
                                        }
                                        else if (i == mappings.Length - 1)
                                        {
                                            _ = sb.Append("Name:" + mappings[i].Description + ",");
                                            _ = sb.Append("Protocol:" + mappings[i].Protocol.ToString() + ",");
                                            _ = sb.Append("Address:" + mappings[i].PrivateIP.ToString() + ",");
                                            _ = sb.Append("PrivatePort:" + mappings[i].PrivatePort.ToString(CultureInfo.InvariantCulture) + ",");
                                            _ = sb.Append("PublicPort:" + mappings[i].PublicPort.ToString(CultureInfo.InvariantCulture) + ",");
                                            _ = sb.Append("RemainingLifetime:" + mappings[i].Lifetime.ToString(CultureInfo.InvariantCulture));
                                        }
                                    }
                                }
                                sw.WriteLine(sb.ToString());
                                break;
                            case "open":
                                sw.WriteLine("ready");
                                ruleData = sr.ReadLine();
                                ruleParts = ruleData.Split(',');
                                foreach (string part in ruleParts)
                                {
                                    string[] values = part.Split(':');
                                    switch (values[0])
                                    {
                                        case "Name":
                                            name = values[1];
                                            break;
                                        case "Protocol":
                                            if (values[1] == "UDP")
                                            {
                                                protocol = Protocol.Udp;
                                            }
                                            break;
                                        case "Address":
                                            address = IPAddress.Parse(values[1]);
                                            break;
                                        case "PrivatePort":
                                            privatePort = int.Parse(values[1], CultureInfo.InvariantCulture);
                                            break;
                                        case "PublicPort":
                                            publicPort = int.Parse(values[1], CultureInfo.InvariantCulture);
                                            break;
                                        case "RemainingLifetime":
                                            lifetime = int.Parse(values[1], CultureInfo.InvariantCulture);
                                            break;
                                    }
                                }
                                Mapping Mapping = new Mapping(protocol, address, privatePort, publicPort, lifetime, name);
                                PortOperationsMethods.OpenPort(Mapping);
                                break;
                            case "close":
                                sw.WriteLine("ready");
                                string ruleName = sr.ReadLine();
                                PortOperationsMethods.ClosePort(ruleName);
                                break;
                            case "runscript":
                                sw.WriteLine("ready");
                                string scriptPath = sr.ReadLine();
                                AutomaticProcessing.ProcessScript(scriptPath);
                                break;
                            case "openrange":
                                sw.WriteLine("ready");
                                ruleData = sr.ReadLine();
                                ruleParts = ruleData.Split(',');
                                foreach (string part in ruleParts)
                                {
                                    string[] values = part.Split(':');
                                    switch (values[0])
                                    {
                                        case "Name":
                                            name = values[1];
                                            break;
                                        case "Protocol":
                                            if (values[1] == "UDP")
                                            {
                                                protocol = Protocol.Udp;
                                            }
                                            break;
                                        case "Address":
                                            address = IPAddress.Parse(values[1]);
                                            break;
                                        case "PrivateRangeStart":
                                            privateRangeStart = int.Parse(values[1], CultureInfo.InvariantCulture);
                                            break;
                                        case "PublicRangeStart":
                                            publicRangeStart = int.Parse(values[1], CultureInfo.InvariantCulture);
                                            break;
                                        case "PrivateRangeEnd":
                                            privateRangeEnd = int.Parse(values[1], CultureInfo.InvariantCulture);
                                            break;
                                        case "PublicRangeEnd":
                                            publicRangeEnd = int.Parse(values[1], CultureInfo.InvariantCulture);
                                            break;
                                        case "RemainingLifetime":
                                            lifetime = int.Parse(values[1], CultureInfo.InvariantCulture);
                                            break;
                                    }
                                }
                                // Metodo mancante
                                break;
                            case "knownservices":
                                string[] knownRules = KnownPorts.ListKnownPortMappings().ToArray();
                                sb = new StringBuilder();
                                for (int i = 0; i < knownRules.Length; i++)
                                {
                                    if (i < knownRules.Length - 1)
                                    {
                                        _ = sb.Append(knownRules[i] + ",");
                                    }
                                    else if (i == knownRules.Length - 1)
                                    {
                                        sb.Append(knownRules[i]);
                                    }
                                }
                                sw.WriteLine(sb.ToString());
                                break;
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                _inputPipe.Dispose();
                _outputPipe.Dispose();
            }
            _disposed = true;
        }
    }
}