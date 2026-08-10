#Region "Microsoft.VisualBasic::7991f8f7f9b319259ddd40149164ca7c, src\Flute\Configuration\Configuration.vb"

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
    '    Code Lines: 31 (59.62%)
    ' Comment Lines: 11 (21.15%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (19.23%)
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

        <Description("request read timeout in milliseconds, a slow client will be disconnected when exceeded this value. Default 30000 (30s).")>
        Public Property request_timeout As Integer = 30000

        <Description("a secret token required in the 'X-Shutdown-Token' header to allow the /ctrl/kill endpoint to stop the server. A null/empty value disables remote shutdown entirely.")>
        Public Property shutdown_token As String = ""

        <Description("comma separated list of allowed CORS origins. Use '*' (default) to allow any origin.")>
        Public Property cors_allow_origin As String = "*"

        <Description("comma separated list of allowed CORS methods.")>
        Public Property cors_allow_methods As String = "POST, GET, OPTIONS"

        <Description("comma separated list of allowed CORS headers.")>
        Public Property cors_allow_headers As String = "X-PINGOTHER, Content-Type"

        <Description("a logical value for enable the RFC6455 websocket protocol upgrade handshake on this http server. Default is enabled.")>
        Public Property websocket_enabled As Boolean = True

        <Description("comma separated list of the websocket sub-protocol names that accepted by this server. An empty value(default) means that no sub-protocol will be negotiated.")>
        Public Property websocket_subprotocols As String = ""

        <Description("the max size in bytes of a single websocket application message after the fragmentation re-assembly, default 16MB. A value which is less than or equals to zero means no limit.")>
        Public Property websocket_max_message_size As Integer = 16 * 1024 * 1024

        <Description("the socket read timeout in milliseconds of an established websocket connection. Default 0 means infinite waiting for the next data frame.")>
        Public Property websocket_read_timeout As Integer = 0

        ''' <summary>
        ''' get the websocket sub-protocol name list from the
        ''' <see cref="websocket_subprotocols"/> configuration value.
        ''' </summary>
        ''' <returns>
        ''' this function always returns an array object with no null value.
        ''' </returns>
        Public Function GetWebSocketSubProtocols() As String()
            If websocket_subprotocols.StringEmpty Then
                Return {}
            Else
                Return websocket_subprotocols _
                    .Split(","c) _
                    .Select(Function(str) str.Trim()) _
                    .Where(Function(str) Not str.StringEmpty) _
                    .ToArray
            End If
        End Function

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
