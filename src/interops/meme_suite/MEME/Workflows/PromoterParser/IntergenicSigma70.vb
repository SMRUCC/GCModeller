#Region "Microsoft.VisualBasic::9b897e106cd80e8157899b29c7062d8e, meme_suite\MEME\Workflows\PromoterParser\IntergenicSigma70.vb"

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

    '     Module IntergenicSigma70
    ' 
    '         Function: DIPAssociation, SaveView, (+2 Overloads) Sigma70Parser
    '         Class MEME_DIP
    ' 
    '             Properties: DIP, EValue, MotifGuid, PValue, RightEndDownStream
    '                         Sequence, Signature, Site, Start
    ' 
    '         Delegate Function
    ' 
    '             Function: LoadTranscripts, (+2 Overloads) MEMEPredictedTSSsAssociations, OverlapCommon, TrimNotStrictOverlap, TrimStrictOverlap
    '                       VirtualFootprintDIP, WriteTranscripts
    '         Class VF_DIP
    ' 
    '             Properties: DIP_ENTRY, Distance, Ends, Length, LocationDescriptions
    '                         MotifFamily, MotifId, MotifLocation, ORF, ORFDirection
    '                         RNAGene, Sequence, Signature, Starts, Strand
    ' 
    '         Class Transcript
    ' 
    '             Properties: MEMEPredictedTSSs, Minus10Start, Minus10Stop
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.ContextModel.LocationDescriptions
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.LDM
Imports SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Workflows.PromoterParser

    ''' <summary>
    ''' 应用于Sigma70的预测分析的序列片段的解析模块
    ''' 
    ''' 只解析出在基因间隔区的，ORF上游100bp的序列片段，假若片段和其他的ORF发生了重叠，就缩短片段，但是不得短于25bp
    ''' </summary>
    ''' <remarks>
    ''' Initially, all 100 nt regions upstream of all protein encoding genes were selected from the genome. 
    ''' 
    ''' Subsequently, these sequences were evaluated for their potential overlap With a preceding gene, 
    ''' And In such cases, only the intergenic sequence was used For analysis, provided they
    ''' were 25 nt Or longer.
    ''' 
    ''' The resulting Set Of selected intergenic sequences was searched For conserved motifs using MEME, applying standard DNA
    ''' parameter settings. 
    ''' 
    ''' Only motifs reported by MEME With E-values below 10-4 were considered relevant for further analysis.
    ''' 
    ''' Additional parameters used For selection Of candidate sequences were: 
    ''' 
    ''' + 1) Zero Or One Occurrence Per Sequence (ZOOPS mode),
    ''' + 2) a maximum of ten different motifs per sequence, And 
    ''' + 3) each motif should be found In at least thirty-five different sequences.
    ''' 
    ''' PWM models were constructed For the most abundantly encountered motifs, including those resembling the canonical 235 And
    ''' 210 elements known from general s70-dependent promoters.
    ''' 
    ''' 黄单胞菌应该长一点150bp，非严格重叠？？？？
    ''' </remarks>
    ''' 
    <Package("Sigma70", Description:="Initially, all 100 nt regions upstream of all protein encoding genes were selected from the genome. 
Subsequently, these sequences were evaluated for their potential overlap With a preceding gene, 
And In such cases, only the intergenic sequence was used For analysis, provided they were 25 nt Or longer.

The resulting Set Of selected intergenic sequences was searched For conserved motifs using MEME, applying standard DNA parameter settings. 

Only motifs reported by MEME With E-values below 10-4 were considered relevant for further analysis.

Additional parameters used For selection Of candidate sequences were: 
<br />
<li> 1) Zero Or One Occurrence Per Sequence (ZOOPS mode),
<li> 2) a maximum of ten different motifs per sequence, And 
<li> 3) each motif should be found In at least thirty-five different sequences.

PWM models were constructed For the most abundantly encountered motifs, including those resembling the canonical 235 And
210 elements known from general s70-dependent promoters.",
                        Publisher:="xie.guigang@gcmodeller.org",
                        Url:="http://gcmodeller.org")>
    Public Module IntergenicSigma70

        Public Class MEME_DIP

            Public Property Signature As String
            Public Property Sequence As String
            Public Property Start As Integer
            Public Property RightEndDownStream As Integer
            Public Property EValue As Double
            Public Property PValue As Double
            Public Property MotifGuid As String
            <Column("Site")> Public Property Site As String
            Public Property DIP As String
            ' Public Property LociLeft As Integer
            ' Public Property LociRight As Integer
            ' Public Property Strand As String
        End Class

        <ExportAPI("Write.Csv.MEME_DIP")>
        Public Function SaveView(data As IEnumerable(Of MEME_DIP), saveTo As String) As Boolean
            Return data.SaveTo(saveTo, False)
        End Function

        ''' <summary>
        ''' The canonical sigma factor sigma70 is commonly involved in transcription of the cell’s housekeeping genes, which is mediated by the conserved sigma70 promoter sequence motifs
        ''' </summary>
        ''' <param name="MEMECSV"></param>
        ''' <param name="DIPCsv"></param>
        ''' <returns></returns>
        <ExportAPI("MEME.DIP",
                   Info:="The canonical sigma factor sigma70 is commonly involved in transcription of the cell’s housekeeping genes, 
                   which is mediated by the conserved sigma70 promoter sequence motifs.")>
        Public Function DIPAssociation(MEMECSV As String, DIPCsv As String) As MEME_DIP()
            Dim MEME = MEMECSV.LoadCsv(Of MEME_DIP)(False).ToArray,
                DIP = (From row As MEME_DIP In DIPCsv.LoadCsv(Of MEME_DIP)(False)
                       Select row
                       Group row By row.Site Into Group) _
                            .ToDictionary(Function(x) x.Site,
                                          Function(x) x.Group.ToArray)
            Dim setValue = New SetValue(Of MEME_DIP)() <= NameOf(MEME_DIP.DIP)
            Dim LQuery As MEME_DIP() =
                LinqAPI.Exec(Of MEME_DIP) <= From row As MEME_DIP
                                             In MEME.AsParallel
                                             Let dip_value As MEME_DIP() = DIP.TryGetValue(row.Site)
                                             Let dipId As String = If(dip_value.IsNullOrEmpty,
                                                 "",
                                                 LinqAPI.DefaultFirst(Of String) <= From item As MEME_DIP
                                                                                    In dip_value
                                                                                    Where Not String.IsNullOrEmpty(item.DIP)
                                                                                    Select str = item.DIP
                                                                                    Distinct)
                                             Select setValue(row, dipId)
            Return LQuery
        End Function

        '''' <summary>
        '''' 将Motif位点重新定位回到基因组上面得位置之中
        '''' </summary>
        '''' <param name="data"></param>
        '''' <param name="PTT"></param>
        '''' <returns></returns>
        '''' 
        '<Command("Relocated")>
        'Public Function Relocated(data As Generic.IEnumerable(Of MEME_DIP), PTT As SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT) As MEME_DIP()
        '    Dim LQuery = (From Site As MEME_DIP
        '                  In data'.AsParallel
        '                  Select Relocated(Site, PTT)).ToArray
        '    Return LQuery
        'End Function

        '''' <summary>
        '''' 请注意，MEME的输出位点都是从左端开始的，对于反向的序列已经被反向互补过了
        '''' </summary>
        '''' <param name="site"></param>
        '''' <param name="PTT"></param>
        '''' <returns>
        '''' 由于在解析序列数据的时候序列会根据重叠的情况进行修剪的，故而在这里已经不能够得到精确的位点了，只能够确定这个位点在该基因的上游的100bp以内的区域
        '''' 假若还需要得到精确的位点，是否还需要进行blastn进行精确定位？？？？？？？？
        '''' </returns>
        'Private Function Relocated(site As MEME_DIP, PTT As SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT) As MEME_DIP
        '    Dim Gene = PTT.GeneObject(site.Site)

        '    If Gene.Location.Strand = Strands.Forward Then

        '    End If
        'End Function



        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="genomeOS"></param>
        ''' <param name="PTT"></param>
        ''' <param name="strictOverlap">
        ''' 当有重叠发生的时候，假若严格重叠的话，则不管重叠是发生在哪一条链之上都会认为是重叠的
        ''' 假若为非严格重叠的话，则对于不同链之上的位置重叠将会被忽略
        ''' </param>
        ''' <param name="Length">默认是文献之中所提供的100nt的长度，但是好像太短了，无法预测出-35区，建议使用150或者200bp的长度</param>
        ''' <returns></returns>
        <ExportAPI("Sigma70.Parser")>
        <Extension>
        Public Function Sigma70Parser(genomeOS As FastaSeq,
                                      PTT As PTT,
                                      Optional strictOverlap As Boolean = False,
                                      Optional length% = 100) As FastaFile

            Static trimLooseOverlap As New GetFastaToken(AddressOf TrimNotStrictOverlap)
            Static trimStrictOverlap As New GetFastaToken(AddressOf IntergenicSigma70.TrimStrictOverlap)

            Dim getFastaSegment As GetFastaToken = trimLooseOverlap Or trimStrictOverlap.When(Not strictOverlap)
            Dim LQuery = LinqAPI.Exec(Of FastaSeq) _
 _
                () <= From gene As GeneBrief
                      In PTT.GeneObjects.AsParallel
                      Let segment = getFastaSegment(genomeOS, PTT, gene, length)
                      Where Not segment Is Nothing
                      Select segment

            Return New FastaFile(LQuery)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="genomeOS">The genome origin sequence.</param>
        ''' <param name="PTT"></param>
        ''' <param name="Motifs"></param>
        ''' <param name="StrictOverlap"></param>
        ''' <param name="Length"></param>
        ''' <param name="EXPORT"></param>
        ''' <returns></returns>
        <ExportAPI("Sigma70.Parser")>
        <Extension>
        Public Function Sigma70Parser(genomeOS As FastaSeq,
                                      PTT As PTT,
                                      motifs As IEnumerable(Of Motif),
                                      Optional StrictOverlap As Boolean = False,
                                      Optional Length As Integer = 100,
                                      <Parameter("Dir.Export")> Optional EXPORT As String = "./") As Boolean

            Call "Start to parsing furthur region sequence...".__DEBUG_ECHO

            Dim LQuery = (From motif As Motif
                          In motifs.AsParallel
                          Let IDList = (From site In motif.Sites Select site.Name Distinct).ToArray
                          Let PTTPart = PTT.Copy(IDList)
                          Select motif,
                              regionFasta = genomeOS.Sigma70Parser(PTTPart, StrictOverlap, Length)).ToArray

            Call $"[Job Done!] Saved data to location {EXPORT}".__DEBUG_ECHO

            For Each Motif In LQuery
                Dim path As String = $"{EXPORT}/{Motif.motif.uid.NormalizePathString}.fasta"
                Call Motif.regionFasta.Save(LineBreak:=-1,
                                            Path:=path,
                                            encoding:=Encodings.ASCII)
            Next

            Call "[Job Done!]".__DEBUG_ECHO

            Return True
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="reader"></param>
        ''' <param name="PTT"></param>
        ''' <param name="gene"></param>
        ''' <param name="len%"></param>
        ''' <returns></returns>
        Private Delegate Function GetFastaToken(reader As IPolymerSequenceModel, PTT As PTT, gene As GeneBrief, len%) As FastaSeq

        ''' <summary>
        ''' 不管链的方向，只要发生了重叠就必须要剪裁
        ''' </summary>
        ''' <param name="Reader"></param>
        ''' <param name="PTT"></param>
        ''' <param name="GeneObject"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function TrimStrictOverlap(Reader As IPolymerSequenceModel, PTT As PTT, GeneObject As GeneBrief, Length As Integer) As FASTA.FastaSeq
            Return OverlapCommon(PTT.GeneObjects, Reader, GeneObject, Length)
        End Function

        ''' <summary>
        ''' 严格或者非严格实际上就只是数据源不同：
        ''' 
        ''' + 严格的会同时检查两条链，所以数据是整个PTT文件
        ''' + 非严格的则是与目标基因相同链的基因数据
        ''' </summary>
        ''' <param name="Genes"></param>
        ''' <param name="Reader"></param>
        ''' <param name="GeneObject"></param>
        ''' <returns></returns>
        Private Function OverlapCommon(genes As GeneBrief(), reader As IPolymerSequenceModel, geneObject As GeneBrief, length As Integer) As FastaSeq
            Dim loci As NucleotideLocation = geneObject.Location.GetUpStreamLoci(length)
            ' Dim relatedGenes = genes.GetRelatedGenes(loci.Left, loci.Right, 0)

            ''重叠的定义是位点和基因之间的关系为上下游重叠，覆盖，等于或者内部
            'Dim Overlaps = (From relG As Relationship(Of GeneBrief)
            '                In RelatedGenes
            '                Where relG.Relation = SegmentRelationships.Cover OrElse'覆盖了那个基因，或者在哪个基因的内部又或者就是在哪个基因的位置上，则不能用，返回空值
            '                    relG.Relation = SegmentRelationships.Equals OrElse
            '                    relG.Relation = SegmentRelationships.Inside
            '                Select relG).ToArray
            'If Not Overlaps.IsNullOrEmpty Then
            '    Return Nothing
            'End If

            'Overlaps = (From relates In RelatedGenes
            '            Where relates.Relation = SegmentRelationships.DownStreamOverlap OrElse
            '                relates.Relation = SegmentRelationships.UpStreamOverlap
            '            Select relates).ToArray

            'If Overlaps.IsNullOrEmpty Then                '太好了，没有重叠，则直接返回fasta序列
            '    Dim attrs As String() = {GeneObject.Synonym & " ", "Loci: " & Loci.ToString, GeneObject.Location.ToString, "Length=" & Loci.FragmentSize}
            '    Return New SequenceModel.FASTA.FastaToken With {
            '        .SequenceData = Reader.TryParse(Loci).SequenceData,
            '        .Attributes = attrs
            '    }
            'End If

            ''若果是上游重叠，假定5'UTR最短为50bp，则假若重叠的长度小于这个长度就减去这个长度，假若长度超过了这个值，则不能要

            ''由于是非严格的重叠，故而二者的链的方向是一致的，只需要计算左右位置的重叠程度就可以了
            'For Each gene As Relationship(Of GeneBrief) In Overlaps
            '    If Loci.Right > gene.Gene.Location.Left AndAlso
            '        Loci.Left < gene.Gene.Location.Left Then  '这个位点和下游的基因重叠，则缩短right至下游基因的left
            '        Loci.Right = gene.Gene.Location.Left
            '    End If

            '    If Loci.Left < gene.Gene.Location.Right AndAlso
            '        Loci.Right > gene.Gene.Location.Right Then '下游重叠，则将left改为上游基因的right
            '        Loci.Left = gene.Gene.Location.Right
            '    End If

            'Next

            'If Loci.FragmentSize < 25 Then
            '    Return Nothing ' 太短了，文献里面说不能要了
            'End If

            'If Loci.Strand = Strands.Forward Then
            '    Loci.Left += 1
            'Else
            '    Loci.Left += 1
            '    Loci.Right -= 1
            'End If

            'Dim seqAttrs As String() = {GeneObject.Synonym & " ", "Loci: " & Loci.ToString, GeneObject.Location.ToString, "Length=" & Loci.FragmentSize}
            'Return New FASTA.FastaToken With {
            '    .SequenceData = Reader.TryParse(Loci).SequenceData,
            '    .Attributes = seqAttrs
            '}

            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' 只要目标位点和基因不在同一条链之上，就不算是重叠
        ''' 
        ''' 这个函数只需要对同一条链之上的基因进行计算就可以了
        ''' </summary>
        ''' <param name="Reader"></param>
        ''' <param name="PTT"></param>
        ''' <param name="GeneObject"></param>
        ''' <returns></returns>
        Public Function TrimNotStrictOverlap(Reader As IPolymerSequenceModel, PTT As PTT, GeneObject As GeneBrief, Length As Integer) As FASTA.FastaSeq
            Dim genes As GeneBrief() = PTT.forwards Or PTT.reversed.AsDefault.When(GeneObject.Location.Strand <> Strands.Forward)
            Return OverlapCommon(genes, Reader, GeneObject, Length)
        End Function

        ''' <summary>
        ''' 为什么解释器就是找不到这个函数的入口点？？？？？
        ''' </summary>
        ''' <param name="Csv"></param>
        ''' <param name="DIPCsv"></param>
        ''' <returns></returns>
        <ExportAPI("VirtualFootprint_DIP", Info:="Associate the dip information with the Sigma 70 virtual footprints.")>
        Public Function VirtualFootprintDIP(<Parameter("vf.csv")> Csv As String,
                                            <Parameter("dip.csv")> DIPCsv As String) As Boolean

            Dim VirtualFootprints = Csv.LoadCsv(Of VirtualFootprints)(False)
            Dim DIP As Dictionary(Of String, MEME_DIP()) =
                (From row As MEME_DIP
                 In DIPCsv.LoadCsv(Of MEME_DIP)(False).AsParallel
                 Where Not String.IsNullOrEmpty(row.DIP)
                 Select row
                 Group row By row.Site Into Group) _
                      .ToDictionary(Function(obj) obj.Site,
                                    Function(obj) obj.Group.ToArray)
            'Dim LQuery = (From vf As NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints.VirtualFootprints In VirtualFootprints.AsParallel
            '              Let Genes = vf. ' vf.ExtractGenes
            '              Where Not Genes.IsNullOrEmpty
            '              Select vf.Distance, vf.Ends, vf.Length, vf.LociDescrib, vf.MotifFamily,
            '                  vf.MotifId, vf.MotifLocation, vf.ORF, vf.ORFDirection, vf.RNAGene,
            '                  vf.Sequence, vf.Signature, vf.Starts, vf.Strand,
            '                  DIP_ENTRY = String.Join("; ", (From id As String
            '                                                 In Genes
            '                                                 Let dip_value = DIP.TryGetValue(id)
            '                                                 Where Not dip_value.IsNullOrEmpty
            '                                                 Select (From obj In dip_value Select str = obj.DIP Distinct).FirstOrDefault).ToArray)).ToArray
            'Return LQuery.SaveTo(Csv & ".dip.csv", False)
        End Function

        Public Class VF_DIP

            Public Property Distance As String
            Public Property Ends As Integer
            Public Property Length As Integer
            Public Property LocationDescriptions As String
            Public Property MotifFamily As String
            Public Property MotifId As String
            Public ReadOnly Property MotifLocation As String
                Get
                    Return VirtualFootprints.GetLociDescrib(LocationDescriptions)
                End Get
            End Property

            Public Property ORF As String
            Public Property ORFDirection As String
            Public Property RNAGene As String
            Public Property Sequence As String
            Public Property Signature As String
            Public Property Starts As Integer
            Public Property Strand As String
            Public Property DIP_ENTRY As String

        End Class

        Public Class Transcript : Inherits DocumentFormat.Transcript

            <Column("-10(Start)")> Public Property Minus10Start As Long
            <Column("-10(Stop)")> Public Property Minus10Stop As Long
            <Column("TSSs(MEME)")> Public Property MEMEPredictedTSSs As Long

        End Class

        ''' <summary>
        ''' 将MEME的-10区预测结果和转录组装配所得到的结果进行整合以了解二者的分析结果的一致性如何
        ''' </summary>
        ''' <param name="Transcripts"></param>
        ''' <param name="PTT"></param>
        ''' <param name="Length"></param>
        ''' <returns></returns>
        ''' <remarks>函数只关联在MEME之中出现的基因，其他的都会留下空值</remarks>
        <ExportAPI("MEME.Association")>
        Public Function MEMEPredictedTSSsAssociations(Transcripts As IEnumerable(Of Transcript),
                                                      PTT As PTT,
                                                      MEME As IEnumerable(Of Motif),
                                                      Optional Length As Integer = 150) As Transcript()
            Dim MEMESites = (From site As Site
                             In (From obj As Motif In MEME.AsParallel Select obj.Sites).IteratesALL
                             Select site
                             Group site By site.Site Into Group) _
                                  .ToDictionary(Function(obj) obj.Site,
                                                Function(obj) obj.Group.ToArray)
            Dim LQuery = (From Transcript As Transcript
                          In Transcripts.AsParallel
                          Select MEMEPredictedTSSsAssociations(Transcript, PTT, MEMESites, Length)).ToVector
            Return LQuery
        End Function

        Private Function MEMEPredictedTSSsAssociations(Transcript As Transcript, PTT As PTT, MEME As Dictionary(Of String, Motif()), Length As Integer) As Transcript()
            If Not MEME.ContainsKey(Transcript.Synonym) Then
                Return {Transcript}
            End If
        End Function

        <ExportAPI("Read.Csv.Transcripts")>
        Public Function LoadTranscripts(Path As String) As Transcript()
            Return Path.LoadCsv(Of Transcript)(False).ToArray
        End Function

        <ExportAPI("Write.Csv.Transcripts")>
        Public Function WriteTranscripts(data As Generic.IEnumerable(Of Transcript), SaveTo As String) As Boolean
            Return data.SaveTo(SaveTo, False)
        End Function
    End Module
End Namespace
