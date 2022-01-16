Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' General Information about an assembly is controlled through the following
' set of attributes. Change these attribute values to modify the information
' associated with an assembly.

' Review the values of the assembly attributes
#if netcore5=0 then 
<Assembly: AssemblyTitle("GCModeller VirtualCell Dynamics")>
<Assembly: AssemblyDescription("GCModeller VirtualCell Dynamics")>
<Assembly: AssemblyCompany("SMRUCC")>
<Assembly: AssemblyProduct("GCModeller")>
<Assembly: AssemblyCopyright("Copyright © xie.guigang@gcmodeller.org 2019")>
<Assembly: AssemblyTrademark("Dynamics")>

<Assembly: ComVisible(False)>

'The following GUID is for the ID of the typelib if this project is exposed to COM
<Assembly: Guid("c67708e1-f8fa-4120-abbb-f169a9fc090b")>

' Version information for an assembly consists of the following four values:
'
'      Major Version
'      Minor Version
'      Build Number
'      Revision
'
' You can specify all the values or you can default the Build and Revision Numbers
' by using the '*' as shown below:
' <Assembly: AssemblyVersion("1.0.*")>

<Assembly: AssemblyVersion("2.711.*")>
<Assembly: AssemblyFileVersion("1.234.*")>
#end if