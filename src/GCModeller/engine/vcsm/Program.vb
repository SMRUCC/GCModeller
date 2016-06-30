Imports Microsoft.VisualBasic.CommandLine.Reflection

''' <summary>
''' Virtual cell engine program main entry.
''' </summary>
Module Program

    Public ReadOnly Property Settings As Settings.File =
        Global.LANS.SystemsBiology.GCModeller.Settings.Session.Initialize

    ''' <summary>
    ''' 计算框架的外部系统模块的注册表
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Property ExternalModuleRegistry As LANS.SystemsBiology.GCModeller.ModellingEngine.PlugIns.ModuleRegistry =
        LANS.SystemsBiology.GCModeller.ModellingEngine.PlugIns.ModuleRegistry.Load(LANS.SystemsBiology.GCModeller.ModellingEngine.PlugIns.ModuleRegistry.XmlFile)

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Function Main() As Integer
        Return GetType(CommandLines).RunCLI(App.CommandLine)
    End Function
End Module