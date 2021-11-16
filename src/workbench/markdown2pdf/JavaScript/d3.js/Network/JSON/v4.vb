#Region "Microsoft.VisualBasic::582139fac86f0d3b7a5cd23b61f68a90, markdown2pdf\JavaScript\d3.js\Network\JSON\v4.vb"

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

    '     Class node
    ' 
    '         Properties: group, id, name, value
    ' 
    '     Class link
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.WebCloud.JavaScript.d3js.Network.JSON.v3

Namespace Network.JSON.v4

    Public Class node

        Public Property id As String
        Public Property name As String
        Public Property group As String
        Public Property value As String
    End Class

    Public Class link : Inherits link(Of String)

    End Class
End Namespace
