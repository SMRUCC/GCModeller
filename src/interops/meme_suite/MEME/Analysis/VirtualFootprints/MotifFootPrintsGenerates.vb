#Region "Microsoft.VisualBasic::77f398926ff054bc182386b46e520994, meme_suite\MEME\Analysis\VirtualFootprints\MotifFootPrintsGenerates.vb"

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

    '     Module MotifFootPrintsGenerates
    ' 
    '         Function: __checkMoitfCoRegulations, __getEffect, __reGenerate, CreateEntryDictionary, CreateMotifInformations
    '                   DataFilteringByPathwayCoExpression, Diff, Effect2Pcc, ExpandRegulatorRegulations, ExportRegulators
    '                   FilteringEmptyData, FootPrintMatches, FootprintMatchesTEXT, GenerateDocument, GenerateNetwork
    '                   GetSiteTag, GroupMotifs, LoadMotifDatabase, Matches, (+2 Overloads) MatchRegulator
    '                   MergeFootprints, PathwayFunctionAssociation, PccAssumption, ReadRegulations, ReadVirtualFootprints
    '                   SaveRegulations, StructurePcc, WriteMotifDatabase, WriteVirtualFootprints
    ' 
    '         Sub: __pathwaysInfo
    '         Class GetPccValue
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: EmptyReader, GetPcc, InternalGetMixedMATValue, InternalGetPccMATValue
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.RNA_Seq
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MAST.HTML
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.HTML
Imports SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports sys = System.Math

