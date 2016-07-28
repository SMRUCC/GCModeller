---
title: Master
---

# Master
_namespace: [Microsoft.VisualBasic.ComputingServices.Asymmetric](N-Microsoft.VisualBasic.ComputingServices.Asymmetric.html)_

非对等网络里面的中心主节点



### Methods

#### #ctor
```csharp
Microsoft.VisualBasic.ComputingServices.Asymmetric.Master.#ctor(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|PublicToken|计算自于宿主节点的证书哈希值|


#### __getLoad
```csharp
Microsoft.VisualBasic.ComputingServices.Asymmetric.Master.__getLoad(System.String,Microsoft.VisualBasic.Net.SSL.Certificate)
```


|Parameter Name|Remarks|
|--------------|-------|
|Node|IP地址|


#### __getPreferNode
```csharp
Microsoft.VisualBasic.ComputingServices.Asymmetric.Master.__getPreferNode
```
获取得到最优先的物理机来开启新的计算节点，优先级别是和分布式计算网络之中的物理主机节点的CPU，内存负载量成反比的(本机的节点优先级别最低)

#### Folk
```csharp
Microsoft.VisualBasic.ComputingServices.Asymmetric.Master.Folk(System.String)
```
创建出一个新的计算节点里面的服务实例

|Parameter Name|Remarks|
|--------------|-------|
|CLI|-|


#### GetInstances
```csharp
Microsoft.VisualBasic.ComputingServices.Asymmetric.Master.GetInstances
```
获取当前网络之中的所有的已经运行的计算服务实例

#### NodeRegister
```csharp
Microsoft.VisualBasic.ComputingServices.Asymmetric.Master.NodeRegister(System.Int64,Microsoft.VisualBasic.Net.Protocols.RequestStream,System.Net.IPEndPoint)
```


|Parameter Name|Remarks|
|--------------|-------|
|CA|-|
|request|-|
|remote|可能得到的是内网IP，所以不太准确，不是用这个参数来标记节点的位置|



### Properties

#### _nodes
键名是IP地址，由于一台物理主机上面只会有一个管理节点，所以端口号都是固定了的
#### Nodes
获取在当前的服务器上面注册了的所有的管理节点的位置
