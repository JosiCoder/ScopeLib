using System;
using System.Collections.Generic;
using System.Linq;
using Lomont;

namespace ScopeLib.Sampling
{
    /// <summary>
    /// Provides FFT transform for converting time-domain data to the frequency-domain
    /// and vice versa. It is based on the work of other people I appreciate.
    /// More information about different FFT implementations is available here:
    /// https://www.codeproject.com/Articles/1095473/Comparison-of-FFT-implementations-for-NET
    /// </summary>
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
            var sampleRate = 1 / timeDomainSamples.XInterval;

            // FFT frequency resolution is (sample rate) / (FFT window size).
            var frequencyResolution = sampleRate / fftValues.Length;

            _fft.RealFFT(fftValues, true); // fftValues is modified in place
            var amplitudes = ComputeAmplitudes(fftValues).ToArray();

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

