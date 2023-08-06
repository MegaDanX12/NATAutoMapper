using System;
using System.Collections.Generic;
using System.Net;
using Open.Nat;

namespace NATAutoMapper
{
    /// <summary>
    /// Contiene le definizioni dei servizi noti e i metodi per aprire e chiudere le relative porte su un dispositivo NAT.
    /// </summary>
    public static class KnownPorts
    {
        /// <summary>
        /// Informazioni già note sui servizi noti.
        /// </summary>
        private static readonly Dictionary<string, (Protocol Protocol, int PublicPort)> _knownPortsData = new Dictionary<string, (Protocol, int)>()
        {
            {"TCP Multiplexor", (Protocol.Tcp, 1) },
            {"compressnet Management Utility", (Protocol.Tcp, 2) },
            {"compressnet Compression Process", (Protocol.Tcp, 3) },
            {"Echo Protocol", (Protocol.Udp, 7) },
            {"Bif Protocol", (Protocol.Udp, 8) },
            {"Discard Protocol", (Protocol.Udp, 9) },
            {"Daytime Protocol", (Protocol.Tcp, 13) },
            {"Quote of the Day", (Protocol.Tcp, 17) },
            {"Chargen Protocol", (Protocol.Udp, 19) },
            {"File Trasfer Protocol (FTP) data", (Protocol.Tcp, 20) },
            {"File Transfer Protocol (FTP) control", (Protocol.Tcp, 21) },
            {"Secure Shell (SSH)", (Protocol.Tcp, 22) },
            {"Telnet insecure text communications", (Protocol.Tcp, 23) },
            {"SMTP", (Protocol.Tcp, 25) },
            {"DNS", (Protocol.Udp, 53) },
            {"Bootstrap Protocol (BOOTP) server e DHCP server", (Protocol.Udp, 67) },
            {"Bootstrap Protocol (BOOTP) client e DHCP client", (Protocol.Udp, 68) },
            {"Trivial File Transfer Protocol (TFTP)", (Protocol.Udp, 69) },
            {"Gopher", (Protocol.Tcp, 70) },
            {"Finger", (Protocol.Tcp, 79) },
            {"HyperText Transfer Protocol (HTTP)", (Protocol.Tcp, 80) },
            {"Kerberos Authenticating Agent", (Protocol.Tcp, 88) },
            {"Digital Imaging and Communications in Medicine (DICOM)", (Protocol.Tcp, 104) },
            {"Post Office Protocol (POP3)", (Protocol.Tcp, 110) },
            {"Network News Transfer Protocol (NNTP)", (Protocol.Tcp, 119) },
            {"Sincronizzazione orologio (NTP)", (Protocol.Udp, 123) },
            {"NetBIOS Name Service", (Protocol.Udp, 137) },
            {"NetBIOS Datagram Service", (Protocol.Udp, 138) },
            {"NetBIOS Session Service", (Protocol.Tcp, 139) },
            {"Internet Message Access Protocol (IMAP4)", (Protocol.Tcp, 143) },
            {"Simple Network Management Protocol (SNMP) agent", (Protocol.Udp, 161) },
            {"Simple Network Management Protocol (SNMP) manager", (Protocol.Udp, 162) },
            {"LDAP", (Protocol.Tcp, 389) },
            {"Direct Connect", (Protocol.Tcp, 411) },
            {"HyperText Transfer Protocol Secure (HTTPS)", (Protocol.Tcp, 443) },
            {"Microsoft-DS SMB file sharing", (Protocol.Udp, 445) },
            {"Simple Mail Transfer Protocol (SMTP) su SSL", (Protocol.Tcp, 465) },
            {"Modbus", (Protocol.Tcp, 502) },
            {"SysLog", (Protocol.Udp, 514) },
            {"Real Time Streaming Protocol (RTSP)", (Protocol.Udp, 554) },
            {"Network News Transfer Protocol (NNTP) su SSL", (Protocol.Tcp, 563) },
            {"e-mail message submission (SMTP)", (Protocol.Tcp, 587) },
            {"FileMaker 6.0 Web Sharing", (Protocol.Tcp, 591) },
            {"Common Unix Printing System (IPP/CUPS)", (Protocol.Udp, 631) },
            {"LDAP su SSL", (Protocol.Tcp, 636) },
            {"Doom", (Protocol.Tcp, 666) },
            {"Internet Message Access Protocol (IMAP4) su SSL", (Protocol.Tcp, 993) },
            {"Post Office Protocol (POP3) su SSL", (Protocol.Tcp, 995) },
            {"SOCKS Proxy", (Protocol.Tcp, 1080) },
            {"OpenVPN", (Protocol.Udp, 1194) },
            {"MQTT", (Protocol.Tcp, 1883) },
            {"Microsoft SQL Server", (Protocol.Tcp, 1433) },
            {"Microsoft SQL Monitor (TCP)", (Protocol.Tcp, 1434) },
            {"Microsoft SQL Monitor (UDP)", (Protocol.Udp, 1434) },
            {"Big Brother", (Protocol.Tcp, 1984) },
            {"Network File System", (Protocol.Udp, 2049) },
            {"rtcm-sc104 (correzioni differenziali gps) (TCP)", (Protocol.Tcp, 2101) },
            {"rtcm-sc104 (correzioni differenziali gps) (UDP)", (Protocol.Udp, 2101) },
            {"DICOM Integrated Secure Communication Layer (ISCL) (TCP)", (Protocol.Tcp, 2761) },
            {"DICOM Integrated Secure Communication Layer (ISCL) (UDP)", (Protocol.Udp, 2761) },
            {"DICOM TLS (TCP)", (Protocol.Tcp, 2762) },
            {"DICOM TLS (UDP)", (Protocol.Udp, 2762) },
            {"Firebird Database System", (Protocol.Tcp, 3050) },
            {"HTTP web cache e Squid cache default", (Protocol.Tcp, 3128) },
            {"MySQL Database System", (Protocol.Tcp, 3306) },
            {"Desktop Remoto di Windows e Microsoft Terminal Server (RDP)", (Protocol.Tcp, 3389) },
            {"Voispeed (1)", (Protocol.Tcp, 3541) },
            {"Voispeed (2)", (Protocol.Tcp, 3542) },
            {"Subversion (TCP)", (Protocol.Tcp, 3690) },
            {"Subversion (UDP)", (Protocol.Udp, 3690) },
            {"eMule (< v0.47) (TCP)", (Protocol.Tcp, 4662) },
            {"eMule (< v0.47) (UDP)", (Protocol.Udp, 4672) },
            {"eMule Web Server", (Protocol.Tcp, 4711) },
            {"Radmin Connessione Remota", (Protocol.Tcp, 4899) },
            {"Sybase database server", (Protocol.Tcp, 5000) },
            {"SIP (TCP)", (Protocol.Tcp, 5060) },
            {"SIP (UDP)", (Protocol.Udp, 5060) },
            {"EPCglobal Low-Level Reader Protocol (LLRP) (TCP)", (Protocol.Tcp, 5084) },
            {"EPCglobal Low-Level Reader Protocol (LLRP) (UDP)", (Protocol.Udp, 5084) },
            {"EPCglobal Low-Level Reader Protocol (LLRP) criptato (TCP)", (Protocol.Tcp, 5085) },
            {"EPCglobal Low-Level Reader Protocol (LLRP) criptato (UDP)", (Protocol.Udp, 5085) },
            {"AOL e AOL Instant Messenger", (Protocol.Tcp, 5190) },
            {"XMPP Client Connection", (Protocol.Tcp, 5222) },
            {"XMPP Server Connection", (Protocol.Tcp, 5269) },
            {"PostgreSQL Database system", (Protocol.Tcp, 5432) },
            {"Symantec PcAnywhere (TCP)", (Protocol.Tcp, 5631) },
            {"Symantec PcAnywhere (UDP)", (Protocol.Udp, 5632) },
            {"Ultra VNC (http) (1)", (Protocol.Tcp, 5800) },
            {"Ultra VNC (http) (2)", (Protocol.Tcp, 5900) },
            {"X11 (X-windows)", (Protocol.Tcp, 6000) },
            {"SANE", (Protocol.Tcp, 6566) },
            {"Internet Relay Chat (IRC)", (Protocol.Tcp, 6667) },
            {"iRDMI", (Protocol.Tcp, 8000) },
            {"HTTP Alternate o web cache", (Protocol.Tcp, 8080) },
            {"HTTP Filtering Proxy Service", (Protocol.Tcp, 8118) },
            {"MQTT con SSL", (Protocol.Tcp, 8883) },
            {"TVersity Media Server (TCP)", (Protocol.Tcp, 41951) },
            {"TVersity Media Server (UDP)", (Protocol.Udp, 41951) }
        };