Namespace Analysis.GenomeMotifFootPrints

    ''' <summary>
    ''' 这里所定义的所有对象都是和数据解析无关了，都是用于进行数据存储的对象类型
    ''' </summary>
    ''' <remarks></remarks>
    <Package("MEME.app.Genome_Footprints",
        Description:="Steps for genome wide regulation analysis: <br />
     <li> 1. Download the regprecise database And then extract the motif sequence.
     <li> 2. Download the regprecise regulators protein sequence from kegg
     <li> 3. besthit blastp using regprecise regulator database And your protein sequence.
     <li> 4. MEME analysis of the motif sequence and then build a motif database
     <li> 5. MAST analysis using the regprecise meme analysis result on the genome sequence
     <li> 6. Result analysis and then for the regulation analysis.",
                        Publisher:="xie.guigang@gmail.com; amethyst.asuka@gcmodeller.org")>
    Public Module MotifFootPrintsGenerates

        <ExportAPI("Write.Csv.VirtualFootprint")>
        Public Function WriteVirtualFootprints(data As IEnumerable(Of VirtualFootprints), <Parameter("Path.Save")> SaveTo As String) As Boolean
            Return data.SaveTo(SaveTo, False)
        End Function

        <ExportAPI("Read.Csv.VirtualFootprints")>
        Public Function ReadVirtualFootprints(path As String) As VirtualFootprints()
            Return path.LoadCsv(Of VirtualFootprints)(False).ToArray
        End Function

        ''' <summary>
        ''' 按照家族将可能重复的motif归为一组数据
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Footprint.Data.Group", Info:="Group the duplicated motif data by regulator family.")>
        <Extension>
        Public Function GroupMotifs(data As IEnumerable(Of PredictedRegulationFootprint), Genome As NucleicAcid, Optional length As Integer = 5) As PredictedRegulationFootprint()
            Dim setValue = New SetValue(Of PredictedRegulationFootprint) <= NameOf(PredictedRegulationFootprint.MotifId)
            data = (From item As PredictedRegulationFootprint
                    In data.AsParallel
                    Let TrimedItem = setValue(item, item.MotifId.Split(CChar(".")).First)
                    Select TrimedItem
                    Order By TrimedItem.Starts Ascending).ToArray
            Dim GroupOperation = (From item As PredictedRegulationFootprint In data.AsParallel
                                  Let possible_duplicated = (From o As PredictedRegulationFootprint In data
                                                             Where Math.Abs(o.Starts - item.Starts) < length AndAlso
                                                                   Math.Abs(o.Length - item.Length) < length AndAlso
                                                                   String.Equals(item.MotifId, o.MotifId, StringComparison.OrdinalIgnoreCase)
                                                             Select o
                                                             Order By o.Starts).ToArray
                                  Select GroupTag = String.Join("", (From o In possible_duplicated Select o.Starts Order By Starts Ascending).ToArray) & "-" & item.MotifFamily.ToUpper, possible_duplicated
                                  Order By GroupTag Ascending).ToArray
            GroupOperation = (From GroupTag As String
                              In (From item In GroupOperation Select item.GroupTag Distinct).ToArray.AsParallel
                              Select (From item In GroupOperation Where String.Equals(GroupTag, item.GroupTag) Select item).First).ToArray

            data = (From item In GroupOperation.AsParallel
                    Select If(item.possible_duplicated.Count = 1,
                        item.possible_duplicated.First,
                        __reGenerate(item.possible_duplicated, Genome:=Genome))).ToArray
            Return data
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="grouped">已经按照<see cref="PredictedRegulationFootprint.Starts"></see>属性进行排序</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function __reGenerate(grouped As IEnumerable(Of PredictedRegulationFootprint), Genome As IPolymerSequenceModel) As PredictedRegulationFootprint
            'Dim RightAligned = (From item In grouped Select item Order By item.Ends Descending).First
            'Dim ReGeneratedData As PredictedRegulationFootprint = grouped.First.Clone
            'ReGeneratedData.Starts = grouped.First.Starts
            'ReGeneratedData.Ends = RightAligned.Ends
            'ReGeneratedData.MotifId = grouped.First.MotifFamily
            'ReGeneratedData.Regulator = (From item In grouped Where Not item.Regulators.IsNullOrEmpty Select item.Regulators).ToArray.MatrixToVector
            'ReGeneratedData.Regprecise = (From item In grouped Where Not item.Regprecise.IsNullOrEmpty Select item.Regprecise).ToArray.MatrixToVector
            'ReGeneratedData.Signature = (From item In grouped Select item.Signature Order By Len(Signature) Descending).First
            'ReGeneratedData.Sequence = Genome.TryParse(ReGeneratedData.Starts, ReGeneratedData.Length)

            'Return ReGeneratedData
        End Function

        ''' <summary>
        ''' 通过同一个代谢途径之中的所有的基因可能共表达的生物学知识，在缺乏转录组数据的条件之下进行数据的筛选，算法的要点：
        ''' 假若同一个代谢途径之中的基因，有65%或者以上的基因具有同一个motif位点，则该motif位点可能是正确的位点
        ''' 
        ''' 本方法的缺点是，仅能够大致的筛选出具有代谢途径信息的基因
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="pathwayInfo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Select.From.Pathway.COExpression",
                   Info:="includes_regulators indicate that if the regulator information is also included in the output data")>
        Public Function DataFilteringByPathwayCoExpression(data As IEnumerable(Of PredictedRegulationFootprint),
                                                           pathwayInfo As IEnumerable(Of KEGG.Archives.Csv.Pathway),
                                                           <Parameter("Regulators.Includes")>
                                                           Optional includeRegulators As Boolean = False) _
                                                           As PredictedRegulationFootprint()

            'Dim PathwayGenes = (From pwy As SMRUCC.genomics.Assembly.KEGG.Archives.Csv.Pathway
            '                    In pathwayInfo
            '                    Where Not pwy.PathwayGenes.IsNullOrEmpty
            '                    Select pwy.EntryId, pwyGeneObjs = pwy.GetPathwayGenes).ToArray

            'data = From FootprintData As PredictedRegulationFootprint
            '       In data.AsParallel
            '       Where Not String.IsNullOrEmpty(FootprintData.ORF)
            '       Select FootprintData  '数据转换与数据筛选

            'Dim OperonData As PredictedRegulationFootprint() = (From prf As PredictedRegulationFootprint
            '                                                    In data
            '                                                    Where String.Equals(prf.IsOperonPromoter, "1")
            '                                                    Select prf
            '                                                    Distinct).ToArray  '代谢途径基因可能是操纵子之中的一个结构基因，故而该基因找不到调控位点的时候，会查找操纵子的第一个基因上面的位点
            'Dim ChunkBuffer As List(Of PredictedRegulationFootprint) =
            '    New List(Of PredictedRegulationFootprint)

            'For Each Pathway In PathwayGenes
            '    Call __pathwaysInfo(Pathway.pwyGeneObjs, OperonData, data, ChunkBuffer)
            'Next

            'If includeRegulators Then
            '    Dim Regulators As String() = (From pfrData As PredictedRegulationFootprint In OperonData
            '                                  Where Not pfrData.Regulators.IsNullOrEmpty
            '                                  Select regEntries = pfrData.Regulators).ToArray.MatrixToVector.Distinct.ToArray
            '    Call ChunkBuffer.AddRange((From prf As PredictedRegulationFootprint In data.AsParallel
            '                               Where Array.IndexOf(Regulators, prf.ORF) > -1
            '                               Select prf).ToArray)
            'End If

            'Return FilteringEmptyData((From prf As PredictedRegulationFootprint
            '                           In ChunkBuffer
            '                           Select prf
            '                           Distinct).ToArray)
        End Function

        Private Sub __pathwaysInfo(pwyGeneObjs As String(),
                                   OperonData As PredictedRegulationFootprint(),
                                   data As IEnumerable(Of PredictedRegulationFootprint),
                                   ByRef chunkBuffer As List(Of PredictedRegulationFootprint))

            Dim PathwayRelatedOperon As String() = (From GeneId As String In pwyGeneObjs
                                                    Let DoorList = (From fp_Data As PredictedRegulationFootprint In OperonData
                                                                    Where String.Equals(fp_Data.ORF, GeneId) OrElse Array.IndexOf(fp_Data.StructGenes, GeneId) > -1
                                                                    Select fp_Data.DoorId).ToArray
                                                    Select DoorList).ToArray.ToVector.Distinct.ToArray  '首先根据代谢途径之中的基因的信息得到相关的操纵子的编号信息
            Dim LQuery = (From fp_Data As PredictedRegulationFootprint In data.AsParallel
                          Where Array.IndexOf(pwyGeneObjs, fp_Data.ORF) > -1
                          Select fp_Data).AsList   '得到代谢途径之中的基因的调控数据
            Call LQuery.AddRange((From GeneId As String In pwyGeneObjs.AsParallel
                                  Let OperonFirst = (From item In OperonData Where Array.IndexOf(item.StructGenes, GeneId) > -1 Select item).ToArray
                                  Select OperonFirst).ToArray.ToVector)  '代谢途径的基因所在的操纵子的第一个基因

            Dim Grouped = (From fp_Data As PredictedRegulationFootprint
                           In LQuery
                           Select fp_Data
                           Group fp_Data By fp_Data.MotifId Into Group).ToDictionary(Function(item) item.MotifId, elementSelector:=Function(item) item.Group.ToArray)      '将基因按motifId进行分组

            Dim GetPossibleMoitf = (From groupItem In Grouped
                                    Let GeneID_List As String() = (From fp_Data As PredictedRegulationFootprint In groupItem.Value Select fp_Data.DoorId Distinct).ToArray
                                    Where GeneID_List.Count / PathwayRelatedOperon.Count >= 0.65
                                    Select MotifId = groupItem.Key, groupItem.Value).ToArray       '从分组的数据之中筛选出基因的数目站代谢途径的65以上的motif分组，则该分组为可能的正确的数据

            Call chunkBuffer.AddRange((From item In GetPossibleMoitf Select item.Value).ToArray.ToVector)

            For Each item In GetPossibleMoitf
                Call Grouped.Remove(item.MotifId)
            Next

            If Grouped.IsNullOrEmpty Then Return

            '对于剩下的motif而言，则开始查找分组的motif的基因之间是否有相同的调控因子，假若存在相同的调控因子的话，则可能为一个比较可靠的共表达的调控关系，假若都不存在，则忽略当前的这个motif
            For Each Motif As KeyValuePair(Of String, PredictedRegulationFootprint()) In Grouped
                Dim value = __checkMoitfCoRegulations(Motif.Value)
                If Not value.IsNullOrEmpty Then
                    Call chunkBuffer.AddRange(value)
                End If
            Next
        End Sub

        ''' <summary>
        ''' 假若目标Regulator可以被<paramref name="groupedMotif"></paramref>里面的多余60%的对象所具备，则认为该调控因子对目标motif的调控关系是可能存在的
        ''' </summary>
        ''' <param name="groupedMotif"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function __checkMoitfCoRegulations(groupedMotif As PredictedRegulationFootprint()) As PredictedRegulationFootprint()
            'Dim RegulatorIds As String() = (From item In groupedMotif Select item.Regulators).ToArray.MatrixToList.Distinct.ToArray
            'Dim TrimedData = (From item In groupedMotif Select Regulators = item.Regulators.AsList, Regulation = item).ToArray
            'Dim n As Integer = TrimedData.Count

            'For Each RegulatorId As String In RegulatorIds
            '    Dim Counts = (From item In TrimedData Where item.Regulators.IndexOf(RegulatorId) > -1 Select 1).ToArray.Count      '假若不满足筛选条件，则将这个调控因子的编号删除

            '    If Counts / n < 0.65 Then
            '        For Each item In TrimedData           '将调控因子移除
            '            Call item.Regulators.Remove(RegulatorId)
            '        Next
            '    End If
            'Next

            'groupedMotif = (From item In TrimedData
            '                Where Not item.Regulators.IsNullOrEmpty
            '                Select item.Regulation.InvokeSet(NameOf(item.Regulation.Regulators), item.Regulators.ToArray)).ToArray
            'Return groupedMotif
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="MEME_OUT">MEME html文件夹集合</param>
        ''' <param name="MAST_OUT">Mast html文件夹集合</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Footprint.Creates")>
        Public Function FootPrintMatches(<Parameter("DIR.MEME_OUT", "Directory which contains the meme html output result.")>
                                         MEME_OUT As String,
                                         <Parameter("DIR.MAST_OUT", "Directory which contains the mast html output result.")>
                                         MAST_OUT As String) As MEMEOutput()

            Dim LQuery = (From Path As String
                          In FileIO.FileSystem.GetDirectories(MAST_OUT, FileIO.SearchOption.SearchTopLevelOnly).AsParallel
                          Let docPath As String = Path & "/mast.html"    '以MAST文件为标准
                          Where FileIO.FileSystem.FileExists(docPath)
                          Let MASTDoc As MASTHtml = MAST.HTML.LoadDocument(docPath, FootPrintMode:=True)
                          Let ObjectId As String = FileIO.FileSystem.GetDirectoryInfo(Path).Name
                          Let MEME_doc As MEMEHtml = MEME.HTML.LoadDocument($"{MEME_OUT}/{ObjectId}/meme.html")
                          Select MAST.HTML.MatchMEMEAndMast(MEME_doc, MASTDoc)).ToVector
            Return LQuery
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="MEME_Text"></param>
        ''' <param name="MAST_html"></param>
        ''' <param name="GenomeSequence"></param>
        ''' <param name="GeneBriefInformation"></param>
        ''' <param name="ATGDistance"></param>
        ''' <param name="FilterPromoter">由于某一个位点是落在某一个基因的内部或者下游区的，所以使用这个参数来过滤掉这些不在启动子区的位点，
        ''' 本参数为真，则执行过滤操作，返回的记过之中仅包含有启动子区的位点，不为真，则返回所有类型的位点。
        ''' 默认不进行过滤操作</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Footprint.Generate.From.Text")>
        Public Function FootprintMatchesTEXT(MEME_Text As String,
                                             MAST_html As String,
                                             GenomeSequence As IPolymerSequenceModel,
                                             GeneBriefInformation As PTT,
                                             Optional ATGDistance As Integer = 500,
                                             Optional FilterPromoter As Boolean = False) As VirtualFootprints()

            Dim MEMEText As LDM.Motif() = DocumentFormat.MEME.Text.Load(MEME_Text)
            Dim Mast As MASTHtml = DocumentFormat.MAST.HTML.LoadDocument_v410(MAST_html, False)
            Dim Footprint = (From MastEntry As MatchedSite
                             In Mast.MatchedSites.AsParallel
                             Let p As Integer = MastEntry.MotifId - 1
                             Let MEME As LDM.Motif = MEMEText(p)
                             Select VirtualFootprintAPI.CreateMotifSiteInfo(Of
                                 ComponentModels.GeneBrief)(
                                 MEME,
                                 MastEntry,
                                 GenomeSequence,
                                 GeneBriefInformation,
                                 ATGDistance)).Unlist
            If FilterPromoter Then Footprint = (From fp As VirtualFootprints
                                                In Footprint
                                                Where fp.Distance < 0
                                                Select fp).AsList
            Return Footprint.ToArray
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="data1"></param>
        ''' <param name="data2"></param>
        ''' <param name="export">导出的文件夹</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Site.Diff")>
        Public Function Diff(data1 As IEnumerable(Of VirtualFootprints),
                             data2 As IEnumerable(Of VirtualFootprints),
                             export As String,
                             Optional data1Tag As String = "data1",
                             Optional data2Tag As String = "data2") As Boolean

            Dim DiffData As New List(Of VirtualFootprints), DiffIdentity As New List(Of VirtualFootprints) '差异的数据和一致的数据
            Dim setValue = New SetValue(Of VirtualFootprints) <= NameOf(VirtualFootprints.MotifId)

            data1Tag = data1Tag & "."
            data2Tag = data2Tag & "."

            For Each Site In data1
                Dim LQuery = (From site2 As VirtualFootprints In data2.AsParallel
                              Where String.Equals(Site.ORF, site2.ORF) AndAlso
                                  Math.Abs(site2.Starts - Site.Starts) <= sys.Min(Site.Length, site2.Length) * 0.85 AndAlso
                                  Math.Abs(Site.Length - site2.Length) <= sys.Min(Site.Length, site2.Length) * 0.85
                              Select setValue(site2, data2Tag & site2.MotifId)).ToArray
                If Not LQuery.IsNullOrEmpty Then
                    '找得到对应的位点，则是一致的数据
                    Call DiffIdentity.Add(setValue(Site, data1Tag & Site.MotifId))
                    Call DiffIdentity.AddRange(LQuery)
                    Call ("data identity at   ===> " & Site.ToString).__DEBUG_ECHO
                Else
                    Call DiffData.Add(setValue(Site, data1Tag & Site.MotifId))
                End If
            Next

            Call DiffData.AddRange((From site As VirtualFootprints
                                    In data2.AsParallel
                                    Where DiffIdentity.IndexOf(site) = -1
                                    Select setValue(site, data2Tag & site.MotifId)).ToArray)

            Call DiffData.SaveTo(export & "/DiffData.csv", False)
            Call DiffIdentity.SaveTo(export & "/DiffIdentity.csv", False)

            Return True
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="KEGGPathway">数据存放的文件夹</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <param name="EXPORT">结果数据的导出文件名，当参数不为空的时候，会导出一个Csv文件</param>
        <ExportAPI("KEGG.Pathway.Associates", Info:="Associates the virtual footprint data with the KEGG pathways.")>
        Public Function PathwayFunctionAssociation(data As IEnumerable(Of VirtualFootprints),
                                                   <Parameter("Dir.KEGG.Pathway", "Directory which stores of the KEGG pathway data that download from the KEGG database.")>
                                                   KEGGPathway As String,
                                                   Optional EXPORT As String = "") As NamedVector(Of String)()

            Dim LoadKeggPathways = (From path As KeyValuePair(Of String, String)
                                    In KEGGPathway.LoadSourceEntryList({"*.xml"}).AsParallel
                                    Select path.Value.LoadXml(Of bGetObject.Pathway)()).ToArray
            Dim AssociationLQuery = (From site As VirtualFootprints
                                     In data
                                     Where Not String.IsNullOrEmpty(site.ORF)
                                     Select Gene = site.ORF,
                                         Tag = site.MotifId,
                                         func = (From pathway As bGetObject.Pathway
                                                 In LoadKeggPathways
                                                 Where pathway.IsContainsGeneObject(site.ORF)
                                                 Select pathway.EntryId,
                                                     pathway.Name,
                                                     pathway.Description).ToArray).ToArray

            If Not String.IsNullOrEmpty(EXPORT) Then
                Dim Csv As New IO.File
                Call Csv.Add({"Gene.ID", "Gene.Tag", "KEGG.Pathways", "PathwaysTagData"})
                Call Csv.AppendRange((From line In AssociationLQuery
                                      Select CType(
                                          {
                                              line.Gene,
                                              line.Tag,
                                              String.Join("; ", (From obj In line.func Select obj.EntryId).ToArray),
                                              String.Join(";  ", (From obj In line.func Select obj.Description).ToArray)
                                          }, RowObject)).ToArray)
                Call Csv.Save(EXPORT, False)
            End If

            Return (From obj In AssociationLQuery
                    Select New NamedVector(Of String) With {
                        .name = obj.Gene,
                        .vector = (From objt In obj.func Select objt.EntryId).ToArray}).ToArray
        End Function

        ''' <summary>
        ''' 使用Lambda表达式有一个很严重的BUG，只能够使用本方法来消除这个BUG了
        ''' </summary>
        ''' <remarks></remarks>
        Private Class GetPccValue
            Dim PccMatrix As PccMatrix
            Dim Cutoff As Double
            Dim GetValueMethod As Func(Of Double, String)

            Sub New(PccMatrix As PccMatrix, pccCutoff As Double)
                Me.Cutoff = pccCutoff
                Me.PccMatrix = PccMatrix

                If Not PccMatrix Is Nothing AndAlso PccMatrix.PCC_SPCC_MixedType Then
                    Call "Target matrix is a mixed type matrix".__DEBUG_ECHO
                    GetValueMethod = AddressOf InternalGetMixedMATValue
                Else
                    GetValueMethod = AddressOf InternalGetPccMATValue
                End If
            End Sub

            Private Function InternalGetMixedMATValue(pcc As Double) As String
                If pcc <> 0.0R Then
                    Return pcc.ToString
                Else
                    Return ""
                End If
            End Function

            Private Function InternalGetPccMATValue(pcc As Double) As String
                If pcc >= Cutoff OrElse pcc <= -0.65 Then
                    Return pcc.ToString
                Else
                    Return ""
                End If
            End Function

            ''' <summary>
            ''' 如果PCC矩阵是一个混合矩阵，则会直接忽略阈值筛选
            ''' </summary>
            ''' <param name="RegulatorId"></param>
            ''' <param name="ORF"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function GetPcc(RegulatorId As String, ORF As String) As String
                Dim Pcc As Double = PccMatrix.GetValue(RegulatorId, ORF)
                Return Me.GetValueMethod(Pcc)
            End Function

            Public Shared Function EmptyReader(Effect As String, cutoff As String) As String
                Return "1"
            End Function
        End Class

        <ExportAPI("Data.Filtering.Empty")>
        Public Function FilteringEmptyData(data As IEnumerable(Of PredictedRegulationFootprint)) As PredictedRegulationFootprint()
            Dim LQuery = (From prf As PredictedRegulationFootprint
                          In data.AsParallel
                          Where Not String.IsNullOrEmpty(prf.ORF) AndAlso
                              Not String.IsNullOrEmpty(prf.Regulator)'s.IsNullOrEmpty
                          Select prf
                          Order By prf.MotifId).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 可以使用本方法来假设Pcc数据，以方便后面的模拟计算的实验
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="RegulatorMatches"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 对于所有基因调控关系对，都假设为0.85，根据Regprecise数据库之中所记录的Effect来猜测可能的符号
        ''' </remarks>
        ''' 
        <ExportAPI("pcc.make_assumption", Info:="Please notice that the pcc just the assumption value! and all of the data without regulator property will be trimmed.")>
        Public Function PccAssumption(data As IEnumerable(Of PredictedRegulationFootprint), RegulatorMatches As IEnumerable(Of RegpreciseMPBBH)) As PredictedRegulationFootprint()

            'Dim LQuery = (From item As PredictedRegulationFootprint
            '              In data.AsParallel
            '              Where Not String.IsNullOrEmpty(item.Regulator)'s.IsNullOrEmpty
            '              Let PccVector As String() = (From regulator As String
            '                                           In item.Regulators
            '                                           Let Pcc As String = __getEffect(item, regulator, RegulatorMatches)
            '                                           Select Pcc).ToArray
            '              Select item.InvokeSet(NameOf(item.corr), PccVector)).ToArray '.AsParallel
            'Return LQuery
        End Function

        Private Function __getEffect(footprint As PredictedRegulationFootprint,
                                     predictedRegulator As String,
                                     db As IEnumerable(Of Regprecise.RegpreciseMPBBH)) As String

            '查找条件，可以在记录之中查找到，motif编号一致
            Dim LQuery = (From rgBBH In db
                          Where String.Equals(predictedRegulator, rgBBH.QueryName)
                          Select rgBBH).ToArray
            LQuery = (From item In LQuery
                      Where String.Equals(item.Family, footprint.MotifFamily) AndAlso
                          String.Equals(footprint.RegulatorTrace, item.GetLocusTag)
                      Select item).ToArray
            Dim Effect As String = If(LQuery.IsNullOrEmpty, "0.5", Effect2Pcc(LQuery.First.regulationMode))
            Return Effect
        End Function

        Private Function Effect2Pcc(effect As String) As String
            If InStr(effect, "activator", CompareMethod.Text) > 0 Then
                If InStr(effect, "repressor", CompareMethod.Text) > 0 Then
                    Return "0.65"
                Else
                    Return "0.85"
                End If
            Else
                Return "-0.8"
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="MotifDatabase"></param>
        ''' <param name="RegulatorMatches"></param>
        ''' <param name="GenomeBrief"></param>
        ''' <param name="Door"></param>
        ''' <param name="KEGG_Pathways"></param>
        ''' <param name="PccMatrix">PCC或者SPCC的混合矩阵</param>
        ''' <param name="ignoreDirection"></param>
        ''' <param name="pccCutoff"></param>
        ''' <param name="ATGDistance"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Regulation.Analysis", Info:="Export the footprint analysis data from the MEME calculation data.")>
        Public Function Matches(data As IEnumerable(Of MEMEOutput),
                                MotifDatabase As IEnumerable(Of MotifDb.MotifFamily),
                                RegulatorMatches As IEnumerable(Of RegpreciseMPBBH),
                                GenomeBrief As PTTDbLoader,
                                Door As String,
                                KEGG_Pathways As IEnumerable(Of KEGG.Archives.Csv.Pathway),
                                Optional PccMatrix As PccMatrix = Nothing,
                                <Parameter("Direction.Ignored")> Optional ignoreDirection As Boolean = True,
                                <Parameter("PCC.Cutoff")> Optional pccCutoff As Double = 0.85,
                                Optional ATGDistance As Integer = 500) As PredictedRegulationFootprint()

            Call Console.WriteLine("{0} motifs records", MotifDatabase.Count)

            Dim MotifEntries = MotifDatabase.CreateEntryDictionary
            Dim PccReader As GetPccValue = New GetPccValue(PccMatrix, pccCutoff)
            Dim GetPcc As Func(Of String, String, String)
            If PccMatrix Is Nothing Then
                GetPcc = AddressOf GetPccValue.EmptyReader
            Else
                GetPcc = AddressOf PccReader.GetPcc
            End If

            Dim SequenceData = NucleicAcid.CreateObject(GenomeBrief.GenomeFasta.SequenceData)
            Dim LQuery = (From item As MEMEOutput
                          In data.AsParallel
                          Select PredictedRegulationFootprint.__createRegulationObject(item, SequenceData, GenomeBrief, ignoreDirection, ATGDistance)).ToArray.ToVector
            Dim DoorOperonPromoters = (From Operon As SMRUCC.genomics.Assembly.DOOR.Operon
                                       In DOOR_API.Load(Door).DOOROperonView.Operons
                                       Select PromoterGene = Operon.InitialX.Synonym,
                                              DoorData = Operon,
                                              DoorId = Operon.Key,
                                              OperonGenes = (From Gene In Operon Select Gene.Key).ToArray).ToDictionary(Function(Operon) Operon.PromoterGene)

            Dim AssignOperonDataA = Function(Regulation As PredictedRegulationFootprint) As PredictedRegulationFootprint
                                        If String.IsNullOrEmpty(Regulation.ORF) Then Return Regulation

                                        Dim GetOperonId = (From item In DoorOperonPromoters.Values Where Array.IndexOf(item.OperonGenes, Regulation.ORF) > -1 Select item).ToArray

                                        If GetOperonId.IsNullOrEmpty Then
                                            '  Regulation.RNAGene = "1"
                                            Return Regulation 'RNA基因在Door之中是找不到记录的
                                        Else
                                            '   Regulation.RNAGene = " "
                                        End If

                                        Dim Operon = GetOperonId.First
                                        Regulation.DoorId = Operon.DoorId
                                        Regulation.InitX = If(String.Equals(Operon.PromoterGene, Regulation.ORF), "1", " ")
                                        Regulation.StructGenes = (From item In Operon.DoorData.GetLast(Regulation.ORF) Select item.Synonym).ToArray

                                        Return Regulation
                                    End Function

            KEGG_Pathways = (From item In KEGG_Pathways.AsParallel Where Not item.PathwayGenes.IsNullOrEmpty Select item).ToArray

            Dim AssignOperonData As PredictedRegulationFootprint() = (From Regulation In LQuery.AsParallel Select AssignOperonDataA(Regulation)).ToArray
            Dim AssignPhenotype As Func(Of PredictedRegulationFootprint, PredictedRegulationFootprint) =
                Function(Regulation As PredictedRegulationFootprint) As PredictedRegulationFootprint
                    If Not String.IsNullOrEmpty(Regulation.ORF) Then
                        Dim Pathways = (From item As SMRUCC.genomics.Assembly.KEGG.Archives.Csv.Pathway In KEGG_Pathways
                                        Where Array.IndexOf(item.PathwayGenes, Regulation.ORF) > -1
                                        Select item).FirstOrDefault
                        If Not Pathways Is Nothing Then
                            Regulation.Class = Pathways.Class ' (From item In Pathways Select item.Class).ToArray
                            Regulation.Category = Pathways.Category ' (From item In Pathways Select item.Category).ToArray
                        End If

                    End If
                    Return Regulation
                End Function

