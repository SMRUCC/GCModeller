﻿#Region "Microsoft.VisualBasic::1e6544ab2610e58baf94b53acc929b8e, Data\DataFrame\IO\Generic\DataSet.vb"

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

    '     Class DataSet
    ' 
    '         Properties: ID, MyHashCode
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: __getID, Copy, (+2 Overloads) LoadDataSet, SubSet, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language.Default

Namespace IO

    ''' <summary>
    ''' The numeric dataset, <see cref="DynamicPropertyBase(Of Double)"/>, <see cref="Double"/>.
    ''' (数值类型的数据集合，每一个数据实体对象都有自己的编号以及数据属性)
    ''' </summary>
    Public Class DataSet : Inherits DynamicPropertyBase(Of Double)
        Implements INamedValue

        Public Overridable Property ID As String Implements INamedValue.Key

        Protected Overrides ReadOnly Property MyHashCode As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return ID.GetHashCode
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(id$)
            Me.ID = id
            Me.Properties = New Dictionary(Of String, Double)
        End Sub

        ''' <summary>
        ''' Copy prop[erty value
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Function Copy() As DataSet
            Return New DataSet With {
                .ID = ID,
                .Properties = New Dictionary(Of String, Double)(Properties)
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"{ID} has ({Properties.Keys.Take(5).JoinBy(", ")}...) {MyBase.ToString}"
        End Function

        ''' <summary>
        ''' 直接使用<paramref name="labels"/>取出<see cref="Properties"/>之中的一个子集
        ''' 对于不存在的属性，默认值为零
        ''' </summary>
        ''' <param name="labels"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function SubSet(labels As IEnumerable(Of String)) As DataSet
            Return New DataSet With {
                .ID = ID,
                .Properties = labels _
                    .ToDictionary(Function(x) x,
                                  Function(x) Me(x))
            }
        End Function

        ''' <summary>
        ''' <paramref name="uidMap"/>一般情况下会自动进行判断，不需要具体的设置
        ''' </summary>
        ''' <param name="path"></param>
        ''' <param name="uidMap">
        ''' 默认是使用csv文件的第一行第一个单元格中的内容作为标识符，但是有时候可能标识符不是在第一列的，则这个时候就需要对这个参数进行赋值了
        ''' </param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadDataSet(path$,
                                           Optional uidMap$ = Nothing,
                                           Optional tsv As Boolean = False,
                                           Optional encoding As Encoding = Nothing) As IEnumerable(Of DataSet)
            Return EntityObject.LoadDataSet(path, uidMap, tsv, encoding).AsDataSet
        End Function

        Public Shared Function LoadDataSet(Of T As DataSet)(path$,
                                                            Optional uidMap$ = Nothing,
                                                            Optional encoding As Encoding = Nothing) As IEnumerable(Of T)

            Dim mapFrom$ = uidMap Or New DefaultValue(Of String) With {
                .lazy = New Func(Of String)(Function() __getID(path)).AsLazy
            }
            Return path.LoadCsv(Of T)(
                explicit:=False,
                maps:={{mapFrom, NameOf(DataSet.ID)}},
                encoding:=encoding
            )
        End Function

        Private Shared Function __getID(path$) As String
            Dim first As New RowObject(path.ReadFirstLine)
            Return first.First
        End Function
    End Class
End Namespace
