Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.DocumentStream
Imports Microsoft.VisualBasic.Data.csv
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
        Dim out As EntityObject() = genes.log2(designers, label).ToArray
        Dim csv As csv = out.ToCsvDoc
        Return csv
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

        Public Overrides Function ToString() As String
            Return $"{Experiment}/{Control}"
        End Function

        Public Function Log2(gene As EntityObject, Optional label$ = Nothing) As Double
            Dim A#, B#

            If label Is Nothing Then
                A = gene(Experiment).ParseNumeric
                B = gene(Control).ParseNumeric
            Else
                A = gene(label & "." & Experiment).ParseNumeric
                B = gene(label & "." & Control).ParseNumeric
            End If

            Dim out As Double = Math.Log(A / B, 2)
            Return out
        End Function
    End Structure
End Module
