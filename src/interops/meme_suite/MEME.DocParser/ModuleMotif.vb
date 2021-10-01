#Region "Microsoft.VisualBasic::1cd9d3851f344eec8c8ab10f03675050, meme_suite\MEME.DocParser\ModuleMotif.vb"

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

    ' Class ModuleMotif
    ' 
    '     Properties: [class], [module], category, describ, evalue
    '                 family, motif, regulators, source, tom
    '                 type
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' 相当于Regulon信息
''' </summary>
Public Class ModuleMotif

    ''' <summary>
    ''' 模块的名称
    ''' </summary>
    ''' <returns></returns>
    Public Property [module] As String
    ''' <summary>
    ''' 产生这个motif的基因的列表
    ''' </summary>
    ''' <returns></returns>
    Public Property source As String()
    ''' <summary>
    ''' 这个模块的描述信息
    ''' </summary>
    ''' <returns></returns>
    Public Property describ As String
    ''' <summary>
    ''' A
    ''' </summary>
    ''' <returns></returns>
    Public Property type As String
    ''' <summary>
    ''' B
    ''' </summary>
    ''' <returns></returns>
    Public Property [class] As String
    ''' <summary>
    ''' C
    ''' </summary>
    ''' <returns></returns>
    Public Property category As String
    Public Property motif As String
    Public Property family As String
    Public Property regulators As String()
    Public Property evalue As Double
    Public Property tom As String
End Class
