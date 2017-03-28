#Region "Microsoft.VisualBasic::1588dae07f7766032ccb076f099fffc4, ..\visualbasic.DBI\SQLite_Interface\TableDump.vb"

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

Imports System.Data.Common
Imports System.Text
Imports System.Data.Linq.Mapping
Imports System.Data.Entity.Core

<Table(Name:="table_schema")>
Public Class TableDump

    <Column(Name:="guid", DbType:="int", IsPrimaryKey:=True)> Public Property Guid As Integer
    <Column(Name:="table_name", DbType:="varchar(128)")> Public Property TableName As String
    <Column(Name:="field", DbType:="varchar(64)")> Public Property FieldName As String
    <Column(Name:="dbtype", DbType:="varchar(32)")> Public Property DbType As String
    ''' <summary>
    ''' 1或者0
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Column(Name:="is_primary_key", DbType:="int")> Public Property IsPrimaryKey As Integer

    Public Overrides Function ToString() As String
        Return SchemaCache.CreateInsertSQL(Me)
    End Function
End Class
