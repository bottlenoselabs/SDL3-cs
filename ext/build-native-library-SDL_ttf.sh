#!/bin/bash
# https://google.github.io/styleguide/shellguide.html
# https://github.com/koalaman/shellcheck

source ./build-native-library-SDL.sh $1 $2

function build_library_SDL_ttf() {
    local DIRECTORY_SOURCE="$DIRECTORY/SDL_ttf"
    local CMAKE_BUILD_FLAGS="
        -D SDL3_DIR=$LIB_SDL3_DIRECTORY_BUILD
        -D BUILD_SHARED_LIBS=ON
        -D SDLTTF_BUILD_SHARED_LIBS=OFF
        -D SDLTTF_VENDORED=ON
        -D SDLTTF_STRICT=ON
        -D SDLTTF_SAMPLES=OFF
        -D SDLTTF_TESTS=OFF
        "

    build_library_cmake "SDL3_ttf" "$DIRECTORY_SOURCE" "$CMAKE_BUILD_FLAGS"
}

function cleanup_library_files_SDL_ttf() {
    cd "$DIRECTORY_COPY_DESTINATION" || return
    if [[ $RID == 'osx-x64' || $RID == 'osx-arm64' ]]; then
        rm libSDL3_ttf.dylib
        rm libSDL3_ttf.0.dylib
        mv libSDL3_ttf.*.*.*.dylib libSDL3_ttf.dylib
        install_name_tool -id @rpath/libSDL3_ttf.dylib libSDL3_ttf.dylib
        install_name_tool -change @rpath/libSDL3.0.dylib @rpath/libSDL3.dylib libSDL3_ttf.dylib
    fi
    cd "$DIRECTORY" || return
}

build_library_SDL_ttf
move_library_files
cleanup_library_files_SDL_ttf
