# SekaiLink：又一个耳机管理平台

SekaiLink 是一个耳机管理平台，旨在适配多个耳机品牌，并支持不同操作系统。为实现这一点，我们实现了抽象层 `SekaiLink.Protocols`，以提供可扩展的管理接口。

架构组织如下：

- `SekaiLink.Protocol.BrandName`：各品牌的基础协议应在此实现。
- `SekaiLink.Device.BrandName`：设备特定功能存放于此。
- `SekaiLink.Native.Platform`：平台特定方法（如 Windows Runtime API）在此实现。
- `SekaiLink.ShellType`：所有外壳界面（如 WinUI 3、Console）在此实现。

我们还参考了 [Zhaoyi-ya/OppoPodsManager](https://github.com/Zhaoyi-ya/OppoPodsManager) 的代码，并感谢他们的出色工作。