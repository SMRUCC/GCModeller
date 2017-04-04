#Region "Microsoft.VisualBasic::04c66936375f26b6a4c443185b96f19d, ..\visualbasic.DBI\SQLite_Interface\Reflects\TableSchema.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language

Public Class TableSchema : Implements INamedValue
    Implements IEnumerable(Of SchemaCache)

    Public Property TableName As String Implements INamedValue.Key
    Public Property DatabaseFields As SchemaCache()

    ''' <summary>
    ''' FieldName, DbType
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PrimaryKey As SchemaCache()

    Sub New(type As Type)
        TableName = GetTableName(type)
        DatabaseFields = InternalGetSchemaCache(type)
        PrimaryKey =
            LinqAPI.Exec(Of SchemaCache) <= From field As SchemaCache
                                            In DatabaseFields
                                            Where field.FieldEntryPoint.IsPrimaryKey
                                            Select field
    End Sub

    Public Shared Function CreateObject(Of T As Class)() As TableSchema
        Return New TableSchema(type:=GetType(T))
    End Function

    Public Overrides Function ToString() As String
        Return TableName
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator(Of SchemaCache) Implements IEnumerable(Of SchemaCache).GetEnumerator
        For Each item In DatabaseFields
            Yield item
        Next
    End Function

    Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
        Yield GetEnumerator()
    End Function
End Class
