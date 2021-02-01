#Region "Microsoft.VisualBasic::d6464d01c02c8a251126ee808cf11c9f, meme_suite\MEME\Workflows\PromoterParser\Parser.vb"

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

'     Class DataPreparingParser
' 
'         Constructor: (+2 Overloads) Sub New
' 
'         Function: __getFastaList, (+2 Overloads) CreateObject, ExtractFromStringInteraction, ExtractMekkPathwayPromoter, ExtractMetacycPathwayPromoter
'                   ExtractPromoterRegion_Pathway, ExtractPromoterRegion_PhenotypePathways, (+2 Overloads) ExtractWholeGenomePromoter, GetFasta
' 
'         Sub: Extract, ExtractKEGGModulesPromoter, ExtractPromoterRegion_KEGGModules, StringDbInteractions, (+2 Overloads) WholeGenomeParser
'         Class __extractFromStringTask
' 
'             Constructor: (+1 Overloads) Sub New
'             Sub: __extractForGenes, __extractFromString
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Threading.Tasks.Parallel
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.RNA_Seq
Imports SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA.Network
Imports SMRUCC.genomics.Analysis.RNA_Seq.WGCNA
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.PathwayBrief
Imports SMRUCC.genomics.Data.STRING
Imports SMRUCC.genomics.Data.STRING.SimpleCsv
Imports SMRUCC.genomics.foundation.psidev.XML
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Workflows.PromoterParser

    ''' <summary>
    ''' 按照给定的代谢途径或者其他的规则分组取出启动子序列，这个对象是考虑了操纵子的情况的
    ''' </summary>
    <Package("Analysis.MEME.Data_Preparing",
                        Category:=APICategories.ResearchTools,
                        Publisher:="xie.guigang@gmail.com")>
    Public Class DataPreparingParser

        Dim PromoterParser As OperonPromoterParser
        Dim MetaCyc As DatabaseLoadder

        Sub New(MetaCyc As DatabaseLoadder, DOOR As String)
            Me.MetaCyc = MetaCyc
            Me.PromoterParser = New OperonPromoterParser(MetaCyc.Database.FASTAFiles.OriginSourceFile, DOOR)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="os">全基因组核酸序列的<see cref="FastaSeq"/>单序列文件的文件路径</param>
        ''' <param name="Door"></param>
        Sub New(os As String, Door As String)
            Me.PromoterParser = New OperonPromoterParser(os, Door)
        End Sub

        ''' <summary>
        ''' 代谢途径的
        ''' </summary>
        ''' <param name="ExportLocation"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExtractPromoterRegion_Pathway(ExportLocation As String) As Integer
            Dim Pwys = PwyFilters.Performance(MetaCyc)
            Dim Pathways = PwyFilters.GenerateReport(Pwys, MetaCyc.GetGenes)
            Dim Door = PromoterParser.DoorOperonView

            Call ForEach(Pathways, Sub(Pathway As PathwayBrief)
                                       Dim DoorIdList As String() = (From GeneId As String In Pathway.PathwayGenes Select Door.Select(GeneId, False).Key Distinct).ToArray

                                       Call Extract(DoorIdList, PromoterParser.Promoter_150, Pathway.EntryId, 150, ExportLocation)
                                       Call Extract(DoorIdList, PromoterParser.Promoter_200, Pathway.EntryId, 200, ExportLocation)
                                       Call Extract(DoorIdList, PromoterParser.Promoter_250, Pathway.EntryId, 250, ExportLocation)
                                       Call Extract(DoorIdList, PromoterParser.Promoter_300, Pathway.EntryId, 300, ExportLocation)
                                       Call Extract(DoorIdList, PromoterParser.Promoter_350, Pathway.EntryId, 350, ExportLocation)
                                       Call Extract(DoorIdList, PromoterParser.Promoter_400, Pathway.EntryId, 400, ExportLocation)
                                       Call Extract(DoorIdList, PromoterParser.Promoter_450, Pathway.EntryId, 450, ExportLocation)
                                       Call Extract(DoorIdList, PromoterParser.Promoter_500, Pathway.EntryId, 500, ExportLocation)
                                   End Sub)
            Return 0
        End Function

        Private Shared Sub Extract(DoorIdList As String(),
                                   DataSource As Dictionary(Of String, FastaSeq),
                                   PathwayId As String,
                                   Length As Integer,
                                   ExportLocation As String)
            Dim LQuery = (From strId As String In DoorIdList Select DataSource(strId)).ToArray
            Dim Path As String = $"{ExportLocation}/MetaCycPathways_{Length}bp/{PathwayId}.fasta"
            Call CType(LQuery, SMRUCC.genomics.SequenceModel.FASTA.FastaFile).Save(Path)
        End Sub

        Public Function ExtractPromoterRegion_PhenotypePathways(KEGGPathways As IEnumerable(Of bGetObject.Pathway), Export As String) As Integer
            Dim Operons As Assembly.DOOR.OperonView = PromoterParser.DoorOperonView
            Dim ExportAction = Sub([Module] As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway)
                                   Dim module_operons = (From item In Operons.Select([Module].GetPathwayGenes) Select item.Key Distinct).ToArray
                                   Call GetFasta(module_operons, PromoterParser.Promoter_150).Save(String.Format("{0}/KEGGPathways_{1}bp/{2}.fasta", Export, 150, [Module].EntryId))
                                   Call GetFasta(module_operons, PromoterParser.Promoter_200).Save(String.Format("{0}/KEGGPathways_{1}bp/{2}.fasta", Export, 200, [Module].EntryId))
                                   Call GetFasta(module_operons, PromoterParser.Promoter_250).Save(String.Format("{0}/KEGGPathways_{1}bp/{2}.fasta", Export, 250, [Module].EntryId))
                                   Call GetFasta(module_operons, PromoterParser.Promoter_300).Save(String.Format("{0}/KEGGPathways_{1}bp/{2}.fasta", Export, 300, [Module].EntryId))
                                   Call GetFasta(module_operons, PromoterParser.Promoter_350).Save(String.Format("{0}/KEGGPathways_{1}bp/{2}.fasta", Export, 350, [Module].EntryId))
                                   Call GetFasta(module_operons, PromoterParser.Promoter_400).Save(String.Format("{0}/KEGGPathways_{1}bp/{2}.fasta", Export, 400, [Module].EntryId))
                                   Call GetFasta(module_operons, PromoterParser.Promoter_450).Save(String.Format("{0}/KEGGPathways_{1}bp/{2}.fasta", Export, 450, [Module].EntryId))
                                   Call GetFasta(module_operons, PromoterParser.Promoter_500).Save(String.Format("{0}/KEGGPathways_{1}bp/{2}.fasta", Export, 500, [Module].EntryId))
                               End Sub

            Call ForEach(KEGGPathways, ExportAction)
            Return 0
        End Function

        Private Shared Function GetFasta(operonList As String(), promoterRegions As Dictionary(Of String, FastaSeq)) As FastaFile

            Dim LQuery = (From itemId As String In operonList Select promoterRegions(itemId)).ToArray
            Return CType(LQuery, SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
        End Function

        <ExportAPI("kegg_pathways.extract")>
        Public Shared Function ExtractMekkPathwayPromoter([operator] As DataPreparingParser,
                                                          Export As String,
                                                          keggpathways As IEnumerable(Of bGetObject.Pathway)) As Integer
            Return [operator].ExtractPromoterRegion_PhenotypePathways(keggpathways, Export)
        End Function

        <ExportAPI("pathway_promoter.extract")>
        Public Shared Function ExtractMetacycPathwayPromoter([operator] As DataPreparingParser, export As String) As Integer
            Return [operator].ExtractPromoterRegion_Pathway(export)
        End Function

        <ExportAPI("KEGG.Modules.Promoter.Extract")>
        Public Shared Sub ExtractKEGGModulesPromoter(Parser As DataPreparingParser,
                                                     <Parameter("Dir.Export")> exportDir As String,
                                                     <Parameter("KEGG.Modules")> KEGG_Modules As IEnumerable(Of KEGG.Archives.Csv.Module))
            Call Parser.ExtractPromoterRegion_KEGGModules(exportDir, KEGG_Modules)
        End Sub

        Public Sub ExtractPromoterRegion_KEGGModules(ExportDir As String, KEGGModules As KEGG.Archives.Csv.Module())
            Dim Operons As DOOR.OperonView = PromoterParser.DoorOperonView

            Dim getFasta = Function(operonList As String(), promoterRegions As Dictionary(Of String, FastaSeq)) As FastaFile
                               Dim LQuery = (From oprId As String
                                             In operonList
                                             Select promoterRegions(oprId)).ToArray
                               Return CType(LQuery, FastaFile)
                           End Function

            Dim InvokeAction As Action(Of KEGG.Archives.Csv.Module) =
                Sub([Module] As KEGG.Archives.Csv.Module)
                    Dim module_operons As String() = (From item In Operons.Select([Module].PathwayGenes) Select item.Key Distinct).ToArray

                    Call getFasta(module_operons, PromoterParser.Promoter_150).Save(String.Format("{0}/KEGGModules_{1}bp/{2}.fasta", ExportDir, 150, [Module].EntryId))
                    Call getFasta(module_operons, PromoterParser.Promoter_200).Save(String.Format("{0}/KEGGModules_{1}bp/{2}.fasta", ExportDir, 200, [Module].EntryId))
                    Call getFasta(module_operons, PromoterParser.Promoter_250).Save(String.Format("{0}/KEGGModules_{1}bp/{2}.fasta", ExportDir, 250, [Module].EntryId))
                    Call getFasta(module_operons, PromoterParser.Promoter_300).Save(String.Format("{0}/KEGGModules_{1}bp/{2}.fasta", ExportDir, 300, [Module].EntryId))
                    Call getFasta(module_operons, PromoterParser.Promoter_350).Save(String.Format("{0}/KEGGModules_{1}bp/{2}.fasta", ExportDir, 350, [Module].EntryId))
                    Call getFasta(module_operons, PromoterParser.Promoter_400).Save(String.Format("{0}/KEGGModules_{1}bp/{2}.fasta", ExportDir, 400, [Module].EntryId))
                    Call getFasta(module_operons, PromoterParser.Promoter_450).Save(String.Format("{0}/KEGGModules_{1}bp/{2}.fasta", ExportDir, 450, [Module].EntryId))
                    Call getFasta(module_operons, PromoterParser.Promoter_500).Save(String.Format("{0}/KEGGModules_{1}bp/{2}.fasta", ExportDir, 500, [Module].EntryId))
                End Sub

            Call ForEach(KEGGModules, InvokeAction)
        End Sub

        ''' <summary>
        ''' 解析出string-db之中的蛋白质互作的网络之中的蛋白质基因的ATG上游的调控区序列
        ''' </summary>
        ''' <param name="stringDIR"></param>
        ''' <param name="ExportDir"></param>
        Public Sub StringDbInteractions(stringDIR As String, ExportDir As String)
            Call ForEach(Of String)(
                FileIO.FileSystem.GetFiles(stringDIR, FileIO.SearchOption.SearchTopLevelOnly, "*.xml").ToArray,
                AddressOf New __extractFromStringTask(ExportDir, Me).__extractFromString)
        End Sub

        Private Class __extractFromStringTask

            Dim Parser As DataPreparingParser
            Dim ExportDir As String

            Sub New(ExportDir As String, Parser As DataPreparingParser)
                Me.ExportDir = ExportDir
                Me.Parser = Parser
            End Sub

            Public Sub __extractFromString(stringEntry As String)
                Dim entry = stringEntry.LoadXml(Of EntrySet)(throwEx:=False)

                If entry Is Nothing Then
                    Return
                End If

                Dim Interactions As PitrNode() = entry.ExtractNetwork
                Dim GeneId As String = Strings.Split(stringEntry.Replace("\", "/"), "/").Last.Split(CChar(".")).First
                Dim Door = Parser.PromoterParser.DoorOperonView.Select(GeneId)
                Dim InteractingGeneIds As String() = LinqAPI.Exec(Of String) <=
 _
                    From intr As PitrNode
                    In Interactions
                    Let Itr_Id As String = intr.GetInteractNode(GeneId)
                    Where Not String.IsNullOrEmpty(Itr_Id)
                    Select Itr_Id
                    Distinct

                If Not InteractingGeneIds.IsNullOrEmpty Then
                    Dim DOORids As List(Of String) = LinqAPI.MakeList(Of String) <=
                        From strId As String
                        In InteractingGeneIds
                        Select Parser.PromoterParser.DoorOperonView.Select(strId).Key

                    Call DOORids.Add(Door.Key)

                    Call __extractForGenes(DOORids.Distinct.ToArray, Parser.PromoterParser.Promoter_150, GeneId, 150, ExportDir)
                    Call __extractForGenes(DOORids.Distinct.ToArray, Parser.PromoterParser.Promoter_200, GeneId, 200, ExportDir)
                    Call __extractForGenes(DOORids.Distinct.ToArray, Parser.PromoterParser.Promoter_250, GeneId, 250, ExportDir)
                    Call __extractForGenes(DOORids.Distinct.ToArray, Parser.PromoterParser.Promoter_300, GeneId, 300, ExportDir)
                    Call __extractForGenes(DOORids.Distinct.ToArray, Parser.PromoterParser.Promoter_350, GeneId, 350, ExportDir)
                    Call __extractForGenes(DOORids.Distinct.ToArray, Parser.PromoterParser.Promoter_400, GeneId, 400, ExportDir)
                    Call __extractForGenes(DOORids.Distinct.ToArray, Parser.PromoterParser.Promoter_450, GeneId, 450, ExportDir)
                    Call __extractForGenes(DOORids.Distinct.ToArray, Parser.PromoterParser.Promoter_500, GeneId, 500, ExportDir)
                End If
            End Sub

            Private Shared Sub __extractForGenes(DoorIdList As String(),
                                     DataSource As Dictionary(Of String, FastaSeq),
                                     GeneId As String,
                                     Length As Integer,
                                     ExportDir As String)

                Dim LQuery = (From strId As String In DoorIdList Select DataSource(strId)).ToArray
                Dim Fasta = CType(LQuery, SMRUCC.genomics.SequenceModel.FASTA.FastaFile)

                Call Fasta.Save(String.Format("{0}/{1}/{2}.fsa", ExportDir, Length, GeneId))
            End Sub
        End Class

        Public Sub WholeGenomeParser(ChipData As MatrixFrame, pccCutoff As Double, Export As String)
            Dim PccMatrix = ChipData.CalculatePccMatrix
            Dim GetFastaList = Function(DoorIdList As List(Of String), DataSource As Dictionary(Of String, FastaSeq)) As FastaFile
                                   Dim LQuery = (From strId As String In DoorIdList Select DataSource(strId)).ToArray
                                   Return LQuery
                               End Function

            Dim BriefInfo As New File
            Call BriefInfo.Add(New String() {"OperonId", "OperonCounts_Gt_PccCutOff", "OperonIdList"})

            Dim InvokeAction As Action(Of DOOR.Operon) = Sub(Operon As DOOR.Operon)
                                                             Dim currentPromoter = Operon.InitialX
                                                             Dim LQuery = (From PairedOperon In PromoterParser.DoorOperonView.Operons
                                                                           Let condition = Function() As Boolean
                                                                                               Dim Pcc As Double = PccMatrix.GetValue(currentPromoter.Synonym, PairedOperon.InitialX.Synonym)

                                                                                               If Pcc < 0 Then  '二者为负调控关系，则不可能受同一个调控因子调控，故而不会讲这组数据用于MEME分析
                                                                                                   Return False
                                                                                               Else
                                                                                                   Return Pcc >= pccCutoff
                                                                                               End If
                                                                                           End Function Where condition() = True Select PairedOperon).ToArray

                                                             Dim DoorIDList As New List(Of String)(From item In LQuery Select item.Key Distinct)
                                                             Call BriefInfo.Add(New String() {Operon.Key, DoorIDList.Count, String.Join(";", DoorIDList)})
                                                             Call DoorIDList.Add(Operon.Key)

                                                             Call GetFastaList(DoorIDList, Me.PromoterParser.Promoter_150).Save(String.Join("/", Export, 150, Operon.Key & ".fsa"))
                                                             Call GetFastaList(DoorIDList, Me.PromoterParser.Promoter_200).Save(String.Join("/", Export, 200, Operon.Key & ".fsa"))
                                                             Call GetFastaList(DoorIDList, Me.PromoterParser.Promoter_250).Save(String.Join("/", Export, 250, Operon.Key & ".fsa"))
                                                             Call GetFastaList(DoorIDList, Me.PromoterParser.Promoter_300).Save(String.Join("/", Export, 300, Operon.Key & ".fsa"))
                                                             Call GetFastaList(DoorIDList, Me.PromoterParser.Promoter_350).Save(String.Join("/", Export, 350, Operon.Key & ".fsa"))
                                                             Call GetFastaList(DoorIDList, Me.PromoterParser.Promoter_400).Save(String.Join("/", Export, 400, Operon.Key & ".fsa"))
                                                             Call GetFastaList(DoorIDList, Me.PromoterParser.Promoter_450).Save(String.Join("/", Export, 450, Operon.Key & ".fsa"))
                                                             Call GetFastaList(DoorIDList, Me.PromoterParser.Promoter_500).Save(String.Join("/", Export, 500, Operon.Key & ".fsa"))
                                                         End Sub

            Call ForEach(PromoterParser.DoorOperonView.Operons, InvokeAction)

            Call BriefInfo.Save(String.Join("/", Export, "BriefInfo.csv"), False)
        End Sub

        Private Function __getFastaList(lstDOOR As List(Of String), DataSource As Dictionary(Of String, FastaSeq)) As FastaFile
            Dim LQuery = (From sId As String In lstDOOR Select DataSource(sId)).ToArray
            Return LQuery
        End Function

        Public Sub WholeGenomeParser(WGCNA As WGCNAWeight, Cutoff As String, Export As String)
            Dim BriefInfo As New File
            Call BriefInfo.Add(New String() {"OperonId", "OperonCounts_Gt_WGCNACutOff", "OperonIdList"})

            Dim InvokeAction As Action(Of SMRUCC.genomics.Assembly.DOOR.Operon) =
                Sub(Operon As SMRUCC.genomics.Assembly.DOOR.Operon)
                    Dim currentPromoter = Operon.InitialX
                    Dim LQuery = (From PairedOperon In PromoterParser.DoorOperonView.Operons
                                  Let condition = Function() As Boolean
                                                      Dim WGCNA_Object = WGCNA.Find(currentPromoter.Synonym, PairedOperon.InitialX.Synonym)
                                                      If WGCNA_Object Is Nothing Then
                                                          Return False
                                                      End If

                                                      Dim weight As Double = WGCNA_Object.Weight

                                                      If weight < 0 Then  '二者为负调控关系，则不可能受同一个调控因子调控，故而不会讲这组数据用于MEME分析
                                                          Return False
                                                      Else
                                                          Return weight >= Cutoff
                                                      End If
                                                  End Function Where condition() = True Select PairedOperon).ToArray

                    Dim DoorIDList As New List(Of String)(From item In LQuery Select item.Key Distinct)

                    Call BriefInfo.Add(New String() {Operon.Key, DoorIDList.Count, String.Join(";", DoorIDList)})
                    Call DoorIDList.Add(Operon.Key)

                    Call __getFastaList(DoorIDList, Me.PromoterParser.Promoter_150).Save(String.Join("/", Export, 150, Operon.Key & ".fsa"))
                    Call __getFastaList(DoorIDList, Me.PromoterParser.Promoter_200).Save(String.Join("/", Export, 200, Operon.Key & ".fsa"))
                    Call __getFastaList(DoorIDList, Me.PromoterParser.Promoter_250).Save(String.Join("/", Export, 250, Operon.Key & ".fsa"))
                    Call __getFastaList(DoorIDList, Me.PromoterParser.Promoter_300).Save(String.Join("/", Export, 300, Operon.Key & ".fsa"))
                    Call __getFastaList(DoorIDList, Me.PromoterParser.Promoter_350).Save(String.Join("/", Export, 350, Operon.Key & ".fsa"))
                    Call __getFastaList(DoorIDList, Me.PromoterParser.Promoter_400).Save(String.Join("/", Export, 400, Operon.Key & ".fsa"))
                    Call __getFastaList(DoorIDList, Me.PromoterParser.Promoter_450).Save(String.Join("/", Export, 450, Operon.Key & ".fsa"))
                    Call __getFastaList(DoorIDList, Me.PromoterParser.Promoter_500).Save(String.Join("/", Export, 500, Operon.Key & ".fsa"))
                End Sub

            Call ForEach(PromoterParser.DoorOperonView.Operons, InvokeAction)

            Call BriefInfo.Save(String.Join("/", Export, "BriefInfo.csv"), False)
        End Sub

        <ExportAPI("Session.New")>
        Public Shared Function CreateObject(MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Door As String) As DataPreparingParser
            Return New DataPreparingParser(MetaCyc, Door)
        End Function

        <ExportAPI("Session.New")>
        Public Shared Function CreateObject(<Parameter("os.Fasta")> osNT As String,
                                            <Parameter("genome.opr")> door As String) As DataPreparingParser
            Return New DataPreparingParser(osNT, door)
        End Function

        <ExportAPI("Extract.From.string-db",
                   Info:="Extract the ATG upstream region sequence from the protein interaction predicts data from the string-db.")>
        Public Shared Function ExtractFromStringInteraction([Operator] As DataPreparingParser,
               <Parameter("string-db", "The directory for the string-db download location, which contains the protein interaction entrySet objects' xml file in that directory.")>
               StringDb As String,
               <Parameter("Dir.Export", "Directory for export the fasta sequence.")> export As String) As Integer
            Call [Operator].StringDbInteractions(StringDb, export)
            Return 0
        End Function

        <ExportAPI("extract.whole_genome_promoter")>
        Public Shared Function ExtractWholeGenomePromoter([Operator] As DataPreparingParser, export As String, chipdata As MatrixFrame, pcccutoff As String) As Integer
            Call [Operator].WholeGenomeParser(chipdata, pcccutoff, export)
            Return 0
        End Function

        <ExportAPI("Extract.From.WGCNA", Info:="Extract data from WGCNA co-expression data.")>
        Public Shared Function ExtractWholeGenomePromoter([Operator] As DataPreparingParser,
                                                          export As String,
                                                          wgcna As WGCNAWeight,
                                                          cutoff As String) As Integer
            Call [Operator].WholeGenomeParser(wgcna, cutoff, export)
            Return 0
        End Function
    End Class
End Namespace
