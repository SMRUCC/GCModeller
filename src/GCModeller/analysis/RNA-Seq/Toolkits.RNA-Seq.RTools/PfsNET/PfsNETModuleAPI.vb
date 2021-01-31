#Region "Microsoft.VisualBasic::fc0da98f938e3476b8e13f9bf448097a, analysis\RNA-Seq\Toolkits.RNA-Seq.RTools\PfsNET\PfsNETModuleAPI.vb"

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

    '     Module PfsNETModuleAPI
    ' 
    '         Function: __getPath, AnalysisPhenotype, BatchScript, CreateList, CreateMatrix
    '                   CreatePathwayData, DenormalizePhenotypeData, Evaluate, ExportCytoscape, GetRegulationGeneIdlist
    '                   Initialize, KEGGPathwaysPhenotypeAnalysis, LoadResult, (+2 Overloads) ParseCsv, PathwayGeneRelationship
    '                   ReadPfsnet, ReadRegulations, RegulationRelationship, SaveCsvResult, SavePathwayData
    '                   SavePfsNET, SavePFSNet, WriteKEGGPhenotypes
    '         Class Regulation
    ' 
    '             Properties: DoorId, Pcc, SequenceId, TF
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.ListExtensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.TabularArchives
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.PathwayBrief
Imports SMRUCC.genomics.ComponentModel

#Const DEBUG = True

