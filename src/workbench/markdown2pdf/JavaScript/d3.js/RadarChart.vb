#Region "Microsoft.VisualBasic::c09479b1d7f0245575e702e0a01dca85, markdown2pdf\JavaScript\d3.js\RadarChart.vb"

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

    '     Class AxisValue
    ' 
    '         Properties: axis, value
    ' 
    '         Function: ToString
    ' 
    '     Class RadarData
    ' 
    '         Properties: colors, layers, names
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace RadarChart

    Public Class AxisValue
        Public Property axis As String
        Public Property value As Double

        Public Overrides Function ToString() As String
            Return $"Dim {axis} = {value}"
        End Function
    End Class

    Public Class RadarData
        Public Property colors As String()
        Public Property names As String()
        Public Property layers As AxisValue()()

        Public Overrides Function ToString() As String
            Return names.GetJson
        End Function
    End Class
End Namespace
