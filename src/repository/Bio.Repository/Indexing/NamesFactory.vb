#Region "Microsoft.VisualBasic::6db33cd81dbf487b48294df55fb0d5a9, Bio.Repository\Indexing\NamesFactory.vb"

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

    ' Interface NamesFactory
    ' 
    '     Function: GetNames, PopulateObjects
    ' 
    ' /********************************************************************************/

#End Region

Public Interface NamesFactory(Of T)

    ''' <summary>
    ''' 从该对象之中提取出名字的方法
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    Function GetNames(obj As T) As String()
    ''' <summary>
    ''' 这个函数是index的数据源
    ''' </summary>
    ''' <returns></returns>
    Function PopulateObjects() As IEnumerable(Of T)

End Interface
