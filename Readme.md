# SekaiLink: Yet Another Headphone Management Platform

SekaiLink is a headphone management platform designed to support multiple headphone brands across different operating systems. To achieve this, an abstraction layer, `SekaiLink.Protocols`, has been implemented to provide an extensible management interface.

The architecture is organized as follows:

- `SekaiLink.Protocol.BrandName`: basic protocols for each brand should be implemented here.
- `SekaiLink.Device.BrandName`: device-specific features are stored here.
- `SekaiLink.Native.Platform`: platform-specific methods (e.g., Windows Runtime APIs) are implemented here.
- `SekaiLink.ShellType`: all shells (e.g., WinUI 3, Console) are implemented here.

We also referenced code from [Zhaoyi-ya/OppoPodsManager](https://github.com/Zhaoyi-ya/OppoPodsManager). We appreciate their excellent work.