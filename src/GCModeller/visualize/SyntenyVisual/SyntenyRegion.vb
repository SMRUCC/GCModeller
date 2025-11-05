#Region "Microsoft.VisualBasic::a404a9b5ddbc130ad4f858b2bd687ab1, visualize\SyntenyVisual\SyntenyRegion.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Class SyntenyRegion
    ' 
    '     Properties: query, score, subject
    ' 
    '     Function: Translate
    ' 
    ' Module SyntenyRegionExtensions
    ' 
    '     Function: PopulateRegions, rangeSelector
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping

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

        Dim i As i32 = Scan0

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
    ''' <remarks>
    ''' 如果基因组很大的话，这个函数的工作效率会非常低，这个函数只适合生成小基因组之间的绘图模型
    ''' </remarks>
    <Extension>
    Public Iterator Function PopulateRegions(Of Map As {IMapping, Class, New})(maps As IEnumerable(Of Map), Optional cutoff# = 0.25, Optional stepOffset As (q%, s%) = Nothing) As IEnumerable(Of SyntenyRegion)
        Dim blastn = maps.ToArray
        Dim qSize%() = blastn.Select(Function(n) {n.Qstart, n.Qstop}).IteratesALL.AsRange.Sequence(stepOffset.q).ToArray
        Dim sSize%() = blastn.Select(Function(n) {n.Sstart, n.Sstop}).IteratesALL.AsRange.Sequence(stepOffset.s).ToArray
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
        Dim intSymbol As New GenericSymbol(Of Integer)(
            equals:=Function(i, j) scoreProvider(i, j) >= 0.85,
            similarity:=Function(i, j) scoreProvider(i, j),
            toChar:=Function(i) Strings.Chr(i),
            empty:=Function() 0
        )
        Dim smithwaterMan As New GSW(Of Integer)(
            query:=qSize,
            subject:=sSize,
            symbol:=intSymbol
        )

        ' match的位置就是基因组上面的坐标位置
        For Each map In smithwaterMan.Matches(cutoff * smithwaterMan.MaxScore)
            Yield New SyntenyRegion With {
                .query = New DoubleRange({map.fromA, map.toA}),
                .subject = New DoubleRange({map.fromB, map.toB}),
                .score = map.score
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
