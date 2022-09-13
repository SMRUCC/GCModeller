#Region "Microsoft.VisualBasic::727cfb5c243094080afacb37610fca26, GCModeller\core\Bio.Assembly\Assembly\KEGG\Web\Form\Extensions.vb"

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

    '   Total Lines: 43
    '    Code Lines: 32
    ' Comment Lines: 5
    '   Blank Lines: 6
    '     File Size: 1.47 KB


    '     Module Extensions
    ' 
    '         Function: DivInternals, IsShowAllLink, Strip_NOBR
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

Namespace Assembly.KEGG.WebServices.InternalWebFormParsers

    <HideModuleName> Public Module Extensions

        Public Const DBGET$ = "DBGET integrated database retrieval system"

        <Extension>
        Public Function DivInternals(html$) As String()
            If html.StringEmpty Then
                Return {}
            Else
                Dim ms$() = Regex.Matches(html, "<div.+?</div>", RegexICSng).ToArray
                Return ms
            End If
        End Function

        ''' <summary>
        ''' 这个函数只会将第一个nobr标签，即key标签字符串部分给删除掉，其他的nobr标签会被保留
        ''' </summary>
        ''' <param name="html$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Strip_NOBR(html$) As String
            If html.StringEmpty Then
                Return ""
            Else
                Dim m$ = Regex.Match(html, "<nobr>.+?</nobr>", RegexICSng).Value
                If Not m.Length = 0 Then
                    html = html.Replace(m, "")
                End If
                Return html
            End If
        End Function

        <Extension>
        Public Function IsShowAllLink(s$) As Boolean
            Return InStr(s, "show all", Compare:=CompareMethod.Text) > 0
        End Function
    End Module
End Namespace
