﻿#Region "Microsoft.VisualBasic::9e6d99b19ed54d4c9d48e38453567107, Model\Phenotype.vb"

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

    ' Structure Phenotype
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' 在进行计算的时候，是以代谢反应过程为基础的
''' 代谢途径等信息可以在导出结果之后做分类分析
''' 所以在模型之中并没有包含有代谢途径的信息？？？
''' </summary>
Public Structure Phenotype

    ''' <summary>
    ''' enzyme = protein + RNA
    ''' </summary>
    Public enzymes As String()

    ''' <summary>
    ''' 
    ''' </summary>
    Public fluxes As Reaction()

    ''' <summary>
    ''' Some protein is not an enzyme
    ''' </summary>
    Public proteins As Protein()

End Structure

