#pragma once

#include "od_def.h"

std::valarray<double> fft(float array[], int length);

std::vector<double> spectroFlux(std::vector<std::valarray<double>> spectrogram);

double hz2mel(double f);

double mel2hz(double m);

std::vector<std::valarray<double>> makeMelFilterBank(int sampleRate, int fft_window, int numChannels);

Spectrogram melSpectrogram(Spectrogram spec, int sampleRate, int channel=20);

std::vector<int> peakDetect(std::vector<double> data, int width);