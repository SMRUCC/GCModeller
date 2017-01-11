Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.DocumentStream
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Mathematical.Statistics.Hypothesis
Imports Microsoft.VisualBasic.Scripting
Imports csv = Microsoft.VisualBasic.Data.csv.DocumentStream.File

Public Module DEGDesigner

    <Extension>
    Public Iterator Function log2(data As IEnumerable(Of EntityObject), designers As Designer(), Optional label$ = Nothing) As IEnumerable(Of EntityObject)
        For Each gene As EntityObject In data
            For Each designer In designers
                Dim tag$ = $"log2({designer.ToString})"
                gene.Properties(tag) = designer.Log2(gene, label)
            Next

            Yield gene
        Next
    End Function

    Public Function log2(path$, designers As Designer(), Optional label$ = Nothing) As csv
        Dim genes As EntityObject() = EntityObject.LoadDataSet(path).ToArray
        Dim groups As Dictionary(Of String, Designer()) = designers _
            .GroupBy(Function(x) x.GroupLabel) _
            .ToDictionary(Function(k) k.Key,
                          Function(repeats) repeats.ToArray)
        Dim out As EntityObject() = genes _
            .log2(designers, label) _
            .StudentTtest(groups, label) _
            .ToArray
        Dim csv As csv = out.ToCsvDoc
        Return csv
    End Function

    <Extension>
    Public Iterator Function StudentTtest(data As IEnumerable(Of EntityObject), designers As Dictionary(Of String, Designer()), Optional label$ = Nothing) As IEnumerable(Of EntityObject)
        For Each gene As EntityObject In data
            For Each group In designers
                Dim a#() = New Double(group.Value.Length - 1) {}
                Dim b#() = New Double(group.Value.Length - 1) {}

                For i As Integer = 0 To a.Length - 1
                    Dim l = group.Value(i)
                    Dim tag = l.GetLabel(label)

                    a(i) = Val(gene(tag.exp))
                    b(i) = Val(gene(tag.control))
                Next

                Dim result As TwoSampleResult = t.Test(a, b)

                gene($"{group.Key}.P-value") = result.Pvalue
            Next

            Yield gene
        Next
    End Function

    Public Structure Designer

        ''' <summary>
        ''' 分子
        ''' </summary>
        Dim Experiment$
        ''' <summary>
        ''' 分母
        ''' </summary>
        Dim Control$

        ''' <summary>
        ''' 具有相同的这个属性的标签值的都是生物学重复
        ''' </summary>
        Dim GroupLabel$

        Public Overrides Function ToString() As String
            Return $"{Experiment}/{Control}"
        End Function

        Public Function GetLabel(Optional label$ = Nothing) As (exp$, control$)
            If label Is Nothing Then
                Return (Experiment, Control)
            Else
                Return (label & "." & Experiment, label & "." & Control)
            End If
        End Function

        Public Function Log2(gene As EntityObject, Optional label$ = Nothing) As Double
            Dim tag As (exp$, Control As String) = GetLabel(label)
            Dim A# = gene(tag.exp).ParseNumeric
            Dim B# = gene(tag.Control).ParseNumeric
            Dim out As Double = Math.Log(A / B, 2)
            Return out
        End Function
    End Structure
End Module
