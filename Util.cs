﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class Utilities
    {
        public static void Log(params object[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                Console.Write(args[i].ToString() + " ");
            }
            Console.WriteLine();
            return;
        }
    }
}
