Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' General Information about an assembly is controlled through the following
' set of attributes. Change these attribute values to modify the information
' associated with an assembly.

' Review the values of the assembly attributes

#if netcore5=0 then 

<Assembly: AssemblyTitle("Flute http web server")>
<Assembly: AssemblyDescription("Flute http web server")>
<Assembly: AssemblyCompany("https://biocad.cloud")>
<Assembly: AssemblyProduct("Flute")>
<Assembly: AssemblyCopyright("Copyright © i@xieguigang.me 2020")>
<Assembly: AssemblyTrademark("")>

<Assembly: ComVisible(False)>

'The following GUID is for the ID of the typelib if this project is exposed to COM
<Assembly: Guid("cf23784d-71ca-4a3d-9b90-82bae3dd9f0d")>

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