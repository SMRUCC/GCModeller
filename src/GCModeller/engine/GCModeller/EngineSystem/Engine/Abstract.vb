Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.Configuration
Imports Microsoft.VisualBasic.Logging

Namespace EngineSystem.Engine

    Public Interface IContainerSystemRuntimeEnvironment
        ReadOnly Property SystemLogging As LogFile
        ReadOnly Property RuntimeTicks As Long
        ReadOnly Property SystemVariable(var As String) As String
        ReadOnly Property ConfigurationData As ConfigReader

        Sub MemoryDump(DumpFile As String)

        ''' <summary>
        ''' 从最开始输入程序的命令行之中获取目标开关的参数
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetArguments(Name As String) As String
    End Interface
End Namespace