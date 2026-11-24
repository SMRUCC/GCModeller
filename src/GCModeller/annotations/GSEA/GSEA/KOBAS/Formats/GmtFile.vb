Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace KOBAS

    ''' <summary>
    ''' 当 GMT 文件之中出现重复的基因集编号时的处理策略。
    ''' </summary>
    Public Enum DuplicateGeneSetPolicy
        ''' <summary>
        ''' 与 KOBAS 的 <c>gsea.read_gmt_file</c> 行为一致：直接抛出格式错误。
        ''' </summary>
        ThrowError
        ''' <summary>
        ''' 与 KOBAS 的 <c>snbea.read_gmt_file</c> 行为一致：保留首次出现的那个基因集。
        ''' </summary>
        KeepFirst
    End Enum

    ''' <summary>
    ''' GMT（Gene Matrix Transposed）基因集文件的解析器。
    ''' </summary>
    ''' <remarks>
    ''' 本模块是 KOBAS 3.0 <c>src/kobas/gsea.py</c> 的 <c>read_gmt_file</c>
    ''' 与 <c>src/kobas/snbea.py</c> 的 <c>read_gmt_file</c> 的 VB.NET 移植。
    ''' 
    ''' GMT 每一行的格式为：``基因集编号 \t 描述 \t 基因1 \t 基因2 ...``。
    ''' 与 KOBAS 原实现的差异：
    ''' <list type="bullet">
    ''' <item>本实现会跳过以 <c>#</c> 开头的注释行（KOBAS 的 gsea 版本不会跳过，
    ''' 但标准 GMT 允许注释行，这里与 snbea 版本保持一致）。</item>
    ''' <item>重复编号的处理方式可以通过 <see cref="DuplicateGeneSetPolicy"/> 选择。</item>
    ''' <item>空基因名与同一行内部的重复基因会被剔除。</item>
    ''' </list>
    ''' </remarks>
    Public Module GmtFile

        ''' <summary>
        ''' 读取 GMT 文件并构建 GSEA 富集计算所需要的背景知识库模型。
        ''' </summary>
        ''' <param name="gmtFile">GMT 文件路径。</param>
        ''' <param name="duplicate">重复基因集编号的处理策略，默认抛出异常。</param>
        ''' <param name="verbose">是否输出解析日志。</param>
        ''' <returns>
        ''' 一个 <see cref="Background"/> 对象；<see cref="Background.size"/> 为所有
        ''' 基因集成员的并集大小。
        ''' </returns>
        ''' <exception cref="FormatException">出现列数少于 3 的行或重复的基因集编号时抛出。</exception>
        Public Function Load(gmtFile As String,
                             Optional duplicate As DuplicateGeneSetPolicy = DuplicateGeneSetPolicy.ThrowError,
                             Optional verbose As Boolean = False) As Background

            Dim result As Background = Parse(File.ReadLines(gmtFile), duplicate, verbose)

            result.name = Path.GetFileName(gmtFile)
            result.id = Path.GetFileNameWithoutExtension(gmtFile)

            Return result
        End Function

        ''' <summary>
        ''' 从文本行序列解析 GMT 数据。
        ''' </summary>
        ''' <param name="lines">GMT 文本行。</param>
        ''' <param name="duplicate">重复基因集编号的处理策略。</param>
        ''' <param name="verbose">是否输出解析日志。</param>
        ''' <returns>解析得到的背景知识库模型。</returns>
        Public Function Parse(lines As IEnumerable(Of String),
                              Optional duplicate As DuplicateGeneSetPolicy = DuplicateGeneSetPolicy.ThrowError,
                              Optional verbose As Boolean = False) As Background

            Dim clusters As New List(Of Cluster)
            Dim [set] As New HashSet(Of String)(StringComparer.Ordinal)
            Dim duplicated As New List(Of String)
            Dim uniques As New HashSet(Of String)(StringComparer.Ordinal)

            For Each line As String In lines
                If line Is Nothing Then
                    Continue For
                End If

                Dim text As String = line.Trim()

                If text.Length = 0 OrElse text.StartsWith("#") Then
                    Continue For
                End If

                Dim words As String() = text.Split(ControlChars.Tab)

                If words.Length < 3 Then
                    Throw New FormatException(
                        "Bad gene set database format. Please check your gene set database format, " &
                        "only gmt file, a tab delimited file are allowed." &
                        $"{vbCrLf}invalid line: {text}")
                End If

                Dim id As String = words(0)
                Dim notes As String = words(1)
                Dim members As String() = words _
                    .Skip(2) _
                    .Select(Function(gene) gene.Trim()) _
                    .Where(Function(gene) Not String.IsNullOrEmpty(gene)) _
                    .Distinct(StringComparer.Ordinal) _
                    .ToArray

                If Not [set].Add(id) Then
                    If duplicate = DuplicateGeneSetPolicy.ThrowError Then
                        Throw New FormatException(
                            "Duplicate gene set names are not allowed in a gmt file. " &
                            $"{vbCrLf}duplicated gene set: {id}")
                    Else
                        duplicated.Add(id)
                        Continue For
                    End If
                End If

                For Each gene As String In members
                    Call uniques.Add(gene)
                Next

                clusters.Add(New Cluster With {
                    .ID = id,
                    .names = notes,
                    .description = notes,
                    .members = members _
                        .Select(Function(gene) New BackgroundGene(gene)) _
                        .ToArray
                })
            Next

            If clusters.Count = 0 Then
                Throw New FormatException($"No gene set was loaded from the given gmt data, {NameOf(GmtFile)}")
            End If

            If verbose Then
                Call Console.WriteLine($"[KOBAS::GMT] {clusters.Count} gene set(s) loaded, {uniques.Count} unique gene(s).")

                If duplicated.Count > 0 Then
                    Call Console.WriteLine($"[KOBAS::GMT] {duplicated.Count} duplicated gene set id(s) were skipped: {duplicated.JoinBy(", ")}")
                End If
            End If

            Return New Background With {
                .name = "GMT",
                .comments = NameOf(GmtFile),
                .build = Now,
                .clusters = clusters.ToArray,
                .size = uniques.Count
            }
        End Function

        ''' <summary>
        ''' 按基因集成员数量过滤背景模型（对应 KOBAS <c>snbea.filter_gsets</c>）。
        ''' </summary>
        ''' <param name="background">待过滤的背景模型。</param>
        ''' <param name="minSize">最小成员数（闭区间）。</param>
        ''' <param name="maxSize">最大成员数（闭区间）。</param>
        ''' <returns>只保留成员数位于 <paramref name="minSize"/> 与 <paramref name="maxSize"/> 之间的基因集。</returns>
        ''' <remarks>
        ''' KOBAS 原实现使用严格不等号（<c>min_size &lt; size &lt; max_size</c>），
        ''' 这里统一为闭区间 <c>[minSize, maxSize]</c>，以便与 <c>HitMatrix</c> 的过滤口径一致。
        ''' </remarks>
        <Extension>
        Public Function FilterBySize(background As Background,
                                     Optional minSize As Integer = 0,
                                     Optional maxSize As Integer = Integer.MaxValue) As Background

            Return New Background With {
                .name = background.name,
                .id = background.id,
                .comments = background.comments,
                .build = background.build,
                .size = background.size,
                .clusters = background.clusters _
                    .Where(Function(cluster) cluster.size >= minSize AndAlso cluster.size <= maxSize) _
                    .ToArray
            }
        End Function
    End Module
End Namespace
