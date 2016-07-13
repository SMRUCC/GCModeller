---
title: Session
---

# Session
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.Settings](N-SMRUCC.genomics.Analysis.AnnotationTools.Settings.html)_

GCModeller program profile session.(GCModeller的应用程序配置会话)



### Methods

#### Finallize
```csharp
SMRUCC.genomics.Analysis.AnnotationTools.Settings.Session.Finallize
```
Close the application session and save the settings file.

#### FolkShoalThread
```csharp
SMRUCC.genomics.Analysis.AnnotationTools.Settings.Session.FolkShoalThread(System.String,System.String)
```
For unawareless of overrides the original data file, this function will automatcly add a .std_out extension to the STDOUT filepath.
 (新构建一个Shoal实例运行一个分支脚本任务，在.NET之中线程的效率要比进程的效率要低，使用这种多线程的方法来实现并行的效率要高很多？？？？)

|Parameter Name|Remarks|
|--------------|-------|
|Script|脚本文件的文件文本内容|
|STDOUT|Standard output redirect to this file. Facility the GCModeller debugging.|


#### Initialize
```csharp
SMRUCC.genomics.Analysis.AnnotationTools.Settings.Session.Initialize(System.Type)
```
Initialize the application session and get the program profile data.(初始化应用程序会话，并获取应用程序的配置数据)

#### TryGetShoalShellBin
```csharp
SMRUCC.genomics.Analysis.AnnotationTools.Settings.Session.TryGetShoalShellBin
```
首先尝试通过配置文件得到脚本环境，假若没有配置这个值，则会尝试通过本身程序来测试，因为这个函数的调用可能是来自于Shoal脚本的


### Properties

#### _LogDir
Directory for stores the application log file.(存放应用程序的日志文件的文件系统目录)
#### DataCache
The cache data directory for this application session.(本应用程序的数据缓存文件夹)
#### Initialized
This property indicates that whether this application session is initialized or not.(应用程序是否已经初始化完毕)
#### LogDIR
Directory for stores the application log file.(存放应用程序的日志文件的文件系统目录)
#### SettingsDIR
Data storage directory for the program settings.(配置文件所存放的文件夹)
#### TEMP
Temp data directory for this application session.(本应用程序会话的临时数据文件夹)
#### Templates
Templates directory of the GCModeller inputs data.
 (在这个文件夹里面存放的是一些分析工具的输入的数据的模板文件)
