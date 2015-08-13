/* Date: 12.8.2015, Time: 2:20 */
using System;
using System.IO;
using System.Linq;
using System.Text;
using CreateAndPlayMIDI;

namespace IllidanS4.Amiga.Hippel
{
	public class HippelCosoFile
	{
		public HippelCosoFile(Stream stream, byte[] sampleData)
		{
			long pos = stream.Position;
			var reader = new BinaryReader(stream, Encoding.ASCII);
			string coso = new string(reader.ReadChars(4));
			if(coso == "COSO")
			{
				int p1 = reader.ReadInt32BE();
				int p2 =reader.ReadInt32BE();
				int p3 = reader.ReadInt32BE();
				int pnotes = reader.ReadInt32BE();
				int p5 = reader.ReadInt32BE();
				int pinstr = reader.ReadInt32BE();
				int psampl = reader.ReadInt32BE();
				string tfmx = new string(reader.ReadChars(4));
				if(tfmx == "TFMX")
				{
					reader.ReadInt16BE();
					reader.ReadInt16BE();
					reader.ReadInt16BE();
					int nnotes = reader.ReadInt16BE()+1;
					reader.ReadInt16BE();
					reader.ReadInt16BE();
					short nsongs = reader.ReadInt16BE();
					short ninstr = reader.ReadInt16BE();
					
					stream.Position = pos+pnotes;
					Pattern[] notes = new Pattern[nnotes];
					for(int i = 0; i < nnotes; i++)
					{
						notes[i] = new Pattern(reader);
					}
					
					stream.Position = pos+pinstr;
					Instrument[] instr = new Instrument[ninstr];
					for(int i = 0; i < ninstr; i++)
					{
						instr[i] = new Instrument(reader);
					}
					
					if(sampleData == null)
					{
						int endPos = instr.Max(i => i.SourceSampleStart+i.SourceSampleLength);
						stream.Position = psampl;
						sampleData = reader.ReadBytes(endPos);
					}
					
					for(int i = 0; i < ninstr; i++)
					{
						var inst = instr[i];
						Array.Copy(sampleData, inst.SourceSampleStart, inst.Sample, 0, inst.SourceSampleLength);
					}
					
					for(int j = 0; j < 12; j++)
					{
						Console.WriteLine("Byte "+j);
						Console.WriteLine(notes.Select(n => n.Data).Any(d => d[j] > 10));
						/*var b = notes[i].Data[j];
						if(b > 10)
						{
							Console.WriteLine(b);
						}*/
					}
				}
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
				return new HippelCosoFile(stream, File.ReadAllBytes(samples));
			}
		}
	}
	
	public struct Instrument
	{
		public int SourceSampleStart{get; private set;}
		public int SourceSampleLength{get; private set;}
		public int LoopStart{get; private set;}
		public byte[] Sample{get; private set;}
		
		public Instrument(BinaryReader reader) : this()
		{
			SourceSampleStart = reader.ReadInt32BE();
			SourceSampleLength = reader.ReadUInt16BE()*2;
			LoopStart = reader.ReadInt32BE();
			
			Sample = new byte[SourceSampleLength];
		}
	}
	
	public struct Pattern
	{
		public byte[] Data{get; private set;}
		
		public Pattern(BinaryReader reader) : this()
		{
			Data = reader.ReadBytes(12);
		}
	}
}
