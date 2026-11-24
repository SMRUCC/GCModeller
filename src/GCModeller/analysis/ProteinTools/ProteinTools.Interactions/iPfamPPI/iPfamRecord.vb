Imports System.Text

Namespace iPfamPPI

    ''' <summary>
    ''' iPfam 数据库之中的一条「结构域–结构域」互作记录。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' iPfam 从 PDB 复合物结构之中提取哪些 Pfam 结构域彼此接触，因此它是
    ''' 「结构层面互作证据」的来源。结合直系同源映射(interolog)之后，
    ''' 就可以把已知复合物之中观测到的结构域互作迁移到目标基因组。
    ''' </para>
    ''' <para>
    ''' 原始文件是制表符分隔的文本，列的含义依次为
    ''' <c>pfamA_acc</c>、<c>pfamB_acc</c>、<c>pfamA_id</c>、<c>pfamB_id</c>、
    ''' <c>pdb_id</c>、<c>score</c>、<c>evidence</c>。解析时会容忍列数不足的行。
    ''' </para>
    ''' </remarks>
    Public Class iPfamRecord

        ''' <summary>
        ''' 第一个结构域的 Pfam 登录号
        ''' </summary>
        Public Property PfamA As String
        ''' <summary>
        ''' 第二个结构域的 Pfam 登录号
        ''' </summary>
        Public Property PfamB As String
        ''' <summary>
        ''' 第一个结构域的名称
        ''' </summary>
        Public Property NameA As String
        ''' <summary>
        ''' 第二个结构域的名称
        ''' </summary>
        Public Property NameB As String
        ''' <summary>
        ''' 证据来源的 PDB 编号
        ''' </summary>
        Public Property PdbId As String
        ''' <summary>
        ''' 证据强度(接触的原子对数或者实验支持数)
        ''' </summary>
        Public Property Score As Double
        ''' <summary>
        ''' 附加证据描述(例如实验方法)
        ''' </summary>
        Public Property Evidence As String

        ''' <summary>
        ''' 无序的结构域对键，用于去重
        ''' </summary>
        Public ReadOnly Property PairKey As String
            Get
                Dim left As String = If(PfamA, "")
                Dim right As String = If(PfamB, "")

                If String.CompareOrdinal(left, right) <= 0 Then
                    Return left & "|" & right
                Else
                    Return right & "|" & left
                End If
            End Get
        End Property

        ''' <summary>
        ''' 从制表符或者逗号分隔的文本之中解析全部记录
        ''' </summary>
        ''' <param name="path">数据文件路径</param>
        Public Shared Function Load(path As String) As iPfamRecord()
            If Not System.IO.File.Exists(path) Then
                Throw New System.IO.FileNotFoundException($"the iPfam table '{path}' does not exist", path)
            End If

            Return Parse(System.IO.File.ReadAllLines(path))
        End Function

        ''' <summary>
        ''' 解析文本行
        ''' </summary>
        Public Shared Function Parse(lines As IEnumerable(Of String)) As iPfamRecord()
            Dim result As New List(Of iPfamRecord)

            For Each line As String In lines
                If String.IsNullOrWhiteSpace(line) Then
                    Continue For
                End If
                If line.StartsWith("#"c) Then
                    Continue For
                End If

                Dim cells As String() = SplitCells(line)

                If cells.Length < 2 Then
                    Continue For
                End If

                ' 跳过标题行
                If String.Equals(cells(0).Trim(), "pfamA_acc", StringComparison.OrdinalIgnoreCase) Then
                    Continue For
                End If

                Dim record As New iPfamRecord With {
                    .PfamA = cells(0).Trim(),
                    .PfamB = cells(1).Trim(),
                    .NameA = If(cells.Length > 2, cells(2).Trim(), ""),
                    .NameB = If(cells.Length > 3, cells(3).Trim(), ""),
                    .PdbId = If(cells.Length > 4, cells(4).Trim(), ""),
                    .Evidence = If(cells.Length > 6, cells(6).Trim(), "")
                }
                Dim score As Double = 0.0

                If cells.Length > 5 Then
                    Double.TryParse(cells(5).Trim(), score)
                End If

                record.Score = score

                If String.IsNullOrEmpty(record.PfamA) OrElse String.IsNullOrEmpty(record.PfamB) Then
                    Continue For
                End If

                Call result.Add(record)
            Next

            Return result.ToArray()
        End Function

        Private Shared Function SplitCells(line As String) As String()
            If line.IndexOf(vbTab) >= 0 Then
                Return line.Split(vbTab)
            Else
                Return line.Split(","c)
            End If
        End Function

        ''' <summary>
        ''' 按照结构域对去重，保留得分最高的那一条
        ''' </summary>
        Public Shared Function Distinct(records As IEnumerable(Of iPfamRecord)) As iPfamRecord()
            Return records _
                .Where(Function(r) r IsNot Nothing) _
                .GroupBy(Function(r) r.PairKey) _
                .Select(Function(g) g.OrderByDescending(Function(r) r.Score).First()) _
                .ToArray()
        End Function

        ''' <summary>
        ''' 按得分阈值过滤
        ''' </summary>
        Public Shared Function FilterByScore(records As IEnumerable(Of iPfamRecord), minScore As Double) As iPfamRecord()
            Return records.Where(Function(r) r.Score >= minScore).ToArray()
        End Function

        ''' <summary>
        ''' 导出为制表符分隔的文本
        ''' </summary>
        Public Shared Function ToTable(records As IEnumerable(Of iPfamRecord)) As String
            Dim sb As New StringBuilder

            Call sb.AppendLine("pfamA_acc" & vbTab & "pfamB_acc" & vbTab & "pfamA_id" & vbTab & "pfamB_id" & vbTab & "pdb_id" & vbTab & "score" & vbTab & "evidence")

            For Each record As iPfamRecord In records
                Call sb.AppendLine(String.Join(vbTab, {
                    If(record.PfamA, ""),
                    If(record.PfamB, ""),
                    If(record.NameA, ""),
                    If(record.NameB, ""),
                    If(record.PdbId, ""),
                    record.Score.ToString("0.###"),
                    If(record.Evidence, "")
                }))
            Next

            Return sb.ToString()
        End Function

        Public Overrides Function ToString() As String
            Return $"{PfamA} -- {PfamB} (pdb={PdbId}, score={Score:0.###})"
        End Function
    End Class
End Namespace
