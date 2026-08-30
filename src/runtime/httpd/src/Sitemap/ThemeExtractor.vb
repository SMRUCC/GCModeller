Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine

''' <summary>
''' The website theme model that is extracted from the css style of the
''' target website, all of the visual style of the generated sitemap.xsl
''' file is driven by the property values in this theme model.
''' </summary>
Public Class SiteTheme

    ''' <summary>
    ''' the page background color
    ''' </summary>
    Public Property Background As String
    ''' <summary>
    ''' the card or the table header background color
    ''' </summary>
    Public Property Surface As String
    ''' <summary>
    ''' the theme accent color of the target website
    ''' </summary>
    Public Property Primary As String
    ''' <summary>
    ''' the readable text color that is rendered on top of the
    ''' <see cref="Primary"/> color.
    ''' </summary>
    Public Property OnPrimary As String
    ''' <summary>
    ''' the body text color
    ''' </summary>
    Public Property TextColor As String
    ''' <summary>
    ''' the secondary description text color
    ''' </summary>
    Public Property MutedText As String
    ''' <summary>
    ''' the hyperlink text color
    ''' </summary>
    Public Property LinkColor As String
    ''' <summary>
    ''' the table border and the splitter line color
    ''' </summary>
    Public Property BorderColor As String
    ''' <summary>
    ''' the zebra stripe background color of the table row
    ''' </summary>
    Public Property RowAlt As String
    ''' <summary>
    ''' the font family stack of the website
    ''' </summary>
    Public Property FontFamily As String
    ''' <summary>
    ''' the border radius value of the card and button element
    ''' </summary>
    Public Property Radius As String
    ''' <summary>
    ''' is the target website using a dark theme?
    ''' </summary>
    Public Property IsDark As Boolean
    ''' <summary>
    ''' the website title text, this value comes from the ``&lt;title>``
    ''' tag of the index page.
    ''' </summary>
    Public Property SiteTitle As String
    ''' <summary>
    ''' how many css rule blocks have been parsed for the theme extraction
    ''' </summary>
    Public Property CssRules As Integer

    ''' <summary>
    ''' the built-in fallback theme, this theme will be used when the css
    ''' theme extraction is failed or the target website does not contains
    ''' any css style file.
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function DefaultTheme() As SiteTheme
        Return New SiteTheme With {
            .Background = "#f6f8fa",
            .Surface = "#ffffff",
            .Primary = "#4c8bf5",
            .OnPrimary = "#ffffff",
            .TextColor = "#24292f",
            .MutedText = "#6a737d",
            .LinkColor = "#0969da",
            .BorderColor = "#d8dee4",
            .RowAlt = "#f1f4f8",
            .FontFamily = """Segoe UI"", ""Helvetica Neue"", Helvetica, Arial, sans-serif",
            .Radius = "6px",
            .IsDark = False,
            .SiteTitle = "Sitemap"
        }
    End Function

    ''' <summary>
    ''' extract the website theme from a collection of css document text
    ''' </summary>
    ''' <param name="cssTexts">
    ''' a collection of the css document text, includes the external css
    ''' file content and the inline ``&lt;style>`` block text.
    ''' </param>
    ''' <param name="themeOverride">
    ''' the command line user theme override values, the Nothing property
    ''' value in this model means use the auto extract result.
    ''' </param>
    ''' <param name="siteTitle"></param>
    ''' <returns></returns>
    Public Shared Function Extract(cssTexts As IEnumerable(Of String),
                                   Optional themeOverride As ThemeOverride = Nothing,
                                   Optional siteTitle As String = Nothing) As SiteTheme

        Dim defaults As SiteTheme = DefaultTheme()
        Dim stats As New ThemeStatistics
        Dim count As Integer = 0

        If Not cssTexts Is Nothing Then
            For Each css As String In cssTexts
                If String.IsNullOrWhiteSpace(css) Then
                    Continue For
                End If

                Call stats.Collect(css)
                count += 1
            Next
        End If

        Dim overrideBg As String = ColorTool.FindColor(coalesce(themeOverride?.Background, Nothing))

        ' the alpha channel of a semi-transparent color value should be
        ' composited on top of the website background color, so that the
        ' background color must be determined at first.
        Dim background As String = overrideBg
        Dim autoBg As String = stats.Pick("bg", background)

        If background Is Nothing Then
            background = If(autoBg, defaults.Background)
        End If

        Dim isDark As Boolean = ColorTool.Luminance(background) < 0.45

        If Not themeOverride Is Nothing AndAlso Not themeOverride.IsDark Is Nothing Then
            isDark = themeOverride.IsDark.Value
        End If

        Dim text As String = firstColor(themeOverride?.TextColor, stats.Pick("text", background), background)
        Dim surface As String = firstColor(themeOverride?.Surface, stats.Pick("surface", background), background)
        Dim primary As String = firstColor(themeOverride?.Primary, stats.Pick("primary", background), background)
        Dim link As String = firstColor(themeOverride?.LinkColor, stats.Pick("link", background), background)
        Dim border As String = stats.Pick("border", background)
        Dim muted As String = stats.Pick("muted", background)
        Dim font As String = If(themeOverride?.FontFamily, stats.Pick("font", Nothing))
        Dim radius As String = If(themeOverride?.Radius, stats.Pick("radius", Nothing))

        ' fallback for the missing theme value
        If text Is Nothing Then
            text = If(isDark, "#e6edf3", "#24292f")
        End If

        ' the body text color should keeps a readable contrast with the background
        If Math.Abs(ColorTool.Luminance(text) - ColorTool.Luminance(background)) < 0.25 Then
            text = ColorTool.ReadableTextOn(background)
        End If

        If surface Is Nothing OrElse
           Math.Abs(ColorTool.Luminance(surface) - ColorTool.Luminance(background)) < 0.015 Then

            surface = If(isDark,
                ColorTool.Lighten(background, 0.07),
                ColorTool.Lighten(background, 0.7))
        End If

        If primary Is Nothing Then
            primary = If(isDark, "#58a6ff", "#4c8bf5")
        End If

        If link Is Nothing Then
            link = primary
        End If

        If border Is Nothing OrElse
           Math.Abs(ColorTool.Luminance(border) - ColorTool.Luminance(background)) < 0.05 Then

            border = ColorTool.Mix(text, background, 0.78)
        End If

        If muted Is Nothing Then
            muted = ColorTool.Mix(text, background, 0.38)
        End If

        If font Is Nothing Then
            font = defaults.FontFamily
        End If

        If radius Is Nothing Then
            radius = defaults.Radius
        End If

        Dim rowAlt As String = If(isDark,
            ColorTool.Lighten(background, 0.04),
            ColorTool.Darken(background, 0.04))

        Return New SiteTheme With {
            .Background = background,
            .Surface = surface,
            .Primary = primary,
            .OnPrimary = ColorTool.ReadableTextOn(primary),
            .TextColor = text,
            .MutedText = muted,
            .LinkColor = link,
            .BorderColor = border,
            .RowAlt = rowAlt,
            .FontFamily = font,
            .Radius = radius,
            .IsDark = isDark,
            .SiteTitle = If(String.IsNullOrWhiteSpace(siteTitle), "Sitemap", siteTitle.Trim),
            .CssRules = stats.RuleCount
        }
    End Function

    ''' <summary>
    ''' get the first not empty string value
    ''' </summary>
    Private Shared Function coalesce(ParamArray values As String()) As String
        For Each value As String In values
            If Not String.IsNullOrWhiteSpace(value) Then
                Return value
            End If
        Next

        Return Nothing
    End Function

    Private Shared Function firstColor(override As String, auto As String, background As String) As String
        If Not String.IsNullOrWhiteSpace(override) Then
            Dim color As String = ColorTool.FindColor(override, background)

            If Not color Is Nothing Then
                Return color
            End If

            Return override.Trim
        End If

        Return auto
    End Function

    Public Overrides Function ToString() As String
        Return $"[{If(IsDark, "dark", "light")}] bg={Background}, surface={Surface}, primary={Primary}, text={TextColor}, link={LinkColor}, font={FontFamily}, radius={Radius}"
    End Function
End Class

''' <summary>
''' The theme value that is specific by the command line user, the Nothing
''' property value in this model means use the auto extract result from
''' the css style of the target website.
''' </summary>
Public Class ThemeOverride

    ''' <summary>
    ''' override the page background color, example as ``#0a0d12``
    ''' </summary>
    Public Property Background As String
    ''' <summary>
    ''' override the card background color, example as ``#10141c``
    ''' </summary>
    Public Property Surface As String
    ''' <summary>
    ''' override the theme accent color, example as ``#ff3b2f``
    ''' </summary>
    Public Property Primary As String
    ''' <summary>
    ''' override the body text color, example as ``#eef2f7``
    ''' </summary>
    Public Property TextColor As String
    ''' <summary>
    ''' override the hyperlink text color, example as ``#ff3b2f``
    ''' </summary>
    Public Property LinkColor As String
    ''' <summary>
    ''' override the font family stack, example as ``"Inter", sans-serif``
    ''' </summary>
    Public Property FontFamily As String
    ''' <summary>
    ''' override the border radius value, example as ``6px``
    ''' </summary>
    Public Property Radius As String
    ''' <summary>
    ''' force the dark or the light theme mode
    ''' </summary>
    Public Property IsDark As Boolean?

    ''' <summary>
    ''' is there any theme value that is specific by the user?
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property IsEmptyOverride As Boolean
        Get
            Return Background Is Nothing AndAlso
                   Surface Is Nothing AndAlso
                   Primary Is Nothing AndAlso
                   TextColor Is Nothing AndAlso
                   LinkColor Is Nothing AndAlso
                   FontFamily Is Nothing AndAlso
                   Radius Is Nothing AndAlso
                   IsDark Is Nothing
        End Get
    End Property

    ''' <summary>
    ''' parse the theme override value from the command line arguments
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns>
    ''' this function returns Nothing if there is no theme override value
    ''' that is specific by the command line user.
    ''' </returns>
    Public Shared Function Parse(args As CommandLine) As ThemeOverride
        If args Is Nothing Then
            Return Nothing
        End If

        Dim theme As New ThemeOverride With {
            .Background = nullIfEmpty(args("--bg").ToString()),
            .Surface = nullIfEmpty(args("--surface").ToString()),
            .Primary = nullIfEmpty(args("--primary").ToString()),
            .TextColor = nullIfEmpty(args("--text").ToString()),
            .LinkColor = nullIfEmpty(args("--link").ToString()),
            .FontFamily = nullIfEmpty(args("--font").ToString()),
            .Radius = nullIfEmpty(args("--radius").ToString())
        }

        If CBool(args("--dark")) Then
            theme.IsDark = True
        ElseIf CBool(args("--light")) Then
            theme.IsDark = False
        End If

        If theme.IsEmptyOverride Then
            Return Nothing
        End If

        Return theme
    End Function

    Private Shared Function nullIfEmpty(value As String) As String
        If String.IsNullOrWhiteSpace(value) Then
            Return Nothing
        End If

        Return value.Trim
    End Function
End Class

''' <summary>
''' the css font-family expression value normalization
''' </summary>
Public Module FontStack

    ''' <summary>
    ''' cleanup the css font-family value as a valid html style value
    ''' </summary>
    ''' <param name="expression"></param>
    ''' <returns></returns>
    Public Function Clean(expression As String) As String
        If String.IsNullOrWhiteSpace(expression) Then
            Return Nothing
        End If

        Dim fonts As New List(Of String)
        Dim buffer As New StringBuilder
        Dim inQuot As Boolean = False
        Dim quot As Char = Nothing

        For i As Integer = 0 To expression.Length - 1
            Dim ch As Char = expression(i)

            If inQuot Then
                If ch = quot Then
                    inQuot = False
                Else
                    buffer.Append(ch)
                End If
            ElseIf ch = """"c OrElse ch = "'"c Then
                quot = ch
                inQuot = True
            ElseIf ch = ","c Then
                fonts.Add(buffer.ToString.Trim)
                buffer.Clear()
            Else
                buffer.Append(ch)
            End If
        Next

        fonts.Add(buffer.ToString.Trim)

        Dim stack As String() = fonts _
            .Select(Function(s) s.Trim) _
            .Where(Function(s) s.Length > 0) _
            .Where(Function(s) Not s.Equals("inherit", StringComparison.OrdinalIgnoreCase)) _
            .Where(Function(s) Not s.Equals("initial", StringComparison.OrdinalIgnoreCase)) _
            .Where(Function(s) Not s.Equals("unset", StringComparison.OrdinalIgnoreCase)) _
            .Distinct _
            .Take(6) _
            .ToArray

        If stack.Length = 0 Then
            Return Nothing
        End If

        Return String.Join(", ", stack.Select(Function(s)
                                                  If s.IndexOf(" "c) > -1 AndAlso s.First <> """"c Then
                                                      Return """" & s & """"
                                                  Else
                                                      Return s
                                                  End If
                                              End Function))
    End Function
End Module

''' <summary>
''' The weighted voting statistics of the css style value for the theme
''' color extraction.
''' </summary>
Friend Class ThemeStatistics

    ''' <summary>
    ''' category name => (raw css value => weighted score)
    ''' </summary>
    ReadOnly votes As New Dictionary(Of String, Dictionary(Of String, Double))
    ''' <summary>
    ''' the css variable name => the raw css value
    ''' </summary>
    ReadOnly variables As New Dictionary(Of String, String)

    Public Property RuleCount As Integer

    ReadOnly colorWords As String() = {
        "red", "orange", "amber", "yellow", "gold", "lime", "green",
        "teal", "cyan", "blue", "indigo", "violet", "purple", "magenta",
        "pink", "brown", "turquoise", "crimson", "brand"
    }

    ''' <summary>
    ''' the css variable name that means the color value is a soft,
    ''' a muted or a semi-transparent variant of the theme color.
    ''' </summary>
    ReadOnly weakNames As String() = {
        "soft", "mute", "dim", "faint", "light", "alpha", "ghost",
        "hover", "disabled", "subtle", "shade", "tint", "overlay"
    }

    ReadOnly varPattern As New Regex("var\(\s*(--[\w-]+)\s*(?:,\s*([^)]+?))?\s*\)", RegexOptions.IgnoreCase)

    ''' <summary>
    ''' collect the theme style value votes from a css document text
    ''' </summary>
    ''' <param name="cssText"></param>
    Public Sub Collect(cssText As String)
        Dim rules As List(Of CssRule) = CssRuleParser.Parse(cssText)

        If rules.Count = 0 Then
            Return
        End If

        ' pass 1: collect all of the css variables at first
        For Each rule As CssRule In rules
            For Each [property] As KeyValuePair(Of String, String) In rule.Properties
                If [property].Key.StartsWith("--") Then
                    variables([property].Key) = [property].Value
                End If
            Next
        Next

        ' pass 2: vote for the theme style values
        For Each rule As CssRule In rules
            RuleCount += 1

            Dim weight As Double = SelectorWeight(rule.Selector)

            For Each [property] As KeyValuePair(Of String, String) In rule.Properties
                Call vote(rule.Selector, [property].Key, [property].Value, weight)
            Next
        Next

        ' pass 3: the css variable name is a strong hint of the theme color
        For Each var As KeyValuePair(Of String, String) In variables
            Dim name As String = var.Key.Substring(2).ToLower
            Dim value As String = ResolveVariables(var.Value)

            If value Is Nothing Then
                Continue For
            End If

            Dim weak As Boolean = weakNames.Any(Function(w) name.IndexOf(w, StringComparison.Ordinal) > -1)
            Dim classify As (category As String, weight As Double) = ClassifyVariable(name)
            Dim score As Double = classify.weight

            If weak Then
                score *= 0.3
            End If

            If score > 0 AndAlso Not classify.category Is Nothing Then
                Call addVote(classify.category, value, score)
            End If
        Next
    End Sub

    Private Sub vote(selectorText As String, key As String, rawValue As String, weight As Double)
        Dim value As String = ResolveVariables(rawValue)

        If value Is Nothing Then
            Return
        End If

        Dim name As String = key.ToLower.Trim
        Dim isLink As Boolean = Regex.IsMatch(selectorText, "(^|[\s,>])a(:\w+)?(\s|,|$)", RegexOptions.IgnoreCase)
        Dim isBrand As Boolean = Regex.IsMatch(selectorText, "btn|button|badge|tag|chip|accent|active|current|primary|logo|brand", RegexOptions.IgnoreCase)

        Select Case name
            Case "background-color", "background", "background-image"
                If name = "background-image" AndAlso value.StartsWith("url(") Then
                    Return
                End If

                Call addVote("bg", value, weight)

                If isBrand Then
                    Call addVote("primary", value, weight * 1.2)
                End If

            Case "color"
                If isLink Then
                    Call addVote("link", value, weight * 1.5)
                End If

                Call addVote("text", value, weight)

                If isBrand Then
                    Call addVote("primary", value, weight * 0.8)
                End If

            Case "accent-color", "outline-color", "caret-color"
                Call addVote("primary", value, weight * 3)

            Case "fill", "stroke"
                Call addVote("primary", value, weight * 0.6)

            Case "border-color", "border-top-color", "border-bottom-color",
                 "border-left-color", "border-right-color", "border", "outline"
                Call addVote("border", value, weight * 0.6)

            Case "font-family", "font"
                ' the font-family of the body element is the website font,
                ' the font-family of the code block is not.
                Call addVote("font", value, If(weight >= 7, weight * 2.5, weight))

            Case "border-radius"
                Call addVote("radius", value, weight * 0.4)
        End Select
    End Sub

    Private Sub addVote(category As String, rawValue As String, score As Double)
        If score <= 0 OrElse String.IsNullOrWhiteSpace(rawValue) Then
            Return
        End If

        If Not votes.ContainsKey(category) Then
            votes(category) = New Dictionary(Of String, Double)
        End If

        Dim bucket As Dictionary(Of String, Double) = votes(category)
        Dim key As String = rawValue.Trim

        If Not bucket.ContainsKey(key) Then
            bucket(key) = 0
        End If

        bucket(key) += score
    End Sub

    ''' <summary>
    ''' resolve the ``var(--name)`` css function expression as the literal
    ''' css value string.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns>
    ''' this function returns Nothing if the css variable can not be
    ''' resolved from the parsed css documents.
    ''' </returns>
    Public Function ResolveVariables(value As String) As String
        If value Is Nothing Then
            Return Nothing
        End If

        Dim text As String = value.Trim
        Dim level As Integer = 0

        Do While text.IndexOf("var(", StringComparison.OrdinalIgnoreCase) > -1 AndAlso level < 8
            Dim replaced As Boolean = False

            text = varPattern.Replace(
                text,
                Function(m As Match)
                    Dim name As String = m.Groups(1).Value

                    If variables.ContainsKey(name) Then
                        replaced = True
                        Return variables(name)
                    ElseIf m.Groups(2).Success Then
                        replaced = True
                        Return m.Groups(2).Value
                    Else
                        Return m.Value
                    End If
                End Function)

            If Not replaced Then
                Exit Do
            End If

            level += 1
        Loop

        If text.IndexOf("var(", StringComparison.OrdinalIgnoreCase) > -1 Then
            Return Nothing
        End If

        text = text.Trim

        If text.Length = 0 Then
            Return Nothing
        End If

        Return text
    End Function

    ''' <summary>
    ''' get the css value that owns the highest weighted score in the
    ''' given theme category.
    ''' </summary>
    ''' <param name="category"></param>
    ''' <param name="background">
    ''' the background color that is used for compositing the
    ''' semi-transparent color value.
    ''' </param>
    ''' <returns></returns>
    Public Function Pick(category As String, background As String) As String
        If Not votes.ContainsKey(category) Then
            Return Nothing
        End If

        If category = "radius" Then
            Dim best As String = Nothing
            Dim bestScore As Double = 0

            For Each vote As KeyValuePair(Of String, Double) In votes(category)
                Dim value As String = normalizeRadius(vote.Key)

                If value Is Nothing Then
                    Continue For
                End If

                If vote.Value > bestScore Then
                    bestScore = vote.Value
                    best = value
                End If
            Next

            Return best
        End If

        If category = "font" Then
            Dim candidates As New List(Of (font As String, score As Double))

            For Each vote As KeyValuePair(Of String, Double) In votes(category)
                Dim value As String = FontStack.Clean(vote.Key)

                If value Is Nothing Then
                    Continue For
                End If

                candidates.Add((value, vote.Value))
            Next

            If candidates.Count = 0 Then
                Return Nothing
            End If

            Dim best As (font As String, score As Double) = candidates.OrderByDescending(Function(c) c.score).First
            Dim sans = candidates _
                .Where(Function(c) Not isMonospace(c.font)) _
                .OrderByDescending(Function(c) c.score) _
                .FirstOrDefault

            ' the monospace font of the code block should not be used as
            ' the website font when there is a proportional font candidate
            If Not sans.font Is Nothing AndAlso sans.score >= best.score * 0.5 Then
                Return sans.font
            End If

            Return best.font
        End If

        Dim scores As New Dictionary(Of String, Double)

        For Each vote As KeyValuePair(Of String, Double) In votes(category)
            Dim color As String = ColorTool.FindColor(vote.Key, background)

            If color Is Nothing Then
                Continue For
            End If

            Dim score As Double = vote.Value * colorQuality(color, category)

            If Not scores.ContainsKey(color) Then
                scores(color) = 0
            End If

            scores(color) += score
        Next

        If scores.Count = 0 Then
            Return Nothing
        End If

        Return scores.OrderByDescending(Function(v) v.Value).First.Key
    End Function

    ''' <summary>
    ''' is the given font stack a monospace font stack?
    ''' </summary>
    ''' <param name="font"></param>
    ''' <returns></returns>
    Private Function isMonospace(font As String) As Boolean
        If font Is Nothing Then
            Return False
        End If

        Dim lower As String = font.ToLower

        Return lower.Contains("monospace") OrElse
               lower.Contains("mono") OrElse
               lower.Contains("consolas") OrElse
               lower.Contains("courier")
    End Function

    ''' <summary>
    ''' how well the color value is fit for the given theme category
    ''' </summary>
    Private Function colorQuality(color As String, category As String) As Double
        Dim sat As Double = ColorTool.Saturation(color)
        Dim lum As Double = ColorTool.Luminance(color)
        Dim bright As Double = 1.0 - Math.Abs(lum - 0.5) * 1.3

        If bright < 0.15 Then
            bright = 0.15
        End If

        Select Case category
            Case "primary"
                ' a good accent color should be a saturated color
                Return Math.Max(0.05, (0.15 + 0.85 * sat) * bright)
            Case "bg"
                ' the background color is usually a low saturation color
                Return Math.Max(0.05, 1.0 - sat * 0.55)
            Case "text", "muted"
                Return Math.Max(0.05, 1.0 - sat * 0.35)
            Case Else
                Return Math.Max(0.05, bright)
        End Select
    End Function

    Private Function normalizeRadius(value As String) As String
        If String.IsNullOrWhiteSpace(value) Then
            Return Nothing
        End If

        Dim m As Match = Regex.Match(value.Trim.ToLower, "(\d+(\.\d+)?)\s*(px|rem|em|%)")

        If Not m.Success Then
            Return Nothing
        End If

        Dim size As Double = Double.Parse(m.Groups(1).Value, CultureInfo.InvariantCulture)
        Dim unit As String = m.Groups(3).Value

        If unit = "rem" OrElse unit = "em" Then
            size *= 16
        End If

        ' a too large border radius value is not a good choice for the
        ' table and card rendering
        If size > 16 Then
            size = 16
        End If

        Return $"{Math.Round(size)}px"
    End Function

    ''' <summary>
    ''' classify a css variable into the theme category by its name
    ''' </summary>
    Public Function ClassifyVariable(name As String) As (category As String, weight As Double)
        Dim n As String = name.ToLower

        If n = "bg" OrElse n = "background" OrElse n = "body-bg" OrElse n = "body-background" Then
            Return ("bg", 24)
        ElseIf n.StartsWith("bg") OrElse n.Contains("background") Then
            If n.Contains("elev") OrElse n.Contains("surface") OrElse n.Contains("card") OrElse
               n.Contains("panel") OrElse n.Contains("code") OrElse n.Contains("alt") Then
                Return ("surface", 14)
            Else
                Return ("bg", 8)
            End If
        ElseIf n.Contains("surface") OrElse n.Contains("card-bg") OrElse n = "card" OrElse n = "panel" Then
            Return ("surface", 14)
        ElseIf n.Contains("mute") OrElse n.Contains("dim") OrElse n.Contains("faint") OrElse
               n.Contains("secondary") OrElse n.Contains("subtext") Then
            Return ("muted", 9)
        ElseIf n = "ink" OrElse n = "text" OrElse n = "fg" OrElse n = "foreground" OrElse
               n.Contains("text-color") OrElse n.Contains("foreground") Then
            Return ("text", 16)
        ElseIf n = "primary" OrElse n = "accent" OrElse n = "brand" OrElse n = "theme-color" OrElse
               n.Contains("primary") OrElse n.Contains("accent") OrElse n.Contains("brand") Then
            Return ("primary", 22)
        ElseIf colorWords.Any(Function(w) n.IndexOf(w, StringComparison.Ordinal) > -1) Then
            Return ("primary", 6)
        ElseIf n.Contains("link") OrElse n.Contains("anchor") Then
            Return ("link", 14)
        ElseIf n.Contains("line") OrElse n.Contains("border") OrElse n.Contains("divider") OrElse
               n.Contains("stroke") OrElse n.Contains("hairline") Then
            Return ("border", 12)
        ElseIf n.Contains("radius") OrElse n.Contains("rounded") Then
            Return ("radius", 8)
        ElseIf n.Contains("font") OrElse n = "sans" OrElse n = "serif" OrElse n = "mono" Then
            Return ("font", 12)
        Else
            Return (Nothing, 0)
        End If
    End Function

    ''' <summary>
    ''' the importance weight of a css selector
    ''' </summary>
    Private Function SelectorWeight(selectorText As String) As Double
        If selectorText Is Nothing Then
            Return 1
        End If

        Dim parts As String() = selectorText.Split(","c)

        If parts.Length > 1 Then
            ' a css rule block may owns a list of the selector, example as
            ' the ``html, body {...}`` rule block, the importance weight of
            ' such rule block is the max weight of its selector list.
            Return parts.Max(Function(part) SelectorWeight(part))
        End If

        Dim s As String = Regex.Replace(selectorText.Trim.ToLower, "\s+", " ")

        If s = "body" OrElse s = "html" Then
            Return 12
        ElseIf s.StartsWith(":root") OrElse s = "root" Then
            Return 6
        ElseIf s = "*" Then
            Return 0.4
        ElseIf Regex.IsMatch(s, "^a(:\w+)?$") OrElse Regex.IsMatch(s, "^[\w\.\#\-]+\s+a(:\w+)?$") Then
            Return 7
        ElseIf s.Contains("body") Then
            Return 7
        ElseIf s.Contains("btn") OrElse s.Contains("button") Then
            Return 4
        ElseIf s.Contains("nav") OrElse s.Contains("header") OrElse s.Contains("footer") OrElse
               s.Contains("card") OrElse s.Contains("panel") Then
            Return 3
        ElseIf s.StartsWith(".") OrElse s.StartsWith("#") Then
            Return 1.5
        Else
            Return 1
        End If
    End Function
End Class
