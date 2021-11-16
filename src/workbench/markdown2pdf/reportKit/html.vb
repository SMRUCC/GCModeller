#Region "Microsoft.VisualBasic::aa2037c010be3151c703068db7b5b27a, markdown2pdf\reportKit\html.vb"

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

    ' Module html
    ' 
    '     Function: markdownToHtml, template
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.MIME.text.markdown
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.Workbench.ReportBuilder.HTML

<Package("html", Category:=APICategories.UtilityTools)>
Public Module html

    ''' <summary>
    ''' Create a html template model from the given template file
    ''' </summary>
    ''' <param name="url"></param>
    ''' <returns></returns>
    <ExportAPI("template")>
    Public Function template(url As String) As TemplateHandler
        Return New TemplateHandler(file:=url)
    End Function

    ''' <summary>
    ''' Render markdown to html text
    ''' </summary>
    ''' <param name="markdown"></param>
    ''' <returns></returns>
    <ExportAPI("markdown.html")>
    Public Function markdownToHtml(markdown As String) As String
        Static render As New MarkdownHTML
        Return render.Transform(markdown)
    End Function
End Module
