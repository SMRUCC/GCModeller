#Region "Microsoft.VisualBasic::e9d5ea3519f60cffb17d375d1c187e0f, ..\workbench\d3js\d3svg\d3svg\DrawSVG.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Drawing
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.Imaging

''' <summary>
''' 
''' </summary>
''' <remarks>似乎只能够x86编译</remarks>
Public Class DrawSVG

    ''' <summary>
    ''' The install directory of GIMP library/software
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property GIMP As String

    Sub New(bin As String)
        GIMP = bin
    End Sub

    Public Overrides Function ToString() As String
        Return GIMP
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Public Shared Function SetDllDirectory(DIR As String) As Boolean
    End Function

    <DllImport("libgobject-2.0-0.dll", EntryPoint:="g_type_init", SetLastError:=True)>
    Public Shared Sub GraphicsTypeInit()
    End Sub

    ' http://stackoverflow.com/questions/2390407/pinvokestackimbalance-c-sharp-call-to-unmanaged-c-function

    ' It's good.I update function define as follow:
    ' [DllImport("mydll.dll", CallingConvention = CallingConvention.Cdecl)]
    ' It works well.

    <DllImport("librsvg-2-2.dll",
               EntryPoint:="rsvg_pixbuf_from_file_at_size",
               CallingConvention:=CallingConvention.Cdecl,
               CharSet:=CharSet.Ansi,
               SetLastError:=True)>
    Public Shared Function ReadsvgPixbufFromFileWithSize(path As String, width As Integer, height As Integer, <Out> ByRef [Error] As IntPtr) As IntPtr
    End Function

    <DllImport("libgdk_pixbuf-2.0-0.dll",
               EntryPoint:="gdk_pixbuf_save",
               CallingConvention:=CallingConvention.Cdecl,
               CharSet:=CharSet.Ansi)>
    Public Shared Function SaveGdkPixbuf(pixbuf As IntPtr, filename As String, type As String, <Out> ByRef [Error] As IntPtr, __arglist As Object) As Boolean
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="svg">SVG inputs file path</param>
    ''' <param name="png">PNG output path</param>
    Public Sub RasterizeSvg(svg As String, png As String, Optional size As Size = Nothing, Optional format As ImageFormats = ImageFormats.Png)
        Dim callSuccessful As Boolean = SetDllDirectory(GIMP)

        If Not callSuccessful Then
            Throw New Exception("Could not set DLL directory")
        End If

        Call GraphicsTypeInit()

        Dim w As Integer = size.Width
        Dim h As Integer = size.Height

        w = If(w = 0, -1, w)
        h = If(h = 0, -1, h)

        Dim [error] As IntPtr
        Dim result As IntPtr = ReadsvgPixbufFromFileWithSize(svg, w, h, [error])

        If [error] <> IntPtr.Zero Then
            Throw New Exception(Marshal.ReadInt32([error]).ToString())
        End If

        Dim type As String = format.ToString.ToLower

        callSuccessful = SaveGdkPixbuf(result, png, type, [error], Nothing)

        If Not callSuccessful Then
            Throw New Exception([error].ToInt32().ToString())
        End If
    End Sub
End Class

