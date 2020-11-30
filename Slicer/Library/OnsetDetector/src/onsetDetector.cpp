
#include "onsetDetector.h"

#include "od_def.h"
#include "freqAnalyzer.h"

Spectrogram MakeSpectrogram(float* data, uint64_t dataLength);

int main() {
    Test();
}

void Test() {
	printf("test2");
}

void OnsetDetect(float* data, uint64_t dataLength, uint64_t sampleRate, double* onset, int onsetLength) {

    const int fft_window = 2048;
    const int slide = 2;
    int slide_window = fft_window / slide;

    if (dataLength < fft_window) return;

    Spectrogram spec = MakeSpectrogram(data, dataLength);
    spec = melSpectrogram(spec, sampleRate, 20);
    std::vector<int> peak = peakDetect(spectroFlux(spec), 10);

    //次の改良ネタ：高いほうから順番にする、AnalyzeとGetを分けて処理を軽くする
    onsetLength = peak.size() < onsetLength ? peak.size() : onsetLength;
    for (int i = 0; i < onsetLength; i++) {
        onset[i] = peak[i] * (double)slide_window / (double)sampleRate;
    }
}

Spectrogram MakeSpectrogram(float* data, uint64_t dataLength) {

    int length = 1024 * 2;
    int slide = 2;
    int div_num = dataLength / length * slide;

    Spectrogram spectrogram;

    for (int i = 0; i < div_num - 1; i++) {

        int start = length / slide * i;

        std::vector<float> array;
        for (int j = start; j < start + length; j++) {
            if (start + j >= dataLength) break;
            
            array.push_back(data[j]);
        }

        if (array.size() != length) break;

        std::valarray<double> result = fft(array.data(), array.size());
        spectrogram.push_back(result);
    }

    return spectrogram;
}