#Region "Microsoft.VisualBasic::ac07d5066ef75a456fdbfaf550500a59, localblast\LocalBLAST\Pipeline\COG\Whog\TextParser.vb"

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

    '     Module TextParser
    ' 
    '         Function: Parse, parseList
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports r = System.Text.RegularExpressions.Regex

Namespace Pipeline.COG.Whog

    Module TextParser

        Const REGX_CATAGORY As String = "\[[^]]+\]"
        Const REGX_COG_ID As String = "COG\d+"

        Public Function Parse(srcText$()) As Category
            Dim list As NamedValue() = parseList(srcText.Skip(1).ToArray)
            Dim description As String = srcText(Scan0)
            Dim cat$ = r.Match(description, REGX_CATAGORY).Value
            Dim item As New Category With {
                .category = Mid(cat, 2, Len(cat) - 2),
                .COG_id = r.Match(description, REGX_COG_ID).Value,
                .description = Mid(description, Len(.category) + Len(.COG_id) + 4).Trim,
                .IdList = list
            }
            Return item
        End Function

        Private Function parseList(lines As IEnumerable(Of String)) As NamedValue()
            Dim list As New List(Of NamedValue)

            For Each line As String In lines
                Dim nid As NamedValue(Of String) = line.GetTagValue(":", trim:=True)
                Dim genome$ = nid.Name.Trim

                If Not String.IsNullOrEmpty(genome) Then
                    list += New NamedValue With {
                        .name = genome,
                        .Text = nid.Value
                    }
                Else
                    list.Last = New NamedValue With {
                        .name = list.Last.name,
                        .text = list.Last.text & " " & Strings.Trim(nid.Value)
                    }
                End If
            Next

            Return list
        End Function
    End Module
End Namespace
