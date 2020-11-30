#pragma once

#include <cstdint>

#define DllExport   __declspec( dllexport )

extern "C"
{
    DllExport void Test();

    DllExport void OnsetDetect(float* data, uint64_t dataLength, uint64_t sampleRate, double* onset, int onsetLength);
}