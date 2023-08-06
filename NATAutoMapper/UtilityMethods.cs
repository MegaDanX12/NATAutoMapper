using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace NATAutoMapper
{
    /// <summary>
    /// Metodi di utilità per le operazioni del programma.
    /// </summary>
    public static class UtilityMethods
    {
        /// <summary>
        /// Controlla se l'indirizzo fornito è un indirizzo IP locale.
        /// </summary>
        /// <param name="address">Indirizzo IP.</param>
        /// <returns>true se l'indirizzo è locale, false altrimenti.</returns>
        public static bool IsAddressLocal(IPAddress address)
        {
            /* Gli indirizzi IP privati sono definiti in tre fasce:
             * Da 10.0.0.0 a 10.255.255.255
             * Da 172.16.0.0 a 172.31.255.255
             * Da 192.168.0.0 a 192.168.255.255
             * Questo metodo ignora gli indirizzi locali di alcune reti VPN e gli indirizzi link-local (169.254/16).
             */
            if (address is null)
            {
                return false;
            }
            int[] ipParts = address.ToString().Split('.').Select(int.Parse).ToArray();
            if (ipParts[0] == 10 || (ipParts[0] == 192 && ipParts[1] == 168) || (ipParts[0] == 172 && (ipParts[1] >= 16 && ipParts[1] <= 31)))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Recupera l'indirizzo IP locale.
        /// </summary>
        /// <returns>L'indirizzo IP locale.</returns>
        /// <exception cref="NetworkInformationException"></exception>
        public static IPAddress GetLocalIP()
        {
            UnicastIPAddressInformation localIP = null;
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface netInterface in interfaces)
            {
                // Se l'interfaccia di rete non è in funzione allora non può avere un indirizzo IP.
                if (netInterface.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }
                IPInterfaceProperties properties = netInterface.GetIPProperties();
                // Se l'interfaccia non ha impostato nessun indirizzo di gateway significa che non è connessa a una rete.
                if (properties.GatewayAddresses.Count == 0)
                {
                    continue;
                }
                foreach (UnicastIPAddressInformation address in properties.UnicastAddresses)
                {
                    // Se l'indirizzo non è di tipo InterNetwork allora non è un indirizzo IPv4
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                    {
                        continue;
                    }
                    // L'indirizzo di Loopback (127.0.0.1) non è un indirizzo privato.
                    if (IPAddress.IsLoopback(address.Address))
                    {
                        continue;
                    }
                    // Se l'indirizzo non è utilizzabile in un DNS allora può essere un indirizzo privato.
                    if (!address.IsDnsEligible)
                    {
                        if (localIP is null)
                        {
                            localIP = address;
                        }
                        continue;
                    }
                    // Se l'indirizzo proviene da un server DHCP allora è un indirizzo privato.
                    if (address.PrefixOrigin is PrefixOrigin.Dhcp)
                    {
                        if (localIP == null || !localIP.IsDnsEligible)
                        {
                            localIP = address;
                        }
                        continue;
                    }
                }
            }
            return localIP.Address;
        }
    }
}