#Region "Microsoft.VisualBasic::b99e312bda83487c55fa684f6ddf8c8d, Shared\InternalApps_CLI\Apps.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Class Apps
    ' 
    '     Properties: eggHTS, FBA, GCModellerCompiler, KEGG_tools, localblast
    '                 MEME, metaProfiler, NCBI_tools, seqtools, SyntenyVisual
    '                 VennDiagram
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: IsAppHome, LinuxPath, WindowsPath
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' A list of GCModeller apps 
''' </summary>
Public NotInheritable Class Apps

    Public Shared ReadOnly Property NCBI_tools As GCModellerApps.NCBI_tools
    Public Shared ReadOnly Property localblast As GCModellerApps.localblast
    Public Shared ReadOnly Property MEME As GCModellerApps.MEME
    Public Shared ReadOnly Property KEGG_tools As GCModellerApps.KEGG_tools
    Public Shared ReadOnly Property seqtools As GCModellerApps.seqtools
    Public Shared ReadOnly Property VennDiagram As GCModellerApps.venn
    Public Shared ReadOnly Property eggHTS As GCModellerApps.eggHTS
    Public Shared ReadOnly Property metaProfiler As GCModellerApps.metaProfiler
    Public Shared ReadOnly Property SyntenyVisual As GCModellerApps.Synteny
    Public Shared ReadOnly Property GCModellerCompiler As GCModellerApps.GCC
    Public Shared ReadOnly Property FBA As GCModellerApps.FBA

    Const developmentPathPattern$ = "GCModeller\GCModeller\bin"

    ''' <summary>
    ''' 枚举一些比较常用的路径
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
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

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Shared Function LinuxPath() As String()
        Return {}
    End Function

    Shared Sub New()
        Dim list$() = LinuxPath() Or WindowsPath.When(App.IsMicrosoftPlatform)

        For Each HOME As String In list
            If Apps.IsAppHome(HOME) Then

                If InStr(HOME, developmentPathPattern) > 0 Then
                    Call "Using GCModeller development version.".debug
                End If

                _eggHTS = GCModellerApps.eggHTS.FromEnvironment(directory:=HOME)
                _NCBI_tools = GCModellerApps.NCBI_tools.FromEnvironment(directory:=HOME)
                _localblast = GCModellerApps.localblast.FromEnvironment(directory:=HOME)
                _MEME = GCModellerApps.MEME.FromEnvironment(directory:=HOME)
                _KEGG_tools = GCModellerApps.KEGG_tools.FromEnvironment(directory:=HOME)
                _seqtools = GCModellerApps.seqtools.FromEnvironment(directory:=HOME)
                _VennDiagram = GCModellerApps.venn.FromEnvironment(directory:=HOME)
                _metaProfiler = GCModellerApps.metaProfiler.FromEnvironment(directory:=HOME)
                _SyntenyVisual = GCModellerApps.Synteny.FromEnvironment(directory:=HOME)
                _GCModellerCompiler = GCModellerApps.GCC.FromEnvironment(directory:=HOME)
                _FBA = GCModellerApps.FBA.FromEnvironment(directory:=HOME)

                Exit For
            End If
        Next

        If eggHTS Is Nothing Then
            Dim platform$ = If(App.IsMicrosoftPlatform, "MS Win_x86/x64", "UNIX_x64")
            Call $"GCModeller platform is not installed on your system! (platform={platform})".Warning
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
