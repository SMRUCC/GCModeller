Namespace Keywords

    Public Class Header : Inherits Keyword

        Public Property pdbID As String
        Public Property [Date] As String
        Public Shadows Property Title As String

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_HEADER
            End Get
        End Property

        Sub New(itemDatas As KeyValuePair(Of Integer, String)())
            Call MyBase.New(itemDatas)

        End Sub
    End Class

    Public Class Title : Inherits Keyword
        Sub New(itemDatas As KeyValuePair(Of Integer, String)())
            Call MyBase.New(itemDatas)

        End Sub

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_TITLE
            End Get
        End Property
    End Class

    Public Class Compound : Inherits Keyword

        Sub New(itemDatas As KeyValuePair(Of Integer, String)())
            Call MyBase.New(itemDatas)

        End Sub

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_COMPND
            End Get
        End Property
    End Class

    Public Class Source : Inherits Keyword

        Sub New(itemDatas As KeyValuePair(Of Integer, String)())
            Call MyBase.New(itemDatas)

        End Sub

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_SOURCE
            End Get
        End Property
    End Class

    Public Class Keywords : Inherits Keyword

        Sub New(itemDatas As KeyValuePair(Of Integer, String)())
            Call MyBase.New(itemDatas)

        End Sub

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_KEYWDS
            End Get
        End Property
    End Class

    Public Class Author : Inherits Keyword

        Sub New(itemDatas As KeyValuePair(Of Integer, String)())
            Call MyBase.New(itemDatas)

        End Sub

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_AUTHOR
            End Get
        End Property
    End Class

    Public Class Journal : Inherits Keyword

        Sub New(itemDatas As KeyValuePair(Of Integer, String)())
            Call MyBase.New(itemDatas)

        End Sub

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_JRNL
            End Get
        End Property
    End Class

    Public Class Remark : Inherits Keyword

        Sub New(itemDatas As KeyValuePair(Of Integer, String)())
            Call MyBase.New(itemDatas)

        End Sub

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_REMARK
            End Get
        End Property
    End Class

    Public Class DbReference : Inherits Keyword

        Sub New(itemDatas As KeyValuePair(Of Integer, String)())
            Call MyBase.New(itemDatas)

        End Sub

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_DBREF
            End Get
        End Property
    End Class

    Public Class Sequence : Inherits Keyword

        Sub New(itemDatas As KeyValuePair(Of Integer, String)())
            Call MyBase.New(itemDatas)

        End Sub

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_SEQRES
            End Get
        End Property
    End Class

    Public Class Helix : Inherits Keyword

        Sub New(itemDatas As KeyValuePair(Of Integer, String)())
            Call MyBase.New(itemDatas)

        End Sub

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_HELIX
            End Get
        End Property
    End Class

    Public Class Sheet : Inherits Keyword

        Sub New(itemDatas As KeyValuePair(Of Integer, String)())
            Call MyBase.New(itemDatas)

        End Sub

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_SHEET
            End Get
        End Property
    End Class

    Public Class Site : Inherits Keyword

        Sub New(itemDatas As KeyValuePair(Of Integer, String)())
            Call MyBase.New(itemDatas)

        End Sub

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_SITE
            End Get
        End Property
    End Class

    Public Class Master : Inherits Keyword

        Sub New(itemDatas As KeyValuePair(Of Integer, String)())
            Call MyBase.New(itemDatas)

        End Sub

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_MASTER
            End Get
        End Property
    End Class
End Namespace