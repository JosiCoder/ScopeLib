using System;
using System.Collections.Generic;
using System.Linq;
using Lomont;

namespace ScopeLib.Sampling
{
    //TODO
    public class Fourier
    {
        private readonly LomontFFT _fft = new LomontFFT();

        public Fourier ()
        {
            //TODO which scaling?
            //_fft.A = 1;
        }

        public SampleSequence TransformForward(SampleSequence timeDomainSamples)
        {
            var fftValues = timeDomainSamples.Values.ToArray();

            // FFT frequency resolution is (sample rate) / (number of samples).
            var frequencyResolution =
                (1/timeDomainSamples.XInterval) / fftValues.Length;

            _fft.RealFFT(fftValues, true);
            var amplitudes = ComputeAmplitudes(fftValues);

            //TODO Test
            var a = amplitudes.ToArray();

            return new SampleSequence(frequencyResolution, amplitudes);
        }

        private IEnumerable<double> ComputeAmplitudes(double[] fftValues)
        {
            var length = fftValues.Length / 2;

            for (int i = 0; i < length; i++)
            {
                double x = fftValues[2 * i];
                double y = fftValues[2 * i + 1];
                yield return Math.Sqrt(x*x + y*y);
            }
        }
    }
}

