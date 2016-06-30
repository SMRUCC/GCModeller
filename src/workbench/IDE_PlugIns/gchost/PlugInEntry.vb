Imports Microsoft.VisualBasic
Imports PlugIn

<PlugIn.PlugInEntry(name:="测试", description:="")>
Module PlugInEntry

    Public IDEProxy As LANS.SystemsBiology.GCModeller.Workbench.MultipleTabpageProxy
    Public IDEInstance As LANS.SystemsBiology.GCModeller.Workbench.IDEInstance

    <PlugIn.PlugInCommand(name:="运行")>
    Public Function Run() As Integer
        Throw New NotImplementedException
    End Function

    <PlugIn.PlugInCommand(name:="打开控制面板")>
    Public Function OpenPanel() As Integer
        Call IDEProxy.AddTabPage(Of Global.gchost.GcHostControlPanel)("GCModeller Host")
        Return 0
    End Function

    <PlugIn.EntryFlag(entrytype:=EntryFlag.EntryTypes.Initialize)>
    Public Function Initialize(Target As Global.LANS.SystemsBiology.GCModeller.Workbench.FormMain) As Integer
        PlugInEntry.IDEProxy = Target.TabpageProxy
        PlugInEntry.IDEInstance = Target.IDEInstance
        Return 0
    End Function
End Module
