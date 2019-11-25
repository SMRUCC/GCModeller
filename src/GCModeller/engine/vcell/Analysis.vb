#Region "Microsoft.VisualBasic::c02e48e00423f152e3a2ce59f11bedeb, vcell\Analysis.vb"

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

    ' Module CLI
    ' 
    '     Function: UnionCompareMatrix
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.DATA

Partial Module CLI

    ''' <summary>
    ''' 将多个实验设计得到的指定时间点的模拟计算数据提取出来然后合并为一个矩阵进行后续分析
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/union")>
    <Usage("/union /raw <*.csv data_directory> /time <timepoint> [/out <union_matrix.csv>]")>
    Public Function UnionCompareMatrix(args As CommandLine) As Integer
        Dim raw$ = args <= "/raw"
        Dim time$ = args <= "/time"
        Dim out$ = args("/out") Or $"{raw.TrimDIR},time={time}.matrix.csv"
        Dim matrix As New DataFrame



        Return matrix _
            .SaveTable(out) _
            .CLICode
    End Function
End Module
