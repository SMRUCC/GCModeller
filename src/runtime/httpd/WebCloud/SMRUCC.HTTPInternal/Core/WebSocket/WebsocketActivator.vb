#Region "Microsoft.VisualBasic::d9242509967eb1106f7819970cf49dcc, WebCloud\SMRUCC.HTTPInternal\Core\WebSocket\WebsocketActivator.vb"

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

    '     Delegate Function
    ' 
    ' 
    '     Module Extensions
    ' 
    '         Function: GetActivator
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Net.Sockets
Imports System.Reflection
Imports Microsoft.VisualBasic.ApplicationServices.Plugin
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Language.Values

Namespace Core.WebSocket

    ''' <summary>
    ''' A static method should be marked with <see cref="PluginAttribute"/>
    ''' </summary>
    ''' <param name="tcp"></param>
    ''' <returns></returns>
    Public Delegate Function WebsocketActivator(tcp As TcpClient) As WsProcessor

    <HideModuleName> Public Module Extensions

        Public Function GetActivator(directory$, entry As String) As WebsocketActivator
            Dim activator As New Value(Of MethodInfo)

            For Each dll As String In ls - l - r - "*.dll" <= directory
                If Not (activator = Loader.GetPluginMethod(dll, entry)) Is Nothing Then
                    Return activator.CreateDelegate(GetType(WebsocketActivator))
                End If
            Next

            Return Nothing
        End Function
    End Module
End Namespace
