
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("parser")>
Public Module parser

    ''' <summary>
    ''' Request kegg map data model
    ''' </summary>
    ''' <param name="html"></param>
    ''' <param name="url">The original source url of this map data</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("kegg_map")>
    <RApiReturn(GetType(Map))>
    Public Function ParseHTML(url As String,
                              Optional fs As IFileSystemEnvironment = Nothing,
                              Optional env As Environment = Nothing) As Object

    End Function
End Module
