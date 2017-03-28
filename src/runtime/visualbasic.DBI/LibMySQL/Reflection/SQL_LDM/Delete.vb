#Region "Microsoft.VisualBasic::ed16a903b6fde2a72bb4e066c9db6931, ..\visualbasic.DBI\LibMySQL\Reflection\SQL_LDM\Delete.vb"

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

Imports System.Text
Imports System.Reflection
Imports Oracle.LinuxCompatibility.MySQL.Reflection.Schema
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Reflection.SQL

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="Schema"></typeparam>
    ''' <remarks>
    ''' Example SQL:
    ''' 
    ''' DELETE FROM `TableName` WHERE `IndexFieldName`='value';
    ''' </remarks>
    Public Class Delete(Of Schema) : Inherits SQL

        Public Function Generate(Record As Schema) As String
            Dim [String] As String = MyBase._schemaInfo.IndexProperty.GetValue(Record, Nothing).ToString
            Return System.String.Format(GenerateDeleteSql(_schemaInfo), [String])
        End Function

        Public Shared Widening Operator CType(tbl As Reflection.Schema.Table) As Delete(Of Schema)
            Return New Delete(Of Schema) With {._schemaInfo = tbl}
        End Operator
    End Class
End Namespace
