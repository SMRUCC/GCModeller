#Region "Microsoft.VisualBasic::b953160f25b56165a1c1b92cbed651fd, ..\interops\visualize\Circos\Circos\Karyotype\chrKaryotype.vb"

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

Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
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
        ''' <param name="bandData"><see cref="TripleKeyValuesPair.Key"/>为颜色，其余的两个属性分别为左端起始和右端结束</param>
        Sub New(gSize As Integer, color As String, Optional bandData As TripleKeyValuesPair() = Nothing)
            Me.Size = gSize
            Me.__bands = New List(Of Band)(GenerateDocument(bandData))
            Call __karyotype(color)
        End Sub

        Protected Sub New()
        End Sub

        Private Overloads Shared Iterator Function GenerateDocument(data As IEnumerable(Of TripleKeyValuesPair)) As IEnumerable(Of Band)
            If Not data.IsNullOrEmpty Then
                Dim i As Integer

                For Each x As TripleKeyValuesPair In data
                    Yield New Band With {
                        .chrName = "chr1",
                        .bandX = "band" & i,
                        .bandY = "band" & i,
                        .start = x.Value1.ParseInteger,
                        .end = x.Value2.ParseInteger,
                        .color = x.Key
                    }

                    i += 1
                Next
            End If
        End Function

        Public Overrides ReadOnly Property Size As Integer

        Public Shared Function FromNts(chrs As IEnumerable(Of FastaToken), Optional colors As String() = Nothing) As KaryotypeChromosomes
            If colors.IsNullOrEmpty Then
                colors = CircosColor.AllCircosColors.Shuffles
            End If

            Dim rnd As New Random
            Dim ks As Karyotype() =
                LinqAPI.Exec(Of Karyotype) <= From nt As SeqValue(Of FastaToken)
                                              In chrs.SeqIterator(offset:=1)
                                              Let name As String =
                                                  nt.obj.Title _
                                                        .Split("."c).First _
                                                        .NormalizePathString(True) _
                                                        .Replace(" ", "_")
                                              Let clInd As Integer = rnd.NextInteger(colors.Length).value
                                              Select New Karyotype With {
                                                  .chrName = "chr" & nt.i,
                                                  .chrLabel = name,
                                                  .color = colors(clInd),
                                                  .start = 0,
                                                  .end = nt.obj.Length
                                              }.nt.SetValue(nt.obj).As(Of Karyotype)

            Return New KaryotypeChromosomes With {
                .__karyotypes = ks.ToList
            }
        End Function

        ''' <summary>
        ''' Creates the model for the multiple chromosomes genome data in circos.(使用这个函数进行创建多条染色体的)
        ''' </summary>
        ''' <param name="source">Band数据</param>
        ''' <param name="chrs">karyotype数据</param>
        ''' <returns></returns>
        Public Shared Function FromBlastnMappings(source As IEnumerable(Of BlastnMapping), chrs As IEnumerable(Of FastaToken)) As KaryotypeChromosomes
            Dim ks As KaryotypeChromosomes = FromNts(chrs)
            Dim labels As Dictionary(Of String, Karyotype) =
                ks.__karyotypes.ToDictionary(Function(x) x.nt.Value.Title,
                                             Function(x) x)
            Dim bands As List(Of Band) =
                LinqAPI.MakeList(Of Band) <= From x As SeqValue(Of BlastnMapping)
                                             In source.SeqIterator(offset:=1)
                                             Let chr As String = labels(x.obj.Reference).chrName
                                             Let loci As NucleotideLocation = x.obj.MappingLocation
                                             Select New Band With {
                                                 .chrName = chr,
                                                 .start = loci.Left,
                                                 .end = loci.Right,
                                                 .color = "",
                                                 .bandX = "band" & x.i,
                                                 .bandY = "band" & x.i
                                             }.MapsRaw.SetValue(x.obj).As(Of Band)

            Dim nts As Dictionary(Of String, SegmentReader) =
                chrs.ToDictionary(
                Function(x) x.Title,
                Function(x) New SegmentReader(NucleicAcid.RemoveInvalids(x.SequenceData)))

            Dim __getNt As Func(Of Band, FastaToken) =
                Function(x) As FastaToken
                    Dim map As BlastnMapping = x.MapsRaw.Value
                    Dim nt As SegmentReader = nts(map.Reference)
                    Dim fragment As FastaToken =
                        nt.TryParse(map.MappingLocation) _
                          .GetFasta(map.ReadQuery)
                    Return fragment
                End Function

            Dim props = bands.Select(__getNt).PropertyMaps

            For Each band As Band In bands
                Dim GC As Double = props.props(band.MapsRaw.Value.ReadQuery).value
                band.color = props.GC(GC)
            Next

            ks.__bands = bands.OrderBy(Function(x) x.chrName).ToList

            Return ks
        End Function
    End Class
End Namespace
