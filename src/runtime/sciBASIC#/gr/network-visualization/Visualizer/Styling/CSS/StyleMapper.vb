#Region "Microsoft.VisualBasic::341a5a61b041e90b31f9831e07433468, gr\network-visualization\Visualizer\Styling\StyleMapper.vb"

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

'     Structure StyleMapper
' 
'         Function: (+2 Overloads) __createSelector, (+2 Overloads) FromJSON
' 
'     Delegate Function
' 
' 
'     Delegate Function
' 
' 
'     Structure StyleCreator
' 
'         Function: CompileSelector
' 
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Expressions
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Styling.CSS

    ''' <summary>
    ''' Network object visualize styling object model, the network render css file parser
    ''' </summary>
    Public Structure StyleMapper

        Dim nodeStyles As StyleCreator()
        Dim edgeStyles As StyleCreator()

        ''' <summary>
        ''' node label styling
        ''' </summary>
        Dim labelStyles As StyleCreator()

        Public Shared Function FromJSON(json$) As StyleMapper
            Dim styleJSON As StyleJSON = json.LoadJSON(Of StyleJSON)
            Return FromJSON(styleJSON)
        End Function

        Public Shared Function FromJSON(json As StyleJSON) As StyleMapper
            Return New StyleMapper With {
                .nodeStyles = StyleMapper.__createSelector(json.nodes)
            }
        End Function

        Private Shared Function __createSelector(styles As Dictionary(Of String, NodeStyle)) As StyleCreator()
            Return styles _
                .Select(Function(x) __createSelector(x.Key, x.Value)) _
                .ToArray
        End Function

        Private Shared Function __createSelector(selector$, style As NodeStyle) As StyleCreator
            Dim mapper As New StyleCreator With {
                .selector = selector,
                .fill = BrushExpression.Evaluate(style.fill),
                .stroke = Stroke.TryParse(style.stroke),
                .size = SizeExpression.Evaluate(style.size)
            }
            Return mapper
        End Function
    End Structure
End Namespace
