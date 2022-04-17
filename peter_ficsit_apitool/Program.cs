using System;
using CommandLine;
using EnsureThat;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using peter_ficsit_api;
using peter_ficsit_api.GraphQL;
using peter_ficsit_api_implement;

namespace peter_ficsit_apitool
{
    internal class Program
    {
        public enum CommandType {
            NewModVersion,
            ModSearch,
            SMLVersions,
            ListMods,
            GetMod,
            GetModVersion
        }

        public class Options
        {
            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }

            [Option("Production", Required = false, HelpText = "Use Production Server. Staging is used by default.")]
            public bool Production { get; set; }

            [Option("APIKey", Required = false, HelpText = "Ficsit API Key.")]
            public string APIKey { get; set; }

            [Option("Command", Required = true, HelpText = $"Command Type.")]
            public CommandType CommandType { get; set; }


            [Option("SearchText", Required = false, HelpText = "Search Text.")]
            public string? SearchText { get; set; } = null;
            
            [Option("SearchLimit", Required = false, HelpText = "Search Limit.")]
            public int SearchLimit { get; set; } = 10;

            [Option("SearchPage", Required = false, HelpText = "Search Page.")]
            public int SearchPage { get; set; } = 1;


            //string ModID, string filepath, VersionStabilities stabilities, string Changelog

            [Option("ModID", Required = false, HelpText = "A Mods ModID.")]
            public string ModID { get; set; }

            [Option("VersionID", Required = false, HelpText = "A Mods VersionID.")]
            public string VersionID { get; set; }

            [Option("Filepath", Required = false, HelpText = "New Version Filepath.")]
            public string Filepath { get; set; }

            [Option("Changelog", Required = false, HelpText = "New Version Changelog text.")]
            public string Changelog { get; set; }

            [Option("Stability", Required = false, HelpText = "Version Stability. Values {Alpha / Beta / Release}")]
            public VersionStabilities Stability { get; set; }
        }

        public static ILogger logger { get; set; }

        static void Main(string[] args)
        {
            Options opts = null;

            using ILoggerFactory loggerFactory =
                LoggerFactory.Create(builder =>
                    builder.AddSimpleConsole(options =>
                    {
                        options.ColorBehavior = LoggerColorBehavior.Default;
                        options.IncludeScopes = false;
                        options.SingleLine = false;
                        options.TimestampFormat = "hh:mm:ss ";
                    }));

            logger = loggerFactory.CreateLogger("APITool");
            StaticOptions.Logger = logger;

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    opts = o;
                }).WithNotParsed(a =>
                {
                    foreach (Error error in a)
                    {
                        if (error.Tag == ErrorType.MissingRequiredOptionError)
                        {
                            MissingRequiredOptionError mroe = error as MissingRequiredOptionError;
                            logger.LogError("Missing Option " + mroe.NameInfo.LongName.ToString());
                            if (mroe.NameInfo.LongName == "Command")
                            {
                                string CommandOptions = $"CommandType Options: {(String.Join(", ", Enum.GetValues<Program.CommandType>().Select(x => x.ToString())))}";
                                logger.LogWarning(CommandOptions);
                            }
                        }
                        else
                        {
                            logger.LogError(error.ToString());
                        }
                    }

                    //Just quit
                    Environment.Exit(0);
                });

            StaticOptions.APIKEY = opts.APIKey;
            if (!opts.Production)
            {
                StaticOptions.APIURL = "https://api.ficsit.dev/v2/query";
            }
            else
            {
                StaticOptions.APIURL = "https://api.ficsit.app/v2/query";
            }
            Console.WriteLine();
            try
            {
                switch (opts.CommandType)
                {
                    case CommandType.ModSearch:
                        Ensure.String.IsNotNullOrEmpty(opts.SearchText, nameof(opts.SearchText));
                        Ensure.String.IsNotNullOrWhiteSpace(opts.SearchText, nameof(opts.SearchText));
                        ApiSearchImplem.DoModSearch(opts.SearchText, opts.SearchLimit);
                        break;
                    case CommandType.NewModVersion:
                        Ensure.String.IsNotNullOrEmpty(opts.APIKey, nameof(opts.APIKey));
                        Ensure.String.IsNotNullOrWhiteSpace(opts.APIKey, nameof(opts.APIKey));
                        Ensure.String.IsNotNullOrEmpty(opts.ModID, nameof(opts.ModID));
                        Ensure.String.IsNotNullOrWhiteSpace(opts.ModID, nameof(opts.ModID));
                        Ensure.String.IsNotNullOrEmpty(opts.Filepath, nameof(opts.Filepath));
                        Ensure.String.IsNotNullOrWhiteSpace(opts.Filepath, nameof(opts.Filepath));
                        Ensure.String.IsFilePath(opts.Filepath, nameof(opts.Filepath));
                        Ensure.String.IsNotNullOrEmpty(opts.Changelog, nameof(opts.Changelog));
                        Ensure.String.IsNotNullOrWhiteSpace(opts.Changelog, nameof(opts.Changelog));
                        Ensure.Enum.IsDefined(opts.Stability, nameof(opts.Stability));
                        if (!UploadFileImplem.UploadFile(opts.ModID, opts.Filepath, opts.Stability, opts.Changelog))
                            Environment.Exit(1);
                        break;
                    case CommandType.SMLVersions:
                        ApiSearchImplem.GetSMLVersions(opts.SearchLimit);
                        break;
                    case CommandType.ListMods:
                        ApiSearchImplem.ListMods(opts.SearchLimit, opts.SearchPage, opts.SearchText);
                        break;
                    case CommandType.GetMod:
                        Ensure.String.IsNotNullOrEmpty(opts.ModID, nameof(opts.ModID));
                        Ensure.String.IsNotNullOrWhiteSpace(opts.ModID, nameof(opts.ModID));
                        ApiSearchImplem.GetMod(opts.ModID);
                        break;
                    case CommandType.GetModVersion:
                        Ensure.String.IsNotNullOrEmpty(opts.VersionID, nameof(opts.VersionID));
                        Ensure.String.IsNotNullOrWhiteSpace(opts.VersionID, nameof(opts.VersionID));
                        ApiSearchImplem.GetModVersion(opts.ModID);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (ArgumentNullException ane)
            {
                logger.LogError($"Required Parameter Missing. {ane.ParamName} needs to be set for Command {opts.CommandType.ToString()}");
                logger.LogError(ane, ane.Message);
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                Environment.Exit(1);
            }

        }
    }
}