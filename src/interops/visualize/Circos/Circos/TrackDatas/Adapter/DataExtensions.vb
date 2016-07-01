#Region "Microsoft.VisualBasic::87ea480ae3fa9082e726f7fd3424ee35, ..\Circos\Circos\TrackDatas\Adapter\DataExtensions.vb"

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

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace TrackDatas

    Public Module DataExtensions

        <Extension>
        Public Function Hits(source As IEnumerable(Of BlastnMapping), karyotype As Karyotype.SkeletonInfo, Optional steps As Integer = 2048) As ValueTrackData()
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
                Dim idata As SeqValue(Of Value(Of Integer))() =
                    LinqAPI.Exec(Of SeqValue(Of Value(Of Integer))) <= From i As Integer
                                                                       In chr.end.Sequence
                                                                       Select New SeqValue(Of Value(Of Integer)) With {
                                                                           .i = i,
                                                                           .obj = New Value(Of Integer)
                                                                       }

                For Each reads In ch.Group
                    For i As Integer = reads.MappingLocation.Left To reads.MappingLocation.Right
                        idata(i).obj.Value += 1
                    Next
                Next

                Dim slides = idata.SlideWindows(4096, steps)
                Dim tmp As New List(Of SeqValue(Of Value(Of Integer)))

                For Each chunk In slides
                    Dim n As Integer =
                        CInt(chunk.Elements.Select(Function(x) x.obj.Value).Average)

                    tmp += New SeqValue(Of Value(Of Integer)) With {
                        .i = chunk.Left,
                        .obj = New Value(Of Integer)(n)
                    }
                Next

                list += From x As SeqValue(Of SeqValue(Of Value(Of Integer)))
                        In tmp.SeqIterator
                        Let left As Integer = x.obj.i
                        Select New ValueTrackData With {
                            .chr = chr.chrName,
                            .start = left,
                            .end = left + steps,
                            .value = x.obj.obj.Value
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
                                                                         .obj = New List(Of Double)
                                                                     }

                For Each reads In ch.Group
                    For i As Integer = reads.MappingLocation.Left To reads.MappingLocation.Right
                        idata(i).obj.Add(reads.Identity)
                    Next
                Next

                For Each x In idata
                    If x.obj.Count = 0 Then
                        Call x.obj.Add(0R)
                    End If
                Next

                Dim slides = idata.SlideWindows(4096, steps)
                Dim tmp As New List(Of SeqValue(Of List(Of Double)))

                For Each chunk In slides
                    tmp += New SeqValue(Of List(Of Double)) With {
                        .i = chunk.Left,
                        .obj = LinqAPI.MakeList(Of Double) <= From x As SeqValue(Of List(Of Double))
                                                              In chunk.Elements
                                                              Let bufs As IEnumerable(Of Double) = x.obj
                                                              Select bufs.Average
                    }
                Next

                list += From x As SeqValue(Of SeqValue(Of List(Of Double)))
                        In tmp.SeqIterator
                        Where x.obj.obj.Count > 0
                        Let left As Integer = x.obj.i
                        Select New ValueTrackData With {
                            .chr = chr.chrName,
                            .start = left,
                            .end = left + steps,
                            .value = x.obj.obj.Average
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
                        Where x.obj.Count > 0
                        Let left As Integer = x.i
                        Select New ValueTrackData With {
                            .chr = chr.chrName,
                            .start = left,
                            .end = left + 1,
                            .value = x.obj.Average
                        }

                Call Console.Write(".")
            Next

            Return list.ToArray
        End Function

        <Extension>
        Public Function GetchrLabels(karyotype As Karyotype.SkeletonInfo, Optional getKey As Func(Of Karyotype.Karyotype, String) = Nothing) As Dictionary(Of String, Karyotype.Karyotype)
            If getKey Is Nothing Then
                getKey = Function(x) x.chrName
            End If

            Return karyotype.Karyotypes.ToDictionary(getKey)
        End Function
    End Module
End Namespace
