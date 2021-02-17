# Spark3D

Spark3D is a 3D multimedia engine written in C# using DirectX11 as its rendering API. The engine is split across
several DLLs which feature:
- Content loading, importing, and exporting functionality
- DirectX11 rendering support
- Fully featured render engine comprising of support for lighting, materials, cameras, and full resource management
- A DirectX11 .fx file compiler
- Basic entity/component/framework support
- Simple UI rendering
- Platform specific input and windowing system handling (currently only supporting Win32)
This library has quickly grown and has gotten to the point where it is realistically too large of a project for me to continue,
however it was interesting to design and implement this kind of functionality, originally starting as a re-implementation of the
XNA/MonoGame runtimes I began making changes to support more modern rendering features, such as multi-threaded rendering, multi-dimensional
texture arrays, and better render state management.

![Screenshot](https://github.com/Mattiemus/Spark/blob/master/screenshot.png?raw=true)
