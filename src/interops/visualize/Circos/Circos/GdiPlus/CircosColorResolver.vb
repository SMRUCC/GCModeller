Imports System.Drawing
Imports System.Globalization
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Visualize.Circos.Colors

Namespace GdiPlus

    ''' <summary>
    ''' 将 circos/perl 的颜色表达式解析为 .NET 的 <see cref="Color"/> 对象。
    ''' 
    ''' 现有的 <see cref="CircosColor.FromKnownColorName"/> 无法解析 circos 发行版之中的
    ''' 间接颜色定义（例如 ``vdgrey = greys-9-seq-7``）以及带透明度的颜色名
    ''' （例如 ``red_a2`` / ``black_a4``），会导致大量颜色退化为黑色，所以在这里提供一套
    ''' 完整的颜色解析器。
    ''' 
    ''' 支持的输入形式：
    ''' 
    ''' + 直接 RGB：``(r,g,b)`` 或者 ``r,g,b``
    ''' + 颜色名：``black`` / ``white`` / ``grey`` / ``chr1`` / ``hs1`` 等
    ''' + brewer 列表引用：``greys-9-seq-7`` / ``reds-7-seq-4`` 等
    ''' + 透明度后缀：``red_a2``(20%) / ``black_a4``(40%) / ``hs1_a5``(50%)
    ''' + hsv 表达式：``hsv(19,1,1)``
    ''' </summary>
    Public Module CircosColorResolver

        Private ReadOnly inlineComment As New Regex("#.*$")
        Private ReadOnly rgbLiteral As New Regex("^\(?\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*(\d{1,3})\s*\)?$")
        Private ReadOnly hsvLiteral As New Regex("^hsv\(\s*([\d\.]+)\s*,\s*([\d\.]+)\s*,\s*([\d\.]+)\s*\)$", RegexOptions.IgnoreCase)
        Private ReadOnly alphaSuffix As New Regex("^(?<name>.+)_a(?<a>\d{1,2})$", RegexOptions.IgnoreCase)

        ''' <summary>
        ''' ``颜色名(小写) = 原始定义值`` 的字典（由 circos 发行版的 colors*.txt 资源文件构建）
        ''' </summary>
        Private ReadOnly rawValues As Dictionary(Of String, String) = loadRawValues()
        Private ReadOnly cache As New Dictionary(Of String, Color)
        Private ReadOnly warns As New HashSet(Of String)

        ''' <summary>
        ''' 解析 circos 颜色表达式，无法解析的时候返回灰色并记录一次警告
        ''' </summary>
        Public Function Resolve(expr As String, Optional fallback As Color? = Nothing) As Color
            Dim text$ = If(expr, "").Trim()

            If String.IsNullOrEmpty(text) Then
                If fallback.HasValue Then
                    Return fallback.Value
                Else
                    Return Color.Gray
                End If
            End If

            SyncLock cache
                Dim cached As Color

                If cache.TryGetValue(text, cached) Then
                    Return cached
                End If
            End SyncLock

            Dim color As Color = resolveInternal(text, 0, fallback)

            SyncLock cache
                cache(text) = color
            End SyncLock

            Return color
        End Function

        Private Function resolveInternal(expr As String, depth As Integer, fallback As Color?) As Color
            Dim text$ = expr.Trim()

            ' 1. 直接 RGB 值：(r,g,b) 或者 r,g,b
            Dim m As Match = rgbLiteral.Match(text)
            If m.Success Then
                Return Color.FromArgb(
                    CInt(Val(m.Groups(1).Value)),
                    CInt(Val(m.Groups(2).Value)),
                    CInt(Val(m.Groups(3).Value)))
            End If

            ' 2. hsv 表达式
            m = hsvLiteral.Match(text)
            If m.Success Then
                Return CircosColor.ColorFromHSV(
                    CDbl(Val(m.Groups(1).Value)),
                    CDbl(Val(m.Groups(2).Value)),
                    CDbl(Val(m.Groups(3).Value)))
            End If

            ' 3. 透明度后缀：red_a2 / black_a4 / hs1_a5
            m = alphaSuffix.Match(text)
            If m.Success Then
                Dim baseColor = resolveInternal(m.Groups("name").Value, depth + 1, Nothing)

                If baseColor <> Color.Empty Then
                    Dim alpha As Integer = alphaValue(m.Groups("a").Value)
                    Return Color.FromArgb(alpha, baseColor)
                End If
            End If

            ' 4. 颜色名（含间接引用）
            If depth < 16 Then
                Dim key$ = text.ToLowerInvariant()
                Dim raw As String

                If rawValues.TryGetValue(key, raw) Then
                    If Not String.IsNullOrEmpty(raw) AndAlso Not String.Equals(raw.Trim(), text, StringComparison.OrdinalIgnoreCase) Then
                        Dim resolved = resolveInternal(raw, depth + 1, Nothing)

                        If resolved <> Color.Empty Then
                            Return resolved
                        End If
                    End If
                End If
            End If

            ' 5. .NET 已知颜色名
            Dim known = Color.FromName(text)
            If known.IsKnownColor Then
                Return known
            End If

            ' 6. 兜底
            SyncLock warns
                If warns.Add(text) Then
                    Call $"Unknown circos color literal '{text}', fallback to gray.".warning
                End If
            End SyncLock

            If fallback.HasValue Then
                Return fallback.Value
            Else
                Return Color.Gray
            End If
        End Function

        ''' <summary>
        ''' ``_aN`` 后缀的透明度换算：1 位数字表示 10% 的倍数（``_a4`` = 40%），
        ''' 2 位数字表示百分比（``_a40`` = 40%）
        ''' </summary>
        Private Function alphaValue(code As String) As Integer
            Dim n As Integer = CInt(Val(code))
            Dim percent As Integer = If(code.Length <= 1, n * 10, n)

            If percent < 0 Then percent = 0
            If percent > 100 Then percent = 100

            Return CInt(Math.Round(percent / 100.0 * 255))
        End Function

        ''' <summary>
        ''' 从 circos 发行版的 colors*.txt 资源文件之中加载 ``名称 = 原始定义值`` 的映射表
        ''' </summary>
        Private Function loadRawValues() As Dictionary(Of String, String)
            Dim dict As New Dictionary(Of String, String)
            Dim files As String() = {
                My.Resources.ColorSet.colors,
                My.Resources.ColorSet.colors_brewer,
                My.Resources.ColorSet.colors_brewer_lists,
                My.Resources.ColorSet.colors_ucsc,
                My.Resources.ColorSet.colors_unix,
                My.Resources.ColorSet.colors_hsv
            }

            For Each text As String In files
                If String.IsNullOrEmpty(text) Then
                    Continue For
                End If

                For Each line As String In text.Replace(vbCrLf, vbLf).Split(CChar(vbLf))
                    Dim s$ = line.Trim()

                    If s.Length = 0 OrElse s.StartsWith("#") Then
                        Continue For
                    End If

                    Dim idx As Integer = s.IndexOf("="c)

                    If idx <= 0 Then
                        Continue For
                    End If

                    Dim name$ = s.Substring(0, idx).Trim().ToLowerInvariant()
                    Dim value$ = inlineComment.Replace(s.Substring(idx + 1), "").Trim()

                    If name.Length = 0 OrElse value.Length = 0 Then
                        Continue For
                    End If

                    dict(name) = value
                Next
            Next

            Return dict
        End Function
    End Module
End Namespace
