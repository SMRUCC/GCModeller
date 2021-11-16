Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' General Information about an assembly is controlled through the following
' set of attributes. Change these attribute values to modify the information
' associated with an assembly.

' Review the values of the assembly attributes
#If netcore5 = 0 Then
<Assembly: AssemblyTitle("GSEA analysis library")>
<Assembly: AssemblyDescription("GSEA analysis library")>
<Assembly: AssemblyCompany("SMRUCC")>
<Assembly: AssemblyProduct("GCModeller")>
<Assembly: AssemblyCopyright("Copyright © http://gcmodeller.org 2019")>
<Assembly: AssemblyTrademark("Profiler")>

<Assembly: ComVisible(False)>

'The following GUID is for the ID of the typelib if this project is exposed to COM
<Assembly: Guid("3dc81e42-e288-444a-940a-673ddf1c1349")>

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

<Assembly: AssemblyVersion("2.10.*")>
<Assembly: AssemblyFileVersion("1.99.*")>
#end if