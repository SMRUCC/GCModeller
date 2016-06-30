
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports PlugIn

<[PackageNamespace]("GCModeller.Workbench.Plugins.ModelEditor", Publisher:="xie.guigang@gcmodeller.org")>
<PlugIn.PlugInEntry(Name:="Model Editor", Description:="")>
Public Module PlugInEntry

    <ExportAPI("Open.ModelEditor")>
    <PlugIn.PlugInCommand(Name:="Open Model Editor")>
    Public Function OpenEditor() As Integer
        Dim Editor = New ModelEditor()
        If Editor.OpenModel() = True Then
            Call Editor.ShowDialog()
        End If

        Return 0
    End Function
End Module
