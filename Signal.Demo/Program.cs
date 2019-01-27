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

namespace ScopeLib.Signal.Demo
{
    class MainClass
    {
        private const string _inFilePath = @"DemoWaveformFile.wav";
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
