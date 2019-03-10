#Region "Microsoft.VisualBasic::65dc8ee42f57f1e90ba296c5890998f4, visualize\Cytoscape\Cytoscape\Graph\cytoscape.js\Styles\Selector.vb"

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

    '     Class Selector
    ' 
    '         Properties: Expression, Key, Type, value
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Test, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace CytoscapeGraphView.Cyjs.style

    Public Class Selector

        Public ReadOnly Property Expression As String
        ''' <summary>
        ''' node or edge?
        ''' </summary>
        ''' <returns></returns>
        Public Property Type As String
        Public Property Key As String
        Public Property value As String

        Const regexp$ = "^[a-z]+\[.+?\s*=\s*'.+'\s*\]$"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ctor$">
        ''' 字典条件
        ''' ```
        ''' type[key = 'value']
        ''' ```
        ''' </param>
        Sub New(ctor$)
            Expression = ctor

            If ctor.IsPattern(regexp, RegexICMul) Then
                Dim t = ctor.GetTagValue("[", trim:=True)

                ctor = t.Value
                Type = t.Name
                t = ctor.GetTagValue("=", trim:=True)
                Key = t.Name
                value = t.Value.Trim.GetStackValue("'", "'")
            Else
                ' 所有的对象都符合
                Type = ctor
            End If
        End Sub

        Public Function Test(obj As Dictionary(Of String, String)) As Boolean
            If String.IsNullOrEmpty(Key) Then
                Return True ' 类似于 selector: 'node' ， 即所有的节点都符合条件 
            End If

            If Not obj.ContainsKey(Key) Then
                Return False ' 不存在目标属性，则这个节点当然不符合条件
            End If

            Dim value As String = obj(Key)
            Return value = Me.value
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
