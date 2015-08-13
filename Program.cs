/* Date: 8.5.2015, Time: 12:10 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CreateAndPlayMIDI;
using IllidanS4.Amiga.Hippel;
using IllidanS4.Amiga.SonicArranger;

namespace IllidanS4.Amiga.AmiMus
{
	static class Program
	{
		public static void Main(string[] args)
		{
			//SA2MIDI("sa.Extro_music");
			//SA2MIDI("sa.Amber20");
			
			var cfg = ReadConfig();
			
			Console.WriteLine("AmiMus v"+Assembly.GetExecutingAssembly().GetName().Version.ToString(2));
			Console.WriteLine("Created by IllidanS4@gmail.com");
			if(args.Length != 2)
			{
				Console.WriteLine("Usage:");
				Console.WriteLine("  amimus [input] [output]");
				Console.WriteLine("Example:");
				Console.WriteLine("  amimus [sa.Song1] [Song1.mid]");
				Console.WriteLine("Currently supported formats:");
				Console.WriteLine("Input - Sonic Arranger (classic or with replayer)");
				Console.WriteLine("Output - MIDI");
			}else{
				SA2MIDI(args[0], args[1]);
				Console.WriteLine("Song converted.");
			}
		}
		
		public static void SA2MIDI(string file, string output)
		{
			var sa = SonicArrangerFile.Open(file);
			var song = sa.ToMidi(ReadConfig());
			using(var stream = new FileStream(output, FileMode.Create))
			{
				song.Save(stream);
			}
		}
		
		public static Dictionary<string, int> ReadConfig()
		{
			var dict = new Dictionary<string, int>();
			if(!File.Exists("amimus.ini")) return dict;
			using(var reader = File.OpenText("amimus.ini"))
			{
				string line;
				while((line = reader.ReadLine()) != null)
				{
					line = line.Trim();
					if(line.Length > 0)
					{
						if(line[0] == '#')
						{
							continue;
						}else{
							string[] split = line.Split(new[]{'='}, 2, StringSplitOptions.RemoveEmptyEntries);
							try{
								dict[split[0]] = Int32.Parse(split[1]);
							}catch{
								Console.WriteLine("Malformed settings line: "+line);
							}
						}
					}
				}
			}
			return dict;
		}
	}
}