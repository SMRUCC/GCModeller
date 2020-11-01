#Region "Microsoft.VisualBasic::e71145d8d0d4a6d688f6ee808262f420, RDotNET.Extensions.VisualBasic\Server\RInit.vb"

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

' Module RInit
' 
'     Function: platformNotSupport, searchAuto, (+2 Overloads) StartEngineServices
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.FileIO.Path
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq

Module RInit

    ''' <summary>
    ''' Automatically search for the path of the R system and then construct a R session for you.
    ''' (如果在注册表之中已经存在了R的路径的值或者你已经设置好了环境变量，则可以直接使用本函数进行初始化操作)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' ``/@set R_HOME='/path/to/R_server/'``
    ''' </remarks>
    Public Function StartEngineServices() As ExtendedEngine
        Static R_HOME As New [Default](Of String) With {
            .lazy = New Lazy(Of String)(AddressOf searchAuto)
        }

        ' Read R_HOME path from environment variable or using 
        ' auto Path search value as default if the R_HOME 
        ' variable Is Nothing in the commandline.
        With App.GetVariable(NameOf(R_HOME)) Or R_HOME
            Return .DoCall(AddressOf RInit.StartEngineServices)
        End With
    End Function

    ''' <summary>
    ''' Search R server bin on this localhost's file system
    ''' </summary>
    ''' <returns></returns>
    Private Function searchAuto() As String
        Dim directories$() = ProgramPathSearchTool.SearchDirectory("R").ToArray
        Dim files$()

        If directories.IsNullOrEmpty Then
            Throw New Exception(INIT_FAILURE)
        End If

        For Each direactory As String In directories
            files$ = ProgramPathSearchTool.SearchProgram(direactory, "R").ToArray

            If Not files.IsNullOrEmpty Then
                Return files.First.ParentPath
            End If
        Next

        Throw New Exception(INIT_FAILURE)
    End Function

    Const INIT_FAILURE As String = "Could not initialize the R session automatically!"
    Const R_HOME_NOT_FOUND As String = "Could not found the specified path to the directory containing R.dll: "

    ''' <summary>
    ''' R server running on x86 CPU platform
    ''' </summary>
    ReadOnly i386 As [Default](Of String) = NameOf(i386).AsDefault(Function() Not Environment.Is64BitProcess)
    ReadOnly x64$ = NameOf(x64)

    ''' <summary>
    ''' Manual setup the R system path.(这个函数是在没有自动设置好环境变量的时候进行手动的环境变量设置所使用的初始化方法)
    ''' </summary>
    ''' <param name="R_HOME"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function StartEngineServices(R_HOME As String) As ExtendedEngine
        Dim oldPath$ = Environment.GetEnvironmentVariable("PATH")
        Dim rPath As String = $"{R_HOME}/{x64 Or i386}"

        If Directory.Exists(rPath) = False Then
            Throw New DirectoryNotFoundException(R_HOME_NOT_FOUND & " ---> """ & rPath & """")
        Else
            Dim pathEnvir As String() = oldPath _
                .Split(Path.PathSeparator) _
                .Where(Function(path) path <> rPath) _
                .ToArray

            oldPath = pathEnvir.JoinBy(Path.PathSeparator)
        End If

        If VBDebugger.debugMode Then
            Call $"R_HOME={rPath}".__DEBUG_ECHO
        End If

        Dim newPath = String.Format("{0}{1}{2}", rPath, Path.PathSeparator, oldPath)
        Dim rHome$ = ""

        Select Case (Environment.OSVersion.Platform)
            Case PlatformID.Win32NT
            Case PlatformID.MacOSX : rHome = "/Library/Frameworks/R.framework/Resources"
            Case PlatformID.Unix : rHome = "/usr/lib/R"
            Case Else
                Throw platformNotSupport()
        End Select

        Call Environment.SetEnvironmentVariable("PATH", newPath)

        If Not String.IsNullOrEmpty(rHome) Then
            Call Environment.SetEnvironmentVariable("R_HOME", rHome)
        End If

        Dim dll As String = $"{rPath}/R.dll"
        Dim R = ExtendedEngine.__init("RDotNet_" & App.GetNextUniqueName("process_"), dll)

        Call REngine.SetEnvironmentVariables()
        Call R.Initialize()

        Return R
    End Function

    ''' <summary>
    ''' Current platform is not surppoted for running R server!
    ''' </summary>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function platformNotSupport() As Exception
        Return New NotSupportedException($"No support such platform: {Environment.OSVersion.Platform}")
    End Function
End Module
