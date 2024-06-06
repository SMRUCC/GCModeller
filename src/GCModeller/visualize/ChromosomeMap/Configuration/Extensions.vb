#Region "Microsoft.VisualBasic::341cdad17a1ad1bd0aa8937c1574ac85, visualize\ChromosomeMap\Configuration\Extensions.vb"

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

    '   Total Lines: 48
    '    Code Lines: 36 (75.00%)
    ' Comment Lines: 6 (12.50%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (12.50%)
    '     File Size: 1.88 KB


    '     Module Extensions
    ' 
    '         Function: FromConfig, GetFlagHeight, GetLineHeight, Load
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Data.csv.IO.Properties

Namespace Configuration

    Public Module Extensions

        <Extension>
        Public Function FromConfig(config As Config) As ChromosomeMap.DrawingDevice
            Dim Data As DataReader = config.ToConfigurationModel
            Dim Device As New ChromosomeMap.DrawingDevice(Data)
            Return Device
        End Function

        <Extension>
        Public Function GetFlagHeight(config As Config) As Integer
            If String.Equals(config.FLAG_HEIGHT, Regex.Match(config.LineHeight, "\d+").Value) Then '是一个数字
                Return Val(config.FLAG_HEIGHT)
            Else '是一个表达式
                Dim value As Double = Val(Regex.Match(config.FLAG_HEIGHT, "\d+(.\d+)?").Value)
                value = config.GeneObjectHeight * value
                Return value
            End If
        End Function

        <Extension>
        Public Function GetLineHeight(config As Config) As Integer
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
            Return Properties.Load(path).FillObject(Of Config)()
        End Function
    End Module
End Namespace
