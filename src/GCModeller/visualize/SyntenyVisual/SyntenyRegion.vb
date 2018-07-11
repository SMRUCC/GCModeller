Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application

Public Class SyntenyRegion

    Public Property query As DoubleRange
    Public Property subject As DoubleRange

    Public Iterator Function Translate(genomeSize As (query&, subject&), plotRegion As RectangleF) As IEnumerable(Of PointF)
        Dim qY = plotRegion.Top
        Dim sY = plotRegion.Top
        Dim posX As Vector = {query.Min, query.Max, subject.Min, subject.Max}
        Dim gSize As Vector = {
            genomeSize.query, genomeSize.query,
            genomeSize.subject, genomeSize.subject
        }

        posX = posX / gSize * plotRegion.Width

        Dim i As int = Scan0

        Yield New PointF(posX(++i), qY)
        Yield New PointF(posX(++i), qY)
        Yield New PointF(posX(++i), sY)
        Yield New PointF(posX(++i), sY)
    End Function

    ''' <summary>
    ''' 在这里会使用SmithWater-Man算法将blastn独立的基因比对结果链接为更加宽泛的同源区域
    ''' </summary>
    ''' <param name="maps"></param>
    ''' <param name="schema$"></param>
    ''' <returns></returns>
    Public Shared Iterator Function PopulateRegions(maps As IEnumerable(Of BlastnMapping), Optional schema$ = "Set2:c8") As IEnumerable(Of SyntenyRegion)
        Dim blastn = maps.ToArray
        Dim qSize%() = blastn.Select(Function(n) {n.QueryLeft, n.QueryRight}).IteratesALL.AsRange.Sequence.ToArray
        Dim sSize%() = blastn.Select(Function(n) {n.ReferenceLeft, n.ReferenceRight}).IteratesALL.AsRange.Sequence.ToArray
        Dim sortQ = blastn.OrderBy(Function(n) n.QueryLeft).ToArray
        Dim sortS = blastn.OrderBy(Function(n) n.ReferenceLeft).ToArray
        Dim smithwaterMan As New GSW(Of Integer)(
            qSize, sSize,
            Function(q, s)
                Dim qSel = rangeSelector(sortQ, q, Function(n) n.QueryLeft).ToArray
                Dim sSel = rangeSelector(sortS, s, Function(n) n.ReferenceLeft).ToArray

                ' 查看二者是否存在交集？
                ' 存在交集的话返回相等
                If qSel.Length = 0 OrElse sSel.Length = 0 Then
                    Return 0
                End If

                Dim score As Integer = 0

                For Each qMap In qSel
                    For Each sMap In sSel
                        If qMap Is sMap Then
                            score += 1
                        End If
                    Next
                Next

                If score = 0 Then
                    Return -1
                Else
                    Return score
                End If
            End Function,
            Function(x)
                Return "-"c
            End Function)
    End Function

    Private Shared Iterator Function rangeSelector(maps As BlastnMapping(), x%, getX As Func(Of BlastnMapping, Integer)) As IEnumerable(Of BlastnMapping)
        Dim i As Integer = 0

        For i = i To maps.Length - 1
            If getX(maps(i)) >= x Then
                Exit For
            End If
        Next

        Do While (i <= maps.Length - 1) AndAlso getX(maps(i)) <= x
            Yield maps(i)
            i += 1
        Loop
    End Function
End Class