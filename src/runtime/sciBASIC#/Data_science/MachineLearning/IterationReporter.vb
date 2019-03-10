﻿#Region "Microsoft.VisualBasic::dddbd731a4d85881944d31a88c3434e8, Data_science\MachineLearning\IterationReporter.vb"

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

    ' Class IterationReporter
    ' 
    ' 
    '     Delegate Sub
    ' 
    '         Function: AttachReporter
    ' 
    ' Class Model
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' 用于报告基于迭代的机器学习算法的状态进度之类的信息的框架
''' </summary>
''' <remarks>
''' 这个对象模块应该是应用于训练部分的模块
''' </remarks>
Public MustInherit Class IterationReporter(Of T As Model)

    Protected reporter As DoReport

    Public Delegate Sub DoReport(iteration%, error#, model As T)

    Public Function AttachReporter(reporter As DoReport) As IterationReporter(Of T)
        Me.reporter = reporter
        Return Me
    End Function

    Public MustOverride Sub Train(Optional parallel As Boolean = False)

End Class

Public MustInherit Class Model

End Class
