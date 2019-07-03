#Region "Microsoft.VisualBasic::2f2c5b8ec1206130294f2d6b5d990555, WebCloud\SMRUCC.WebCloud.VBScript\DimVar.vb"

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

    ' Module vbhtml
    ' 
    '     Function: InterplotDimVar, ParseDimVar
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports r = System.Text.RegularExpressions.Regex

Partial Module vbhtml

    ''' <summary>
    ''' ```vbnet
    ''' Dim $var As &lt;%= path %> 
    ''' ```
    ''' </summary>
    Const DimVar$ = "<\?vb\s+Dim\s+.+?\s+As\s+" & PartialIncludes & "\s+\?>"

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function ParseDimVar(vbhtml$) As NamedValue(Of String)()
        Dim expression = r _
            .Matches(vbhtml, DimVar, RegexICSng) _
            .EachValue(AddressOf parserInternal) _
            .ToArray

        Return expression
    End Function

    <Extension>
    Public Function InterplotDimVar(html As StringBuilder, parent$, args As InterpolateArgs) As StringBuilder
        Dim expressions = ParseDimVar(html.ToString)
        Dim content$

        ' 在这里只生成variable，可能不会进行模板字符串的替换
        For Each var As NamedValue(Of String) In expressions
            With var
                content = New StringBuilder(.Value) _
                    .TemplateInterplot(parent, args)

                Call args.variables.Add(Mid(.Name, 2), content)
                Call html.Replace(.Description, "")
            End With
        Next

        Return html
    End Function
End Module
