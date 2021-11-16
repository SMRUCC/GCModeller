Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' 有关程序集的常规信息通过下列特性集
' 控制。更改这些特性值可修改
' 与程序集关联的信息。

' 查看程序集特性的值
#if netcore5=0 then 
<Assembly: AssemblyTitle("Cytoscape interaction interface")>
<Assembly: AssemblyDescription("Cytoscape interaction interface")>
<Assembly: AssemblyCompany("GCModeller")>
<Assembly: AssemblyProduct("Cytoscape")>
<Assembly: AssemblyCopyright("Copyright © SMRUCC.org 2014")>
<Assembly: AssemblyTrademark("GCModeller")>

<Assembly: ComVisible(False)>

'如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
<Assembly: Guid("59997b26-8c3c-4350-a4b2-b274becd1e8a")> 

' 程序集的版本信息由下面四个值组成: 
'
'      主版本
'      次版本
'      生成号
'      修订号
'
' 可以指定所有这些值，也可以使用“生成号”和“修订号”的默认值，
' 方法是按如下所示使用“*”: 
' <Assembly: AssemblyVersion("1.0.*")> 

<Assembly: AssemblyVersion("1.0.0.0")> 
<Assembly: AssemblyFileVersion("1.0.0.0")> 
#end if