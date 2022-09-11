#Region "Microsoft.VisualBasic::8c70df60561749df1f52434f9d2a7f84, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Module\ModuleDBGet.vb"

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

    '   Total Lines: 38
    '    Code Lines: 26
    ' Comment Lines: 5
    '   Blank Lines: 7
    '     File Size: 1.84 KB


    '     Module ModuleDBGet
    ' 
    '         Function: Download
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Module ModuleDBGet

        ''' <summary>
        ''' 这个函数下载的是物种特定的模块信息，不是参考模块数据
        ''' </summary>
        ''' <param name="url"></param>
        ''' <returns></returns>
        Public Function Download(url As String) As [Module]
            Dim WebForm As New WebForm(url)

            If WebForm.Count = 0 Then
                Return Nothing
            End If

            Dim sp As String = WebForm.GetValue("Organism").FirstOrDefault
            sp = Regex.Match(sp, "\[GN:<a href="".+?"">.+?</a>]").Value.GetValue

            Dim [Module] As New [Module] With {
                .EntryId = Regex.Match(WebForm.GetValue("Entry").FirstOrDefault, "[a-z]+_M\d+").Value,
                .Name = WebForm.GetValue("Name").FirstOrDefault,
                .Description = WebForm.GetValue("Definition").FirstOrDefault,
                .Pathway = WebForm.parseList(WebForm.GetValue("Pathway").FirstOrDefault, "<a href=""/kegg-bin/show_pathway\?.+?"">.+?</a>"),
                .PathwayGenes = WebForm.parseList(WebForm.GetValue("Gene").FirstOrDefault, String.Format("<a href=""/dbget-bin/www_bget\?{0}:.+?"">.+?</a>", sp)),
                .Compound = WebForm.parseList(WebForm.GetValue("Compound").FirstOrDefault, "<a href=""/dbget-bin/www_bget\?cpd:.+?"">.+?</a>"),
                .Reaction = WebForm.parseList(WebForm.GetValue("Reaction").FirstOrDefault, "<a href=""/dbget-bin/www_bget\?rn:.+?"">.+?</a>")
            }

            Return [Module]
        End Function
    End Module
End Namespace
