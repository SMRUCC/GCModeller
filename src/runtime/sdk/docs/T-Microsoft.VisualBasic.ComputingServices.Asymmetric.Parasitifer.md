---
title: Parasitifer
---

# Parasitifer
_namespace: [Microsoft.VisualBasic.ComputingServices.Asymmetric](N-Microsoft.VisualBasic.ComputingServices.Asymmetric.html)_

主节点下面的每一台物理机上面的宿主服务，提供给该物理机上面的服务实例



### Methods

#### #ctor
```csharp
Microsoft.VisualBasic.ComputingServices.Asymmetric.Parasitifer.#ctor(System.String,System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|Instance|全路径|
|Master|主节点的IP公网地址|
|PublicToken|-|


#### __getIPAddress
```csharp
Microsoft.VisualBasic.ComputingServices.Asymmetric.Parasitifer.__getIPAddress
```
获取这台主机的公网IP地址

#### SystemLoad
```csharp
Microsoft.VisualBasic.ComputingServices.Asymmetric.Parasitifer.SystemLoad
```
获取当前物理主机上面的系统负载


### Properties

#### _instance
服务实例的文件名
#### _instanceList
在当前的物理主机上面所运行的服务实例列表
#### _invokeAuthnic
当前的这个管理节点和其所管理的服务实例之间相互通信所需要的身份认证信息
#### _OAuth
这个节点在主节点上面的授权认证信息
