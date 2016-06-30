Imports Microsoft.VisualBasic.Terminal.stdio

''' <summary>
''' GCModeller模型文件编译工具，主要的工作为将一个MetaCyc数据库编译为一个GCML模型文件，以及根据建模项目文件编译模型文件
''' </summary>
''' <remarks>
''' </remarks>
Module Program

    Public Function Main() As Integer
        Return GetType(CLI).RunCLI(App.CommandLine)
    End Function

    Sub New()
        Call Settings.Session.Initialize()
        Call FileIO.FileSystem.CreateDirectory(Settings.LogDIR)
    End Sub
End Module
