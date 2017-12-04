#Region "Microsoft.VisualBasic::a99770c361d4cc950f16063dc7d3e3c1, ..\interops\visualize\Circos\Circos\TrackDatas\Adapter\Highlights\GradientMappings.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports SMRUCC.genomics.Visualize.Circos.Colors

Namespace TrackDatas.Highlights

    Public Class GradientMappings : Inherits Highlights

        Sub New(locis As IEnumerable(Of ILoci),
                length%, mapName$, winSize%,
                Optional replaceBase As Boolean = False,
                Optional extTails As Boolean = False,
                Optional chr$ = "chr1")

            Dim g = From site As ILoci
                    In locis
                    Select site
                    Group site By site.Left Into Group
            Dim d As Dictionary(Of Integer, Double) =
                g.ToDictionary(
                Function(site) site.Left,
                Function(site) CDbl(site.Group.ToArray.Length))

            __source = __initCommon(chr, d, length, mapName, winSize, replaceBase, extTails)
        End Sub

        Protected Shared Function __initCommon(chr$,
                                               d As Dictionary(Of Integer, Double),
                                               length As Integer,
                                               mapName As String,
                                               winSize As Integer,
                                               replaceBase As Boolean,
                                               extTails As Boolean,
                                               Optional steps As Integer = 0) As List(Of ValueTrackData)
            Dim values As Double() =
                length.Sequence.Select(Function(idx) d.TryGetValue(idx, [default]:=0)).ToArray

            Return __initCommon(
                chr, values, length,
                mapName,
                winSize, replaceBase,
                extTails, steps)
        End Function

        Protected Shared Function __initCommon(chr$, values#(), len%,
                                               mapName$, winSize%,
                                               replaceBase As Boolean,
                                               extTails As Boolean,
                                               Optional steps% = 0) As List(Of ValueTrackData)
            Dim avgs As Double()

            Call $"  >>{GetType(GradientMappings).FullName}   min= {values.Min};   max={values.Max};  @{mapName}".__DEBUG_ECHO

            If winSize > 0 Then
                Dim slids = values.CreateSlideWindows(winSize, extTails:=extTails)  '划窗平均值
                avgs = slids.Select(Function(win, idx) win.Average).ToArray  '- values(idx) / 15)
            Else
                avgs = values
            End If

            Dim colors As Mappings() = GradientMaps.GradientMappings(
                avgs, mapName,
                replaceBase:=replaceBase)

            Dim out As List(Of ValueTrackData)

            If winSize > 0 Then
                out = New List(Of ValueTrackData)(
                    colors.Select(
                    Function(site, idx) FromColorMapping(site, idx + 1, 0)))
            Else
                Dim bufs As New List(Of ValueTrackData)
                Dim i As Integer

                For Each x As Mappings In colors
                    Dim o As ValueTrackData = FromColorMapping(x, i, steps)
                    i += steps
                    bufs += o
                Next

                out = bufs
            End If

            For Each x As ValueTrackData In out
                x.chr = chr
                x.value = 1
            Next

            out += New ValueTrackData With {
                .value = 0,
                .chr = "chr1",
                .start = 1,
                .end = 1,
                .comment = "This data point used for makes ranges of [0, 1]"
            }

            Return out
        End Function

        Sub New(values As IEnumerable(Of Double),
                length As Integer,
                mapName As String,
                winSize As Integer,
                Optional replaceBase As Boolean = False,
                Optional extTails As Boolean = False, Optional chr As String = "chr1")
            Dim d As Dictionary(Of Integer, Double) =
                values.Sequence.ToDictionary(Function(i) i, Function(i) values(i))
            Me.__source = __initCommon(
                chr, d, length, mapName, winSize, replaceBase, extTails)
        End Sub

        Sub New(values As IEnumerable(Of ValueTrackData),
                karyotype As Karyotype.SkeletonInfo,
                mapName As String,
                Optional replaceBase As Boolean = False,
                Optional extTails As Boolean = False,
                Optional winsize As Integer = 2048)

            Dim labels As Dictionary(Of String, Karyotype.Karyotype) = karyotype.GetchrLabels
            Dim chrs = From x As ValueTrackData
                       In values
                       Select x
                       Group x By x.chr Into Group

            Me.__source = New List(Of ValueTrackData)

            For Each ch In chrs
                Dim length As Integer = labels(ch.chr).end
                Dim bufs As Double() = ch.Group.Vector(length, Function(x) x.value)

                Dim slides = bufs.SlideWindows(winsize, 2048)

                bufs = slides.Select(Function(x) x.Average).ToArray

                Dim chunk As List(Of ValueTrackData) = __initCommon(
                    ch.chr, bufs, length,
                    mapName, -1,
                    replaceBase, extTails, 2048)

                Call __source.AddRange(chunk)
            Next
        End Sub
    End Class
End Namespace
