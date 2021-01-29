#Region "Microsoft.VisualBasic::253c227652ca87e84d1c95bcd01339fb, meme_suite\MEME\Analysis\HtmlMatchs.vb"

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

'     Module HtmlMatching
' 
'         Function: ____match, __assignOperonInfo, __createObject, __isSingle, (+6 Overloads) __match
'                   __matchProcess, FilteringPcc, (+2 Overloads) Invoke, LoadMEMEXml, (+5 Overloads) Match
'                   MatchedTargetRegulator, MergeResult, NovelSites, PhenotypeRegulations, Process
'                   ReadData, SaveMatchedResult
'         Class MEMEAnalysisResult
' 
'             Properties: Regulated, RegulationMode, Regulator
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.RNA_Seq
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA.Network
Imports SMRUCC.genomics.Analysis.RNA_Seq.WGCNA
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.HTML
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.LDM
Imports SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat
Imports SMRUCC.genomics.SequenceModel
Imports Strands = SMRUCC.genomics.ComponentModel.Loci.Strands

Namespace Analysis

    <Package("MEME_Html.Analysis.Parser", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gmail.com")>
    Public Module HtmlMatching

        Public Class MEMEAnalysisResult : Implements RegulonDatabase.IRegulationModel

            Public Property Regulated As String Implements RegulonDatabase.IRegulationModel.Regulated
            Public Property RegulationMode As String Implements RegulonDatabase.IRegulationModel.RegulationMode
            Public Property Regulator As String Implements RegulonDatabase.IRegulationModel.Regulator
        End Class

        Private Function Process(Match As MatchedResult, BestMatches As Regprecise.RegpreciseMPBBH()) As MatchedResult()
            Dim LQuery = (From item In BestMatches Where String.Equals(Match.RegpreciseRegulator, item.HitName.Split(CChar(":")).Last) Select item).ToArray
            Dim TempChunk = New List(Of MatchedResult)
            For Each item In LQuery
                Dim CloneItem As MatchedResult = Match.Clone

                CloneItem.BiologicalProcess = item.pathway
                CloneItem.Effectors = item.effectors
                CloneItem.TF = item.QueryName
                CloneItem.TFFamily = item.Family

                Call TempChunk.Add(CloneItem)
            Next

            Return TempChunk.ToArray
        End Function

        Public Function MatchedTargetRegulator(ExportFile As IO.File, BestMatches As RegpreciseMPBBH()) As MatchedResult()
            Dim ChunkBuffer As MatchedResult() = ExportFile.AsDataSource(Of MatchedResult)(False)
            Dim MatchLQuery = (From Match As MatchedResult In ChunkBuffer.AsParallel Select Process(Match, BestMatches)).ToArray.ToVector
            Return MatchLQuery
        End Function

        <ExportAPI("MEME.ResultMatch")>
        Public Function Match(<Parameter("MEME.Xml", "The xml file path of the meme program output.")> MEMEXml As String,
                              <Parameter("MAST.html", "The HTML file path of the mast output.")> MASTHtml As String,
                              <Parameter("Os.Fasta.Path", "The fasta file path of the bacteria whole genome nucleotide sequence data.")> Genome As String,
                              <Parameter("CDS.Info")> CDSInfo As IEnumerable(Of GeneTable)) As VirtualFootprints()

            Dim MEME = MEMEXml.LoadXml(Of XmlOutput.MEME.MEME)().ToMEMEHtml
            Dim GenomeFasta As FASTA.FastaSeq = FASTA.FastaSeq.Load(Genome)
            Dim result = __match(MEME, MASTHtml, GenomeFasta, CDSInfo)
            Return result
        End Function

        <ExportAPI("Load.MEME.Xml")>
        Public Function LoadMEMEXml(path As String) As DocumentFormat.XmlOutput.MEME.MEME
            Dim doc As XmlOutput.MEME.MEME = path.LoadXml(Of XmlOutput.MEME.MEME)
            Return doc
        End Function

        <ExportAPI("Match")>
        Public Function Match(Motifs As IEnumerable(Of MEME.LDM.Motif), MastXml As XmlOutput.MAST.MAST) As ComponentModel.MotifSite()
            If MastXml Is Nothing OrElse MastXml.Sequences.SequenceList.IsNullOrEmpty Then  ' Motif模型在基因组上面找不到对应的位点
                Return New ComponentModel.MotifSite() {}
            End If

            Dim dictMotif = Motifs.ToDictionary(Function(motif) motif.Id)
            Dim LQuery = (From seq As DocumentFormat.XmlOutput.MAST.SequenceDescript
                          In MastXml.Sequences.SequenceList
                          Select __match(dictMotif, seq)).ToArray.ToVector.TrimNull
            Return LQuery
        End Function

        ''' <summary>
        ''' 使用函数<see cref="HtmlMatching.Match(IEnumerable(Of MEME.LDM.Motif), XmlOutput.MAST.MAST)"/>得到匹配的位点之后可以使用这个函数得到未被匹配到的可能的新的位点
        ''' </summary>
        ''' <param name="Motifs"></param>
        ''' <param name="matches"></param>
        ''' <returns></returns>
        Public Function NovelSites(Motifs As IEnumerable(Of MEME.LDM.Motif), matches As IEnumerable(Of MotifSite)) As ComponentModel.MotifSite()
            Dim Matched = matches.Select(Function(m) m.Tag).Distinct.ToArray
            Dim NonSingleMotifs = (From motif In Motifs
                                   Let id = motif.Id
                                   Where Not motif.__isSingle AndAlso ' 不是单一重复所照成的并且没有在结果之中被匹配上的就可能是novel的motif位点
                                       Array.IndexOf(Matched, id) = -1
                                   Select motif).ToArray  ' 不是由于带一个基因重复所照成的
            Dim resultSet = NonSingleMotifs.Select(Function(motif) motif.__createObject)
            Return resultSet
        End Function

        <Extension> Private Function __createObject(site As MEME.LDM.Motif) As MotifSite
            Dim length As Integer = Len(Regex.Replace(site.Signature, "\[.+?\]", "+"))

            Return New ComponentModel.MotifSite With {
                .EValue = site.Evalue,
                .Signature = site.Signature,
                .uid = site.uid,
                .Locus_tag = String.Join("; ", (From loci In site.Sites Where loci.Name.Count("_"c) <= 1 Select loci.Name).ToArray),
                .Tag = length
            }
        End Function

        <Extension> Private Function __isSingle(motif As MEME.LDM.Motif) As Boolean
            Dim LQuery = (From site In motif.Sites Where site.Name.Count("_"c) <= 1 Select site.Name Distinct).ToArray
            If LQuery.Length <= 1 Then
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' 复写位点里面的在基因组上面的位置以及链的方向
        ''' </summary>
        ''' <param name="sites"></param>
        ''' <param name="PTT"></param>
        ''' <param name="Length">进行启动子区的序列的解析的时候的片段的长度</param>
        ''' <returns></returns>
        <ExportAPI("Match")>
        Public Function Match(sites As IEnumerable(Of MotifSite), PTT As PTT, Length As Integer) As MotifSite()
            Return sites.Select(Function(site) Match(site, PTT(site.Locus_tag), Length))
        End Function

        Private Function Match(site As ComponentModel.MotifSite, gene As GeneBrief, Length As Integer) As ComponentModel.MotifSite
            Try
                Return __match(site, gene, Length)
            Catch ex As Exception
                If gene Is Nothing Then
                    ex = New Exception("gene parameter is null!", ex)
                Else
                    If gene.Location Is Nothing Then
                        ex = New Exception("gene location value is null!", ex)
                    End If
                End If

                If site Is Nothing Then
                    ex = New Exception("site value is nothing!", ex)
                End If
                Call App.LogException(ex)

                site.Locus_tag = site.Locus_tag & " *"

                Return site
            End Try
        End Function

        ''' <summary>
        ''' 从位点上面的相对位置得到基因组上面的绝对位置
        ''' </summary>
        ''' <param name="site"></param>
        ''' <param name="gene"></param>
        ''' <param name="Length"></param>
        ''' <returns></returns>
        Private Function __match(site As ComponentModel.MotifSite, gene As GeneBrief, Length As Integer) As ComponentModel.MotifSite
            If gene.Location.Strand = Strands.Forward Then
                site.Strand = "+"
                site.gStart = gene.Location.left - Length + site.Start
                site.gStop = site.gStart + Len(site.Sequence)
                site.RightEndDownStream = Length - site.Start + Len(site.Sequence)
            Else
                site.Strand = "-"
                site.gStart = gene.Location.right + Length - site.Start
                site.gStop = site.gStart - Len(site.Sequence)
                site.gStart.Swap(site.gStop)   ' 再交换位置变换为正常的位点位置
                site.RightEndDownStream = Length - site.Start - Len(site.Sequence)
            End If

            Return site
        End Function

        Private Function __match(Motifs As Dictionary(Of String, MEME.LDM.Motif),
                                 seq As DocumentFormat.XmlOutput.MAST.SequenceDescript) As MotifSite()
            Dim resultSet = seq.Segments.Select(Function(site) __match(Motifs, site, seq.name))
            Return resultSet.ToVector
        End Function

        Private Function __match(Motifs As Dictionary(Of String, MEME.LDM.Motif), site As XmlOutput.MAST.Segment, uid As String) As MotifSite()
            Return site.Hits.Select(Function(loci) __match(Motifs, loci, uid)).ToVector
        End Function

        Private Function __match(Motifs As Dictionary(Of String, MEME.LDM.Motif), site As XmlOutput.MAST.HitResult, uid As String) As MotifSite()
            Dim MotifId As String = site.motif.Split("_"c).Last
            If Not Motifs.ContainsKey(MotifId) Then
                Return Nothing  '数据出错了，匹配不上
            End If

            Dim Motif As MEME.LDM.Motif = Motifs(MotifId)
            Dim motifSites = (From loci As MEME.LDM.Site
                              In Motif.Sites
                              Where loci.Name.Count("_"c) <= 1
                              Select loci).ToArray

            If motifSites.Length < 2 Then  '这个Motif是由一个基因反复填充而来的，则不正确，丢弃掉这个motif
                Return Nothing
            End If

            Dim resultSet As MotifSite() = (
                From loci As MEME.LDM.Site
                In motifSites
                Select __match(Motif, loci, site, uid, MotifId)).ToArray
            Return resultSet
        End Function

        Private Function __match(Motif As MEME.LDM.Motif,
                                 motifSite As MEME.LDM.Site,
                                 site As XmlOutput.MAST.HitResult,
                                 uid As String,
                                 MotifId As String) As MotifSite
            Dim siteLoci As New MotifSite With {
                .EValue = Motif.Evalue,
                .Locus_tag = motifSite.Name,
                .PValue = site.pvalue,
                .Sequence = motifSite.Site,
                .Signature = Motif.Signature,
                .uid = uid,
                .Start = motifSite.Start,
                .RightEndDownStream = motifSite.Right,
                .Family = uid,
                .Tag = MotifId
            }
            Return siteLoci
        End Function

        Const __MAST_HTML_TRACE As String = "[ERROR] exception occurred while trying to create the footprint data from thread  ""{0}"", try to ignore this error!"

        Private Function __match(meme As MEME.HTML.MEMEHtml,
                                 masthtml As String,
                                 genome As SequenceModel.FASTA.FastaSeq,
                                 cdsInfo As IEnumerable(Of GeneTable)) As VirtualFootprints()
            Try
                Return ____match(meme, masthtml, genome, cdsInfo)
            Catch ex As Exception
                Dim trace As String = String.Format(__MAST_HTML_TRACE, masthtml.ToFileURL)
                ex = New Exception(trace, ex)
                Call ex.PrintException
                Call App.LogException(ex, MethodBase.GetCurrentMethod.GetFullName & "::ERROR-Thread.log")
                Return New VirtualFootprints() {}
            End Try
        End Function

        Private Function ____match(meme As MEME.HTML.MEMEHtml,
                                   masthtml As String,
                                   genome As SequenceModel.FASTA.FastaSeq,
                                   cdsInfo As IEnumerable(Of GeneTable)) As VirtualFootprints()

            Dim MAST = DocumentFormat.MAST.HTML.LoadDocument_v410(masthtml, False)
            Dim result = DocumentFormat.MAST.HTML.MatchMEMEAndMast(meme, MAST)
            Dim Footprints As VirtualFootprints() = (
                From motif As MEMEOutput
                In result
                Select __createMotifSiteInfo(Of GeneTable)(
                    motif, genome, GeneBriefInformation:=cdsInfo)).ToArray.ToVector

            Return Footprints
        End Function

        ''' <summary>
        ''' <paramref name="MastSourceDir"></paramref>之中的文件夹名称应该和<paramref name="GbkSourceDir"></paramref>之中的文件名是一一对应的
        ''' </summary>
        ''' <param name="MEME_Xml"></param>
        ''' <param name="MastSourceDir"></param>
        ''' <param name="export"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("match_result")>
        Public Function Match(<Parameter("MEME.Xml")> MEME_Xml As String,
                              <Parameter("Dir.Mast.Source")> MastSourceDir As String,
                              <Parameter("Dir.Gbk.Source")> GbkSourceDir As String,
                              <Parameter("Dir.Export")> export As String) As Boolean
            Dim GBKSource = (From path As KeyValuePair(Of String, String)
                             In LoadSourceEntryList(source:=FileIO.FileSystem.GetFiles(
                                 GbkSourceDir, FileIO.SearchOption.SearchTopLevelOnly, "*.gbk", "*.gb")).AsParallel
                             Select ID = path.Key, gbk = GBFF.File.Load(path.Value)).ToDictionary(
                                Function(key) key.ID, elementSelector:=Function(obj) obj.gbk)
            Dim mast = (From path As String
                        In FileIO.FileSystem.GetDirectories(MastSourceDir, FileIO.SearchOption.SearchTopLevelOnly)
                        Select mast_html = path & "/mast.html",
                            ID = FileIO.FileSystem.GetDirectoryInfo(path).Name.Split(CChar(".")).First).ToArray
            Dim meme = MEME_Xml.LoadXml(Of MEME_Suite.DocumentFormat.XmlOutput.MEME.MEME)().ToMEMEHtml
            Dim LQuery = (From mast_data In mast.AsParallel
                          Let ptt = GBKSource(mast_data.ID)
                          Select virtual_footprints = __match(
                              meme,
                              mast_data.mast_html,
                              ptt.Origin.ToFasta,
                              cdsInfo:=gbExportService.ExportGeneFeatures(ptt)),
                              ID = mast_data.ID).ToArray

            For Each item In LQuery
                Dim path As String = String.Format("{0}/{1}.csv", export, item.ID)
                Call item.virtual_footprints.SaveTo(path, False)
            Next

            Return True
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="MEME_out">X bp文件夹的路径</param>
        ''' <param name="MAST_out"></param>
        ''' <param name="faDIR"></param>
        ''' <param name="RegpreciseTFBS"></param>
        ''' <param name="bh"></param>
        ''' <param name="WGCNA"></param>
        ''' <param name="ChipData"></param>
        ''' <param name="Door"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("match_result", Info:="Procedure the meme suite analysis data for the bacteria phenotype regulation data.")>
        Public Function Invoke(MEME_out As String,
                               MAST_out As String,
                               faDIR As String,
                               RegpreciseTFBS As String,
                               bh As String,
                               WGCNA As String,
                               ChipData As String,
                               Door As String,
                               <Parameter("cutoff.Pcc")> Optional pccCutoff As Double = 0.65) As Integer
            Call Console.WriteLine("start to export meme analysis data...")

            Dim ExportedData As String =
                MAST.HTML.HtmlParser.Export(
                MEME_out,
                MAST_out,
                faDIR,
                SequenceModel.FASTA.FastaFile.Read(RegpreciseTFBS))  '所保存的结果Csv文件

            Dim RegulatorsBestMatch As Regprecise.RegpreciseMPBBH() = bh.LoadCsv(Of Regprecise.RegpreciseMPBBH).ToArray
            Call Console.WriteLine("Start to load data of WGCNA weights!")
            Dim WGCNAWeights = RTools.WGCNA.CreateObject(WGCNA)

            Call Console.WriteLine("Start to load data of Pcc values from the chipdata!")
            Dim Pcc As PccMatrix = CreatePccMAT(ChipData, True)
            Dim DoorOperons = SMRUCC.genomics.Assembly.DOOR.Load(Door).DOOROperonView

            Dim RegulatorIdList As String() = (From item In RegulatorsBestMatch Select item.QueryName Distinct).ToArray
            Call WGCNAWeights.Filtering(RegulatorIdList)
            Call Pcc.Filtering(RegulatorIdList)

            Console.WriteLine("Data Filtering done!")

            Call Console.WriteLine("Thread ""{0}"" started!", ExportedData)
            Dim Result = MatchedTargetRegulator(ExportedData, RegulatorsBestMatch)
            Call Console.WriteLine("Start to export data")
            Dim LQuery = (From item In Result.AsParallel Select __matchProcess(DoorOperons, item, Pcc, WGCNAWeights)).ToArray

            Call Console.WriteLine("Filtering result data with pcc cutoff {0}", pccCutoff)
            Dim FilterResult = FilteringPcc(LQuery, pccCutoff)
            Call Console.WriteLine("Result data filtering job done! start to write data into filesystem.")

            Call FilterResult.SaveTo(FileIO.FileSystem.GetParentPath(ExportedData) & "/Result_Filtered_" & pccCutoff & "/" & FileIO.FileSystem.GetName(ExportedData), False)
            Call LQuery.SaveTo(FileIO.FileSystem.GetParentPath(ExportedData) & "/Result/" & FileIO.FileSystem.GetName(ExportedData), False)
            Console.WriteLine("Thread ""{0}"" job done and threads exit with no error!", ExportedData)

            Return 0
        End Function

        Private Function __matchProcess(DOOR As OperonView,
                                        item As MatchedResult,
                                        Pcc As PccMatrix,
                                        WGCNAWeights As WGCNAWeight) As MatchedResult

            If Not DOOR.HaveOperon(item.DoorId) Then
                Call $"{item.DoorId} is not exists in the operons data!".__DEBUG_ECHO
            Else
                item.OperonPromoter = DOOR(item.DoorId).InitialX.Synonym
            End If
            item.TFPcc = Pcc.GetValue(item.TF, item.OperonPromoter)
            item.PccArray = (From Id As String In item.OperonGeneIds Select Pcc.GetValue(item.TF, Id)).ToArray
            item.WGCNAWeight = (From Id As String In item.OperonGeneIds Select WGCNAWeights.GetValue(item.TF, Id, Parallel:=False)).ToArray

            Return item
        End Function

        <ExportAPI("Matched.Merge")>
        Public Function MergeResult(DataDir As String) As MatchedResult()
            Dim CsvFileList = (From Path As String
                               In FileIO.FileSystem.GetFiles(DataDir, FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
                               Select Path.LoadCsv(Of MatchedResult)(False)).ToArray
            Dim ChunkBuffer As List(Of MatchedResult) = New List(Of MatchedResult)
            For Each File In CsvFileList
                Call ChunkBuffer.AddRange(File)
            Next

            Return ChunkBuffer.ToArray
        End Function

        <ExportAPI("Read.Csv.MEME_Matched")>
        Public Function ReadData(file As String) As MatchedResult()
            Return file.LoadCsv(Of MatchedResult)(False).ToArray
        End Function

        <ExportAPI("Statics.Regulations.CellPhenotype")>
        Public Function PhenotypeRegulations(MatchedRegulations As IEnumerable(Of MatchedResult), Pathways As IEnumerable(Of bGetObject.Pathway)) As IO.File
            Dim Regulations = (From Regulator As String
                               In (From item In MatchedRegulations Select item.TF Distinct).ToArray
                               Let RegulatedGenes = (From item As MatchedResult In MatchedRegulations
                                                     Where String.Equals(Regulator, item.TF)
                                                     Select item.OperonGeneIds).ToVector.Distinct
                               Select Regulator, RegulatedGenes).ToArray
            Dim PathwayFunctions As Dictionary(Of String, BriteHEntry.Pathway) =
                BriteHEntry.Pathway.LoadFromResource.ToDictionary(Function(item) item.EntryId)
            Dim PathwayRegulations = (From Regulator In Regulations.AsParallel
                                      Let RegulatedPathways = (From RegulatedGeneId As String
                                                               In Regulator.RegulatedGenes
                                                               Select (From Pathway In Pathways
                                                                       Where Not Pathway.genes.IsNullOrEmpty AndAlso Pathway.IsContainsGeneObject(RegulatedGeneId)
                                                                       Let [Class] As BriteHEntry.Pathway = PathwayFunctions(Regex.Match(Pathway.EntryId, "\d{5}").Value)
                                                                       Select Pathway.EntryId, [Class].category).ToArray).ToArray.ToVector
                                      Select Regulator.Regulator, RegulatePhenotypes = RegulatedPathways).ToArray
            Dim CsvFile As IO.File = New IO.File
            Dim Head As IO.RowObject = New IO.RowObject From {"Regulator", "Family"}
            Dim Phenotypes As String() = (From item In PathwayFunctions Select item.Value.category Distinct).ToArray
            Dim LQuery = (From Regulator In PathwayRegulations.AsParallel
                          Let RegulatorId As String = Regulator.Regulator
                          Let PhenotypeStatics = (From Type As String
                                              In Phenotypes
                                                  Select RegulatorTF = RegulatorId, Type, Counts = (From item In Regulator.RegulatePhenotypes Where String.Equals(item.Category, Type) Select 1).ToArray.Sum).ToArray
                          Select PhenotypeStatics).ToArray

            Dim st = (From i As Integer
                      In LQuery.First.Sequence
                      Where Not (From Line In LQuery Let c = Line(i).Counts Where c = 0 Select 1).Count = LQuery.Count
                      Select (From Line In LQuery Select Line(i)).ToArray).ToArray.MatrixTranspose

            Call Head.AddRange((From item In st.First Select item.Type).ToArray)
            Call CsvFile.AppendLine(Head)
            Call CsvFile.AppendRange((From Line In st
                                      Let Counts = (From item In Line Select CStr(item.Counts)).ToArray
                                      Let Array = New String()() {New String() {Line.First.RegulatorTF, ""}, Counts}
                                      Let value = Array.ToVector
                                      Select CType(value, RowObject)).ToArray)

            st = st.MatrixTranspose

            Dim PossibleSignificantPhenoTypeGene = (From Phenotype In st
                                                    Let SignificantGene = (From ll
                                                                           In (From Regulator In Phenotype
                                                                               Select Regulator
                                                                               Order By Regulator.Counts Descending).Take(Phenotype.Count * 0.15).ToArray
                                                                           Select ll.RegulatorTF).ToArray
                                                    Select Phenotype.First.Type,
                                                        Genes = String.Join(";", SignificantGene)).ToArray 'String.Format("{0} {1}", ll.RegulatorTF, ll.Counts)
            Call CsvFile.AppendLine()
            Call CsvFile.AppendLine(New String() {"Cell Phenotype", "Possible-Significant-Phenotype-Related-Regulator"})
            Call CsvFile.AppendRange((From Line In PossibleSignificantPhenoTypeGene
                                      Select CType({Line.Type, Line.Genes}, RowObject)).ToArray)

            Return CsvFile
        End Function

        ''' <summary>
        ''' 不适用的WGCNA权重进行筛选的原因是WGCNA不能够表示出负调控关系
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="pccCutoff"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Filtering.Pcc")>
        Public Function FilteringPcc(data As MatchedResult(), pccCutoff As Double) As MatchedResult()
            Dim LQuery = (From item As MatchedResult In data.AsParallel Where System.Math.Abs(item.TFPcc) >= pccCutoff OrElse item.TFPcc <= -0.65 Select item).ToArray
            Return LQuery
        End Function

        <ExportAPI("Write.Csv.MEME_Matched")>
        Public Function SaveMatchedResult(data As IEnumerable(Of MatchedResult), <Parameter("Path.Save")> SaveTo As String) As Boolean
            Return data.SaveTo(SaveTo, False)
        End Function

        ''' <summary>
        ''' 导出所分析出的所有数据，不加任何筛选操作
        ''' </summary>
        ''' <param name="MEME_out"></param>
        ''' <param name="MAST_out"></param>
        ''' <param name="FastaFileDir"></param>
        ''' <param name="bh"></param>
        ''' <param name="Door"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Export.MEME_Result", Info:="Export all of the meme analysis data whitout any data filtering operations.")>
        Public Function Invoke(MEME_out As String, MAST_out As String, FastaFileDir As String, bh As String, Door As String, Regprecise_TFBS As String) As Integer
            Call "Start to export meme analysis data...".__DEBUG_ECHO

            Dim ExportedData As String =
                MAST.HTML.HtmlParser.Export(
                MEME_out,
                MAST_out,
                FastaFileDir,
                RegpreciseTFBS:=SMRUCC.genomics.SequenceModel.FASTA.FastaFile.Read(Regprecise_TFBS))  '所保存的结果Csv文件

            Dim RegulatorsBestMatch As Regprecise.RegpreciseMPBBH() = bh.LoadCsv(Of Regprecise.RegpreciseMPBBH).ToArray
            Dim DoorOperons = SMRUCC.genomics.Assembly.DOOR.Load(Door).DOOROperonView
            Dim RegulatorIdList As String() = (From item In RegulatorsBestMatch Select item.QueryName Distinct).ToArray

            Call Console.WriteLine("Thread ""{0}"" started!", ExportedData)
            Dim Result = MatchedTargetRegulator(ExportedData, RegulatorsBestMatch)
            Call Console.WriteLine("Start to export data")
            Dim LQuery = (From item In Result.AsParallel Select __assignOperonInfo(item, DoorOperons)).ToArray

            Call LQuery.SaveTo(FileIO.FileSystem.GetParentPath(ExportedData) & "/Result/" & FileIO.FileSystem.GetName(ExportedData), False)
            Console.WriteLine("Thread ""{0}"" job done and threads exit with no error!", ExportedData)

            Return 0
        End Function

        Private Function __assignOperonInfo(item As MatchedResult, DOOR As OperonView) As MatchedResult
            If Not DOOR.HaveOperon(item.DoorId) Then
                Call $"{item.DoorId} is not exists in the operons data!".__DEBUG_ECHO
            Else
                item.OperonPromoter = DOOR(item.DoorId).InitialX.Synonym
            End If

            Return item
        End Function
    End Module
End Namespace
