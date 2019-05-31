
Option Strict Off

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.Visualize.Circos.Colors
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Namespace Karyotype

    Public Module ChromosomeGenerator

        ReadOnly shuffleCircosColors As New [Default](Of String())(Function() CircosColor.AllCircosColors.Shuffles, isLazy:=False)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="chrs"></param>
        ''' <param name="colors">
        ''' 如果这个参数为空值的话，则默认使用随机的<see cref="CircosColor.AllCircosColors"/>
        ''' </param>
        ''' <returns></returns>
        Public Function FromNts(chrs As IEnumerable(Of FastaSeq), Optional colors As String() = Nothing) As KaryotypeChromosomes
            Dim chrVector As FastaSeq() = chrs.ToArray
            Dim ks As Karyotype() = chrVector.chromosomes(colors Or shuffleCircosColors).ToArray

            With ks.VectorShadows
                .nt = chrVector
            End With

            Return New KaryotypeChromosomes(ks)
        End Function

        <Extension>
        Private Function chromosomes(chrVector As FastaSeq(), colors$()) As IEnumerable(Of Karyotype)
            Return From nt As SeqValue(Of FastaSeq)
                   In chrVector.SeqIterator(offset:=1)
                   Let fasta = nt.value
                   Let name As String = fasta.Title _
                       .Split("."c) _
                       .First _
                       .NormalizePathString(True) _
                       .Replace(" ", "_")
                   Let clInd As Integer = randf.NextInteger(colors.Length)
                   Select New Karyotype With {
                       .chrName = "chr" & nt.i,
                       .chrLabel = name,
                       .color = colors(clInd),
                       .start = 0,
                       .end = fasta.Length
                   }
        End Function

        ''' <summary>
        ''' Creates the model for the multiple chromosomes genome data in circos.(使用这个函数进行创建多条染色体的)
        ''' </summary>
        ''' <param name="source">Band数据</param>
        ''' <param name="chrs">karyotype数据</param>
        ''' <returns></returns>
        Public Function FromBlastnMappings(source As IEnumerable(Of BlastnMapping), chrs As IEnumerable(Of FastaSeq)) As KaryotypeChromosomes
            Dim ks As KaryotypeChromosomes = FromNts(chrs)
            Dim labels As Dictionary(Of String, Karyotype) = ks.Karyotypes.ToDictionary(Function(x) x.nt.value.Title, Function(x) x)
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
                                                       Dim map As BlastnMapping = x.MapsRaw.value
                                                       Dim nt As SimpleSegment = nts(map.Reference)
                                                       Dim fragment As FastaSeq = nt _
                                                          .CutSequenceLinear(map.MappingLocation) _
                                                          .SimpleFasta(map.ReadQuery)

                                                       Return fragment
                                                   End Function

            Dim props = bands.Select(getNT).PropertyMaps
            Dim gc#

            For Each band As Band In bands
                gc = props.props(band.MapsRaw.value.ReadQuery).value
                band.color = props.GC(gc)
            Next

            Return ks.AddBands(bands.OrderBy(Function(x) x.chrName))
        End Function

        <Extension>
        Private Function createBands(reads As BlastnMapping(), labels As Dictionary(Of String, Karyotype)) As IEnumerable(Of Band)
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
    End Module
End Namespace