
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy

Namespace Metagenomics

    ''' <summary>
    ''' LCA（最低公共祖先）算法
    ''' </summary>
    Public Class LCA

        ReadOnly _taxonomyTree As NcbiTaxonomyTree

        Public Sub New(taxonomyTree As NcbiTaxonomyTree)
            _taxonomyTree = taxonomyTree
        End Sub

        ''' <summary>
        ''' 使用路径比较法计算两个taxid的最近公共祖先（LCA）
        ''' </summary>
        ''' <param name="taxid1">第一个taxonomy ID</param>
        ''' <param name="taxid2">第二个taxonomy ID</param>
        ''' <returns>最近公共祖先的TaxonomyNode，如果找不到返回Nothing</returns>
        Public Function GetLCA(taxid1 As Integer, taxid2 As Integer, Optional ByRef distance As Integer = -1) As TaxonomyNode
            If taxid1 = taxid2 Then
                distance = 0
                Return _taxonomyTree.GetAscendantsWithRanksAndNames(taxid1).FirstOrDefault()
            End If

            ' 获取两个taxid到根节点的完整路径
            Dim path1 = _taxonomyTree.GetAscendantsWithRanksAndNames(taxid1)
            Dim path2 = _taxonomyTree.GetAscendantsWithRanksAndNames(taxid2)

            If path1.Length = 0 OrElse path2.Length = 0 Then
                Return Nothing
            End If

            ' 反转路径，使其从根节点开始
            Dim reversedPath1 = path1.Reverse().ToArray()
            Dim reversedPath2 = path2.Reverse().ToArray()

            ' 找到最后一个共同的祖先
            Dim lcaNode As TaxonomyNode = Nothing
            Dim minLength = Math.Min(reversedPath1.Length, reversedPath2.Length)

            For i As Integer = 0 To minLength - 1
                distance = i

                If reversedPath1(i).taxid = reversedPath2(i).taxid Then
                    lcaNode = reversedPath1(i)
                Else
                    Exit For
                End If
            Next

            distance = Math.Max(reversedPath1.Length - distance, reversedPath2.Length - distance)

            Return lcaNode
        End Function

        ''' <summary>
        ''' 使用倍增法计算LCA（适用于深度较大的树，效率更高）
        ''' </summary>
        ''' <param name="taxid1">第一个taxonomy ID</param>
        ''' <param name="taxid2">第二个taxonomy ID</param>
        ''' <returns>最近公共祖先的TaxonomyNode</returns>
        Public Function GetLCAByBinaryLifting(taxid1 As Integer, taxid2 As Integer) As TaxonomyNode
            If taxid1 = taxid2 Then
                Return _taxonomyTree.GetAscendantsWithRanksAndNames(taxid1).FirstOrDefault()
            End If

            ' 获取深度信息
            Dim depth1 = GetDepth(taxid1)
            Dim depth2 = GetDepth(taxid2)

            Dim node1 = _taxonomyTree(taxid1)
            Dim node2 = _taxonomyTree(taxid2)

            ' 确保node1是较深的节点
            If depth1 < depth2 Then
                Dim temp = node1
                node1 = node2
                node2 = temp
                Dim tempDepth = depth1
                depth1 = depth2
                depth2 = tempDepth
            End If

            ' 将较深的节点提升到相同深度
            Dim depthDiff = depth1 - depth2
            Dim currentNode = node1

            For i As Integer = 0 To depthDiff - 1
                If currentNode IsNot Nothing AndAlso currentNode.parent IsNot Nothing Then
                    currentNode = _taxonomyTree(Integer.Parse(currentNode.parent))
                Else
                    Exit For
                End If
            Next

            ' 现在两个节点在同一深度，同时向上寻找公共祖先
            While currentNode IsNot Nothing AndAlso node2 IsNot Nothing AndAlso currentNode.taxid <> node2.taxid
                If currentNode.parent IsNot Nothing AndAlso node2.parent IsNot Nothing Then
                    currentNode = _taxonomyTree(Integer.Parse(currentNode.parent))
                    node2 = _taxonomyTree(Integer.Parse(node2.parent))
                Else
                    Exit While
                End If
            End While

            If currentNode IsNot Nothing AndAlso node2 IsNot Nothing AndAlso currentNode.taxid = node2.taxid Then
                Return New TaxonomyNode With {
                    .taxid = currentNode.taxid,
                    .name = currentNode.name,
                    .rank = currentNode.rank,
                    .parent = currentNode.parent
                }
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' 计算taxid在树中的深度（距离根节点的步数）
        ''' </summary>
        ''' <param name="taxid">taxonomy ID</param>
        ''' <returns>深度值</returns>
        Private Function GetDepth(taxid As Integer) As Integer
            Dim depth = 0
            Dim currentNode = _taxonomyTree(taxid)

            While currentNode IsNot Nothing AndAlso currentNode.parent IsNot Nothing
                depth += 1
                currentNode = _taxonomyTree(Integer.Parse(currentNode.parent))
            End While

            Return depth
        End Function

        ''' <summary>
        ''' 计算多个taxid的最近公共祖先
        ''' </summary>
        ''' <param name="taxids">taxonomy ID集合</param>
        ''' <returns>最近公共祖先的TaxonomyNode</returns>
        Public Function GetLCA(taxids As IEnumerable(Of Integer),
                               Optional minSupport As Double = 0.35,
                               Optional maxDistance As Integer = 3) As TaxonomyNode

            If taxids Is Nothing OrElse Not taxids.Any() Then
                Return Nothing
            End If

            ' 将第一个taxid的路径作为起始的LCA
            Dim taxidList As Integer() = taxids.ToArray
            Dim allLineages As Lineage() = taxidList _
                .Select(Function(taxid) _taxonomyTree.GetAscendantsWithRanksAndNames(taxid)) _
                .Where(Function(line) Not line.IsNullOrEmpty) _
                .Select(Function(line) New Lineage(line.Reverse)) _
                .ToArray

            If allLineages.IsNullOrEmpty Then
                Return Nothing
            End If

            Dim longPath As Integer = Aggregate line As Lineage In allLineages Into Max(line.length)
            Dim cutoff As Integer = taxidList.Length * minSupport
            Dim depth As Integer = -1

            For i As Integer = 0 To longPath - 1
                Dim offset As Integer = i
                Dim levelNode As IGrouping(Of Integer, Lineage) = allLineages _
                    .Where(Function(line) line.length > offset) _
                    .GroupBy(Function(line) line.SetOffset(offset).taxid) _
                    .OrderByDescending(Function(a) a.Count) _
                    .FirstOrDefault

                If levelNode Is Nothing OrElse levelNode.Count < cutoff Then
                    Exit For
                Else
                    depth = offset
                    allLineages = levelNode.ToArray
                End If
            Next

            If allLineages.IsNullOrEmpty Then
                Return Nothing
            End If

            Dim averageDistance As Integer = allLineages _
                .Select(Function(line) line.length - depth) _
                .Average

            If averageDistance > maxDistance Then
                Return Nothing
            End If

            Dim LCA As TaxonomyNode = allLineages(0).SetOffset(depth)
            Return LCA
        End Function

        ''' <summary>
        ''' 在宏基因组分类场景中常用的LCA方法：找到覆盖所有输入taxid的最小公共分类单元
        ''' </summary>
        ''' <param name="taxids">k-mer匹配到的所有taxonomy ID集合</param>
        ''' <param name="minSupport">最小支持度阈值（0-1之间）</param>
        ''' <returns>LCA结果及其支持度信息</returns>
        Public Function GetLCAForMetagenomics(taxids As IEnumerable(Of Integer),
                                              Optional minSupport As Double = 0.35,
                                              Optional maxDistance As Integer = 9) As LcaResult

            If taxids Is Nothing OrElse Not taxids.Any() Then
                Return New LcaResult With {
                    .lcaNode = Nothing,
                    .supportRatio = 0,
                    .supportedTaxids = New Integer() {}
                }
            End If

            Dim taxidList As Integer() = taxids.Distinct().ToArray()

            If taxidList.Length = 1 Then
                Return New LcaResult With {
                    .lcaNode = _taxonomyTree.GetAscendantsWithRanksAndNames(taxidList(0)).FirstOrDefault(),
                    .supportRatio = 1.0,
                    .supportedTaxids = taxidList
                }
            End If

            ' 计算所有taxid的LCA
            Dim lcaNode As TaxonomyNode = GetLCA(taxidList, minSupport, maxDistance:=maxDistance)

            If lcaNode Is Nothing Then
                Return New LcaResult With {
                    .lcaNode = Nothing,
                    .supportRatio = 0,
                    .supportedTaxids = New Integer() {}
                }
            End If

            ' 计算支持度：有多少taxid是LCA的后代
            Dim supportedCount = taxidList.AsEnumerable.Count(Function(taxid) Check(taxid, lcaNode))
            Dim supportRatio = supportedCount / taxidList.Length

            ' 如果支持度低于阈值，尝试在更高层级寻找LCA
            If supportRatio < minSupport Then
                Dim currentLca = lcaNode
                While currentLca IsNot Nothing AndAlso currentLca.parent IsNot Nothing
                    Dim parentNode = _taxonomyTree(Integer.Parse(currentLca.parent))
                    Dim parentSupportedCount = taxidList.AsEnumerable.Count(Function(taxid) Check(taxid, parentNode))
                    Dim parentSupportRatio = parentSupportedCount / taxidList.Length

                    If parentSupportRatio >= minSupport Then
                        lcaNode = parentNode
                        supportRatio = parentSupportRatio
                        Exit While
                    End If

                    currentLca = parentNode
                End While
            End If

            Return New LcaResult With {
                .lcaNode = lcaNode,
                .supportRatio = supportRatio,
                .supportedTaxids = taxidList.Where(Function(taxid) Check(taxid, lcaNode)).ToArray
            }
        End Function

        Private Function Check(taxid As Integer, node As TaxonomyNode) As Boolean
            Dim path = _taxonomyTree.GetAscendantsWithRanksAndNames(taxid)
            Return path.Any(Function(a) a.taxid = node.taxid)
        End Function

        Public Function GetNode(taxid As Integer) As TaxonomyNode
            Return _taxonomyTree(taxid)
        End Function
    End Class

    Public Class Lineage

        Dim lineage As TaxonomyNode()
        Dim offset As Integer = 0

        Public ReadOnly Property length As Integer
            Get
                Return lineage.Length
            End Get
        End Property

        Public ReadOnly Property taxid As Integer
            Get
                Return lineage(offset).taxid
            End Get
        End Property

        Sub New(lineage As IEnumerable(Of TaxonomyNode))
            Me.lineage = lineage.ToArray
        End Sub

        Public Function SetOffset(offset As Integer) As Lineage
            Me.offset = offset
            Return Me
        End Function

        Public Overrides Function ToString() As String
            Return lineage(offset).ToString
        End Function

        Public Shared Narrowing Operator CType(lineage As Lineage) As TaxonomyNode
            Return lineage.lineage(lineage.offset)
        End Operator

    End Class

    ''' <summary>
    ''' LCA计算结果
    ''' </summary>
    Public Class LcaResult

        Public Property lcaNode As TaxonomyNode
        Public Property supportRatio As Double
        Public Property supportedTaxids As Integer()

        Public ReadOnly Property LCATaxid As Integer
            Get
                If lcaNode Is Nothing Then
                    Return 0
                Else
                    Return lcaNode.taxid
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            If lcaNode Is Nothing Then
                Return "LCA not found"
            Else
                Return $"LCA: {lcaNode.name} (taxid: {lcaNode.taxid}, rank: {lcaNode.rank}), Support: {supportRatio:P2}"
            End If
        End Function
    End Class
End Namespace