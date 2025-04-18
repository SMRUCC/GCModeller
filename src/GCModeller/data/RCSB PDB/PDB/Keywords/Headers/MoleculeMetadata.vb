#Region "Microsoft.VisualBasic::c8b0f1fdcf87017aae45183028733eae, data\RCSB PDB\PDB\Keywords\Headers\MoleculeMetadata.vb"

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

'   Total Lines: 125
'    Code Lines: 96 (76.80%)
' Comment Lines: 1 (0.80%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 28 (22.40%)
'     File Size: 4.01 KB


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
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Keywords

    Public MustInherit Class MoleculeMetadata : Inherits Keyword

        Public Property Mols As Dictionary(Of String, Properties)

        Protected lines As New List(Of String)

        Friend Overrides Sub Flush()
            Dim last As String = Nothing
            Dim tag As NamedValue(Of String)
            Dim mol As Properties = Nothing

            Mols = New Dictionary(Of String, Properties)

            For Each line As String In lines
                If InStr(line, ": ") = 0 Then
                    ' line of metadata
                    ' end of multiple line
                    If last Is Nothing Then
                        Call $"Invalid multiple line document({line}) for the metadata parser!".Warning
                    Else
                        last = last & " " & line.GetTagValue(" ", trim:=True).Value
                    End If

                    Continue For
                ElseIf last Is Nothing Then
                    Continue For
                Else
                    tag = last.GetTagValue(":", trim:=True)
                    last = line
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

End Namespace
