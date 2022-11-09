Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Rhea
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports sbXML = SMRUCC.genomics.Model.SBML.Level3.XmlFile(Of SMRUCC.genomics.Data.SABIORK.SBML.SBMLReaction)

''' <summary>
''' enzymatic reaction network modeller
''' </summary>
<Package("enzymatic")>
Module Enzymatic

    <ExportAPI("query_reaction")>
    Public Function QueryReaction(reaction As Rhea.Reaction, Optional cache As String = "./.cache/") As Object
        Dim list As New Dictionary(Of String, sbXML)

        For Each id As String In reaction.enzyme.SafeQuery
            list.Add(id, SABIORK.WebRequest.QueryByECNumber(id, cache))
        Next

        Return list
    End Function

    <ExportAPI("read.rhea")>
    Public Function ParseRhea(file As String) As Object
        Return TextParser.ParseReactions(file.ReadAllLines).DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    <ExportAPI("open.rhea")>
    Public Function openRheaQuery(repo As String) As Object
        Return New RheaNetworkReader(repo.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
    End Function

    <ExportAPI("imports_rhea")>
    Public Function ImportsRhea(<RRawVectorArgument>
                                rhea As Object,
                                repo As String,
                                Optional env As Environment = Nothing) As Object

        Dim reactions = pipeline.TryCreatePipeline(Of Rhea.Reaction)(rhea, env)

        If reactions.isError Then
            Return reactions.getError
        End If

        Using file As Stream = New FileStream(repo, FileMode.OpenOrCreate)
            Dim writer As New RheaNetworkWriter(New StreamPack(file, meta_size:=32 * 1024 * 1024))

            For Each reaction As Rhea.Reaction In reactions.populates(Of Rhea.Reaction)(env)
                Call writer.AddReaction(reaction)
            Next

            Call writer.Dispose()
        End Using

        Return Nothing
    End Function
End Module
