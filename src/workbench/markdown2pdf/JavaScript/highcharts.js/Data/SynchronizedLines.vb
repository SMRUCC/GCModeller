#Region "Microsoft.VisualBasic::55c26a994a28ecd5d22c9b3af492119d, markdown2pdf\JavaScript\highcharts.js\Data\SynchronizedLines.vb"

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

    ' Class SynchronizedLines
    ' 
    '     Properties: datasets, xData
    ' 
    ' Class LineDataSet
    ' 
    '     Properties: data, max, name, type, unit
    '                 valueDecimals
    ' 
    ' /********************************************************************************/

#End Region

Public Class SynchronizedLines

    Public Property xData As Double()
    Public Property datasets As LineDataSet()

End Class

Public Class LineDataSet

    Public Property name As String
    Public Property type As String
    Public Property unit As String
    Public Property valueDecimals As Integer
    Public Property data As Double()
    Public Property max As Double?

End Class
