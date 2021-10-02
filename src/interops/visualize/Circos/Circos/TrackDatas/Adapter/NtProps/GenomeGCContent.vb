#Region "Microsoft.VisualBasic::87395104e44ef49ba32b99e983a946d5, visualize\Circos\Circos\TrackDatas\Adapter\NtProps\GenomeGCContent.vb"

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

    '     Class GenomeGCContent
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: __sourceGC, FillData, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace TrackDatas.NtProps

    ''' <summary>
    ''' 整个基因组范围内的GC%变化的情况
    ''' </summary>
    Public Class GenomeGCContent : Inherits data(Of ValueTrackData)

        Sub New(nt As FastaSeq, Optional SegmentLength As Integer = -1, Optional steps As Integer = 10, Optional avg As Boolean = True)
            Call MyBase.New(
                __sourceGC(nt, If(SegmentLength <= 0, 10, SegmentLength), steps, avg))
        End Sub

        ''' <summary>
        ''' 请注意``chr``的值
        ''' </summary>
        ''' <param name="data"></param>
        Sub New(data As IEnumerable(Of NASegment_GC))
            MyBase.New(data)
        End Sub

        Private Overloads Shared Function __sourceGC(nt As FastaSeq, segLen As Integer, steps As Integer, usingAvg As Boolean) As NASegment_GC()
            Dim source As NASegment_GC() = GCProps.GetGCContentForGENOME(
                nt,
                winSize:=segLen,
                steps:=steps)
            Dim avg As Double = source.Average(Function(n) n.value)

            For Each x As NASegment_GC In source
                x.value -= avg
            Next

            Return source
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="len%">The genome nt length</param>
        ''' <returns></returns>
        Public Shared Function FillData(data As IEnumerable(Of NASegment_GC),
                                        len%,
                                        Optional slidWinSize% = 250,
                                        Optional steps% = 250,
                                        Optional chr$ = "chr1",
                                        Optional usingAvg As Boolean = True) As NASegment_GC()

            Dim out As NASegment_GC() = New NASegment_GC(len - 1) {}

            For i As Integer = 0 To out.Length - 1
                out(i) = New NASegment_GC With {
                    .start = i,
                    .end = i,
                    .chr = chr
                }
            Next

            For Each x As NASegment_GC In data
                For Each i% In seq(x.start, x.end, 1)
                    out(i - 1).value = (out(i - 1).value + x.value) / 2
                Next
            Next

            Dim slides = out.SlideWindows(slidWinSize, steps).ToArray

            out = New NASegment_GC(slides.Length - 1) {}

            For Each x As SeqValue(Of SlideWindow(Of NASegment_GC)) In slides.SeqIterator
                out(x.i) = x.value.First
                out(x.i).value = x.value.Average(Function(o) o.value)
                out(x.i).end = x.value.Last.end
            Next

            If usingAvg Then
                Dim avg# = out.Average(Function(x) x.value)

                For Each x As NASegment_GC In out
                    x.value -= avg#
                Next
            End If

            Return out
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
