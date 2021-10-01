#Region "Microsoft.VisualBasic::7b40c11c81946c90a59c12de9a63e176, markdown2pdf\JavaScript\d3.js\PieChart.vb"

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

    '     Class Slice
    ' 
    '         Properties: caption, color, Data, label, value
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace PieChart

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' d3pie所需要的数据结构：
    ''' 
    ''' ```javascript
    ''' // label: "Ruby", value: 2, caption: "Foreign and strange", color: "#00aa00"
    ''' ```
    ''' </remarks>
    Public Class Slice

        Public Property label As String
        Public Property color As String
        Public Property value As Double
        Public Property caption As String
        Public Property Data As Dictionary(Of String, String)

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
