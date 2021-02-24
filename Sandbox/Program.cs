﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FF1Lib;
using FF1Lib.Procgen;
using RomUtilities;

namespace Sandbox
{
	internal class Program
	{
		// private static FF1Rom rom;

		private static async Task Main()
		{
			//var filename = "ff1.nes";
			//var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);

			//rom = new FF1Rom(fs);

			await Performance.Run();
		}
	}
}
