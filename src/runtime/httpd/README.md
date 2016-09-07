# REST_Framework
[The GCModeller open platform online services system](http://services.gcmodeller.org) REST services framework.

This cloud engine power these projects:

+ [Surveillance of infectious diseases](http://120.76.195.65/)

This product includes:

+ ``SMRUCC.WebCloud.HTTPInternal`` - A framework for develop the WebApp
+ ``SMRUCC.WebCloud.GIS`` - GIS database services for build web app that may associate with the Geographic Information likes the population genetics data.

##### Runtime

Require of VisualBasic server CLI runtime

> PM> Install-Package VB_AppFramework

Or reference to source code project:

> https://github.com/xieguigang/VisualBasic_AppFramework


# REST [version 1.0.0.0]
> SMRUCC.REST.CLI

<!--more-->

**REST**
_http://services.gcmodeller.org REST API framework_
Copyright © R&D, SMRUCC 2016. All rights reserved.

**Module AssemblyName**: file:///D:/httpd/httpd.exe
**Root namespace**: ``SMRUCC.REST.CLI``


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|[/GET](#/GET)||
|[/run](#/run)||
|[/start](#/start)||


## CLI API list
--------------------------
<h3 id="/GET"> 1. /GET</h3>


**Prototype**: ``SMRUCC.REST.CLI::Int32 GET(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
httpd /GET /url [<url>/std_in] [/out <file/std_out>]
```
###### Example
```bash
httpd
```
<h3 id="/run"> 2. /run</h3>


**Prototype**: ``SMRUCC.REST.CLI::Int32 RunApp(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
httpd /run /dll <app.dll> [/port <80> /root <wwwroot_DIR>]
```
###### Example
```bash
httpd
```
<h3 id="/start"> 3. /start</h3>


**Prototype**: ``SMRUCC.REST.CLI::Int32 Start(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
httpd /start [/port 80 /root <wwwroot_DIR> /threads -1 /cache]
```
###### Example
```bash
httpd
```
