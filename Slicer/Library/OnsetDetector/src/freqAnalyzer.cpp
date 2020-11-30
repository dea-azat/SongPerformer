#define _USE_MATH_DEFINES
#include <cmath>

#include "od_def.h"
#include "freqAnalyzer.h"

#include <fftw3.h>

std::vector<double> spectroFlux(std::vector<std::valarray<double>> spectrogram) {
    std::vector<double> flux;

    std::valarray<double> v_minus1;
    for (auto v : spectrogram) {
        if (v_minus1.size() != 0) {
            auto diff = sqrt((v - v_minus1) * (v - v_minus1)) * sqrt(v * v).sum();
            //std::cout <<  << std::endl;
            flux.push_back(diff.sum());
        }

        v_minus1 = v;
    }

    return flux;
}

std::valarray<double> fft(float array[], int length) {
    const int fftsize = length;
    fftw_complex* in, * out;
    fftw_plan plan;
    int i;

    in = (fftw_complex*)fftw_malloc(sizeof(fftw_complex) * fftsize);
    out = (fftw_complex*)fftw_malloc(sizeof(fftw_complex) * fftsize);
    plan = fftw_plan_dft_1d(fftsize, in, out, FFTW_FORWARD, FFTW_ESTIMATE);

    for (i = 0; i < fftsize; i++) {
        in[i][0] = array[i] * (0.5 - 0.5 * cos(2 * M_PI * ((float)i / (float)(fftsize - 1))));
        in[i][1] = 0.0;
    }

    fftw_execute(plan); /* repeat as needed */

    fftw_destroy_plan(plan);
    fftw_free(in);

    /* get amplitude */
    std::valarray<double> amplitude(fftsize);
    for (i = 0; i < fftsize; i++) {
        amplitude[i] = out[i][0] * out[i][0] + out[i][1] * out[i][1];
    }
    /* print the result */
    //for(i=0;i<fftsize;i++) printf("amplitude[%d] = %lf\n",i,amplitude[i]);

    fftw_free(out);

    return amplitude;
}

double hz2mel(double f) {
    return 2595 * log(f / 700.0 + 1.0);
}

double mel2hz(double m) {
    return 700 * (exp(m / 2595) - 1.0);
}

std::vector<std::valarray<double>> makeMelFilterBank(int sampleRate, int fft_window, int numChannels) {
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

    std::vector<double> melcenters;
    std::vector<double> fcenters;
    std::vector<int> indexcenter;
    for (int i = 1; i < numChannels + 1; i++) {

        double i_mel = i * dmel;
        double i_hz = mel2hz(i_mel);

        melcenters.push_back(i_mel);
        fcenters.push_back(i_hz);
        indexcenter.push_back((int)( i_hz / df ));
    }

    std::vector<int> indexstart;
    std::vector<int> indexstop;
    for (int i = 0; i < numChannels; i++) {

        int start = (i == 0) ? 0 : indexcenter[((i - 1) + numChannels) % numChannels];
        int end = (i < numChannels - 1) ? indexcenter[ (i + 1) % numChannels ] : nmax;

        indexstart.push_back(start);
        indexstop.push_back(end);
    }

    std::vector<std::valarray<double>> filterbank;
    for (int c = 0; c < numChannels; c++) {
        std::valarray<double> filter(nmax);
        // 三角フィルタの左の直線の傾きから点を求める
        double increment = 1.0 / (double)(indexcenter[c] - indexstart[c]);
        for (int i = indexstart[c]; i < indexcenter[c]; i++) {
            filter[i] = (double)(i - indexstart[c]) * increment;
        }
        // 三角フィルタの右の直線の傾きから点を求める
        double decrement = 1.0 / (double)(indexstop[c] - indexcenter[c]);
        for (int i = indexcenter[c]; i < indexstop[c]; i++) {
            filter[i] = 1.0 - ((double)(i - indexcenter[c]) * decrement);
        }
        filterbank.push_back(filter);
    }

    return filterbank;
}

Spectrogram melSpectrogram(Spectrogram spec, int sampleRate, int channel) {
    auto filterBank = makeMelFilterBank(sampleRate, spec[0].size(), channel);

    Spectrogram melSpectrogram;

    for (int i = 0; i < spec.size(); i++) {
        std::valarray<double> melSpectrum(channel);

        for (int j = 0; j < channel; j++) {
            melSpectrum[j] = (filterBank[j] * spec[i]).sum();
        }

        melSpectrogram.push_back(melSpectrum);
    }

    return melSpectrogram;
}

std::vector<int> peakDetect(std::vector<double> data, int width) {

    if (width > data.size()) return std::vector<int>{0};

    std::vector<double> peak(data.size());

    std::vector<int> peak_point;

    for (int i = 0; i < data.size() - width; i++) {

        peak[i] = data[i];

        /* +++ 現在値よりwitdh前までにpeakがあればthruする +++ */
        double before_sum = 0.0;
        for (int j = 0; j < width; j++) {
            int index = i - (j + 1);
            if (index < 0) continue;
            before_sum += peak[index];
        }

        if ((i >= 1) && (before_sum > 0.01)) {
            peak[i] = 0;
            continue;
        }

        /* +++ 現在値よりwitdh後までの値が全て小さければpeakとみなす +++ */
        for (int w = 1; w < width + 1; w++) {
            if (data[i] <= data[i + w]) {
                peak[i] = 0;
                break;
            }
        }

        if (peak[i] > 0) {
            peak_point.push_back(i);
        }
    }

    return peak_point;
}