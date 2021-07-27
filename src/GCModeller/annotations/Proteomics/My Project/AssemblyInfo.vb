Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' General Information about an assembly is controlled through the following
' set of attributes. Change these attribute values to modify the information
' associated with an assembly.

' Review the values of the assembly attributes
#if netcore5=0 then
<Assembly: AssemblyTitle("Processing the Label free, iTraq/TMT proteomics fingerprint data")>
<Assembly: AssemblyDescription("Processing the Label free, iTraq/TMT proteomics fingerprint data")>
<Assembly: AssemblyCompany("SMRUCC")>
<Assembly: AssemblyProduct("GCModeller")>
<Assembly: AssemblyCopyright("Copyright © xie.guigang@gcmodeller.org 2017")>
<Assembly: AssemblyTrademark("Proteomics")>

<Assembly: ComVisible(False)>

'The following GUID is for the ID of the typelib if this project is exposed to COM
<Assembly: Guid("6b2e6331-d351-4734-9f16-459a612fd447")>

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

<Assembly: AssemblyVersion("1.0.0.0")>
<Assembly: AssemblyFileVersion("1.0.0.0")>
#end if