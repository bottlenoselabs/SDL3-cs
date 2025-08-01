#!/bin/bash
# https://google.github.io/styleguide/shellguide.html
# https://github.com/koalaman/shellcheck

DIRECTORY="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

source $DIRECTORY/build-native-library-SDL.sh $1 $2

function build_library_SDL_image() {
    local DIRECTORY_SOURCE="$DIRECTORY/SDL_image"
    local CMAKE_BUILD_FLAGS="
        -D SDL3_DIR=$LIB_SDL3_DIRECTORY_BUILD
        -D BUILD_SHARED_LIBS=ON
        -D SDLIMAGE_DEPS_SHARED=OFF
        -D SDLIMAGE_VENDORED=ON
        -D SDLIMAGE_STRICT=ON
        -D SDLIMAGE_SAMPLES=OFF
        -D SDLIMAGE_TESTS=OFF
        "

    build_library_cmake "SDL3_image" "$DIRECTORY_SOURCE" "$CMAKE_BUILD_FLAGS"
}

function cleanup_library_files_SDL_image() {
    cd "$DIRECTORY_COPY_DESTINATION" || return
    if [[ $RID == 'osx-x64' || $RID == 'osx-arm64' ]]; then
        rm libSDL3_image.dylib
        rm libSDL3_image.0.dylib
        mv libSDL3_image.*.*.*.dylib libSDL3_image.dylib
        install_name_tool -id @rpath/libSDL3_image.dylib libSDL3_image.dylib
        install_name_tool -change @rpath/libSDL3.0.dylib @rpath/libSDL3.dylib libSDL3_image.dylib
    elif [[ $RID == 'linux-x64' || $RID == 'linux-arm64' ]]; then
        rm libSDL3_image.so
        rm libSDL3_image.so.0
        mv libSDL3_image.so.*.*.* libSDL3_image.so
    fi
    cd "$DIRECTORY" || return
}

build_library_SDL_image
move_library_files
cleanup_library_files_SDL_image