Namespace PfsNET

    ''' <summary>
    ''' 文件1,2之中所放着的基因是自身所感兴趣的基因突变体的转录组表达值，文件三则是文件1,2之中的两两组合的基因的关系
    ''' </summary>
    ''' <remarks></remarks>
    <Package("PfsNET",
                 Description:="PFSNet computes signifiance of subnetworks generated through a process that selects genes in a pathway based on fuzzy scoring and a majority voting procedure.",
                        Cites:="Lim, K. and L. Wong (2014). ""Finding consistent disease subnetworks Using PFSNet."" Bioinformatics 30(2): 189-196.
<p> MOTIVATION: Microarray data analysis is often applied to characterize disease populations by identifying individual genes linked to the disease. In recent years, efforts have shifted to focus on sets of genes known to perform related biological functions (i.e. in the same pathways). Evaluating gene sets reduces the need to correct for false positives in multiple hypothesis testing. However, pathways are often large, and genes in the same pathway that do not contribute to the disease can cause a method to miss the pathway. In addition, large pathways may not give much insight to the cause of the disease. Moreover, when such a method is applied independently to two datasets of the same disease phenotypes, the two resulting lists of significant pathways often have low agreement. RESULTS: We present a powerful method, PFSNet, that identifies smaller parts of pathways (which we call subnetworks), and show that significant subnetworks (and the genes therein) discovered by PFSNet are up to 51% (64%) more consistent across independent datasets of the same disease phenotypes, even for datasets based on different platforms, than previously published methods. We further show that those methods which initially declared some large pathways to be insignificant would declare subnetworks detected by PFSNet in those large pathways to be significant, if they were given those subnetworks as input instead of the entire large pathways. AVAILABILITY: http://compbio.ddns.comp.nus.edu.sg:8080/pfsnet/

",
                        Publisher:="Kevin Lim and Limsoon Wong",
                        Url:="http://compbio.ddns.comp.nus.edu.sg:8080/pfsnet/")>
    <Cite(Title:="Finding consistent disease subnetworks using PFSNet",
          ISSN:="1367-4803;
1460-2059", Year:=2013, Volume:=30, Issue:="2", Pages:="189-196", DOI:="10.1093/bioinformatics/btt625",
          Journal:="Bioinformatics", Authors:="Lim, K.
Wong, L.", Abstract:="Motivation: Microarray data analysis is often applied to characterize disease populations by identifying individual genes linked to the disease. 
In recent years, efforts have shifted to focus on sets of genes known to perform related biological functions (i.e. in the same pathways). 
Evaluating gene sets reduces the need to correct for false positives in multiple hypothesis testing. However, pathways are often large, and genes in the same pathway that do not contribute to the disease can cause a method to miss the pathway. 
In addition, large pathways may not give much insight to the cause of the disease. Moreover, when such a method is applied independently to two data sets of the same disease phenotypes, the two resulting lists of significant pathways often have low agreement.
<p>
<p>Results:We present a powerful method, PFSNet, that identifies smaller parts of pathways (which we call subnetworks), and show that significant subnetworks (and the genes therein) discovered by PFSNet are up to 51% (64%) more consistent across independent datasets of the same disease phenotypes, even for datasets based on different platforms, than previously published methods. 
We further show that thosemethods which initially declared some large pathways to be insignificant would declare subnetworks detected by PFSNet in those large pathways to be significant, if they were given those subnetworks as input instead of the entire large pathways.
Availability: http://compbio.ddns.comp.nus.edu.sg:8080/pfsnet/", AuthorAddress:="kevinl@comp.nus.edu.sg")>
    Public Module PfsNETModuleAPI

        <ExportAPI("Session.Initialize")>
        Public Function Initialize(Optional Java_Path As String = "", Optional R_HOME As String = "") As Boolean
            Return PfsNETRInvoke.InitializeSession(Java_Path, R_HOME)
        End Function

        <ExportAPI("pathway_data.from_metacyc")>
        Public Function CreatePathwayData(MetaCyc As DatabaseLoadder) As PathwayBrief()
            Dim list = PwyFilters.Performance(MetaCyc)
            Dim PathwayData = PwyFilters.GenerateReport(list, MetaCyc.GetGenes)
            Return PathwayData
        End Function

        <ExportAPI("Write.Csv.Pathway")>
        Public Function SavePathwayData(pathways As IEnumerable(Of PathwayBrief), saveCsv As String) As Boolean
            Return pathways.SaveTo(saveCsv, False)
        End Function

        ''' <summary>
        ''' 生成文件3
        ''' </summary>
        ''' <returns>返回所保存的文件的路径</returns>
        ''' <remarks></remarks>
        <ExportAPI("relationship.from_pathways")>
        Public Function PathwayGeneRelationship(pathways As IEnumerable(Of Annotation.PathwayBrief),
                                                Optional pathwayIds As IEnumerable(Of String) = Nothing,
                                                Optional saveto As String = "") As String
            Dim bufs As New List(Of String)

            Dim strPathwayIds As String()

            If Not pathwayIds Is Nothing Then
                strPathwayIds = LinqAPI.Exec(Of String) <= From id As String
                                                           In pathwayIds
                                                           Let value As String = id.ToString.ToUpper
                                                           Select value
            Else
                strPathwayIds = (From item In pathways Select item.EntryId).ToArray
            End If

            For Each PathwayInfo As Annotation.PathwayBrief In pathways
                If Array.IndexOf(strPathwayIds, PathwayInfo.EntryId) = -1 OrElse
                    PathwayInfo.GetPathwayGenes.IsNullOrEmpty Then

                    Continue For
                End If

                Dim Combo As Comb(Of String) = Comb(Of String).CreateObject(PathwayInfo.GetPathwayGenes)

                For Each Line In Combo.CombList
                    bufs += From pair As Tuple(Of String, String)
                            In Line
                            Let strPair As String() = (From strId As String
                                                       In pair.Iterates
                                                       Select strId
                                                       Order By strId Ascending).ToArray
                            Select String.Join(vbTab, PathwayInfo.EntryId, strPair(0), strPair(1))
                Next
            Next

            Dim file As String = (From strLine As String In bufs Select strLine Distinct).JoinBy(vbLf)

            saveto = __getPath(saveto, "pathway_relationship")
            file.SaveTo(saveto, encoding:=Encoding.ASCII)

            If FileIO.FileSystem.FileExists(saveto) Then
                Call Console.WriteLine($"Gene releationship network was saved at ""{saveto.ToFileURL}""")
            Else
                Dim ex As New Exception("Gene releationship network file saved failured!")
                ex = New Exception(file, ex)
                Throw ex
            End If

            Return saveto
        End Function

        Private Function __getPath(path As String, prefix As String) As String
            If String.IsNullOrEmpty(path) Then
                path = $"./{prefix}_pid_{App.PID}_{App.NextTempName}.tmp"
            End If

            Return FileIO.FileSystem.GetFileInfo(path).FullName
        End Function

        <ExportAPI("relationship.from_regulations")>
        Public Function RegulationRelationship(regulations As PfsNETModuleAPI.Regulation(), Optional saveto As String = "") As String
            Dim Activations = (From Line In regulations Where Line.Pcc > 0 Select (From GeneId As String In Line.SequenceId Select String.Join(vbTab, "Activated", Line.TF, GeneId)).ToArray).ToArray
            Dim Repressions = (From Line In regulations Where Line.Pcc < 0 Select (From GeneId As String In Line.SequenceId Select String.Join(vbTab, "Repressed", Line.TF, GeneId)).ToArray).ToArray
            Dim ChunkBuffer As List(Of String) = New List(Of String)
            For Each Line In Activations
                Call ChunkBuffer.AddRange(Line)
            Next
            For Each Line In Repressions
                Call ChunkBuffer.AddRange(Line)
            Next
            ChunkBuffer = New List(Of String)(ChunkBuffer.Distinct)
            '  Dim RemovedLines As String() = (From strLine As String In ChunkBuffer Where (From GeneId As String In _RemovedList Where InStr(strLine, GeneId) > 0 Select 1).ToArray.Count > 0 Select strLine).ToArray
            '  For Each strLine As String In RemovedLines
            'Call ChunkBuffer.Remove(strLine)
            '  Next

            saveto = __getPath(saveto, "regulations_relationship")
            Call IO.File.WriteAllLines(path:=saveto, contents:=ChunkBuffer.ToArray, encoding:=Encoding.ASCII)

            If FileIO.FileSystem.FileExists(saveto) Then
                Call Console.WriteLine("Gene releationship network was saved at ""{0}""", saveto)
            Else
                Throw New Exception("Gene releationship network file saved failured!")
            End If

            Return saveto
        End Function

        <ExportAPI("read.regulations")>
        Public Function ReadRegulations(path As String) As Regulation()
            Return path.LoadCsv(Of Regulation)(False).ToArray
        End Function

        <ExportAPI("get.gene_idlist")>
        Public Function GetRegulationGeneIdlist(regulations As PfsNETModuleAPI.Regulation()) As String()
            Dim gIds As List(Of String) =
                regulations.Select(Function(r) r.TF).AsList +
                regulations.Select(Function(x) x.SequenceId)

            Return LinqAPI.Exec(Of String) <= From strId As String
                                              In gIds
                                              Select strId
                                              Order By strId Ascending
                                              Distinct
        End Function

        Public Class Regulation
            Public Property DoorId As String
            Public Property TF As String
            Public Property SequenceId As String()
            <Column("Pcc.TF->Operon")>
            Public Property Pcc As Double
        End Class

        ''' <summary>
        ''' 生成与特定表型相关的代谢途径中的所有的基因列表，生成文件1,2所需要的
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("pathway_genelist.create_from")>
        Public Function CreateList(pathwaydata As IEnumerable(Of Annotation.PathwayBrief), Optional pathwayIds As IEnumerable(Of String) = Nothing) As String()
            Dim strPathwayIds As String()

            If Not pathwayIds Is Nothing Then
                strPathwayIds = LinqAPI.Exec(Of String) <=
                    From pId As String
                    In pathwayIds
                    Let value As String = pId.ToString.ToUpper
                    Select value
            Else
                strPathwayIds = (From item In pathwaydata Select item.EntryId).ToArray
            End If

            Dim gIds As New List(Of String)

            For Each brief As Annotation.PathwayBrief
                In From pwy As Annotation.PathwayBrief
                   In pathwaydata
                   Where Array.IndexOf(strPathwayIds, pwy.EntryId) > -1
                   Select pwy

                Dim PathwayGenes As String() = brief.GetPathwayGenes

                If Not PathwayGenes.IsNullOrEmpty Then
                    gIds += brief.GetPathwayGenes
                End If
            Next

            Return LinqAPI.Exec(Of String) <= From strId As String
                                              In gIds
                                              Select strId
                                              Distinct
                                              Order By strId Ascending
        End Function

        ' Dim _RemovedList As String()

        ''' <summary>
        ''' If the experiment_ids parameter is null or empty then all of the chipdata will be write to the file.
        ''' 创建文件1和文件2
        ''' </summary>
        ''' <param name="chipdata"></param>
        ''' <param name="Experiments"></param>
        ''' <param name="lstLocus">感兴趣的表型相关的基因列表，本列表需要专门的一个函数来生成</param>
        ''' <param name="saveTxt"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateMatrix(Chipdata As MatrixFrame,
                                     <Parameter("lst.Locus", "A list of gene id.")>
                                     lstLocus As IEnumerable(Of String),
                                     <Parameter("lst.Expers", "Id list of the RNA-seq experiments.")>
                                     Optional Experiments As IEnumerable(Of String) = Nothing,
                                     Optional saveTxt As String = "",
                                     Optional DEBUG As Boolean = False) As String

            Dim expIds As String()

            If Experiments Is Nothing Then
                expIds = Chipdata.LstExperiments
            Else
                expIds = LinqAPI.Exec(Of String) <=
                    From eId As String
                    In Experiments
                    Let value As String = eId.ToString
                    Select value
            End If

            lstLocus = From item In lstLocus Let strValue As String = item.ToString Select strValue

            Dim exprs As List(Of NamedValue(Of List(Of Double))) =
                LinqAPI.MakeList(Of NamedValue(Of List(Of Double))) <=
                    From strId As String
                    In lstLocus
                    Select New NamedValue(Of List(Of Double)) With {
                        .Name = strId,
                        .Value = New List(Of Double)
                    }

            For Each experimentId As String In expIds
                Call Chipdata.SetColumnAuto(experimentId)

                For Each item In exprs
                    Call item.Value.Add(Chipdata.GetValue(locusTag:=item.Name, DEBUGInfo:=DEBUG))
                Next
            Next

            '当执行了Remove操作之后，可能会产生行数不一致而导致的R错误，故而在这里取消了Remove操作

            'Dim GetRemovedList = (From item In Chunkbuffer Where item.DataChunk.Sum = 0.0R Select item).ToArray
            'For Each item In GetRemovedList
            'Call Chunkbuffer.Remove(item)
            'Next

            '_RemovedList = (From item In GetRemovedList Select item.GeneId).ToArray

            saveTxt = __getPath(saveTxt, "expression_matrix")

            Dim sb As New StringBuilder(1024)

            For Each strLine In exprs
                Call sb.AppendLine(String.Join(vbTab, strLine.Name, String.Join(vbTab, strLine.Value.ToArray)))
            Next

            Call sb.Remove(sb.Length - 2, 2)
            Call sb.SaveTo(saveTxt, encoding:=Encoding.ASCII)

            If FileIO.FileSystem.FileExists(saveTxt) Then
                Call Console.WriteLine("Expression matrix was saved at ""{0}""", saveTxt)
            Else
                Throw New Exception("Expression Matrix file saved failured!")
            End If

            Return saveTxt
        End Function

        ''' <summary>
        ''' 使用PfsNET计算突变体表型变化数据
        ''' </summary>
        ''' <param name="file1"></param>
        ''' <param name="file2"></param>
        ''' <param name="file3"></param>
        ''' <param name="b"></param>
        ''' <param name="t1"></param>
        ''' <param name="t2"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("PfsNET.Evaluate")>
        Public Function Evaluate(file1 As String, file2 As String, file3 As String,
                                 Optional b As String = "0.5",
                                 Optional t1 As String = "0.95",
                                 Optional t2 As String = "0.85",
                                 Optional n As String = "1000") As PFSNetResultOut

            ' Return PfsNETModuleAPI.PFSNet_EvaluateHandle(file1, file2, file3, b, t1, t2, n)
        End Function

        <ExportAPI("phenotypes.automatically")>
        Public Function AnalysisPhenotype(phenList As String,
                                          chipdata As MatrixFrame,
                                          pathways As IKeyValuePairObject(Of String, String())(),
                                          export As String) As Integer

            Dim ComboList = Comb(Of String).CreateObject(IO.File.ReadAllLines(phenList))
            Dim GeneList = CreateList(pathways)
            Dim file3 As String = ""

            For Each Comb In ComboList.CombList
                For Each item In Comb
                    Dim p1 = CreateMatrix(chipdata, GeneList, item.Item1.Split(CChar(",")))
                    Dim p2 = CreateMatrix(chipdata, GeneList, item.Item2.Split(CChar(",")))

                    If String.IsNullOrEmpty(file3) Then
                        file3 = PathwayGeneRelationship(pathways)
                    End If

                    Dim savePath As String = String.Format("{0}/{1}_{2}.xml", export, item.Item1, item.Item2)
                    Try
                        Dim result = PfsNETRInvoke.Evaluate(p1, p2, file3, 0.5, 0.8, 0.8)
                        Call SavePfsNET(result, saveCsv:=savePath)
                    Catch ex As Exception
                        Call Console.WriteLine(ex.ToString)
                        Call FileIO.FileSystem.WriteAllText(file:=savePath, text:="PfsNET.ERROR" & vbCrLf & vbCrLf & ex.ToString, append:=False)
                    End Try
                Next
            Next

            Return 0
        End Function

        <ExportAPI("export.cytoscape_cell_phenotype")>
        Public Function ExportCytoscape(data As IEnumerable(Of KEGGPhenotypes), saveto As String) As Boolean
            Return KEGGPhenotypes.ExportCytoscape(data, saveto)
        End Function

        ''' <summary>
        ''' 将xml格式的结果文件转换为csv格式的数据文件
        ''' </summary>
        ''' <param name="Imports"></param>
        ''' <param name="PathwayBriefs"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("generate.csv_result")>
        Public Function ParseCsv([Imports] As String, PathwayBriefs As String) As TabularArchives.SubNetTable()
            Dim DictPathwayBriefs As New Dictionary(Of String, Annotation.PathwayBrief)
            For Each item In PathwayBriefs.LoadCsv(Of PathwayBrief)(False)
                Call DictPathwayBriefs.Add(item.EntryId, item)
            Next

            Dim XmlFiles = FileIO.FileSystem.GetFiles([Imports], FileIO.SearchOption.SearchTopLevelOnly, "*.xml").ToArray
            Dim ChunkBuffer As List(Of TabularArchives.SubNetTable) = New List(Of TabularArchives.SubNetTable)
            For Each File As String In XmlFiles
                Call ChunkBuffer.AddRange(TabularArchives.SubNetTable.CreateObject(File, DictPathwayBriefs))
            Next

            Return ChunkBuffer.ToArray
        End Function

        ''' <summary>
        ''' export the pfsnet data log file into the csv data file.
        ''' 将原始的计算数据导出为Csv文件，与<see cref="ParseCsv"></see>所不同的是，所导出的对象在本函数之中为原始计算数据的log文件，而在另外一个重载函数之中则为解析好的xml文件
        ''' </summary>
        ''' <param name="imports"></param>
        ''' <param name="PathwayBriefs"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ParseCsv([imports] As String, PathwayBriefs As IEnumerable(Of Annotation.PathwayBrief)) As TabularArchives.SubNetTable()
            Dim DictPathwayBriefs As New Dictionary(Of String, Annotation.PathwayBrief)

            For Each item In PathwayBriefs
                Call DictPathwayBriefs.Add(item.EntryId, item)
            Next

            Dim dataFiles As IEnumerable(Of String) = ls - l - wildcards("*.txt") <= [imports]
            Dim LQuery As TabularArchives.SubNetTable() =
                LinqAPI.Exec(Of TabularArchives.SubNetTable) <= From File As String
                                                                In dataFiles.AsParallel
                                                                Let pName As String = File.BaseName.ToLower
                                                                Let dataParsed = SubnetParser.TryParse(IO.File.ReadAllLines(File))
                                                                Let p1 As String = pName & ".Class1"
                                                                Let p2 As String = pName & ".Class2"
                                                                Let a = TabularArchives.SubNetTable.CreateObject(dataParsed.Key, p1, DictPathwayBriefs)
                                                                Let b = TabularArchives.SubNetTable.CreateObject(dataParsed.Value, p2, DictPathwayBriefs)
                                                                Select {a, b}.IteratesALL
            Return LQuery
        End Function

        <ExportAPI("Write.Csv.PfsNET")>
        Public Function SaveCsvResult(data As IEnumerable(Of TabularArchives.SubNetTable), saveto As String) As Boolean
            Return data.SaveTo(saveto, False)
        End Function

        <ExportAPI("Read.Csv.PfsNET")>
        Public Function ReadPfsnet(path As String) As TabularArchives.SubNetTable()
            Return path.LoadCsv(Of TabularArchives.SubNetTable)(False).ToArray
        End Function

        ''' <summary>
        ''' 生成用于启动pfsnet批量分析所使用的批处理脚本
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="saveCsv"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("write.pfsnet")>
        Public Function SavePfsNET(data As PFSNetResultOut, saveCsv As String) As Boolean
            Call data.STD_OUTPUT.FlushAllLines(saveCsv & ".txt", Encodings.ASCII)
            Return data.GetXml.SaveTo(saveCsv)
        End Function

        ''' <summary>
        ''' parameter export is the directory of the pfsnet data will be saved.
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="EXPORT"></param>
        ''' <returns></returns>
        Public Function SavePFSNet(data As IEnumerable(Of PFSNetResultOut), EXPORT As String) As Boolean
            For Each i As SeqValue(Of PFSNetResultOut) In data.SeqIterator
                Dim net As PFSNetResultOut = i.value
                Dim name As String = If(String.IsNullOrEmpty(net.tag), CStr(i.i), net.tag)
                Dim path As String = $"{EXPORT}/{name}.xml"

                Call SavePfsNET(net, path)
            Next

            Return True
        End Function

        <ExportAPI("batch_script.generate")>
        Public Function BatchScript(phenlist As String, scriptfile As String, saveto As String, shell As String) As Integer
            Dim clist As Comb(Of String) =
                Comb(Of String).CreateObject(IO.File.ReadAllLines(phenlist))
            Dim CMD As New StringBuilder(1024)
            Dim parentDir As String = FileIO.FileSystem.GetParentPath(saveto)

            For Each Comb In clist.CombList
                For Each item In Comb
                    Dim saveFile As String = String.Format("{0}/PfsNET.OUT/{1}_____{2}.xml", parentDir, item.Item1, item.Item2)
                    Call CMD.AppendLine(String.Format("start /b {0} ""{1}"" p1 ""{2}"" p2 ""{3}"" save ""{4}""", shell, scriptfile, item.Item1, item.Item2, saveFile))
                Next
            Next

            Return CMD.SaveTo(saveto, Encoding.ASCII).CLICode
        End Function

        ''' <summary>
        ''' 从pfsNET所产生的R输出的文本文件之中解析出结果，以用于XML数据或者Csv数据的保存和下一步分析
        ''' </summary>
        ''' <param name="testfile"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("pfsnet_log.parse")>
        Public Function LoadResult(testfile As String) As PfsNET()
            Dim strLines As String() = IO.File.ReadAllLines(testfile)
            Dim result = SubnetParser.TryParse(strLines)
            Dim data = {result.Key, result.Value}.ToVector
            Return data
        End Function

        ''' <summary>
        ''' Associate the kegg pathways pfsnet calculation result with the kegg pathways phenotypes to see which route that the mutated gene can affected the specific phenotype.
        ''' </summary>
        ''' <param name="pfsnet"></param>
        ''' <param name="KEGGPathways"></param>
        ''' <returns></returns>
        Public Function KEGGPathwaysPhenotypeAnalysis(pfsnet As IEnumerable(Of TabularArchives.SubNetTable),
                                                      KEGGPathways As IEnumerable(Of KEGG.Archives.Csv.Pathway)) As KEGGPhenotypes()
            Dim ChunkBuffer = KEGGPhenotypes.PhenotypeAssociations(Result:=pfsnet.ToArray, KEGGPathways:=KEGGPathways.ToArray)
            Return ChunkBuffer
        End Function

        <ExportAPI("write.csv.kegg_phenotypes")>
        Public Function WriteKEGGPhenotypes(data As IEnumerable(Of KEGGPhenotypes), saveto As String) As Boolean
            Return data.SaveTo(saveto, False)
        End Function

        <ExportAPI("kegg_phenotype.denormalize")>
        Public Function DenormalizePhenotypeData(data As String, ptt As String, saveto As String) As Boolean
            Dim Result = KEGGPhenotypeDenormalizeData.Denormalize(data.LoadCsv(Of KEGGPhenotypes)(False), ptt)
            Return Result.SaveTo(saveto, False)
        End Function
    End Module
End Namespace
