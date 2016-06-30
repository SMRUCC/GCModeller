Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic

Public Class CifDictionary

    Public Property Sections As Section()

    Public Class Section

        Public Property KeyValuePairs As KeyValuePair(Of String, String)()

        Const SPLIT_REGX As String = "(.+)?"

        Protected Friend Shared Function TryParse(strData As String) As Section
            Dim Tokens As String() = (From strLine As String In Strings.Split(strData, vbLf)
                                      Let str As String = strLine.TrimA
                                      Where Not String.IsNullOrEmpty(str)
                                      Select str).ToArray
            Dim PairList As List(Of KeyValuePair(Of String, String)) =
                New List(Of KeyValuePair(Of String, String))
            For Each item As String In Tokens

            Next
            Throw New NotImplementedException
        End Function
    End Class

    Public Shared Function Load(Path As String) As CifDictionary
        Dim FileContent As String = FileIO.FileSystem.ReadAllText(Path)
        Dim Tokens As String() = Strings.Split(FileContent, "# ")
        Dim LQuery = (From strData As String In Tokens Select Section.TryParse(strData)).ToArray

        Dim CifDict As CifDictionary = New CifDictionary
        CifDict.Sections = LQuery
        Return CifDict
    End Function
End Class
