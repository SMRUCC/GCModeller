﻿#Region "Microsoft.VisualBasic::c39421dda57d8bc4ffed9d1ade215a80, visualize\Circos\Circos\Karyotype\Chromosome.vb"

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

    '     Class KaryotypeChromosomes
    ' 
    '         Properties: Size
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: FromBlastnMappings, FromNts, GenerateDocument
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.Visualize.Circos.Colors

Namespace Karyotype

    ''' <summary>
    ''' The very basically genome skeleton information description.(基因组的基本框架的描述信息)
    ''' </summary>
    Public Class KaryotypeChromosomes : Inherits SkeletonInfo

        ''' <summary>
        ''' 这个构造函数是用于单个染色体的
        ''' </summary>
        ''' <param name="gSize">The genome size.</param>
        ''' <param name="color"></param>
        ''' <param name="bandData"><see cref="NamedTuple(Of String).Name"/>为颜色，其余的两个属性分别为左端起始和右端结束</param>
        Sub New(gSize As Integer, color As String, Optional bandData As NamedTuple(Of String)() = Nothing)
            Me.Size = gSize
            Me.__bands = New List(Of Band)(GenerateDocument(bandData))
            Call __karyotype(color)
        End Sub

        Protected Sub New()
        End Sub

        Private Overloads Shared Iterator Function GenerateDocument(data As IEnumerable(Of NamedTuple(Of String))) As IEnumerable(Of Band)
            If Not data Is Nothing Then
                Dim i As Integer

                For Each x As NamedTuple(Of String) In data
                    Yield New Band With {
                        .chrName = "chr1",
                        .bandX = "band" & i,
                        .bandY = "band" & i,
                        .start = x.Item1.ParseInteger,
                        .end = x.Item2.ParseInteger,
                        .color = x.Name
                    }

                    i += 1
                Next
            End If
        End Function

        Public Overrides ReadOnly Property Size As Integer

        Public Shared Function FromNts(chrs As IEnumerable(Of FastaSeq), Optional colors As String() = Nothing) As KaryotypeChromosomes
            If colors.IsNullOrEmpty Then
                colors = CircosColor.AllCircosColors.Shuffles
            End If

            Dim rnd As New Random
            Dim chrVector = chrs.ToArray
            Dim ks As Karyotype() =
 _
                LinqAPI.Exec(Of Karyotype) <= From nt As SeqValue(Of FastaSeq)
                                              In chrVector.SeqIterator(offset:=1)
                                              Let fasta = nt.value
                                              Let name As String = fasta.Title _
                                                  .Split("."c) _
                                                  .First _
                                                  .NormalizePathString(True) _
                                                  .Replace(" ", "_")
                                              Let clInd As Integer = rnd.NextInteger(colors.Length).value
                                              Select New Karyotype With {
                                                  .chrName = "chr" & nt.i,
                                                  .chrLabel = name,
                                                  .color = colors(clInd),
                                                  .start = 0,
                                                  .end = fasta.Length
                                              }
            With ks.VectorShadows
                .nt = chrVector
            End With

            Return New KaryotypeChromosomes With {
                .__karyotypes = ks.AsList
            }
        End Function

        ''' <summary>
        ''' Creates the model for the multiple chromosomes genome data in circos.(使用这个函数进行创建多条染色体的)
        ''' </summary>
        ''' <param name="source">Band数据</param>
        ''' <param name="chrs">karyotype数据</param>
        ''' <returns></returns>
        Public Shared Function FromBlastnMappings(source As IEnumerable(Of BlastnMapping), chrs As IEnumerable(Of FastaSeq)) As KaryotypeChromosomes
            Dim ks As KaryotypeChromosomes = FromNts(chrs)
            Dim labels As Dictionary(Of String, Karyotype) =
                ks.__karyotypes.ToDictionary(Function(x) x.nt.value.Title,
                                             Function(x) x)
            Dim reads = source.ToArray
            Dim bands As List(Of Band) =
 _
                LinqAPI.MakeList(Of Band) <= From x As SeqValue(Of BlastnMapping)
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

            Dim __getNt As Func(Of Band, FastaSeq) = Function(x) As FastaSeq
                                                           Dim map As BlastnMapping = x.MapsRaw.value
                                                           Dim nt As SimpleSegment = nts(map.Reference)
                                                           Dim fragment As FastaSeq =
                                                               nt _
                                                               .CutSequenceLinear(map.MappingLocation) _
                                                               .SimpleFasta(map.ReadQuery)
                                                           Return fragment
                                                       End Function

            Dim props = bands.Select(__getNt).PropertyMaps

            For Each band As Band In bands
                Dim GC As Double = props.props(band.MapsRaw.value.ReadQuery).value
                band.color = props.GC(GC)
            Next

            ks.__bands = bands.OrderBy(Function(x) x.chrName).AsList

            Return ks
        End Function
    End Class
End Namespace
