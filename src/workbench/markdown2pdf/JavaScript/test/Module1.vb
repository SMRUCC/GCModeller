#Region "Microsoft.VisualBasic::1aaae4afa3c668dad377ab98029dde7a, markdown2pdf\JavaScript\test\Module1.vb"

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

    ' Module Module1
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS.Parser
Imports SMRUCC.WebCloud.JavaScript.FontAwesome

Module Module1

    Sub Main()
        Dim cssFile = "C:\Users\administrator\Desktop\fontawesome.css"
        Dim code = VBScript.FromCSS(fontawesome:=cssFile)

        Call code.SaveTo("D:\GCModeller\src\runtime\httpd\WebCloud\JavaScript\FontAwesome\Icons.vb")

        Pause()
    End Sub
End Module
