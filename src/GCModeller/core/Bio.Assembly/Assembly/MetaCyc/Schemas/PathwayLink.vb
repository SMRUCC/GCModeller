Imports System.Text
Imports System.Text.RegularExpressions

Namespace Assembly.MetaCyc.Schema.Metabolism

    Public Class PathwayLink

        Public Property Substrate As String
        Public Property LinkedPathways As PathwaysLink()

        Public Class PathwaysLink
            Public Enum LinkTypes
                NotSpecific
                OUTGOING
                INCOMING
            End Enum

            Public Property Id As String
            Public Property LinkType As LinkTypes

            Sub New(str As String)
                Dim Tokens = str.Split

                Id = Tokens.First
                If Tokens.Count > 1 Then
                    If InStr(Tokens.Last, "INCOMING") Then
                        LinkType = LinkTypes.INCOMING
                    ElseIf InStr(Tokens.Last, "OUTGOING") Then
                        LinkType = LinkTypes.OUTGOING
                    Else
                        LinkType = LinkTypes.NotSpecific
                    End If
                Else
                    LinkType = LinkTypes.NotSpecific
                End If
            End Sub
        End Class

        ''' <summary>
        ''' A regex expression string that use for split the commandline text.
        ''' (用于分析命令行字符串的正则表达式)
        ''' </summary>
        ''' <remarks></remarks>
        Public Const SPLIT_REGX_EXPRESSION As String = " (?=(?:[^""]|""[^""]*"")*$)"
        Protected Friend Const QUOT As Char = """"c

        Sub New(str As String)
            Dim sBuilder As StringBuilder = New StringBuilder(Mid(str, 2, Len(str) - 2))
            Call sBuilder.Replace("(", QUOT)
            Call sBuilder.Replace(")", QUOT)

            Dim Tokens = GetTokens(sBuilder.ToString) '向解析命令行一样进行解析
            LinkedPathways = New PathwaysLink(Tokens.Length - 2) {}
            For i As Integer = 1 To Tokens.Length - 1
                LinkedPathways(i - 1) = New PathwaysLink(Tokens(i))
            Next
        End Sub

        Private Shared Function GetTokens(str As String) As String()
            If String.IsNullOrEmpty(str) Then
                Return {""}
            End If

            Dim Tokens = Regex.Split(str, SPLIT_REGX_EXPRESSION)

            For i As Integer = 0 To Tokens.Length - 1
                Dim s As String = Tokens(i)
                If s.First = QUOT AndAlso s.Last = QUOT Then    '消除单词单元中的双引号
                    Tokens(i) = Mid(s, 2, Len(s) - 2)
                End If
            Next

            Return Tokens
        End Function
    End Class
End Namespace