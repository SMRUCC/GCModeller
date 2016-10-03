#Region "Microsoft.VisualBasic::190ae4801d7df912b3eec71782d2c7c9, ..\R.Bioconductor\RDotNET\R.NET\DataFrameRowAttribute.vb"

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

Imports System.Collections.Generic
Imports System.Linq
Imports System.Reflection

Friend Delegate Sub Map(from As DataFrameRow, [to] As Object)

''' <summary>
''' Indicates the class with the attribute represents rows of certain data frames.
''' </summary>
<AttributeUsage(AttributeTargets.[Class], Inherited := False, AllowMultiple := False)> _
Public Class DataFrameRowAttribute
	Inherits Attribute
	Private ReadOnly cache As Dictionary(Of Type, Map)

	''' <summary>
	''' Initializes a new instance.
	''' </summary>
	Public Sub New()
		Me.cache = New Dictionary(Of Type, Map)()
	End Sub

	Friend Function Convert(Of TRow As {Class, New})(row As DataFrameRow) As TRow
		Dim rowType = GetType(TRow)
        Dim map As Map = Nothing
		If Not Me.cache.TryGetValue(rowType, map) Then
			map = CreateMap(rowType)
			Me.cache.Add(rowType, map)
		End If
		Dim result = Activator.CreateInstance(rowType)
		map(row, result)
		Return DirectCast(result, TRow)
	End Function

	Private Shared Function CreateMap(rowType As Type) As Map
        Dim tuples = (From [property] In rowType.GetProperties() Let attribute = DirectCast([property].GetCustomAttributes(GetType(DataFrameColumnAttribute), True).SingleOrDefault(), DataFrameColumnAttribute) Where attribute IsNot Nothing Select Tuple.Create(attribute, [property].GetSetMethod())).ToArray()
        Return Sub(from, [to]) Call Map([from], [to], tuples)
    End Function

	Private Shared Sub Map(from As DataFrameRow, [to] As Object, tuples As Tuple(Of DataFrameColumnAttribute, MethodInfo)())
		Dim names = from.DataFrame.ColumnNames
        For Each t As Object In tuples
            Dim attribute = t.Item1
            Dim setter = t.Item2
            Dim index = attribute.GetIndex(names)
            setter.Invoke([to], New Object() {from.GetInnerValue(index)})
        Next
	End Sub
End Class
