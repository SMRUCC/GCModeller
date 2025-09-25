Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Keywords

    Public Class HetName : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return KEYWORD_HETNAM
            End Get
        End Property

        Public ReadOnly Property Residues As NamedValue(Of String)()
            Get
                Return residueList.ToArray
            End Get
        End Property

        ReadOnly residueList As New List(Of NamedValue(Of String))

        Sub New()
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(ParamArray ligands As NamedValue(Of String)())
            Call residueList.AddRange(ligands)
        End Sub

        Public Function ToPDBText() As String
            Dim sb As New StringBuilder
            For Each residue As NamedValue(Of String) In residueList
                Call sb.AppendLine($"HETNAM     {residue.Name} {residue.Value}")
            Next
            Return sb.ToString
        End Function

        ''' <summary>
        ''' append a new line of hetname record to the current hetname object
        ''' </summary>
        ''' <param name="hetname"></param>
        ''' <param name="line"></param>
        ''' <returns></returns>
        Friend Shared Function Append(ByRef hetname As HetName, line As String) As HetName
            If hetname Is Nothing Then
                hetname = New HetName
            End If

            line = Strings.LTrim(line)

            Dim residueType = Strings.Mid(line, 1, 3).Trim()
            Dim chemicalName = Strings.Mid(line, 5).Trim()
            Dim data As New NamedValue(Of String)(residueType, chemicalName, line)

            Call hetname.residueList.Add(data)

            Return hetname
        End Function

    End Class
End Namespace