#Region "Microsoft.VisualBasic::def1b9729f5e2c4ebd78cd9afcfd93ea, markdown2pdf\JavaScript\highcharts.js\Common\AbstractSerial.vb"

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

    ' Class AbstractSerial
    ' 
    '     Properties: data, name, type
    ' 
    '     Function: ToString
    ' 
    ' Class GenericDataSerial
    ' 
    '     Properties: pointPlacement
    ' 
    ' Class serial
    ' 
    '     Properties: colorByPoint, data, dataLabels, tooltip
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.WebCloud.JavaScript.highcharts.PieChart

''' <summary>
''' With basic ``type``, ``name`` and ``data``.
''' </summary>
''' <typeparam name="T"></typeparam>
Public MustInherit Class AbstractSerial(Of T)

    ''' <summary>
    ''' 数据序列的展示类型。
    ''' </summary>
    ''' <returns></returns>
    Public Property type As String
    ''' <summary>
    ''' 数据序列的名称。
    ''' </summary>
    ''' <returns></returns>
    Public Property name As String

    Public Overridable Property data As T()

    Public Overrides Function ToString() As String
        Return $"Dim {name} As {type} = {data.GetJson}"
    End Function
End Class

Public Class GenericDataSerial : Inherits AbstractSerial(Of Double)
    Public Property pointPlacement As String
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
