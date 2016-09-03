#Region "Microsoft.VisualBasic::c903141e2986f316acdccad7b96c562c, ..\LibMySQL\Reflection\SQL_LDM\CreateTable.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
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
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports Oracle.LinuxCompatibility.MySQL.Reflection.Schema

Namespace Reflection.SQL

    ''' <summary>
    ''' Generate the CREATE TABLE sql of the target table schema class object.
    ''' (生成目标数据表模式的"CREATE TABLE" sql语句)
    ''' </summary>
    ''' <remarks>
    ''' Example SQL:
    ''' 
    ''' CREATE  TABLE `Table_Name` (
    '''   `Field1` INT UNSIGNED ZEROFILL NOT NULL DEFAULT 4444 ,
    '''   `Field2` VARCHAR(45) BINARY NOT NULL DEFAULT '534534' ,
    '''   `Field3` INT UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT ,
    '''  PRIMARY KEY (`Field1`, `Field2`, `Field3`) ,
    '''  UNIQUE INDEX `Field1_UNIQUE` (`Field1` ASC) ,
    '''  UNIQUE INDEX `Field2_UNIQUE` (`Field2` ASC) );
    ''' </remarks>
    Public Class CreateTableSQL

        Friend Const CREATE_TABLE As String = "CREATE  TABLE `{0}` ("
        Friend Const PRIMARY_KEY As String = "PRIMARY KEY ({0})"
        Friend Const UNIQUE_INDEX As String = "UNIQUE INDEX `%s_UNIQUE` (`%s` ASC)"

        ''' <summary>
        ''' Generate the 'CREATE TABLE' sql command.
        ''' (生成'CREATE TABLE' sql命令)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function FromSchema(Schema As Schema.Table) As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Dim sBuilder2 As StringBuilder = New StringBuilder(128)

            sBuilder.AppendFormat(CREATE_TABLE & vbCrLf, Schema.TableName)

            Dim Fields = Schema.Fields
            For i As Integer = 0 To Fields.Length - 1
                sBuilder.AppendLine("  " & Fields(i).ToString & " ,")
            Next

            Dim PrimaryField = Schema.PrimaryFields
            For Each PK As String In PrimaryField
                sBuilder2.AppendFormat("`{0}`, ", PK)
            Next
            sBuilder2.Remove(sBuilder2.Length - 2, 2)
            sBuilder.AppendFormat(PRIMARY_KEY & vbCrLf, sBuilder2.ToString)

            Dim UniqueFields = Schema.UniqueFields
            If UniqueFields.Count > 0 Then
                sBuilder.Append(" ,")
            End If
            For Each UniqueField As String In UniqueFields
                sBuilder.AppendLine(UNIQUE_INDEX.Replace("%s", UniqueField) & " ,")
            Next
            sBuilder.Remove(sBuilder.Length - 3, 3)
            sBuilder.Append(");") 'End of the sql statement

            Return sBuilder.ToString
        End Function
    End Class
End Namespace

