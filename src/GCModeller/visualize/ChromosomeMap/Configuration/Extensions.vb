#Region "Microsoft.VisualBasic::a84d04d85e62f3dc29da020404f07ada, GCModeller\visualize\ChromosomeMap\Configuration\Extensions.vb"

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


    ' Code Statistics:

    '   Total Lines: 47
    '    Code Lines: 35
    ' Comment Lines: 6
    '   Blank Lines: 6
    '     File Size: 1.85 KB


    '     Module Extensions
    ' 
    '         Function: FromConfig, GetFlagHeight, GetLineHeight, Load
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Oracle.Java.IO.Properties

Namespace Configuration

    Public Module Extensions

        <Extension>
        Public Function FromConfig(config As Config) As ChromosomeMap.DrawingDevice
            Dim Data As DataReader = config.ToConfigurationModel
            Dim Device As New ChromosomeMap.DrawingDevice(Data)
            Return Device
        End Function

        <Extension> Public Function GetFlagHeight(config As Config) As Integer
            If String.Equals(config.FLAG_HEIGHT, Regex.Match(config.LineHeight, "\d+").Value) Then '是一个数字
                Return Val(config.FLAG_HEIGHT)
            Else '是一个表达式
                Dim value As Double = Val(Regex.Match(config.FLAG_HEIGHT, "\d+(.\d+)?").Value)
                value = config.GeneObjectHeight * value
                Return value
            End If
        End Function

        <Extension> Public Function GetLineHeight(config As Config) As Integer
            If String.Equals(config.LineHeight, Regex.Match(config.LineHeight, "\d+").Value) Then '是一个数字
                Return Val(config.LineHeight)
            Else '是一个表达式
                Dim value As Double = Val(Regex.Match(config.LineHeight, "\d+(.\d+)?").Value)
                value = config.GeneObjectHeight * value
                Return value
            End If
        End Function

        ''' <summary>
        ''' 从文件之中读取配置数据
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Load(path$) As Config
            Return Oracle.Java.IO.Properties.Properties.Load(path).FillObject(Of Config)()
        End Function
    End Module
End Namespace
