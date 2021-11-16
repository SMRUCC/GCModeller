#Region "Microsoft.VisualBasic::67552911f65ee2c416346ab5aa13513b, markdown2pdf\ReportBuilder\EMailMsg.vb"

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

    ' Module EMailMsg
    ' 
    '     Function: GetMessage
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

Public Module EMailMsg

    Public Function GetMessage(Title As String, Message As String, User As String, Link As String, LinkTitle As String) As String
        Dim html As New StringBuilder(My.Resources.readmail)

        Call html.Replace("{Title}", Title)
        Call html.Replace("{UserName}", User)
        Call html.Replace("{Message}", Message)
        Call html.Replace("{LinkTitle}", LinkTitle)
        Call html.Replace("{Link}", Link)

        Return html.ToString
    End Function
End Module
