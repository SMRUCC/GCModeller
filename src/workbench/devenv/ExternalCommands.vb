Imports Microsoft.VisualBasic.ConsoleDevice.STDIO

Module ExternalCommands

    Private ReadOnly Reengineering As String = My.Application.Info.DirectoryPath & "/c2.exe"

    Public Sub Build(File As String, SaveFile As String)
        Dim FileFormat As String
        If String.Equals(System.IO.Path.GetExtension(File), ".gbk") Then
            FileFormat = "-f gbk"
        Else
            FileFormat = "-f fsa"
        End If
        Dim Arguments As String = String.Format("build -i ""{0}"" -o ""{1}"" {2} -p T", File, SaveFile, FileFormat)
        Out(String.Format("Start external command to build a fasta sequence database:{0}{1}", vbCrLf,
                          String.Format("{0} {1}", Reengineering, Arguments)), ConsoleColor.Green, "LocalBLAST")
        Call Microsoft.VisualBasic.Interaction.Shell(Format("%s %s", Reengineering, Arguments))
    End Sub
End Module
