#Region "Microsoft.VisualBasic::3425cfa33c4f21b3816825d23a6a4441, visualize\Cytoscape\CLI_tool\CLI\Phenotype.vb"

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

    ' Module CLI
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: __clusterFastCommon, __clusteringCommon, __expends, __getMaxMods, __getMaxRelates
    '               __getMods, __merges, BuildTreeNET, BuildTreeNET_DEGs, BuildTreeNET_KEGGModules
    '               BuildTreeNET_KEGGPathways, BuildTreeNET_MergeRegulons, BuildTreeNETCOGs, BuildTreeNetTF, ClusterMatrix
    '               FastCluster, MotifCluster, MotifClusterSites, rFBATreeCluster, TreeCluster
    ' 
    '     Sub: __briefTrim
    '     Class FamilyHit
    ' 
    '         Properties: Family, HitName, QueryName
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.Utility
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.visualize
Imports Microsoft.VisualBasic.Data.visualize.KMeans
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.DataMining
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.FBA_DP.Models.rFBA
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.NCBI.COG
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.COG
Imports SMRUCC.genomics.Model.Network.Regulons.MotifCluster

Partial Module CLI

    Sub New()
        Call Settings.Session.Initialize()
    End Sub

    <ExportAPI("/Build.Tree.NET.COGs", Usage:="/Build.Tree.NET.COGs /cluster <cluster.csv> /COGs <myvacog.csv> [/out <outDIR>]")>
    Public Function BuildTreeNETCOGs(args As CommandLine) As Integer
        Dim func As [Function] = [Function].Default
        Dim inFile As String = args - "/cluster"
        Dim cog As String = args - "/cogs"
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & $"-{cog.BaseName}.bTree/")
        Dim clusters = inFile.LoadCsv(Of EntityClusterModel)
        Dim bTree As NetworkTables = clusters.bTreeNET
        Dim state = COGFunction.GetClass(cog.LoadCsv(Of MyvaCOG), func)
        Dim COGs = (From x As COGFunction
                    In state
                    Select (From g As String   ' 有些基因是有多个COG值的，这个情况还不清楚如何处理
                            In x.IDs
                            Select g,
                                cogCat = x)).IteratesALL.GroupBy(Function(x) x.g) _
                                            .ToDictionary(Function(x) x.Key,
                                                          Function(x) x.First.cogCat)

        Call __briefTrim(bTree)

        For Each node As Node In bTree.nodes
            If Not String.Equals(node.NodeType, "Entity", StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If

            If COGs.ContainsKey(node.ID) Then
                Dim gene As COGFunction = COGs(node.ID)
                Call node.Add("COG", gene.Catalog)
                Call node.Add("Func", gene.Description)
                Call node.Add("Category", gene.Category.Description)
            End If
        Next

        Return bTree.Save(out).CLICode
    End Function

    <ExportAPI("/Motif.Cluster", Usage:="/Motif.Cluster /query <meme.txt/MEME_OUT.DIR> /LDM <LDM-name/xml.path> [/clusters <3> /out <outCsv>]")>
    <Argument("/clusters", True,
                   Description:="If the expects clusters number is greater than the maps number, then the maps number divid 2 is used.")>
    Public Function MotifCluster(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim name As String = args("/LDM")
        Dim out As String = args.GetValue("/out", query.TrimSuffix & "." & BaseName(name) & ".Csv")
        Dim source As AnnotationModel()

        If query.FileExists Then
            source = AnnotationModel.LoadDocument(query)
        Else
            Dim files = FileIO.FileSystem.GetFiles(query, FileIO.SearchOption.SearchAllSubDirectories, "*.txt")
            source = files.Select(Function(x) AnnotationModel.LoadDocument(x)).ToVector
        End If

        If Not name.FileExists Then
            name = GCModeller.FileSystem.GetMotifLDM & name & ".Xml"
        End If

        Dim LDM As AnnotationModel = name.LoadXml(Of AnnotationModel)
        Dim param As New Parameters
        Dim Maps As ClusterEntity() = Mappings(source, LDM, param)
        Dim nClusters As Integer = args.GetValue("/clusters", 3)
        If nClusters >= Maps.Length Then
            nClusters = Maps.Length / 2
        End If

        Dim result = __clusteringCommon(nClusters, Maps, Nothing)
        Return result.SaveTo(out).CLICode
    End Function

    Private Function __clusteringCommon(nClusters As Integer, Maps As ClusterEntity(), mapNames As String()) As List(Of EntityClusterModel)
        Dim Clusters As ClusterCollection(Of ClusterEntity) = Maps.ClusterDataSet(nClusters)
        Dim result As New List(Of EntityClusterModel)
        Dim i As Integer = 1
        Dim setValue = New SetValue(Of EntityClusterModel) <= NameOf(EntityClusterModel.Cluster)

        For Each cluster As KMeansCluster(Of ClusterEntity) In Clusters
            Dim array As EntityClusterModel()

            If mapNames Is Nothing Then
                array = cluster.Select(Function(x) setValue(x.ToDataModel, CStr(i)))
            Else
                array = cluster.Select(Function(x) setValue(x.ToDataModel(mapNames), CStr(i)))
            End If

            i += 1
            Call result.Add(array)
        Next

        Return result
    End Function

    ''' <summary>
    ''' 居于全局比对来创建矩阵
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Motif.Cluster.MAT", Usage:="/Motif.Cluster.MAT /query <meme_OUT.DIR> [/LDM <ldm-DIR> /clusters 5 /out <outDIR>]")>
    Public Function ClusterMatrix(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim LDM As String = args("/LDM")
        Dim nClusters As Integer = args.GetValue("/clusters", 5)
        Dim out As String = args.GetValue("/out", query & ".ClusterMatrix/")

        If Not LDM.DirectoryExists Then
            LDM = GCModeller.FileSystem.GetMotifLDM
        End If

        Dim param As New Parameters
        Dim files = FileIO.FileSystem.GetFiles(query, FileIO.SearchOption.SearchAllSubDirectories, "*.txt")
        Dim source As AnnotationModel() = files.Select(Function(x) AnnotationModel.LoadDocument(x)).ToVector
        Dim result As Dictionary(Of String, EntityClusterModel) =
            source.Select(Function(x)
                              Return New EntityClusterModel With {
                                  .ID = x.Uid,
                                  .Properties = New Dictionary(Of String, Double)
                              }
                          End Function) _
                  .ToDictionary(Function(x) x.ID)

        If nClusters >= source.Length Then
            nClusters = source.Length / 2
        End If

        For Each xml As String In FileIO.FileSystem.GetFiles(LDM, FileIO.SearchOption.SearchTopLevelOnly, "*.xml")
            Dim Model As AnnotationModel = xml.LoadXml(Of AnnotationModel)
            Dim Maps As ClusterEntity() = Mappings(source, Model, param)

            If Maps.IsNullOrEmpty Then
                Call $"{xml.ToFileURL} unable creates mappings...".__DEBUG_ECHO
                Continue For
            End If

            Dim resultSet As List(Of EntityClusterModel) = __clusteringCommon(nClusters, Maps, Nothing)
            Dim sId As String = BaseName(xml)
            Dim outFile As String = out & "/" & sId & ".Csv"

            Call resultSet.SaveTo(outFile)

            For Each map As EntityClusterModel In resultSet
                result(map.ID).Properties(sId) = map.Cluster
            Next

            Dim all As [Set] = New [Set](result.Keys)
            Dim mapSet As [Set] = New [Set](resultSet.Select(Function(x) x.ID))
            Dim delta = all - mapSet

            For Each unMap As String In delta.ToArray(Of String)
                result(unMap).Properties(sId) = 0R
            Next

            Call xml.ToFileURL.__DEBUG_ECHO
        Next

        Dim mapNames = result.First.Value.Properties.Keys.ToArray
        Dim datas = result.Values.Select(Function(x) x.ToModel)
        Dim names = datas.Select(Function(x) x.uid)
        Dim Tree As ClusterEntity() = KMeans.TreeCluster(datas)
        Dim saveResult = Tree.Select(Function(x) x.ToDataModel(mapNames))

        For Each name In names
            For Each x In saveResult
                If InStr(x.ID, name) > 0 Then
                    x.Cluster = x.ID.Replace(name & ".", "")
                    x.ID = name
                    Exit For
                End If
            Next
        Next

        Return saveResult.SaveTo(out & "/ClusterMatrix.Csv")
    End Function

    <ExportAPI("/Motif.Cluster.Fast.Sites", Usage:="/Motif.Cluster.Fast.Sites /in <meme.txt.DIR> [/out <outDIR> /LDM <ldm-DIR>]")>
    Public Function MotifClusterSites(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR)
        Dim loads = (From file As String
                     In FileIO.FileSystem.GetFiles(inDIR, FileIO.SearchOption.SearchTopLevelOnly, "*.txt")
                     Select AnnotationModel.LoadDocument(file)).Unlist
        Dim resultSet As EntityClusterModel() = __clusterFastCommon(loads, args("/ldm"))
        Dim QueryHash As Dictionary(Of AnnotationModel) = loads.ToDictionary
        '将Entity和sites位点联系起来
        Dim asso = (From x In resultSet Select x, sites = QueryHash(x.ID)).ToArray
        Dim merges = (From gene In (From x In asso Select __expends(x.x, x.sites)).Unlist Select gene Group gene By gene.ID Into Group).ToArray
        Dim result As EntityClusterModel() = merges.Select(Function(x) __merges(x.Group.ToArray)).ToArray

        Call result.SaveTo(out & "/resultSet.Csv")

        ' 树形聚类
        Dim saveResult As EntityClusterModel() = KMeans.TreeCluster(result)
        out = out & "/TreeCluster.Csv"
        Return saveResult.SaveTo(out).CLICode
    End Function

    Private Function __merges(source As EntityClusterModel()) As EntityClusterModel
        Dim keys = source.First.Properties.Keys.ToArray
        Dim prop As New Dictionary(Of String, Double)

        For Each key As String In keys
            Dim value As Double() = source.Select(Function(x) x.Properties(key))
            Call prop.Add(key, value.Average)
        Next

        Return New EntityClusterModel With {
            .ID = source.First.ID,
            .Properties = prop
        }
    End Function

    Private Function __expends(source As EntityClusterModel, site As AnnotationModel) As EntityClusterModel()
        Dim LQuery = (From x As LDM.Site
                      In site.Sites
                      Select New EntityClusterModel With {
                          .ID = x.Name,
                          .Cluster = source.Cluster,
                          .Properties = New Dictionary(Of String, Double)(source.Properties)}).ToArray
        Return LQuery
    End Function

    Private Function __clusterFastCommon(source As IEnumerable(Of AnnotationModel), LDM As String) As EntityClusterModel()
        If Not LDM.DirectoryExists Then
            LDM = GCModeller.FileSystem.GetMotifLDM
        End If

        Dim loadLDMs = (From file As String
                        In FileIO.FileSystem.GetFiles(LDM, FileIO.SearchOption.SearchTopLevelOnly, "*.xml")
                        Select file.LoadXml(Of AnnotationModel)).ToArray
        Dim param As New Parameters
        Call param.__DEBUG_ECHO

        Dim dl As EventProc = source.LinqProc
        Dim LQuery = (From x As AnnotationModel       ' 逐行比对
                      In source'.AsParallel
                      Let pp = dl.Tick
                      Select x,
                          prop = (From hit As AnnotationModel
                                  In loadLDMs
                                  Select SWTom.Compare(x, hit, param)).ToArray).ToArray
        Dim resultSet = (From x In LQuery   ' 生成数据集
                         Select New EntityClusterModel With {
                             .ID = x.x.Uid,
                             .Properties = (From hit As Output In x.prop
                                            Select hit.Subject.Uid,
                                                score = hit.Similarity) _
                                                .ToDictionary(Function(xx) xx.Uid,
                                                              Function(xx) xx.score)}).ToArray
        Return resultSet
    End Function

    ''' <summary>
    ''' 基于局部比对的快速矩阵创建
    ''' </summary>
    ''' <param name="args">假若在最开始还没有赋值基因号，而是使用位置来代替的话，可以使用/map参数来讲基因从位置重新映射回基因编号</param>
    ''' <returns></returns>
    <ExportAPI("/Motif.Cluster.Fast"， Usage:="/Motif.Cluster.Fast /query <meme_OUT.DIR> [/LDM <ldm-DIR> /out <outDIR> /map <gb.gbk> /maxw -1 /ldm_loads]")>
    <Argument("/maxw", True,
                   Description:="If this parameter value is not set, then no motif in the query will be filterd, or all of the width greater then the width value will be removed.
                   If a filterd is necessary, value of 52 nt is recommended as the max width of the motif in the RegPrecise database is 52.")>
    Public Function FastCluster(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim LDM As String = args("/LDM")
        Dim out As String = args.GetValue("/out", query & ".TreeCluster.Csv")
        Dim source As AnnotationModel()

        If args.GetBoolean("/ldm_loads") Then
            Dim files = FileIO.FileSystem.GetFiles(query, FileIO.SearchOption.SearchAllSubDirectories, "*.xml")
            source = files.Select(Function(x) x.LoadXml(Of AnnotationModel))
        Else
            Dim files = FileIO.FileSystem.GetFiles(query, FileIO.SearchOption.SearchAllSubDirectories, "*.txt")
            source = files.Select(Function(x) AnnotationModel.LoadDocument(x)).ToVector
        End If

        If Not String.IsNullOrEmpty(args("/map")) Then
            Dim gb = GBFF.File.Load(args("/map"))
            Dim maps As Dictionary(Of String, String) = gb.LocusMaps
            Dim mapsName = (From x In maps.AsParallel
                            Select uid = x.Key.NormalizePathString & ".",
                                locus = x.Value & ".").ToArray

            For Each x In source
                Dim rp = (From s In mapsName.AsParallel
                          Where InStr(x.Uid, s.uid, CompareMethod.Text) = 1
                          Select s).FirstOrDefault
                If rp Is Nothing Then
                    Continue For
                End If
                x.Uid = x.Uid.Replace(rp.uid, rp.locus)
            Next
        End If

        Dim maxw As Integer = args.GetValue("/maxw", -1)
        If maxw > 0 Then
            Call $"/maxw is set to {maxw}".__DEBUG_ECHO
            source = (From x In source Where x.PWM.Length <= maxw Select x).ToArray
        End If

        Dim resultSet As EntityClusterModel() = __clusterFastCommon(source, LDM)
        Call resultSet.SaveTo(query & "/resultSet.Csv")

        ' 树形聚类
        Dim saveResult = KMeans.TreeCluster(resultSet)
        Return saveResult.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Tree.Cluster",
               Usage:="/Tree.Cluster /in <in.MAT.csv> [/out <out.cluster.csv> /Locus.Map <Name>]",
               Info:="This method is not recommended.")>
    Public Function TreeCluster(args As CommandLine) As Integer
        Dim inMAT As String = args("/in")
        Dim out As String = args.GetValue("/out", inMAT.TrimSuffix & ".Tree.Csv")
        Dim map As String = args("/Locus.Map")
        Dim maps As NameMapping = Nothing

        If Not String.IsNullOrEmpty(map) Then  ' 提升对其他的数据源的兼容性
            maps = {{map, NameOf(EntityClusterModel.ID)}}
        End If

        Dim inEntity = inMAT.LoadCsv(Of EntityClusterModel)(maps:=maps)
        Dim saveResult = KMeans.TreeCluster(inEntity)
        Return saveResult.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Tree.Cluster.rFBA",
               Usage:="/Tree.Cluster.rFBA /in <in.flux.pheno_OUT.Csv> [/out <out.cluster.csv>]")>
    Public Function rFBATreeCluster(args As CommandLine) As Integer
        Dim inMAT As String = args("/in")
        Dim out As String = args.GetValue("/out", inMAT.TrimSuffix & ".Cluster.Csv")
        Dim MAT = inMAT.LoadCsv(Of RPKMStat)(fast:=True)
        Dim inEntity = MAT.Select(
            Function(x) New EntityClusterModel With {
                .ID = x.Locus,
                .Properties = x.Properties
            }).ToArray
        Dim saveResult As EntityClusterModel() = KMeans.TreeCluster(inEntity)
        Return saveResult.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Build.Tree.NET.DEGs",
               Usage:="/Build.Tree.NET.DEGs /in <cluster.csv> /up <locus.txt> /down <locus.txt> [/out <outDIR> /brief]")>
    Public Function BuildTreeNET_DEGs(args As CommandLine) As Integer
        Dim inMAT As String = args("/in")
        Dim upFile As String = args("/up")
        Dim downFile As String = args("/down")
        Dim out As String = args.GetValue("/out", inMAT.TrimSuffix & ".TreeNET/")
        Dim MAT = inMAT.LoadCsv(Of EntityClusterModel)
        Dim net As NetworkTables = MAT.bTreeNET
        Dim brief As Boolean = args.GetBoolean("/brief")

        If brief Then Call __briefTrim(net)

        Dim DEGs As Dictionary(Of String, String) = upFile.ReadAllLines.ToDictionary(Function(x) x, Function(null) "+")
        DEGs.AddRange(downFile.ReadAllLines.ToDictionary(Function(x) x, Function(null) "-"))

        For Each node As Node In net.nodes
            If Not String.Equals(node.NodeType, "Entity", StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If

            If DEGs.ContainsKey(node.ID) Then
                Call node.Add("DEG", DEGs(node.ID))
            End If
        Next

        Return net.Save(out, Encoding.ASCII).CLICode
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="source">实体的属性是代谢反应对象</param>
    ''' <param name="cut"></param>
    ''' <param name="maps"></param>
    ''' <returns></returns>
    Private Function __getMaxRelates(source As IEnumerable(Of EntityClusterModel), cut As Double, maps As Dictionary(Of String, String)) As List(Of EntityClusterModel)
        Dim setValue = New SetValue(Of EntityClusterModel) <= NameOf(EntityClusterModel.Properties)
        Dim LQuery =
            LinqAPI.MakeList(Of EntityClusterModel) <= From x As EntityClusterModel
                                              In source
                                                       Let cuts As Dictionary(Of String, Double) = (
                                                           From p
                                                           In x.Properties
                                                           Where Math.Abs(p.Value) >= cut
                                                           Let mapId As String = maps(p.Key)
                                                           Select New KeyValuePair(Of String, Double)(mapId, p.Value)).ToDictionary(True)
                                                       Select setValue(x, cuts)
        Return LQuery
    End Function

    ''' <summary>
    ''' {调控因子, ModsId}
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="modDIR"></param>
    ''' <returns></returns>
    Private Function __getMaxMods(source As IEnumerable(Of EntityClusterModel),
                                  modDIR As String,
                                  ByRef modsDist As Dictionary(Of String, Dictionary(Of String, Integer))) As Dictionary(Of String, String())
        Dim mods = (From file As String
                    In FileIO.FileSystem.GetFiles(modDIR, FileIO.SearchOption.SearchAllSubDirectories, "*.xml").AsParallel
                    Let kgMod = file.LoadXml(Of bGetObject.[Module])
                    Where Not kgMod.reaction.IsNullOrEmpty
                    Select kgMod).ToArray
        Dim cats As Dictionary(Of String, BriteHEntry.Module) =
            BriteHEntry.Module.GetDictionary
        Dim result As New Dictionary(Of String, String())
        modsDist = New Dictionary(Of String, Dictionary(Of String, Integer))

        For Each x As EntityClusterModel In source
            Dim dist As Dictionary(Of String, Integer) =
                New Dictionary(Of String, Integer)
            Dim modsId As String() = __getMods(x.Properties.Keys.ToArray, mods, cats, dist)

            Call result.Add(x.ID, modsId)
            Call modsDist.Add(x.ID, dist)
        Next

        Return result
    End Function

    ''' <summary>
    ''' 经过数量降序排序了的
    ''' </summary>
    ''' <param name="keys"></param>
    ''' <param name="mods"></param>
    ''' <returns></returns>
    Private Function __getMods(keys As String(), mods As bGetObject.Module(), cats As Dictionary(Of String, BriteHEntry.Module), ByRef modSum As Dictionary(Of String, Integer)) As String()
        Dim LQuery = (From id As String In keys Select (From x In mods Where x.ContainsReaction(id) Select x)).IteratesALL
        Dim mIds = (From x In LQuery Select x.briteID Group By briteID Into Count).ToArray
        Dim catQuery = (From x In mIds Select [mod] = cats(x.briteID).SubCategory, x.Count Group By [mod] Into Group).ToArray
        Dim orders = (From x In catQuery
                      Where Not String.Equals(CCM, x.mod, StringComparison.Ordinal)
                      Select x.mod,
                          ss = x.Group.Select(Function(xx) xx.Count).Sum
                      Order By ss Descending).ToArray

        modSum = orders.ToDictionary(Function(x) x.mod, Function(x) x.ss)

        Dim out = orders.Select(Function(x) x.mod)
        Return out
    End Function

    Const CCM As String = "Central carbohydrate metabolism"

    ''' <summary>
    ''' 根据调控因子对代谢反应过程的相关性的聚类结果得到构建调控因子和Pathway的相关性
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Build.Tree.NET.TF",
               Usage:="/Build.Tree.NET.TF /in <cluster.csv> /maps <TF.Regprecise.maps.Csv> /map <keyvaluepair.xml> /mods <kegg_modules.DIR> [/out <outDIR> /brief /cuts 0.8]")>
    Public Function BuildTreeNetTF(args As CommandLine) As Integer
        Dim inMAT As String = args("/in")
        Dim maps As String = args("/maps")
        Dim out As String = args.GetValue("/out", inMAT.TrimSuffix & ".TreeNET/")
        Dim MAT = inMAT.LoadCsv(Of EntityClusterModel)
        Dim net As NetworkTables = MAT.bTreeNET
        Dim brief As Boolean = args.GetBoolean("/brief")
        Dim cut As Double = args.GetValue("/cuts", 0.8)
        Dim mods As String = args("/mods")
        Dim map As String = args("/map")
        Dim fluxMapHash = map.LoadXml(Of KeyValuePair()).ToDictionary(
            Function(x) x.Key,
            Function(x) x.Value)

        MAT = __getMaxRelates(MAT, cut, fluxMapHash)

        If brief Then Call __briefTrim(net)

        Dim modsDist As Dictionary(Of String, Dictionary(Of String, Integer)) = Nothing
        Dim modsHash = __getMaxMods(MAT, mods, modsDist)
        Dim mapsBBH = maps.LoadCsv(Of bbhMappings)
        Dim FamilyInfo = (From x As bbhMappings
                          In mapsBBH
                          Select x
                          Group x By x.hit_name Into Group) _
                               .ToDictionary(Function(x) x.hit_name,
                                             Function(x) x.Group.Select(Function(xx) xx.Family).ToArray)

        For Each edge As NetworkEdge In net.edges
            Dim depth As Integer = edge.fromNode.Split("."c).Length
            Call edge.Properties.Add(NameOf(depth), depth)

            If InStr(edge.interaction, "Leaf") = 0 Then
                Continue For
            End If

            Dim mName As String = edge.toNode.Split("."c).First

            Call edge.Properties.Add(NameOf(mName), mName)
        Next

        Dim maxLen As Integer = modsHash.Select(Function(x) x.Value.Length).Max

        For Each node In net.nodes
            If Not String.Equals(node.NodeType, "Entity", StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If

            Dim mName As String = node.ID.Split("."c).First

            Call node.Properties.Add(NameOf(mName), mName)

            If FamilyInfo.ContainsKey(mName) Then
                Dim modX = (From s As String
                            In FamilyInfo(mName)
                            Select s.Replace("*", "").ToUpper
                            Group By ToUpper Into Count).ToArray
                Dim sorts = (From x In modX Select x Order By x.Count Descending).ToArray

                For i As Integer = 0 To sorts.Length - 1
                    Call node.Properties.Add("Family" & (i + 1), sorts(i).ToUpper)
                Next
            End If

            If modsHash.ContainsKey(mName) Then
                Dim modNames = modsHash(mName)
                Dim last As String = modNames.LastOrDefault

                For i As Integer = 0 To maxLen - 1
                    If i < modNames.Length Then
                        Call node.Properties.Add("Phenotype" & (i + 1), modNames(i))
                    Else
                        Call node.Properties.Add("Phenotype" & (i + 1), last)
                    End If
                Next
            End If
        Next

        Dim MATHash = MAT.ToDictionary
        Dim modsdE As EntityClusterModel() = modsDist.Select(
            Function(Id, modsD) New EntityClusterModel With {
                .ID = Id,
                .Cluster = MATHash(Id).Cluster,
                .Properties = modsD.ToDictionary(Function(x) x.Key, Function(x) CDbl(x.Value))
            }).ToArray
        Call modsdE.SaveTo(out & "/TF-Mods.Csv")

        Return net.Save(out, Encoding.ASCII).CLICode
    End Function

    <ExportAPI("/Build.Tree.NET.KEGG_Pathways",
               Usage:="/Build.Tree.NET.KEGG_Pathways /in <cluster.csv> /mods <pathways.XML.DIR> [/out <outDIR> /brief /trim]")>
    Public Function BuildTreeNET_KEGGPathways(args As CommandLine) As Integer
        Dim inMAT As String = args("/in")
        Dim mods As String = args("/mods")
        Dim out As String = args.GetValue("/out", inMAT.TrimSuffix & ".TreeNET/")
        Dim MAT = inMAT.LoadCsv(Of EntityClusterModel)
        Dim net As NetworkTables = MAT.bTreeNET
        Dim brief As Boolean = args.GetBoolean("/brief")

        If brief Then Call __briefTrim(net)

        Dim modsLoad = (From file As String
                        In FileIO.FileSystem.GetFiles(mods, FileIO.SearchOption.SearchAllSubDirectories, "*.xml").AsParallel
                        Select file.LoadXml(Of bGetObject.Pathway)).ToDictionary

        For Each edge As NetworkEdge In net.edges
            Dim depth As Integer = edge.fromNode.Split("."c).Length
            Call edge.Properties.Add(NameOf(depth), depth)

            If InStr(edge.interaction, "Leaf") = 0 Then
                Continue For
            End If

            Dim mName As String = edge.toNode.Split("."c).First

            Call edge.Properties.Add(NameOf(mName), mName)
        Next

        Dim brits = BriteHEntry.Pathway.LoadDictionary
        Dim trim As Boolean = args.GetBoolean("/trim")

        For Each node As Node In net.nodes
            If Not String.Equals(node.NodeType, "Entity", StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If

            Dim mName As String

            If trim Then
                mName = Regex.Match(node.ID, "[a-z]{1,3}\d+").Value
            Else
                mName = node.ID.Split("."c).First
            End If

            Call node.Properties.Add(NameOf(mName), mName)

            If modsLoad.ContainsKey(mName) Then
                Dim modX = brits(modsLoad(mName).briteID)

                Call node.Properties.Add(NameOf(modX.category), modX.category)
                Call node.Properties.Add(NameOf(modX.class), modX.class)
                Call node.Properties.Add("Pathway", modX.entry.text)
            End If
        Next

        Return net.Save(out, Encoding.ASCII).CLICode
    End Function

    <ExportAPI("/Build.Tree.NET.KEGG_Modules",
               Usage:="/Build.Tree.NET.KEGG_Modules /in <cluster.csv> /mods <modules.XML.DIR> [/out <outDIR> /brief /trim]")>
    Public Function BuildTreeNET_KEGGModules(args As CommandLine) As Integer
        Dim inMAT As String = args("/in")
        Dim mods As String = args("/mods")
        Dim out As String = args.GetValue("/out", inMAT.TrimSuffix & ".TreeNET/")
        Dim MAT = inMAT.LoadCsv(Of EntityClusterModel)
        Dim net As NetworkTables = MAT.bTreeNET
        Dim brief As Boolean = args.GetBoolean("/brief")

        If brief Then Call __briefTrim(net)

        Dim modsLoad = (From file As String
                        In FileIO.FileSystem.GetFiles(mods, FileIO.SearchOption.SearchAllSubDirectories, "*.xml").AsParallel
                        Select file.LoadXml(Of bGetObject.Module)).ToDictionary

        For Each edge As NetworkEdge In net.edges
            Dim depth As Integer = edge.fromNode.Split("."c).Length
            Call edge.Properties.Add(NameOf(depth), depth)

            If InStr(edge.interaction, "Leaf") = 0 Then
                Continue For
            End If

            Dim mName As String = edge.toNode.Split("."c).First

            Call edge.Properties.Add(NameOf(mName), mName)
        Next

        Dim brits = BriteHEntry.Module.GetDictionary
        Dim trim As Boolean = args.GetBoolean("/trim")

        For Each node In net.nodes
            If Not String.Equals(node.NodeType, "Entity", StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If

            Dim mName As String

            If trim Then
                mName = Regex.Match(node.ID, "[a-z]+_M\d+", RegexOptions.IgnoreCase).Value
            Else
                mName = node.ID.Split("."c).First
            End If

            Call node.Properties.Add(NameOf(mName), mName)

            If modsLoad.ContainsKey(mName) Then
                Dim m = modsLoad(mName)
                Dim modX = brits(m.briteID)
                Dim title As String = $"[{modX.SubCategory} - {m.EntryId}]{m.name}"

                Call node.Properties.Add(NameOf(modX.Category), modX.Category)
                Call node.Properties.Add(NameOf(modX.Class), modX.Class)
                Call node.Properties.Add(NameOf(modX.SubCategory), modX.SubCategory)
                Call node.Properties.Add(NameOf(title), title)
            End If
        Next

        Return net.Save(out, Encoding.ASCII).CLICode
    End Function

    Private Sub __briefTrim(ByRef net As NetworkTables)
        For Each x In net.nodes
            x.Properties = Nothing
        Next
        For Each x In net.edges
            x.Properties = Nothing
        Next
    End Sub

    <ExportAPI("/Build.Tree.NET.Merged_Regulons", Usage:="/Build.Tree.NET.Merged_Regulons /in <cluster.csv> /family <family_Hits.Csv> [/out <outDIR> /brief]")>
    Public Function BuildTreeNET_MergeRegulons(args As CommandLine) As Integer
        Dim inMAT As String = args("/in")
        Dim out As String = args.GetValue("/out", inMAT.TrimSuffix & ".TreeNET/")
        Dim MAT = inMAT.LoadCsv(Of EntityClusterModel)
        Dim net As NetworkTables = MAT.bTreeNET
        Dim brief As Boolean = args.GetBoolean("/brief")

        If brief Then Call __briefTrim(net)

        Dim FamilyHits = (From x As FamilyHit In args("/family").LoadCsv(Of FamilyHit)
                          Select x
                          Group x By x.QueryName Into Group) _
                               .ToDictionary(Function(x) x.QueryName,
                                             Function(x) x.Group.Select(Function(xx) xx.Family).Distinct.ToArray)

        For Each edge As NetworkEdge In net.edges
            Dim depth As Integer = edge.fromNode.Split("."c).Length
            Call edge.Properties.Add(NameOf(depth), depth)

            If InStr(edge.interaction, "Leaf") = 0 Then
                Continue For
            End If

            Dim mName As String = Regex.Replace(edge.toNode, "\.\d+", "")

            Call edge.Properties.Add(NameOf(mName), mName)
        Next

        For Each node In net.nodes
            If Not String.Equals(node.NodeType, "Entity", StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If

            Dim mName As String = Regex.Replace(node.ID, "\.\d+", "")
            Dim pair = mName.Split("."c)
            Dim locus As String = pair(Scan0)

            Call node.Properties.Add(NameOf(mName), mName)
            Call node.Properties.Add(NameOf(locus), locus)
            Call node.Properties.Add("Regulon", pair.Last)

            If FamilyHits.ContainsKey(locus) Then
                Call node.Properties.Add("Family", FamilyHits(locus).JoinBy("+"))
            End If
        Next

        Return net.Save(out, Encoding.ASCII).CLICode
    End Function

    Public Class FamilyHit
        Public Property QueryName As String
        Public Property Family As String
        Public Property HitName As String
    End Class

    <ExportAPI("/Build.Tree.NET", Usage:="/Build.Tree.NET /in <cluster.csv> [/out <outDIR> /brief /FamilyInfo <regulons.DIR>]")>
    Public Function BuildTreeNET(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".Tree.NET/")
        Dim inData = inFile.LoadCsv(Of EntityClusterModel)
        Dim net As NetworkTables = inData.bTreeNET
        Dim brief As Boolean = args.GetBoolean("/brief")

        If brief Then Call __briefTrim(net)

        Dim FamilyHash As Dictionary(Of String, Regulator)

        If (args <= "/familyinfo").DirectoryExists Then
            Dim regulons = (From file As String
                            In FileIO.FileSystem.GetFiles(args("/familyinfo"), FileIO.SearchOption.SearchTopLevelOnly, "*.xml").AsParallel
                            Let regs = file.LoadXml(Of BacteriaRegulome).regulome
                            Where Not regs Is Nothing OrElse regs.regulators.IsNullOrEmpty
                            Select regs.regulators).ToArray.ToVector
            FamilyHash = (From x As Regulator In regulons
                          Let uid As String = x.LocusId & "." & x.locus_tag.text.Replace(":", "_")
                          Select x,
                              uid
                          Group By uid Into Group) _
                                .ToDictionary(Function(x) x.uid,
                                              Function(x) x.Group.First.x)
        Else
            FamilyHash = New Dictionary(Of String, Regulator)
        End If

        For Each edge As NetworkEdge In net.edges
            Dim depth As Integer = edge.fromNode.Split("."c).Length
            Call edge.Properties.Add(NameOf(depth), depth)

            If InStr(edge.interaction, "Leaf") = 0 Then
                Continue For
            End If

            Dim bbh As String = Regex.Replace(edge.toNode, "\.\d+", "")
            Dim hit As String() = bbh.Split("."c)

            If hit.Length = 1 Then
                bbh = Regex.Replace(bbh, "__\d+", "")
                Call edge.Properties.Add("TF", bbh)
            Else
                Call edge.Properties.Add(NameOf(bbh), bbh)
                Call edge.Properties.Add("query", hit(Scan0))
                Call edge.Properties.Add("hit", hit(1))

                If FamilyHash.ContainsKey(bbh) Then
                    Dim Family = FamilyHash(bbh)
                    Call edge.Properties.Add("Family", Family.family)
                    Call edge.Properties.Add("Effector", Family.effector)
                    Call edge.Properties.Add("BiologicalProcess", Family.biological_process.JoinBy("; "))
                    Call edge.Properties.Add("Pathway", Family.pathway)
                End If
            End If
        Next

        For Each node In net.nodes
            If Not String.Equals(node.NodeType, "Entity", StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If

            Dim bbh As String = Regex.Replace(node.ID, "\.\d+", "")
            Dim hit As String() = bbh.Split("."c)

            If hit.Length = 1 Then
                bbh = Regex.Replace(bbh, "__\d+", "")
                Call node.Properties.Add("TF", bbh)
            Else
                Call node.Properties.Add(NameOf(bbh), bbh)
                Call node.Properties.Add("query", hit(Scan0))
                Call node.Properties.Add("hit", hit(1))

                If FamilyHash.ContainsKey(bbh) Then
                    Dim Family = FamilyHash(bbh)
                    Call node.Properties.Add("Family", Family.family)
                    Call node.Properties.Add("Effector", Family.effector)
                    Call node.Properties.Add("BiologicalProcess", Family.biological_process.JoinBy("; "))
                    Call node.Properties.Add("Pathway", Family.pathway)
                    Call node.Properties.Add("Phenotype", $"[{hit(Scan0)}]{Family.biological_process}")
                End If
            End If
        Next

        Return net.Save(out, Encoding.ASCII).CLICode
    End Function
End Module
