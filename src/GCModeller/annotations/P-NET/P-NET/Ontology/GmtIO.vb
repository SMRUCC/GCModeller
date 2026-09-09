Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' Gene Matrix Transposed (.gmt) 格式的通路层级读写器
''' </summary>
''' <remarks>
''' 论文中的 P-NET 从 Reactome 通路数据库下载全部通路并整理为 <c>.gmt</c> 文件，
''' 之后由代码自动读取 gmt 文件，把「基因 → 精细通路 → 粗通路 → 生物过程」的父子关系
''' 直接翻译为网络的层数、节点数以及连接方式。
'''
''' gmt 文件的每一行格式为：
'''
''' <code>通路名&lt;TAB&gt;描述&lt;TAB&gt;成员1&lt;TAB&gt;成员2&lt;TAB&gt;...</code>
'''
''' 在 P-NET 的层级描述中，第 k 个 gmt 文件的"成员"实际上是第 k - 1 层的节点名称
''' （第 0 个 gmt 文件的成员为基因名）。因此只需要按照由精细到粗的顺序给出若干 gmt 文件，
''' 即可完整重建整个网络拓扑：更换 KEGG、Gene Ontology 或者自定义通路模块时，
''' 只需要更换 gmt 文件即可重建网络。
''' </remarks>
Public Module GmtIO

    ''' <summary>
    ''' 读取单个 gmt 文件，并把其中的成员名称解析为上一层节点的索引
    ''' </summary>
    ''' <param name="path">gmt 文件路径</param>
    ''' <param name="previousNames">
    ''' 上一层（更精细的层）的节点名称数组；
    ''' 读取第 0 层（成员为基因名）时请改用 <see cref="ReadBaseLevel"/>
    ''' </param>
    ''' <returns>
    ''' 解析得到的通路层对象；若在 <paramref name="previousNames"/> 中不存在的成员名称会被忽略
    ''' </returns>
    Public Function ReadGmt(path As String, previousNames As String()) As HierarchyLevel
        Dim nodes As New List(Of String)()
        Dim members As New List(Of Integer())()
        Dim index As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To previousNames.Length - 1
            If Not index.ContainsKey(previousNames(i)) Then
                index.Add(previousNames(i), i)
            End If
        Next

        For Each line As String In File.ReadLines(path)
            If line Is Nothing Then
                Continue For
            End If

            Dim raw As String = line.Trim()

            If raw.Length = 0 OrElse raw.StartsWith("#") Then
                Continue For
            End If

            Dim parts As String() = raw.Split(New Char() {ControlChars.Tab, ","c})
            Dim name As String = parts(0).Trim()
            Dim startCol As Integer = If(parts.Length < 3, 1, 2)

            If parts.Length <= startCol Then
                Continue For
            End If

            Dim idx As New List(Of Integer)()

            For i As Integer = startCol To parts.Length - 1
                Dim member As String = parts(i).Trim()

                If member.Length = 0 Then
                    Continue For
                End If
                If index.ContainsKey(member) Then
                    idx.Add(index(member))
                End If
            Next

            If idx.Count = 0 Then
                Continue For
            End If

            nodes.Add(name)
            members.Add(idx.Distinct().OrderBy(Function(x) x).ToArray())
        Next

        Return New HierarchyLevel(System.IO.Path.GetFileNameWithoutExtension(path), nodes.ToArray(), members.ToArray())
    End Function

    ''' <summary>
    ''' 读取第 0 层（基因层）gmt 文件，同时取得基因名并集
    ''' </summary>
    ''' <param name="path">gmt 文件路径</param>
    ''' <param name="genes">返回该文件内出现的所有基因名（已去重并保持出现顺序）</param>
    ''' <returns>解析得到的最精细通路层</returns>
    Public Function ReadBaseLevel(path As String, ByRef genes As String()) As HierarchyLevel
        Dim nodes As New List(Of String)()
        Dim members As New List(Of Integer())()
        Dim union As New List(Of String)()

        For Each line As String In File.ReadLines(path)
            If line Is Nothing Then
                Continue For
            End If

            Dim raw As String = line.Trim()

            If raw.Length = 0 OrElse raw.StartsWith("#") Then
                Continue For
            End If

            Dim parts As String() = raw.Split(New Char() {ControlChars.Tab, ","c})
            Dim name As String = parts(0).Trim()
            Dim startCol As Integer = If(parts.Length < 3, 1, 2)

            If parts.Length <= startCol Then
                Continue For
            End If

            Dim idx As New List(Of Integer)()

            For i As Integer = startCol To parts.Length - 1
                Dim member As String = parts(i).Trim()

                If member.Length = 0 Then
                    Continue For
                End If

                Dim at As Integer = union.IndexOf(member)

                If at < 0 Then
                    union.Add(member)
                    at = union.Count - 1
                End If

                idx.Add(at)
            Next

            If idx.Count = 0 Then
                Continue For
            End If

            nodes.Add(name)
            members.Add(idx.Distinct().OrderBy(Function(x) x).ToArray())
        Next

        genes = union.ToArray()

        Return New HierarchyLevel(System.IO.Path.GetFileNameWithoutExtension(path), nodes.ToArray(), members.ToArray())
    End Function

    ''' <summary>
    ''' 按照由精细到粗的顺序读取若干 gmt 文件，组装出完整的生物层级本体
    ''' </summary>
    ''' <param name="files">
    ''' gmt 文件路径数组，必须按照由精细到粗的顺序排列，
    ''' 第 0 个文件的成员为基因名，其余文件的成员为上一文件中的通路名
    ''' </param>
    ''' <returns>清洗之后的生物层级本体对象</returns>
    Public Function FromGmtFiles(files As IEnumerable(Of String)) As PathwayHierarchy
        Dim list As String() = files.ToArray()

        If list.Length = 0 Then
            Throw New ArgumentException("至少需要提供一个 gmt 文件用于描述通路层级", NameOf(files))
        End If

        Dim hierarchy As New PathwayHierarchy()
        Dim genes As String() = Nothing
        Dim baseLevel As HierarchyLevel = ReadBaseLevel(list(0), genes)

        hierarchy.GeneNames = genes
        hierarchy.Levels.Add(baseLevel)

        Dim previousNames As String() = baseLevel.Nodes

        For i As Integer = 1 To list.Length - 1
            Dim level As HierarchyLevel = ReadGmt(list(i), previousNames)

            hierarchy.Levels.Add(level)
            previousNames = level.Nodes
        Next

        Return hierarchy.Cleanup()
    End Function

    ''' <summary>
    ''' 把一个通路层写出为 gmt 文件
    ''' </summary>
    ''' <param name="level">待写出的通路层</param>
    ''' <param name="memberNames">上一层（更精细的层）的节点名称，用于把成员索引还原为名称</param>
    ''' <param name="path">输出的 gmt 文件路径</param>
    ''' <param name="description">写入到 gmt 第二列的描述文本</param>
    Public Sub WriteGmt(level As HierarchyLevel, memberNames As String(), path As String,
                       Optional description As String = "P-NET pathway level")

        Dim sb As New StringBuilder()

        For i As Integer = 0 To level.Count - 1
            sb.Append(level.Nodes(i)).Append(ControlChars.Tab)
            sb.Append(description).Append(ControlChars.Tab)

            Dim members As Integer() = level.GetMembers(i)
            Dim parts As New List(Of String)()

            For Each ci As Integer In members
                If ci >= 0 AndAlso ci < memberNames.Length Then
                    parts.Add(memberNames(ci))
                End If
            Next

            sb.Append(String.Join(ControlChars.Tab, parts))
            sb.AppendLine()
        Next

        File.WriteAllText(path, sb.ToString(), Encoding.UTF8)
    End Sub

    ''' <summary>
    ''' 把整个生物层级本体按照逐层一个 gmt 文件的方式导出到指定目录
    ''' </summary>
    ''' <param name="hierarchy">待导出的生物层级本体</param>
    ''' <param name="directory">输出目录，不存在时会被自动创建</param>
    ''' <returns>按照由精细到粗顺序排列的 gmt 文件路径数组</returns>
    ''' <remarks>
    ''' 文件名采用 ``level1_xxx.gmt`` 的编号形式，重新读取时按文件名排序即可得到正确的层级顺序。
    ''' </remarks>
    Public Function WriteHierarchy(hierarchy As PathwayHierarchy, directory As String) As String()
        If Not System.IO.Directory.Exists(directory) Then
            Call System.IO.Directory.CreateDirectory(directory)
        End If

        Dim files As New List(Of String)()
        Dim previousNames As String() = hierarchy.GeneNames

        For i As Integer = 0 To hierarchy.Levels.Count - 1
            Dim level As HierarchyLevel = hierarchy.Levels(i)
            Dim file As String = System.IO.Path.Combine(directory, $"level{i + 1}_{level.Name}.gmt")

            Call WriteGmt(level, previousNames, file)

            files.Add(file)
            previousNames = level.Nodes
        Next

        Return files.ToArray()
    End Function

End Module