#Const DEBUG = 0

#If DEBUG Then
        Call Randomize()
        Call LQuery.SaveTo(My.Computer.FileSystem.CurrentDirectory & "/" & Rnd() & "debug.csv", False)
#End If

            '将物种前缀去除
            Dim MatchData = (From item In RegulatorMatches.AsParallel Select New KeyValuePair(Of String, String)(item.HitName.Split(CChar(":")).Last, item.QueryName)).ToArray
            Dim MatchedRegulators = (From Regulation In LQuery.AsParallel Select MatchRegulator(Regulation, MotifEntries, MatchData, GetPcc)).ToArray.Unlist  '.AsParallel
            Dim AssignPathwayPhenotypesResult = (From Regulation As PredictedRegulationFootprint
                                                 In MatchedRegulators.AsParallel
                                                 Select AssignPhenotype(Regulation)).ToArray

            Return AssignPathwayPhenotypesResult
        End Function

        ''' <summary>
        ''' Export a cytoscape network file from the predicted footprint data.(从所预测的footprint调控数据之中导出一个cytoscape网络的定义文件)
        ''' </summary>
        ''' <param name="Regulations"></param>
        ''' <param name="saveto">所导出的文件夹</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("write.csv.cytoscape_network", Info:="Export a cytoscape network file from the predicted footprint data.")>
        Public Function GenerateNetwork(Regulations As IEnumerable(Of PredictedRegulationFootprint), saveto As String) As Boolean
            'Dim FiltedData As PredictedRegulationFootprint() = (From footprint_regulation As PredictedRegulationFootprint
            '                                                    In Regulations.AsParallel
            '                                                    Where Not (String.IsNullOrEmpty(footprint_regulation.ORF) OrElse String.IsNullOrEmpty(footprint_regulation.Regulator)'s.IsNullOrEmpty)
            '                                                    Select footprint_regulation).ToArray
            ''对于第一个基因是直接调控的
            ''对于操纵子之中的后半部分的基因是间接调控的, Pcc为第一个基因的一半
            'Dim LQuery = (From footprint_regulation As PredictedRegulationFootprint
            '              In FiltedData.AsParallel
            '              Let FirstRegulations = ExpandRegulatorRegulations(footprint_regulation, factor:=1)
            '              Let StructRegulations = (From struct In footprint_regulation.OperonRegulationCopies Select ExpandRegulatorRegulations(struct, factor:=0.65)).ToArray.MatrixToVector
            '              Select {FirstRegulations, StructRegulations}.MatrixToVector).ToArray.MatrixToVector

            'Dim Csv As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File =
            '    New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File From {{"Regulator", "Interaction", "ORF", "weight"}.ToCsvRow}
            'Call Csv.AppendRange(LQuery)
            'Dim FileData = Csv.GenerateDocument(False).Distinct.ToArray
            'Call FileIO.FileSystem.CreateDirectory(saveto)
            'Call IO.File.WriteAllLines(saveto & "/Edges.csv", FileData)

            'Csv = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File From {{"UniqueId", "NodeType"}.ToCsvRow}
            'Dim ChunkBuffer = (From item In FiltedData.AsParallel Select item.ORF, NodeType = "Gene").AsList
            'Call ChunkBuffer.AddRange((From item In FiltedData.AsParallel Select (From struct As String In item.StructGenes Select ORF = struct, NodeType = "Gene").ToArray).ToArray.MatrixToVector)
            'Call ChunkBuffer.AddRange((From item In FiltedData.AsParallel Select (From Regulator As String In item.Regulators Select ORF = Regulator, NodeType = "Regulator").ToArray).ToArray.MatrixToVector)

            'Dim gChunkBuffer = (From item In ChunkBuffer Select item Group item By item.ORF Into Group).ToArray
            'Dim Nodes = (From item In gChunkBuffer.AsParallel Let Types As String() = (From n In item.Group Select n.NodeType Distinct).ToArray Select {item.ORF, String.Join(";", Types)}.ToCsvRow).ToArray
            'Call Csv.AppendRange(Nodes)
            'Call Csv.Save(saveto & "/Nodes.csv", False)

            Return True
        End Function

        Private Function ExpandRegulatorRegulations(footprint_regulation As PredictedRegulationFootprint, factor As Double) As RowObject()
            'Dim LQuery = (From i As Integer In footprint_regulation.Regulators.Sequence
            '              Let RegulatorId As String = footprint_regulation.Regulators(i)
            '              Let Pcc As String = StructurePcc(footprint_regulation.corr(i), factor)
            '              Let DataRow = {RegulatorId, "Regulates", footprint_regulation.ORF, Pcc}.ToCsvRow
            '              Select DataRow).ToArray
            'Return LQuery
        End Function

        Private Function StructurePcc(pcc As String, factor As Double) As String
            Dim p As Double = Val(pcc)
            Dim value = pcc * factor
            Return value.ToString
        End Function

        Private Function MatchRegulator(Regulation As PredictedRegulationFootprint,
                                        MotifEntries As Dictionary(Of MotifDb.Motif),
                                        RegulatorMatches As KeyValuePair(Of String, String)(),
                                        GetPcc As Func(Of String, String, String)) As PredictedRegulationFootprint()

            If String.IsNullOrEmpty(Regulation.ORF) Then
                Return {Regulation}  'Overlap或者下游的基因，没有ORF赋值，则不再查找调控因子
            End If

            'Dim Matches = MatchRegulator(Regulation, MotifEntries, RegulatorMatches)
            'Regulation.Regulators = Matches.Value
            'Regulation.Regprecise = Matches.Key.Distinct.ToArray

            ''If String.IsNullOrEmpty(Regulation.ORF) Then Return Regulation

            'Dim RegTFPcc = (From RegulatorId As String
            '                In Regulation.Regulators
            '                Let PccValue As String = GetPcc(RegulatorId, Regulation.ORF)
            '                Where Not String.IsNullOrEmpty(PccValue)
            '                Select Pcc = PccValue, RegulatorId).ToArray

            'If Not RegTFPcc.IsNullOrEmpty Then
            '    Regulation.Regulators = (From item In RegTFPcc Select item.RegulatorId).ToArray
            '    Regulation.corr = (From item In RegTFPcc Let PccValue As String = item.Pcc.ToString Select PccValue).ToArray
            'Else
            '    Regulation.Regulators = New String() {}  '所有的调控因子都不符合筛选条件，则都删除掉
            '    Regulation.corr = New String() {}
            'End If

            'Return Regulation
        End Function

        <ExportAPI("/Merge.Footprints")>
        Public Function MergeFootprints(DIR As String) As PredictedRegulationFootprint()
            Dim LQuery = (From file As String
                          In DIR.EnumerateFiles("*.csv")
                          Select file.LoadCsv(Of PredictedRegulationFootprint)).IteratesALL
            Return LQuery.ToArray
        End Function

        <ExportAPI("Write.Csv.Regulon.Footprints")>
        Public Function SaveRegulations(data As IEnumerable(Of PredictedRegulationFootprint), saveto As String) As Boolean
            Return data.SaveTo(saveto, False)
        End Function

        <ExportAPI("Read.Csv.Regulon.Footprints")>
        Public Function ReadRegulations(path As String) As PredictedRegulationFootprint()
            Return path.LoadCsv(Of PredictedRegulationFootprint)(False).ToArray
        End Function

        <ExportAPI("Hash")>
        <Extension> Private Function CreateEntryDictionary(data As IEnumerable(Of MotifDb.MotifFamily)) As Dictionary(Of MotifDb.Motif)
            Dim ChunkBuffer = (From Family In data Select Family.Motifs).IteratesALL
            Return ChunkBuffer.ToDictionary
        End Function

        ''' <summary>
        ''' 通过<see cref="MEMEOutput.ObjectId"></see>
        ''' </summary>
        ''' <param name="item"></param>
        ''' <param name="MotifDb"></param>
        ''' <param name="maps"></param>
        ''' <returns>{RegpreciseRegulators(), bh_Matches()}</returns>
        ''' <remarks></remarks>
        Private Function MatchRegulator(item As PredictedRegulationFootprint,
                                        MotifDb As Dictionary(Of String, MotifDb.Motif),
                                        maps As KeyValuePair(Of String, String)()) As KeyValuePair(Of String(), String())

            Dim MotifRegulators = MotifDb(item.MotifId).RegpreciseRegulators
            Dim LQuery = (From matched In maps Where Array.IndexOf(MotifRegulators, matched.Key) > -1 Select matched.Value).ToArray
            Dim values As String() = (From strValue As String In LQuery Select strValue Distinct Order By strValue Ascending).ToArray

            Return New KeyValuePair(Of String(), String())(MotifRegulators, values)
        End Function

        ''' <summary>
        ''' 将Regprecise数据库之中的Moitf数据解析出来进行MEME分析之后，将MEME_OUT文件夹里面的数据整理出来
        ''' </summary>
        ''' <param name="MotifMEME_OUT"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Build.Db.Motifs")>
        Public Function CreateMotifInformations(MotifMEME_OUT As String, Regprecise As TranscriptionFactors) As MotifDb.MotifFamily()
            Dim LQuery = (From strPath As String
                          In FileIO.FileSystem.GetDirectories(MotifMEME_OUT).AsParallel
                          Let doc = MEME.HTML.LoadDocument(String.Format("{0}/meme.html", strPath))
                          Where Not doc.Motifs.IsNullOrEmpty
                          Select doc).ToArray
            Dim Chunkbuffer As MotifDb.MotifFamily() = (From Line As MEMEHtml
                                                        In LQuery.AsParallel
                                                        Select GenerateDocument(Line, Regprecise)).ToArray
            Return Chunkbuffer
        End Function

        Private Function GenerateDocument(line As MEMEHtml, Regprecise As TranscriptionFactors) As MotifDb.MotifFamily
            Dim Family As MotifDb.MotifFamily = New MotifDb.MotifFamily
            Family.Family = line.ObjectId
            Family.Motifs = (From item As Motif
                             In line.Motifs
                             Select MotifDb.Motif.CopyFrom(item, line.ObjectId)).ToArray

            For i As Integer = 0 To Family.Motifs.Length - 1
                Dim Motif = Family.Motifs(i)
                Motif.RegpreciseRegulators = (From item As SiteInfo
                                              In Motif.MatchedSites
                                              Let LocusTag = GetSiteTag(item.Name)
                                              Select Regprecise.GetRegulatorId(locus_tag:=LocusTag.Key, MotifPosition:=LocusTag.Value)).ToArray
                Motif.Family = ""
            Next

            Return Family
        End Function

        Private Function GetSiteTag(strData As String) As KeyValuePair(Of String, Integer)
            Dim strDataTemp = Regex.Match(strData, ".+?:-?\d+(\.\d+)?").Value()
            Dim Tokens As String() = strDataTemp.Split(CChar(":"))
            If String.IsNullOrEmpty(Tokens.Last) Then
                Console.WriteLine("DataException:= " & strDataTemp)
            End If
            Return New KeyValuePair(Of String, Integer)(Tokens.First, Val(Tokens.Last))
        End Function

        <ExportAPI("Write.Xml.Motif_Database")>
        Public Function WriteMotifDatabase(data As IEnumerable(Of MotifDb.MotifFamily), saveXML As String) As Boolean
            Return New MotifDb.MotifDatabase() With {
                .DatabaseBuildTime = Now.ToString,
                .MotifFamilies = data.ToArray()
            }.GetXml.SaveTo(saveXML)
        End Function

        <ExportAPI("Read.Xml.Motif_Database")>
        Public Function LoadMotifDatabase(path As String) As MotifDb.MotifFamily()
            Return path.LoadXml(Of MotifDb.MotifDatabase).MotifFamilies
        End Function

        ''' <summary>
        ''' 从调控数据之中导出真正被使用到的调控因子
        ''' </summary>
        ''' <param name="Regulations"></param>
        ''' <returns></returns>
        <ExportAPI("Export.Regulators")>
        Public Function ExportRegulators(Regulations As IEnumerable(Of PredictedRegulationFootprint)) As String()
            Dim LQuery = (From regulates As PredictedRegulationFootprint
                          In Regulations.AsParallel
                          Where Not String.IsNullOrEmpty(regulates.Regulator)
                          Select regulates.Regulator).ToArray
            LQuery = (From id As String
                      In LQuery
                      Where Not String.IsNullOrEmpty(id)
                      Select id
                      Distinct
                      Order By id Ascending).ToArray
            Return LQuery
        End Function
    End Module
End Namespace
