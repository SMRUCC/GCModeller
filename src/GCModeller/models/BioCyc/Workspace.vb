Imports System.IO

Public Class Workspace

    ReadOnly dir As String

    Dim m_reactions As AttrDataCollection(Of reactions)

    Public ReadOnly Property reactions As AttrDataCollection(Of reactions)
        Get
            If m_reactions Is Nothing Then
                Using file As Stream = "".Open()

                End Using
                m_reactions = AttrDataCollection(Of reactions).LoadFile()
            End If
        End Get
    End Property

    Sub New(dir As String)
        Me.dir = dir.GetDirectoryFullPath
    End Sub

    Public Overrides Function ToString() As String
        Return dir
    End Function

End Class
