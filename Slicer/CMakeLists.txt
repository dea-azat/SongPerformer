cmake_minimum_required(VERSION 2.8)

project(plt)

set(CMAKE_CXX_STANDARD 14) # C++14...
set(CMAKE_CXX_STANDARD_REQUIRED ON) #...is required...
set(CMAKE_CXX_EXTENSIONS OFF) #...without compiler extensions like gnu++11
set(CMAKE_CXX_FLAGS "-g -pthread")

set(CMAKE_C_FLAGS "-g")

file(GLOB MY_SOURCE 

./*.cpp

)

add_executable(plt
./test/test.cpp
${MY_SOURCE}
)

include_directories(Main
./test/
./test/AudioFile/
/Users/waku/Library/
)

find_package(Python3 COMPONENTS Development NumPy)
target_include_directories(plt PRIVATE ${Python3_INCLUDE_DIRS} ${Python_NumPy_INCLUDE_DIRS})

list(APPEND CMAKE_MODULE_PATH "${CMAKE_CURRENT_SOURCE_DIR}/cmake")
find_package(FFTW REQUIRED)

target_link_libraries(plt Python3::Python Python3::NumPy FFTW::FFTW)
