Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports SMRUCC.Rsharp.Runtime.Interop

' General Information about an assembly is controlled through the following
' set of attributes. Change these attribute values to modify the information
' associated with an assembly.

' Review the values of the assembly attributes

#if netcore5=0 then

<Assembly: AssemblyTitle("biological sequence analysis toolkit")>
<Assembly: AssemblyDescription("motif analysis, similarity search, functional annotations for the biological sequence object.")>
<Assembly: AssemblyCompany("SMRUCC")>
<Assembly: AssemblyProduct("seqtoolkit")>
<Assembly: AssemblyCopyright("Copyright © SMRUCC/GCModeller 2020")>
<Assembly: AssemblyTrademark("")>

<Assembly: ComVisible(False)>

'The following GUID is for the ID of the typelib if this project is exposed to COM
<Assembly: Guid("43b85b7f-ac3b-409d-b91b-25f7217ce849")>

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
