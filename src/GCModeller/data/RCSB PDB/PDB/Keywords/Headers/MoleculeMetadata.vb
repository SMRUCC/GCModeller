#Region "Microsoft.VisualBasic::ffd5095d124380489b830ff3733c554d, data\RCSB PDB\PDB\Keywords\Headers\MoleculeMetadata.vb"

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

    '   Total Lines: 171
    '    Code Lines: 132 (77.19%)
    ' Comment Lines: 7 (4.09%)
    '    - Xml Docs: 71.43%
    ' 
    '   Blank Lines: 32 (18.71%)
    '     File Size: 5.65 KB


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
    '         Function: ToString
    ' 
    '         Sub: add
    ' 
    '     Class Source
    ' 
    '         Properties: GeneId, Keyword, Ncbi_taxid
    ' 
    '         Function: Append
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

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
                    last = line
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
                    End If

                    mol = New Properties With {
                        .id = tag.Value.Trim(";"c),
                        .metadata = New Dictionary(Of String, String)
                    }
                Else
                    mol.add(tag.Name, tag.Value.Trim(";"c))
                End If
            Next

            If Not mol Is Nothing Then
                If Not last Is Nothing Then
                    tag = last _
                        .GetTagValue(" ").Value _
                        .GetTagValue(":", trim:=True)
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

        Default Public ReadOnly Property GetString(key As String) As String
            Get
                Return metadata.TryGetValue(key, [default]:="")
            End Get
        End Property

        Public Sub add(key As String, value As String)
            Call metadata.Add(key, value)
        End Sub

        Public Overrides Function ToString() As String
            Return $"{id} - {metadata.Keys.GetJson}"
        End Function

    End Class

    Public Class Source : Inherits MoleculeMetadata

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_SOURCE
            End Get
        End Property

        ''' <summary>
        ''' get gene id
        ''' </summary>
        ''' <param name="mol"></param>
        ''' <returns></returns>
        Public ReadOnly Property GeneId(Optional mol As String = Nothing) As String()
            Get
                If mol Is Nothing Then
                    If Mols.Count = 1 Then
                        Return Mols.First.Value!GENE.StringSplit(",\s+")
                    Else
                        Return Mols _
                            .Select(Function(a) a.Value!GENE.StringSplit(",\s+")) _
                            .IteratesALL _
                            .Distinct _
                            .ToArray
                    End If
                ElseIf Mols.ContainsKey(mol) Then
                    Return Mols(mol)!GENE.StringSplit(",\s+")
                Else
                    Throw New KeyNotFoundException
                End If
            End Get
        End Property

        Public ReadOnly Property Ncbi_taxid(Optional mol As String = Nothing) As String
            Get
                If mol Is Nothing Then
                    If Mols.Count = 1 Then
                        Return Mols.First.Value!ORGANISM_TAXID
                    Else
                        For Each moldata As Properties In Mols.Values
                            If moldata.metadata.ContainsKey("ORGANISM_TAXID") Then
                                Return moldata!ORGANISM_TAXID
                            End If
                        Next

                        Return Nothing
                    End If
                ElseIf Mols.ContainsKey(mol) Then
                    Return Mols(mol)!ORGANISM_TAXID
                Else
                    Throw New KeyNotFoundException
                End If
            End Get
        End Property

        Public ReadOnly Property ScientificName(Optional mol As String = Nothing) As String
            Get
                If mol Is Nothing Then
                    If Mols.Count = 1 Then
                        Return Mols.First.Value!ORGANISM_SCIENTIFIC
                    Else
                        For Each moldata As Properties In Mols.Values
                            If moldata.metadata.ContainsKey("ORGANISM_SCIENTIFIC") Then
                                Return moldata!ORGANISM_SCIENTIFIC
                            End If
                        Next

                        Return Nothing
                    End If
                ElseIf Mols.ContainsKey(mol) Then
                    Return Mols(mol)!ORGANISM_SCIENTIFIC
                Else
                    Throw New KeyNotFoundException
                End If
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
