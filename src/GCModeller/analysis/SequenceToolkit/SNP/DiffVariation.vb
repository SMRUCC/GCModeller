Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.DocumentStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module DiffVariation

    ''' <summary>
    ''' 必须是经过对齐了的，第一条序列为参考序列
    ''' </summary>
    ''' <param name="aln"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Iterator Function GetSeqs(aln As IEnumerable(Of FastaToken)) As IEnumerable(Of KSeq)
        Dim ref As FastaToken = aln.First
        Dim refs As Char() = ref.SequenceData.ToUpper.ToCharArray

        For Each seq As FastaToken In aln.Skip(1)
            Dim nts As Char() = seq.SequenceData.ToUpper.ToCharArray
            Dim diffs As New List(Of SeqValue(Of NamedValue(Of Integer)))
            Dim b As Integer

            For Each i As SeqValue(Of Char) In refs.SeqIterator
                If i.obj = "-"c Then
                    b = 0
                Else
                    If nts(i.i) = "-"c OrElse i.obj = nts(i.i) Then
                        b = 0
                    Else
                        b = 1
                    End If
                End If

                diffs += New SeqValue(Of NamedValue(Of Integer)) With {
                    .i = i.i,
                    .obj = New NamedValue(Of Integer) With {
                        .Name = nts(i.i).ToString,
                        .x = b
                    }
                }
            Next

            Dim x As New KSeq With {
                .attrs = seq.Attributes,
                .Diffs = diffs.ToDictionary(Function(o) o.i,
                                            Function(o) o.obj)
            }
            x.Date.Value = Regex.Match(seq.Attributes.Last, "\d{6}").Value

            Yield x
        Next
    End Function

    <Extension>
    Public Function GroupByDate(source As IEnumerable(Of KSeq)) As DataSet()
        Dim LGroup = From x As KSeq
                     In source
                     Select x
                     Group x By x.Date.Value Into Group
                     Order By Value Ascending

        Dim bufs = LGroup.ToArray
        Dim out As New List(Of DataSet)
        Dim aLen As Integer =
            bufs.First.Group.First.Diffs.Count

        For Each g In bufs   ' 都是同一个年/月的
            Dim tag As String = g.Value
            Dim ar As KSeq() = g.Group.ToArray
            Dim hash As New Dictionary(Of String, Double)

            For i As Integer = 0 To aLen - 1
                Dim ind As Integer = i
                Dim d As Integer = (From x As KSeq
                                    In ar
                                    Let b = x.Diffs(ind).x
                                    Where b <> 0
                                    Select 1).Count

                hash.Add((i + 1).ToString, d / ar.Length)
            Next

            out += New DataSet With {
                .Identifier = tag,
                .Properties = hash
            }

            Call tag.__DEBUG_ECHO
        Next

        Return out
    End Function

    <Extension>
    Public Function [Date](x As KSeq) As PropertyValue(Of String)
        Return PropertyValue(Of String).Read(Of KSeq)(x, NameOf([Date]))
    End Function
End Module

Public Class KSeq : Inherits ClassObject

    Public Property attrs As String()
    Public Property Diffs As Dictionary(Of Integer, NamedValue(Of Integer))

End Class