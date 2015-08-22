/* Date: 12.8.2015, Time: 2:20 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CreateAndPlayMIDI;

namespace IllidanS4.Amiga.Hippel
{
	public class HippelCosoFile
	{
		public Sample[] Samples{get; private set;}
		public Song[] Songs{get; private set;}
		public Voice[] Voices{get; private set;}
		public Pattern[] Patterns{get; private set;}
		
		public HippelCosoFile(Stream stream, byte[] sampleData)
		{
			long pos = stream.Position;
			var reader = new BinaryReader(stream, Encoding.ASCII);
			string coso = new string(reader.ReadChars(4));
			if(coso == "COSO")
			{
				int frqseqs = reader.ReadInt32BE();
				int volseqs = reader.ReadInt32BE();
				int patternsPtr = reader.ReadInt32BE();
				int voicesPtr = reader.ReadInt32BE();
				int songsData = reader.ReadInt32BE();
				int headers = reader.ReadInt32BE();
				int samplesPtr = reader.ReadInt32BE();
				
				/*string tfmx = new string(reader.ReadChars(4));
				if(tfmx == "TFMX")
				{
					reader.ReadInt16BE();
					reader.ReadInt16BE();
					reader.ReadInt16BE();
					int npatterns = reader.ReadInt16BE()+1;
					reader.ReadInt16BE();
					reader.ReadInt16BE();
					short nsongs = reader.ReadInt16BE();
					short nsamples = reader.ReadInt16BE();
				}*/
				
				int nsamples = (samplesPtr-headers)/10 - 1;
				stream.Position = pos+headers;
				
				Samples = new Sample[nsamples];
				for(int i = 0; i < nsamples; i++)
				{
					Samples[i] = new Sample(reader);
				}
				
				if(sampleData == null)
				{
					int endPos = Samples.Max(i => i.SourceStart+i.SourceLength);
					stream.Position = samplesPtr;
					sampleData = reader.ReadBytes(endPos);
				}
				if(sampleData.Length > 0)
				{
					for(int i = 0; i < nsamples; i++)
					{
						var inst = Samples[i];
						Array.Copy(sampleData, inst.SourceStart, inst.Data, 0, inst.SourceLength);
					}
				}
				
				int nsongs = (headers-songsData)/6;
				stream.Position = pos+songsData;
				
				Songs = new Song[nsongs];
				for(int i = 0; i < nsongs; i++)
				{
					Songs[i] = new Song(reader);
				}
				
				int nvoices = (songsData-voicesPtr)/3;
				stream.Position = pos+voicesPtr;
				
				Voices = new Voice[nvoices];
				for(int i = 0; i < nvoices; i++)
				{
					Voices[i] = new Voice(reader);
				}
				
				stream.Position = pos+patternsPtr;
				
				int npatterns = (voicesPtr-patternsPtr)/2;
				Patterns = new Pattern[npatterns];
				
				for(int i = 0; i < npatterns; i++)
				{
					Patterns[i] = new Pattern(reader);
				}
				
				
				
				
				/*var patterns = new List<Pattern>();
				while(stream.Position < pos+voicesPtr)
				{
					patterns.Add(new Pattern(reader));
				}
				int c = patterns.Count;
				int x = voices.Max(v => v.PatternAddress);*/
				
				
				//int npatterns = (voicesPtr-patternsPtr)/
				
				/*stream.Position = pos+tracks;
				Pattern[] notes = new Pattern[nnotes];
				for(int i = 0; i < nnotes; i++)
				{
					notes[i] = new Pattern(reader);
				}
				
				for(int j = 0; j < 12; j++)
				{
					Console.WriteLine("Byte "+j);
					Console.WriteLine(notes.Select(n => n.Data).Any(d => d[j] > 10));
				}*/
			}
		}
		
		public MIDISong ToMidi()
		{
			return null;
		}
		
		public static HippelCosoFile Open(string file, string samples)
		{
			using(var stream = new FileStream(file, FileMode.Open))
			{
				byte[] sampleData = null;
				if(samples != null) sampleData = File.ReadAllBytes(samples);
				return new HippelCosoFile(stream, sampleData);
			}
		}
	}
	
	public struct Pattern
	{
		public byte Note{get; private set;}
		public byte Command{get; private set;}
		public int Info{get; private set;}
		
		public Pattern(BinaryReader reader) : this()
		{
			byte sig = reader.ReadByte();
			switch(sig)
			{
				case 0xFF: //transpose
					Command = 1;
					Info = reader.ReadByte();
					break;
				case 0xFE: //speed
					Command = 2;
					Info = reader.ReadByte();
					break;
				case 0xFD: //speed, loop
					Command = 3;
					Info = reader.ReadByte();
					break;
				default:
					Note = sig;
					Info = reader.ReadByte();
					break;
			}
		}
	}
	
	public struct Voice
	{
		public byte PatternAddress{get; private set;}
		public byte TrackTranspose{get; private set;}
		public byte VolTranspose{get; private set;}
		
		public Voice(BinaryReader reader) : this()
		{
			PatternAddress = reader.ReadByte();
			TrackTranspose = reader.ReadByte();
			TrackTranspose = reader.ReadByte();
		}
	}
	
	public struct Song
	{
		public short Start{get; private set;}
		public short End{get; private set;}
		public short Speed{get; private set;}
		
		public Song(BinaryReader reader) : this()
		{
			Start = reader.ReadInt16BE();
			End = reader.ReadInt16BE();
			Speed = reader.ReadInt16BE();
		}
	}
	
	public struct Sample
	{
		public int SourceStart{get; private set;}
		public int SourceLength{get; private set;}
		public int LoopStart{get; private set;}
		public byte[] Data{get; private set;}
		
		public Sample(BinaryReader reader) : this()
		{
			SourceStart = reader.ReadInt32BE();
			SourceLength = reader.ReadUInt16BE()*2;
			LoopStart = reader.ReadInt32BE();
			
			Data = new byte[SourceLength];
		}
	}
}
