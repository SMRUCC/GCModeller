#Region "Microsoft.VisualBasic::6c41e345fc2dd182ba88d94d26bf7f7d, visualize\Circos\Circos\TrackDatas\Adapter\NtProps\GCSkew.vb"

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

    '     Class GCSkew
    ' 
    '         Constructor: (+4 Overloads) Sub New
    '         Function: CreateLineData, FromValueContents, means, trackValues
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace TrackDatas.NtProps

    ''' <summary>
    ''' G+C/G-C偏移量
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GCSkew : Inherits data(Of ValueTrackData)

        Sub New(nt As IPolymerSequenceModel,
                slideWinSize As Integer,
                steps As Integer,
                isCircular As Boolean,
                Optional chr As String = "chr1")
            Call MyBase.New(trackValues(chr, NucleotideModels.GCSkew(nt, slideWinSize, steps, isCircular), steps))
        End Sub

        Sub New(data As IEnumerable(Of Double), [step] As Integer, Optional chr As String = "chr1")
            Call MyBase.New(trackValues(chr, means(data), [step]))
        End Sub

        Sub New(trackValues As IEnumerable(Of ValueTrackData))
            Call MyBase.New(trackValues)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="genome"></param>
        ''' <param name="karyotype">chr标记的来源</param>
        ''' <param name="winSize"></param>
        ''' <param name="steps"></param>
        ''' <param name="isCircular"></param>
        Sub New(genome As IEnumerable(Of FastaSeq),
                karyotype As Karyotype.SkeletonInfo,
                winSize As Integer,
                steps As Integer,
                isCircular As Boolean)

            Dim list As New List(Of ValueTrackData)
            Dim chrs As Dictionary(Of String, String) = karyotype _
                .Karyotypes _
                .ToDictionary(Function(chr) chr.chrLabel,
                              Function(chr)
                                  Return chr.chrName
                              End Function)

            For Each nt As FastaSeq In genome
                Dim raw As Double() = NucleotideModels.GCSkew(
                    nt,
                    winSize,
                    steps,
                    isCircular
                )
                Dim chr As String = nt.Title.Split("."c).First

                chr = chrs(chr)
                list += trackValues(chr, means(raw), steps)

                Call $" > {nt.Title}".__DEBUG_ECHO
            Next

            source = list
        End Sub

        Friend Shared Function means(data As IEnumerable(Of Double)) As Double()
            Dim array As Double() = data.ToArray
            Dim avg As Double = array.Average

            For i As Integer = 0 To array.Length - 1
                array(i) = array(i) - avg
            Next

            Return array
        End Function

        Friend Shared Iterator Function trackValues(chr As String, data As IEnumerable(Of Double), [step] As Integer) As IEnumerable(Of ValueTrackData)
            Dim p As Integer

            For Each n As Double In data
                Yield New ValueTrackData With {
                    .chr = chr,
                    .start = p,
                    .end = p + [step],
                    .value = n
                }
                p += [step]
            Next
        End Function

        Public Shared Iterator Function FromValueContents(genes As IEnumerable(Of IGeneBrief), contents As Dictionary(Of String, Double), winSize%, steps%) As IEnumerable(Of ValueTrackData)
            Dim context As New GenomeContext(Of IGeneBrief)(genes)
            Dim gSize = context.size
            Dim values#()

            For i As Integer = 0 To gSize Step steps
                genes = context.SelectByRange(i, i + winSize)
                values = genes _
                    .Where(Function(g) contents.ContainsKey(g.Key)) _
                    .Select(Function(g)
                                Return contents(g.Key)
                            End Function) _
                    .ToArray

                Yield New ValueTrackData With {
                    .chr = "chr1",
                    .[end] = i + winSize,
                    .start = i,
                    .value = If(values.Length = 0, 0, values.Average)
                }
            Next
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Length"></param>
        ''' <param name="Width">Width设置为0的时候为1个像素的宽度</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function CreateLineData(length As Integer, width As Integer) As GCSkew
            Return New GCSkew({CDbl(width)}, [step]:=length)
        End Function
    End Class
End Namespace
