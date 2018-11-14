#Region "Microsoft.VisualBasic::9597a2cfcf72fb72c92cb3e0c901004b, Model\Genotype.vb"

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

' Structure Genotype
' 
'     Function: ToString
' 
' 
' /********************************************************************************/

#End Region

''' <summary>
''' 目标细胞模型的基因组模型
''' </summary>
Public Structure Genotype

    ''' <summary>
    ''' 假设基因组之中的基因型定义信息全部都是由中心法则来构成的
    ''' </summary>
    ''' <remarks>
    ''' 请注意，当前的模块之中所定义的计算模型和GCMarkup之类的数据模型在看待基因组的构成上面的角度是有一些差异的：
    ''' 
    ''' 例如，对于复制子的描述上面，GCMarkup数据模型之中是更加倾向于将复制子分开进行描述的，这样子
    ''' 会更加的方便人类进行模型文件的阅读
    ''' 而在本计算模型之中，因为计算模型是计算机程序所阅读的，并且在实际的生命活动之中，染色体和质粒是在同一个环境之中
    ''' 工作的，所以在计算模型之中，没有太多的复制子的概念，而是将他们都合并到当前的这个中心法则列表对象之中，作为一个
    ''' 整体来进行看待
    ''' </remarks>
    Dim centralDogmas As CentralDogma()

    Public Overrides Function ToString() As String
        Return $"{centralDogmas.Length} genes"
    End Function
End Structure


