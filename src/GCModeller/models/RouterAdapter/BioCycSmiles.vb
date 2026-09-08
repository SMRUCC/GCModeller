' ============================================================================
' BioCycSmiles.vb — BioCyc SMILES 净化与安全解析
' ----------------------------------------------------------------------------
' RetroPath 的 SmilesIO 仅支持：元素/支链/环闭合数字/键符 - = # / 电价 [NH3+] [O-]
'   / 显式氢 / 多组分 "."；不支持芳香小写、@/@@ 立体标记、/ \ 的 E/Z 标记、%nn 环号
'   与通配 *，元素表仅含 Cl Br Si C N O S P F I B H。
' BioCyc 的 SMILES 为 Kekulé 大写式（无芳香小写）但普遍带立体标记
'   （[C@@H]、C(\C=C/C(\C=1)=2)），且部分条目含金属（如 PROTOHEME 的 [Fe-2]），
'   因此必须先净化/过滤再交给 SmilesIO，否则 Parse 会直接抛异常。
' 所有 API 均为无状态纯函数且内部 Try/Catch——单条脏数据只跳过，不影响整库装配。
' ============================================================================

Imports System.Text
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Analysis.RetroPath.Chem

Public Module BioCycSmiles

    ''' <summary>
    ''' RetroPath 分子模型所支持的元素集合（超出该集合的化合物一律跳过）
    ''' </summary>
    Private ReadOnly supportedElements As New HashSet(Of String)(
        New String() {"C", "N", "O", "S", "P", "F", "I", "B", "H", "Cl", "Br", "Si"},
        StringComparer.Ordinal)

    ''' <summary>该元素是否可被 RetroPath 的分子模型表达</summary>
    Public Function IsSupportedElement(el As String) As Boolean
        Return el IsNot Nothing AndAlso supportedElements.Contains(el)
    End Function

    ''' <summary>
    ''' 剔除立体/构型标记（/ \ @），返回可交给 SmilesIO.Parse 的 SMILES。
    ''' 含 %nn 环号或通配 * 的串无法净化，返回 Nothing。
    ''' </summary>
    Public Function Sanitize(raw As String, Optional ByRef reason As String = Nothing) As String
        reason = Nothing

        If raw Is Nothing Then Return Nothing

        Dim s As String = raw.Trim()
        If s.Length = 0 Then Return Nothing

        If s.IndexOf("%"c) >= 0 Then
            reason = "ring-number-%nn"
            Return Nothing
        End If
        If s.IndexOf("*"c) >= 0 Then
            reason = "wildcard-*"
            Return Nothing
        End If

        Dim sb As New StringBuilder(s.Length)

        For Each c As Char In s
            If c = "/"c OrElse c = "\"c OrElse c = "@"c Then
                ' 立体化学信息不建模（README 已知边界），直接丢弃
                Continue For
            End If
            sb.Append(c)
        Next

        Dim out As String = sb.ToString()

        ' [C@] -> [C] 之类的净化残留：空括号原子已无意义
        If out.IndexOf("[]") >= 0 Then
            reason = "empty-bracket-atom"
            Return Nothing
        End If

        ' 括号原子必须整体合法。
        ' 这一关不能省：SmilesIO 的括号解析只做前缀匹配，遇到 [Cr+3] 会把 "C" 当成元素、
        ' 后面的 "r+3" 直接忽略，于是铬离子被静默解析成一个"碳原子"——它随后会作为
        ' 单碳碎片进入汇集合，让逆推时掉下来的碳碎片被误判为"已内源"，从而拼出伪通路。
        For Each m As Match In BracketAtom.Matches(out)
            If Not IsSupportedBracketAtom(m.Groups(1).Value) Then
                ' 金属离子、R 基团、[a protein] 之类的泛型类条目
                reason = "unsupported-atom[" & m.Groups(1).Value & "]"
                Return Nothing
            End If
        Next

        Return out
    End Function

    ''' <summary>匹配 SMILES 里的括号原子 [...]</summary>
    Private ReadOnly BracketAtom As New Regex("\[([^]]*)\]", RegexOptions.Compiled)

    ''' <summary>合法括号原子的模式：白名单元素 + 可选显式氢 + 可选电荷</summary>
    Private ReadOnly AtomBody As New Regex(
        "^(Cl|Br|Si|C|N|O|S|P|F|I|B|H)(H[0-9]*)?([+-][0-9]*)?$", RegexOptions.Compiled)

    ''' <summary>
    ''' 括号原子内容是否可被 RetroPath 的分子模型正确表达。
    ''' 必须整体匹配（而非前缀匹配），否则 [Cr+3] / [Fe-2] / [a protein] 会被误读。
    ''' </summary>
    Private Function IsSupportedBracketAtom(body As String) As Boolean
        Return AtomBody.IsMatch(body)
    End Function

    ''' <summary>净化 + 解析 + 原子数上限 + 元素白名单；任一环节失败返回 False</summary>
    Public Function TryParse(raw As String, ByRef mol As Molecule,
                             Optional maxAtoms As Integer = 80,
                             Optional ByRef reason As String = Nothing) As Boolean
        Dim smiles As String = Nothing
        Return TryParse(raw, smiles, mol, maxAtoms, reason)
    End Function

    ''' <summary>
    ''' 净化 + 解析 + 原子数上限 + 元素白名单；任一环节失败返回 False。
    ''' <paramref name="smiles"/> 返回净化后的 SMILES（可直接用于汇集合/写出）；
    ''' <paramref name="reason"/> 失败原因（用于装配诊断统计）。
    ''' </summary>
    Public Function TryParse(raw As String, ByRef smiles As String, ByRef mol As Molecule,
                             Optional maxAtoms As Integer = 80,
                             Optional ByRef reason As String = Nothing) As Boolean
        smiles = Nothing
        mol = Nothing
        reason = Nothing

        Dim s As String = Sanitize(raw, reason)
        If s Is Nothing Then
            If reason Is Nothing Then reason = "not-sanitizable"
            Return False
        End If

        Dim m As Molecule = Nothing

        Try
            m = SmilesIO.Parse(s)
        Catch ex As Exception
            reason = "syntax[" & ex.Message.Split(":"c)(0).Trim() & "]"
            Return False
        End Try

        If m Is Nothing OrElse m.NumAtoms() = 0 Then
            reason = "empty"
            Return False
        End If
        If maxAtoms > 0 AndAlso m.NumAtoms() > maxAtoms Then
            reason = "too-many-atoms"
            Return False
        End If

        For Each el As String In m.Elements
            If Not supportedElements.Contains(el) Then
                reason = "element:" & el
                Return False
            End If
        Next

        smiles = s
        mol = m
        Return True
    End Function

End Module
