Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.DocumentStream
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus

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

            For Each x As BlastnMapping In source

                With x
                    If (taxid = taxidFromRef(x.Reference)) > -1 Then

                        .Extensions(Protocol.taxid) = +taxid

                        Dim nodes = taxonomy.GetAscendantsWithRanksAndNames(+taxid, True)
                        Dim tree$ = TaxonomyNode.BuildBIOM(nodes)
                        Dim name$ = taxonomy(taxid)?.name

                        .Extensions(Protocol.taxonomyName) = name
                        .Extensions(Protocol.Taxonomy) = tree
                    Else
                        Call .Reference.Warning
                        Call notFound.Add(.Reference)

                        .Extensions(Protocol.taxid) = -100  ' 找不到具体的物种分类数据的
                        .Extensions(Protocol.taxonomyName) = "unknown"
                        .Extensions(Protocol.Taxonomy) = "unknown"
                    End If
                End With

                Yield x
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
            For Each query As Query In blastn.Queries
                Dim besthits As SubjectHit() = query.GetBesthits(coverage:=coverage, identities:=identities)
                Dim similarity As Dictionary(Of String, Double) =
                    besthits.ToDictionary(Function(h) h.Name,
                                          Function(h) h.Score.Identities.Value)
                Yield New DataSet With {
                    .Identifier = query.QueryName,
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
        ''' <returns>使用于``Cytoscape``进行绘图可视化的网络数据模型</returns>
        <Extension>
        Public Function BuildNetwork(matrix As IEnumerable(Of DataSet), taxid As IEnumerable(Of BlastnMapping)) As FileStream.Network
            Dim nodes As New List(Of Node)
            Dim edges As New List(Of NetworkEdge)
            Dim taxonomyTypes As New Dictionary(Of String, (taxid%, taxonomyName$, Taxonomy As String))

            For Each SSU As BlastnMapping In taxid
                Dim tax = (CInt(SSU(Protocol.taxid)), SSU(Protocol.taxonomyName), SSU(Protocol.Taxonomy))
                taxonomyTypes.Add(SSU.ReadQuery, tax)
            Next

            For Each SSU As DataSet In matrix ' 从矩阵之中构建出网络的数据模型
                Dim taxonomy = taxonomyTypes(SSU.Identifier)

                nodes += New Node With {
                    .Identifier = SSU.Identifier,
                    .NodeType = taxonomy.taxid,
                    .Properties = New Dictionary(Of String, String)
                }

                With nodes.Last
                    Call .Properties.Add(NameOf(taxonomy.Taxonomy), taxonomy.Taxonomy)
                    Call .Properties.Add(NameOf(taxonomy.taxonomyName), taxonomy.taxonomyName)
                End With

                For Each hit In SSU.Properties
                    Dim type%() = {
                        taxonomy.taxid,
                        taxonomyTypes(hit.Key).taxid
                    }

                    edges += New NetworkEdge With {
                        .FromNode = SSU.Identifier,
                        .ToNode = hit.Key,
                        .Confidence = hit.Value,
                        .InteractionType = type _
                            .OrderBy(Function(t) t) _
                            .Distinct _
                            .JoinBy("-")
                    }
                Next
            Next

            Return New FileStream.Network With {
                .Nodes = nodes,
                .Edges = edges
            }
        End Function
    End Module
End Namespace