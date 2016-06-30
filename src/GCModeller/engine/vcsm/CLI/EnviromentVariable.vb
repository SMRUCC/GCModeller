Imports Microsoft.VisualBasic.CommandLine.Reflection

Partial Module CommandLines

    ' ''' <summary>
    ' ''' 
    ' ''' </summary>
    ' ''' <param name="CommandLine"></param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    '<Command("matrix", info:="", usage:="matrix <systemTarget> <- <matrix_file>", example:="")>
    '<ParameterDescription("<systemTarget>", optional:=False,
    '    description:="Set up the matrix profile data file for the sub system module calculation parameter," & vbCrLf &
    '                    "Metabolism, ExpressionNetwork")>
    'Public Shared Function Matrix(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
    '    Using MatrixFile As Microsoft.VisualBasic.ComponentModel.Settings.Settings(Of LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.ObjectModels.System.Matrix.MatrixFile) =
    '        Microsoft.VisualBasic.ComponentModel.Settings.Settings(Of LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.ObjectModels.System.Matrix.MatrixFile).LoadFile(Program.MatrixFile)
    '        Dim SystemTarget As String = CommandLine.Parameters.First
    '        Dim FilePath As String = CommandLine.Parameters(2)

    '        Call MatrixFile.Set(SystemTarget, FilePath)   '向文件之中写入配置数据
    '    End Using
    '    Return 0
    'End Function
End Module