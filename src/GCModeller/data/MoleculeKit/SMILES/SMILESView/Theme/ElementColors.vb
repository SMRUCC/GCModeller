Imports System.Drawing
Imports Mol = SMRUCC.genomics.Molecule.SMILES.Graph

Namespace Styling

    ''' <summary>
    ''' 元素配色（CPK/Jmol 方案）解析与缓存。
    ''' </summary>
    ''' <remarks>
    ''' CPK 颜色以十六进制字符串保存在 SMILES 库中（该库不引用 System.Drawing），
    ''' 视图层在这里统一转换并缓存，避免每次绘制都做字符串解析。
    ''' </remarks>
    Public Module ElementColors

        Private ReadOnly cache As New Dictionary(Of String, Color)(StringComparer.OrdinalIgnoreCase)

        ''' <summary>
        ''' 取元素的 CPK 配色。
        ''' </summary>
        ''' <param name="element">元素符号。</param>
        Public Function ColorOf(element As String) As Color
            If String.IsNullOrEmpty(element) Then Return Color.Gray

            Dim value As Color

            SyncLock cache
                If cache.TryGetValue(element, value) Then Return value
            End SyncLock

            value = parse(Mol.ElementTable.CpkColor(element))

            SyncLock cache
                cache(element) = value
            End SyncLock

            Return value
        End Function

        ''' <summary>
        ''' 把 <c>RRGGBB</c> / <c>#RRGGBB</c> / <c>AARRGGBB</c> 字符串解析为颜色。
        ''' </summary>
        ''' <param name="hex">十六进制颜色字符串。</param>
        ''' <param name="fallback">解析失败时的回退颜色。</param>
        Public Function ParseHex(hex As String, Optional fallback As Color = Nothing) As Color
            If String.IsNullOrWhiteSpace(hex) Then Return fallback

            Dim text As String = hex.Trim()

            If text.StartsWith("#"c) Then text = text.Substring(1)

            If text.Length = 3 Then
                text = New String({text(0), text(0), text(1), text(1), text(2), text(2)})
            End If

            Dim value As Integer

            If Not Integer.TryParse(text, Globalization.NumberStyles.HexNumber, Nothing, value) Then
                Return fallback
            End If

            If text.Length = 8 Then
                Return Color.FromArgb(value)
            End If

            Return Color.FromArgb(255, (value >> 16) And &HFF, (value >> 8) And &HFF, value And &HFF)
        End Function

        Private Function parse(hex As String) As Color
            Return ParseHex(hex, Color.Gray)
        End Function

    End Module

End Namespace
