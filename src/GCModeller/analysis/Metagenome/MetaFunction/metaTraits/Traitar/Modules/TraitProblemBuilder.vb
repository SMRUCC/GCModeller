' ============================================================================
' TraitProblemBuilder.vb
'
' 数据装配层：把 Pfam 归一化丰度嵌入（特征）与 TraitAnnotation 数据集（标签）
' 装配为 LibSVM 的 ProblemTable。
'
'   ProblemTable 的每一行 = 一个微生物基因组样本
'   ProblemTable 的每一个 topic（labels 的 key） = 一种生物表型
'
' 因此每一个 topic 都可以直接通过 ProblemTable.GetProblem(topic) 取出一个
' 独立的 Problem 用于训练该表型所对应的 SVM 模型实例。
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.SVM.StorageProcedure
Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString

Namespace metaTraits.Traitar.Modules

    ''' <summary>
    ''' 一个完整的表型训练数据集：嵌入配置 + ProblemTable + 表型元数据
    ''' </summary>
    Public Class TraitTrainingSet

        ''' <summary>Pfam 词表与编码配置</summary>
        Public Property embedding As PfamEmbedding
        ''' <summary>行=基因组样本，topic=表型名的训练数据集</summary>
        Public Property problems As ProblemTable
        ''' <summary>参与训练的表型元数据列表</summary>
        Public Property traits As PhenotypeTrait()

        ''' <summary>样本（基因组）数量</summary>
        Public Function SampleCount() As Integer
            If problems Is Nothing OrElse problems.vectors Is Nothing Then
                Return 0
            End If

            Return problems.vectors.Length
        End Function

        ''' <summary>
        ''' 获取指定表型在训练集之中的有效标签样本数量
        ''' </summary>
        Public Function LabelCount(trait_name As String) As Integer
            If problems Is Nothing OrElse problems.vectors Is Nothing Then
                Return 0
            End If

            Return problems.vectors.Count(Function(v) v.labels.ContainsKey(trait_name))
        End Function

        Public Overrides Function ToString() As String
            Return $"{SampleCount()} genomes, {If(traits Is Nothing, 0, traits.Length)} traits, {embedding}"
        End Function

    End Class

    ''' <summary>
    ''' 把 Pfam 嵌入结果与 TraitAnnotation 标签装配为 ProblemTable
    ''' </summary>
    Public Module TraitProblemBuilder

        ''' <summary>
        ''' 从一组基因组的 Pfam 注释之中构建嵌入配置（词表 + 编码方式）
        ''' </summary>
        ''' <param name="pfams">基因组名 -> 该基因组的蛋白质组 Pfam 注释</param>
        ''' <param name="minGenomes">词表剪枝阈值，默认为 1（不剪枝）</param>
        ''' <param name="encoding">编码方式，默认为 per-genome 归一化计数</param>
        Public Function CreateEmbedding(pfams As IDictionary(Of String, PfamString()),
                                        Optional minGenomes As Integer = 1,
                                        Optional encoding As PfamEncoding = PfamEncoding.NormalizedCount) As PfamEmbedding

            Return PfamEmbedding.BuildVocabulary(pfams, minGenomes, encoding)
        End Function

        ''' <summary>
        ''' 装配训练数据集
        ''' </summary>
        ''' <param name="annotations">
        ''' TraitAnnotation 训练数据集（GTDB + NCBI 的 species summary 表）
        ''' </param>
        ''' <param name="pfams">基因组名 -> 该基因组的蛋白质组 Pfam 注释</param>
        ''' <param name="traits">表型元数据（决定需要提取哪些表型的标签）</param>
        ''' <param name="embedding">Pfam 嵌入配置</param>
        ''' <returns>
        ''' 一个 <see cref="ProblemTable"/>，其 topic 即表型名称
        ''' </returns>
        Public Function Build(annotations As IEnumerable(Of TraitAnnotation),
                              pfams As IDictionary(Of String, PfamString()),
                              traits As IEnumerable(Of PhenotypeTrait),
                              embedding As PfamEmbedding) As TraitTrainingSet

            Dim traitList As PhenotypeTrait() = traits.ToArray
            ' 基因组目录名规范化之后作为索引：
            ' "Carnobacterium_divergens" <-> "Carnobacterium divergens"
            Dim genomes As New Dictionary(Of String, PfamString())
            Dim genomeIds As New Dictionary(Of String, String)

            For Each genome As KeyValuePair(Of String, PfamString()) In pfams
                Dim key As String = PfamEmbedding.NormalizeSpeciesName(genome.Key)

                If key.Length = 0 OrElse genomes.ContainsKey(key) Then
                    Continue For
                End If

                genomes(key) = genome.Value
                genomeIds(key) = genome.Key
            Next

            ' 物种名 -> (表型名 -> 标签值)
            Dim labels As New Dictionary(Of String, Dictionary(Of String, String))
            ' 只保留在表型定义表之中出现过的表型
            Dim traitTypes As New Dictionary(Of String, TraitDataType)

            For Each trait As PhenotypeTrait In traitList
                traitTypes(trait.trait_name) = trait.GetDataType()
            Next

            For Each anno As TraitAnnotation In annotations
                If anno Is Nothing OrElse anno.trait_name Is Nothing Then
                    Continue For
                End If
                If Not traitTypes.ContainsKey(anno.trait_name) Then
                    Continue For
                End If

                Dim taxonKey As String = PfamEmbedding.NormalizeSpeciesName(anno.taxon_name)

                If taxonKey.Length = 0 OrElse Not genomes.ContainsKey(taxonKey) Then
                    ' 该物种没有对应的蛋白质组 Pfam 数据，无法作为训练样本
                    Continue For
                End If

                Dim label As String = anno.GetLabelValue(traitTypes(anno.trait_name))

                If label Is Nothing Then
                    Continue For
                End If

                Dim profile As Dictionary(Of String, String) = Nothing

                If Not labels.TryGetValue(taxonKey, profile) Then
                    profile = New Dictionary(Of String, String)
                    labels(taxonKey) = profile
                End If

                If Not profile.ContainsKey(anno.trait_name) Then
                    profile(anno.trait_name) = label
                End If
            Next

            ' 只保留同时具备 Pfam 特征与至少一个表型标签的基因组
            Dim vectors As New List(Of SupportVector)

            For Each genome As KeyValuePair(Of String, PfamString()) In genomes
                Dim profile As Dictionary(Of String, String) = Nothing

                If Not labels.TryGetValue(genome.Key, profile) Then
                    Continue For
                End If

                Dim counts As Dictionary(Of String, Integer) = PfamEmbedding.CountDomains(genome.Value)

                If counts.Count = 0 Then
                    ' 没有任何 Pfam 结构域注释的基因组不参与训练
                    Continue For
                End If

                vectors.Add(New SupportVector With {
                    .id = genomeIds(genome.Key),
                    .Properties = embedding.Embed(counts),
                    .labels = profile
                })
            Next

            Dim table As New ProblemTable With {
                .dimensionNames = embedding.dimensionNames,
                .vectors = vectors.ToArray
            }

            Return New TraitTrainingSet With {
                .embedding = embedding,
                .problems = table,
                .traits = traitList
            }
        End Function

    End Module
End Namespace
