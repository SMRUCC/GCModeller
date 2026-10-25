Imports System.IO
Imports System.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSEA

Namespace Omics

    ''' <summary>
    ''' 拼接矩阵中的单个特征条目，记录 (feature_id, omics_source, gene) 三元索引。
    ''' </summary>
    Public Class OmicsFeature
        ''' <summary>拼接矩阵中的唯一特征 id，格式为 <c>{omics}:{gene}</c></summary>
        Public Property feature_id As String
        ''' <summary>原始的基因符号（用于与基因集对齐）</summary>
        Public Property gene As String
        ''' <summary>该特征所属的组学来源</summary>
        Public Property omics As String

        Public Overrides Function ToString() As String
            Return feature_id
        End Function
    End Class

    ''' <summary>
    ''' K 个组学层的容器：统一样本轴 + 特征三元索引 + 拼接视图。
    ''' </summary>
    ''' <remarks>
    ''' 对应 method.md 步骤1：输入为拼接矩阵 X ∈ R^{(N1+N2+...+NK)×M}，
    ''' 特征标签表保留 (feature_id, omics_source, gene)；
    ''' 但主流程<b>不会</b>真的把矩阵拼接后做一次 GSVA（见 <see cref="Ablation.PooledBaseline"/> 的消融对照），
    ''' 拼接视图只用于对外导出特征表与消融实验。
    ''' </remarks>
    Public Class MultiOmicsDataset

        ''' <summary>已按统一样本轴对齐的组学层</summary>
        Public Property Layers As OmicsLayer()

        ''' <summary>跨组学共享的样本轴（按第一个层的原始顺序）</summary>
        Public Property SampleID As String()

        ''' <summary>拼接矩阵的特征表</summary>
        Public Property FeatureTable As OmicsFeature()

        ''' <summary>因为并非所有层都含有而被丢弃的样本</summary>
        Public Property DroppedSamples As String()

        Public ReadOnly Property LayerCount As Integer
            Get
                Return If(Layers Is Nothing, 0, Layers.Length)
            End Get
        End Property

        Public ReadOnly Property OmicsNames As String()
            Get
                If Layers Is Nothing Then
                    Return New String() {}
                End If

                Return Layers.Select(Function(l) l.Name).ToArray
            End Get
        End Property

        ''' <summary>样本数量</summary>
        Public ReadOnly Property SampleCount As Integer
            Get
                Return If(SampleID Is Nothing, 0, SampleID.Length)
            End Get
        End Property

        ''' <summary>所有层特征数之和（即拼接矩阵的行数）</summary>
        Public ReadOnly Property FeatureCount As Integer
            Get
                If Layers Is Nothing Then
                    Return 0
                End If

                Return Layers.Sum(Function(l) l.FeatureCount)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{LayerCount} omics layer(s), {FeatureCount} features x {SampleCount} samples"
        End Function

        Public Function GetLayer(name As String) As OmicsLayer
            Return Layers.FirstOrDefault(Function(l) l.Name.TextEquals(name, null_equals:=False, empty_equals:=False))
        End Function

        ''' <summary>
        ''' 构造多组学数据集：校验层名唯一、求样本交集、逐层对齐、建立特征表。
        ''' </summary>
        Public Shared Function Create(layers As IEnumerable(Of OmicsLayer), Optional verbose As Boolean = True) As MultiOmicsDataset
            If layers Is Nothing Then
                Throw New ArgumentNullException(NameOf(layers))
            End If

            Dim list As OmicsLayer() = layers.ToArray

            If list.Length = 0 Then
                Throw New ArgumentException("at least one omics layer is required!")
            End If

            Dim names As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            For Each layer As OmicsLayer In list
                If Not names.Add(layer.Name) Then
                    Throw New ArgumentException($"duplicated omics layer name: {layer.Name}")
                End If
            Next

            Dim sets As Dictionary(Of String, HashSet(Of String)) = list _
                .ToDictionary(Function(l) l.Name,
                              Function(l) New HashSet(Of String)(l.SampleID, StringComparer.Ordinal))
            Dim axis As New List(Of String)

            For Each id As String In list(0).SampleID
                Dim ok As Boolean = True

                For k As Integer = 1 To list.Length - 1
                    If Not sets(list(k).Name).Contains(id) Then
                        ok = False
                        Exit For
                    End If
                Next

                If ok Then
                    axis.Add(id)
                End If
            Next

            Dim dropped As String() = list(0).SampleID _
                .Where(Function(id) Not axis.Contains(id, StringComparer.Ordinal)) _
                .ToArray

            If axis.Count < 2 Then
                Throw New InvalidOperationException("less than two samples are shared by all omics layers!")
            End If
            If verbose AndAlso dropped.Length > 0 Then
                Call $"[moGSVA] {dropped.Length} sample(s) are not shared by all omics layers and were dropped: {String.Join(", ", dropped.Take(5))}".warning
            End If

            Dim canonical As String() = axis.ToArray
            Dim aligned As OmicsLayer() = list _
                .Select(Function(l)
                            If l.SampleID.SequenceEqual(canonical) Then
                                Return l
                            Else
                                Return l.AlignSamples(canonical)
                            End If
                        End Function) _
                .ToArray

            Return New MultiOmicsDataset With {
                .Layers = aligned,
                .SampleID = canonical,
                .DroppedSamples = dropped,
                .FeatureTable = BuildFeatureTable(aligned)
            }
        End Function

        ''' <summary>
        ''' 只保留给定的基因集合，返回新的数据集（用于置换检验的降规模视图）。
        ''' </summary>
        Public Function SubsetGenes(genes As IEnumerable(Of String)) As MultiOmicsDataset
            Dim subset As String() = genes.Distinct(StringComparer.Ordinal).ToArray
            Dim layers As OmicsLayer() = Me.Layers _
                .Select(Function(l) l.SubsetGenes(subset)) _
                .ToArray

            Return New MultiOmicsDataset With {
                .Layers = layers,
                .SampleID = SampleID,
                .DroppedSamples = DroppedSamples,
                .FeatureTable = BuildFeatureTable(layers)
            }
        End Function

        ''' <summary>
        ''' 拼接视图：所有层的行按层顺序堆叠，特征 id 加上 <c>{omics}:</c> 前缀避免撞名。
        ''' </summary>
        Public Function Pooled() As Matrix
            Dim rows As New List(Of DataFrameRow)

            For Each layer As OmicsLayer In Layers
                For Each r As DataFrameRow In layer.Expr.expression
                    rows.Add(New DataFrameRow With {
                        .geneID = $"{layer.Name}:{r.geneID}",
                        .experiments = r.experiments
                    })
                Next
            Next

            Return New Matrix With {
                .sampleID = SampleID,
                .tag = "moGSVA:pooled",
                .expression = rows.ToArray
            }
        End Function

        ''' <summary>
        ''' 计算每条通路在每个组学层中的成员数 |S_k|。
        ''' </summary>
        ''' <returns>pathway → (omics → 成员数)</returns>
        ''' <remarks>
        ''' 成员数的统计口径与 GSVA 内部 <c>Cluster.Intersect(features)</c> 一致：
        ''' 通路成员 id 去重后与当前层的特征 id 集合求交集。
        ''' </remarks>
        Public Function Coverage(geneSets As Background) As Dictionary(Of String, Dictionary(Of String, Integer))
            Dim result As New Dictionary(Of String, Dictionary(Of String, Integer))(StringComparer.Ordinal)

            If geneSets Is Nothing OrElse geneSets.clusters Is Nothing Then
                Return result
            End If

            Dim featureSets As Dictionary(Of String, HashSet(Of String)) = Layers _
                .ToDictionary(Function(l) l.Name,
                              Function(l) New HashSet(Of String)(l.Genes, StringComparer.Ordinal))

            For Each cluster As Cluster In geneSets.clusters
                If cluster Is Nothing OrElse String.IsNullOrEmpty(cluster.ID) Then
                    Continue For
                End If
                If result.ContainsKey(cluster.ID) Then
                    Continue For
                End If

                Dim members As String() = If(cluster.members, New BackgroundGene() {}) _
                    .Select(Function(g) If(g Is Nothing, Nothing, g.accessionID)) _
                    .Where(Function(id) Not String.IsNullOrEmpty(id)) _
                    .Distinct(StringComparer.Ordinal) _
                    .ToArray
                Dim perLayer As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

                For Each layer As OmicsLayer In Layers
                    Dim features As HashSet(Of String) = featureSets(layer.Name)
                    perLayer(layer.Name) = members.Where(Function(id) features.Contains(id)).Count()
                Next

                result(cluster.ID) = perLayer
            Next

            Return result
        End Function

        ''' <summary>
        ''' 计算每条通路在每个组学层中实际匹配到的特征 id 列表。
        ''' </summary>
        Public Function GeneSetMembers(geneSets As Background) As Dictionary(Of String, Dictionary(Of String, String()))
            Dim result As New Dictionary(Of String, Dictionary(Of String, String()))(StringComparer.Ordinal)

            If geneSets Is Nothing OrElse geneSets.clusters Is Nothing Then
                Return result
            End If

            For Each cluster As Cluster In geneSets.clusters
                If cluster Is Nothing OrElse String.IsNullOrEmpty(cluster.ID) Then
                    Continue For
                End If
                If result.ContainsKey(cluster.ID) Then
                    Continue For
                End If

                Dim members As New HashSet(Of String)(
                    If(cluster.members, New BackgroundGene() {}) _
                        .Select(Function(g) If(g Is Nothing, Nothing, g.accessionID)) _
                        .Where(Function(id) Not String.IsNullOrEmpty(id)),
                    StringComparer.Ordinal)
                Dim perLayer As New Dictionary(Of String, String())(StringComparer.Ordinal)

                For Each layer As OmicsLayer In Layers
                    perLayer(layer.Name) = layer.Genes.Where(Function(g) members.Contains(g)).ToArray
                Next

                result(cluster.ID) = perLayer
            Next

            Return result
        End Function

        ''' <summary>
        ''' 导出特征表为 csv 文件（feature_id, omics, gene）。
        ''' </summary>
        Public Sub SaveFeatureTable(path As String)
            Using writer As New StreamWriter(path, False, Text.Encoding.UTF8)
                Call writer.WriteLine("feature_id,omics,gene")

                For Each feature As OmicsFeature In FeatureTable
                    Call writer.WriteLine($"{feature.feature_id},{feature.omics},{feature.gene}")
                Next
            End Using
        End Sub

        Private Shared Function BuildFeatureTable(layers As OmicsLayer()) As OmicsFeature()
            Return layers _
                .SelectMany(
                    Function(l)
                        Return l.Expr.expression.Select(
                            Function(r)
                                Return New OmicsFeature With {
                                    .feature_id = $"{l.Name}:{r.geneID}",
                                    .gene = r.geneID,
                                    .omics = l.Name
                                }
                            End Function)
                    End Function) _
                .ToArray
        End Function

    End Class

End Namespace
