#!/bin/bash
# https://google.github.io/styleguide/shellguide.html
# https://github.com/koalaman/shellcheck

# NOTE: It's not ideal, but the SDL base library is built several times.

source ./build-native-library-SDL.sh $1 $2
source ./build-native-library-SDL_image.sh $1 $2
source ./build-native-library-SDL_ttf.sh $1 $2
