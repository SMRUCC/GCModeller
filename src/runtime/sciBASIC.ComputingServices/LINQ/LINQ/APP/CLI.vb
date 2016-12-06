Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq.Framework.Provider.ImportsAPI
Imports Microsoft.VisualBasic.Linq.Framework.Provider

''' <summary>
''' 框架程序自带的注册模块以及一些配置的管理终端
''' </summary>
Module CLI

    ''' <summary>
    ''' 扫描应用程序文件夹之中可能的插件信息
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/install", Usage:="/install")>
    Public Function InstallPlugins(args As CommandLine.CommandLine) As Integer
        Using registry As TypeRegistry = TypeRegistry.LoadDefault
            Call registry.InstallCurrent()
        End Using
        Using api As APIProvider = APIProvider.LoadDefault
            Call api.Install()
        End Using

        Return 0
    End Function
End Module
