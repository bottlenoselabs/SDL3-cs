# SDL3-cs

Manually updated idiomatic C# wrapper with automatic updated C# native bindings for SDL and extensions using the `main` branches for v3.

|Libary|Branch|Link|
|-|-|-|
|`SDL`|`main`|https://github.com/libsdl-org/SDL|
|`SDL_image`|`main`|https://github.com/libsdl-org/SDL_image|
|`SDL_ttf`|`main`|https://github.com/libsdl-org/SDL_ttf|

## Goals

Development is driven primarily for internal use at `bottlenoselabs` with the following goals. Pull requests are welcome as long as they match the following goals of `bottlenoselabs`.

1. Provide a high-level idiomatic C# wrapper API over the native C# bindings API for developer ease of use.
2. Use the permissive `MIT` license where ever possible.
3. Use the latest released .NET version: currently `.NET 9`. Please see [limitations of supported operating systems, versions, and CPU architectures with `.NET 9`](https://github.com/dotnet/core/blob/main/release-notes/9.0/supported-os.md).
    - Support for Windows, macOS, and Ubuntu Linux as first class. They are actively tested *during* development using the *latest* operating system version. This is based on limited in-house physical hardware and [GitHub's Action runner images](https://github.com/actions/runner-images) which are [free for `standard` (latest) images in public repositories](https://docs.github.com/en/billing/managing-billing-for-your-products/managing-billing-for-github-actions/.about-billing-for-github-actions).
    - Support for other systems are tested *when* and *where* hardware and/or development licenses/kits are available by individuals.
        - iOS not yet supported. See https://github.com/bottlenoselabs/SDL3-cs/issues/547.
        - Android not yet supported. See https://github.com/bottlenoselabs/SDL3-cs/issues/548.
        - Browser (WebAssembly) not yet supported. Dependant upon `SDL_GPU` being available. See https://github.com/libsdl-org/SDL/pull/12046.
        - Consoles not yet supported. For primary support on getting `SDL` in C# running on consoles please refer to [`FNA-XNA`](`https://fna-xna.github.io`) and specifically the documentation of [`FNA on consoles`](https://fna-xna.github.io/docs/appendix/Appendix-B%3A-FNA-on-Consoles/#general-advice). If the `FNA-XNA` team helped you in anyway please consider [donating to their cause](https://github.com/sponsors/flibitijibibo).
5. NuGet packages updated monthly or sooner as needed. See https://www.nuget.org/packages?q=bottlenoselabs.SDL for list of packages on NuGet.
6. Semi-automatic continuous delivery. All C functions and types intended for export found in SDL3 are automatically generated using [`c2cs`](https://github.com/bottlenoselabs/c2cs). This happens via GitHub Action workflows in this repository starting from [Dependabot](https://docs.github.com/en/code-security/dependabot/dependabot-version-updates/about-dependabot-version-updates#) to create the pull request daily. Minimal to zero human interaction is the goal for *writing* (generating) the native interopability C# code while human interaction is required for *reviewing* (reading) the code.
    - If you need a specific released version of SDL3, please see https://github.com/bottlenoselabs/SDL3-cs/issues/549.
7. For the C# native bindings, follow P/Invoke [best practices](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/best-practices) including using only blittable types and C# function pointers for callbacks. C# types are 1-1 to C types. This includes naming conventions. This includes enabling and using `unsafe` code in C#. However, in some cases, C# types (e.g. `CBool`, `CString`, `Span<T>`) may be perferred over raw C type equivalents in C# for performance or idiomatic reasons.
8. Runtime marshalling is [disabled](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/disabled-marshalling). C# functions are 1-1 to C functions using [P/Invoke source generation](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke-source-generation). There are no overloads.

These goals might not align to your goals or your organization's goals to which I recommend looking at other similiar bindings for `SDL3` in C#:

- https://github.com/dotnet/Silk.NET
- https://github.com/flibitijibibo/SDL3-CS
- https://github.com/ppy/SDL3-CS
- https://github.com/edwardgushchin/SDL3-CS

## Getting Started

### From NuGet

See https://github.com/bottlenoselabs/template-SDL3-cs for a minimum template repository of how to use `SDL3-cs` with NuGet packages. By using the NuGet packages, you do not need to install C/C++ development tools and instead can use pre-built native libraries.

### From source

1. Download and install [.NET 9](https://dotnet.microsoft.com/download).
2. Clone the repository manually with submodules: `git clone --recurse-submodules https://github.com/bottlenoselabs/SDL3-cs`.
3. Build the native shared libraries (SDL and SDL extensions) by running [`./ext/build-native-libraries.sh`](./ext/build-native-libraries.sh).
    - Windows. To execute `.sh` (Bash) scripts on Windows, use Git Bash which can be [installed with Git](https://git-scm.com/download/win). For CMake on Windows, it's recommended to install through [Visual Studio Installer](https://visualstudio.microsoft.com/downloads/) for the workloads "Desktop development with C++" and "Game development with C++". Additionally on Windows, the [NASM assembler](https://www.nasm.us/) is required and to be in your `PATH` environment variable.
    - macOS. Install XCode through the App Store.
    - Linux. See [required packages](https://wiki.libsdl.org/SDL3/README/linux).
4. Run an example suite suite to test things are working. Use '1' and '2' on your keyboard to move between examples in the suite once run.
   - `SDL_GPU`: `dotnet run --project ./src/cs/examples/Examples.Gpu/Examples.Gpu.csproj`
   - `LazyFoo`: `dotnet run --project ./src/cs/examples/Examples.LazyFoo/Examples.LazyFoo.csproj`
5. Add the following C# project to your solution and reference it in one of your C# project:
    - `./src/cs/production/SDL/SDL.csproj`
    - `./src/cs/production/SDL.Native.*.csproj` (Choose the runtime identifiers that make sense for you.)
    - `./src/cs/production/SDL_image.Native.*.csproj` (Choose the runtime identifiers that make sense for you.)
    - `./src/cs/production/SDL_ttf.Native.*.csproj` (Choose the runtime identifiers that make sense for you.)

## Documentation

### "Safe" API

The higher level API that is more idiomatic to C# which does not require usage of the `unsafe` keyword in C#. Basically, it's a wrapper over the direct native C# bindings. Almost all methods and types have built-in extensive XML comments that serve as documentation through your IDE. For examples of the "Safe" API, see the examples:
    
- [`SDL_GPU` examples](./src/cs/examples/Examples.Gpu/Examples)
- [`LazyFoo` examples](./src/cs/examples/Examples.LazyFoo/Examples)

There are however some decisions made that differ from the native bindings.

1. `Options` objects. These are C# classes that are used to create objects. They are intended to be allocated normally and then disposed using `IDisposable` pattern after the object is created. For example, there are `GpuXYZOptions` to properly fill in the `XYZCreateInfo` structs such as `GpuGraphicsPipelineOptions`, `GpuGraphicsShaderOptions`, `GpuSamplerOptios`, `GpuTextureOptions`, etc.

### "Unsafe" API

The lower level API that matches 1-1 to the C libraries and requires usage of the `unsafe` keyword in C#. For more information on how the native C# bindings work, see [`c2cs`](https://github.com/lithiumtoast/c2cs), the tool that generates the bindings for `SDL` and other C libraries at `bottlenoselabs`.

To learn how to use `SDL` in C, check out the [official documentation](https://wiki.libsdl.org/SDL3) and [Lazy Foo' Production](https://lazyfoo.net/tutorials/SDL).

## License

`SDL-cs` is licensed under the MIT License (`MIT`) - see the [LICENSE file](LICENSE) for details.

### Dependencies

All dependencies are compatible with the MIT License (`MIT`). However, some libraries have licenses that add additional things you must do with your project beyond the normal. These are listed under "Actions" table section.

|Name|License|Link|Actions
|-|-|-|-|
|`SDL`|zlib License|https://github.com/libsdl-org/SDL/blob/main/LICENSE.txt|~|
|`SDL_image`|zlib License|https://github.com/libsdl-org/SDL_image/blob/main/LICENSE.txt|~|
|`aom`|BSD 2-Clause License|https://github.com/libsdl-org/aom/blob/main/LICENSE|~|
|`dav1d`|BSD 2-Clause License|https://github.com/libsdl-org/dav1d/blob/master/COPYING|~|
|`jpeg`|Libjpeg License (Custom BSD-like License)|https://github.com/libsdl-org/jpeg/blob/main/README|~|
|`libavif`|BSD-2-Clause License|https://github.com/libsdl-org/libavif/blob/main/LICENSE|~|
|`libjxl`|BSD 3-Clause License|https://github.com/libsdl-org/libjxl/blob/main/LICENSE|~|
|`libpng`|PNG Reference Library V2 License|https://github.com/libsdl-org/libpng/blob/master/LICENSE|~|
|`libtiff`|LibTIFF License|https://github.com/libsdl-org/libtiff/blob/master/LICENSE.md|~|
|`libwebp`|BSD 3-Clause License|https://github.com/libsdl-org/libwebp/blob/main/COPYING|~|
|`zlib`|zlib License|https://github.com/libsdl-org/zlib/blob/master/LICENSE|~|
|`SDL_ttf`|zlib License|https://github.com/libsdl-org/SDL_ttf/blob/main/LICENSE.txt|~|
|`freetype`|FreeType License|https://github.com/libsdl-org/freetype/blob/master/LICENSE.TXT & https://github.com/libsdl-org/freetype/blob/master/docs/FTL.TXT|Must mention/acknowledge FreeType project in your product's documentation.|
|`hardbuzz`|MIT License (Expat)|https://github.com/libsdl-org/harfbuzz/blob/main/COPYING|~|
|`plutosvg`|MIT License|https://github.com/libsdl-org/plutosvg/blob/master/LICENSE|~|
|`plutosvg`|MIT License|https://github.com/libsdl-org/plutovg/blob/main/LICENSE|~|
