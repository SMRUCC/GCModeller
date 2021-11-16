#Region "Microsoft.VisualBasic::ddddfa5186ae52117c86c3a0c97a9521, markdown2pdf\ReportBuilder\Interpolation\Interpolation.vb"

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

    ' Module Interpolation
    ' 
    '     Function: Interpolate
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

Module Interpolation

    ''' <summary>
    ''' 在模板之中可能还会存在html碎片的插值
    ''' 在这里进行模板的html碎片的加载
    ''' 
    ''' 模板的插值格式为${relpath}
    ''' </summary>
    ''' <param name="templateUrl">the file path of the template</param>
    ''' <returns></returns>
    Public Function Interpolate(templateUrl As String) As String
        Dim dir As String = templateUrl.ParentPath
        Dim template As New StringBuilder(templateUrl.ReadAllText)
        Dim fragmentRefs As String() = template.ToString.Matches("[$]\{.+?\}", RegexICSng).ToArray
        Dim url As String
        Dim part As String

        For Each ref As String In fragmentRefs
            url = dir & "/" & ref.GetStackValue("{", "}")
            part = Interpolate(templateUrl:=url)
            template.Replace(ref, part)
        Next

        Return template.ToString
    End Function
End Module
