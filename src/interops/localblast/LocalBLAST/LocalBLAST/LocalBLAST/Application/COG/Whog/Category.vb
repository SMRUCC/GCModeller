Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language

Namespace LocalBLAST.Application.RpsBLAST.Whog

    Public Class Category

        <XmlAttribute> Public Property CategoryId As String
        <XmlAttribute> Public Property CogId As String
        <XmlElement> Public Property Description As String

        <XmlElement> Public Property IdList As KeyValuePair()
            Get
                Return _IdList
            End Get
            Set(value As KeyValuePair())
                If value Is Nothing Then
                    Return
                End If

                Dim LQuery = (From item In value Select New KeyValuePair(Of String, String())(item.Key, item.Value.Split)).ToArray
                _IdList = value
                IdTokens = LQuery
                _lstLocus = (From item In LQuery Let IdList As String() = item.Value Select IdList).ToArray.MatrixToVector
            End Set
        End Property

        Dim _IdList As KeyValuePair()
        Dim IdTokens As KeyValuePair(Of String, String())()
        Dim _lstLocus As String()

        Const REGX_CATAGORY As String = "\[[^]]+\]"
        Const REGX_COG_ID As String = "COG\d+"

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1} --> {2}", CategoryId, CogId, Description)
        End Function

        Public Function ContainsGene(id As String) As Boolean
            Return Array.IndexOf(_lstLocus, id) > -1
        End Function

        Protected Friend Shared Function Parse(srcText As String) As Category
            Dim item As Category = New Category
            Dim Tokens As String() = Strings.Split(srcText, vbLf)
            Dim description As String = Tokens.First

            item.CategoryId = Regex.Match(description, REGX_CATAGORY).Value
            item.CategoryId = Mid(item.CategoryId, 2, Len(item.CategoryId) - 2)
            item.CogId = Regex.Match(description, REGX_COG_ID).Value
            item.Description = Mid(description, Len(item.CategoryId) + Len(item.CogId) + 4).Trim
            Dim list As List(Of KeyValuePair) = New List(Of KeyValuePair)

            For Each line As String In Tokens.Skip(1)
                Dim sss = Strings.Split(line, ":")
                If sss.Count = 2 Then
                    list += New KeyValuePair With {
                        .Key = sss(0).TrimA,
                        .Value = sss(1).Trim
                    }
                Else
                    list.Last.Value &= " " & Trim(line)
                End If
            Next

            item.IdList = list.ToArray

            Return item
        End Function

        Public Function Find(Id As String) As String
            If IdTokens Is Nothing Then
                Return ""
            End If

            Dim LQuery As String =
                LinqAPI.DefaultFirst(Of String) <= From item
                                                   In IdTokens
                                                   Where Array.IndexOf(item.Value, Id) > -1
                                                   Select item.Key
            Return LQuery
        End Function
    End Class
End Namespace