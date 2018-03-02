#Region "Microsoft.VisualBasic::57f3681481a9033032d1351aa5f94112, RDotNET.Extensions.VisualBasic\API\grDevices\dev.vb"

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

    '     Module dev
    ' 
    '         Function: off
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace API.grDevices

    Public Module dev

        ''' <summary>
        ''' ``dev.off`` returns the number and name of the new active device (after the specified device has been shut down).
        ''' </summary>
        ''' <param name="which"></param>
        ''' <returns></returns>
        Public Function off(Optional which As String = "dev.cur()") As Integer
            Dim dev = $"dev.off(which={which})".__call
            Throw New NotImplementedException
        End Function
    End Module
End Namespace
