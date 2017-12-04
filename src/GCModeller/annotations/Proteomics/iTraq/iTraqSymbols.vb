#Region "Microsoft.VisualBasic::a5c5e2cb3ac213b863e0eb2276ddf690, ..\GCModeller\annotations\Proteomics\iTraq\iTraqSymbols.vb"

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

Imports Microsoft.VisualBasic.Data.csv

''' <summary>
''' iTraq信号标记替换
''' </summary>
<Template("iTraq")> Public Class iTraqSymbols

    ''' <summary>
    ''' iTraq信号标记
    ''' </summary>
    ''' <returns></returns>
    Public Property Symbol As String
    ''' <summary>
    ''' 将质谱实验下机数据转录结果文件之中的信号标记<see cref="Symbol"/>替换为用户的样品<see cref="SampleID"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property SampleID As String
    ''' <summary>
    ''' 样品进行数据分析的时候所使用的名称
    ''' </summary>
    ''' <returns></returns>
    Public Property AnalysisID As String
    Public Property SampleGroup As String

    Public Overrides Function ToString() As String
        Return $"{Symbol} -> {SampleID}"
    End Function
End Class

