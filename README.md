# UwU.Communicate

**A lightweight, high-performance WebSocket library for .NET — supports typed messaging, Snappier compression, and zero-allocation design using Span\<T\>.**

---

## ✨ Overview

`UwU.Communicate` is a fast and efficient WebSocket library built for modern .NET applications. It enables:

- Strongly-typed message sending and receiving.
- Automatic compression and decompression using [Snappier](https://github.com/google/snappy).
- Low memory allocations thanks to `Span<T>` and memory-efficient data handling.

---

## 🚀 Key Features

- **Typed Message Support**  
  Send and receive strongly-typed objects with no need to manually handle raw byte arrays or JSON parsing.

- **High-Speed Compression with Snappier**  
  Efficient packet compression and decompression reduce bandwidth usage without compromising speed.

- **Allocation-Free Design**  
  Leverages `Span<T>` to minimize heap allocations and reduce GC pressure.

- **Flexible Integration**  
  Easily integrates with Windows Form, ASP.NET Core, console applications, or any .NET-based server or client.
  Currently not support Unity 3D because this library is based on .NET 9. (Unity3D still using old .NET 4.6 and performance not good)
---

## 🔧 Installation

The package is not published to NuGet yet. You can clone the repository and reference it locally:

```bash
git clone https://github.com/sandichhuu/UwU.Communicate.git
