#Region "Microsoft.VisualBasic::8b8fa8c2ef5d58a8ba5453a5da47125f, RCSB PDB\PDB\Keywords\Header.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Class Header
    ' 
    '         Properties: [Date], Keyword, pdbID, Title
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class Title
    ' 
    '         Properties: Keyword
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class Compound
    ' 
    '         Properties: Keyword
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class Source
    ' 
    '         Properties: Keyword
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class Keywords
    ' 
    '         Properties: Keyword
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class Author
    ' 
    '         Properties: Keyword
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class Journal
    ' 
    '         Properties: Keyword
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class Remark
    ' 
    '         Properties: Keyword
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class DbReference
    ' 
    '         Properties: Keyword
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class Sequence
    ' 
    '         Properties: Keyword
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class Helix
    ' 
    '         Properties: Keyword
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class Sheet
    ' 
    '         Properties: Keyword
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class Site
    ' 
    '         Properties: Keyword
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class Master
    ' 
    '         Properties: Keyword
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
