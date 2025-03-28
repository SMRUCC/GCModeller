﻿#Region "Microsoft.VisualBasic::e6ad5bd331452203eea231a7e8393d94, data\RCSB PDB\PDB\Keywords\Header.vb"

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

    '   Total Lines: 291
    '    Code Lines: 220 (75.60%)
    ' Comment Lines: 1 (0.34%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 70 (24.05%)
    '     File Size: 8.58 KB


    '     Class Header
    ' 
    '         Properties: [Date], Keyword, pdbID, Title
    ' 
    '         Function: Parse
    ' 
    '     Class Title
    ' 
    '         Properties: Keyword, Title
    ' 
    '         Function: Append
    ' 
    '     Class MoleculeMetadata
    ' 
    '         Properties: Mols
    ' 
    '         Sub: Flush
    ' 
    '     Class Compound
    ' 
    '         Properties: Keyword
    ' 
    '         Function: Append
    ' 
    '     Class Properties
    ' 
    '         Properties: id, metadata
    ' 
    '         Sub: add
    ' 
    '     Class Source
    ' 
    '         Properties: Keyword
    ' 
    '         Function: Append
    ' 
    '     Class Keywords
    ' 
    '         Properties: Keyword, keywords
    ' 
    '         Function: Parse
    ' 
    '     Class ExperimentData
    ' 
    '         Properties: Experiment, Keyword
    ' 
    '         Function: Parse
    ' 
    '     Class Author
    ' 
    '         Properties: Keyword, Name
    ' 
    '         Function: Parse
    ' 
    '     Class Journal
    ' 
    '         Properties: Keyword
    ' 
    '     Class Remark
    ' 
    '         Properties: Keyword
    ' 
    '     Class DbReference
    ' 
    '         Properties: Keyword
    ' 
    '     Class Sequence
    ' 
    '         Properties: Keyword
    ' 
    '     Class Helix
    ' 
    '         Properties: Keyword
    ' 
    '     Class Sheet
    ' 
    '         Properties: Keyword
    ' 
    '     Class Site
    ' 
    '         Properties: Keyword
    ' 
    '     Class Master
    ' 
    '         Properties: Keyword
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

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

    Public MustInherit Class MoleculeMetadata : Inherits Keyword

        Public Property Mols As Dictionary(Of String, Properties)

        Protected lines As New List(Of String)

        Friend Overrides Sub Flush()
            Dim last As String = Nothing
            Dim tag As NamedValue(Of String)
            Dim mol As Properties = Nothing

            Mols = New Dictionary(Of String, Properties)

            For Each line As String In lines
                If line.Last <> ";"c Then
                    If last Is Nothing Then
                        last = line
                    Else
                        last = last & " " & line.GetTagValue(" ", trim:=True).Value
                    End If

                    Continue For
                ElseIf InStr(line, ": ") = 0 Then
                    ' end of multiple line
                    If last Is Nothing Then
                        Throw New InvalidProgramException("invalid multiple line document for the metadata parser!")
                    Else
                        last = last & " " & line.GetTagValue(" ", trim:=True).Value
                        tag = last.GetTagValue(":", trim:=True)
                        last = Nothing
                    End If
                Else
                    tag = line.GetTagValue(":", trim:=True)
                End If

                If tag.Name.IsPattern("\d+ [^\s]+") Then
                    tag = New NamedValue(Of String)(tag.Name.GetTagValue(" ").Value, tag.Value)
                End If

                If tag.Name = "MOL_ID" Then
                    If Not mol Is Nothing Then
                        Mols.Add(mol.id, mol)
                        last = Nothing
                    End If

                    mol = New Properties With {
                        .id = tag.Value,
                        .metadata = New Dictionary(Of String, String)
                    }
                Else
                    mol.add(tag.Name, tag.Value)
                End If
            Next

            If Not mol Is Nothing Then
                If Not last Is Nothing Then
                    tag = last.GetTagValue(":", trim:=True)
                    mol.add(tag.Name, tag.Value)
                End If

                Mols.Add(mol.id, mol)
            End If
        End Sub
    End Class

    Public Class Compound : Inherits MoleculeMetadata

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_COMPND
            End Get
        End Property

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
        Public Property id As String

        Public Sub add(key As String, value As String)
            Call metadata.Add(key, value)
        End Sub

    End Class

    Public Class Source : Inherits MoleculeMetadata

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_SOURCE
            End Get
        End Property

        Friend Shared Function Append(ByRef src As Source, str As String) As Source
            If src Is Nothing Then
                src = New Source With {
                    .lines = New List(Of String) From {str}
                }
            Else
                src.lines.Add(str)
            End If

            Return src
        End Function

    End Class

    Public Class Keywords : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return RCSB.PDB.Keywords.Keyword.KEYWORD_KEYWDS
            End Get
        End Property

        Public Property keywords As String()

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

        Friend Shared Function Parse(line As String) As Author
            Return New Author With {.Name = line}
        End Function

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
