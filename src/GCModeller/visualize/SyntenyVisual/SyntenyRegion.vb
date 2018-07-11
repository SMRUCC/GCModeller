Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application

Public Class SyntenyRegion

    Public Property query As DoubleRange
    Public Property subject As DoubleRange
    Public Property score As Double

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
End Class

Public Module SyntenyRegionExtensions

    ''' <summary>
    ''' 在这里会使用SmithWater-Man算法将blastn独立的基因比对结果链接为更加宽泛的同源区域
    ''' </summary>
    ''' <param name="maps"></param>
    ''' <param name="cutoff">[0, 1]</param>
    ''' <returns></returns>
    Public Iterator Function PopulateRegions(Of Map As {IMapping, Class, New})(maps As IEnumerable(Of Map), Optional cutoff# = 0.25) As IEnumerable(Of SyntenyRegion)
        Dim blastn = maps.ToArray
        Dim qSize%() = blastn.Select(Function(n) {n.Qstart, n.Qstop}).IteratesALL.AsRange.Sequence.ToArray
        Dim sSize%() = blastn.Select(Function(n) {n.Sstart, n.Sstop}).IteratesALL.AsRange.Sequence.ToArray
        Dim sortQ = blastn.OrderBy(Function(n) n.Qstart).ToArray
        Dim sortS = blastn.OrderBy(Function(n) n.Sstart).ToArray
        Dim scoreProvider As ISimilarity(Of Integer) =
            Function(q, s) As Double
                Dim qSel = rangeSelector(sortQ, q, Function(n) n.Qstart).ToArray
                Dim sSel = rangeSelector(sortS, s, Function(n) n.Sstart).ToArray

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
            End Function
        Dim smithwaterMan As New GSW(Of Integer)(
            query:=qSize,
            subject:=sSize,
            similarity:=scoreProvider,
            asChar:=Function(x) "-"c
        )

        ' match的位置就是基因组上面的坐标位置
        For Each map In smithwaterMan.Matches(cutoff * smithwaterMan.MaxScore)
            Yield New SyntenyRegion With {
                .query = {map.FromA, map.ToA},
                .subject = {map.FromB, map.ToB},
                .score = map.Score
            }
        Next
    End Function

    <Extension>
    Private Iterator Function rangeSelector(Of Map As IMapping)(maps As Map(), x%, getX As Func(Of Map, Integer)) As IEnumerable(Of Map)
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
End Module