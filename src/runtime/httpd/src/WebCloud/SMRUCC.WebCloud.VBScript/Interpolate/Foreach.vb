#Region "Microsoft.VisualBasic::3e77d7f86c69e0e024e90c780bda389e, G:/GCModeller/src/runtime/httpd/src/WebCloud/SMRUCC.WebCloud.VBScript//Interpolate/Foreach.vb"

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

    '   Total Lines: 70
    '    Code Lines: 40
    ' Comment Lines: 15
    '   Blank Lines: 15
    '     File Size: 2.55 KB


    '     Module ForeachInterpolate
    ' 
    '         Function: FillTemplate, toHtml
    ' 
    '         Sub: ForeachTemplate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions

Namespace Interpolate

    Module ForeachInterpolate

        ''' <summary>
        ''' processing foreach loop in template
        ''' </summary>
        ''' <param name="vbhtml"></param>
        ''' <remarks>
        ''' syntax for define a for each loop:<br /><br />
        ''' 
        ''' <strong>
        ''' &lt;foreach @array_variable_name&gt;<br />
        '''     ...template content...<br />
        ''' &lt;/foreach&gt;
        ''' </strong><br /><br />
        ''' 
        ''' inside the template content, you can use the variable reference like this:
        ''' 
        ''' @property_name
        ''' 
        ''' where property_name is the property of the object inside the array_variable_name collection 
        ''' </remarks>
        Public Sub ForeachTemplate(vbhtml As VBHtml)
            Dim templates = VBHtml.foreach.Matches(vbhtml.ToString).ToArray
            Dim str As String

            For Each template As String In templates
                str = FillTemplate(vbhtml, template)

                ' nothing and "" empty string has different meaning at here:
                '
                ' nothing means not found: the foreach variable is existed in the variables
                ' no action at here
                ' "" empty string means empty foreach collection at here, due to 
                ' the reason of no elements inside the collection, so foreach template
                ' interplote generates empty string

                If Not str Is Nothing Then
                    Call vbhtml.Replace(template, str)
                End If
            Next
        End Sub

        ReadOnly array_name As New Regex("<foreach [^>]+>", RegexOptions.IgnoreCase Or RegexOptions.Singleline Or RegexOptions.Compiled)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="vbhtml"></param>
        ''' <param name="template">
        ''' the for each template
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' syntax for define a variable inside the html template is @xxxx 
        ''' </remarks>
        Private Function FillTemplate(vbhtml As VBHtml, template As String) As String
            Dim foreach As String = array_name.Match(template).Value
            Dim var As String = VBHtml.variable.Match(foreach).Value.Trim("@"c)

            If Not vbhtml.HasSymbol(var) Then
                Return Nothing
            Else
                template = template.GetStackValue(">", "<")
            End If

            Dim array As IEnumerable = vbhtml.GetSymbol(var)
            Dim list As New List(Of String)

            For Each item As Object In array
                Call list.Add(toHtml(item, template))
            Next

            Return list.JoinBy(vbCrLf)
        End Function

        Private Function toHtml(item As Object, template As String) As String
            Dim html As New StringBuilder(template)
            Dim vars = VBHtml.variable.Matches(template).ToArray

            For Each var_ref As String In vars
                Call html.Replace(var_ref, VariableInterpolate.GetValueString(item, var_ref.Trim("@"c)))
            Next

            Return html.ToString
        End Function
    End Module
End Namespace
