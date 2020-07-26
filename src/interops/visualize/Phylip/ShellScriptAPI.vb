#Region "Microsoft.VisualBasic::cfcda6ff973cd3e94c270c81bc8d8cbf, visualize\Phylip\ShellScriptAPI.vb"

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

    ' Module ShellScriptAPI
    ' 
    '     Function: __exportMatrix, __labelTrimming, __linkLabel, __sort, (+3 Overloads) CreateDocumentFile
    '               CreateGeneDist, CreateMotifDist, CreateNeighborMatrixFromVennMatrix, CreateNodeLabelAnnotation, ExportGendistMatrixFromBesthitMeta
    '               LoadGendist, LoadHitsVennData, LoadNewickTree, LoadXmlMeta, NeighborMatrixFromMeta
    '               NeighborMatrixFromVennMatrix, (+2 Overloads) SelfOverviewsGendist, SelfOverviewsMAT, SubMatrix, TreeLabelFastaReplace
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.Utility
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.Views
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models
Imports SMRUCC.genomics.Interops.Visualize.Phylip.MatrixFile
Imports PathEntry = System.Collections.Generic.KeyValuePair(Of String, String)

<Package("Phylip.Matrix",
                    Cites:="PLOTREE, D. and D. PLOTGRAM (1989). ""PHYLIP-phylogeny inference package (version 3.2).""",
                    Publisher:="amethyst.asuka@gcmodeller.org",
                    Category:=APICategories.ResearchTools)>
