using System;

namespace Classe_RegexIPandMAC
{

    public class RegexIPandMAC
    {
        private static bool Ethernet_MAC = false;
        private static bool Wireless_MAC = false;

        public RegexIPandMAC()
        {
            Console.WriteLine("");
            Console.WriteLine($"Endereço IPv4 : {ObterEndereçoIPv4()}");
            Console.WriteLine("");
            Console.WriteLine($"Endereço IPv4 : {ObterEndereçoMAC()}");
            Console.WriteLine("");
        }

        static string ObterEndereçoIPv4()
        {
            string ipConfigResult = ObterEndereçosIPs();

            // Busca o IPv4 para o Adaptador Ethernet Ethernet:
            string patternEthernet = @"Adaptador Ethernet Ethernet:(.*?[\r\n]){7}([\s\S]*?)(?=\r?\n\r?\n|$)";
            Match matchEthernet = Regex.Match(ipConfigResult, patternEthernet, RegexOptions.Singleline);
            if (matchEthernet.Success)
            {
                string ipv4Section = matchEthernet.Groups[2].Value.Trim();
                string[] lines = ipv4Section.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    if (line.StartsWith("Endere‡o IPv4"))
                    {
                        string[] ipv4Address = line.Split(':');
                        if (ipv4Address.Length == 2)
                        {
                            Ethernet_MAC = true; // Rede encontrada
                            return ipv4Address[1].Trim();
                        }
                        break;
                    }
                }
            }

            // Se não encontrou para o Adaptador Ethernet Ethernet:, busca para o Adaptador de Rede sem Fio Wi-Fi:
            string patternWiFi = @"Adaptador de Rede sem Fio Wi-Fi:(.*?[\r\n]){7}([\s\S]*?)(?=\r?\n\r?\n|$)";
            Match matchWiFi = Regex.Match(ipConfigResult, patternWiFi, RegexOptions.Singleline);
            if (matchWiFi.Success)
            {
                string ipv4Section = matchWiFi.Groups[2].Value.Trim();
                string[] lines = ipv4Section.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    if (line.StartsWith("Endere‡o IPv4"))
                    {
                        string[] ipv4Address = line.Split(':');
                        if (ipv4Address.Length == 2)
                        {
                            Wireless_MAC = true; // Rede encontrada
                            return ipv4Address[1].Trim();
                        }
                        break;
                    }
                }
            }
            return "Sem conexão com a internet";
        }

        static string ObterEndereçosIPs()
        {
            // Executar o comando ipconfig
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "ipconfig",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            process.StartInfo = startInfo;
            process.Start();

            // Ler o resultado completo
            string result = process.StandardOutput.ReadToEnd();

            // Aguardar a finalização do processo
            process.WaitForExit();

            return result;
        }



        static string ObterEndereçoMAC()
        {
            string macConfigResult = ObterEndereçosMACs();

            ObterEndereçoIPv4();

            if (Ethernet_MAC)
            {
                string patternEthernet = @"Ethernet\s+.*?(\b([A-F0-9]{2}-[A-F0-9]{2}-[A-F0-9]{2}-[A-F0-9]{2}-[A-F0-9]{2}-[A-F0-9]{2})\b)";
                Match matchEthernet = Regex.Match(macConfigResult, patternEthernet, RegexOptions.IgnoreCase);
                if (matchEthernet.Success)
                {
                    string macAddress = matchEthernet.Groups[2].Value;
                    return macAddress;
                }

            }

            if (Wireless_MAC)
            {
                string patternWiFi = @"Wi-Fi\s+.*?(\b([A-F0-9]{2}-[A-F0-9]{2}-[A-F0-9]{2}-[A-F0-9]{2}-[A-F0-9]{2}-[A-F0-9]{2})\b)";
                Match matchWiFi = Regex.Match(macConfigResult, patternWiFi, RegexOptions.IgnoreCase);
                if (matchWiFi.Success)
                {
                    string macAddress = matchWiFi.Groups[1].Value;
                    return macAddress;
                }
            }

            return "Nenhuma mídia em uso";
        }

        static string ObterEndereçosMACs()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = "/c getmac /v",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            process.StartInfo = startInfo;
            process.Start();

            string result = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return result;
        }
    }
}
