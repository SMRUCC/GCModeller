Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Public Module SampleColorBend

    ''' <summary>
    ''' 仅对行进行计算
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function GetColors(matrix As Matrix, Optional colorSet$ = "RdYlGn:c8", Optional levels As Integer = 25) As IEnumerable(Of NamedCollection(Of Color))
        Dim colors As Color() = Designer.GetColors(colorSet, n:=levels)
        Dim indexRange As DoubleRange = {0, colors.Length - 1}

        For Each gene As DataFrameRow In matrix.expression
            Dim levelRange As New DoubleRange(gene.experiments)
            Dim index As Integer() = gene.experiments _
                .Select(Function(a)
                            Return levelRange.ScaleMapping(a, indexRange)
                        End Function) _
                .Select(Function(d) CInt(d)) _
                .ToArray

            Yield New NamedCollection(Of Color) With {
                .name = gene.geneID,
                .value = index _
                    .Select(Function(i) colors(i)) _
                    .ToArray
            }
        Next
    End Function

    Public Sub Draw(g As IGraphics, layout As RectangleF, Optional horizontal As Boolean = True）

    End Sub
End Module
