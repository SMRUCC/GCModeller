#Region "Microsoft.VisualBasic::4152fc0d72667c992837985db6ac8f6e, data\RCSB PDB\PDB\Keywords\Header.vb"

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
        Public Property Title As String

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_HEADER
            End Get
        End Property

        Friend Shared Function Parse(line As String) As Header
            Dim str = line.StringSplit("\s+")
            Dim header As New Header With {
                .pdbID = str(str.Length - 1),
                .[Date] = str(str.Length - 2),
                .Title = str.Take(str.Length - 2).JoinBy(" ")
            }

            Return header
        End Function

    End Class

    Public Class Title : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_TITLE
            End Get
        End Property

        Public Property Title As String

        Friend Shared Function Append(ByRef title As Title, str As String) As Title
            If title Is Nothing Then
                title = New Title With {.Title = str}
            Else
                title.Title = title.Title & " " & str.GetTagValue(" ", trim:=True).Value
            End If

            Return title
        End Function

    End Class

    Public Class Compound : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_COMPND
            End Get
        End Property

        Public Property Mols As Dictionary(Of String, Properties)

        Dim lines As New List(Of String)

        Friend Overrides Sub Flush()
            Mols = New Dictionary(Of String, Properties)

            For Each line As String In lines

            Next
        End Sub

        Friend Shared Function Append(ByRef compound As Compound, str As String) As Compound
            If compound Is Nothing Then
                compound = New Compound With {
                    .lines = New List(Of String) From {str}
                }
            Else
                compound.lines.Add(str)
            End If

            Return compound
        End Function

    End Class

    Public Class Properties

        Public Property metadata As Dictionary(Of String, String)

    End Class

    Public Class Source : Inherits Keyword
        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_SOURCE
            End Get
        End Property
    End Class

    Public Class Keywords : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_KEYWDS
            End Get
        End Property
    End Class

    Public Class Author : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_AUTHOR
            End Get
        End Property
    End Class

    Public Class Journal : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_JRNL
            End Get
        End Property
    End Class

    Public Class Remark : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_REMARK
            End Get
        End Property
    End Class

    Public Class DbReference : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_DBREF
            End Get
        End Property
    End Class

    Public Class Sequence : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_SEQRES
            End Get
        End Property
    End Class

    Public Class Helix : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_HELIX
            End Get
        End Property
    End Class

    Public Class Sheet : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_SHEET
            End Get
        End Property
    End Class

    Public Class Site : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_SITE
            End Get
        End Property
    End Class

    Public Class Master : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_MASTER
            End Get
        End Property
    End Class
End Namespace