        /// <summary>
        /// Enumera le regole di port mapping note in base al filtro specificato (se presente).
        /// </summary>
        /// <param name="filter">Filtro.</param>
        /// <returns>Una lista delle regole di port mapping note.</returns>
        public static List<string> ListKnownPortMappings(string filter = null)
        {
            List<string> knownServices = new List<string>();
            if (filter == null)
            {
                foreach (string service in _knownPortsData.Keys)
                {
                    knownServices.Add(service);
                }
            }
            else
            {
                if (filter == "TCP" || filter == "tcp" || filter == "UDP" || filter == "udp")
                {
                    string protocol = filter.ToLowerInvariant();
                    Protocol protocolEnum = Protocol.Tcp;
                    switch (protocol)
                    {
                        case "tcp":
                            protocolEnum = Protocol.Tcp;
                            break;
                        case "udp":
                            protocolEnum = Protocol.Udp;
                            break;
                    }
                    foreach (KeyValuePair<string, (Protocol KnownProtocol, int PublicPort)> pair in _knownPortsData)
                    {
                        if (pair.Value.KnownProtocol == protocolEnum)
                        {
                            knownServices.Add(pair.Key);
                        }
                    }
                }
                else if (int.TryParse(filter, out int Port))
                {
                    foreach (KeyValuePair<string, (Protocol KnownProtocol, int PublicPort)> pair in _knownPortsData)
                    {
                        if (pair.Value.PublicPort == Port)
                        {
                            knownServices.Add(pair.Key);
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, (Protocol KnownProtocol, int PublicPort)> pair in _knownPortsData)
                    {
                        if (pair.Key.Contains(filter))
                        {
                            knownServices.Add(pair.Key);
                        }
                    }
                }
            }
            return knownServices;
        }

        /// <summary>
        /// Apre la porta per un servizio noto, reindirizzando i dati verso l'IP specificato con la porta privata, la porta pubblica, il tempo di validità e la descrizione fornita.
        /// </summary>
        /// <param name="serviceName">Nome del servizio.</param>
        /// <param name="address">Indirizzo IP del dispositivo interno.</param>
        /// <param name="privatePort">Porta privata, se è 0 sarà utilizzata lo stesso numero di porta della porta pubblica già noto.</param>
        /// <param name="publicPort">Porta pubblica, se è 0 sarà utilizzato il numero di porta già noto.</param>
        /// <param name="description">Descrizione della regola, se è una stringa vuota sarà utilizzato il nome del servizio.</param>
        /// <param name="lifetime">Tempo, in secondi, di validità della regole, se è 0 la regola sarà permanente.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void OpenKnownServicePort(string serviceName, IPAddress address, int privatePort, int publicPort, int lifetime, string description)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                throw new ArgumentNullException(nameof(serviceName), Properties.NatAutoMapper.ArgumentNullExceptionKnownServiceNameText);
            }
            else
            {
                if (!_knownPortsData.ContainsKey(serviceName))
                {
                    throw new ArgumentException(Properties.NatAutoMapper.ArgumentExceptionKnownServiceInexistentText, nameof(serviceName));
                }
            }
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address), Properties.NatAutoMapper.ArgumentNullExceptionKnownServiceNullAddressText);
            }
            else if (!UtilityMethods.IsAddressLocal(address))
            {
                throw new ArgumentException(Properties.NatAutoMapper.NotLocalIPError, nameof(address));
            }
            if (privatePort < 0 || privatePort > 65535 || publicPort < 0 || privatePort > 65535)
            {
                throw new ArgumentException(Properties.NatAutoMapper.ArgumentExceptionKnownServiceInvalidPort);
            }
            (Protocol Protocol, int PublicPort) knownServiceData = _knownPortsData[serviceName];
            if (string.IsNullOrWhiteSpace(description))
            {
                description = serviceName;
            }
            if (privatePort == 0 && publicPort != 0)
            {
                privatePort = knownServiceData.PublicPort;
            }
            else if (publicPort == 0 && privatePort != 0)
            {
                publicPort = knownServiceData.PublicPort;
            }
            else if (privatePort == 0 && publicPort == 0)
            {
                privatePort = publicPort = knownServiceData.PublicPort;
            }
            Mapping rule = new Mapping(knownServiceData.Protocol, address, privatePort, publicPort, lifetime, description);
            PortOperationsMethods.OpenPort(rule);
        }
    }
}