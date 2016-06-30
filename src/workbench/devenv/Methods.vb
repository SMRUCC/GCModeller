Module Methods

    Public Function SelectFolder() As String
        Using Folder As New System.Windows.Forms.FolderBrowserDialog
            If Folder.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                Return Folder.SelectedPath
            Else
                Return ""
            End If
        End Using
    End Function
End Module
