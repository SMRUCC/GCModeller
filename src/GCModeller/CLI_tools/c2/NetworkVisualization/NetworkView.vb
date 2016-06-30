Public Module NetworkView

    Public Function CreateOperonNetwork(File As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
        Dim NetworkFile As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New DocumentFormat.Csv.DocumentStream.File
        Call NetworkFile.AppendLine(New String() {"Regulator", "Regulation", "Operon"})
        For Each row In File.Skip(1)
            Dim regulatorId As String = Trim(row(3))
            If Not String.IsNullOrEmpty(regulatorId) Then
                Dim effect As String = Trim(row(6))
                If Not String.IsNullOrEmpty(effect) Then
                    Call NetworkFile.AppendLine(New String() {regulatorId, effect, "OperonId:" & row(0)})
                Else
                    Call NetworkFile.AppendLine(New String() {regulatorId, "regulate", "OperonId:" & row(0)})
                End If
            End If
        Next

        Return NetworkFile
    End Function

    Public Sub Action()
        For Each file In {"E:\meme_analysis_logs_result\finalView\operon_150bp.csv",
                          "E:\meme_analysis_logs_result\finalView\operon_200bp.csv",
                          "E:\meme_analysis_logs_result\finalView\operon_250bp.csv"}
            Dim fileName As String = FileIO.FileSystem.GetFileInfo(file).Name
            Call CreateOperonNetwork(file).Save("E:\meme_analysis_logs_result\network\" & fileName, False)
        Next
    End Sub
End Module
