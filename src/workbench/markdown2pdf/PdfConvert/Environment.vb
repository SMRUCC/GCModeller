#Region "Microsoft.VisualBasic::f2802d5a4956d22869a3d24f384150e9, markdown2pdf\PdfConvert\Environment.vb"

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

' Module InternalEnvironment
' 
'     Properties: Environment
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: GetWkhtmlToPdfExeLocation
' 
' /********************************************************************************/

#End Region

Imports System.Configuration
Imports System.IO
Imports Microsoft.VisualBasic.Language.Default
Imports WkHtmlToPdf.Arguments
Imports ProgramFiles = Microsoft.VisualBasic.FileIO.Path.ProgramPathSearchTool

Module InternalEnvironment

    Public ReadOnly Property Environment As [Default](Of PdfConvertEnvironment)

    Public Const wkhtmltopdf$ = "wkhtmltopdf.exe"
    Public Const wkhtmltopdfInstall$ = "wkhtmltopdf\wkhtmltopdf.exe"

#If DEBUG Then
    Const isDebugMode As Boolean = True
#Else
    Const isDebugMode As Boolean = False
#End If

    Sub New()
        Environment = New [Default](Of PdfConvertEnvironment) With {
            .lazy = New Lazy(Of PdfConvertEnvironment)(AddressOf lazyGetEnvironment)
        }
    End Sub

    Private Function lazyGetEnvironment() As PdfConvertEnvironment
        Return New PdfConvertEnvironment With {
            .TempFolderPath = Path.GetTempPath(),
            .WkHtmlToPdfPath = GetWkhtmlToPdfExeLocation(),
            .Timeout = 60000,
            .Debug = isDebugMode
        }
    End Function

    Private Function GetWkhtmlToPdfExeLocation() As String
        Dim customPath As String = ConfigurationManager.AppSettings("wkhtmltopdf:path")
        Dim search As New ProgramFiles() With {
            .CustomDirectories = {
                customPath, App.HOME
            }
        }
        Dim file As String = search _
            .FindProgram("wkhtmltopdf", includeDll:=False) _
            .FirstOrDefault

        If Not file.StringEmpty Then
            Return file
        End If

        For Each dir As String In ProgramFiles.SearchDirectory("wkhtmltopdf")
            For Each exeFile As String In ProgramFiles.SearchProgram(dir, "wkhtmltopdf", includeDll:=False)
                Return exeFile
            Next
        Next

        Throw New FileNotFoundException("Progream ""wkhtmltopdf"" is not installed!")
    End Function
End Module

