
Imports kegg_api.Html
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("parser")>
Public Module parser

    ''' <summary>
    ''' Request kegg map data model
    ''' </summary>
    ''' <param name="fs">
    ''' the cache filesystem, could be a local directory or a
    ''' filesystem object that implements the interface 
    ''' <see cref="IFileSystemEnvironment"/>.
    ''' </param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("kegg_map")>
    <RApiReturn(GetType(Map))>
    Public Function ParseHTML(id As String,
                              Optional fs As Object = "./.cache/",
                              Optional env As Environment = Nothing) As Object

        Static cache As New Dictionary(Of UInteger, WebQuery)

        Dim web As WebQuery

        If fs Is Nothing Then
            Return Internal.debug.stop("the cache filesystem could not be nothing!", env)
        ElseIf TypeOf fs Is String Then
            web = New WebQuery(CLRVector.asCharacter(fs).First)
        ElseIf fs.GetType.ImplementInterface(Of IFileSystemEnvironment) Then
            web = cache.ComputeIfAbsent(
                key:=CUInt(fs.GetHashCode),
                lazyValue:=Function()
                               Return New WebQuery(DirectCast(fs, IFileSystemEnvironment))
                           End Function)
        Else
            Return Message.InCompatibleType(GetType(IFileSystemEnvironment), fs.GetType, env)
        End If

        Dim ref As New Pathway With {.entry = New NamedValue(id, $"https://www.kegg.jp/pathway/{id}")}
        Dim map As Map = web.Query(Of Map)(ref, cacheType:=".html")

        Return map
    End Function
End Module
