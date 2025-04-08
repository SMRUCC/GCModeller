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

End Namespace