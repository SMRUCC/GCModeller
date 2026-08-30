Imports System.Text
Imports System.Text.RegularExpressions

''' <summary>
''' A css rule block that is parsed from a css document: it contains the
''' selector expression text and the style property declarations of this
''' rule block.
''' </summary>
Public Class CssRule

    ''' <summary>
    ''' the css selector expression text, example as ``body`` or ``.btn-primary``
    ''' </summary>
    ''' <returns></returns>
    Public Property Selector As String
    ''' <summary>
    ''' the style property name and value pairs, the property name is
    ''' always in lower case text.
    ''' </summary>
    ''' <returns></returns>
    Public Property Properties As Dictionary(Of String, String)

    ''' <summary>
    ''' get a style property value by the property name
    ''' </summary>
    ''' <param name="name">the style property name, it is case insensitive</param>
    ''' <returns></returns>
    Default Public ReadOnly Property Item(name As String) As String
        Get
            If Properties Is Nothing Then
                Return Nothing
            End If

            Dim value As String = Nothing

            If Properties.TryGetValue(name.ToLower, value) Then
                Return value
            End If

            Return Nothing
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{Selector} {{ {Properties.Count} properties }}"
    End Function
End Class

''' <summary>
''' A light weight css document parser for the website theme extraction.
'''
''' (the css parser of the ``Microsoft.VisualBasic.MIME.Html`` library is
''' implemented by a regular expression that will be catastrophic
''' backtracking on the real world css document, so that a linear scan
''' tokenizer is implemented here for the css document parsing)
''' </summary>
Public Module CssRuleParser

    ReadOnly commentBlock As New Regex("/\*.*?\*/", RegexOptions.Singleline)

    ''' <summary>
    ''' the css at-rule that wraps a collection of the css rule blocks,
    ''' the rule blocks inside these at-rules should be parsed as the
    ''' normal css rule block.
    ''' </summary>
    ReadOnly groupAtRules As String() = {
        "@media", "@supports", "@document", "@layer", "@container", "@scope"
    }

    ''' <summary>
    ''' the css at-rule that owns a declaration block, it should be parsed
    ''' as a normal css rule block.
    ''' </summary>
    ReadOnly blockAtRules As String() = {
        "@font-face", "@page", "@counter-style", "@property", "@viewport"
    }

    ''' <summary>
    ''' parse a css document text as a collection of the css rule blocks
    ''' </summary>
    ''' <param name="css">
    ''' the css document text, it can be the content of an external css
    ''' file or the content of an inline ``&lt;style>`` block.
    ''' </param>
    ''' <returns></returns>
    Public Function Parse(css As String) As List(Of CssRule)
        Dim rules As New List(Of CssRule)

        If String.IsNullOrWhiteSpace(css) Then
            Return rules
        End If

        Dim scanner As New Scanner(css)

        Try
            Call scanner.ReadRules(rules, 0)
        Catch ex As Exception
            ' the css document is malformed, just returns the rules that
            ' have been parsed already.
        End Try

        Return rules
    End Function

    ''' <summary>
    ''' the css document text scanner
    ''' </summary>
    Private Class Scanner

        ReadOnly text As String
        Dim pos As Integer

        Sub New(css As String)
            text = css
            pos = 0
        End Sub

        Private ReadOnly Property EndOfText As Boolean
            Get
                Return pos >= text.Length
            End Get
        End Property

        Private ReadOnly Property Current As Char
            Get
                Return text(pos)
            End Get
        End Property

        ''' <summary>
        ''' read all of the css rule blocks from the current position
        ''' </summary>
        ''' <param name="rules"></param>
        ''' <param name="depth">
        ''' the nested level of the current css block, it is used for
        ''' avoiding the infinite loop on a malformed css document.
        ''' </param>
        Public Sub ReadRules(rules As List(Of CssRule), depth As Integer)
            If depth > 16 Then
                Return
            End If

            Do While Not EndOfText
                Call SkipBlanks()

                If EndOfText Then
                    Return
                End If

                If Current = "}"c Then
                    ' the end of the current css block
                    pos += 1
                    Return
                End If

                Dim prelude As String = ReadUntil("{"c, ";"c, "}"c)

                If EndOfText Then
                    Return
                End If

                If Current = "}"c Then
                    pos += 1
                    Return
                ElseIf Current = ";"c Then
                    ' a css statement, example as the @import url(...) statement
                    pos += 1
                    Continue Do
                End If

                ' the current character is the '{' character
                pos += 1

                Dim name As String = CleanSelector(prelude)

                If IsGroupAtRule(name) Then
                    Call ReadRules(rules, depth + 1)
                Else
                    Dim body As String = ReadBlock()

                    If Not body Is Nothing Then
                        Dim rule As CssRule = ParseDeclarations(name, body)

                        If Not rule Is Nothing Then
                            Call rules.Add(rule)
                        End If
                    End If
                End If
            Loop
        End Sub

        ''' <summary>
        ''' skip the whitespace and the comment block of the css document
        ''' </summary>
        Private Sub SkipBlanks()
            Do While Not EndOfText
                If Char.IsWhiteSpace(Current) Then
                    pos += 1
                ElseIf Current = "/"c AndAlso pos + 1 < text.Length AndAlso text(pos + 1) = "*"c Then
                    ' skip the css comment block
                    pos += 2

                    Do While Not EndOfText
                        If Current = "*"c AndAlso pos + 1 < text.Length AndAlso text(pos + 1) = "/"c Then
                            pos += 2
                            Exit Do
                        End If

                        pos += 1
                    Loop
                Else
                    Return
                End If
            Loop
        End Sub

        ''' <summary>
        ''' read the css text until one of the given stop character is
        ''' reached, the string literal and the parenthesis block inside
        ''' the css text will be skipped.
        ''' </summary>
        ''' <param name="stops"></param>
        ''' <returns></returns>
        Private Function ReadUntil(ParamArray stops As Char()) As String
            Dim buffer As New StringBuilder
            Dim depth As Integer = 0

            Do While Not EndOfText
                Dim ch As Char = Current

                If ch = """"c OrElse ch = "'"c Then
                    Call buffer.Append(ReadString(ch))
                    Continue Do
                ElseIf ch = "("c Then
                    depth += 1
                ElseIf ch = ")"c Then
                    If depth > 0 Then
                        depth -= 1
                    End If
                ElseIf ch = "/"c AndAlso pos + 1 < text.Length AndAlso text(pos + 1) = "*"c Then
                    Call SkipBlanks()
                    Call buffer.Append(" "c)
                    Continue Do
                ElseIf depth = 0 AndAlso stops.Contains(ch) Then
                    Return buffer.ToString
                End If

                Call buffer.Append(ch)
                pos += 1
            Loop

            Return buffer.ToString
        End Function

        ''' <summary>
        ''' read a css string literal text
        ''' </summary>
        ''' <param name="quot"></param>
        ''' <returns></returns>
        Private Function ReadString(quot As Char) As String
            Dim buffer As New StringBuilder

            Call buffer.Append(quot)
            pos += 1

            Do While Not EndOfText
                Dim ch As Char = Current

                If ch = "\"c AndAlso pos + 1 < text.Length Then
                    Call buffer.Append(ch)
                    pos += 1
                    Call buffer.Append(Current)
                    pos += 1
                    Continue Do
                End If

                Call buffer.Append(ch)
                pos += 1

                If ch = quot Then
                    Exit Do
                End If
            Loop

            Return buffer.ToString
        End Function

        ''' <summary>
        ''' read the declaration body of a css rule block, the nested
        ''' brace block inside the declaration body will be skipped.
        ''' </summary>
        ''' <returns></returns>
        Private Function ReadBlock() As String
            Dim buffer As New StringBuilder
            Dim depth As Integer = 1

            Do While Not EndOfText
                Dim ch As Char = Current

                If ch = """"c OrElse ch = "'"c Then
                    Call buffer.Append(ReadString(ch))
                    Continue Do
                ElseIf ch = "{"c Then
                    depth += 1
                ElseIf ch = "}"c Then
                    depth -= 1

                    If depth = 0 Then
                        pos += 1
                        Return buffer.ToString
                    End If
                ElseIf ch = "/"c AndAlso pos + 1 < text.Length AndAlso text(pos + 1) = "*"c Then
                    Dim mark As Integer = pos

                    Call SkipBlanks()

                    If EndOfText Then
                        Return buffer.ToString
                    End If

                    If Current <> "}"c AndAlso Current <> "{"c Then
                        Call buffer.Append(" "c)
                    Else
                        pos = mark
                    End If

                    Continue Do
                End If

                Call buffer.Append(ch)
                pos += 1
            Loop

            Return buffer.ToString
        End Function

        Private Function IsGroupAtRule(selector As String) As Boolean
            If Not selector.StartsWith("@") Then
                Return False
            End If

            Dim name As String = selector.Split(" "c).First.ToLower

            If blockAtRules.Contains(name) Then
                Return False
            End If

            If groupAtRules.Contains(name) Then
                Return True
            End If

            ' the @keyframes block contains the percentage keyframe
            ' blocks, these blocks are not the css selector rule
            Return name.StartsWith("@keyframes") OrElse name.StartsWith("@-")
        End Function

        Private Function CleanSelector(prelude As String) As String
            If prelude Is Nothing Then
                Return ""
            End If

            Dim name As String = prelude.Trim

            ' remove the css comment text from the selector
            name = commentBlock.Replace(name, " ")

            Return Regex.Replace(name, "\s+", " ").Trim
        End Function

        ''' <summary>
        ''' parse the declaration body text of a css rule block as the
        ''' style property name and value pairs.
        ''' </summary>
        ''' <param name="selector"></param>
        ''' <param name="body"></param>
        ''' <returns>
        ''' this function returns Nothing if there is no valid style
        ''' property declaration inside the given rule block.
        ''' </returns>
        Private Function ParseDeclarations(selector As String, body As String) As CssRule
            Dim properties As New Dictionary(Of String, String)

            For Each part As String In SplitDeclarations(body)
                Dim colon As Integer = part.IndexOf(":"c)

                If colon <= 0 Then
                    Continue For
                End If

                Dim name As String = part.Substring(0, colon).Trim.ToLower
                Dim value As String = part.Substring(colon + 1).Trim

                If name.Length = 0 OrElse value.Length = 0 Then
                    Continue For
                End If

                ' remove the !important flag from the property value
                If value.EndsWith("!important", StringComparison.OrdinalIgnoreCase) Then
                    value = value.Substring(0, value.Length - "!important".Length).Trim
                End If

                If value.EndsWith(";") Then
                    value = value.TrimEnd(";"c).Trim
                End If

                If value.Length = 0 Then
                    Continue For
                End If

                properties(name) = value
            Next

            If properties.Count = 0 Then
                Return Nothing
            End If

            Return New CssRule With {
                .Selector = selector,
                .Properties = properties
            }
        End Function

        ''' <summary>
        ''' split the declaration body text by the ``;`` character, the
        ''' string literal and the parenthesis block will be skipped.
        ''' </summary>
        ''' <param name="body"></param>
        ''' <returns></returns>
        Private Function SplitDeclarations(body As String) As List(Of String)
            Dim parts As New List(Of String)
            Dim buffer As New StringBuilder
            Dim depth As Integer = 0
            Dim quot As Char = Nothing

            For i As Integer = 0 To body.Length - 1
                Dim ch As Char = body(i)

                If quot <> Nothing Then
                    Call buffer.Append(ch)

                    If ch = "\"c AndAlso i + 1 < body.Length Then
                        Call buffer.Append(body(i + 1))
                        i += 1
                    ElseIf ch = quot Then
                        quot = Nothing
                    End If

                    Continue For
                End If

                If ch = """"c OrElse ch = "'"c Then
                    quot = ch
                    Call buffer.Append(ch)
                ElseIf ch = "("c Then
                    depth += 1
                    Call buffer.Append(ch)
                ElseIf ch = ")"c Then
                    If depth > 0 Then
                        depth -= 1
                    End If

                    Call buffer.Append(ch)
                ElseIf ch = ";"c AndAlso depth = 0 Then
                    parts.Add(buffer.ToString.Trim)
                    buffer.Clear()
                Else
                    Call buffer.Append(ch)
                End If
            Next

            parts.Add(buffer.ToString.Trim)

            Return parts
        End Function
    End Class
End Module
