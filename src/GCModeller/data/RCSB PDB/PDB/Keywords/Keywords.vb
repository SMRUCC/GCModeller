Namespace Keywords

    Public MustInherit Class Keyword

        Public Const KEYWORD_HEADER As String = "HEADER"
        Public Const KEYWORD_TITLE As String = "TITLE"
        Public Const KEYWORD_COMPND As String = "COMPND"
        Public Const KEYWORD_SOURCE As String = "SOURCE"
        Public Const KEYWORD_KEYWDS As String = "KEYWDS"
        Public Const KEYWORD_AUTHOR As String = "AUTHOR"
        Public Const KEYWORD_REVDAT As String = "REVDAT"
        Public Const KEYWORD_JRNL As String = "JRNL"
        Public Const KEYWORD_REMARK As String = "REMARK"
        Public Const KEYWORD_EXPDTA As String = "EXPDTA"
        Public Const KEYWORD_SEQRES As String = "SEQRES"
        Public Const KEYWORD_HET As String = "HET"
        Public Const KEYWORD_HETNAM As String = "HETNAM"
        Public Const KEYWORD_FORMUL As String = "FORMUL"
        Public Const KEYWORD_HELIX As String = "HELIX"
        Public Const KEYWORD_SHEET As String = "SHEET"
        Public Const KEYWORD_SITE As String = "SITE"
        Public Const KEYWORD_CRYST1 As String = "CRYST1"
        Public Const KEYWORD_ATOM As String = "ATOM"
        Public Const KEYWORD_HETATM As String = "HETATM"
        Public Const KEYWORD_CONECT As String = "CONECT"
        Public Const KEYWORD_MASTER As String = "MASTER"
        Public Const KEYWORD_DBREF As String = "DBREF"

        Public MustOverride ReadOnly Property Keyword As String

        Protected Friend _originalData As KeyValuePair(Of Integer, String)()

        Protected Friend Sub New(itemDatas As KeyValuePair(Of Integer, String)())
            Me._originalData = itemDatas
        End Sub

        Protected Friend Shared Function GetData(Keyword As String, strData As KeyValuePair(Of String, String)()) As KeyValuePair(Of Integer, String)()
            Dim LQuery = (From item As KeyValuePair(Of String, String) In strData.AsParallel
                          Let Tokens As String() = (From s As String In item.Key.Split Where Not String.IsNullOrEmpty(s) Select s).ToArray
                          Where String.Equals(Keyword, Tokens.First)
                          Let itemData = New KeyValuePair(Of Integer, String)(Val(Tokens.Last), item.Value)
                          Select itemData
                          Order By itemData.Key Ascending).ToArray
            Return LQuery
        End Function

        Public Overrides Function ToString() As String
            Return Keyword
        End Function
    End Class
End Namespace