#Region "Microsoft.VisualBasic::2d3abd4c0a7a8006e5f27647498f899b, visualize\Cytoscape\Cytoscape\Graph\cytoscape.js\Styles\Expression.vb"

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

    '     Delegate Function
    ' 
    ' 
    '     Module Expression
    ' 
    '         Function: ValueMap
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv.IO

Namespace CytoscapeGraphView.Cyjs.style

    Public Delegate Function GetProperty(name$) As String

    ''' <summary>
    ''' The <see cref="style.css"/> (<see cref="CSSTranslator"/>) value expression
    ''' </summary>
    Public Module Expression

        ''' <summary>
        ''' ``mapData(strength,70,100,2,6)``
        ''' </summary>
        ''' <param name="exp$"></param>
        ''' <returns></returns>
        Public Function ValueMap(exp$) As Func(Of GetProperty, Double)
            Dim params$ = exp.GetStackValue("(", ")")
            Dim t As String() = Tokenizer.CharsParser(params)
            Dim key$ = t(Scan0)
            Dim rangeData As New DoubleRange(Val(t(1)), Val(t(2)))
            Dim rangeValue As New DoubleRange(Val(t(3)), Val(t(4)))

            Return Function(obj As GetProperty) As Double
                       Dim value As Double = Val(obj(name:=key))

                       If value <= rangeData.Min Then
                           Return rangeValue.Min
                       ElseIf value >= rangeData.Max Then
                           Return rangeValue.Max
                       Else
                           Return rangeData.ScaleMapping(value, rangeValue)
                       End If
                   End Function
        End Function
    End Module
End Namespace
