﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.CommandLineParser;

namespace Jhu.Graywulf.Registry.CmdLineUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            List<Type> verbs = new List<Type>() { typeof(CreateDb), typeof(CreateCluster), typeof(CreateAdmin), typeof(Save), typeof(Merge) };

            Verb v = null;

            try
            {
                PrintHeader();
                v = (Verb)ArgumentParser.Parse(args, verbs);
            }
            catch (ArgumentParserException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                Console.WriteLine();

                ArgumentParser.PrintUsage(verbs, Console.Out);
            }

            if (v != null)
            {
                v.Run();
            }
        }

        private static void PrintHeader()
        {
            Console.WriteLine(
@"JHU Graywulf Cluster Registry Command-Line Utility
(c) 2008-2012 László Dobos dobos@pha.jhu.edu
Department of Physics and Astronomy, The Johns Hopkins University

");
        }
    }
}
