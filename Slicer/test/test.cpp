
#include "./AudioFile/AudioFile.h"
#include <matplotlib-cpp/matplotlibcpp.h>
#include <fftw3.h>

#include <valarray>
#include <cmath>

namespace plt = matplotlibcpp;

using Spectrogram = std::vector<std::valarray<double>>;

std::valarray<double> fft( float array[], int length );
void plot(std::vector<double> y);
void plot(std::vector<double> x, std::vector<double> y);

AudioFile<double> test_LoadAudioFile(){
    AudioFile<double> audioFile;
    audioFile.load ("/Users/waku/04_Charange/SingTempo/hinamatsuri_syn.wav");

    audioFile.printSummary();

    return audioFile;
}

std::vector<std::valarray<double>> test_spectrogram(){
    AudioFile<double> audioFile = test_LoadAudioFile();

    int length = 1024 * 2;
    int slide = 2;
    int div_num = audioFile.getNumSamplesPerChannel() / length * slide;

    std::vector<std::valarray<double>> spectrogram;

    for(int i = 0 ; i < div_num - 1; i++){
        int start = length / slide * i;
        float array[length];

        auto begin = audioFile.samples[0].begin() + start;
        auto end = begin + length;
        std::copy(begin, end, array);
        //std::memcpy(audioFile.samples.data()[0][start], array, length);

        std::valarray<double> result = fft( array, length );
        spectrogram.push_back( result );
    }

    std::cout << spectrogram.size() << std::endl;
    std::cout << spectrogram[0].size() << std::endl;

    return spectrogram;
}

std::vector<double> spectroFlux( std::vector<std::valarray<double>> spectrogram ){
    std::vector<double> flux;

    std::valarray<double> v_minus1;
    for( auto v: spectrogram ){
        if( v_minus1.size() != 0 ){
            auto diff = sqrt(( v - v_minus1 ) * ( v - v_minus1 )) * sqrt(v * v).sum();
            //std::cout <<  << std::endl;
            flux.push_back( diff.sum() );
        }

        v_minus1 = v;
	}

    return flux;
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

std::valarray<double> fft( float array[], int length ){
    const int fftsize = length;
    fftw_complex *in, *out;
    fftw_plan plan;
    int i;

    in = (fftw_complex *)fftw_malloc(sizeof(fftw_complex) * fftsize);
    out = (fftw_complex *)fftw_malloc(sizeof(fftw_complex) *fftsize);
    plan = fftw_plan_dft_1d(fftsize, in, out, FFTW_FORWARD, FFTW_ESTIMATE);

    for(i=0;i<fftsize;i++){
        in[i][0] = array[i] * (0.5 - 0.5 * cos(2 * M_PI * ((float)i/(float)(fftsize-1))));
        in[i][1] = 0.0;
    }

    fftw_execute(plan); /* repeat as needed */

    fftw_destroy_plan(plan);
    fftw_free(in);

    /* get amplitude */
    std::valarray<double> amplitude(fftsize);
    for(i=0;i<fftsize;i++){
        amplitude[i] = out[i][0] * out[i][0] + out[i][1] * out[i][1];
    }
    /* print the result */
    //for(i=0;i<fftsize;i++) printf("amplitude[%d] = %lf\n",i,amplitude[i]);

    fftw_free(out);

    return amplitude;
}

double hz2mel( double f ){
    return 2595 * log(f / 700.0 + 1.0);
}

double mel2hz( double m ){
    return 700 * (exp(m / 2595) - 1.0);
}

std::vector<std::valarray<double>> makeMelFilterBank( int sampleRate, int fft_window, int numChannels ){
    // ナイキスト周波数（Hz）
    double fmax = sampleRate / 2.0;
    // ナイキスト周波数（mel）
    double melmax = hz2mel(fmax);
    // 周波数インデックスの最大数
    int nmax = fft_window / 2;
    // 周波数解像度（周波数インデックス1あたりのHz幅）
    double df = sampleRate / fft_window;
    // メル尺度における各フィルタの中心周波数を求める
    double dmel = melmax / (double)(numChannels + 1);

    double melcenters[numChannels];
    double fcenters[numChannels];
    int indexcenter[numChannels];
    for(int i=1; i<numChannels+1; i++){
        melcenters[i-1] = i * dmel;
        fcenters[i-1] = mel2hz(melcenters[i-1]);
        indexcenter[i-1] = (int)(fcenters[i-1] / df);
    }

    int indexstart[numChannels];
    int indexstop[numChannels];
    for(int i = 0; i < numChannels; i++){
        indexstart[i] = ( i == 0 ) ? 0 : indexcenter[i - 1];
        indexstop[i] = ( i < numChannels - 1 ) ? indexcenter[i + 1] : nmax;
    }

    std::vector<std::valarray<double>> filterbank;
    for(int c = 0; c < numChannels; c++){
        std::valarray<double> filter(nmax);
        // 三角フィルタの左の直線の傾きから点を求める
        double increment = 1.0 / (float)(indexcenter[c] - indexstart[c]);
        for(int i = indexstart[c]; i < indexcenter[c]; i++){
            filter[i] = (i - indexstart[c]) * increment;
        }
        // 三角フィルタの右の直線の傾きから点を求める
        double decrement = 1.0 / (float)(indexstop[c] - indexcenter[c]);
        for(int i = indexcenter[c]; i < indexstop[c]; i++){
            filter[i] = 1.0 - ((i - indexcenter[c]) * decrement);
        }
        filterbank.push_back(filter);
    }

    return filterbank;
}

Spectrogram melSpectrogram( Spectrogram spec, int sampleRate, int channel = 20 ){
    auto filterBank = makeMelFilterBank( sampleRate, spec[0].size(), channel );

    Spectrogram melSpectrogram;

    for(int i=0; i<spec.size(); i++){
        std::valarray<double> melSpectrum(channel);

        for(int j=0; j<channel; j++){
            melSpectrum[j] = (filterBank[j] * spec[i]).sum();
        }

        melSpectrogram.push_back(melSpectrum);
    }

    return melSpectrogram;
}

std::vector<double> peakDetect( std::vector<double> data, int width ){
    std::vector<double> peak(data.size());

    std::vector<double> peak_point;

    for(int i=0; i < data.size() - width; i++){

        peak[i] = data[i];

        /* +++ 現在値よりwitdh前までにpeakがあればthruする +++ */
        int before_sum = 0;
        for(int j = 0; j < width; j++){
            before_sum += peak[ i - ( j + 1 ) ];
        }

        if ( ( i >= 1 ) && ( before_sum > 0 ) ){
            peak[i] = 0;
            continue;
        }

        /* +++ 現在値よりwitdh後までの値が全て小さければpeakとみなす +++ */
        for(int w = 1; w < width + 1; w++){
            if ( data[i] <= data[i + w] ){
                peak[i] = 0;
                break;
            }
        }

        if( peak[i] > 0 ){
            peak_point.push_back(i);
            //std::cout << (i * (double)1024 / (double)44100) << ", " << std::endl;
        }
    }

    return peak;
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