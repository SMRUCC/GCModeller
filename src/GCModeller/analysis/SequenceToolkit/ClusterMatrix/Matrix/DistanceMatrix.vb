Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module DistanceMatrix

    <Extension>
    Public Function NeedlemanWunsch(locis As IEnumerable(Of FastaToken), Optional ByRef out As StreamWriter = Nothing) As DataSet()
        Dim dev As StreamWriter = out

        If dev Is Nothing Then
            dev = App.NullDevice
            out = dev
        End If

        Dim buffer As FastaToken() = locis.ToArray
        Dim LQuery = From fa As SeqValue(Of FastaToken)
                     In buffer _
                         .SeqIterator _
                         .AsParallel
                     Select index = fa.i,
                         vector = (+fa).__needlemanWunsch(buffer, dev)
                     Order By index Ascending
        Dim result As DataSet() = LQuery _
            .Select(Function(d) d.vector) _
            .ToArray

        Return result
    End Function

    <Extension>
    Private Function __needlemanWunsch(query As FastaToken, buffer As FastaToken(), out As StreamWriter) As DataSet
        Dim dev As StreamWriter = App.NullDevice(GetEncodings(out.Encoding))
        Dim score#
        Dim vector As New Dictionary(Of String, Double)

        For Each target As FastaToken In buffer
            score = RunNeedlemanWunsch.RunAlign(query, target, True, dev)
            dev.WriteLine()
            vector.Add(target.Title, score)
        Next

        SyncLock out

            Dim buf As Byte() = dev _
                .BaseStream _
                .As(Of MemoryStream) _
                .ToArray
            Dim block$ = dev.Encoding _
                .GetString(buf)

            Call out.WriteLine(query.Title)
            Call out.WriteLine(block)
            Call out.WriteLine()
        End SyncLock

        Return New DataSet With {
            .ID = query.Title,
            .Properties = vector
        }
    End Function
End Module
