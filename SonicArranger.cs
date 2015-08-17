using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CreateAndPlayMIDI;

/* Date: 12.8.2015, Time: 2:15 */

namespace IllidanS4.Amiga.SonicArranger
{
	public class SonicArrangerFile
	{
		public string Version{get; private set;}
		
		public Song[] Songs{get; private set;}
		public Voice[] Voices{get; private set;}
		public Note[] Notes{get; private set;}
		public Instrument[] Instruments{get; private set;}
		
		private static readonly byte[] pcstart = new byte[]{0, 0, 0, 0x28};
		
		public SonicArrangerFile(Stream stream)
		{
			var reader = new BinaryReader(stream, Encoding.ASCII);
			string soar = new string(reader.ReadChars(4));
			if(soar != "SOAR")
			{
				long start = ReadTo(reader, pcstart);
				int songptr = 0x28;
				int ovtbptr = reader.ReadInt32BE();
				int notbptr = reader.ReadInt32BE();
				int intbptr = reader.ReadInt32BE();
				int syntptr = reader.ReadInt32BE();
				
				stream.Position = start+songptr;
				int songs = (ovtbptr-songptr)/12;
				Songs = new Song[songs];
				for(int i = 0; i < songs; i++)
				{
					Songs[i] = new Song(reader);
				}
				
				stream.Position = start+ovtbptr;
				int voices = (notbptr-ovtbptr)/4;
				Voices = new Voice[voices];
				for(int i = 0; i < voices; i++)
				{
					Voices[i] = new Voice(reader);
				}
				
				stream.Position = start+notbptr;
				int notes = (intbptr-notbptr)/4;
				Notes = new Note[notes];
				for(int i = 0; i < notes; i++)
				{
					Notes[i] = new Note(reader);
				}
				
				stream.Position = start+intbptr;
				int instrs = (syntptr-intbptr)/152;
				Instruments = new Instrument[instrs];
				for(int i = 0; i < instrs; i++)
				{
					Instruments[i] = new Instrument(reader);
				}
			}else{
				Version = new string(reader.ReadChars(4));
				
				string tag;
				while(true)
				{
					tag = new string(reader.ReadChars(4));
					switch(tag)
					{
						case "STBL":
							Songs = new SongTable(reader).Songs;
							break;
						case "OVTB":
							Voices = new OverTable(reader).Voices;
							break;
						case "NTBL":
							Notes = new NoteTable(reader).Notes;
							break;
						case "INST":
							Instruments = new InstrumentTable(reader).Instruments;
							break;
						default:
							return;
					}
				}
			}
		}
		
		public MIDISong ToMidi(Dictionary<string, int> instrumentMap, int songId = 0)
		{
			var inst = Instruments;
			var ovtb = Voices;
			var stbl = Songs[songId];
			var notb = Notes;
			var song = new MIDISong();
			for(int i = 0; i < 4; i++)
			{
				var track = song.AddTrack(null);
				song.SetTempo(track, 2956/stbl.SongSpeed);
				song.SetTimeSignature(track, 4, 4);
			}
			for(int i = 0; i < inst.Length; i++)
			{
				var instr = inst[i];
				int minst;
				if(!instrumentMap.TryGetValue(instr.Name, out minst))
				{
					Console.WriteLine("Unknown instrument "+instr.Name);
				}else{
					Console.WriteLine(minst+" set for "+instr.Name);
				}
				if(i < 15 && i != 9)
				{
					for(int j = 0; j < 4; j++)
					{
						if(minst != -1)
						{
							song.SetChannelInstrument(j, i, minst);
						}
					}
				}
			}
			
			int lastinstr = Int32.MinValue;
			for(int i = stbl.StartPos; i < stbl.StopPos; i++)
			{
				for(int j = 0; j < 4; j++)
				{
					var voice = ovtb[i*4+j];
					for(int k = 0; k < stbl.PatternLength; k++)
					{
						var note = notb[voice.NoteAddress+k];
						int instr = note.Instrument-1;
						if(instr >= 0)
						{
							if(instr == 9 || instr >= 15)
							{
								if(lastinstr != instr)
								{
									int minst;
									instrumentMap.TryGetValue(inst[instr].Name, out minst);
									song.SetChannelInstrument(j, 15, minst);
									lastinstr = instr;
								}
								song.AddNote(j, 15, note.Value-1, 12);
							}else{
								song.AddNote(j, instr, note.Value-1, 12);
							}
						}else{
							song.AddNote(j, 0, -1, 12);
						}
					}
				}
			}
			return song;
		}
		
		private static long ReadTo(BinaryReader reader, byte[] find)
		{
			try{
				while(true)
				{
					long pos = reader.BaseStream.Position;
					bool found = true;
					foreach(byte b in find)
					{
						if(b != reader.ReadByte())
						{
							found = false;
							break;
						}
					}
					if(found)
					{
						return pos;
					}
					reader.BaseStream.Position = pos+1;
				}
			}catch(EndOfStreamException)
			{
				return -1;
			}
		}
		
		public static SonicArrangerFile Open(string file)
		{
			using(var stream = new FileStream(file, FileMode.Open))
			{
				return new SonicArrangerFile(stream);
			}
		}
	}
	
	public class SongTable
	{
		public int Count{get; private set;}
		public Song[] Songs{get; private set;}
		
		public SongTable(BinaryReader reader)
		{
			Count = reader.ReadInt32BE();
			Songs = new Song[Count];
			for(int i = 0; i < Count; i++)
			{
				Songs[i] = new Song(reader);
			}
		}
	}
	
	public struct Song
	{
		public short SongSpeed{get; private set;}
		public short PatternLength{get; private set;}
		public short StartPos{get; private set;}
		public short StopPos{get; private set;}
		public short RepeatPos{get; private set;}
		public short NBIrqps{get; private set;}
		
