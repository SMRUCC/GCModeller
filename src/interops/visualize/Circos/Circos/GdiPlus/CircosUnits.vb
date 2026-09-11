Imports System.Globalization
Imports System.Text.RegularExpressions

Namespace GdiPlus

    ''' <summary>
    ''' circos 配置之中 ``r`` / ``p`` / ``u`` 等尺寸单位的解析与换算工具。
    ''' 
    ''' + ``r``：相对于图像半径的比例值（例如 ``0.85r``）
    ''' + ``p``：绝对像素值（例如 ``25p``）
    ''' + ``u``：相对于 ``chromosomes_units`` 的基因组坐标单位（例如 ``1u``）
    ''' + ``dims(...)``：引用图像/ideogram 的尺寸表达式（例如 ``dims(ideogram,radius) + 0.05r``）
    ''' </summary>
    Public Module CircosUnits

        Private ReadOnly dimsPattern As New Regex("dims\s*\(\s*[^\)]*\)", RegexOptions.IgnoreCase)

#Region "布尔值"

        ''' <summary>
        ''' circos 的 ``yes`` / ``no`` 开关解析
        ''' </summary>
        Public Function IsYes(value As String) As Boolean
            Dim v$ = If(value, "").Trim().ToLowerInvariant()

            Select Case v
                Case "yes", "y", "true", "1"
                    Return True
                Case Else
                    Return False
            End Select
        End Function

#End Region

#Region "数值"

        ''' <summary>
        ''' 去除单位后缀之后的数值解析（使用 InvariantCulture）
        ''' </summary>
        Public Function ParseNumber(text As String, Optional fallback As Double = 0) As Double
            Dim v$ = If(text, "").Trim()

            If v.Length = 0 Then
                Return fallback
            End If

            Dim d As Double

            If Double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, d) Then
                Return d
            End If

            Return fallback
        End Function

        ''' <summary>
        ''' 解析带有 ``r`` / ``p`` 后缀的长度表达式，返回像素值
        ''' </summary>
        ''' <param name="expr">长度表达式</param>
        ''' <param name="imageRadius">图像半径（像素），用于换算 ``r`` 单位</param>
        ''' <param name="fallback">解析失败时返回的默认值</param>
        Public Function ParseLength(expr As String, imageRadius As Double, Optional fallback As Double = 0) As Double
            Dim s$ = If(expr, "").Trim()

            If s.Length = 0 Then
                Return fallback
            End If

            If s.EndsWith("r", StringComparison.OrdinalIgnoreCase) Then
                Return ParseNumber(s.Substring(0, s.Length - 1), fallback) * imageRadius
            End If

            If s.EndsWith("p", StringComparison.OrdinalIgnoreCase) Then
                Return ParseNumber(s.Substring(0, s.Length - 1), fallback)
            End If

            Return ParseNumber(s, fallback)
        End Function

        ''' <summary>
        ''' 解析可能包含 ``dims(...)`` 表达式的半径设置，返回像素值。
        ''' 
        ''' GDI+ 引擎之中将 ``dims(ideogram,radius_outer)`` 近似处理为图像半径，
        ''' ``dims(image,radius)`` 同样处理为图像半径。
        ''' </summary>
        Public Function ParseRadius(expr As String,
                                    imageRadius As Double,
                                    Optional fallback As Double = 0,
                                    Optional resolveDims As Func(Of String, Double) = Nothing) As Double

            Dim s$ = If(expr, "").Trim()

            If s.Length = 0 Then
                Return fallback
            End If

            Dim m As Match = dimsPattern.Match(s)

            If m.Success Then
                Dim rest$ = s.Substring(m.Index + m.Length).Trim()
                Dim baseValue As Double

                If resolveDims IsNot Nothing Then
                    baseValue = resolveDims(m.Value)

                    If baseValue <= 0 Then
                        baseValue = imageRadius
                    End If
                Else
                    baseValue = imageRadius
                End If

                Return baseValue + parseOffset(rest, imageRadius)
            End If

            Return ParseLength(s, imageRadius, fallback)
        End Function

        Private Function parseOffset(rest As String, imageRadius As Double) As Double
            Dim s$ = If(rest, "").Trim()

            If s.Length = 0 Then
                Return 0
            End If

            Dim sign As Double = 1

            If s.StartsWith("-") Then
                sign = -1
                s = s.Substring(1).Trim()
            ElseIf s.StartsWith("+") Then
                s = s.Substring(1).Trim()
            End If

            Return sign * ParseLength(s, imageRadius, 0)
        End Function

        ''' <summary>
        ''' 将 circos 的角度间距表达式（``1u`` / ``0.005r`` / 数值）换算为角度值（度）
        ''' </summary>
        ''' <param name="expr">间距表达式</param>
        ''' <param name="genomeSize">基因组的跨度（nt）</param>
        ''' <param name="chromosomesUnits">``chromosomes_units`` 的值</param>
        Public Function ParseSpacingAngle(expr As String, genomeSize As Double, chromosomesUnits As Double) As Double
            Dim s$ = If(expr, "").Trim()

            If s.Length = 0 OrElse genomeSize <= 0 Then
                Return 0
            End If

            If s.EndsWith("r", StringComparison.OrdinalIgnoreCase) Then
                Return 360.0 * ParseNumber(s.Substring(0, s.Length - 1), 0)
            End If

            Dim nt As Double

            If s.EndsWith("u", StringComparison.OrdinalIgnoreCase) Then
                Dim units As Double = ParseNumber(s.Substring(0, s.Length - 1), 0)

                nt = units * If(chromosomesUnits <= 0, 1, chromosomesUnits)
            Else
                nt = ParseNumber(s, 0)
            End If

            Return 360.0 * nt / genomeSize
        End Function

        ''' <summary>
        ''' 将占位数值（[0,1] 范围的普通数字）解析为比例值
        ''' </summary>
        Public Function ParseFraction(expr As String, Optional fallback As Double = 0) As Double
            Dim d As Double = ParseNumber(expr, fallback)

            If d < 0 Then d = 0
            If d > 1 Then d = 1

            Return d
        End Function

#End Region

#Region "sprintf 风格的格式化"

        ''' <summary>
        ''' 模拟 perl 的 ``sprintf(format, position * multiplier)`` 语义，
        ''' 用于生成刻度标签（``%d`` / ``%f`` / ``%.1f`` / ``%.2f`` / ``%s``）
        ''' </summary>
        Public Function FormatTick(value As Double, format As String) As String
            Dim fmt$ = If(format, "").Trim()

            If fmt.Length = 0 Then
                fmt = "%s"
            End If

            If fmt.StartsWith("%.") AndAlso fmt.EndsWith("f", StringComparison.OrdinalIgnoreCase) Then
                Dim digitsStr$ = fmt.Substring(2, fmt.Length - 3)
                Dim digits As Integer = CInt(Val(digitsStr))

                Return value.ToString("F" & digits, CultureInfo.InvariantCulture)
            End If

            Select Case fmt.ToLowerInvariant()
                Case "%d", "%i", "%u"
                    Return CInt(Math.Round(value)).ToString(CultureInfo.InvariantCulture)
                Case "%f"
                    Return value.ToString("F6", CultureInfo.InvariantCulture)
                Case "%e"
                    Return value.ToString("E", CultureInfo.InvariantCulture)
                Case "%g"
                    Return value.ToString("G", CultureInfo.InvariantCulture)
                Case Else
                    Return CInt(Math.Round(value)).ToString(CultureInfo.InvariantCulture)
            End Select
        End Function

#End Region

    End Module
End Namespace
