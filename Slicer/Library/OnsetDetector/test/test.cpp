
#include "./AudioFile/AudioFile.h"
#include <matplotlib-cpp/matplotlibcpp.h>

#include <valarray>

namespace plt = matplotlibcpp;

void plot(std::vector<double> y);
void plot(std::vector<double> x, std::vector<double> y);

AudioFile<double> test_LoadAudioFile() {
    AudioFile<double> audioFile;
    audioFile.load("/Users/waku/04_Charange/SingTempo/hinamatsuri_syn.wav");

    audioFile.printSummary();

    return audioFile;
}

std::vector<std::valarray<double>> test_spectrogram() {
    AudioFile<double> audioFile = test_LoadAudioFile();

    int length = 1024 * 2;
    int slide = 2;
    int div_num = audioFile.getNumSamplesPerChannel() / length * slide;

    std::vector<std::valarray<double>> spectrogram;

    for (int i = 0; i < div_num - 1; i++) {
        int start = length / slide * i;
        float array[length];

        auto begin = audioFile.samples[0].begin() + start;
        auto end = begin + length;
        std::copy(begin, end, array);
        //std::memcpy(audioFile.samples.data()[0][start], array, length);

        std::valarray<double> result = fft(array, length);
        spectrogram.push_back(result);
    }

    std::cout << spectrogram.size() << std::endl;
    std::cout << spectrogram[0].size() << std::endl;

    return spectrogram;
}

void test_MatplotLib(){
    int n = 1000;
    std::vector<double> x(n), y(n);

    for(int i=0; i<n; ++i) {
        x[i] = 2 * M_PI * i / n;
        y[i] = sin(x[i]);
    }

    plt::plot(x, y);
    plt::show();
}

void test_melFilterBank(){
    auto filterBank = makeMelFilterBank( 22050, 2048, 20 );

    for(int i=0; i<filterBank.size(); i++){
        std::vector<double> arr(begin(filterBank[i]), end(filterBank[i]));
        plot( arr );
    }

    plt::show();
}

void test_spectroFlux(){
    auto spectrogram = test_spectrogram();
    auto flux = spectroFlux(spectrogram);
    plot( flux );
    plt::show();
}

void test_spectroFlux_mel(){
    auto spectrogram = test_spectrogram();
    spectrogram = melSpectrogram( spectrogram, 22050, 20 );
    auto flux = spectroFlux(spectrogram);

    std::vector<double> times;
    for(int i=0; i<flux.size(); i++){
        double time = (double)1024 / (double)44100 * (double)i;
        times.push_back(time);
    }

    plot( times, flux );
    plot( times, peakDetect(flux, 10) );
    plt::show();
}

void plot(std::vector<double> y){
    int n = y.size();
    std::vector<double> x(n);

    for(int i=0; i<n; i++) {
        x[i] = i;
    }

    plt::plot(x, y);
}

void plot(std::vector<double> x, std::vector<double> y){
    plt::plot(x, y);
}

int main()
{
    printf("test\n");

    test_spectroFlux_mel();
}