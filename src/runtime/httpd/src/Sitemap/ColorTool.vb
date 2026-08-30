Imports System
Imports System.Globalization
Imports System.Text.RegularExpressions

''' <summary>
''' A tiny css color expression parser and color arithmetic helper module
''' for the website theme extraction.
''' </summary>
Public Module ColorTool

    ''' <summary>
    ''' the css color value that can not be parsed as a valid rgb color
    ''' </summary>
    Public ReadOnly InvalidColor As String = Nothing

    ''' <summary>
    ''' the value that means the css color expression is not a valid color
    ''' </summary>
    ReadOnly invalidValues As New HashSet(Of String) From {
        "transparent", "inherit", "initial", "unset", "currentcolor",
        "none", "auto", "revert", "!important"
    }

    ReadOnly namedColors As New Dictionary(Of String, String) From {
        {"black", "#000000"}, {"white", "#ffffff"}, {"red", "#ff0000"},
        {"green", "#008000"}, {"blue", "#0000ff"}, {"gray", "#808080"},
        {"grey", "#808080"}, {"silver", "#c0c0c0"}, {"maroon", "#800000"},
        {"olive", "#808000"}, {"lime", "#00ff00"}, {"aqua", "#00ffff"},
        {"teal", "#008080"}, {"navy", "#000080"}, {"fuchsia", "#ff00ff"},
        {"purple", "#800080"}, {"orange", "#ffa500"}, {"yellow", "#ffff00"},
        {"cyan", "#00ffff"}, {"magenta", "#ff00ff"}, {"gold", "#ffd700"},
        {"indigo", "#4b0082"}, {"violet", "#ee82ee"}, {"pink", "#ffc0cb"},
        {"crimson", "#dc143c"}, {"tomato", "#ff6347"}, {"salmon", "#fa8072"},
        {"coral", "#ff7f50"}, {"steelblue", "#4682b4"}, {"darkblue", "#00008b"},
        {"darkred", "#8b0000"}, {"darkgreen", "#006400"}, {"darkgray", "#a9a9a9"},
        {"darkgrey", "#a9a9a9"}, {"lightgray", "#d3d3d3"}, {"lightgrey", "#d3d3d3"},
        {"whitesmoke", "#f5f5f5"}, {"gainsboro", "#dcdcdc"}, {"dimgray", "#696969"},
        {"dimgrey", "#696969"}, {"slategray", "#708090"}, {"slategrey", "#708090"},
        {"lightslategray", "#778899"}, {"midnightblue", "#191970"},
        {"royalblue", "#4169e1"}, {"dodgerblue", "#1e90ff"}, {"deepskyblue", "#00bfff"},
        {"skyblue", "#87ceeb"}, {"lightblue", "#add8e6"}, {"seagreen", "#2e8b57"},
        {"forestgreen", "#228b22"}, {"darkorange", "#ff8c00"}, {"orangered", "#ff4500"}
    }

    ''' <summary>
    ''' test the given css color expression string is a valid color value or not
    ''' </summary>
    ''' <param name="expression"></param>
    ''' <returns></returns>
    Public Function IsValidColor(expression As String) As Boolean
        Return Not ParseColor(expression) Is Nothing
    End Function

    ''' <summary>
    ''' the css color literal pattern: the hex color, the rgb/rgba function
    ''' and the hsl/hsla function
    ''' </summary>
    ReadOnly colorLiteral As New Regex(
        "#(?:[0-9a-f]{8}|[0-9a-f]{6}|[0-9a-f]{3})\b|rgba?\([^)]*\)|hsla?\([^)]*\)",
        RegexOptions.IgnoreCase)

    ''' <summary>
    ''' find the first valid css color literal from a complex css value,
    ''' example as the shorthand property ``background: #fff url(bg.png)``
    ''' </summary>
    ''' <param name="expression"></param>
    ''' <param name="overBackground"></param>
    ''' <returns></returns>
    Public Function FindColor(expression As String, Optional overBackground As String = Nothing) As String
        Dim direct As String = ParseColor(expression, overBackground)

        If Not direct Is Nothing Then
            Return direct
        End If

        If expression Is Nothing Then
            Return InvalidColor
        End If

        For Each m As Match In colorLiteral.Matches(expression)
            Dim color As String = ParseColor(m.Value, overBackground)

            If Not color Is Nothing Then
                Return color
            End If
        Next

        Return InvalidColor
    End Function

    ''' <summary>
    ''' parse a css color expression as the rgb color integer bits, the
    ''' <see cref="InvalidColor"/> (Nothing) will be returns if the given
    ''' expression is not a valid css color value.
    ''' </summary>
    ''' <param name="expression">
    ''' the supported color expression format includes:
    '''
    ''' + the hex color: ``#abc``, ``#aabbcc`` and ``#aabbccdd``
    ''' + the function color: ``rgb(r,g,b)``, ``rgba(r,g,b,a)`` and ``hsl(h,s%,l%)``
    ''' + the well known color names, example as ``white``
    '''
    ''' the ``var(...)`` css function color value is not a valid color
    ''' value, you should resolves the css variable at first.
    ''' </param>
    ''' <param name="overBackground">
    ''' the solid background color that is used for compositing the
    ''' semi-transparent color value, if this parameter is not specified
    ''' then the black or the white color will be used based on the alpha
    ''' channel value.
    ''' </param>
    ''' <returns>
    ''' this function returns a color value in ``#rrggbb`` hex text format
    ''' </returns>
    Public Function ParseColor(expression As String, Optional overBackground As String = Nothing) As String
        If expression Is Nothing Then
            Return InvalidColor
        End If

        Dim value As String = expression.Trim.ToLower

        ' rgba(255, 59, 47, 0.12) may be trimmed by the css parser
        value = value.Trim(""""c, "'"c, ";"c)

        If value.Length = 0 OrElse invalidValues.Contains(value) Then
            Return InvalidColor
        End If

        If value.StartsWith("var(") Then
            Return InvalidColor
        End If

        If value.First = "#"c Then
            Return parseHexColor(value)
        ElseIf value.StartsWith("rgb") Then
            Return parseRgbFunction(value, overBackground)
        ElseIf value.StartsWith("hsl") Then
            Return parseHslFunction(value)
        ElseIf namedColors.ContainsKey(value) Then
            Return namedColors(value)
        ElseIf Regex.IsMatch(value, "^[0-9a-f]{6}$") Then
            Return "#" & value
        ElseIf Regex.IsMatch(value, "^[0-9a-f]{3}$") Then
            Return parseHexColor("#" & value)
        Else
            Return InvalidColor
        End If
    End Function

    Private Function parseHexColor(hex As String) As String
        Dim code As String = hex.Substring(1)

        If Regex.IsMatch(code, "^[0-9a-f]{3}$") Then
            Return "#" & New String(code(0), 2) & New String(code(1), 2) & New String(code(2), 2)
        ElseIf Regex.IsMatch(code, "^[0-9a-f]{6}$") Then
            Return "#" & code
        ElseIf Regex.IsMatch(code, "^[0-9a-f]{8}$") Then
            ' #rrggbbaa, drop the alpha channel
            Return "#" & code.Substring(0, 6)
        Else
            Return InvalidColor
        End If
    End Function

    Private Function parseRgbFunction(value As String, Optional overBackground As String = Nothing) As String
        Dim args As String() = functionArgs(value)

        If args Is Nothing OrElse args.Length < 3 Then
            Return InvalidColor
        End If

        Dim r As Integer = channelValue(args(0))
        Dim g As Integer = channelValue(args(1))
        Dim b As Integer = channelValue(args(2))
        Dim a As Double = 1.0

        If args.Length > 3 Then
            Double.TryParse(args(3), NumberStyles.Any, CultureInfo.InvariantCulture, a)
        End If

        If a < 0.9 Then
            ' composite the semi-transparent color on the given background,
            ' or on the white/black background based on the alpha value
            Dim back As String = If(overBackground Is Nothing, If(a < 0.5, "#000000", "#ffffff"), overBackground)
            Return Composite(back, ToHex(r, g, b), a)
        End If

        Return ToHex(r, g, b)
    End Function

    Private Function parseHslFunction(value As String) As String
        Dim args As String() = functionArgs(value)

        If args Is Nothing OrElse args.Length < 3 Then
            Return InvalidColor
        End If

        Dim h As Double = num(args(0))
        Dim s As Double = num(args(1)) / 100.0
        Dim l As Double = num(args(2)) / 100.0

        Return ToHex(HSLtoRGB(h, s, l))
    End Function

    Private Function functionArgs(value As String) As String()
        Dim open As Integer = value.IndexOf("("c)
        Dim close As Integer = value.LastIndexOf(")"c)

        If open < 0 OrElse close <= open Then
            Return Nothing
        End If

        Dim body As String = value.Substring(open + 1, close - open - 1)

        Return body _
            .Replace("/"c, ","c) _
            .Split(","c) _
            .Select(Function(s) s.Trim) _
            .Where(Function(s) s.Length > 0) _
            .ToArray
    End Function

    Private Function num(text As String) As Double
        Dim x As Double = 0

        Double.TryParse(
            text.Trim("%"c, " "c),
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            x)

        Return x
    End Function

    Private Function channelValue(text As String) As Integer
        Dim value As String = text.Trim

        If value.EndsWith("%") Then
            Return CInt(Math.Round(255 * num(value) / 100.0))
        End If

        Return CInt(Math.Round(num(value)))
    End Function

    ''' <summary>
    ''' composite a semi-transparent foreground color on a solid background color
    ''' </summary>
    ''' <param name="background">the solid background color in hex format</param>
    ''' <param name="foreground">the foreground color in hex format</param>
    ''' <param name="alpha">the alpha channel value in range [0, 1]</param>
    ''' <returns></returns>
    Public Function Composite(background As String, foreground As String, alpha As Double) As String
        Dim bg As (r As Integer, g As Integer, b As Integer) = ToRGB(background)
        Dim fg As (r As Integer, g As Integer, b As Integer) = ToRGB(foreground)

        If alpha < 0 Then
            alpha = 0
        ElseIf alpha > 1 Then
            alpha = 1
        End If

        Return ToHex(
            CInt(Math.Round(bg.r * (1 - alpha) + fg.r * alpha)),
            CInt(Math.Round(bg.g * (1 - alpha) + fg.g * alpha)),
            CInt(Math.Round(bg.b * (1 - alpha) + fg.b * alpha)))
    End Function

    ''' <summary>
    ''' mix two color by a given weight of the <paramref name="foreground"/> color
    ''' </summary>
    ''' <param name="background"></param>
    ''' <param name="foreground"></param>
    ''' <param name="weight">
    ''' a value in range [0, 1], 0 means fully the background color and
    ''' 1 means fully the foreground color.
    ''' </param>
    ''' <returns></returns>
    Public Function Mix(background As String, foreground As String, weight As Double) As String
        Return Composite(background, foreground, weight)
    End Function

    ''' <summary>
    ''' make a lighter color of the given color
    ''' </summary>
    Public Function Lighten(color As String, weight As Double) As String
        Return Mix(color, "#ffffff", weight)
    End Function

    ''' <summary>
    ''' make a darker color of the given color
    ''' </summary>
    Public Function Darken(color As String, weight As Double) As String
        Return Mix(color, "#000000", weight)
    End Function

    ''' <summary>
    ''' parse the hex color string as the rgb channel tuple
    ''' </summary>
    ''' <param name="hex"></param>
    ''' <returns></returns>
    Public Function ToRGB(hex As String) As (r As Integer, g As Integer, b As Integer)
        Dim color As String = ParseColor(hex)

        If color Is Nothing Then
            color = "#000000"
        End If

        color = color.Substring(1)

        Return (
            Convert.ToInt32(color.Substring(0, 2), 16),
            Convert.ToInt32(color.Substring(2, 2), 16),
            Convert.ToInt32(color.Substring(4, 2), 16))
    End Function

    ''' <summary>
    ''' convert the rgb channel values as the hex color string
    ''' </summary>
    Public Function ToHex(r As Integer, g As Integer, b As Integer) As String
        Return "#" & clamp(r).ToString("x2") & clamp(g).ToString("x2") & clamp(b).ToString("x2")
    End Function

    ''' <summary>
    ''' convert the rgb channel tuple as the hex color string
    ''' </summary>
    Public Function ToHex(rgb As (r As Integer, g As Integer, b As Integer)) As String
        Return ToHex(rgb.r, rgb.g, rgb.b)
    End Function

    Public Function HSLtoRGB(h As Double, s As Double, l As Double) As (r As Integer, g As Integer, b As Integer)
        Dim c As Double = (1 - Math.Abs(2 * l - 1)) * s
        Dim hp As Double = ((h Mod 360) + 360) Mod 360 / 60.0
        Dim x As Double = c * (1 - Math.Abs((hp Mod 2) - 1))
        Dim r As Double = 0, g As Double = 0, b As Double = 0

        If hp < 1 Then
            r = c : g = x
        ElseIf hp < 2 Then
            r = x : g = c
        ElseIf hp < 3 Then
            g = c : b = x
        ElseIf hp < 4 Then
            g = x : b = c
        ElseIf hp < 5 Then
            r = x : b = c
        Else
            r = c : b = x
        End If

        Dim m As Double = l - c / 2

        Return (clamp(CInt(Math.Round((r + m) * 255))),
                clamp(CInt(Math.Round((g + m) * 255))),
                clamp(CInt(Math.Round((b + m) * 255))))
    End Function

    ''' <summary>
    ''' convert the rgb color as the hsl color space
    ''' </summary>
    Public Function ToHSL(hex As String) As (h As Double, s As Double, l As Double)
        Dim rgb As (r As Integer, g As Integer, b As Integer) = ToRGB(hex)
        Dim r As Double = rgb.r / 255.0
        Dim g As Double = rgb.g / 255.0
        Dim b As Double = rgb.b / 255.0
        Dim max As Double = Math.Max(r, Math.Max(g, b))
        Dim min As Double = Math.Min(r, Math.Min(g, b))
        Dim l As Double = (max + min) / 2
        Dim h As Double = 0
        Dim s As Double = 0
        Dim d As Double = max - min

        If d > 0 Then
            s = If(l > 0.5, d / (2 - max - min), d / (max + min))

            If max = r Then
                h = ((g - b) / d + If(g < b, 6, 0))
            ElseIf max = g Then
                h = (b - r) / d + 2
            Else
                h = (r - g) / d + 4
            End If

            h *= 60
        End If

        Return (h, s, l)
    End Function

    ''' <summary>
    ''' the perceived brightness of the given color, a value in range [0,1]
    ''' </summary>
    Public Function Luminance(hex As String) As Double
        Dim rgb As (r As Integer, g As Integer, b As Integer) = ToRGB(hex)

        Return (0.2126 * rgb.r + 0.7152 * rgb.g + 0.0722 * rgb.b) / 255.0
    End Function

    ''' <summary>
    ''' the hsl saturation of the given color, a value in range [0,1]
    ''' </summary>
    Public Function Saturation(hex As String) As Double
        Return ToHSL(hex).s
    End Function

    ''' <summary>
    ''' true if the given color is a light color, so that the text color
    ''' on top of it should be a dark color.
    ''' </summary>
    Public Function IsLightColor(hex As String) As Boolean
        Return Luminance(hex) > 0.5
    End Function

    ''' <summary>
    ''' get a readable foreground text color for the given background color
    ''' </summary>
    Public Function ReadableTextOn(hex As String) As String
        Return If(IsLightColor(hex), "#1a1a1a", "#ffffff")
    End Function

    ''' <summary>
    ''' is the given color a nearly white or nearly black color?
    ''' </summary>
    Public Function IsNeutral(hex As String) As Boolean
        If hex Is Nothing Then
            Return True
        End If

        Return Saturation(hex) < 0.12
    End Function

    Private Function clamp(x As Integer) As Integer
        If x < 0 Then
            Return 0
        ElseIf x > 255 Then
            Return 255
        Else
            Return x
        End If
    End Function
End Module
