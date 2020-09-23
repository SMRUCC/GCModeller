#Region "Microsoft.VisualBasic::d071dc3e39c7eec2443bd3a949730d54, analysis\RNA-Seq\TSSsTools\Transcriptome.UTRs\IdentifyUTRs.vb"

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

    '     Module IdentifyUTRs
    ' 
    '         Function: __assignRNA, __createAssociationInfo, __dataPartitionings, __genomeAssumption, __getPoissonPDF
    '                   __setBoundary, __setStart, __site, __testSites, GenomicsContext
    '                   getUTR_length, identifyUTRs, siRNAPredictes, TestSites
    ' 
    '         Sub: __analysis
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.DocumentFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel
Imports sys = System.Math

Namespace Transcriptome.UTRs

    ''' <summary>
    ''' For each gene, identify its 5'UTR and 3'UTR based on the expression data.
    ''' </summary>
    ''' 
    <Package("Transcriptome.UTRs",
                  Description:="To generate a transcriptome map based on reads from an RNA-seq experiment, a multi-step approach is used. 
                  
<p>First, a set of transcript seeds is identified corresponding to annotated genes and to novel transcript seeds. Novel transcript seeds are genomic regions at least w nucleotides in length <strong>(w is 10 by default)</strong> 
                  such that every nucleotide in the region has at least T reads mapping to the nucleotide, where the threshold T is a function of the average number of reads per nucleotide throughout the genome.
                  Novel transcript seeds are maximal, i.e. the number of reads mapping to the nucleotide immediately upstream and to the nucleotide immediately downstream of a novel transcript seed is less than T. 
                  Transcript seeds correspond to genomic regions rather than RNA transcripts.
                  Each transcript seed is then extended, upstream and downstream, using a Bayesian approach. Let s refer to a transcript seed and r be a genomic region consisting of one or more nucleotides adjacent to s. 
                  Our goal is to determine whether r corresponds to part of the same transcript as s, i.e. whether s should be extended to include r. 
                  
                  Using Bayes’ theorem, we have<br />
<li><strong>p(C|xr) = p(C)p(xr|C) / p(xr)</strong></li><br />
where xr is the number of reads mapping to r and C is a dependent class variable with two outcomes, C={c[r<->s],c[r|s]} with c[r<->s] corresponding to r and s being cotranscribed and c[r|s] corresponding to r and s not being co-transcribed. 
                  Following others (30–32), the probability p(xr | c[r<->s]) is determined by fitting a Poisson distribution to the number of reads mapping to s, based on the assumption that reads are sampled uniformly and independently.
                  The probability p(xr | c[r|s]) is determined from a background geometric distribution based on the number of reads mapping antisense throughout the genome to annotated protein coding genes. 
                  Using the maximum a posteriori estimate, the seed s is extended to include r, or not, based on <br />
<br />
<li><strong>arg max  p(C=c)p(xr|C=c)</strong></li> <br />
<li><strong>c<-{C[r<->s], C[r|s]}</strong></li> <br />
                <br />
which is equivalent to the maximum likelihood estimate, as uniform prior probabilities are assumed. 
                  <p>Finally, after each transcript seed has been extended, adjacent or overlapping transcript seeds are merged if the distributions of reads across two adjacent or overlapping transcript seeds are significantly similar. 
                  The transcriptome map reported by Rockhopper corresponds to the set of merged, extended transcript seeds.",
                  Cites:="McClure, R., et al. (2013). ""Computational analysis of bacterial RNA-Seq data."" Nucleic Acids Res 41(14): e140.
<p>	Recent advances in high-throughput RNA sequencing (RNA-seq) have enabled tremendous leaps forward in our understanding of bacterial transcriptomes. However, computational methods for analysis of bacterial transcriptome data have not kept pace with the large and growing data sets generated by RNA-seq technology. Here, we present new algorithms, specific to bacterial gene structures and transcriptomes, for analysis of RNA-seq data. The algorithms are implemented in an open source software system called Rockhopper that supports various stages of bacterial RNA-seq data analysis, including aligning sequencing reads to a genome, constructing transcriptome maps, quantifying transcript abundance, testing for differential gene expression, determining operon structures and visualizing results. We demonstrate the performance of Rockhopper using 2.1 billion sequenced reads from 75 RNA-seq experiments conducted with Escherichia coli, Neisseria gonorrhoeae, Salmonella enterica, Streptococcus pyogenes and Xenorhabdus nematophila. We find that the transcriptome maps generated by our algorithms are highly accurate when compared with focused experimental data from E. coli and N. gonorrhoeae, and we validate our system's ability to identify novel small RNAs, operons and transcription start sites. Our results suggest that Rockhopper can be used for efficient and accurate analysis of bacterial RNA-seq data, and that it can aid with elucidation of bacterial transcriptomes.",
                  Publisher:="amethyst.asuka@gcmodeller.org")>
    Public Module IdentifyUTRs

        ''' <summary>
        ''' 这个函数则会将TSSs和TTS组装在一个构成完整的基因结构的信息
        ''' </summary>
        ''' <param name="Sites">includes TSSs/TTS</param>
        ''' <param name="PTT"></param>
        ''' <returns></returns>
        <ExportAPI("Genomics.Context", Info:="Assembling the TSSs site information with the related gene and export the gene structure information. Please notices that this function just only associates the upstream site information")>
        Public Function GenomicsContext(Sites As IEnumerable(Of DocumentFormat.Transcript),
                                        PTT As PTT,
                                        <Parameter("ATG.Distance")> Optional ATGDistance As Integer = 2000) As DocumentFormat.Transcript()
            'Dim LQuery = (From site As Transcript
            '              In Sites
            '              Where site.TSSsOverlapsATG  ' 在富集的位点的分析之中，上游的位点经过计算已经不能向上游延伸了，则可能是一个TSSs位点，由于不能够向上游延伸，所以表现为位点的ATG和TSSs位点是重合的
            '              Select site,
            '                  genes = PTT.GetRelatedUpstream(
            '                  PTT.GetStrandGene(site.MappingLocation.Strand), site.MappingLocation, ATGDistance)).ToArray
            'Dim TSSs = (From site In LQuery.AsParallel Select __createAssociationInfo(site.site, site.genes)).ToArray.MatrixToVector
            'Return TSSs

            Throw New NotImplementedException
        End Function

        Private Function __createAssociationInfo(site As Transcript, genes As Relationship(Of GeneBrief)()) As DocumentFormat.Transcript()
            Dim setValue = New SetValue(Of DocumentFormat.Transcript) <= NameOf(DocumentFormat.Transcript.Position)
            Dim LQuery = (From gene As Relationship(Of GeneBrief)
                          In genes
                          Let pos As String = gene.Relation.ToString
                          Let transcript As Transcript = Transcript.CreateObject(Of Transcript)(gene.Gene)
                          Select setValue(transcript, pos)).ToArray
            LQuery = (From gene In LQuery Select __setStart(gene, site.TSSs)).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' For each gene, identify its 5'UTR and 3'UTR based on the expression data.(使用现有的基因组上下文数据)
        ''' </summary>
        ''' <param name="Transcripts">File path of the RNA-seq <see cref="DocumentFormat.Transcript"/> csv document</param>
        ''' <param name="minExpression">0-1之间的一个数</param>
        <ExportAPI("IdentifyUTRs", Info:="For each gene, identify its 5'UTR and 3'UTR based on the expression data.")>
        Public Function identifyUTRs(genome As SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,
                                     unstranded As Boolean,
                                     <Parameter("Transcripts.Csv")> Transcripts As String,
                                     Optional readsShared As Integer = 30,
                                     Optional minExpression As Double = 0.5,
                                     Optional prefix As String = "TSSs_") As DocumentFormat.Transcript()

            Dim readsRaw As ReadsCount() = ReadsCount.LoadDb(Transcripts)
            Dim replicate As Replicate = New Replicate(genome.Size, unstranded, readsRaw)

            replicate.minExpression = minExpression

            Dim genes = __testSites(genome, replicate, unstranded)
            Dim readsHash = readsRaw.ToDictionary(Function(x) x.Index)

            ' 读取Reads和TSS位点的Shared
            ' 假设计算所得到的TSSs是最远的那一个TSSs
            ' 则在5'UTR里面，所有超过30条reads的共享位点都看作潜在的TSSs位点，这个可以么？
            Dim altList As New List(Of Transcript)

            For Each transcript As DocumentFormat.Transcript In genes
                If transcript.MappingLocation.Strand = Strands.Forward Then
                    For i As Integer = transcript.TSSs + 1 To transcript.ATG
                        If Not readsHash.ContainsKey(i) Then
                            Continue For
                        End If

                        Dim lociReadsRaw = readsHash(i)
                        If lociReadsRaw.SharedPlus >= readsShared Then
                            ' 这里有一个可能的TSSs位点？？？
                            Dim alt = transcript.Copy(Of Transcript)
                            alt.Left = i
                            alt.TSSsShared = lociReadsRaw.SharedPlus
                            Call altList.Add(alt)
                        End If
                    Next

                    If readsHash.ContainsKey(transcript.TSSs) Then
                        transcript.TSSsShared = readsHash(transcript.TSSs).SharedPlus
                    End If
                Else
                    For i As Integer = transcript.ATG + 1 To transcript.TSSs
                        If Not readsHash.ContainsKey(i) Then
                            Continue For
                        End If

                        Dim lociReadsRaw = readsHash(i)
                        If lociReadsRaw.SharedMinus >= readsShared Then
                            ' 这里有一个可能的TSSs位点？？？
                            Dim alt = transcript.Copy(Of Transcript)
                            alt.Right = i
                            alt.TSSsShared = lociReadsRaw.SharedMinus
                            Call altList.Add(alt)
                        End If
                    Next

                    If readsHash.ContainsKey(transcript.TSSs) Then
                        transcript.TSSsShared = readsHash(transcript.TSSs).SharedMinus
                    End If
                End If
            Next

            Call altList.Add(genes)

            genes = altList.ToArray
            genes = (From x In genes Select x Order By x.Synonym Ascending).ToArray
            genes = genes.AssignTSSsId(prefix)
            genes = Transcript.MergeJason(genes, 3)
            genes = __assignRNA(genes)

            Return genes
        End Function

        Private Function __assignRNA(source As Transcript()) As Transcript()
            Dim list = source.AsList

            source = (From x In source Where Not String.IsNullOrEmpty(x.TSS_ID) Select x).ToArray
            source = (From x In source Where x.IsPossibleRNA Select x).ToArray

            For i As Integer = 0 To source.Length - 1
                ' 将原有的对象的信息清除，然后添加新构造的RNA分析
                Dim tr = source(i)
                Dim RNA = tr.Copy(Of Transcript)
                RNA.ATG = 0
                RNA.TGA = 0
                RNA.Synonym = "PredictedRNA_" & STDIO.ZeroFill(i + 1, 4)

                Call list.Add(RNA)

                If tr.MappingLocation.Strand = Strands.Forward Then
                    tr.Left = tr.ATG
                    tr.Right = tr.TGA
                    tr.TSS_ID = ""
                Else
                    tr.Left = tr.TGA
                    tr.Right = tr.ATG
                    tr.TSS_ID = ""
                End If
            Next

            Return list.ToArray
        End Function

        ''' <summary>
        ''' 和TSS不同的是，小RNA分子的表达量一般较低，所以在这里对原始数据是反向筛选的
        ''' </summary>
        ''' <param name="readsCount"></param>
        ''' <param name="genomeSize"></param>
        ''' <param name="unstranded"></param>
        ''' <param name="sharedReads"></param>
        ''' <param name="minIGD"></param>
        ''' <returns></returns>
        <ExportAPI("Identify.siRNA",
                   Info:="Due to the reason of the siRNA in the most situation is on the lower expression level, so that in this prediction method unlike the TSSs prediction method, 
                   the shared reads number was used to filtering the lower expression sites for the siRNA predictions. ")>
        Public Function siRNAPredictes(<Parameter("Transcripts", "This data should be a original reads count data for each site from the mapping data result.")>
                                       readsCount As Generic.IEnumerable(Of ReadsCount),
                                  <Parameter("genome.Size", "The total nt length of the target bacterial genome.")> genomeSize As Long,
                                  unstranded As Boolean,
                                  <Parameter("Shared.Reads.Max")> Optional sharedReads As Integer = 30, <Parameter("Shared.Reads.Min")> Optional sharedReadsMin As Integer = 5,
                                  <Parameter("IG.min", "The minimal intergenic length for determined the extendible of transcripts region.")> Optional minIGD As Integer = 500) As DocumentFormat.Transcript()
            Call $"Start of testing of the siRNA sites,  parameters a show below: ".__DEBUG_ECHO
            Call $"  {NameOf(genomeSize)}   := {genomeSize}".__DEBUG_ECHO
            Call $"  {NameOf(unstranded)}   := {unstranded}".__DEBUG_ECHO
            Call $"  {NameOf(sharedReads)}  := {sharedReads}".__DEBUG_ECHO
            Call $"  {NameOf(minIGD)}       := {minIGD}".__DEBUG_ECHO

            Dim sw = Stopwatch.StartNew
            Call $"Start to creates replicate data...".__DEBUG_ECHO
            Dim replicate As New Replicate(genomeSize, unstranded, readsCount)
            Call $"Job done in {sw.ElapsedMilliseconds}ms....".__DEBUG_ECHO

            sw = Stopwatch.StartNew
            Call $"Start to data partitioning of your predicted sites data....".__DEBUG_ECHO
            Dim genomes As SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT =
                __dataPartitionings(readsCount, sharedReads:=sharedReads,
                                    sharedReadsMin:=sharedReadsMin,
                                    genomeSize:=genomeSize,
                                    readsLen:=minIGD,
                                    unstrand:=unstranded,
                                    siRNAPredicts:=True)
            Call Console.WriteLine()
            Call $"Partitioning {genomes.Size} data partition in {sw.ElapsedMilliseconds}ms...".__DEBUG_ECHO

            sw = Stopwatch.StartNew
            Call $"Start testing of the predicted sites for the transcription boundary....".__DEBUG_ECHO
            Dim LQuery = __testSites(genomes, replicate, unstranded)
            Call $"job done in {sw.ElapsedMilliseconds}ms...".__DEBUG_ECHO
            Call "Filtering boundary sites data....".__DEBUG_ECHO
            LQuery = (From obj In LQuery.AsParallel Where obj.BoundaryOverlaps Select obj).ToArray '只筛选出不会发生变化的位点
            Call $"Left {LQuery.Length} boundary sites data which was testing from the rockhopper....".__DEBUG_ECHO
            Return LQuery.ToArray
        End Function

        ''' <summary>
        ''' Testing if the site can be identified as a TSSs.(使用种子来鉴定)
        ''' </summary>
        ''' <param name="Transcripts"></param>
        ''' <returns>程序会尝试延伸，假若不能够继续延伸，则认为是转录边界
        ''' 由于序列片段之间会存在重叠的情况，所以在计算之前需要先分区，将序列分割为非重叠的状态，即序列片段之间的最小距离要满足一个用户自定义的基因间隔区的最小距离
        ''' 
        ''' </returns>
        <ExportAPI("TSSs.Test", Info:="Testing if the site can be identified as a TSSs. Only the sites which is testing successfully will be output.")>
        Public Function TestSites(<Parameter("Transcripts", "This data should be a original merged result.")>
                                  Transcripts As Generic.IEnumerable(Of ReadsCount),
                                  <Parameter("genome.Size", "The total nt length of the target bacterial genome.")> genomeSize As Long,
                                  unstranded As Boolean,
                                  <Parameter("Shared.Reads")> Optional sharedReads As Integer = 30,
                                  <Parameter("IG.min", "The minimal intergenic length for determined the extendible of transcripts region.")>
                                  Optional minIGD As Integer = 500) As DocumentFormat.Transcript()
            Call $"Start of testing of the TSSs sites,  parameters a show below: ".__DEBUG_ECHO
            Call $"  {NameOf(genomeSize)}   := {genomeSize}".__DEBUG_ECHO
            Call $"  {NameOf(unstranded)}   := {unstranded}".__DEBUG_ECHO
            Call $"  {NameOf(sharedReads)}  := {sharedReads}".__DEBUG_ECHO
            Call $"  {NameOf(minIGD)}       := {minIGD}".__DEBUG_ECHO

            Dim sw = Stopwatch.StartNew
            Call $"Start to creates replicate data...".__DEBUG_ECHO
            Dim replicate As New Replicate(genomeSize, unstranded, Transcripts)
            Call $"Job done in {sw.ElapsedMilliseconds}ms....".__DEBUG_ECHO

            sw = Stopwatch.StartNew
            Call $"Start to data partitioning of your predicted sites data....".__DEBUG_ECHO
            Dim genome As SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT =
                __dataPartitionings(Transcripts,
                                    sharedReads:=sharedReads,
                                    sharedReadsMin:=-1,
                                    genomeSize:=genomeSize,
                                    readsLen:=minIGD,
                                    unstrand:=unstranded,
                                    siRNAPredicts:=False)

            Call Console.WriteLine()
            Call $"Partitioning {genome.Size} data partition in {sw.ElapsedMilliseconds}ms...".__DEBUG_ECHO

            sw = Stopwatch.StartNew
            Call $"Start testing of the predicted sites for the transcription boundary....".__DEBUG_ECHO
            Dim LQuery = __testSites(genome, replicate, unstranded).ToArray
            Call $"job done in {sw.ElapsedMilliseconds}ms...".__DEBUG_ECHO
            Call "Filtering boundary sites data....".__DEBUG_ECHO

            Dim readsHash = Transcripts.ToDictionary(Function(x) x.Index)
            Dim setValue As New SetValue(Of Transcript)

            LQuery = LinqAPI.Exec(Of Transcript) <=
                From loci As DocumentFormat.Transcript
                In LQuery.AsParallel
                Let rawReads As Integer =
                    replicate.getReads(loci.TSSs, loci.MappingLocation.Strand.GetBriefCode.First)
                Let TSSsharedReads As Integer =
                    If(Not readsHash.ContainsKey(loci.TSSs),
                    0,
                    If(loci.MappingLocation.Strand = Strands.Forward,
                    readsHash(loci.TSSs).SharedPlus,
                    readsHash(loci.TSSs).SharedMinus))
                Where Not (loci.ATG = loci.TSSs AndAlso loci.TGA = loci.TTSs) AndAlso
                    TSSsharedReads >= sharedReads
                Select setValue _
                    .InvokeSetValue(loci, NameOf(loci.Raw), rawReads) _
                    .InvokeSet(NameOf(loci.TSSsShared), TSSsharedReads).obj '只筛选出发生了变化了的

            Call $"Left {LQuery.Length} boundary sites data which was testing from the rockhopper....".__DEBUG_ECHO
            Return LQuery.ToArray
        End Function

        Private Function __testSites(genome As PTT,
                                     replicate As Replicate,
                                     unstranded As Boolean) As DocumentFormat.Transcript()
            Dim genomeCoordinates As SortedDictionary(Of String, DocumentFormat.Transcript) =
                New SortedDictionary(Of String, DocumentFormat.Transcript)(genome.GeneObjects.ToDictionary(
                    Function(obj) obj.Synonym,
                    Function(obj) DocumentFormat.Transcript.CreateObject(Of DocumentFormat.Transcript)(obj)))
            Dim genes = (From obj In genomeCoordinates.Values Select obj Order By obj.MappingLocation.Left Ascending).ToArray

            For x As Integer = 0 To genome.NumOfProducts
                Call __analysis(x, unstranded, genome.NumOfProducts, genome.Size, genes, genomeCoordinates, replicate)
            Next

            For Each gene As DocumentFormat.Transcript In genes
                If gene.MappingLocation.Strand = Strands.Reverse Then
                    'Call gene.Left.SwapWith(gene.Right) ’ 需要调用这个方法强制左右位置的交换
                End If

                Dim arrayLoci = {gene.ATG, gene.TGA}
                Dim reads = replicate.getReadsInRange(arrayLoci.Min, arrayLoci.Max, gene.MappingLocation.Strand.GetBriefCode)
                reads = reads / Math.Abs(gene.ATG - gene.TGA)
                gene.Raw = reads
            Next

            Call Console.Write(".")

            Return genes
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Transcripts"></param>
        ''' <param name="sharedReads"></param>
        ''' <param name="genomeSize"></param>
        ''' <param name="readsLen"></param>
        ''' <param name="unstrand"></param>
        ''' <param name="siRNAPredicts">筛选的模式会反转</param>
        ''' <returns></returns>
        Private Function __dataPartitionings(Transcripts As IEnumerable(Of ReadsCount),
                                             sharedReads As Integer,
                                             sharedReadsMin As Integer,
                                             genomeSize As Long,
                                             readsLen As Integer,
                                             unstrand As Boolean,
                                             siRNAPredicts As Boolean) As PTT

            Dim Trim As List(Of ReadsCount)

            If Not siRNAPredicts Then
                Trim = (From obj In Transcripts.AsParallel
                        Where obj.SharedMinus >= sharedReads OrElse obj.SharedPlus >= sharedReads
                        Select obj
                        Order By obj.Index Ascending).AsList
            Else 'siRNA一般情况下都是低表达量的
                Call $"Filtering lower expression sites ({NameOf(sharedReads)} <= {sharedReads}) for predicts siRNA...".__DEBUG_ECHO

                Trim = (From obj In Transcripts.AsParallel
                        Where obj.ReadsPlus <= sharedReads AndAlso obj.ReadsMinus <= sharedReads AndAlso (obj.SharedPlus > sharedReadsMin OrElse obj.SharedMinus > sharedReadsMin)
                        Select obj
                        Order By obj.Index Ascending).AsList
            End If

            Dim Partitions As List(Of DocumentFormat.Transcript) = (From x As ReadsCount In Trim.AsParallel Select __site(x, readsLen, sharedReads)).Unlist
            Partitions = (From x In Partitions Where Not x Is Nothing Select x).AsList
            Dim genomeSeeds = __genomeAssumption(Partitions, genomeSize)
            Return genomeSeeds
        End Function

        Private Function __site(start As ReadsCount, readsLen As Integer, sharedReads As Integer) As DocumentFormat.Transcript()
            Dim plus As DocumentFormat.Transcript = Nothing
            Dim minus As DocumentFormat.Transcript = Nothing

            If start.SharedPlus >= sharedReads Then
                plus = New DocumentFormat.Transcript With {
                    .ATG = start.Index,
                    .Left = start.Index,
                    .Right = start.Index + readsLen,
                    .TGA = start.Index + readsLen,
                    .Strand = "+"
                }
            End If
            If start.SharedMinus >= sharedReads Then
                minus = New DocumentFormat.Transcript With {
                    .ATG = start.Index,
                    .Right = start.Index,
                    .Left = start.Index - readsLen,
                    .TGA = start.Index - readsLen,
                    .Strand = "-"
                }
            End If

            Return {plus, minus}
        End Function

        ''' <summary>
        ''' 从转录组数据之中生成假基因
        ''' </summary>
        ''' <param name="Transcripts"></param>
        ''' <returns></returns>
        Private Function __genomeAssumption(Transcripts As IEnumerable(Of DocumentFormat.Transcript), genomeSize As Long) As PTT
            '生成ID编号
            Dim Genes = (From i As Integer In Transcripts.Sequence.AsParallel  '42..1370	+	442	66766353	dnaA	XC_0001	-	COG0593L	chromosome replication initiator DnaA
                         Let site = Transcripts(i)
                         Let sId As String = "FkTSSs_" & i
                         Select assumption = New GeneBrief With {
                             .Code = "-",
                             .COG = "-",
                             .Gene = sId,
                             .IsORF = True,
                             .Length = site.Length,
                             .Location = site.MappingLocation,
                             .PID = i,
                             .Product = "Fake_" & i,
                             .Synonym = sId
                         }
                         Order By assumption.Location.Left Ascending).ToArray
            Dim genome As New SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT With {
                .Size = genomeSize,
                .Title = NameOf(__genomeAssumption),
                .GeneObjects = Genes
            }
            Return genome
        End Function

        ''' <summary>
        ''' For each IG region
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="unstranded"></param>
        ''' <param name="replicate"></param>
        ''' <param name="genome">真实的基因组上下文或者假基因种子，请注意ORF一定要填满ATG和TGA位点的值</param>
        Private Sub __analysis(x As Integer, unstranded As Boolean,
                               NumOfProducts As Integer,
                               genomeSize As Long,
                               genome As DocumentFormat.Transcript(),
                               ByRef geneCoordinates As SortedDictionary(Of String, DocumentFormat.Transcript),
                               ByRef replicate As Replicate)

            Dim gPrevious As DocumentFormat.Transcript
            Dim gNext As DocumentFormat.Transcript

            ' Identify downstream gene and IG stop coordinate
            If x = NumOfProducts Then
                Dim sLine As String = (genomeSize - 1) & ".." & (genomeSize - 1) & vbTab & genome(x - 1).Strand & vbTab & "0" & vbTab & "-" & vbTab & "???" & vbTab & "???" & vbTab & "-" & vbTab & "-" & vbTab & "???"
                gNext = DocumentFormat.Transcript.CreateObject(Of DocumentFormat.Transcript)(DocumentParser(sLine))
            Else
                gNext = genome(x)
            End If
            Dim [stop] As Integer = sys.Min(gNext.ATG, gNext.TGA) - 1
            'If Not gNext.IsORF Then
            '    [stop] = gNext.MappingLocation.Left - 1
            'End If

            ' Identify upstream gene and IG start coordinate
            Dim start As Integer = 1
            Dim upstreamGeneIndex As Integer = x - 1
            While upstreamGeneIndex >= 0
                If unstranded Then                    ' Strand ambiguous
                    start = Math.Max(genome(upstreamGeneIndex).ATG, genome(upstreamGeneIndex).TGA) + 1
                    'If Not genome(upstreamGeneIndex).IsORF Then
                    '    start = genome(upstreamGeneIndex).MappingLocation.Right + 1
                    'End If
                    Exit While

                Else                    ' Strand specific
                    If gNext.Strand = genome(upstreamGeneIndex).Strand Then
                        start = Math.Max(genome(upstreamGeneIndex).ATG, genome(upstreamGeneIndex).TGA) + 1
                        'If Not genome(upstreamGeneIndex).IsORF Then
                        '    start = genome(upstreamGeneIndex).MappingLocation.Right + 1
                        'End If
                        Exit While
                    End If
                    upstreamGeneIndex -= 1
                End If
            End While
            If upstreamGeneIndex < 0 Then
                Dim sLine As String = 1 & ".." & 1 & vbTab & gNext.Strand & vbTab & "0" & vbTab & "-" & vbTab & "???" & vbTab & "???" & vbTab & "-" & vbTab & "-" & vbTab & "???"
                gPrevious = DocumentFormat.Transcript.CreateObject(Of DocumentFormat.Transcript)(DocumentParser(sLine))
            Else
                gPrevious = genome(upstreamGeneIndex)
            End If

            Dim IG_length As Integer = [stop] - start + 1
            If IG_length <= 0 Then
                ' No action necessary if there is no IG region.
                Return
            End If

            Dim frontUTR_length_merged As Integer = -1
            Dim backUTR_length_merged As Integer = -1

            ' Get UTR lengths
            Dim frontUTR_length As Integer = getUTR_length(gPrevious, start, [stop], replicate, True, genomeSize, unstranded)
            Dim backUTR_length As Integer = getUTR_length(gNext, start, [stop], replicate, False, genomeSize, unstranded)

            ' Distinguish overlapping UTRs
            If (frontUTR_length > 0) AndAlso
                (backUTR_length > 0) AndAlso
                (Math.Max(gPrevious.ATG, gPrevious.TGA) + frontUTR_length >= sys.Min(gNext.ATG, gNext.TGA) - backUTR_length) Then

                ' Determine mean of genes' expression
                Dim mean1 As Double = replicate.getMeanOfRange(gPrevious.ATG, gPrevious.TGA, gPrevious.Strand)
                Dim mean2 As Double = replicate.getMeanOfRange(gNext.ATG, gNext.TGA, gNext.Strand)
                If unstranded Then
                    mean1 = replicate.getMeanOfRange(gPrevious.ATG, gPrevious.TGA, "?"c)
                    mean2 = replicate.getMeanOfRange(gNext.ATG, gNext.TGA, "?"c)
                End If

                Dim overlapStart As Integer = sys.Min(gNext.ATG, gNext.TGA) - backUTR_length
                Dim overlapStop As Integer = Math.Max(gPrevious.ATG, gPrevious.TGA) + frontUTR_length
                'while ((overlapStart < sys.Min(g2.getStart(), g2.getStop())) && (getPoissonPDF(r.getReads(overlapStart, g1.getStrand()), mean1) > getPoissonPDF(r.getReads(overlapStart, g2.getStrand()), mean2))) overlapStart++;
                If Not unstranded Then
                    ' Strand specific
                    While (overlapStart <= overlapStop) AndAlso (Math.Abs(replicate.getReads(overlapStart, gPrevious.Strand) - mean1) < Math.Abs(replicate.getReads(overlapStart, gNext.Strand) - mean2))
                        overlapStart += 1
                    End While
                    'while ((overlapStop > Math.max(g1.getStart(), g1.getStop())) && (getPoissonPDF(r.getReads(overlapStop, g1.getStrand()), mean1) < getPoissonPDF(r.getReads(overlapStop, g2.getStrand()), mean2))) overlapStop--;
                    While (overlapStop >= overlapStart) AndAlso (Math.Abs(replicate.getReads(overlapStop, gPrevious.Strand) - mean1) > Math.Abs(replicate.getReads(overlapStop, gNext.Strand) - mean2))
                        overlapStop -= 1
                    End While
                Else
                    ' Strand ambiguous
                    While (overlapStart <= overlapStop) AndAlso (Math.Abs(replicate.getReads(overlapStart, "?"c) - mean1) < Math.Abs(replicate.getReads(overlapStart, "?"c) - mean2))
                        overlapStart += 1
                    End While
                    While (overlapStop >= overlapStart) AndAlso (Math.Abs(replicate.getReads(overlapStop, "?"c) - mean1) > Math.Abs(replicate.getReads(overlapStop, "?"c) - mean2))
                        overlapStop -= 1
                    End While
                End If
                If overlapStop - overlapStart < -1 Then
                    Call "Error - overlapping UTR region has invalid size.".__DEBUG_ECHO
                    ' Do nothing. There is no overlap.
                ElseIf overlapStop - overlapStart = -1 Then
                Else
                    ' We still have overlapping UTR regions
                    Dim mean_of_overlap As Double = replicate.getMeanOfRange(overlapStart, overlapStop, gPrevious.Strand)
                    If unstranded Then
                        mean_of_overlap = replicate.getMeanOfRange(overlapStart, overlapStop, "?"c)
                    End If
                    If Math.Abs(mean_of_overlap - mean1) <= Math.Abs(mean_of_overlap - mean2) Then
                        overlapStart = overlapStop + 1
                    Else
                        overlapStop = overlapStart - 1
                        'double prob1 = 1.0;
                        'double prob2 = 1.0;
                        'for (int y=overlapStart; y<=overlapStop; y++) {
                        'prob1 *= getPoissonPDF(r.getReads(y, g1.getStrand()), mean1);
                        'prob2 *= getPoissonPDF(r.getReads(y, g2.getStrand()), mean2);
                        '}
                        'if (prob1 >= prob2) overlapStart = overlapStop + 1;
                        'else overlapStop = overlapStart - 1;
                    End If
                End If
                frontUTR_length = overlapStart - (Math.Max(gPrevious.ATG, gPrevious.TGA) + 1)
                backUTR_length = sys.Min(gNext.ATG, gNext.TGA) - 1 - overlapStop
            End If

            ' Merge UTRs from different experiments
            'frontUTR_length_merged = Math.max(frontUTR_length_merged, frontUTR_length);
            If frontUTR_length_merged = -1 Then
                frontUTR_length_merged = frontUTR_length
            ElseIf frontUTR_length_merged = 0 Then
                frontUTR_length_merged = Math.Max(frontUTR_length_merged, frontUTR_length)
            Else
                frontUTR_length_merged = sys.Min(frontUTR_length_merged, frontUTR_length)
            End If
            'backUTR_length_merged = Math.max(backUTR_length_merged, backUTR_length);
            If backUTR_length_merged = -1 Then
                backUTR_length_merged = backUTR_length
            ElseIf backUTR_length_merged = 0 Then
                backUTR_length_merged = Math.Max(backUTR_length_merged, backUTR_length)
            Else
                backUTR_length_merged = sys.Min(backUTR_length_merged, backUTR_length)
            End If

            '  Update transcription start/stop of genes (if genes are expressed)
            If frontUTR_length_merged >= 0 Then
                ' Gene is expressed and has flanking IG region
                If gPrevious.Strand = "+"c Then
                    geneCoordinates(gPrevious.Synonym).Right = gPrevious.TGA + frontUTR_length_merged
                ElseIf gPrevious.Strand = "-"c Then
                    geneCoordinates(gPrevious.Synonym).Left = gPrevious.ATG + frontUTR_length_merged
                End If
            End If
            If backUTR_length_merged >= 0 Then
                ' Gene is expressed and has flanking IG region
                If gNext.Strand = "+"c Then
                    geneCoordinates(gNext.Synonym).Left = gNext.ATG - backUTR_length_merged
                End If
                If gNext.Strand = "-"c Then
                    geneCoordinates(gNext.Synonym).Right = gNext.TGA - backUTR_length_merged
                End If
            End If
        End Sub

        Private Function __setStart(Transcript As DocumentFormat.Transcript, gStart As Integer) As DocumentFormat.Transcript
            If Transcript.MappingLocation.Strand = Strands.Forward Then
                Transcript.Left = gStart
            Else
                Transcript.Right = gStart
            End If

            Return Transcript
        End Function

        ''' <summary>
        ''' 
        ''' 设置左右端起始和终止的位点，非ATG和TGA
        ''' </summary>
        ''' <param name="Transcript"></param>
        ''' <param name="gStart"></param>
        ''' <param name="gStop"></param>
        ''' <returns></returns>
        Private Function __setBoundary(Transcript As DocumentFormat.Transcript,
                                       gStart As SortedDictionary(Of String, Value(Of Integer)),
                                       gStop As SortedDictionary(Of String, Value(Of Integer))) As DocumentFormat.Transcript
            If Transcript.MappingLocation.Strand = Strands.Forward Then
                Transcript.Left = gStart(Transcript.Synonym).value
                Transcript.Right = gStop(Transcript.Synonym).value
            Else
                Transcript.Right = gStart(Transcript.Synonym).value
                Transcript.Left = gStop(Transcript.Synonym).value
            End If

            Return Transcript
        End Function

        <ExportAPI("UTR.Length", Info:="Get UTR lengths of a gene based on its expression value.")>
        Public Function getUTR_length(gene As DocumentFormat.Transcript,
                                      start As Integer, [stop] As Integer,
                                      replicate As Replicate,
                                      <Parameter("Is.Front")> isFront As Boolean,
                                      genomeSize As Long,
                                      unstranded As Boolean) As Integer

            ' If we do not have a real gene but merely a place holder, do not compute UTR.
            If gene.ATG = gene.TGA Then
                Return -1
            End If

            ' If we have an RNA gene rather than an ORF, do not compute UTR.
            'If Not gene.IsORF Then
            '    Return -1
            'End If

            ' Determine strand
            Dim strand As Char = gene.Strand
            If unstranded Then
                strand = "?"c
            End If

            ' Determine mean of gene's expression
            Dim mean As Double = replicate.getMeanOfRange(gene.ATG, gene.TGA, strand)
            If mean < replicate.minExpressionUTR Then
                ' Gene is not expressed
                Return -1
            End If

            ' Compute expression distribution
            '
            '	double thresh = 1.0;
            '	if (mean >= thresh * r.getMinExpressionUTR()) {
            '	    int start2 = g.getStart();
            '	    int stop2 = g.getStop();
            '	    if (stop2 < start2) {  // Swap
            '		int temp = start2;
            '		start2 = stop2;
            '		stop2 = temp;
            '	    }
            '	    for (int i=start2; i<=stop2; i++) {
            '		int x = (int)Math.round(20000*((r.getMeanOfRange(z, i, i, strand) - mean) / stdev));
            '		if (Math.abs(x) >= distribution.length/2) x = (distribution.length/2)*(x/Math.abs(x));
            '		distribution[x+distribution.length/2]++;
            '	    }
            '	}
            '	

            'int[] IG = new int[stop-start+1+WINDOW];
            'if (isFront) {  // Front UTR
            '   for (int i=start-WINDOW/2; i<=sys.Min(stop+WINDOW/2, genome.size()-1); i++) IG[i-start+WINDOW/2] = r.getReads(i, g.getStrand());
            '} else {  // Back UTR
            '    for (int i=stop+WINDOW/2; i>=Math.max(start-WINDOW/2,1); i--) IG[stop+WINDOW/2-i] = r.getReads(i, g.getStrand());
            '}
            Dim IG As Integer() = New Integer([stop] - start) {}
            If isFront Then                ' Front UTR
                For i As Integer = start To sys.Min([stop], genomeSize - 1)
                    IG(i - start) = replicate.getReads(i, strand)
                Next
            Else                ' Back UTR
                For i As Integer = [stop] To Math.Max(start, 1) Step -1
                    IG([stop] - i) = replicate.getReads(i, strand)
                Next
            End If

            For i As Integer = 0 To IG.Length - 1
                If IG(i) = 0 Then
                    Return i
                End If
                If IG(i) >= SCALE * mean Then
                    Continue For
                End If
                If IG(i) >= SCALE * replicate.minExpressionUTR Then
                    Continue For
                End If
                If SCALE * replicate.getBackgroundProb(IG(i)) > __getPoissonPDF(IG(i), mean) Then
                    Return i
                Else
                    Continue For
                    '
                    '		// Quick check to see if we can easily classify the window as IG or UTR
                    '		int numZeros = 0;
                    '		int numExpressed = 0;
                    '		for (int j=i; j<i+WINDOW; j++) {
                    '		if (IG[j] == 0) numZeros++;
                    '		if (IG[j] >= mean) numExpressed++;
                    '		}
                    '		if (numZeros >= WINDOW/2) return i;
                    '		if (numExpressed >= WINDOW/2) continue;
                    '
                    '		// More extensive check if probability is closer to IG or UTR
                    '		double probIG = 1.0;
                    '		double probUTR = 1.0;
                    '		for (int j=i; j<i+WINDOW; j++) {
                    '		probIG *= r.getBackgroundProb(IG[j]);
                    '		probUTR *= getPoissonPDF(IG[j], mean);
                    '		}
                    '		if (probIG >= probUTR) return i;
                    '		
                End If
            Next

            Return IG.Length    'return IG.length-WINDOW;
        End Function

        Const SCALE As Double = 1.5

        ''' <summary>
        ''' Returns the PDF value at x for the specified Poisson distribution.
        ''' </summary>
        ''' 
        <ExportAPI("PoissonPDF", Info:="Returns the PDF value at x for the specified Poisson distribution.")>
        Public Function __getPoissonPDF(x As Integer, lambda As Double) As <FunctionReturns("Returns the PDF value at x for the specified Poisson distribution.")> Double
            Dim result As Double = Math.Exp(-lambda)
            Dim k As Integer = x
            While k >= 1
                result *= lambda / k
                k -= 1
            End While
            Return result
        End Function
    End Module
End Namespace
