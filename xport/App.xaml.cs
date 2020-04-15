﻿//*********************************************************************
//xTools
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://xtools.xarial.com
//License: https://xtools.xarial.com/license/
//*********************************************************************

using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Xarial.XTools.Xport.Core;

namespace Xarial.XTools.Xport
{
    public partial class App : Application
    {
        [DllImport("Kernel32.dll")]
        private static extern bool AttachConsole(int processId);

        private class ConsoleProgressWriter : IProgress<double>
        {
            public void Report(double value)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Progress: {(value * 100).ToString("F")}%");
                Console.ResetColor();
            }
        }
        
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Any())
            {
                AttachConsole(-1);

                var parser = new Parser(p =>
                {
                    p.CaseInsensitiveEnumValues = true;
                    p.AutoHelp = true;
                    p.EnableDashDash = true;
                    p.HelpWriter = Console.Out;
                    p.IgnoreUnknownArguments = false;
                });

                var hasError = false;

                Arguments args = null;
                parser.ParseArguments<Arguments>(e.Args)
                    .WithParsed(a => args = a)
                    .WithNotParsed(err => hasError = true);

                var res = false;

                if (!hasError) 
                {
                    try
                    {
                        var task = RunConsoleExporter(args);
                        task.ConfigureAwait(false);
                        task.Wait();
                        res = true;
                    }
                    catch (Exception ex)
                    {
                        //TODO: message exception
                        PrintError(ex.Message);
                    }
                }

                Environment.Exit(res ? 0 : 1);
            }
            else
            {
                base.OnStartup(e);
            }
        }

        private async Task RunConsoleExporter(Arguments args) 
        {
            var opts = new ExportOptions()
            {
                Input = args.Input?.ToArray(),
                Filter = args.Filter,
                Format = args.Format?.ToArray(),
                OutputDirectory = args.OutputDirectory,
                ContinueOnError = args.ContinueOnError
            };

            using (var exporter = new Exporter(Console.Out, new ConsoleProgressWriter()))
            {
                await exporter.Export(opts).ConfigureAwait(false);
            }
        }

        private void PrintError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
    }
}
