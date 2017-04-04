#Region "Microsoft.VisualBasic::293b2ded3749cffbcd85b28e38ac3022, ..\visualbasic.DBI\LibMySQL\Reflection\SQL_LDM\Insert.vb"

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

Imports System.Reflection
Imports System.Text
Imports Microsoft.VisualBasic
Imports Oracle.LinuxCompatibility.MySQL.Reflection.Schema

Namespace Reflection.SQL

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Example SQL:
    ''' 
    ''' INSERT INTO `TableName` (`Field1`, `Field2`, `Field3`) VALUES ('1', '1', '1');
    ''' </remarks>
    Public Class Insert(Of Schema) : Inherits SQL

        ''' <summary>
        ''' INSERT INTO `TableName` (`Field1`, `Field2`, `Field3`, ...) VALUES ('{0}', '{1}', '{2}', ...);
        ''' </summary>
        ''' <remarks></remarks>
        Friend InsertSQL As String

        ''' <summary>
        ''' Generate the INSERT sql command of the instance of the specific type of 'Schema'.
        ''' (生成特定的'Schema'数据类型实例的 'INSERT' sql命令)
        ''' </summary>
        ''' <param name="value">The instance to generate this command of type 'Schema'</param>
        ''' <returns>INSERT sql text</returns>
        ''' <remarks></remarks>
        Public Function Generate(value As Schema) As String
            Dim valuesbuffer As List(Of String) = New List(Of String)

            For Each Field In MyBase._schemaInfo.Fields
                Dim s_value As String = Field.PropertyInfo.GetValue(value, Nothing).ToString
                Call valuesbuffer.Add(s_value)
            Next

            Return String.Format(InsertSQL, valuesbuffer.ToArray)
        End Function

        Public Shared Widening Operator CType(schema As Reflection.Schema.Table) As Insert(Of Schema)
            Return New Insert(Of Schema) With {
                ._schemaInfo = schema,
                .InsertSQL = GenerateInsertSql(schema)
            }
        End Operator
    End Class
End Namespace
