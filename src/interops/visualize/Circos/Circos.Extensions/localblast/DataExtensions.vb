#Region "Microsoft.VisualBasic::b169c112ced6d2a3e7b7759d2bdd68e4, visualize\Circos\Circos.Extensions\localblast\DataExtensions.vb"

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

    ' Module DataExtensions
    ' 
    '     Function: createBands, FromBlastnMappings, Hits, Identities, IdentitiesTracks
    '               MapsRaw
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.Visualize.Circos.Colors
Imports SMRUCC.genomics.Visualize.Circos.Karyotype
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas
Imports KaryotypeModel = SMRUCC.genomics.Visualize.Circos.Karyotype.Karyotype

Public Module DataExtensions

    ''' <summary>
    ''' Creates the model for the multiple chromosomes genome data in circos.(使用这个函数进行创建多条染色体的)
    ''' </summary>
    ''' <param name="source">Band数据</param>
    ''' <param name="chrs">karyotype数据</param>
    ''' <returns></returns>
    Public Function FromBlastnMappings(source As IEnumerable(Of BlastnMapping), chrs As IEnumerable(Of FastaSeq)) As KaryotypeChromosomes
        Dim ks As KaryotypeChromosomes = FromNts(chrs)
        Dim labels As Dictionary(Of String, KaryotypeModel) = ks.Karyotypes.ToDictionary(Function(x) x.nt.value.Title, Function(x) x)
        Dim reads = source.ToArray
        Dim bands As List(Of Band) = reads.createBands(labels).AsList

        With bands.VectorShadows
            .MapsRaw = reads
        End With

        Dim nts As Dictionary(Of String, SimpleSegment) =
                chrs.ToDictionary(
                Function(x) x.Title,
                Function(x)
                    Return New SimpleSegment With {
                        .SequenceData = NucleicAcid.RemoveInvalids(x.SequenceData)
                    }
                End Function)

        Dim getNT As Func(Of Band, FastaSeq) = Function(x) As FastaSeq
                                                   Dim map As BlastnMapping = x.MapsRaw
                                                   Dim nt As SimpleSegment = nts(map.Reference)
                                                   Dim fragment As FastaSeq = nt _
                                                          .CutSequenceLinear(map.MappingLocation) _
                                                          .SimpleFasta(map.ReadQuery)

                                                   Return fragment
                                               End Function

        Dim props = bands.Select(getNT).PropertyMaps
        Dim gc#

        For Each band As Band In bands
            gc = props.props(band.MapsRaw.ReadQuery).value
            band.color = props.GC(gc)
        Next

        Return ks.AddBands(bands.OrderBy(Function(x) x.chrName))
    End Function

    <Extension>
    Private Function createBands(reads As BlastnMapping(), labels As Dictionary(Of String, KaryotypeModel)) As IEnumerable(Of Band)
        Return From x As SeqValue(Of BlastnMapping)
               In reads.SeqIterator(offset:=1)
               Let chr As String = labels(x.value.Reference).chrName
               Let loci As NucleotideLocation = x.value.MappingLocation
               Select New Band With {
                       .chrName = chr,
                       .start = loci.Left,
                       .end = loci.Right,
                       .color = "",
                       .bandX = "band" & x.i,
                       .bandY = "band" & x.i
                   }
    End Function

    <Extension>
    Public Function MapsRaw(x As Band, Optional reads As BlastnMapping = Nothing) As BlastnMapping
        If Not reads Is Nothing Then
            x(NameOf(MapsRaw)) = reads
        End If

        Return x(NameOf(MapsRaw))
    End Function

    <Extension>
    Public Function Hits(source As IEnumerable(Of BlastnMapping),
                            karyotype As Karyotype.SkeletonInfo,
                            Optional steps% = 2048) As ValueTrackData()

        Dim chrs As Dictionary(Of String, Karyotype.Karyotype) =
            karyotype.GetchrLabels(Function(x) x.chrLabel)
        Dim LQuery = From x As BlastnMapping
                     In source
                     Select x.MappingLocation,
                         Identity = x.identitiesValue,
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
                    idata(i).value.Value += 1
                Next
            Next

            Dim slides = idata.SlideWindows(4096, steps)
            Dim tmp As New List(Of SeqValue(Of Value(Of Integer)))

            For Each chunk In slides
                Dim n As Integer =
                    CInt(chunk.Items.Select(Function(x) x.value.Value).Average)

                tmp += New SeqValue(Of Value(Of Integer)) With {
                    .i = chunk.left,
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
                        .value = x.value.value.Value
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
                         Identity = x.identitiesValue,
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
                    .i = chunk.left,
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
                         Identity = x.identitiesValue,
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
End Module
