
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.Data.GeneOntology.Annotation
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("go.annotation")>
Module annotation

    <ExportAPI("uniprot.ko2go")>
    <RApiReturn(GetType(SecondaryIDSolver))>
    Public Function CreateKO2GO(uniprot As pipeline, Optional env As Environment = Nothing) As Object
        If uniprot Is Nothing Then
            Return debug.stop("the uniprot annotation source can not be nothing!", env)
        ElseIf Not uniprot.elementType Like GetType(entry) Then
            Return debug.stop({
                 $"invalid element type for input data!",
                 $"required: " & GetType(entry).FullName,
                 $"but given: " & uniprot.elementType.ToString
            }, env)
        End If

        Dim idmaps = uniprot.populates(Of entry) _
            .PopulateMappings _
            .GroupBy(Function(a) a.KO)
        Dim mapper As SecondaryIDSolver = SecondaryIDSolver.Create(
            source:=idmaps,
            mainID:=Function(a) a.Key,
            secondaryID:=Function(a)
                             Return a.Select(Function(m) m.GO).mapTop
                         End Function,
            skip2ndMaps:=True
        )

        Return mapper
    End Function

    <Extension>
    Private Function mapTop(groups As IEnumerable(Of String())) As String()
        Dim allMatrix As String()() = groups.ToArray
        Dim counts = From go_id As String
                     In allMatrix.IteratesALL
                     Group By go_id
                     Into Count

        Return counts _
            .Where(Function(a) (a.Count / allMatrix.Length) >= 0.65) _
            .Select(Function(a) a.go_id) _
            .ToArray
    End Function
End Module
