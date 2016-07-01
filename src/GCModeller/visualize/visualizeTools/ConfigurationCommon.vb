#Region "Microsoft.VisualBasic::009a23e0b5a022deb014d0545af1b4ab, ..\GCModeller\visualize\visualizeTools\ConfigurationCommon.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Drawing
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.ChromosomeMap

Public Module ConfigurationCommon

    Public Function FromConfig(config As Configurations) As ChromosomeMap.DrawingDevice
        Dim Data As Conf = config.ToConfigurationModel
        Dim DeviceSize = Data.Resolution
        Dim Device As ChromosomeMap.DrawingDevice = New ChromosomeMap.DrawingDevice(Data)
        Return Device
    End Function

    <Extension> Public Function GetFlagHeight(config As Configurations) As Integer
        If String.Equals(config.FLAG_HEIGHT, Regex.Match(config.LineHeight, "\d+").Value) Then '是一个数字
            Return Val(config.FLAG_HEIGHT)
        Else '是一个表达式
            Dim value As Double = Val(Regex.Match(config.FLAG_HEIGHT, "\d+(.\d+)?").Value)
            value = config.GeneObjectHeight * value
            Return value
        End If
    End Function

    <Extension> Public Function GetLineHeight(config As Configurations) As Integer
        If String.Equals(config.LineHeight, Regex.Match(config.LineHeight, "\d+").Value) Then '是一个数字
            Return Val(config.LineHeight)
        Else '是一个表达式
            Dim value As Double = Val(Regex.Match(config.LineHeight, "\d+(.\d+)?").Value)
            value = config.GeneObjectHeight * value
            Return value
        End If
    End Function
End Module

