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
			/*var hip = new JHPlayer();
			using(var stream = new FileStream("hipc.Title", FileMode.Open))
			{
				hip.loader(stream);
				var mid = new MIDISong();
				for(int i = 0; i < 4; i++) mid.AddTrack();
				
			}*/
			
			var hip = HippelCosoFile.Open("hipc.City_Walk", null);
			var song = new MIDISong();
			for(int i = 0; i < 4; i++)
			{
				var track = song.AddTrack(null);
				song.SetTempo(track, 3000/hip.Songs[0].Speed);
				song.SetTimeSignature(track, 4, 4);
			}
			
			/*for(int i = 0; i < hip.Voices.Length/4; i++)
			{
				for(int j = 0; j < 4; j++)
				{
					var voice = hip.Voices[i*4+j];
					int vsize = 12;
					for(int k = 0; k < vsize; k++)
					{
						int note = hip.Patterns[voice.PatternAddress*vsize+k].Note*2;
						//if(note <= 5) note = 0;
						song.AddNote(j, 0, note-1, 12);
					}
				}
			}*/
			
			for(int i = 0; i < hip.Patterns.Length; i++)
			{
				int note = hip.Patterns[i].Note;
				song.AddNote(0, 0, note*2-1, 12);
			}
			
			using(var stream = new FileStream("hip.mid", FileMode.Create))
			{
				song.Save(stream);
			}
			
			return;
			
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