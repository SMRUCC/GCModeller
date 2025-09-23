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

        Friend Shared Function Append(ByRef hetname As HetName, line As String) As HetName
            If hetname Is Nothing Then
                hetname = New HetName
            End If

            Dim residueType = Strings.Mid(line, 1, 3).Trim()
            Dim chemicalName = Strings.Mid(line, 5).Trim()
            Dim data As New NamedValue(Of String)(residueType, chemicalName, line)

            Call hetname.residueList.Add(data)

            Return hetname
        End Function

    End Class
End Namespace