#Region "Microsoft.VisualBasic::8792c53243d93ea9afebd7cc956d969c, ..\httpd\WebCloud\SMRUCC.WebCloud.d3js\Network\htmlwidget\JSON.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Namespace Network.htmlwidget

    Public Class JSON
        Public Property x As NetGraph
    End Class

    Public Class NetGraph
        Public Property links As Links
        Public Property nodes As Nodes
        Public Property options As Options
    End Class

    Public Class Options
        Public Property NodeID As String
        Public Property Group As String
        Public Property colourScale As String
        Public Property fontSize As Integer
        Public Property fontFamily As String
        Public Property clickTextSize As Integer
        Public Property linkDistance As Integer
        Public Property linkWidth As String
        Public Property charge As Integer
        Public Property opacity As Double
        Public Property zoom As Boolean
        Public Property legend As Boolean
        Public Property nodesize As Boolean
        Public Property radiusCalculation As String
        Public Property bounded As Boolean
        Public Property opacityNoHover As Double
        Public Property clickAction As String
    End Class

    Public Class Links
        Public Property source As Integer()
        Public Property target As Integer()
        Public Property colour As String()
    End Class

    Public Class Nodes
        Public Property name As String()
        Public Property group As Integer()
    End Class
End Namespace
