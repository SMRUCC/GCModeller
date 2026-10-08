' ============================================================================
' PfamEmbedding.vb
'
' Pfam 嵌入层：把微生物基因组所预测的蛋白质组 Pfam 结构域组成嵌入为定长的
' 数值特征向量。
'
' 计数粒度：遍历该基因组所有蛋白的 Pfam-string 数组的每一个条目累加，同一
'           蛋白内出现在不同位置的同一个 PF 结构域会被重复计数。
'
' 归一化：  value(pfam_i) = count(pfam_i) / max(count(*) over genome)
'           即除以该基因组内的最大出现次数，结果落在 [0,1] 区间。
'           这样相比单纯的 0/1 one-hot，能够保留结构域拷贝数（重复出现）
'           所带来的细节信息。
'
' 注意：词表与编码模式必须随模型一并持久化，训练端与预测端必须使用完全相同
'       的嵌入配置，否则预测结果无意义。
' ============================================================================

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.MachineLearning.SVM
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString

Namespace Traitar

    ''' <summary>
    ''' Pfam 结构域组成向量的编码方式
    ''' </summary>
    Public Enum PfamEncoding
        ''' <summary>
        ''' 默认：以该基因组内的最大出现次数做归一化，
        ''' ``value = count / maxCount``，取值区间 [0,1]
        ''' </summary>
        NormalizedCount = 0
        ''' <summary>
        ''' 对照实验用：仅使用存在/缺失信息，
        ''' ``value = If(count > 0, 1.0, 0.0)``
        ''' </summary>
        Binary = 1
    End Enum

    ''' <summary>
    ''' Pfam 词表与嵌入配置（可 JSON 序列化，随模型一同落盘）
    ''' </summary>
    Public Class PfamEmbedding

        ''' <summary>PF 结构域词表，排序之后作为固定顺序的特征维度</summary>
        Public Property dimensionNames As String()
        ''' <summary>编码方式，默认为 per-genome 归一化计数</summary>
        Public Property encoding As PfamEncoding = PfamEncoding.NormalizedCount
        ''' <summary>
        ''' 词表剪枝阈值：PF 至少在 N 个基因组之中出现才会被保留，
        ''' 1 表示不做任何剪枝
        ''' </summary>
        Public Property minGenomes As Integer = 1

        ''' <summary>
        ''' 特征向量的维度数量
        ''' </summary>
        Public Function GetDimensionCount() As Integer
            If dimensionNames Is Nothing Then
                Return 0
            Else
                Return dimensionNames.Length
            End If
        End Function

        Public Overrides Function ToString() As String
            Return $"{GetDimensionCount()} pfam dims, encoding={encoding}, minGenomes={minGenomes}"
        End Function

        Private Shared ReadOnly pfamIdPattern As New Regex("^[Pp][Ff]\d+$", RegexOptions.Compiled)

        ''' <summary>
        ''' 从 Pfam-string 之中的一个条目之中解析出 PF 结构域编号
        ''' </summary>
        ''' <param name="token">
        ''' Pfam-string 数组之中的单个元素，格式形如
        ''' ``PF00044:Gp_dh_N(3|103)``
        ''' </param>
        ''' <returns>解析成功的 PF 编号（不含版本号），失败时返回 Nothing</returns>
        Public Shared Function ParsePfamId(token As String) As String
            If token Is Nothing Then
                Return Nothing
            End If

            Dim id As String = token.Split(":"c)(0).Trim

            If id.Length = 0 Then
                Return Nothing
            End If
            ' 去掉版本号：PF25078.2 -> PF25078
            If id.IndexOf("."c) > -1 Then
                id = id.Split("."c)(0)
            End If

            If Not pfamIdPattern.IsMatch(id) Then
                Return Nothing
            End If

            Return id.ToUpper
        End Function

        ''' <summary>
        ''' 枚举一个基因组蛋白质组之中出现的全部 PF 结构域编号
        ''' （同一蛋白内重复出现的编号会被重复枚举，用于计数）
        ''' </summary>
        Public Shared Iterator Function EnumerateDomains(proteins As IEnumerable(Of PfamString)) As IEnumerable(Of String)
            If proteins Is Nothing Then
                Return
            End If

            For Each protein As PfamString In proteins
                If protein Is Nothing OrElse protein.PfamString Is Nothing Then
                    Continue For
                End If

                For Each token As String In protein.PfamString
                    Dim id As String = ParsePfamId(token)

                    If id IsNot Nothing Then
                        Yield id
                    End If
                Next
            Next
        End Function

        ''' <summary>
        ''' 统计单个基因组之中每一个 PF 结构域的命中总次数
        ''' </summary>
        ''' <param name="proteins">该基因组所预测出来的蛋白质组 Pfam 注释</param>
        ''' <returns>PF 编号 -> 命中总次数</returns>
        Public Shared Function CountDomains(proteins As IEnumerable(Of PfamString)) As Dictionary(Of String, Integer)
            Dim counts As New Dictionary(Of String, Integer)

            For Each id As String In EnumerateDomains(proteins)
                If counts.ContainsKey(id) Then
                    counts(id) = counts(id) + 1
                Else
                    counts(id) = 1
                End If
            Next

            Return counts
        End Function

        ''' <summary>
        ''' 从一组基因组之中构建全局的 PF 词表
        ''' </summary>
        ''' <param name="pfams">基因组名 -> 该基因组的蛋白质组 Pfam 注释</param>
        ''' <param name="minGenomes">词表剪枝阈值，默认为 1（不剪枝）</param>
        ''' <param name="encoding">编码方式</param>
        Public Shared Function BuildVocabulary(pfams As IDictionary(Of String, PfamString()),
                                               Optional minGenomes As Integer = 1,
                                               Optional encoding As PfamEncoding = PfamEncoding.NormalizedCount) As PfamEmbedding

            If minGenomes < 1 Then
                minGenomes = 1
            End If

            ' 文档频率：某一个 PF 在多少个基因组之中出现过
            Dim df As New Dictionary(Of String, Integer)

            For Each genome As KeyValuePair(Of String, PfamString()) In pfams
                If genome.Value Is Nothing Then
                    Continue For
                End If

                For Each id As String In CountDomains(genome.Value).Keys
                    If df.ContainsKey(id) Then
                        df(id) = df(id) + 1
                    Else
                        df(id) = 1
                    End If
                Next
            Next

            Dim dims As String() = df _
                .Where(Function(kv) kv.Value >= minGenomes) _
                .Select(Function(kv) kv.Key) _
                .OrderBy(Function(id) id) _
                .ToArray

            Return New PfamEmbedding With {
                .dimensionNames = dims,
                .encoding = encoding,
                .minGenomes = minGenomes
            }
        End Function

        ''' <summary>
        ''' 把单个基因组的 PF 命中计数嵌入为定长特征向量
        ''' </summary>
        ''' <param name="counts">PF 编号 -> 命中总次数</param>
        ''' <returns>
        ''' 长度等于 <see cref="dimensionNames"/> 的向量，取值区间为 [0,1]
        ''' </returns>
        Public Function Embed(counts As Dictionary(Of String, Integer)) As Dictionary(Of String, Double)
            Dim vector As New Dictionary(Of String, Double)
            Dim maxCount As Integer = 0

            If counts IsNot Nothing Then
                For Each n As Integer In counts.Values
                    If n > maxCount Then
                        maxCount = n
                    End If
                Next
            End If

            If dimensionNames Is Nothing Then
                Return vector
            End If

            For Each id As String In dimensionNames
                Dim n As Integer = 0

                If counts Is Nothing OrElse Not counts.TryGetValue(id, n) OrElse n <= 0 Then
                    vector(id) = 0.0
                    Continue For
                End If

                If encoding = PfamEncoding.Binary Then
                    vector(id) = 1.0
                ElseIf maxCount > 0 Then
                    ' 除以该基因组内的最大出现次数做归一化
                    vector(id) = n / maxCount
                Else
                    ' maxCount = 0 的保护：不应该发生，因为 n > 0 意味着 maxCount >= n
                    vector(id) = 0.0
                End If
            Next

            Return vector
        End Function

        ''' <summary>
        ''' 直接从一个基因组的蛋白质组 Pfam 注释嵌入为定长特征向量
        ''' </summary>
        Public Function EmbedProteins(proteins As IEnumerable(Of PfamString)) As Dictionary(Of String, Double)
            Return Embed(CountDomains(proteins))
        End Function

        ''' <summary>
        ''' 把嵌入向量转换为 LibSVM 所需要的 1-based 稠密特征向量
        ''' </summary>
        ''' <remarks>
        ''' 必须是稠密的：RangeTransform 会把最小值映射为 -1，如果稀疏地
        ''' 省略掉 0 值位，预测时会被当作 0 而计算出错误的结果。
        ''' </remarks>
        Public Function ToNodes(profile As IDictionary(Of String, Double)) As Node()
            Dim n As Integer = GetDimensionCount()
            Dim vector As Node() = New Node(n - 1) {}

            For i As Integer = 0 To n - 1
                Dim value As Double = 0

                If profile IsNot Nothing Then
                    profile.TryGetValue(dimensionNames(i), value)
                End If

                vector(i) = New Node(i + 1, value)
            Next

            Return vector
        End Function

        ''' <summary>
        ''' 物种名规范化，用于把 pfam 目录名（``Carnobacterium_divergens``）
        ''' 与表型表之中的 ``taxon_name``（``Carnobacterium divergens``）
        ''' 匹配起来
        ''' </summary>
        Public Shared Function NormalizeSpeciesName(name As String) As String
            If name Is Nothing Then
                Return ""
            End If

            Dim sb As New StringBuilder()

            For Each c As Char In name.ToLower
                If Char.IsLetterOrDigit(c) Then
                    Call sb.Append(c)
                End If
            Next

            Return sb.ToString
        End Function

        ''' <summary>
        ''' 把词表与编码配置写入 json 文件
        ''' </summary>
        Public Sub Save(file As String)
            Call System.IO.File.WriteAllText(file, Me.GetJson)
        End Sub

        ''' <summary>
        ''' 从 json 文件之中还原词表与编码配置
        ''' </summary>
        Public Shared Function Load(file As String) As PfamEmbedding
            Return System.IO.File.ReadAllText(file).LoadJSON(Of PfamEmbedding)()
        End Function

    End Class
End Namespace
