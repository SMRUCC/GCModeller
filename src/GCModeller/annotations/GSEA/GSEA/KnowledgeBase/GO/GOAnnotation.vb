Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.DAG

Namespace GO

    ''' <summary>
    ''' 基因 => GO词条 的注释数据模型
    ''' </summary>
    ''' <remarks>
    ''' GO的注释数据与GO的本体数据是相互独立的两个数据集：
    ''' 
    ''' + 本体数据(``go.obo``)描述的是词条之间的继承关系，即DAG图；
    ''' + 注释数据描述的是某一个基因被标注上了哪些GO词条。
    ''' 
    ''' 由于GO是一个有向无环图，只标注到某个较深层的词条上面的基因，
    ''' 按照true path rule也应该被计入其所有的祖先词条之中，这个
    ''' 传播操作由<see cref="Expand(Graph, OntologyRelations())"/>来完成。
    ''' </remarks>
    Public Class GOAnnotation

        ''' <summary>
        ''' 原始的注释数据：``[geneID => GO terms]``，这里的GO词条是**没有**
        ''' 经过祖先传播的
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property raw As Dictionary(Of String, String())

        ''' <summary>
        ''' 背景全集(universe)之中的基因数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property universeSize As Integer
            Get
                Return raw.Count
            End Get
        End Property

        Sub New(Optional map As Dictionary(Of String, String()) = Nothing)
            If map Is Nothing Then
                raw = New Dictionary(Of String, String())
            Else
                raw = map
            End If
        End Sub

        ''' <summary>
        ''' 从``[geneID => GO terms]``字典之中创建注释数据
        ''' </summary>
        ''' <param name="map"></param>
        ''' <returns></returns>
        Public Shared Function FromDictionary(map As Dictionary(Of String, String())) As GOAnnotation
            Return New GOAnnotation(map)
        End Function

        ''' <summary>
        ''' 从UniProt数据库之中提取出GO注释信息
        ''' </summary>
        ''' <param name="entries"></param>
        ''' <param name="getGeneID">
        ''' 从<see cref="entry"/>对象之中获取得到基因编号的函数，默认使用
        ''' 该蛋白的第一个accession编号
        ''' </param>
        ''' <returns></returns>
        Public Shared Function FromUniProt(entries As IEnumerable(Of entry),
                                           Optional getGeneID As Func(Of entry, String) = Nothing) As GOAnnotation

            Dim map As New Dictionary(Of String, String())

            If getGeneID Is Nothing Then
                getGeneID = Function(protein)
                                Return protein.accessions.FirstOrDefault
                            End Function
            End If

            For Each protein As entry In entries.SafeQuery
                Dim id As String = getGeneID(protein)

                If String.IsNullOrEmpty(id) Then
                    Continue For
                End If

                Dim terms As String() = {}

                If Not protein.xrefs Is Nothing AndAlso protein.xrefs.ContainsKey("GO") Then
                    terms = protein.xrefs("GO") _
                        .SafeQuery _
                        .Select(Function(ref) ref.id) _
                        .Where(Function(term) Not String.IsNullOrEmpty(term)) _
                        .Distinct _
                        .ToArray
                End If

                If map.ContainsKey(id) Then
                    map(id) = map(id) _
                        .JoinIterates(terms) _
                        .Distinct _
                        .ToArray
                Else
                    map.Add(id, terms)
                End If
            Next

            Return New GOAnnotation(map)
        End Function

        ''' <summary>
        ''' 从GAF格式的注释文件之中提取出GO注释信息
        ''' </summary>
        ''' <param name="gaf"></param>
        ''' <param name="excludesNOT">
        ''' 是否排除掉qualifier之中标记了``NOT``的负向注释，默认为True
        ''' </param>
        ''' <returns></returns>
        Public Shared Function FromGAF(gaf As IEnumerable(Of GAF), Optional excludesNOT As Boolean = True) As GOAnnotation
            Dim map As New Dictionary(Of String, List(Of String))

            For Each row As GAF In gaf.SafeQuery
                If row Is Nothing Then
                    Continue For
                End If
                If String.IsNullOrEmpty(row.DBObjectID) OrElse String.IsNullOrEmpty(row.GOID) Then
                    Continue For
                End If
                If excludesNOT AndAlso Not String.IsNullOrEmpty(row.Qualifier) Then
                    If row.Qualifier.IndexOf("NOT", StringComparison.OrdinalIgnoreCase) > -1 Then
                        Continue For
                    End If
                End If

                If Not map.ContainsKey(row.DBObjectID) Then
                    Call map.Add(row.DBObjectID, New List(Of String))
                End If
                If Not map(row.DBObjectID).Contains(row.GOID) Then
                    Call map(row.DBObjectID).Add(row.GOID)
                End If
            Next

            Return New GOAnnotation(map.ToDictionary(
                Function(gene) gene.Key,
                Function(gene) gene.Value.ToArray))
        End Function

        ''' <summary>
        ''' 获取得到某一个基因所直接注释的GO词条(没有经过祖先传播)
        ''' </summary>
        ''' <param name="geneID"></param>
        ''' <returns></returns>
        Public Function GetTerms(geneID As String) As String()
            If String.IsNullOrEmpty(geneID) OrElse Not raw.ContainsKey(geneID) Then
                Return {}
            Else
                Return raw(geneID)
            End If
        End Function

        ''' <summary>
        ''' 获取得到背景全集之中的所有基因编号
        ''' </summary>
        ''' <returns></returns>
        Public Function Genes() As String()
            Return raw.Keys.ToArray
        End Function

        ''' <summary>
        ''' 判断目标基因是否存在于当前的背景全集之中
        ''' </summary>
        ''' <param name="geneID"></param>
        ''' <returns></returns>
        Public Function HasGene(geneID As String) As Boolean
            Return Not String.IsNullOrEmpty(geneID) AndAlso raw.ContainsKey(geneID)
        End Function

        ''' <summary>
        ''' 按照GO的true path rule，将每一个基因的注释沿着DAG图向上
        ''' 传播到其所有的祖先词条上面
        ''' </summary>
        ''' <param name="dag">GO本体DAG图</param>
        ''' <param name="relations">
        ''' 除了``is_a``之外还需要参与传播的relationship关系类型，
        ''' 默认为<see cref="Graph.DefaultRelations"/>
        ''' </param>
        ''' <returns>
        ''' ``[geneID => 直接注释的GO词条 ∪ 所有祖先GO词条]``
        ''' </returns>
        Public Function Expand(dag As Graph, Optional relations As OntologyRelations() = Nothing) As Dictionary(Of String, String())
            Dim ancestorTable As Dictionary(Of String, String()) = dag.GetAncestorTable(relations)
            Dim expanded As New Dictionary(Of String, String())

            For Each gene As KeyValuePair(Of String, String()) In raw
                Dim terms As New HashSet(Of String)

                For Each term As String In gene.Value.SafeQuery
                    Dim id As String = dag.GetTermId(term)

                    If String.IsNullOrEmpty(id) OrElse Not dag.Contains(id) Then
                        ' 目标GO词条在当前的本体数据库之中不存在，
                        ' 可能是已经被废弃掉了的编号
                        Continue For
                    End If

                    terms.Add(id)

                    If ancestorTable.ContainsKey(id) Then
                        For Each ancestor As String In ancestorTable(id)
                            terms.Add(ancestor)
                        Next
                    End If
                Next

                expanded(gene.Key) = terms.ToArray
            Next

            Return expanded
        End Function

        ''' <summary>
        ''' 将注释数据倒排为``[GO term => geneID set]``，用于构建富集计算的背景模型
        ''' </summary>
        ''' <param name="dag"></param>
        ''' <param name="relations"></param>
        ''' <returns></returns>
        Public Function TermGenes(dag As Graph, Optional relations As OntologyRelations() = Nothing) As Dictionary(Of String, List(Of String))
            Dim expanded As Dictionary(Of String, String()) = Expand(dag, relations)
            Dim termGenes As New Dictionary(Of String, List(Of String))

            For Each gene As KeyValuePair(Of String, String()) In expanded
                For Each term As String In gene.Value
                    If Not termGenes.ContainsKey(term) Then
                        Call termGenes.Add(term, New List(Of String))
                    End If

                    termGenes(term).Add(gene.Key)
                Next
            Next

            Return termGenes
        End Function

        Public Overrides Function ToString() As String
            Return $"{raw.Count} genes with GO annotations"
        End Function
    End Class
End Namespace
