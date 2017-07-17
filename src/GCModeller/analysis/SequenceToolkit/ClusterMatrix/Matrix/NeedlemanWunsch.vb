Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Matrix

    <Extension>
    Public Function NeedlemanWunsch(locis As IEnumerable(Of FastaToken), Optional ByRef out As StreamWriter = Nothing) As DataSet()
        Dim buffer As FastaToken() = locis.ToArray
        Dim LQuery = From fa As SeqValue(Of FastaToken)
                     In buffer _
                         .SeqIterator _
                         .AsParallel
                     Select index = fa.i,
                         vector = (+fa).__needlemanWunsch(buffer)
                     Order By index Ascending
        Dim tuples = LQuery.Select(Function(v) v.vector).ToArray
        Dim result As DataSet() = tuples _
            .Select(Function(d) d.vector) _
            .ToArray

        If Not out Is Nothing Then
            For Each query In tuples
                Dim block As String = Encoding.ASCII.GetString(
                    query.log _
                    .BaseStream _
                    .As(Of MemoryStream) _
                    .ToArray)

                Call out.WriteLine(query.title)
                Call out.WriteLine(block)
                Call out.WriteLine("---------------------------------------------------------------------")
                Call out.WriteLine()
            Next
        End If

        Return result
    End Function

    <Extension>
    Private Function __needlemanWunsch(query As FastaToken, buffer As FastaToken()) As (vector As DataSet, log As StreamWriter, title$)
        Dim dev As StreamWriter = App.NullDevice(Encodings.ASCII)
        Dim score#
        Dim vector As New Dictionary(Of String, Double)

        For Each target As FastaToken In buffer
            score = RunNeedlemanWunsch.RunAlign(query, target, True, dev, echo:=False)
            dev.WriteLine()
            vector.Add(target.Title, score)
        Next

        Dim buf As Byte() = dev _
            .BaseStream _
            .As(Of MemoryStream) _
            .ToArray
        Dim block$ = dev.Encoding _
            .GetString(buf)

        Call query.Title.__DEBUG_ECHO

        Return (vector:=New DataSet With {
            .ID = query.Title,
            .Properties = vector
        }, Log:=dev,
        title:=query.Title)
    End Function
End Module