Public Module ShellScriptAPI

    ''' <summary>
    ''' {Trimmed_ID, uid}
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("Label.Annotation.Generate")>
    Public Function CreateNodeLabelAnnotation(<Parameter("Dir.SourceInput")> PttSource As String,
                                              <Parameter("Path.OutTree")> TreeFile As String,
                                              <Parameter("Genbank.BBH.Entry", "This entries list information data should be the distincted data " &
                                                  "using as the bbh data source for the phylip tree construction.")>
                                              EntryData As Generic.IEnumerable(Of SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports.gbEntryBrief)) _
        As <FunctionReturns("The node label in the output tree has been trimmed and replaced using the ptt entry annotation on the ncbi FTP website.")> String

        Dim OutTree As StringBuilder = New StringBuilder(FileIO.FileSystem.ReadAllText(TreeFile))
        Dim TreeNodeLabels = (From m As Match In Regex.Matches(OutTree.ToString, "[a-z]+_?\d+", RegexOptions.IgnoreCase) Select m.Value).ToArray
        Dim EntriesList = (From Dir As String
                           In FileIO.FileSystem.GetDirectories(PttSource, FileIO.SearchOption.SearchTopLevelOnly).AsParallel
                           Let GenbankList = Dir.LoadSourceEntryList({"*.ptt"})
                           Select FileIO.FileSystem.GetDirectoryInfo(Dir).Name,
                               Genbanks = (From File As PathEntry In GenbankList Let ID = File.Key Select ID).ToArray)
        Dim EntryLQuery = (From Entry In EntriesList
                           Let InternalLinkLabel = __linkLabel(Entry.Name, Entry.Genbanks)
                           Where Not InternalLinkLabel.IsNullOrEmpty
                           Select InternalLinkLabel).Unlist
        For Each NodeLabel As String In TreeNodeLabels
            Dim FindEntry = (From Entry In EntryLQuery.AsParallel Where InStr(Entry.Name, NodeLabel) = 1 Select Entry).ToArray
            '还必须要在Csv去重复的数据源之中存在
            FindEntry = (From Entry
                         In FindEntry
                         Where Not (From Line In EntryData.AsParallel
                                    Where String.Equals(Line.Locus, Entry.Item1, StringComparison.OrdinalIgnoreCase)
                                    Select Line).ToArray.IsNullOrEmpty
                         Select Entry).ToArray
            If Not FindEntry.IsNullOrEmpty Then
                Call OutTree.Replace(NodeLabel, FindEntry.First.Item2)
            End If
        Next

        Return OutTree.ToString
    End Function

    Private Function __linkLabel(entryName As String, genbanks As String()) As NamedTuple(Of String)()
        If genbanks.IsNullOrEmpty Then
            Return Nothing
        End If
        If genbanks.Length = 1 Then
            Return {New NamedTuple(Of String)(genbanks(Scan0), genbanks(Scan0), entryName & "(" & genbanks.First & ")")}
        Else
            Return (From ID As String
                    In genbanks
                    Select New NamedTuple(Of String)(ID.__labelTrimming, ID, entryName & "(" & ID & ")")).ToArray
        End If
    End Function

    <Extension> Private Function __labelTrimming(Id As String) As String
        If Len(Id) > 10 Then
            Return Mid(Id, 1, 10)
        Else
            Return Id
        End If
    End Function

    <ExportAPI("Load.Gendist")>
    Public Function LoadGendist(Path As String) As MatrixFile.Gendist
        Return MatrixFile.Gendist.LoadDocument(Path)
    End Function

    <ExportAPI("SubMatrix")>
    Public Function SubMatrix(MAT As MatrixFile.Gendist, Count As Integer, MainIndex As String) As MatrixFile.Gendist
        Return MAT.SubMatrix(Count, MainIndex)
    End Function

    <ExportAPI("MotifDist.Create")>
    Public Function CreateMotifDist(dat As IO.File) As Gendist
        Return MatrixFile.Gendist.CreateMotifDistrMAT(dat)
    End Function

    <ExportAPI("Write.Txt.PhylipMatrix")>
    Public Function CreateDocumentFile(mat As MatrixFile.MatrixFile, saveto As String) As Boolean
        Return mat.GenerateDocument.SaveTo(saveto)
    End Function

    <ExportAPI("Write.Txt.PhylipMatrix")>
    Public Function CreateDocumentFile(mat As Gendist, saveto As String) As Boolean
        Return mat.GenerateDocument.SaveTo(saveto)
    End Function

    <ExportAPI("Write.Txt.PhylipMatrix")>
    Public Function CreateDocumentFile(mat As MatrixFile.NeighborMatrix, saveto As String) As Boolean
        Return mat.GenerateDocument.SaveTo(saveto)
    End Function

    <ExportAPI("Load.NewickTree", Info:="Newick format tree is the phylip default output data format.")>
    Public Function LoadNewickTree(tree As String, Optional Name As String = "Evol Tree") As Evolview.PhyloTree
        Dim TreeObject = New Evolview.PhyloTree(Name, tree, "newick")
        Return TreeObject
    End Function

    <ExportAPI("Gendist.Create")>
    Public Function CreateGeneDist(path As String) As Gendist
        Dim Csv = IO.File.Load(path)
        Return MatrixFile.Gendist.CreateMotifDistrMAT(Csv)
    End Function

    <ExportAPI("Neighbor.Create")>
    Public Function CreateNeighborMatrixFromVennMatrix(path As String, Optional fastLoad As Boolean = True) As MatrixFile.NeighborMatrix
        Call $"Start to load venn matrix data from file: {path.ToFileURL}".__DEBUG_ECHO

        Dim Csv As IO.File = If(fastLoad, FileLoader.FastLoad(path), IO.File.Load(path))

        Call $"Venn matrix data load Job done!".__DEBUG_ECHO

        path = $"{path}.Neighbor.csv"
        Call $"Temp matrix was saved at {path.ToFileURL}".__DEBUG_ECHO
        Call Csv.Save(path, False)

        Return NeighborMatrixFromVennMatrix(Csv)
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="source">Xml数据的文件夹</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("Load.Xml.Besthit.MetaSource")>
    Public Function LoadHitsVennData(<Parameter("DIR.Source",
                                                "The directory which contains the compiled bbh besthit xml data file.")>
                                     source As String) As <FunctionReturns("")> SpeciesBesthit()
        Dim resHash As Dictionary(Of String, String) = source.LoadSourceEntryList({"*.xml"})
        Dim proc As EventProc = resHash.LinqProc
        Dim LQuery = (From i As SeqValue(Of PathEntry) In resHash.SeqIterator
                      Let path As PathEntry = i.value
                      Let d As Integer = proc.Tick
                      Select LoadXmlMeta(path)).ToArray

        Call Console.WriteLine("The besthit meta file data load done!")

        Return LQuery
    End Function

    Private Function LoadXmlMeta(Path As PathEntry) As SpeciesBesthit
        Call $"Start to load data from {Path.Value.ToFileURL}....".__DEBUG_ECHO
        Dim Sw = Stopwatch.StartNew
        Dim Meta = Path.Value.LoadXml(Of SpeciesBesthit)
        Call $"Data load done!  /// {Sw.ElapsedMilliseconds} ms.".__DEBUG_ECHO

        Return Meta
    End Function

    ''' <summary>
    ''' 矩阵文件的格式要求为：
    ''' 行的标题（每一行中的第一个元素）为基因组的名称
    ''' 每一列为某一个基因的频率或者其他数值
    ''' 例如：
    '''         基因1，基因2，基因3， ...
    ''' 基因组1   1     1      0
    ''' 基因组2   2     1      4
    ''' </summary>
    ''' <param name="besthit"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function __exportMatrix(besthit As SpeciesBesthit) As IO.File
        Dim MAT As IO.File = New IO.File
        Dim hits = besthit.InternalSort(False).ToArray

        hits = (From hit As HitCollection
                In hits
                Select prot = hit
                Order By prot.QueryName Ascending).ToArray  '对Query的蛋白质编号进行排序

        On Error Resume Next

        Dim head As New IO.RowObject("QueryProtein" + hits.First.hits.ToList(Function(x) x.tag))  '生成表头
        MAT += head

        For Each hit As HitCollection In hits
            Dim Row As New IO.RowObject(hit.QueryName + hit.hits.ToList(Function(x) CStr(x.identities)))
            MAT += Row
        Next

        Return MAT
    End Function

    ''' <summary>
    ''' 直接使用identities值作为最开始的等位基因的频率值
    ''' </summary>
    ''' <param name="MetaSource"></param>
    ''' <param name="Limits">0或者小于零的数值都为不限制,假设做出数量的限制的话，函数只会提取指定数目的基因组数据，都是和外标尺最接近的基因组</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("Gendist.Matrix.Generate")>
    <Extension>
    Public Function ExportGendistMatrixFromBesthitMeta(<Parameter("BBH.Meta.Source")> MetaSource As IEnumerable(Of SpeciesBesthit),
                                                       <Parameter("Index.Main")> Optional MainIndex As String = "",
                                                       <Parameter("Null.Trim")> Optional TrimNull As Boolean = False,
                                                       <Parameter("Limits", "Any value number for limits < 1 is no limits on the export genome number.")>
                                                       Optional Limits As Integer = -1) As <FunctionReturns("")> Gendist

        Call Console.WriteLine("Limits " & Limits)

        Dim DataDict = MetaSource.Shuffles.ToDictionary(Function(item) item.sp)
        Dim IndexKey = DataDict.Keys(VennDataModel.__parserIndex(DataDict, MainIndex))
        Dim MainData = DataDict(IndexKey)

        Call DataDict.Remove(IndexKey)

        If MainData.hits.IsNullOrEmpty Then
            Call $"The profile data of your key ""{MainIndex}"" ---> ""{MainData.sp}"" is null!".__DEBUG_ECHO
            Call $"Thread exists...".__DEBUG_ECHO
            Return Nothing
        Else
            Call $"Main index data has {MainData.hits.Length} hits...".__DEBUG_ECHO
        End If

        If Limits > 1 Then
            Dim ChunTemp = __sort(DataDict, MainData)
            ChunTemp = ChunTemp.Take(Limits).ToArray
            Call $"The output genome data was limited of counts {ChunTemp.Length}".__DEBUG_ECHO
            DataDict = ChunTemp.ToDictionary(Function(obj) obj.sp)
            MainData = MainData.Take(DataDict.Keys.ToArray)
        End If

        Dim MAT As IO.File = __exportMatrix(MainData)
        Dim species As String() = (From hitData As Hit In MainData.hits.First.hits Select hitData.tag).ToArray

        For deltaInd As Integer = 0 To DataDict.Count - 1
            Dim subMain As SpeciesBesthit = DataDict.Values(deltaInd)

            If subMain.hits.IsNullOrEmpty Then
                Call $"Profile data {subMain.sp} is null!".__DEBUG_ECHO
                Continue For
            Else
                Call Console.WriteLine(" {0} > " & subMain.sp, deltaInd)
            End If

            Dim di As Integer = deltaInd
            Dim subMainMatched = (From row In MAT Let d = 2 + di + 1 Let id As String = row(d) Where Not String.IsNullOrEmpty(id) Select id).ToArray
            Dim notmatched = (From hit As HitCollection
                              In subMain.hits
                              Where Array.IndexOf(subMainMatched, hit.QueryName) = -1
                              Select hit.QueryName,
                                  hit.description,
                                  speciesProfile = hit.hits.ToDictionary(Function(prot) prot.tag))

            For Each SubMainNotHitGene In notmatched  '竖直方向遍历第n列的基因号
                Dim row As New IO.RowObject From {SubMainNotHitGene.QueryName}

                Call row.AddRange((From nnn In (deltaInd).Sequence Select "0").ToArray)

                For Each sid As String In species.Skip(deltaInd)
                    Dim matched = SubMainNotHitGene.speciesProfile(sid)
                    Call row.Add(matched.identities)
                Next
                Call MAT.Add(row)
            Next
        Next

        Dim IDChunkBuffer = (From data In DataDict.Values Select data.sp).AsList + MainData.sp
        Call IDChunkBuffer.ToArray().SaveTo("./MAT_ID.txt") '会同时输出矩阵之中的基因组的NCBI编号以方便后面的分析
        Call MAT.Save("./VennMatrix.csv", False)
        Call Console.WriteLine("Export data job done! start to create matrix!")

        Dim StringCollection = (From row In MAT Select row.ToArray).ToArray
        StringCollection = StringCollection.MatrixTranspose
        MAT = New IO.File(From row In StringCollection Select CType(row, IO.RowObject))

        Return MatrixFile.Gendist.CreateMotifDistrMAT(MAT)
    End Function

    Private Function __sort(MAT As Dictionary(Of String, SpeciesBesthit), MainData As SpeciesBesthit) As SpeciesBesthit()
        Dim ids As String() = MainData.GetTopHits
        Dim buf As SpeciesBesthit() = (From IDtag As String
                                       In ids
                                       Where MAT.ContainsKey(IDtag)
                                       Select MAT(IDtag)).ToArray
        Return buf
    End Function

    <ExportAPI("Label.fasta_nno")>
    Public Function TreeLabelFastaReplace(Tree As String, genome As IEnumerable(Of gbEntryBrief)) As String
        Dim Replacement As New StringBuilder(Tree)

        For Each Gen As gbEntryBrief In genome
            Dim Def As String = Gen.Definition.Replace(",", " ").Replace(".", " ").Replace("  ", " ").Trim
            Call Replacement.Replace(Gen.AccessionID, Def)
        Next

        Return Replacement.ToString
    End Function

    ''' <summary>
    ''' 从已经生成的韦恩矩阵之中生成距离矩阵
    ''' </summary>
    ''' <returns></returns>
    '''
    <ExportAPI("Neighbor.From.VennMatrix")>
    Public Function NeighborMatrixFromVennMatrix(VennMatrix As IO.File) As MatrixFile.NeighborMatrix

        Call Console.WriteLine("Start to preparing data matrix...")

        Dim Data = (From col As String()
                    In VennMatrix.Columns.Skip(1).AsParallel
                    Let ID As String = col(Scan0),
                        genElements As Double() = (From p As String In col.Skip(1) Select Val(p)).ToArray
                    Select ID,
                        genElements).ToArray
        '默认使用欧几里得距离
        '为了防止数据混乱，这里不再使用并行拓展，以保持两两对应的顺序
        Dim Head As New IO.RowObject("" + (From sp In Data Select sp.ID).AsList)
        Dim MatBuilder As IO.File = New IO.File + Head

        Call Console.WriteLine("Start to generate matrix file")

        For Each sp In Data
            Dim row As New IO.RowObject
            Call row.Add(sp.ID)

            For Each paired In Data '对角线是自己对自己，距离总是为零
                Call row.Add(sp.genElements.EuclideanDistance(paired.genElements))
            Next
            Call MatBuilder.Add(row)
            Call Console.Write(".")
        Next

        Call Console.WriteLine("Job done!")

        Return NeighborMatrix.CreateObject(MatBuilder)
    End Function

    <ExportAPI("Neighbor.From.Meta")>
    Public Function NeighborMatrixFromMeta(DIR As String) As String
        Dim metas As SpeciesBesthit() = LQuerySchedule.LQuery(ls - l - wildcards("*.xml") <= DIR, AddressOf LoadXml(Of SpeciesBesthit), 200).ToArray

        Dim Genomes As Dictionary(Of String, List(Of Double)) =
            metas.ToDictionary(Function(obj) obj.sp,
                               Function(obj)
                                   Return New List(Of Double)
                               End Function)

        ' 不可以使用并行化，因为矩阵之中要求二者两两对应
        For Each File As SpeciesBesthit In metas
            For Each sp In Genomes
                sp.Value.Add(File.GetTotalIdentities(sp.Key))
            Next
        Next

        Dim MAT As New StringBuilder("   " & Genomes.Count)
        Dim idx As Integer

        For Each Line In Genomes
            Dim str As StringBuilder = New StringBuilder(MatrixFile.MatrixFile.MAT_ID(Line.Key))
            Dim j As Integer

            For Each s In Line.Value
                If j = idx Then
                    Call str.Append(" " & "0.0000")
                Else
                    Call str.Append(" " & MatrixFile.MatrixFile.RoundNumber(CStr(s), 6))
                End If

                j += 1
            Next

            idx += 1
            Call MAT.AppendLine(str.ToString)
        Next

        Return MAT.ToString
    End Function

    ''' <summary>
    ''' 不可以使用并行化拓展，因为有一一对应关系
    ''' </summary>
    ''' <param name="overview"></param>
    ''' <returns></returns>
    <ExportAPI("MAT.From.Self.Overviews")>
    Public Function SelfOverviewsMAT(overview As Overview) As IO.File
        Dim lstId As String() = overview.Queries.Select(Function(x) x.Id).ToArray
        Dim MAT As IO.File =
            New IO.File + "".Join(lstId)

        For Each query In overview.Queries
            Dim row As New List(Of Object)
            Dim hist = query.Hits.ToDictionary(Function(x) x.HitName)

            Call row.Add(query.Id)

            For Each id As String In lstId
                Call row.Add(If(hist.ContainsKey(id), hist(id).identities, 0))
            Next

            Call MAT.Add(New IO.RowObject(row))
        Next

        Return MAT
    End Function

    <ExportAPI("Gendist.From.Self.Overviews")>
    Public Function SelfOverviewsGendist(overview As Overview) As Gendist
        Dim MAT = SelfOverviewsMAT(overview)
        Dim Gendist = MatrixFile.Gendist.CreateMotifDistrMAT(MAT)
        Return Gendist
    End Function

    <ExportAPI("Gendist.From.Self.Overviews")>
    Public Function SelfOverviewsGendist(blastOut As v228) As Gendist
        Dim overview = blastOut.ExportOverview
        Dim MAT = SelfOverviewsMAT(overview)
        Dim Gendist = MatrixFile.Gendist.CreateMotifDistrMAT(MAT)
        Return Gendist
    End Function
End Module
