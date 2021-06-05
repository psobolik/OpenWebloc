using CommandLine;
using CommandLine.Text;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWebloc
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Trace("Args: {args}", string.Join(", ", args));

            var arguments = Parser.Default.ParseArguments<CommandLineOptions>(args);
            arguments.WithParsed(options =>
            {
                foreach (var path in options.Paths)
                {
                    try
                    {
                        OpenWebloc(path, logger);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "Exception");
                    }
                }

            });//.WithNotParsed(errs => Console.Error.WriteLine("Failed with errors:\n{0}", String.Join("\n", errs)));
            NLog.LogManager.Shutdown();
#if DEBUG
            Console.ReadKey();
#endif
        }

        static void OpenWebloc(string path, Logger logger)
        {
            try
            {
                logger.Trace("Processing {path}", path);
                var plist = PlistCS.Plist.readPlist(path) as Dictionary<string, object>;

                logger.Trace("Opening URL: {url}", plist["URL"]);
                Process.Start(plist["URL"].ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }
        }

        internal class CommandLineOptions
        {

            [Usage]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    return new List<Example>() 
                    {
                        new Example("Open the URLs in webloc files in the default browser", new CommandLineOptions { Paths = new string[] { "<webloc 1>", "<webloc 2>" } })
                    };
                }
            }

            [Value(index: 0, MetaName = "<path of webloc>", Required = true, HelpText = "Path of webloc file.")]
            public IEnumerable<string> Paths { get; set; }
        }
    }
}
