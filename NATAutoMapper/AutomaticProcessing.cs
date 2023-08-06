using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace NATAutoMapper
{
    /// <summary>
    /// Contiene metodi per l'elaborazione automatica dei comandi forniti tramite script o linea di comando.
    /// </summary>
    public static class AutomaticProcessing
    {
        /// <summary>
        /// Elabora un file di script.
        /// </summary>
        /// <param name="scriptPath">Percorso script.</param>
        /// <returns>Un intero che indica il risulato dell'elaborazione dello script.</returns>
        public static int ProcessScript(string scriptPath)
        {
            List<int> operationsExitCodes = new List<int>();
            int scriptExitCode = 0;
            List<string[]> scriptLinesSplittedList = new List<string[]>();
            string[][] scriptLinesSplitted = null;
            string[] scriptLines = null;
            try
            {
                scriptLines = File.ReadAllLines(scriptPath);
            }
            catch (PathTooLongException)
            {
                Console.WriteLine(Properties.NatAutoMapper.PathTooLongExceptionText);
                Console.ReadKey(true);
                return -1;
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine(Properties.NatAutoMapper.DirectoryNotFoundExceptionText);
                Console.ReadKey(true);
                return -1;
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(Properties.NatAutoMapper.UnauthorizedAccessExceptionDetailedText + e.Message);
                Console.ReadKey(true);
                return -1;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(Properties.NatAutoMapper.FileNotFoundExceptionText);
                Console.ReadKey(true);
                return -1;
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine(Properties.NatAutoMapper.NotSupportedExceptionDetailedText + e.Message);
                Console.ReadKey(true);
                return -1;
            }
            catch (System.Security.SecurityException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey(true);
                return -1;
            }
            catch (IOException e)
            {
                Console.WriteLine(Properties.NatAutoMapper.IOExceptionDetailedText + e.Message);
                Console.ReadKey(true);
                return -1;
            }
            foreach (string line in scriptLines)
            {
                List<string> splittedLine = line.Split(' ').ToList();
                bool startFound = false;
                List<string> quotedParameterComponents = new List<string>();
                for (int i = 0; i < splittedLine.Count; i++)
                {
                    if (splittedLine[i].Contains('"'))
                    {
                        if (!startFound)
                        {
                            startFound = true;
                            quotedParameterComponents.Add(splittedLine[i]);
                            splittedLine[i] = string.Empty;
                        }
                        else
                        {
                            quotedParameterComponents.Add(splittedLine[i]);
                            splittedLine[i] = string.Empty;
                            break;
                        }
                    }
                    else
                    {
                        if (startFound)
                        {
                            quotedParameterComponents.Add(splittedLine[i]);
                            splittedLine[i] = string.Empty;
                        }
                    }
                }
                if (quotedParameterComponents.Count > 0)
                {
                    splittedLine.RemoveAll(component => string.IsNullOrWhiteSpace(component));
                    string quotedParameter = string.Empty;
                    foreach (string component in quotedParameterComponents)
                    {
                        quotedParameter += component;
                    }
                    splittedLine.Add(quotedParameter);
                }
                scriptLinesSplittedList.Add(splittedLine.ToArray());
            }
            scriptLinesSplitted = scriptLinesSplittedList.ToArray();
            foreach (string[] splittedLine in scriptLinesSplitted)
            {
                string command = splittedLine[0];
                string[] parameters = new string[splittedLine.Length - 1];
                Array.Copy(splittedLine, 1, parameters, 0, parameters.Length);
                operationsExitCodes.Add(ProcessArguments(command, parameters));
            }
            if (operationsExitCodes.Exists(exitCode => exitCode > 0 || exitCode < 0))
            {
                scriptExitCode = 1;
            }
            return scriptExitCode;
        }

        /// <summary>
        /// Elabora la linea di comando.
        /// </summary>
        /// <param name="command">Comando da eseguire.</param>
        /// <param name="parameters">Eventuali parametri del comando.</param>
        /// <returns>Un intero che indica il risultato dell'elaborazione dello script.</returns>
        public static int ProcessArguments(string command, string[] parameters)
        {
            if ((command == "-add" || command == "-remove") && parameters == null)
            {
                return -6;
            }
            int exitCode = 0;
            int parameterIndex = 0;
            if (command == "-add")
            {
                string protocol = null;
                string internalPort = null;
                string externalPort = null;
                string ipAddress = null;
                string validFor = null;
                string description = null;
                if (parameters.Length < 12)
                {
                    Console.WriteLine(Properties.NatAutoMapper.InsufficientArgumentsError);
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }
                else if (parameters.Length > 12)
                {
                    Console.WriteLine(Properties.NatAutoMapper.InvalidArgumentsError);
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }
                else
                {
                    while (parameterIndex < parameters.Length)
                    {
                        switch (parameters[parameterIndex].ToLowerInvariant())
                        {
                            case "/protocol":
                                protocol = parameters[parameterIndex + 1];
                                parameterIndex += 2;
                                break;
                            case "/privateport":
                                internalPort = parameters[parameterIndex + 1];
                                parameterIndex += 2;
                                break;
                            case "/publicport":
                                externalPort = parameters[parameterIndex + 1];
                                parameterIndex += 2;
                                break;
                            case "/ipaddress":
                                ipAddress = parameters[parameterIndex + 1];
                                parameterIndex += 2;
                                break;
                            case "/lifetime":
                                validFor = parameters[parameterIndex + 1];
                                parameterIndex += 2;
                                break;
                            case "/description":
                                description = parameters[parameterIndex + 1];
                                parameterIndex += 2;
                                break;
                        }
                    }
                }
                exitCode = PortOperationsMethods.OpenPort(protocol, internalPort, externalPort, ipAddress, description, validFor);
            }
            else if (command == "-remove")
            {
                if (parameters.Length < 2)
                {
                    Console.WriteLine(Properties.NatAutoMapper.InsufficientArgumentsError);
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }
                else if (parameters.Length > 2)
                {
                    Console.WriteLine(Properties.NatAutoMapper.InvalidArgumentsError);
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }
                else
                {
                    string Description = parameters[1];
                    exitCode = PortOperationsMethods.ClosePort(Description);
                }
            }
            else if (command == "-list")
            {
                exitCode = PortOperationsMethods.ShowAllPortMappingsArgsMethod();
            }
            return exitCode;
        }
    }
}