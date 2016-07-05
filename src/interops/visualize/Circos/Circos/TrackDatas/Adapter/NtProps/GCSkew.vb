#Region "Microsoft.VisualBasic::4342787f47aaf5b9a84ccd453bcc5cf7, ..\interops\visualize\Circos\Circos\TrackDatas\Adapter\NtProps\GCSkew.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports SMRUCC.genomics.SequenceModel.ISequenceModel
Imports System.Text
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports Microsoft.VisualBasic

Namespace TrackDatas.NtProps

    ''' <summary>
    ''' G+C/G-C偏移量
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GCSkew : Inherits data(Of ValueTrackData)

        Sub New(SequenceModel As I_PolymerSequenceModel,
                SlideWindowSize As Integer,
                Steps As Integer,
                Circular As Boolean,
                Optional chr As String = "chr1")
            Call MyBase.New(
                __sourceGC(chr,
                         NucleotideModels.GCSkew(SequenceModel,
                                                 SlideWindowSize,
                                                 Steps,
                                                 Circular),
                         Steps))
        End Sub

        Sub New(data As IEnumerable(Of Double), [step] As Integer, Optional chr As String = "chr1")
            Call MyBase.New(__sourceGC(chr, __avgData(data), [step]))
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="genome"></param>
        ''' <param name="karyotype">chr标记的来源</param>
        ''' <param name="SlideWindowSize"></param>
        ''' <param name="Steps"></param>
        ''' <param name="Circular"></param>
        Sub New(genome As IEnumerable(Of FastaToken),
                karyotype As Karyotype.SkeletonInfo,
                SlideWindowSize As Integer,
                Steps As Integer,
                Circular As Boolean)

            Dim list As New List(Of ValueTrackData)
            Dim chrs = karyotype.Karyotypes.ToDictionary(
                Function(x) x.chrLabel,
                Function(x) x.chrName)

            For Each nt As FastaToken In genome
                Dim raw As Double() = NucleotideModels.GCSkew(
                    nt,
                    SlideWindowSize,
                    Steps,
                    Circular)
                Dim chr As String = nt.Title.Split("."c).First

                chr = chrs(chr)
                list += __sourceGC(chr, __avgData(raw), [Steps])

                Call Console.Write(">")
            Next

            __source = list
        End Sub

        Friend Shared Function __avgData(data As IEnumerable(Of Double)) As Double()
            Dim array As Double() = data.ToArray
            Dim avg As Double = array.Average

            For i As Integer = 0 To array.Length - 1
                array(i) = array(i) - avg
            Next

            Return array
        End Function

        Friend Shared Iterator Function __sourceGC(chr As String, data As IEnumerable(Of Double), [step] As Integer) As IEnumerable(Of ValueTrackData)
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

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Length"></param>
        ''' <param name="Width">Width设置为0的时候为1个像素的宽度</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateLineData(Length As Integer, Width As Integer) As GCSkew
            Return New GCSkew({CDbl(Width)}, [step]:=Length)
        End Function
    End Class
End Namespace
