#!/bin/bash
# https://google.github.io/styleguide/shellguide.html
# https://github.com/koalaman/shellcheck

source ./build-native-libraries-common.sh $1 $2

function build_library_SDL() {
    local DIRECTORY_SOURCE="$DIRECTORY/SDL"
    local CMAKE_BUILD_FLAGS="
        -D SDL_SHARED=ON
        -D SDL_STATIC=OFF
        -D SDL_TEST_LIBRARY=OFF
        -D SDL_TEST_LIBRARY=OFF
        -D SDL_TESTS=OFF
        -D SDL_DISABLE_INSTALL_DOCS=OFF"

    build_library_cmake "SDL3" "$DIRECTORY_SOURCE" "$CMAKE_BUILD_FLAGS"
}

function cleanup_library_files_SDL() {
    cd "$DIRECTORY_COPY_DESTINATION" || return
    if [[ $RID == 'osx-x64' || $RID == 'osx-arm64' ]]; then
        rm -v libSDL3.dylib
        mv -f -v libSDL3.0.dylib libSDL3.dylib
        install_name_tool -id @rpath/libSDL3.dylib libSDL3.dylib
    fi
    cd "$DIRECTORY" || return
}

build_library_SDL
move_library_files
cleanup_library_files_SDL
