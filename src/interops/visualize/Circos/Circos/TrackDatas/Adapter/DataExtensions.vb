#Region "Microsoft.VisualBasic::3c9bb45de680d239756d0f6af5e0f2b8, ..\interops\visualize\Circos\Circos\TrackDatas\Adapter\DataExtensions.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application

Namespace TrackDatas

    Public Module DataExtensions

        <Extension>
        Public Function Hits(source As IEnumerable(Of BlastnMapping),
                             karyotype As Karyotype.SkeletonInfo,
                             Optional steps% = 2048) As ValueTrackData()

            Dim chrs As Dictionary(Of String, Karyotype.Karyotype) =
                karyotype.GetchrLabels(Function(x) x.chrLabel)
            Dim LQuery = From x As BlastnMapping
                         In source
                         Select x.MappingLocation,
                             Identity = x.Identities,
                             chr = x.Reference.Split("."c).First
                         Group By chr Into Group
            Dim list As New List(Of ValueTrackData)

            For Each ch In LQuery
                Dim chr As Karyotype.Karyotype = chrs(ch.chr)
                Dim idata As SeqValue(Of Value(Of Integer))() =
                    LinqAPI.Exec(Of SeqValue(Of Value(Of Integer))) <= From i As Integer
                                                                       In chr.end.Sequence
                                                                       Select New SeqValue(Of Value(Of Integer)) With {
                                                                           .i = i,
                                                                           .value = New Value(Of Integer)
                                                                       }

                For Each reads In ch.Group
                    For i As Integer = reads.MappingLocation.Left To reads.MappingLocation.Right
                        idata(i).value.value += 1
                    Next
                Next

                Dim slides = idata.SlideWindows(4096, steps)
                Dim tmp As New List(Of SeqValue(Of Value(Of Integer)))

                For Each chunk In slides
                    Dim n As Integer =
                        CInt(chunk.Items.Select(Function(x) x.value.value).Average)

                    tmp += New SeqValue(Of Value(Of Integer)) With {
                        .i = chunk.Left,
                        .value = New Value(Of Integer)(n)
                    }
                Next

                list += From x As SeqValue(Of SeqValue(Of Value(Of Integer)))
                        In tmp.SeqIterator
                        Let left As Integer = x.value.i
                        Select New ValueTrackData With {
                            .chr = chr.chrName,
                            .start = left,
                            .end = left + steps,
                            .value = x.value.value.value
                        }

                Call Console.Write(".")
            Next

            Return list.ToArray
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="karyotype">用于得到chr标签</param>
        ''' <returns></returns>
        <Extension>
        Public Function Identities(source As IEnumerable(Of BlastnMapping), karyotype As Karyotype.SkeletonInfo, Optional steps As Integer = 2048) As ValueTrackData()
            Dim chrs As Dictionary(Of String, Karyotype.Karyotype) = karyotype.GetchrLabels(Function(x) x.chrLabel)
            Dim LQuery = From x As BlastnMapping
                         In source
                         Select x.MappingLocation,
                             Identity = x.Identities,
                             chr = x.Reference.Split("."c).First
                         Group By chr Into Group
            Dim list As New List(Of ValueTrackData)

            For Each ch In LQuery
                Dim chr As Karyotype.Karyotype = chrs(ch.chr)
                Dim idata As SeqValue(Of List(Of Double))() =
                    LinqAPI.Exec(Of SeqValue(Of List(Of Double))) <= From i As Integer
                                                                     In chr.end.Sequence
                                                                     Select New SeqValue(Of List(Of Double)) With {
                                                                         .i = i,
                                                                         .value = New List(Of Double)
                                                                     }

                For Each reads In ch.Group
                    For i As Integer = reads.MappingLocation.Left To reads.MappingLocation.Right
                        idata(i).value.Add(reads.Identity)
                    Next
                Next

                For Each x In idata
                    If x.value.Count = 0 Then
                        Call x.value.Add(0R)
                    End If
                Next

                Dim slides = idata.SlideWindows(4096, steps)
                Dim tmp As New List(Of SeqValue(Of List(Of Double)))

                For Each chunk In slides
                    tmp += New SeqValue(Of List(Of Double)) With {
                        .i = chunk.Left,
                        .value = LinqAPI.MakeList(Of Double) <= From x As SeqValue(Of List(Of Double))
                                                                In chunk.Items
                                                                Let bufs As IEnumerable(Of Double) = x.value
                                                                Select bufs.Average
                    }
                Next

                list += From x As SeqValue(Of SeqValue(Of List(Of Double)))
                        In tmp.SeqIterator
                        Where x.value.value.Count > 0
                        Let left As Integer = x.value.i
                        Select New ValueTrackData With {
                            .chr = chr.chrName,
                            .start = left,
                            .end = left + steps,
                            .value = x.value.value.Average
                        }

                Call Console.Write(".")
            Next

            Return list.ToArray
        End Function

        <Extension>
        Public Function IdentitiesTracks(source As IEnumerable(Of BlastnMapping), karyotype As Karyotype.SkeletonInfo) As ValueTrackData()
            Dim chrs As Dictionary(Of String, Karyotype.Karyotype) = karyotype.GetchrLabels(Function(x) x.chrLabel)
            Dim LQuery = From x As BlastnMapping
                         In source
                         Select x.MappingLocation,
                             Identity = x.Identities,
                             chr = x.Reference.Split("."c).First
                         Group By chr Into Group
            Dim list As New List(Of ValueTrackData)

            For Each ch In LQuery
                Dim chr As Karyotype.Karyotype = chrs(ch.chr)
                Dim idata As List(Of Double)() =
                    LinqAPI.Exec(Of List(Of Double)) <= From i As Integer
                                                        In chr.end.Sequence
                                                        Select New List(Of Double)
                For Each reads In ch.Group
                    For i As Integer = reads.MappingLocation.Left To reads.MappingLocation.Right
                        idata(i).Add(reads.Identity)
                    Next
                Next

                For Each x In idata
                    If x.Count = 0 Then
                        Call x.Add(0R)
                    End If
                Next

                list += From x As SeqValue(Of List(Of Double))
                        In idata.SeqIterator
                        Where x.value.Count > 0
                        Let left As Integer = x.i
                        Select New ValueTrackData With {
                            .chr = chr.chrName,
                            .start = left,
                            .end = left + 1,
                            .value = x.value.Average
                        }

                Call Console.Write(".")
            Next

            Return list.ToArray
        End Function

        <Extension>
        Public Function GetchrLabels(karyotype As Karyotype.SkeletonInfo,
                                     Optional getKey As Func(Of Karyotype.Karyotype, String) = Nothing) As Dictionary(Of String, Karyotype.Karyotype)

            If getKey Is Nothing Then
                getKey = Function(x) x.chrName
            End If

            Return karyotype.Karyotypes.ToDictionary(getKey)
        End Function
    End Module
End Namespace
