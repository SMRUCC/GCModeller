Imports System.IO
Imports SMRUCC.genomics.Analysis.HTS.GSEA

Namespace Omics

    ''' <summary>
    ''' 极简的 GMT（Gene Matrix Transposed）解析器。
    ''' </summary>
    ''' <remarks>
    ''' GMT 每行一个基因集：<c>ID \t description \t gene1 \t gene2 ...</c>；
    ''' 以 <c>#</c> 开头的行为注释（通常记录 species / database 等元信息），此处直接跳过。
    ''' 
    ''' 之所以在本项目内自行解析而不复用 GSEA 项目中的 <c>KOBAS.Gmt</c>，
    ''' 是为了避免让 moGSVA 依赖整条 GSEA 分析链（GMT 语法只有十来行）。
    ''' </remarks>
    Public Module GeneSetTable

        ''' <summary>
        ''' 从 GMT 文件加载基因集，包装为 <see cref="Background"/>。
        ''' </summary>
        ''' <param name="path">GMT 文件路径</param>
        ''' <param name="verbose">是否输出解析统计与告警</param>
        Public Function LoadGmt(path As String, Optional verbose As Boolean = True) As Background
            If Not File.Exists(path) Then
                Throw New FileNotFoundException($"the GMT file does not exist: {path}", path)
            End If

            Dim clusters As Cluster() = Parse(File.ReadLines(path), verbose)
            Dim name As String = IO.Path.GetFileNameWithoutExtension(path)

            Return New Background With {
                .id = name,
                .name = name,
                .clusters = clusters,
                .size = clusters _
                    .SelectMany(Function(c) c.members.Select(Function(g) g.accessionID)) _
                    .Distinct(StringComparer.Ordinal) _
                    .Count()
            }
        End Function

        ''' <summary>
        ''' 解析 GMT 文本行，返回基因集数组。
        ''' </summary>
        Public Function Parse(lines As IEnumerable(Of String), Optional verbose As Boolean = True) As Cluster()
            Dim clusters As New List(Of Cluster)
            Dim ids As New HashSet(Of String)(StringComparer.Ordinal)
            Dim skipped As Integer = 0
            Dim duplicated As Integer = 0

            For Each line As String In lines
                If String.IsNullOrWhiteSpace(line) Then
                    Continue For
                End If
                If line.TrimStart().StartsWith("#") Then
                    Continue For
                End If

                Dim fields As String() = line.Split(ControlChars.Tab)

                If fields.Length < 3 Then
                    skipped += 1
                    Continue For
                End If

                Dim id As String = fields(0).Trim
                Dim description As String = fields(1).Trim

                If id.Length = 0 Then
                    skipped += 1
                    Continue For
                End If

                Dim members As String() = fields _
                    .Skip(2) _
                    .Select(Function(s) s.Trim) _
                    .Where(Function(s) s.Length > 0) _
                    .Distinct(StringComparer.Ordinal) _
                    .ToArray

                If members.Length = 0 Then
                    skipped += 1
                    Continue For
                End If

                If Not ids.Add(id) Then
                    duplicated += 1
                    Continue For
                End If

                clusters.Add(New Cluster With {
                    .ID = id,
                    .names = id,
                    .description = If(description.Length = 0, id, description),
                    .members = members.Select(Function(g) New BackgroundGene(g)).ToArray
                })
            Next

            If verbose AndAlso skipped > 0 Then
                Call $"[moGSVA] GMT: {skipped} line(s) were skipped because of insufficient columns or empty member list.".warning
            End If
            If verbose AndAlso duplicated > 0 Then
                Call $"[moGSVA] GMT: {duplicated} duplicated gene set id(s) were ignored.".warning
            End If

            Return clusters.ToArray
        End Function

    End Module

End Namespace
