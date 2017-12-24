#Region "Microsoft.VisualBasic::1d5558e87724b7f4d543a6429aba164b, ..\httpd\WebCloud\SMRUCC.WebCloud.highcharts\Common\AbstractSerial.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.WebCloud.highcharts.PieChart

Public MustInherit Class AbstractSerial(Of T)

    Public Property type As String
    Public Property name As String

    Public Overridable Property data As T()

    Public Overrides Function ToString() As String
        Return $"Dim {name} As {type} = {data.GetJson}"
    End Function
End Class

Public Class GenericDataSerial : Inherits AbstractSerial(Of Double)

End Class

''' <summary>
''' Object array
''' </summary>
Public Class serial : Inherits AbstractSerial(Of Object)

    ''' <summary>
    ''' + <see cref="Double"/>
    ''' + <see cref="pieData"/>
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Property data As Object()
    Public Property dataLabels As dataLabels
    Public Property tooltip As tooltip
    Public Property colorByPoint As Boolean?
End Class
