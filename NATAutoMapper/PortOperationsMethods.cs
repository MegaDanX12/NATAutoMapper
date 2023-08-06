using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Globalization;
using Open.Nat;

namespace NATAutoMapper
{
    /// <summary>
    /// Contiene metodi per aprire, chiudere le porte di un dispositivo NAT ed enumerare le regole UPnP già presenti.
    /// </summary>
    public static class PortOperationsMethods
    {
        /// <summary>
        /// Dispositivo NAT su cui effettuare le operazioni.
        /// </summary>
        private static NatDevice _selectedNatDevice;

        /// <summary>
        /// Imposta il dispositivo NAT su cui verranno effettuate le operazioni.
        /// </summary>
        /// <param name="Device">Dispositivo NAT selezionato.</param>
        public static void SetNatDevice(NatDevice Device)
        {
            _selectedNatDevice = Device;
        }

        /// <summary>
        /// Recupera le regole di port mapping attualmente impostate nel dispositivo NAT.
        /// </summary>
        /// <returns>Istanze di <see cref="Mapping"/> che rappresentano le regole impostate.</returns>
        public static Mapping[] RetrieveRules()
        {
            return _selectedNatDevice.GetAllMappingsAsync().Result.ToArray();
        }

        /// <summary>
        /// Mostra tutte le regole UPnP di port mapping attualmente impostate nel dispositivo NAT.
        /// </summary>
        public static void ShowAllPortMappings()
        {
            Mapping[] mappings = null;
            try
            {
                mappings = _selectedNatDevice.GetAllMappingsAsync().Result.ToArray();
            }
            catch (MappingException)
            {
                Console.WriteLine(Properties.NatAutoMapper.EnumerationError);
            }
            if (mappings.Length > 0)
            {
                Console.WriteLine();
                Console.WriteLine(Properties.NatAutoMapper.PortMappingsListing);
                for (int i = 0; i < mappings.Length; i++)
                {
                    // Ogni informazione disponibile sulla regola viene visualizzata sulla console, \t è una sequenza di escape che indica il carattere tabulatore.
                    Console.WriteLine((i + 1) + ")");
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingDescription);
                    Console.WriteLine(mappings[i].Description);
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingValidFor);
                    Console.WriteLine(mappings[i].Lifetime + "s");
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingAddress);
                    Console.WriteLine(mappings[i].PrivateIP.ToString());
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingInternalPort);
                    Console.WriteLine(mappings[i].PrivatePort);
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingExternalPort);
                    Console.WriteLine(mappings[i].PublicPort);
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingProtocol);
                    Console.WriteLine(Enum.GetName(typeof(Protocol), mappings[i].Protocol));
                    Console.WriteLine();
                }
                Console.WriteLine(Properties.NatAutoMapper.PortMappingsListingWarning);
            }
            else
            {
                Console.WriteLine(Properties.NatAutoMapper.NoPortMappingFound);
                Console.WriteLine();
                Console.WriteLine(Properties.NatAutoMapper.PortMappingsListingWarning);
            }
        }

        /// <summary>
        /// Mostra tutte le regole UPnP di port mapping attualmente impostate nel dispositivo NAT.
        /// </summary>
        /// <returns>0 se l'operazione è riuscita, -1 altrimenti.</returns>
        public static int ShowAllPortMappingsArgsMethod()
        {
            Mapping[] Mappings;
            try
            {
                Mappings = _selectedNatDevice.GetAllMappingsAsync().Result.ToArray();
            }
            catch (MappingException)
            {
                return -1;
            }
            if (Mappings.Length > 0)
            {
                Console.WriteLine(Properties.NatAutoMapper.PortMappingsListing);
                for (int i = 0; i < Mappings.Length; i++)
                {
                    // Ogni informazione disponibile sulla regola viene visualizzata sulla console, \t è una sequenza di escape che indica il carattere tabulatore.
                    Console.WriteLine((i + 1) + ")");
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingDescription);
                    Console.WriteLine(Mappings[i].Description);
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingValidFor);
                    Console.WriteLine(Mappings[i].Lifetime + "s");
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingAddress);
                    Console.WriteLine(Mappings[i].PrivateIP.ToString());
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingInternalPort);
                    Console.WriteLine(Mappings[i].PrivatePort);
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingExternalPort);
                    Console.WriteLine(Mappings[i].PublicPort);
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingProtocol);
                    Console.WriteLine(Enum.GetName(typeof(Protocol), Mappings[i].Protocol));
                    Console.WriteLine();
                }
                Console.WriteLine(Properties.NatAutoMapper.PortMappingsListingWarning);
            }
            else
            {
                Console.WriteLine(Properties.NatAutoMapper.NoPortMappingFound);
                Console.WriteLine();
                Console.WriteLine(Properties.NatAutoMapper.PortMappingsListingWarning);
            }
            return 0;
        }

        /// <summary>
        /// Apre una porta del dispositivo NAT.
        /// </summary>
        // Questo metodo prevede che l'utente fornisca direttamente i parametri.
        public static void OpenPort()
        {
            #region Parameter Parsing
            string protocol; // Protocollo utilizzato.
            IPAddress deviceIPAddress; // Indirizzo IP del dispositivo a cui si dovra inviare i dati.
            int internalPort; // Porta del dispositivo interno dove esso ricevera i dati da Internet.
            int externalPort; // Porta del lato WAN del router a cui saranno inviati dati dall'esterno.
            int validFor = -1; // Tempo di validità della regola, allo scadere di questo tempo la regola sarà cancellata.
            string description; // Descrizione che rappresenta la regola.
            // Tutti i dati vengono richiesti finchè l'utente non ha fornito dati validi.
            #region Protocol
            do
            {
                Console.Write(Properties.NatAutoMapper.OpenPortProtocolSelection);
                protocol = Console.ReadLine();
                if (protocol != "TCP" && protocol != "UDP" && protocol != "Both")
                {
                    Console.WriteLine(Properties.NatAutoMapper.OpenPortProtocolInvalid);
                    protocol = null;
                }
            }
            while (protocol == null);
            #endregion
            Console.WriteLine();
            #region IP Address
            do
            {
                Console.Write(Properties.NatAutoMapper.OpenPortIPAddress);
                try
                {
                    string address = Console.ReadLine();
                    if (address == "Local")
                    {
                        deviceIPAddress = UtilityMethods.GetLocalIP();
                    }
                    else
                    {
                        deviceIPAddress = IPAddress.Parse(address);
                        bool IsLocal = UtilityMethods.IsAddressLocal(deviceIPAddress);
                        if (!IsLocal)
                        {
                            Console.WriteLine(Properties.NatAutoMapper.NotLocalIPErrorOpenPort);
                            deviceIPAddress = null;
                        }
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine(Properties.NatAutoMapper.OpenPortFormatExceptionErrorText);
                    deviceIPAddress = null;
                }
                catch (NetworkInformationException e)
                {
                    Console.WriteLine(Properties.NatAutoMapper.NetworkInformationExceptionDetailedText1 + e.Message + Properties.NatAutoMapper.NetworkInformationExceptionDetailedText2 + e.NativeErrorCode);
                    return;
                }
            }
            while (deviceIPAddress == null);
            #endregion
            Console.WriteLine();
            #region Internal Port
            do
            {
                try
                {
                    Console.Write(Properties.NatAutoMapper.PortMappingInternalPort);
                    internalPort = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture);
                    if (internalPort <= 0 || internalPort > 65535)
                    {
                        Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidPort);
                        Console.WriteLine();
                        internalPort = -1;
                    }
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidPort);
                    Console.WriteLine();
                    internalPort = -1;
                }
            }
            while (internalPort == -1);
            #endregion
            Console.WriteLine();
            #region External Port
            do
            {
                try
                {
                    Console.Write(Properties.NatAutoMapper.PortMappingExternalPort);
                    externalPort = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture);
                    if (externalPort <= 0 || externalPort > 65535)
                    {
                        Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidPort);
                        Console.WriteLine();
                        externalPort = -1;
                    }
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidPort);
                    Console.WriteLine();
                    externalPort = -1;
                }
            }
            while (externalPort == -1);
            #endregion
            Console.WriteLine();
            #region Rule Lifetime
            do
            {
                try
                {
                    Console.Write(Properties.NatAutoMapper.OpenPortLifetime);
                    validFor = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture);
                    if (validFor < -1)
                    {
                        Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidLifetime);
                    }
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine(Properties.NatAutoMapper.OpenPortLifetimeNaN);
                    Console.WriteLine();
                }
            }
            while (validFor == -1);
            #endregion
            Console.WriteLine();
            #region Rule Description and Protocol Parsing
            Console.Write(Properties.NatAutoMapper.PortMappingDescription);
            description = Console.ReadLine();
            Console.WriteLine();
            // Trasforma la stringa della variabile Protocol nel valore enumerativo utilizzato dal metodo CreatePortMapAsync.
            Protocol protocolEnum = Protocol.Tcp;
            // Indica se creare una regola per entrambi i protocolli.
            bool bothProtocols = false;
            switch (protocol.ToLowerInvariant())
            {
                case "tcp":
                    protocolEnum = Protocol.Tcp;
                    break;
                case "udp":
                    protocolEnum = Protocol.Udp;
                    break;
                case "both":
                    protocolEnum = Protocol.Tcp;
                    bothProtocols = true;
                    break;
            }
            #endregion
            #endregion
            #region Rule Creation
            #region Already Defined Rule Lookup
            // Prima di tentare di inserire la nuova regola, il programma controlla se ne esiste già una uguale.
            Console.WriteLine(Properties.NatAutoMapper.OpenPortRuleLookup);
            IEnumerable<Mapping> currentMappings;
            try
            {
                currentMappings = _selectedNatDevice.GetAllMappingsAsync().Result;
            }
            catch (MappingException e)
            {
                Console.WriteLine(Properties.NatAutoMapper.EnumerationErrorDetailedText + e.ErrorCode + ": " + e.ErrorText);
                return;
            }
            Console.WriteLine();
            bool ruleAlreadyPresentTCP = false;
            bool ruleAlreadyPresentUDP = false;
            foreach (Mapping ruleEntry in currentMappings)
            {
                if (!bothProtocols)
                {
                    if (ruleEntry.PublicPort == externalPort && ruleEntry.Protocol == protocolEnum && ruleEntry.PrivatePort == internalPort && ruleEntry.PrivateIP == deviceIPAddress)
                    {
                        Console.WriteLine(Properties.NatAutoMapper.OpenPortRuleAlreadyExists);
                        return;
                    }
                }
                else
                {
                    // Se è stato specificato che si vuole creare una regola per entrambi i protocolli ed esiste già una regola per uno di essi, verrà creata soltanto quella inesistente.
                    
                    if (ruleEntry.PublicPort == externalPort && ruleEntry.Protocol == Protocol.Tcp && ruleEntry.PrivatePort == internalPort && ruleEntry.PrivateIP == deviceIPAddress)
                    {
                        ruleAlreadyPresentTCP = true;
                    }
                    else if (ruleEntry.PublicPort == externalPort && ruleEntry.Protocol == Protocol.Udp && ruleEntry.PrivatePort == internalPort && ruleEntry.PrivateIP == deviceIPAddress)
                    {
                        ruleAlreadyPresentUDP = true;
                    }
                }
                if (ruleAlreadyPresentTCP && !ruleAlreadyPresentUDP)
                {
                    Console.WriteLine(Properties.NatAutoMapper.OpenPortRuleAlreadyExistsTCP);
                    bothProtocols = false;
                    protocolEnum = Protocol.Udp;
                }
                else if (!ruleAlreadyPresentTCP && ruleAlreadyPresentUDP)
                {
                    Console.WriteLine(Properties.NatAutoMapper.OpenPortRuleAlreadyExistsUDP);
                    bothProtocols = false;
                    protocolEnum = Protocol.Tcp;
                }
                else if (ruleAlreadyPresentTCP && ruleAlreadyPresentUDP)
                {
                    Console.WriteLine(Properties.NatAutoMapper.OpenPortRuleAlreadyExistsBothProtocols);
                    return;
                }
            }
            #endregion
            #region Port Opening
            Console.WriteLine(Properties.NatAutoMapper.OpenPortOpening);
            Mapping entry;
            Mapping entry2; // Questa variabile contiene l'eventuale seconda regola quando si vuole creare una regola per entrambi i protocolli.
            try
            {
                if (!bothProtocols)
                {
                    entry = new Mapping(protocolEnum, deviceIPAddress, internalPort, externalPort, validFor, description);
                    _selectedNatDevice.CreatePortMapAsync(entry);
                }
                else
                {
                    entry = new Mapping(Protocol.Tcp, deviceIPAddress, internalPort, externalPort, validFor, description);
                    entry2 = new Mapping(Protocol.Udp, deviceIPAddress, internalPort, externalPort, validFor, description);
                    _selectedNatDevice.CreatePortMapAsync(entry);
                    _selectedNatDevice.CreatePortMapAsync(entry2);
                }
            }
            catch (MappingException e)
            {
                Console.WriteLine(Properties.NatAutoMapper.OpenPortError + e.ErrorCode + ": " + e.ErrorText);
                return;
            }
            Console.WriteLine(Properties.NatAutoMapper.OpenPortOpeningComplete1 + externalPort + Properties.NatAutoMapper.OpenPortOpeningComplete2);
            #endregion
            #endregion
        }

        /// <summary>
        /// Apre un raggio di porte sul dispositivo NAT.
        /// </summary>
        // Questo metodo prevede che l'utente fornisca direttamente i parametri.
        public static void OpenPortRange()
        {
            #region Parameter Parsing
            string protocol; // Protocollo utilizzato.
            IPAddress deviceIPAddress; // Indirizzo IP del dispositivo a cui si dovra inviare i dati.
            int internalStartingPort; // Porta del dispositivo interno dove esso ricevera i dati da Internet.
            int externalStartingPort; // Porta del lato WAN del router a cui saranno inviati dati dall'esterno.
            int internalEndingPort;
            int externalEndingPort;
            int validFor = -1; // Tempo di validità della regola, allo scadere di questo tempo la regola sarà cancellata.
            string description; // Descrizione che rappresenta la regola.
            // Tutti i dati vengono richiesti finchè l'utente non ha fornito dati validi.
            #region Protocol
            do
            {
                Console.Write(Properties.NatAutoMapper.OpenPortProtocolSelection);
                protocol = Console.ReadLine();
                if (protocol != "TCP" && protocol != "UDP" && protocol != "Both")
                {
                    Console.WriteLine(Properties.NatAutoMapper.OpenPortProtocolInvalid);
                    protocol = null;
                }
            }
            while (protocol == null);
            #endregion
            Console.WriteLine();
            #region IP Address
            do
            {
                Console.Write(Properties.NatAutoMapper.OpenPortIPAddress);
                try
                {
                    string address = Console.ReadLine();
                    if (address == "Local")
                    {
                        deviceIPAddress = UtilityMethods.GetLocalIP();
                    }
                    else
                    {
                        deviceIPAddress = IPAddress.Parse(address);
                        bool IsLocal = UtilityMethods.IsAddressLocal(deviceIPAddress);
                        if (!IsLocal)
                        {
                            Console.WriteLine(Properties.NatAutoMapper.NotLocalIPErrorOpenPort);
                            deviceIPAddress = null;
                        }
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine(Properties.NatAutoMapper.OpenPortFormatExceptionErrorText);
                    deviceIPAddress = null;
                }
                catch (NetworkInformationException e)
                {
                    Console.WriteLine(Properties.NatAutoMapper.NetworkInformationExceptionDetailedText1 + e.Message + Properties.NatAutoMapper.NetworkInformationExceptionDetailedText2 + e.NativeErrorCode);
                    return;
                }
            }
            while (deviceIPAddress == null);
            #endregion
            Console.WriteLine();
            #region Internal Starting Port
            do
            {
                try
                {
                    Console.Write(Properties.NatAutoMapper.OpenPortRangeInternalStartingPort);
                    internalStartingPort = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture);
                    if (internalStartingPort <= 0 || internalStartingPort > 65535)
                    {
                        Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidPort);
                        Console.WriteLine();
                        internalStartingPort = -1;
                    }
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidPort);
                    Console.WriteLine();
                    internalStartingPort = -1;
                }
            }
            while (internalStartingPort == -1);
            #endregion
            Console.WriteLine();
            #region Internal Ending Port
            do
            {
                try
                {
                    Console.Write(Properties.NatAutoMapper.OpenPortRangeInternalEndingPort);
                    internalEndingPort = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture);
                    if (internalEndingPort <= 0 || internalEndingPort > 65535)
                    {
                        Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidPort);
                        Console.WriteLine();
                        internalEndingPort = -1;
                    }
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidPort);
                    Console.WriteLine();
                    internalEndingPort = -1;
                } 
            }
            while (internalEndingPort == -1);
            #endregion
            Console.WriteLine();
            #region External Starting Port
            do
            {
                try
                {
                    Console.Write(Properties.NatAutoMapper.OpenPortRangeExternalStartingPort);
                    externalStartingPort = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture);
                    if (externalStartingPort <= 0 || externalStartingPort > 65535)
                    {
                        Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidPort);
                        Console.WriteLine();
                        externalStartingPort = -1;
                    }
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidPort);
                    Console.WriteLine();
                    externalStartingPort = -1;
                }
            }
            while (externalStartingPort == -1);
            #endregion
            Console.WriteLine();
            #region External Ending Port
            do
            {
                try
                {
                    Console.Write(Properties.NatAutoMapper.OpenPortRangeInternalEndingPort);
                    externalEndingPort = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture);
                    if (externalEndingPort <= 0 || externalEndingPort > 65535)
                    {
                        Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidPort);
                        Console.WriteLine();
                        externalEndingPort = -1;
                    }
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidPort);
                    Console.WriteLine();
                    externalEndingPort = -1;
                }
            }
            while (externalEndingPort == -1);
            #endregion
            Console.WriteLine();
            #region Rule Lifetime
            do
            {
                try
                {
                    Console.Write(Properties.NatAutoMapper.OpenPortLifetime);
                    validFor = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture);
                    if (validFor < -1)
                    {
                        Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidLifetime);
                    }
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine(Properties.NatAutoMapper.OpenPortLifetimeNaN);
                    Console.WriteLine();
                }
            }
            while (validFor == -1);
            #endregion
            Console.WriteLine();
            #region Rule Description and Protocol Parsing
            Console.Write(Properties.NatAutoMapper.PortMappingDescription);
            description = Console.ReadLine();
            Console.WriteLine();
            // Trasforma la stringa della variabile Protocol nel valore enumerativo utilizzato dal metodo CreatePortMapAsync.
            Protocol protocolEnum = Protocol.Tcp;
            // Indica se creare una regola per entrambi i protocolli.
            bool bothProtocols = false;
            switch (protocol.ToLowerInvariant())
            {
                case "tcp":
                    protocolEnum = Protocol.Tcp;
                    break;
                case "udp":
                    protocolEnum = Protocol.Udp;
                    break;
                case "both":
                    protocolEnum = Protocol.Tcp;
                    bothProtocols = true;
                    break;
            }
            #endregion
            #endregion
            #region Rule Creation
            #region Already Defined Rule Lookup
            // Prima di tentare di inserire la nuova regola, il programma controlla se ne esiste già una uguale.
            Console.WriteLine(Properties.NatAutoMapper.OpenPortRuleLookup);
            IEnumerable<Mapping> currentMappings;
            try
            {
                currentMappings = _selectedNatDevice.GetAllMappingsAsync().Result;
            }
            catch (MappingException e)
            {
                Console.WriteLine(Properties.NatAutoMapper.EnumerationErrorDetailedText + e.ErrorCode + ": " + e.ErrorText);
                return;
            }
            Console.WriteLine();
            bool ruleAlreadyPresentTCP = false;
            bool ruleAlreadyPresentUDP = false;
            foreach (Mapping ruleEntry in currentMappings)
            {
                if (!bothProtocols)
                {
                    for (int i = internalStartingPort; i == internalEndingPort; i++)
                    {
                        if (ruleEntry.PrivatePort == i)
                        {
                            if (ruleEntry.Protocol == protocolEnum && ruleEntry.PrivateIP == deviceIPAddress)
                            {
                                Console.WriteLine(Properties.NatAutoMapper.OpenPortRangeInternalAlreadyOpen);
                                return;
                            }
                        }
                    }
                    for (int i = externalStartingPort; i == externalEndingPort; i++)
                    {
                        if (ruleEntry.PublicPort == i)
                        {
                            if (ruleEntry.Protocol == protocolEnum && ruleEntry.PrivateIP == deviceIPAddress)
                            {
                                Console.WriteLine(Properties.NatAutoMapper.OpenPortRangeExternalAlreadyOpen);
                                return;
                            }
                        }
                    }
                }
                else
                {
                    // Se è stato specificato che si vuole creare una regola per entrambi i protocolli ed esiste già una regola per uno di essi, verrà creata soltanto quella inesistente.
                    for (int i = internalStartingPort; i == internalEndingPort; i++)
                    {
                        if (ruleEntry.PrivatePort == i)
                        {
                            if (ruleEntry.Protocol == Protocol.Tcp && ruleEntry.PrivateIP == deviceIPAddress)
                            {
                                ruleAlreadyPresentTCP = true;
                                return;
                            }
                        }
                    }
                    for (int i = externalStartingPort; i == externalEndingPort; i++)
                    {
                        if (ruleEntry.PublicPort == i)
                        {
                            if (ruleEntry.Protocol == Protocol.Tcp && ruleEntry.PrivateIP == deviceIPAddress)
                            {
                                ruleAlreadyPresentTCP = true;
                                return;
                            }
                        }
                    }
                    for (int i = internalStartingPort; i == internalEndingPort; i++)
                    {
                        if (ruleEntry.PrivatePort == i)
                        {
                            if (ruleEntry.Protocol == Protocol.Udp && ruleEntry.PrivateIP == deviceIPAddress)
                            {
                                ruleAlreadyPresentUDP = true;
                                return;
                            }
                        }
                    }
                    for (int i = externalStartingPort; i == externalEndingPort; i++)
                    {
                        if (ruleEntry.PublicPort == i)
                        {
                            if (ruleEntry.Protocol == Protocol.Udp && ruleEntry.PrivateIP == deviceIPAddress)
                            {
                                ruleAlreadyPresentUDP = true;
                                return;
                            }
                        }
                    }
                }
                if (ruleAlreadyPresentTCP && !ruleAlreadyPresentUDP)
                {
                    Console.WriteLine(Properties.NatAutoMapper.OpenPortRangeAlreadyExistsTCP);
                    bothProtocols = false;
                    protocolEnum = Protocol.Udp;
                }
                else if (!ruleAlreadyPresentTCP && ruleAlreadyPresentUDP)
                {
                    Console.WriteLine(Properties.NatAutoMapper.OpenPortRangeAlreadyExistsUDP);
                    bothProtocols = false;
                    protocolEnum = Protocol.Tcp;
                }
                else if (ruleAlreadyPresentTCP && ruleAlreadyPresentUDP)
                {
                    Console.WriteLine(Properties.NatAutoMapper.OpenPortRangeAlreadyExistsBothProtocols);
                    return;
                }
            }
            #endregion
            #region Port Opening
            Console.WriteLine(Properties.NatAutoMapper.OpenPortRangeOpening);
            Mapping entry;
            Mapping entry2; // Questa variabile contiene l'eventuale seconda regola quando si vuole creare una regola per entrambi i protocolli.
            try
            {
                if (!bothProtocols)
                {
                    for (int i = internalStartingPort; i <= internalEndingPort; i++)
                    {
                        entry = new Mapping(protocolEnum, deviceIPAddress, i, externalStartingPort + (i - internalStartingPort), validFor, description);
                        _selectedNatDevice.CreatePortMapAsync(entry);
                    }
                }
                else
                {
                    for (int i = internalStartingPort; i <= internalEndingPort; i++)
                    {
                        entry = new Mapping(Protocol.Tcp, deviceIPAddress, i, externalStartingPort + (i - internalStartingPort), validFor, description);
                        entry2 = new Mapping(Protocol.Udp, deviceIPAddress, i, externalStartingPort + (i - internalStartingPort), validFor, description);
                        _selectedNatDevice.CreatePortMapAsync(entry);
                        _selectedNatDevice.CreatePortMapAsync(entry2);
                    }
                }
            }
            catch (MappingException e)
            {
                Console.WriteLine(Properties.NatAutoMapper.OpenPortError + e.ErrorCode + ": " + e.ErrorText);
                return;
            }
            Console.WriteLine(Properties.NatAutoMapper.OpenPortRangeOpened1 + externalStartingPort + Properties.NatAutoMapper.OpenPortRangeOpened2 + externalEndingPort + Properties.NatAutoMapper.OpenPortRangeOpened3);
            #endregion
            #endregion
        }

        // Questo metodo è utilizzato quando i parametri sono stati forniti da linea di comando, è uguale a quello precendente escluso tutto l'output sulla console.
        /// <summary>
        /// Apre una porta del dispositivo NAT con il protocollo, la porta privata, la porta pubblica, l'indirizzo IP, la descrizione e la durata (in secondi) fornita.
        /// </summary>
        /// <param name="ProtocolString">Protocollo.</param>
        /// <param name="InternalPortString">Numero della porta del dispositivo interno.</param>
        /// <param name="ExternalPortString">Numero della porta del lato WAN del router.</param>
        /// <param name="IPAddressString">Indirizzo IP del dispositivo interno.</param>
        /// <param name="DescriptionString">Descrizione della regola.</param>
        /// <param name="ValidForString">Tempo di validità della regola.</param>
        /// <returns>0 se l'operazione è riuscita, -1 altrimenti.</returns>
        public static int OpenPort(string ProtocolString, string InternalPortString, string ExternalPortString, string IPAddressString, string DescriptionString, string ValidForString)
        {
            #region Parameter Parsing
            string protocol = ProtocolString;
            IPAddress deviceIPAddress;
            int internalPort;
            int externalPort;
            int validFor;
            string description = DescriptionString;
            #region Protocol
            if (protocol != "TCP" && protocol != "UDP" && protocol != "Both")
            {
                return -1;
            }
            #endregion
            #region IP Address
            try
            {
                string address = IPAddressString;
                if (address == "Local")
                {
                    deviceIPAddress = UtilityMethods.GetLocalIP();
                }
                else
                {
                    deviceIPAddress = IPAddress.Parse(address);
                    bool IsLocal = UtilityMethods.IsAddressLocal(deviceIPAddress);
                    if (!IsLocal)
                    {
                        return -2;
                    }
                }
            }
            catch (FormatException)
            {
                return -2;
            }
            catch (NetworkInformationException)
            {
                return -2;
            }
            #endregion
            #region Internal Port
            try
            {
                internalPort = Convert.ToInt32(InternalPortString, CultureInfo.InvariantCulture);
                if (internalPort <= 0 || internalPort > 65535)
                {
                    return -3;
                }
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
            {
                return -3;
            }
            #endregion
            #region External Port
            try
            {
                externalPort = Convert.ToInt32(ExternalPortString, CultureInfo.InvariantCulture);
                if (externalPort <= 0 || externalPort > 65535)
                {
                    return -4;
                }
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
            {
                return -4;
            }
            #endregion
            #region Rule Lifetime
            try
            {
                validFor = Convert.ToInt32(ValidForString, CultureInfo.InvariantCulture);
                if (validFor < -1)
                {
                    return -5;
                }
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
            {
                return -5;
            }
            #endregion
            #region Protocol Parsing
            Protocol protocolEnum = Protocol.Tcp;
            bool bothProtocols = false;
            switch (protocol.ToLowerInvariant())
            {
                case "tcp":
                    protocolEnum = Protocol.Tcp;
                    break;
                case "udp":
                    protocolEnum = Protocol.Udp;
                    break;
                case "both":
                    protocolEnum = Protocol.Tcp;
                    bothProtocols = true;
                    break;
            }
            #endregion
            #endregion
            #region Rule Creation
            #region Already Defined Rule Lookup
            IEnumerable<Mapping> currentMappings;
            try
            {
                currentMappings = _selectedNatDevice.GetAllMappingsAsync().Result;
            }
            catch (MappingException)
            {
                return 1;
            }
            bool ruleAlreadyPresentTCP = false;
            bool ruleAlreadyPresentUDP = false;
            foreach (Mapping ruleEntry in currentMappings)
            {
                if (!bothProtocols)
                {
                    if (ruleEntry.PublicPort == externalPort && ruleEntry.Protocol == protocolEnum && ruleEntry.PrivatePort == internalPort && ruleEntry.PrivateIP == deviceIPAddress)
                    {
                        return 2;
                    }
                }
                else
                {
                    if (ruleEntry.PublicPort == externalPort && ruleEntry.Protocol == Protocol.Tcp && ruleEntry.PrivatePort == internalPort && ruleEntry.PrivateIP == deviceIPAddress)
                    {
                        ruleAlreadyPresentTCP = true;
                    }
                    else if (ruleEntry.PublicPort == externalPort && ruleEntry.Protocol == Protocol.Udp && ruleEntry.PrivatePort == internalPort && ruleEntry.PrivateIP == deviceIPAddress)
                    {
                        ruleAlreadyPresentUDP = true;
                    }
                }
            }
            if (ruleAlreadyPresentTCP && !ruleAlreadyPresentUDP)
            {
                bothProtocols = false;
                protocolEnum = Protocol.Udp;
            }
            else if (!ruleAlreadyPresentTCP && ruleAlreadyPresentUDP)
            {
                bothProtocols = false;
                protocolEnum = Protocol.Tcp;
            }
            else if (ruleAlreadyPresentTCP && ruleAlreadyPresentUDP)
            {
                return 2;
            }
            #endregion
            #region Port Opening
            Mapping entry;
            Mapping entry2;
            try
            {
                if (!bothProtocols)
                {
                    entry = new Mapping(protocolEnum, deviceIPAddress, internalPort, externalPort, validFor, description);
                    _selectedNatDevice.CreatePortMapAsync(entry);
                }
                else
                {
                    entry = new Mapping(Protocol.Tcp, deviceIPAddress, internalPort, externalPort, validFor, description);
                    entry2 = new Mapping(Protocol.Udp, deviceIPAddress, internalPort, externalPort, validFor, description);
                    _selectedNatDevice.CreatePortMapAsync(entry);
                    _selectedNatDevice.CreatePortMapAsync(entry2);
                }
            }
            catch (MappingException)
            {
                return 3;
            }
            #endregion
            #endregion
            return 0;
        }

        // Questo metodo utilizza una regola già definita.
        /// <summary>
        /// Apre una porta sul dispositivo NAT in base all'istanza della classe <see cref="Mapping"/> fornita.
        /// </summary>
        /// <param name="Entry">Regola da inserire.</param>
        public static void OpenPort(Mapping Entry)
        {
            #region Already Defined Rule Lookup
            IEnumerable<Mapping> currentMappings;
            try
            {
                currentMappings = _selectedNatDevice.GetAllMappingsAsync().Result;
            }
            catch (MappingException)
            {
                return;
            }
            foreach (Mapping entry in currentMappings)
            {
                if (entry == Entry)
                {
                    return;
                }
            }
            #endregion
            #region Port Opening
            try
            {
                _selectedNatDevice.CreatePortMapAsync(Entry);
            }
            catch (MappingException)
            {
                return;
            }
            #endregion
        }

        /// <summary>
        /// Apre una porta nota.
        /// </summary>
        public static void OpenKnownPort()
        {
            IPAddress deviceAddress;
            int privatePort;
            int publicPort;
            int lifetime;
            string description;
            Console.Write(Properties.NatAutoMapper.OpenKnownPortRuleName);
            string serviceName = Console.ReadLine();
            #region Known Rules Listing
            if (serviceName == "LIST")
            {
                serviceName = null;
                Console.WriteLine();
                Console.WriteLine();
                string filter;
                bool retry;
                do
                {
                    Console.WriteLine(Properties.NatAutoMapper.OpenKnownPortListingFilter);
                    filter = Console.ReadLine();
                    retry = false;
                    if (string.IsNullOrWhiteSpace(filter))
                    {
                        #region No Filter
                        List<string> knownPortMappings = KnownPorts.ListKnownPortMappings();
                        Console.WriteLine();
                        Console.WriteLine(Properties.NatAutoMapper.OpenKnownPortListingNoFilter);
                        Console.WriteLine();
                        for (int i = 0; i < knownPortMappings.Count; i++)
                        {
                            Console.WriteLine(i + 1 + ") " + knownPortMappings[i]);
                            if (i % 10 == 0 && i > 0)
                            {
                                ConsoleKeyInfo keyPressed = Console.ReadKey(true);
                                if (keyPressed.Key == ConsoleKey.Enter)
                                {
                                    continue;
                                }
                                else if (keyPressed.Key == ConsoleKey.Escape)
                                {
                                    break;
                                }
                                else if (keyPressed.Key == ConsoleKey.R)
                                {
                                    retry = true;
                                    break;
                                }
                            }
                        }
                        if (retry)
                        {
                            continue;
                        }
                        Console.WriteLine();
                        int selectedRuleIndex = -1;
                        do
                        {
                            Console.Write(Properties.NatAutoMapper.UserSelection);
                            string selection = Console.ReadLine();
                            if (selection != "exit")
                            {
                                try
                                {
                                    selectedRuleIndex = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture) - 1;
                                }
                                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                                {
                                    Console.WriteLine(Properties.NatAutoMapper.InvalidOperationSelection);
                                    Console.WriteLine();
                                    selectedRuleIndex = -1;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        #endregion
                        while (selectedRuleIndex < 0);
                        if (selectedRuleIndex != -1)
                        {
                            serviceName = knownPortMappings[selectedRuleIndex];
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        #region Filtered
                        List<string> knownPortMappings = KnownPorts.ListKnownPortMappings(filter);
                        Console.WriteLine();
                        Console.WriteLine(Properties.NatAutoMapper.OpenKnownPortListingFiltered);
                        Console.WriteLine();
                        if (knownPortMappings.Count == 0)
                        {
                            Console.WriteLine(Properties.NatAutoMapper.OpenKnownPortListingFilteredNoRule);
                            break;
                        }
                        for (int i = 0; i < knownPortMappings.Count; i++)
                        {
                            Console.WriteLine(i + 1 + ") " + knownPortMappings[i]);
                            if (i % 10 == 0)
                            {
                                ConsoleKeyInfo keyPressed = Console.ReadKey(true);
                                if (keyPressed.Key == ConsoleKey.Enter)
                                {
                                    continue;
                                }
                                else if (keyPressed.Key == ConsoleKey.Escape)
                                {
                                    break;
                                }
                                else if (keyPressed.Key == ConsoleKey.R)
                                {
                                    retry = true;
                                    break;
                                }
                            }
                        }
                        if (retry)
                        {
                            continue;
                        }
                        Console.WriteLine();
                        int selectedRuleIndex = -1;
                        do
                        {
                            Console.Write(Properties.NatAutoMapper.UserSelection);
                            string selection = Console.ReadLine();
                            if (selection != "exit")
                            {
                                try
                                {
                                    selectedRuleIndex = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture) - 1;
                                }
                                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                                {
                                    Console.WriteLine(Properties.NatAutoMapper.InvalidOperationSelection);
                                    Console.WriteLine();
                                    selectedRuleIndex = -1;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        #endregion
                        while (selectedRuleIndex < 0);
                        if (selectedRuleIndex != -1)
                        {
                            serviceName = knownPortMappings[selectedRuleIndex];
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                while (retry);
            }
            #endregion
            if (serviceName != null)
            {
                Console.WriteLine();
                #region Required Parameters
                do
                {
                    Console.Write(Properties.NatAutoMapper.OpenPortIPAddress);
                    string address = Console.ReadLine();
                    Console.WriteLine();
                    if (address == "Local")
                    {
                        try
                        {
                            deviceAddress = UtilityMethods.GetLocalIP();
                        }
                        catch (NetworkInformationException)
                        {
                            Console.WriteLine(Properties.NatAutoMapper.NetworkInformationExceptionText);
                            deviceAddress = null;
                        }
                    }
                    else
                    {
                        try
                        {
                            deviceAddress = IPAddress.Parse(address);
                        }
                        catch (Exception ex) when (ex is ArgumentNullException || ex is FormatException)
                        {
                            Console.WriteLine(Properties.NatAutoMapper.OpenPortFormatExceptionErrorText);
                            deviceAddress = null;
                        }
                        if (!UtilityMethods.IsAddressLocal(deviceAddress))
                        {
                            Console.WriteLine(Properties.NatAutoMapper.NotLocalIPErrorOpenPort);
                            deviceAddress = null;
                        }
                    }
                }
                while (deviceAddress == null);
                Console.WriteLine();
                Console.WriteLine();
                Console.Write(Properties.NatAutoMapper.OpenKnownPortRuleDescription);
                description = Console.ReadLine();
                #endregion
                #region Optional Parameters
                ConsoleKeyInfo responseKey;
                do
                {
                    Console.Write(Properties.NatAutoMapper.OpenKnownPortUseStandardInternalPort);
                    responseKey = Console.ReadKey(false);
                    switch (responseKey.Key)
                    {
                        case ConsoleKey.Y:
                            privatePort = 0;
                            break;
                        case ConsoleKey.N:
                            Console.WriteLine();
                            Console.WriteLine();
                            do
                            {
                                Console.Write(Properties.NatAutoMapper.OpenKnownPortNumber);
                                try
                                {
                                    privatePort = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture);
                                    if (privatePort < 1 || privatePort > 65535)
                                    {
                                        Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidPort);
                                        privatePort = -1;
                                    }
                                }
                                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                                {
                                    Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidPort);
                                    privatePort = -1;
                                }
                            }
                            while (privatePort == -1);
                            break;
                        default:
                            Console.WriteLine(Properties.NatAutoMapper.OpenKnownPortInvalidResponse);
                            privatePort = -1;
                            break;
                    }
                }
                while (privatePort == -1);
                Console.WriteLine();
                Console.WriteLine();
                do
                {
                    Console.Write(Properties.NatAutoMapper.OpenKnownPortUseStandardExternalPort);
                    responseKey = Console.ReadKey(false);
                    switch (responseKey.Key)
                    {
                        case ConsoleKey.Y:
                            publicPort = 0;
                            break;
                        case ConsoleKey.N:
                            Console.WriteLine();
                            Console.WriteLine();
                            do
                            {
                                Console.Write(Properties.NatAutoMapper.OpenKnownPortNumber);
                                try
                                {
                                    publicPort = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture);
                                    if (publicPort < 1 || publicPort > 65535)
                                    {
                                        Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidPort);
                                        publicPort = -1;
                                    }
                                }
                                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                                {
                                    Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidPort);
                                    publicPort = -1;
                                }
                            }
                            while (publicPort == -1);
                            break;
                        default:
                            Console.WriteLine(Properties.NatAutoMapper.OpenKnownPortInvalidResponse);
                            publicPort = -1;
                            break;
                    }
                }
                while (publicPort == -1);
                Console.WriteLine();
                Console.WriteLine();
                do
                {
                    Console.Write(Properties.NatAutoMapper.OpenKnownPortRuleLifetime);
                    string lifetimeString = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(lifetimeString))
                    {
                        lifetime = 0;
                    }
                    else
                    {
                        try
                        {
                            lifetime = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture);
                            if (lifetime < 0)
                            {
                                Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidLifetime);
                            }
                        }
                        catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                        {
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine(Properties.NatAutoMapper.OpenPortInvalidLifetime);
                            lifetime = -1;
                        }
                    }
                }
                while (lifetime == -1);
                #endregion
                KnownPorts.OpenKnownServicePort(serviceName, deviceAddress, privatePort, publicPort, lifetime, description);
            }
            else
            {
                Console.WriteLine(Properties.NatAutoMapper.OpenKnownPortNoRuleSelected);
            }
        }

        // Questo metodo prevede che l'utente fornisca direttamente la descrizione della regola da eliminare.
        /// <summary>
        /// Chiude una porta del dispositivo NAT.
        /// </summary>
        public static void ClosePort()
        {
            // Indica se è stata trovata almeno una regola con la descrizione fornita.
            bool ruleFound = false;
            // Indica se la regola è stata cancellata.
            bool ruleDeleted = false;
            // Indica se l'utente ha selezionato un'opzione, utilizzato per interrompere l'esecuzione del metodo in caso di imput non valido.
            bool optionSelected = false;
            // Vengono recuperate tutte le regole impostate prima di chiedere la descrizione della regola da eliminare.
            #region Rules Retrieval
            Console.WriteLine(Properties.NatAutoMapper.RuleSetRecovery);
            IEnumerable<Mapping> currentMappings;
            try
            {
                currentMappings = _selectedNatDevice.GetAllMappingsAsync().Result;
            }
            catch (MappingException e)
            {
                Console.WriteLine(Properties.NatAutoMapper.EnumerationErrorDetailedText + e.ErrorCode + ": " + e.ErrorText);
                return;
            }
            #endregion
            Console.WriteLine();
            #region Rule Deletion Confirmation
            Console.Write(Properties.NatAutoMapper.ClosePortRuleDescription);
            string description = Console.ReadLine();
            foreach (Mapping rule in currentMappings)
            {
                // Se viene trovata una regola con la descrizione corrispondente a quella indicata, viene richiesta conferma prima di eliminarla.
                if (rule.Description == description)
                {
                    ruleFound = true;
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingDescription);
                    Console.WriteLine(rule.Description);
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingValidFor);
                    Console.WriteLine(rule.Lifetime + "s");
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingAddress);
                    Console.WriteLine(rule.PrivateIP.ToString());
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingInternalPort);
                    Console.WriteLine(rule.PrivatePort);
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingExternalPort);
                    Console.WriteLine(rule.PublicPort);
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingProtocol);
                    Console.WriteLine(Enum.GetName(typeof(Protocol), rule.Protocol));
                    Console.WriteLine();
                    Console.Write(Properties.NatAutoMapper.ClosePortConfirmation);
                    switch (Console.ReadKey().KeyChar.ToString(CultureInfo.InvariantCulture).ToLowerInvariant())
                    {
                        case "s":
                            optionSelected = true;
                            try
                            {
                                _selectedNatDevice.DeletePortMapAsync(rule);
                                ruleDeleted = true;
                            }
                            catch (MappingException e)
                            {
                                Console.WriteLine(Properties.NatAutoMapper.ClosePortError + e.ErrorCode + ": " + e.ErrorText);
                                return;
                            }
                            break;
                        case "n":
                            optionSelected = true;
                            continue;
                        default:
                            continue;
                    }
                }
            }
            #endregion
            Console.WriteLine();
            #region Selection Result
            if (optionSelected)
            {
                if (ruleFound && !ruleDeleted)
                {
                    Console.WriteLine(Properties.NatAutoMapper.ClosePortNoOtherRuleFound);
                }
                else if (ruleFound && ruleDeleted)
                {
                    Console.WriteLine(Properties.NatAutoMapper.ClosePortRuleDeleted);
                }
                else if (!ruleFound)
                {
                    Console.WriteLine(Properties.NatAutoMapper.ClosePortNoRuleFound);
                }
            }
            else
            {
                Console.WriteLine(Properties.NatAutoMapper.ClosePortNoOptionSelected);
            }
            #endregion
        }

        /// <summary>
        /// Chiude una porta sul dispositivo NAT.
        /// </summary>
        /// <param name="Description">Descrizione della regola da eliminare.</param>
        /// <returns>0 se l'operazione è riuscita, -1 altrimenti.</returns>
        public static int ClosePort(string Description)
        {
            bool ruleFound = false;
            bool ruleDeleted = false;
            bool optionSelected = false;
            #region Rules Retrieval
            IEnumerable<Mapping> currentMappings;
            try
            {
                currentMappings = _selectedNatDevice.GetAllMappingsAsync().Result;
            }
            catch (MappingException)
            {
                return 1;
            }
            #endregion
            #region Rule Deletion Confirmation
            foreach (Mapping rule in currentMappings)
            {
                if (rule.Description == Description)
                {
                    ruleFound = true;
                    Console.WriteLine();
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingDescription);
                    Console.WriteLine(rule.Description);
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingValidFor);
                    Console.WriteLine(rule.Lifetime + "s");
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingAddress);
                    Console.WriteLine(rule.PrivateIP.ToString());
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingInternalPort);
                    Console.WriteLine(rule.PrivatePort);
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingExternalPort);
                    Console.WriteLine(rule.PublicPort);
                    Console.Write("\t" + Properties.NatAutoMapper.PortMappingProtocol);
                    Console.WriteLine(Enum.GetName(typeof(Protocol), rule.Protocol));
                    Console.WriteLine();
                    Console.Write(Properties.NatAutoMapper.ClosePortConfirmation);
                    switch (Console.ReadKey().KeyChar.ToString(CultureInfo.InvariantCulture).ToLowerInvariant())
                    {
                        case "s":
                            optionSelected = true;
                            try
                            {
                                _selectedNatDevice.DeletePortMapAsync(rule);
                                ruleDeleted = true;
                            }
                            catch (MappingException)
                            {
                                return 2;
                            }
                            break;
                        case "n":
                            optionSelected = true;
                            continue;
                        default:
                            continue;
                    }
                }
            }
            #endregion
            #region Selection Result
            if (optionSelected)
            {
                if (ruleFound && !ruleDeleted)
                {
                    return 3;
                }
                else if (!ruleFound)
                {
                    return 4;
                }
            }
            else
            {
                return -1;
            }
            #endregion
            return 0;
        }
    }
}