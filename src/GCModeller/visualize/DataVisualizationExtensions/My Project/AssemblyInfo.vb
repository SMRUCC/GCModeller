Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' General Information about an assembly is controlled through the following 
' set of attributes. Change these attribute values to modify the information
' associated with an assembly.

' Review the values of the assembly attributes
#If netcore5 = 0 Then
<Assembly: AssemblyTitle("GCModeller base module for bioinformatics Data Visualization")>
<Assembly: AssemblyDescription("GCModeller base module for bioinformatics Data Visualization")>
<Assembly: AssemblyCompany("SMRUCC")>
<Assembly: AssemblyProduct("DataVisualization")>
<Assembly: AssemblyCopyright("Copyright © xie.guigang@gcmodeller.org 2019")>
<Assembly: AssemblyTrademark("GCModeller")>

<Assembly: ComVisible(False)>

'The following GUID is for the ID of the typelib if this project is exposed to COM
<Assembly: Guid("fcb63ef5-c905-4632-8273-10290f94f796")> 

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