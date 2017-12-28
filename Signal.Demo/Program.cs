//------------------------------------------------------------------------------
// Copyright (C) 2017 Josi Coder

// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.

// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.

// You should have received a copy of the GNU General Public License along with
// this program. If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Media;
using ScopeLib.Utilities;

// https://de.wikipedia.org/wiki/RIFF_WAVE
// https://csharp-tricks.blogspot.de/2011/03/wave-dateien-einlesen.html
// https://blogs.msdn.microsoft.com/dawate/2009/06/23/intro-to-audio-programming-part-2-demystifying-the-wav-format/
// https://channel9.msdn.com/coding4fun/articles/Generating-Sound-Waves-with-C-Wave-Oscillators
// https://web.archive.org/web/20160104081350/http://www.iem.thm.de/telekom-labor/zinke/nw/vp/doku/dito41.htm#Heading54

// short-Bereich nur von -32760 to 32760 ?

// Im Fall von PCM-Daten enthält jeder Frame ein „Sample“ von Abtastwerten, einen pro Kanal.
// Bei zwei Kanälen (Stereo) wird erst der linke, dann der rechte Kanal gespeichert.
// Abtastwerte sind für Bit-Tiefen bis 8 als unsigned char kodiert, sonst signed int.
// Bei Bit-Tiefen, die nicht durch 8 teilbar sind, wird das niederwertigste Byte (LSB)
// rechts mit Nullen aufgefüllt (Zero-Padding).
// Das ergibt beispielsweise für den größten positiven 12-Bit-Wert „0x7FF“ die Bytefolge
// „0xF0 0x7F“. 

// This appears to be wrong.  Again, I haven't touched raw samples myself, but everything I've read says that 8-bit values
// are stored as Offset Binary (0-255).  Treating them as 8-bit signed is going to irreparably mangle the audio.
// If you want to *convert* them to signed, I believe you can just flip the "sign" bit.  (XOR with 0x80.)  This will have
// the effect of converting them to 2's complement values instead.

namespace ScopeLib.Signal.Demo
{
    class MainClass
    {
        private const string _inFilePath = @"../../Demo Data/DemoWaveformFile.wav";
        private const string _copyFile1Path = @"TempDemoStreamCopy.wav";
        private const string _copyFile2Path = @"TempDemoMemoryCopy.wav";
        private const string _createFile3Path = @"TempDemoGenerated.wav";

        public static StreamWaveform ReadWaveformFile(string path)
        {
            var waveFormReader = new WaveformFileWaveformReader(path);
            return waveFormReader.Read();
        }

        public static void ReadAndProcessWaveformFile (string path)
        {
            using (var waveForm = ReadWaveformFile(path))
            {
                var frameCount = 0;
                foreach(var frame in waveForm.GetFrames(1))
                {
                    frameCount++;
                }
                Console.WriteLine("Frames read: {0}/{1}", frameCount, waveForm.FrameCount);
            }
        }

        public static void WriteWaveformFile (string path, WaveformBase waveform)
        {
            using (var waveFormWriter = new WaveformFileWaveformWriter(path))
            {
                waveFormWriter.Write(waveform);
            }
        }

        public static void CopyWaveformFileViaStream (string inPath, string outPath)
        {
            using (var waveForm = ReadWaveformFile(inPath))
            {
                WriteWaveformFile(outPath, waveForm);
            }
        }

        public static void CopyWaveformFileViaMemory (string inPath, string outPath)
        {
            MemoryWaveform memoryWaveForm;
            using (var waveForm = ReadWaveformFile(inPath))
            {
                memoryWaveForm = new MemoryWaveform(waveForm.Format, waveForm.GetFrames().ToList());
            }
            WriteWaveformFile(outPath, memoryWaveForm);
        }

        public static void GenerateWaveformFile (string outPath)
        {
            short channelsCount = 2;
            int samplesPerSecond = 44100;
            short bitsPerSample = 16;

            var frequency = 440;
            var durationInSeconds = 2;
            var amplitude = 0.1;

            var frames = FunctionValueGenerator
                .GenerateSineValuesForFrequency(frequency, samplesPerSecond, durationInSeconds,
                    (x, y) =>
                {                
                    var samples = new []{ amplitude * y, amplitude * y };
                    return new WaveForm16BitFrame(samples);
                });

            var format = new WaveformFormat(channelsCount, samplesPerSecond, bitsPerSample);
            var waveForm = new MemoryWaveform(format, frames);
            WriteWaveformFile(outPath, waveForm);
        }

        public static void Main (string[] args)
        {
            Console.WriteLine("Hello World!");

            ReadAndProcessWaveformFile(_inFilePath);
            CopyWaveformFileViaStream(_inFilePath, _copyFile1Path);
            CopyWaveformFileViaMemory(_inFilePath, _copyFile2Path);
            GenerateWaveformFile(_createFile3Path);

            using (var stream = new FileStream(_createFile3Path, FileMode.Open, FileAccess.Read))
            {
                var player = new SoundPlayer(stream);

                player.PlaySync();
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
