#Region "Microsoft.VisualBasic::ebbdba4d33f857fdff437ac6007bb54d, models\Networks\Network.BLAST\Metagenome\Protocol.vb"

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

    '     Module Protocol
    ' 
    '         Function: __buildNetwork, __subNetwork, (+2 Overloads) BuildMatrix, (+2 Overloads) BuildNetwork, Dump_x2taxid
    '                   TaxonomyMaps
    ' 
    '         Sub: __styleNetwork
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.Metagenomics

Namespace Metagenome

    ''' <summary>
    ''' ###### Protocol Steps
    ''' 
    ''' 1. 先对16S/18S blastn结果注释taxonomy，得到taxid
    ''' 2. 对16S/18S进行blastn得到成员之间的相似性矩阵
    ''' 3. 输出网络模型: ``query -> fromNode, subject -> toNode, taxid -> type(color)``
    ''' </summary>
    Public Module Protocol

        ''' <summary>
        ''' 重新直接生成
        ''' </summary>
        ''' <param name="taxData"></param>
        ''' <param name="gi2taxid"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Dump_x2taxid(taxData As IEnumerable(Of BlastnMapping), gi2taxid As Boolean) As String()
            Dim list As New List(Of String)
            Dim parser = TaxidMaps.GetParser(gi2taxid)

            If gi2taxid Then

                For Each x As BlastnMapping In taxData
                    list += parser(x.Reference) & vbTab & x("taxid")
                Next

            Else

                list += Accession2Taxid.Acc2Taxid_Header

                For Each x In taxData
                    Dim acc$ = parser(x.Reference)
                    list += acc & vbTab & (acc & ".1") & vbTab & x("taxid") & vbTab & 0
                Next

            End If

            Return list.Distinct.ToArray
        End Function

        ''' <summary>
        ''' ###### step 1
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="x2taxid$">已经被subset的数据库</param>
        ''' <param name="taxonomy"></param>
        ''' <param name="is_gi2taxid">
        ''' ``Reference``是否使用的是旧的gi系统？这个参数决定了<paramref name="x2taxid"/>的工作模式
        ''' </param>
        ''' <param name="notFound">iterator函数不能够使用ByRef，所以假若需要得到notfound列表，就将这个参数实例化再传递进来</param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function TaxonomyMaps(source As IEnumerable(Of BlastnMapping),
                                              x2taxid$,
                                              taxonomy As NcbiTaxonomyTree,
                                              Optional is_gi2taxid? As Boolean = False,
                                              Optional notFound As List(Of String) = Nothing) As IEnumerable(Of BlastnMapping)

            Dim taxid As New Value(Of Integer)
            Dim mapping As TaxidMaps.Mapping = If(
                is_gi2taxid,
                TaxidMaps.MapByGI(x2taxid),
                TaxidMaps.MapByAcc(x2taxid))

            Dim taxidFromRef As Mapping = Reference2Taxid(mapping, is_gi2taxid)

            If notFound Is Nothing Then  ' iterator函数不能够使用ByRef，所以假若需要得到notfound列表，就将这个参数实例化再传递进来
                notFound = New List(Of String)
            End If

            For Each hit As BlastnMapping In source
                With hit
                    ' inline value assign of the 18s rRNA taxid
                    If (taxid = taxidFromRef(hit.Reference)) > -1 Then

                        .Extensions(Protocol.taxid) = (taxid.Value)

                        Dim nodes = taxonomy.GetAscendantsWithRanksAndNames(+taxid, True)
                        Dim tree$ = nodes.BuildBIOM
                        Dim name$ = taxonomy(taxid)?.name

                        .Extensions(Protocol.taxonomyName) = name
                        .Extensions(Protocol.Taxonomy) = tree
                    Else
                        Call .Reference.Warning
                        Call notFound.Add(.Reference)

                        .Extensions(Protocol.taxid) = Integer.MaxValue  ' 找不到具体的物种分类数据的
                        .Extensions(Protocol.taxonomyName) = "unknown"
                        .Extensions(Protocol.Taxonomy) = "unknown"
                    End If
                End With

                Yield hit
            Next
        End Function

        Const taxid$ = NameOf(taxid)
        Const taxonomyName$ = "Taxonomy.Name"
        Const Taxonomy$ = NameOf(Taxonomy)

        ''' <summary>
        ''' ###### step 2
        ''' 
        ''' 低于给定的阈值的hit都会被丢弃
        ''' </summary>
        ''' <param name="blastn">SSU的fasta文件自己对自己的比对结果</param>
        ''' <param name="identities#"></param>
        ''' <param name="coverage#"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function BuildMatrix(blastn As v228, Optional identities# = 0.3, Optional coverage# = 0.3) As IEnumerable(Of DataSet)
            For Each query As Query In blastn.Queries  ' 因为是blastn所以可能会存在一部分比对上的结果，所以会出现重复的片段，在这里取出identities最高的
                Dim besthits As SubjectHit() = query.GetBesthits(coverage:=coverage, identities:=identities)
                Dim distinct = From x As SubjectHit
                               In besthits
                               Select x
                               Group x By x.Name Into Group
                Dim similarity As Dictionary(Of String, Double) =
                    distinct.ToDictionary(Function(h) h.Name,
                                          Function(h) h.Group.OrderBy(
                                          Function(x) x.Score.Identities.Value) _
                                                  .Last.Score.Identities.Value)
                Yield New DataSet With {
                    .ID = query.QueryName,
                    .Properties = similarity
                }
            Next
        End Function

        ''' <summary>
        ''' ###### step 2
        ''' </summary>
        ''' <param name="blastn"></param>
        ''' <param name="identities#"></param>
        ''' <param name="coverage#"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function BuildMatrix(blastn As IEnumerable(Of BlastnMapping), Optional identities# = 0.3, Optional coverage# = 0.3) As IEnumerable(Of DataSet)
            Dim g = From x As BlastnMapping
                    In blastn
                    Where x.identitiesValue >= identities AndAlso
                        x.GetCoverage >= coverage
                    Select x
                    Group x By x.ReadQuery Into Group

            For Each query In g
                Dim distinct = From x As BlastnMapping
                               In query.Group
                               Select x
                               Group x By x.Reference Into Group
                Dim similarity As Dictionary(Of String, Double) =
                    distinct.ToDictionary(Function(h) h.Reference,
                                          Function(h) h.Group.OrderBy(
                                          Function(x) x.identitiesValue).Last.identitiesValue)
                Yield New DataSet With {
                    .ID = query.ReadQuery,
                    .Properties = similarity
                }
            Next
        End Function

        ''' <summary>
        ''' ###### step 3
        ''' 
        ''' 节点的颜色分类以及边的颜色分类是通过taxid分组来进行的
        ''' </summary>
        ''' <param name="matrix"><see cref="BuildMatrix"/></param>
        ''' <param name="taxid"><see cref="TaxonomyMaps"/></param>
        ''' <param name="theme">The network color theme, default using colorbrewer theme style: **Paired:c12**</param>
        ''' <returns>使用于``Cytoscape``进行绘图可视化的网络数据模型</returns>
        <Extension>
        Public Function BuildNetwork(matrix As IEnumerable(Of DataSet), taxid As IEnumerable(Of BlastnMapping), Optional theme$ = "Paired:c12", Optional parallel As Boolean = False) As FileStream.NetworkTables
            With New Dictionary(Of String, (taxid%, taxonomyName$, Taxonomy As String))
                For Each SSU As BlastnMapping In taxid
                    Dim tax = (CInt(SSU(Protocol.taxid)), SSU(Protocol.taxonomyName), SSU(Protocol.Taxonomy))
                    Call .Add(SSU.ReadQuery, tax)
                Next

                Return matrix.__buildNetwork(.ByRef, theme, parallel)
            End With
        End Function

        <Extension>
        Private Function __buildNetwork(matrix As IEnumerable(Of DataSet),
                                        taxonomyTypes As Dictionary(Of String, (taxid%, taxonomyName$, Taxonomy As String)),
                                        theme$,
                                        parallel As Boolean) As FileStream.NetworkTables

            Dim nodes As New List(Of Node)
            Dim edges As New List(Of NetworkEdge)

            ' 2016-11-29
            ' 使用负数来表示unknown的taxid会在后面分配颜色，按照-重新分割的时候出现key not found的问题，所以这里用integer的最大值来避免出现这个问题
            Dim unknown As (taxid%, taxonomyName$, Taxonomy As String) = (Integer.MaxValue, "unknown", "unknown")

            If parallel Then
                Dim LQuery = From ssu As DataSet
                             In matrix.AsParallel
                             Select ssu.__subNetwork(unknown, taxonomyTypes)

                For Each x In LQuery
                    nodes += x.node
                    edges += x.edges
                Next
            Else
                For Each SSU As DataSet In matrix ' 从矩阵之中构建出网络的数据模型
                    Dim pops = SSU.__subNetwork(unknown, taxonomyTypes)
                    nodes += pops.node
                    edges += pops.edges
                Next
            End If

            Call theme$.__styleNetwork(nodes, edges)

            Return New FileStream.NetworkTables With {
                .nodes = nodes,
                .edges = edges
            }
        End Function

        ''' <summary>
        ''' ###### step 3
        ''' 
        ''' 节点的颜色分类以及边的颜色分类是通过taxid分组来进行的
        ''' </summary>
        ''' <param name="matrix"><see cref="BuildMatrix"/></param>
        ''' <param name="taxid">Using the exists OTU taxonomy annotation data.</param>
        ''' <param name="theme">The network color theme, default using colorbrewer theme style: **Paired:c12**</param>
        ''' <returns>使用于``Cytoscape``进行绘图可视化的网络数据模型</returns>
        <Extension>
        Public Function BuildNetwork(matrix As IEnumerable(Of DataSet), taxid As IEnumerable(Of OTUData), Optional theme$ = "Paired:c12", Optional parallel As Boolean = False) As FileStream.NetworkTables
            With New Dictionary(Of String, (taxid%, taxonomyName$, Taxonomy As String))
                For Each SSU As OTUData In taxid
                    Dim tax = (CInt(SSU.data(Protocol.taxid)), SSU.data(Protocol.taxonomyName), SSU.taxonomy)
                    Call .Add(SSU.OTU, tax)
                Next

                Return matrix.__buildNetwork(.ByRef, theme, parallel)
            End With
        End Function

        <Extension>
        Private Function __subNetwork(ssu As DataSet,
                                  unknown As (taxid%, taxonomyName$, Taxonomy As String),
                            taxonomyTypes As Dictionary(Of String, (taxid%, taxonomyName$, Taxonomy As String))) _
                                          As (node As Node, edges As IEnumerable(Of NetworkEdge))

            Dim taxonomy As (taxid%, taxonomyName$, taxonomy As String)
            Dim edges As New List(Of NetworkEdge)

            If taxonomyTypes.ContainsKey(ssu.ID) Then
                taxonomy = taxonomyTypes(ssu.ID)
            Else
                taxonomy = unknown
            End If

            Dim node As New Node With {
                .ID = ssu.ID,
                .NodeType = taxonomy.taxid,
                .Properties = New Dictionary(Of String, String)
            }

            With node
                Call .Add(NameOf(taxonomy.taxonomyName), taxonomy.taxonomyName)
                Call .Add(NameOf(taxonomy.taxonomy), taxonomy.taxonomy)
            End With

            For Each hit In ssu.Properties
                Dim hitType% = If(taxonomyTypes.ContainsKey(hit.Key),
                    taxonomyTypes(hit.Key).taxid,
                    unknown.taxid)  ' 有些是没有能够注释出任何物种信息的
                Dim type%() = {
                    taxonomy.taxid,
                    hitType
                }
                Dim interacts$ = type _
                    .OrderBy(Function(t) t) _
                    .Distinct _
                    .JoinBy("-")

                edges += New NetworkEdge With {
                    .fromNode = ssu.ID,
                    .toNode = hit.Key,
                    .value = hit.Value,
                    .Properties = New Dictionary(Of String, String),
                    .interaction = interacts
                }
            Next

            Return (node, edges)
        End Function

        ''' <summary>
        ''' Apply color style for taxonomy group using Graphics2D <see cref="Designer"/> API
        ''' </summary>
        ''' <param name="theme$">Color theme name</param>
        ''' <param name="nodes"></param>
        ''' <param name="edges"></param>
        ''' 
        <Extension>
        Private Sub __styleNetwork(theme$, ByRef nodes As List(Of Node), ByRef edges As List(Of NetworkEdge))
            Dim taxids As String() = nodes _
                .Select(Function(x) x.NodeType) _
                .Distinct _
                .ToArray
            Dim colors As Dictionary(Of String, String) =
                Designer _
                .GetColors(theme, taxids.Length) _
                .Select(AddressOf ColorTranslator.ToHtml) _
                .SeqIterator _
                .ToDictionary(Function(c) taxids(c.i),
                              Function(c) c.value)  ' 手工设置颜色会非常麻烦，当物种的数量非常多的时候，则在这里可以进行手工生成
            Dim colorPaired As Color()
            Dim color As (r#, g#, b#)

            For Each edge As NetworkEdge In edges
                taxids = edge.interaction.Split("-"c)
                colorPaired = taxids _
                    .Select(Function(t) colors(t)) _
                    .Select(AddressOf ColorTranslator.FromHtml) _
                    .ToArray
                color = (
                    colorPaired.Average(Function(c) c.R),
                    colorPaired.Average(Function(c) c.G),
                    colorPaired.Average(Function(c) c.B))
                edge!color = ColorTranslator.ToHtml(
                    Drawing.Color.FromArgb(
                        CInt(color.r),
                        CInt(color.g),
                        CInt(color.b)))
            Next

            For Each node As Node In nodes
                With node
                    !display = $"({node.ID}) {node!taxonomyName}"
                    !color = colors(node.NodeType)
                End With
            Next
        End Sub
    End Module
End Namespace
