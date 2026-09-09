Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.DAG
Imports oboDef = SMRUCC.genomics.Data.GeneOntology.OBO.Definition
Imports oboTerm = SMRUCC.genomics.Data.GeneOntology.OBO.Term

Namespace GO

    ''' <summary>
    ''' 基于GO DAG图的富集分析模块
    ''' </summary>
    ''' <remarks>
    ''' Gene Ontology可分为分子功能(Molecular Function)，生物过程(biological process)
    ''' 和细胞组成(cellular component)三个部分。
    ''' 
    '''
    ''' ###### GO分析
    ''' 
    ''' 根据挑选出的差异基因，计算这些差异基因同GO分类中某（几）个特定的分支的
    ''' 超几何分布关系，GO分析可对实验中差异基因进行GO分类注释，并找出与实验
    ''' 目的显著相关的GO分类。
    ''' 
    ''' 由于在GO之中，一个基因可能只被标注到了某一个非常具体的深层词条上面，
    ''' 而按照GO的true path rule，这个基因同时也属于该词条的所有祖先词条，
    ''' 所以这里在构建背景模型的时候会先沿着DAG图做一次**祖先传播**，
    ''' 之后再进行标准的Fisher精确检验。
    ''' </remarks>
    Public Module GOEnrichment

        ''' <summary>
        ''' 获取得到三大本体所对应的namespace字符串
        ''' </summary>
        ''' <param name="ontology"></param>
        ''' <returns></returns>
        Public Function NamespaceOf(ontology As Ontologies) As String
            Select Case ontology
                Case Ontologies.BiologicalProcess
                    Return "biological_process"
                Case Ontologies.CellularComponent
                    Return "cellular_component"
                Case Ontologies.MolecularFunction
                    Return "molecular_function"
                Case Else
                    Return Nothing
            End Select
        End Function

        ''' <summary>
        ''' 由基因 => GO词条的注释数据构建出GO富集计算所需要的背景模型
        ''' </summary>
        ''' <param name="annotations">基因的GO注释数据</param>
        ''' <param name="dag">GO本体DAG图，即``go.obo``数据库</param>
        ''' <param name="relations">
        ''' 除了``is_a``之外还需要参与注释传播的relationship关系类型
        ''' </param>
        ''' <param name="name">背景模型的名称</param>
        ''' <returns>
        ''' 每一个GO词条就是一个<see cref="Cluster"/>，其中的members为
        ''' **经过祖先传播之后**属于该词条的所有的基因
        ''' </returns>
        <Extension>
        Public Function CreateGOBackground(annotations As GOAnnotation,
                                           dag As Graph,
                                           Optional relations As OntologyRelations() = Nothing,
                                           Optional name As String = Nothing) As Background

            Dim termGenes As Dictionary(Of String, List(Of String)) = annotations.TermGenes(dag, relations)
            Dim clusters As New List(Of Cluster)

            For Each term As KeyValuePair(Of String, List(Of String)) In termGenes
                Dim node As TermNode = dag.GetTerm(term.Key)

                If node Is Nothing OrElse node.GO_term Is Nothing Then
                    Continue For
                End If

                Dim go As oboTerm = node.GO_term
                Dim members As BackgroundGene() = term.Value _
                    .Select(Function(geneID) New BackgroundGene(geneID)) _
                    .ToArray
                Dim definition As String = ""

                If Not String.IsNullOrEmpty(go.def) Then
                    definition = oboDef.Parse(go).definition
                End If

                ' 一个GO词条类似于一个cluster
                ' category/class用于标记该词条所从属的三大本体
                clusters.Add(New Cluster With {
                    .ID = term.Key,
                    .names = If(go.name, term.Key),
                    .description = definition,
                    .category = node.namespace,
                    .class = node.namespace,
                    .members = members
                })
            Next

            Return New Background With {
                .name = If(String.IsNullOrEmpty(name), "Gene Ontology", name),
                .build = Now,
                .clusters = clusters.ToArray,
                .size = annotations.universeSize,
                .comments = "Gene Ontology DAG enrichment background"
            }
        End Function

        ''' <summary>
        ''' 一步到位：由基因的GO注释数据直接做GO富集分析
        ''' </summary>
        ''' <param name="annotations">基因的GO注释数据</param>
        ''' <param name="dag">GO本体DAG图，即``go.obo``数据库</param>
        ''' <param name="geneSet">需要进行富集计算分析的目标基因列表</param>
        ''' <param name="relations"></param>
        ''' <param name="ontology">只计算指定的本体(BP/CC/MF)，为空的时候三个本体一起计算</param>
        ''' <param name="cutSize">过滤掉成员数量小于该值的功能聚类</param>
        ''' <param name="outputAll"></param>
        ''' <param name="isLocustag"></param>
        ''' <returns>
        ''' 已经做了BH校正，并且按照p值从小到大排序之后的结果
        ''' </returns>
        <Extension>
        Public Function Enrichment(annotations As GOAnnotation,
                                   dag As Graph,
                                   geneSet As IEnumerable(Of String),
                                   Optional relations As OntologyRelations() = Nothing,
                                   Optional ontology As Ontologies? = Nothing,
                                   Optional cutSize As Integer = 3,
                                   Optional outputAll As Boolean = False,
                                   Optional isLocustag As Boolean = False) As EnrichmentResult()

            Dim background As Background = annotations.CreateGOBackground(dag, relations)

            Return background.EnrichGO(
                geneSet:=geneSet,
                ontology:=ontology,
                cutSize:=cutSize,
                outputAll:=outputAll,
                isLocustag:=isLocustag
            )
        End Function

        ''' <summary>
        ''' 对已经构建好的GO背景模型做富集计算分析
        ''' </summary>
        ''' <param name="background">
        ''' 由<see cref="CreateGOBackground(GOAnnotation, Graph, OntologyRelations(), String)"/>所构建的背景模型
        ''' </param>
        ''' <param name="geneSet">需要进行富集计算分析的目标基因列表</param>
        ''' <param name="ontology">只计算指定的本体(BP/CC/MF)，为空的时候三个本体一起计算</param>
        ''' <param name="cutSize">过滤掉成员数量小于该值的功能聚类</param>
        ''' <param name="outputAll"></param>
        ''' <param name="isLocustag"></param>
        ''' <param name="showProgress"></param>
        ''' <returns>
        ''' 已经做了BH校正，并且按照p值从小到大排序之后的结果
        ''' </returns>
        <Extension>
        Public Function EnrichGO(background As Background,
                                 geneSet As IEnumerable(Of String),
                                 Optional ontology As Ontologies? = Nothing,
                                 Optional cutSize As Integer = 3,
                                 Optional outputAll As Boolean = False,
                                 Optional isLocustag As Boolean = False,
                                 Optional showProgress As Boolean = False) As EnrichmentResult()

            Dim genes As String() = geneSet _
                .SafeQuery _
                .Where(Function(id) Not String.IsNullOrEmpty(id)) _
                .Distinct _
                .ToArray
            Dim input As Background = background

            If ontology.HasValue Then
                Dim ns As String = NamespaceOf(ontology.Value)

                input = background.SubsetOf(
                    Function(cluster)
                        Return ns.Equals(cluster.category, StringComparison.OrdinalIgnoreCase)
                    End Function)
            End If

            Dim result As EnrichmentResult() = input _
                .Enrichment(list:=genes,
                            resize:=-1,
                            cutSize:=cutSize,
                            outputAll:=outputAll,
                            isLocustag:=isLocustag,
                            showProgress:=showProgress) _
                .ToArray

            Return result.FDRCorrection()
        End Function

        ''' <summary>
        ''' 按照BP/CC/MF三大本体对富集计算的结果进行分组
        ''' </summary>
        ''' <param name="result"></param>
        ''' <returns></returns>
        <Extension>
        Public Function SplitByOntology(result As IEnumerable(Of EnrichmentResult)) As Dictionary(Of String, EnrichmentResult())
            Return result _
                .SafeQuery _
                .GroupBy(Function(term)
                             Return If(String.IsNullOrEmpty(term.category), "Unknown", term.category)
                         End Function) _
                .ToDictionary(Function(group) group.Key,
                              Function(group)
                                  Return group.ToArray
                              End Function)
        End Function

        ''' <summary>
        ''' 将富集计算的结果导出为csv表格文件
        ''' </summary>
        ''' <param name="result"></param>
        ''' <param name="path"></param>
        ''' <returns></returns>
        <Extension>
        Public Function SaveTable(result As IEnumerable(Of EnrichmentResult), path As String) As Boolean
            Using writer As New StreamWriter(path, False, New UTF8Encoding(False))
                Call writer.WriteLine({"goID", "name", "namespace", "class", "description",
                                       "score", "pvalue", "FDR", "termSize", "enriched", "genes"} _
                    .Select(AddressOf csvEscapes) _
                    .JoinBy(","))

                For Each term As EnrichmentResult In result.SafeQuery
                    Call writer.WriteLine({
                        term.term,
                        term.name,
                        term.category,
                        term.class,
                        term.description,
                        term.score.ToString("G6"),
                        term.pvalue.ToString("G6"),
                        term.FDR.ToString("G6"),
                        CStr(term.cluster),
                        CStr(term.enriched),
                        term.IDs.JoinBy("; ")
                    } _
                    .Select(AddressOf csvEscapes) _
                    .JoinBy(","))
                Next
            End Using

            Return True
        End Function

        Private Function csvEscapes(text As String) As String
            If text Is Nothing Then
                Return ""
            End If

            text = text.Replace("""", "'").Replace(vbCr, " ").Replace(vbLf, " ")

            Return $"""{text}"""
        End Function
    End Module
End Namespace
