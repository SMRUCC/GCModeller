Imports Microsoft.VisualBasic.CommandLine
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports LANS.SystemsBiology.Assembly

Module Program

    Public ReadOnly Property Logs As String = Settings.LogDIR & "/c2.log"

    Public Function Main(arguments As String()) As Integer
        Using LogFile As Microsoft.VisualBasic.Logging.LogFile =
              New Microsoft.VisualBasic.Logging.LogFile(Logs)
            LogFile.WriteLine(App.Command, "SUB_TMAIN()", Microsoft.VisualBasic.Logging.MSG_TYPES.INF, False)
        End Using

        Try
            Call Settings.Initialize()
            Return GetType(CommandLines).RunCLI(App.Command, AddressOf DisplayInfo)
        Catch ex As Exception
            Using LogFile As Microsoft.VisualBasic.Logging.LogFile =
                New Microsoft.VisualBasic.Logging.LogFile(Logs)
                LogFile.WriteLine(ex.ToString, "Program_Unhandle_Exception", Microsoft.VisualBasic.Logging.MSG_TYPES.ERR)
            End Using
            Console.WriteLine("An Unexcept exception happen while execute the command line, check the log file for error information:{0}  {1}", vbCrLf, Logs)
            Return -1
        End Try
    End Function

    Private Function DisplayInfo() As Integer
        Dim sBuilder As StringBuilder = New StringBuilder(1024)

        sBuilder.AppendLine("Program 'c2.exe' is a alternative compiler and MetaCyc database reconstruct tool for the 'Genetic Clock Model'." & vbCrLf)
        sBuilder.AppendLine("All of the command that available in this program has been list below:")
        For Each commandInfo As EntryPoints.APIEntryPoint In Interpreter.CreateInstance(GetType(CommandLines)).ListCommandInfo
            sBuilder.AppendLine(String.Format(" {0}:  {1}", commandInfo.Name, commandInfo.Info))
        Next
        sBuilder.AppendLine(vbCrLf & "Using command 'c2 ? <commandName>' for more detail help information.")

        sBuilder.AppendLine(vbCrLf & "c2 variables are list below:")
        sBuilder.AppendLine(String.Format("  blastbin = '{0}'", Settings.GetSettings("blastbin")))
        sBuilder.AppendLine(String.Format("  blastdb  = '{0}'", Settings.GetSettings("blastdb")))
        ' sBuilder.AppendLine(String.Format("  formatdb = '{0}'", Profile.LocalBlast.Bin & "/formatdb [Executable Assembly]"))

        Call sBuilder.ToString.__DEBUG_ECHO

        Return 0
    End Function
End Module
