Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports SMRUCC.Rsharp.Runtime.Interop

' General Information about an assembly is controlled through the following
' set of attributes. Change these attribute values to modify the information
' associated with an assembly.

' Review the values of the assembly attributes
#if netcore5=0 then 
<Assembly: AssemblyTitle("VirtualCell toolkit: R# package for GCModeller VirtualCell analysis")>
<Assembly: AssemblyDescription("VirtualCell toolkit: R# package for GCModeller VirtualCell analysis")>
<Assembly: AssemblyCompany("SMRUCC")>
<Assembly: AssemblyProduct("GCModeller")>
<Assembly: AssemblyCopyright("Copyright © xie.guigang@gcmodeller.org 2019")>
<Assembly: AssemblyTrademark("vcellkit")>

<Assembly: ComVisible(False)>

'The following GUID is for the ID of the typelib if this project is exposed to COM
<Assembly: Guid("1b8ff0a3-9f5e-4863-9ae8-16bc3c2bfee1")>

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

<Assembly: AssemblyVersion("1.50.*")>
<Assembly: AssemblyFileVersion("2.3421.*")>

#end if