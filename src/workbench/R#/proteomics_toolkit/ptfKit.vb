Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime.Internal.Object

''' <summary>
''' toolkit for handle ptf annotation data set
''' </summary>
''' 
<Package("ptfKit")>
Module ptfKit

    <ExportAPI("load.ptf")>
    Public Function loadPtf(file As Object) As pipeline

    End Function
End Module
