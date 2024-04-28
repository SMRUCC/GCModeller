#Region "Microsoft.VisualBasic::7991f8f7f9b319259ddd40149164ca7c, G:/GCModeller/src/runtime/httpd/src/Flute//Configuration/Configuration.vb"

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


    ' Code Statistics:

    '   Total Lines: 52
    '    Code Lines: 31
    ' Comment Lines: 11
    '   Blank Lines: 10
    '     File Size: 1.84 KB


    '     Class Configuration
    ' 
    '         Properties: session, silent, x_powered_by
    ' 
    '         Function: [Default], Load, Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Flute.Http.Core
Imports Microsoft.VisualBasic.ComponentModel.Settings.Inf

Namespace Configurations

    ''' <summary>
    ''' http server configuration
    ''' </summary>
    <ClassName("configuration")>
    Public Class Configuration

        <Description("a string for identify the http server backend.")>
        Public Property x_powered_by As String = HttpProcessor.VBS_platform

        <Description("a logical value for turn the verbose echo of the debug message on.")>
        Public Property silent As Boolean = True

        <Description("user session in server backend")>
        Public Property session As Session

        Public Shared Function [Default]() As Configuration
            Return New Configuration With {.session = New Session}
        End Function

        ''' <summary>
        ''' safe handler for load ini configuration file
        ''' </summary>
        ''' <param name="inifile"></param>
        ''' <returns>
        ''' this function returns the default configuration file if the
        ''' given <paramref name="inifile"/> missing or invalid file format.
        ''' </returns>
        Public Shared Function Load(inifile As String) As Configuration
            If inifile.FileLength <= 0 Then
                Return [Default]()
            End If

            Try
                Return ClassMapper.LoadIni(Of Configuration)(inifile)
            Catch ex As Exception
                Call App.LogException(ex)
                Return New Configuration
            End Try
        End Function

        Public Shared Function Save(settings As Configuration, inifile As String) As Boolean
            Return ClassMapper.WriteClass(settings, inifile, clean:=True)
        End Function

    End Class
End Namespace
