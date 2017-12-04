Imports System.Reflection
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.ApplicationServices

''' <summary>
''' A list of GCModeller apps 
''' </summary>
Public NotInheritable Class Apps

    Public Shared ReadOnly Property NCBI_tools As GCModellerApps.NCBI_tools
    Public Shared ReadOnly Property localblast As GCModellerApps.localblast
    Public Shared ReadOnly Property MEME As GCModellerApps.MEME
    Public Shared ReadOnly Property KEGG_tools As GCModellerApps.KEGG_tools
    Public Shared ReadOnly Property seqtools As GCModellerApps.seqtools
    Public Shared ReadOnly Property VennDiagram As GCModellerApps.VennDiagram
    Public Shared ReadOnly Property eggHTS As GCModellerApps.eggHTS

    Shared Sub New()
        Dim list$() = {
            App.HOME,
            App.HOME & "/GCModeller/",
            App.HOME & "/Apps/",
            "C:\GCModeller\",
            "C:\Program Files\GCModeller\",
            "D:\GCModeller\",
            "E:\GCModeller\",
            "F:\GCModeller\",
            "G:\GCModeller\"
        } ' 枚举一些比较常用的路径

        For Each HOME As String In list
            If Apps.IsAppHome(HOME) Then
                eggHTS = New GCModellerApps.eggHTS(HOME & "/eggHTS.exe")
                NCBI_tools = New GCModellerApps.NCBI_tools(HOME & "/NCBI_tools.exe")
                localblast = New GCModellerApps.localblast(HOME & "/localblast.exe")
                MEME = New GCModellerApps.MEME(HOME & "/MEME.exe")
                KEGG_tools = New GCModellerApps.KEGG_tools(HOME & "/KEGG_tools.exe")
                seqtools = New GCModellerApps.seqtools(HOME & $"/{NameOf(seqtools)}.exe")
                VennDiagram = New GCModellerApps.VennDiagram(HOME & "/venn.exe")

                Exit For
            End If
        Next
    End Sub

    ''' <summary>
    ''' 目标文件夹是不是GCModeller Apps的应用程序组文件夹？
    ''' </summary>
    ''' <param name="DIR$"></param>
    ''' <returns></returns>
    Private Shared Function IsAppHome(DIR$) As Boolean
        Dim Apps$() = GetType(Apps) _
            .GetProperties(BindingFlags.Public Or BindingFlags.Static) _
            .Where(Function(t) t.PropertyType.IsInheritsFrom(GetType(InteropService))) _
            .Select(Function(t) t.PropertyType.GetMember("App")) _
            .IteratesALL _
            .Select(Function(m) DirectCast(m, FieldInfo).GetValue(Nothing)) _
            .Select(Function(o) DirectCast(o, String)) _
            .ToArray

        For Each App$ In Apps.Select(Function(path$) DIR & "/" & path)
            If App.FileExists(True) Then
                Return True
            End If
        Next

        Return False
    End Function
End Class
