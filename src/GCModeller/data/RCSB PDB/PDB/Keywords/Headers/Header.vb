﻿#Region "Microsoft.VisualBasic::693c03baa5cc25e9e841c959b15de969, data\RCSB PDB\PDB\Keywords\Headers\Header.vb"

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


    ' Code Statistics:

    '   Total Lines: 228
    '    Code Lines: 170 (74.56%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 58 (25.44%)
    '     File Size: 6.50 KB


    '     Class Header
    ' 
    '         Properties: [Date], Keyword, pdbID, Title
    ' 
    '         Function: Parse, ToString
    ' 
    '     Class Title
    ' 
    '         Properties: Keyword, Title
    ' 
    '         Function: Append, ToString
    ' 
    '     Class Keywords
    ' 
    '         Properties: Keyword, keywords
    ' 
    '         Function: Parse, ToString
    ' 
    '     Class ExperimentData
    ' 
    '         Properties: Experiment, Keyword
    ' 
    '         Function: Parse, ToString
    ' 
    '     Class Author
    ' 
    '         Properties: Keyword, Name
    ' 
    '         Function: Parse, ToString
    ' 
    '     Class DbReference
    ' 
    '         Properties: db_xrefs, Keyword, XrefIndex
    ' 
    '         Function: Append, ToString
    ' 
    '         Sub: Flush
    ' 
    '     Class Site
    ' 
    '         Properties: Keyword
    ' 
    '         Function: Append, ToString
    ' 
    '     Class Master
    ' 
    '         Properties: Keyword, line
    ' 
    '         Function: Parse, ToString
    ' 
    '     Class CRYST1
    ' 
    '         Properties: Keyword
    ' 
    '         Function: Append
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

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

        Public Overrides Function ToString() As String
            Return $"({pdbID}) {Title}"
        End Function

    End Class

    Public Class Title : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_TITLE
            End Get
        End Property

        Public Property Title As String

        Public Overrides Function ToString() As String
            Return Title
        End Function

        Friend Shared Function Append(ByRef title As Title, str As String) As Title
            If title Is Nothing Then
                title = New Title With {.Title = str}
            Else
                title.Title = title.Title & " " & str.GetTagValue(" ", trim:=True).Value
            End If

            Return title
        End Function

    End Class

    Public Class Keywords : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return RCSB.PDB.Keywords.Keyword.KEYWORD_KEYWDS
            End Get
        End Property

        Public Property keywords As String()

        Public Overrides Function ToString() As String
            Return keywords.JoinBy("; ")
        End Function

        Friend Shared Function Parse(line As String) As Keywords
            Return New Keywords With {.keywords = line.StringSplit(",\s+")}
        End Function

    End Class

    Public Class ExperimentData : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_EXPDTA
            End Get
        End Property

        Public Property Experiment As String

        Public Overrides Function ToString() As String
            Return Experiment
        End Function

        Friend Shared Function Parse(line As String) As ExperimentData
            Return New ExperimentData With {.Experiment = line}
        End Function

    End Class

    Public Class Author : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_AUTHOR
            End Get
        End Property

        Public Property Name As String

        Public Overrides Function ToString() As String
            Return Name
        End Function

        Friend Shared Function Parse(line As String) As Author
            Return New Author With {.Name = line}
        End Function

    End Class

    Public Class DbReference : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_DBREF
            End Get
        End Property

        Dim cache As New List(Of String)

        Public Property db_xrefs As NamedValue(Of String)()
        Public ReadOnly Property XrefIndex As Dictionary(Of String, String())
            Get
                Return db_xrefs _
                    .SafeQuery _
                    .GroupBy(Function(a) a.Name) _
                    .ToDictionary(Function(a) a.Key,
                                  Function(a)
                                      Return a.Values
                                  End Function)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return XrefIndex.GetJson
        End Function

        Friend Shared Function Append(ByRef ref As DbReference, str As String) As DbReference
            If ref Is Nothing Then
                ref = New DbReference
            End If
            ref.cache.Add(str)
            Return ref
        End Function

        Friend Overrides Sub Flush()

        End Sub

    End Class

    Public Class Site : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_SITE
            End Get
        End Property

        Dim str As New List(Of String)

        Public Overrides Function ToString() As String
            Return str.JoinBy(" ")
        End Function

        Friend Shared Function Append(ByRef site As Site, str As String) As Site
            If site Is Nothing Then
                site = New Site
            End If
            site.str.Append(str)
            Return site
        End Function
    End Class

    Public Class Master : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_MASTER
            End Get
        End Property

        Public Property line As String

        Public Overrides Function ToString() As String
            Return line
        End Function

        Public Shared Function Parse(str As String) As Master
            Return New Master With {
                .line = str
            }
        End Function

    End Class

    Public Class CRYST1 : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_CRYST1
            End Get
        End Property

        Dim cache As New List(Of String)

        Friend Shared Function Append(ByRef res As CRYST1, str As String) As CRYST1
            If res Is Nothing Then
                res = New CRYST1
            End If
            res.cache.Add(str)
            Return res
        End Function

    End Class
End Namespace
