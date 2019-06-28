#Region "Microsoft.VisualBasic::051a0f6b78d2766f2ace09ad4e19ec36, WebCloud\SMRUCC.HTTPInternal\Core\WebSocket\Operations.vb"

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

    '     Enum Operations
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Core.WebSocket

    Public Enum Operations As Integer
        ''' <summary>
        ''' Text Data Sent From Client
        ''' </summary>
        TextRecieved = &H1
        ''' <summary>
        ''' Binary Data Sent From Client 
        ''' </summary>
        BinaryRecieved = &H2
        ''' <summary>
        ''' Ping Sent From Client 
        ''' </summary>
        Ping = &H9
        ''' <summary>
        ''' Pong Sent From Client 
        ''' </summary>
        Pong = &HA
    End Enum
End Namespace