		public Song(BinaryReader reader) : this()
		{
			SongSpeed = reader.ReadInt16BE();
			PatternLength = reader.ReadInt16BE();
			StartPos = reader.ReadInt16BE();
			StopPos = reader.ReadInt16BE();
			RepeatPos = reader.ReadInt16BE();
			NBIrqps = reader.ReadInt16BE();
		}
	}
	
	public class OverTable
	{
		public int Count{get; private set;}
		public Voice[] Voices{get; private set;}
		
		public OverTable(BinaryReader reader)
		{
			Count = reader.ReadInt32BE();
			Voices = new Voice[Count*4];
			for(int i = 0; i < Count*4; i++)
			{
				Voices[i] = new Voice(reader);
			}
		}
	}
	
	public struct Voice
	{
		public short NoteAddress{get; private set;}
		public byte SoundTranspose{get; private set;}
		public byte NoteTranspose{get; private set;}
		
		public Voice(BinaryReader reader) : this()
		{
			NoteAddress = reader.ReadInt16BE();
			SoundTranspose = reader.ReadByte();
			NoteTranspose = reader.ReadByte();
		}
	}
	
	public class NoteTable
	{
		public int Count{get; private set;}
		public Note[] Notes{get; private set;}
	
		public NoteTable(BinaryReader reader)
		{
			Count = reader.ReadInt32BE();
			Notes = new Note[Count];
			for(int i = 0; i < Count; i++)
			{
				Notes[i] = new Note(reader);
			}
		}
	}
	
	public struct Note
	{
		public byte Value{get; private set;}
		public byte Instrument{get; private set;}
		public byte Command{get; private set;}
		public byte CommandInfo{get; private set;}
		
		public Note(BinaryReader reader) : this()
		{
			Value = reader.ReadByte();
			Instrument = reader.ReadByte();
			Command = reader.ReadByte();
			CommandInfo = reader.ReadByte();
		}
	}

	public class InstrumentTable
	{
		public int Count{get; private set;}
		public Instrument[] Instruments{get; private set;}
		
		public InstrumentTable(BinaryReader reader)
		{
			Count = reader.ReadInt32BE();
			Instruments = new Instrument[Count];
			for(int i = 0; i < Count; i++)
			{
				Instruments[i] = new Instrument(reader);
			}
		}
	}
	
	public struct Instrument
	{
		public short SynthMode{get; private set;}
		public short SampleWaveNo{get; private set;}
		public short Length{get; private set;}
		public short Repeat{get; private set;}
		//8
		public short Volume{get; private set;}
		public short FineTuning{get; private set;}
		public short Portamento{get; private set;}
		public short VibDelay{get; private set;}
		public short VibSpeed{get; private set;}
		public short VibLevel{get; private set;}
		public short AmfWave{get; private set;}
		public short AmfDelay{get; private set;}
		public short AmfLength{get; private set;}
		public short AmfRepeat{get; private set;}
		public short AdsrWave{get; private set;}
		public short AdsrDelay{get; private set;}
		public short AdsrLength{get; private set;}
		public short AdsrRepeat{get; private set;}
		public short SustainPt{get; private set;}
		public short SustainVal{get; private set;}
		//16
		public short EffectNumber{get; private set;}
		public short Effect1{get; private set;}
		public short Effect2{get; private set;}
		public short Effect3{get; private set;}
		public short EffectDelay{get; private set;}
		
		public Arpeggiato[] ArpegData{get; private set;}
		
		public string Name{get; private set;}
		
		public Instrument(BinaryReader reader) : this()
		{
			SynthMode = reader.ReadInt16BE();
			SampleWaveNo = reader.ReadInt16BE();
			Length = reader.ReadInt16BE();
			Repeat = reader.ReadInt16BE();
			reader.ReadBytes(8);
			Volume = reader.ReadInt16BE();
			FineTuning = reader.ReadInt16BE();
			Portamento = reader.ReadInt16BE();
			VibDelay = reader.ReadInt16BE();
			VibSpeed = reader.ReadInt16BE();
			VibLevel = reader.ReadInt16BE();
			AmfWave = reader.ReadInt16BE();
			AmfDelay = reader.ReadInt16BE();
			AmfLength = reader.ReadInt16BE();
			AmfRepeat = reader.ReadInt16BE();
			AdsrWave = reader.ReadInt16BE();
			AdsrDelay = reader.ReadInt16BE();
			AdsrLength = reader.ReadInt16BE();
			AdsrRepeat = reader.ReadInt16BE();
			SustainPt = reader.ReadInt16BE();
			SustainVal = reader.ReadInt16BE();
			reader.ReadBytes(16);
			EffectNumber = reader.ReadInt16BE();
			Effect1 = reader.ReadInt16BE();
			Effect2 = reader.ReadInt16BE();
			Effect3 = reader.ReadInt16BE();
			EffectDelay = reader.ReadInt16BE();
			
			ArpegData = new Arpeggiato[3];
			for(int i = 0; i < ArpegData.Length; i++)
			{
				ArpegData[i] = new Arpeggiato(reader);
			}
			Name = new string(reader.ReadChars(30)).Split(new[]{'\0'}, 2)[0];
		}
		
		public override string ToString()
		{
			return Name;
		}
	}
	
	public struct Arpeggiato
	{
		public byte Length{get; private set;}
		public byte Repeat{get; private set;}
		public byte[] Data{get; private set;}
		
		public Arpeggiato(BinaryReader reader) : this()
		{
			Length = reader.ReadByte();
			Repeat = reader.ReadByte();
			Data = reader.ReadBytes(14);
		}
	}
}