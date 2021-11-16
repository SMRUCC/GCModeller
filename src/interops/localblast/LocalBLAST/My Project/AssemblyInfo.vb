Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' 有关程序集的常规信息通过下列特性集
' 控制。更改这些特性值可修改
' 与程序集关联的信息。

' 查看程序集特性的值
#if netcore5=0 then 
<Assembly: AssemblyTitle("CLI Interop service & extensions for NCBI BLAST program")> 
<Assembly: AssemblyDescription("NCBI BLAST program extensions")> 
<Assembly: AssemblyCompany("蓝思生物信息工程师工作站")> 
<Assembly: AssemblyProduct("NCBI BLAST")>
<Assembly: AssemblyCopyright("Copyright © 中国南方微生物资源利用中心(SMRUCC) 2015")>
<Assembly: AssemblyTrademark("GCModeller")> 

<Assembly: ComVisible(False)>

'如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
<Assembly: Guid("7b552c81-3522-4e4e-bd1f-cc5622943a4c")> 

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

<Assembly: AssemblyVersion("2.332.*")> 
<Assembly: AssemblyFileVersion("1.986.*")> 
#end if