
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.ComparativeGenomics.OperonMapper
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object

<Package("operon")>
Module operonMapper

    Sub New()
        Call Internal.Object.Converts.addHandler(GetType(OperonRow()), AddressOf operonTable)
    End Sub

    Private Function operonTable(rows As OperonRow(), args As list, env As Environment) As dataframe
        Dim cols As New Dictionary(Of String, Array)

        cols(NameOf(OperonRow.koid)) = rows.Select(Function(a) a.koid).ToArray
        cols(NameOf(OperonRow.name)) = rows.Select(Function(a) a.name).ToArray
        cols(NameOf(OperonRow.org)) = rows.Select(Function(a) a.org).ToArray
        cols(NameOf(OperonRow.op)) = rows.Select(Function(a) a.op.JoinBy(", ")).ToArray
        cols(NameOf(OperonRow.definition)) = rows.Select(Function(a) a.definition).ToArray
        cols(NameOf(OperonRow.source)) = rows.Select(Function(a) a.source).ToArray

        Return New dataframe With {
            .columns = cols,
            .rownames = rows.Keys
        }
    End Function

    <ExportAPI("known_operons")>
    Public Function knownOperons() As OperonRow()
        Return OperonRow.LoadInternalResource.ToArray
    End Function
End Module
