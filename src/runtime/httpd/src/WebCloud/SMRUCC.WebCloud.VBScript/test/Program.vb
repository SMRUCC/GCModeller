#Region "Microsoft.VisualBasic::75ff7b37676fa9cfd4ec34c19c45f7e9, G:/GCModeller/src/runtime/httpd/src/WebCloud/SMRUCC.WebCloud.VBScript/test//Program.vb"

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

    '   Total Lines: 36
    '    Code Lines: 24
    ' Comment Lines: 0
    '   Blank Lines: 12
    '     File Size: 903 B


    ' Module Program
    ' 
    '     Sub: Main, variable_reflection_test
    ' 
    ' Class person
    ' 
    '     Properties: age, name
    ' 
    ' /********************************************************************************/

#End Region

Imports Flute.Template

Module Program

    Sub Main(args As String())
        Console.WriteLine("Hello World!")

        Call variable_reflection_test()
    End Sub

    Const demo_test_template As String = "\GCModeller\src\runtime\httpd\test\template_test\index.vbhtml"

    Sub variable_reflection_test()
        Dim testdata As New Dictionary(Of String, Object) From {
            {"person", New person},
            {"title", "demo html page"},
            {"lang", "zh"}
        }
        Dim html As String = VBHtml.ReadHTML(demo_test_template, testdata)

        Call Console.WriteLine(html)

        testdata!lang = "en"

        Call Console.WriteLine(VBHtml.ReadHTML(demo_test_template, testdata))

        Pause()
    End Sub
End Module

Public Class person

    Public Property name As String = "aaaaaa"
    Public Property age As Integer = 100

End Class
