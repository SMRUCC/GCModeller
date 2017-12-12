Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' A list of GCModeller apps 
''' </summary>
Public NotInheritable Class Apps

    Public Shared ReadOnly Property NCBI_tools As GCModellerApps.NCBI_tools
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return _NCBI_tools
        End Get
    End Property

    Public Shared ReadOnly Property localblast As GCModellerApps.localblast
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return _localblast
        End Get
    End Property

    Public Shared ReadOnly Property MEME As GCModellerApps.MEME
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return _MEME
        End Get
    End Property

    Public Shared ReadOnly Property KEGG_tools As GCModellerApps.KEGG_tools
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return _KEGG_tools
        End Get
    End Property

    Public Shared ReadOnly Property seqtools As GCModellerApps.seqtools
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return _seqtools
        End Get
    End Property

    Public Shared ReadOnly Property VennDiagram As GCModellerApps.VennDiagram
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return _VennDiagram
        End Get
    End Property

    Public Shared ReadOnly Property eggHTS As GCModellerApps.eggHTS
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return _eggHTS
        End Get
    End Property

    Public Shared ReadOnly Property Microbiome As GCModellerApps.meta_community
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return _Microbiome
        End Get
    End Property

    Shared ReadOnly _NCBI_tools As GCModellerApps.NCBI_tools
    Shared ReadOnly _localblast As GCModellerApps.localblast
    Shared ReadOnly _MEME As GCModellerApps.MEME
    Shared ReadOnly _KEGG_tools As GCModellerApps.KEGG_tools
    Shared ReadOnly _seqtools As GCModellerApps.seqtools
    Shared ReadOnly _VennDiagram As GCModellerApps.VennDiagram
    Shared ReadOnly _eggHTS As GCModellerApps.eggHTS
    Shared ReadOnly _Microbiome As GCModellerApps.meta_community

    Const developmentPathPattern$ = "GCModeller\GCModeller\bin"

    ''' <summary>
    ''' 枚举一些比较常用的路径
    ''' </summary>
    ''' <returns></returns>
    Private Shared Function WindowsPath() As String()
        Return {
            App.HOME,
            App.HOME & "/GCModeller/",
            App.HOME & "/Apps/",
            "C:\GCModeller\",
            "C:\Program Files\GCModeller\",
            "D:\GCModeller\",
            "E:\GCModeller\",
            "F:\GCModeller\",
            "G:\GCModeller\",
            "H:\GCModeller\",
            "D:\" & developmentPathPattern, ' Development version
            "E:\" & developmentPathPattern, ' Development version
            "F:\" & developmentPathPattern, ' Development version
            "G:\" & developmentPathPattern, ' Development version
            "H:\" & developmentPathPattern  ' Development version
        }
    End Function

    Private Shared Function LinuxPath() As String()
        Return {}
    End Function

    Shared Sub New()
        Dim list$() = If(App.IsMicrosoftPlatform, WindowsPath(), LinuxPath())

        For Each HOME As String In list
            If Apps.IsAppHome(HOME) Then

                If InStr(HOME, developmentPathPattern) > 0 Then
                    Call "Using GCModeller development version.".__DEBUG_ECHO
                End If

                _eggHTS = New GCModellerApps.eggHTS(HOME & "/eggHTS.exe")
                _NCBI_tools = New GCModellerApps.NCBI_tools(HOME & "/NCBI_tools.exe")
                _localblast = New GCModellerApps.localblast(HOME & "/localblast.exe")
                _MEME = New GCModellerApps.MEME(HOME & "/MEME.exe")
                _KEGG_tools = New GCModellerApps.KEGG_tools(HOME & "/KEGG_tools.exe")
                _seqtools = New GCModellerApps.seqtools(HOME & $"/{NameOf(seqtools)}.exe")
                _VennDiagram = New GCModellerApps.VennDiagram(HOME & "/venn.exe")
                _Microbiome = New GCModellerApps.meta_community(HOME & "/" & GCModellerApps.meta_community.App)

                Exit For
            End If
        Next

        If eggHTS Is Nothing Then
            Call "GCModeller platform is not installed on your system!".Warning
        End If
    End Sub

    ''' <summary>
    ''' 目标文件夹是不是GCModeller Apps的应用程序组文件夹？
    ''' </summary>
    ''' <param name="DIR$"></param>
    ''' <returns></returns>
    Private Shared Function IsAppHome(DIR$) As Boolean
        Dim CLI As Type = GetType(InteropService)
        Dim Apps$() = GetType(Apps) _
            .GetProperties(BindingFlags.Public Or BindingFlags.Static) _
            .Where(Function(t)
                       Return t.PropertyType.IsInheritsFrom(CLI)
                   End Function) _
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
