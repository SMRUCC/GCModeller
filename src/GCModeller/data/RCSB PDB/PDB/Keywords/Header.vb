#Region "Microsoft.VisualBasic::4152fc0d72667c992837985db6ac8f6e, ..\GCModeller\data\RCSB PDB\PDB\Keywords\Header.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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
