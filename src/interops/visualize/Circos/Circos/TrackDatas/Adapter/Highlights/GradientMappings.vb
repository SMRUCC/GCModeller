#Region "Microsoft.VisualBasic::06b95444bcedda74c4e6fe8f71a28be6, visualize\Circos\Circos\TrackDatas\Adapter\Highlights\GradientMappings.vb"

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

    '     Class GradientMappings
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: __initCommon, mapGenerator
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
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
                    Group site By site.left Into Group
            Dim d As Dictionary(Of Integer, Double) =
                g.ToDictionary(Function(site) site.left,
                               Function(site)
                                   Return CDbl(site.Group.ToArray.Length)
                               End Function)

            source = __initCommon(chr, d, length, mapName, winSize, replaceBase, extTails)
        End Sub

        Protected Shared Function __initCommon(chr$,
                                               d As Dictionary(Of Integer, Double),
                                               length As Integer,
                                               mapName As String,
                                               winSize As Integer,
                                               replaceBase As Boolean,
                                               extTails As Boolean,
                                               Optional steps As Integer = 0) As List(Of ValueTrackData)

            Dim values As Double() = length _
                .Sequence _
                .Select(Function(idx)
                            Return d.TryGetValue(idx, [default]:=0)
                        End Function) _
                .ToArray

            Return mapGenerator(
                chr, values, length,
                mapName,
                winSize, replaceBase,
                extTails, steps
            ).AsList
        End Function

        Protected Shared Iterator Function mapGenerator(chr$, values#(), len%,
                                               mapName$, winSize%,
                                               replaceBase As Boolean,
                                               extTails As Boolean,
                                               Optional steps% = 0) As IEnumerable(Of ValueTrackData)
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
                replaceBase:=replaceBase
            )

            If winSize > 0 Then
                For Each point In colors.Select(Function(site, idx) FromColorMapping(site, idx + 1, 0))
                    point.chr = chr
                    point.value = 1

                    Yield point
                Next
            Else
                Dim i As Integer

                For Each x As Mappings In colors
                    Dim o As ValueTrackData = FromColorMapping(x, i, steps)

                    i += steps
                    o.chr = chr
                    o.value = 1

                    Yield o
                Next
            End If

            Yield New ValueTrackData With {
                .value = 0,
                .chr = "chr1",
                .start = 1,
                .end = 1,
                .comment = "This data point used for makes ranges of [0, 1]"
            }
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="values">这个数据点应该是和基因组等长的？</param>
        ''' <param name="length"></param>
        ''' <param name="mapName"></param>
        ''' <param name="winSize"></param>
        ''' <param name="replaceBase"></param>
        ''' <param name="extTails"></param>
        ''' <param name="chr"></param>
        Sub New(values As IEnumerable(Of Double),
                length As Integer,
                mapName As String,
                winSize As Integer,
                Optional replaceBase As Boolean = False,
                Optional extTails As Boolean = False, Optional chr As String = "chr1")
            Dim d As Dictionary(Of Integer, Double) =
                values.Sequence.ToDictionary(Function(i) i, Function(i) values(i))
            Me.source = __initCommon(
                chr, d, length, mapName, winSize, replaceBase, extTails)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="values"></param>
        ''' <param name="mapName">
        ''' <see cref="Designer.GetColors(String, Integer, Integer)"/>
        ''' </param>
        Sub New(values As IEnumerable(Of ValueTrackData), mapName As String)
            Dim chrs = From x As ValueTrackData
                       In values
                       Select x
                       Group x By x.chr Into Group

            Me.source = New List(Of ValueTrackData)

            For Each ch In chrs
                Dim ranges As DoubleRange = ch.Group.Select(Function(x) x.value).ToArray
                Dim colors As String() = Designer _
                    .GetColors(mapName, 500) _
                    .Select(Function(cl) $"({cl.R},{cl.G},{cl.B})") _
                    .ToArray
                Dim indexRange As DoubleRange = {0, colors.Length - 1}
                Dim chunk As ValueTrackData() = ch.Group _
                    .Select(Function(p)
                                p.formatting = New Formatting With {
                                    .fill_color = colors(CInt(Fix(ranges.ScaleMapping(p.value, indexRange))))
                                }

                                Return p
                            End Function) _
                    .ToArray

                Call source.AddRange(chunk)
            Next
        End Sub
    End Class
End Namespace
