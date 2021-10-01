#Region "Microsoft.VisualBasic::8c5f0c71ed8f6807d92c70ec250b62b5, markdown2pdf\JavaScript\highcharts.js\Charts\PieChart.vb"

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

    '     Class PieChart
    ' 
    '         Function: ToString
    ' 
    '     Class PieChart3D
    ' 
    ' 
    ' 
    '     Class pieOptions
    ' 
    '         Properties: allowPointSelect, cursor, depth, showInLegend
    ' 
    '     Class pieData
    ' 
    '         Properties: name, selected, sliced, y
    ' 
    '     Class VariablePieSerial
    ' 
    '         Properties: innerSize, minPointSize, zMin
    ' 
    '     Class VariablePieSerialData
    ' 
    '         Properties: name, y, z
    ' 
    '     Class VariablePieChart
    ' 
    '         Function: ChartType
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace PieChart

    Public Class PieChart : Inherits Highcharts(Of serial)

        Public Overrides Function ToString() As String
            Return title.ToString
        End Function
    End Class

    Public Class PieChart3D : Inherits Highcharts3D(Of serial)

    End Class

    Public Class pieOptions : Inherits seriesoptions
        Public Property allowPointSelect As Boolean
        Public Property cursor As String
        Public Property depth As String
        Public Property showInLegend As Boolean
    End Class

    Public Class pieData
        Public Property name As String
        Public Property y As Double
        Public Property sliced As Boolean
        Public Property selected As Boolean
    End Class

    Public Class VariablePieSerial : Inherits AbstractSerial(Of VariablePieSerialData)

        Public Property minPointSize As Double?
        Public Property innerSize As String
        Public Property zMin As Double?

    End Class

    Public Class VariablePieSerialData
        Public Property name As String
        Public Property y As Double?
        Public Property z As Double?
    End Class

    Public Class VariablePieChart : Inherits Highcharts(Of VariablePieSerial)

        Public Shared Function ChartType() As chart
            Return New chart With {.type = "variablepie"}
        End Function
    End Class
End Namespace
