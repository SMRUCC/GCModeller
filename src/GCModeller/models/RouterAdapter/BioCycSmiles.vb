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
    Public Function Sanitize(raw As String) As String
        If raw Is Nothing Then Return Nothing

        Dim s As String = raw.Trim()
        If s.Length = 0 Then Return Nothing
        ' 多位环闭合 %10 与通配原子 * 均无法表达
        If s.IndexOf("%"c) >= 0 OrElse s.IndexOf("*"c) >= 0 Then Return Nothing

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
        If out.IndexOf("[]") >= 0 Then Return Nothing

        Return out
    End Function

    ''' <summary>净化 + 解析 + 原子数上限 + 元素白名单；任一环节失败返回 False</summary>
    Public Function TryParse(raw As String, ByRef mol As Molecule,
                             Optional maxAtoms As Integer = 80) As Boolean
        Dim smiles As String = Nothing
        Return TryParse(raw, smiles, mol, maxAtoms)
    End Function

    ''' <summary>
    ''' 净化 + 解析 + 原子数上限 + 元素白名单；任一环节失败返回 False。
    ''' <paramref name="smiles"/> 返回净化后的 SMILES（可直接用于汇集合/写出）。
    ''' </summary>
    Public Function TryParse(raw As String, ByRef smiles As String, ByRef mol As Molecule,
                             Optional maxAtoms As Integer = 80) As Boolean
        smiles = Nothing
        mol = Nothing

        Dim s As String = Sanitize(raw)
        If s Is Nothing Then Return False

        Try
            Dim m As Molecule = SmilesIO.Parse(s)

            If m Is Nothing OrElse m.NumAtoms() = 0 Then Return False
            If maxAtoms > 0 AndAlso m.NumAtoms() > maxAtoms Then Return False

            For Each el As String In m.Elements
                If Not supportedElements.Contains(el) Then Return False
            Next

            smiles = s
            mol = m
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

End Module
