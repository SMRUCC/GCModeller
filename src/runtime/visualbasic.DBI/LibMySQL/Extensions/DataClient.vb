#Region "Microsoft.VisualBasic::c57ca95111536862b1767d8aee467054, ..\visualbasic.DBI\LibMySQL\Extensions\DataClient.vb"

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

Imports System.Runtime.CompilerServices
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports Oracle.LinuxCompatibility.MySQL.Reflection.Schema

Public Module DataClient

    ''' <summary>
    ''' 从数据库之中加载所有的数据到程序的内存之中，只推荐表的数据量比较小的使用，
    ''' 使用这个函数加载完数据到内存之中后，进行内存之中的查询操作，会很明显提升应用程序的执行性能
    ''' 
    ''' ```SQL
    ''' SELECT * FROM `{table.Database}`.`{table.TableName}`;
    ''' ```
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="mysql"></param>
    ''' <returns></returns>
    <Extension>
    Public Function SelectALL(Of T As SQLTable)(mysql As MySQL) As T()
        Dim table As TableName = GetType(T).GetAttribute(Of TableName)
        Dim SQL$ = $"SELECT * FROM `{table.Database}`.`{table.Name}`;"
        Return mysql.Query(Of T)(SQL)
    End Function
End Module

