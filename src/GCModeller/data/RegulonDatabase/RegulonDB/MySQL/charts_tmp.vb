#Region "Microsoft.VisualBasic::b4e3de329b7d4ee5a15e05d4688e8c60, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\charts_tmp.vb"

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

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:08:18 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace RegulonDB.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `charts_tmp`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `charts_tmp` (
'''   `chart_name` varchar(150) NOT NULL,
'''   `chart_type` varchar(150) NOT NULL,
'''   `chart_title` varchar(150) DEFAULT NULL,
'''   `title_x` varchar(150) DEFAULT NULL,
'''   `title_y` varchar(150) DEFAULT NULL,
'''   `object_name` varchar(150) DEFAULT NULL,
'''   `number_option` decimal(10,0) DEFAULT NULL,
'''   `query_number` decimal(10,0) DEFAULT NULL,
'''   `chart_id` decimal(15,5) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("charts_tmp", Database:="regulondb_7_5")>
Public Class charts_tmp: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("chart_name"), NotNull, DataType(MySqlDbType.VarChar, "150")> Public Property chart_name As String
    <DatabaseField("chart_type"), NotNull, DataType(MySqlDbType.VarChar, "150")> Public Property chart_type As String
    <DatabaseField("chart_title"), DataType(MySqlDbType.VarChar, "150")> Public Property chart_title As String
    <DatabaseField("title_x"), DataType(MySqlDbType.VarChar, "150")> Public Property title_x As String
    <DatabaseField("title_y"), DataType(MySqlDbType.VarChar, "150")> Public Property title_y As String
    <DatabaseField("object_name"), DataType(MySqlDbType.VarChar, "150")> Public Property object_name As String
    <DatabaseField("number_option"), DataType(MySqlDbType.Decimal)> Public Property number_option As Decimal
    <DatabaseField("query_number"), DataType(MySqlDbType.Decimal)> Public Property query_number As Decimal
    <DatabaseField("chart_id"), NotNull, DataType(MySqlDbType.Decimal)> Public Property chart_id As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `charts_tmp` (`chart_name`, `chart_type`, `chart_title`, `title_x`, `title_y`, `object_name`, `number_option`, `query_number`, `chart_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `charts_tmp` (`chart_name`, `chart_type`, `chart_title`, `title_x`, `title_y`, `object_name`, `number_option`, `query_number`, `chart_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `charts_tmp` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `charts_tmp` SET `chart_name`='{0}', `chart_type`='{1}', `chart_title`='{2}', `title_x`='{3}', `title_y`='{4}', `object_name`='{5}', `number_option`='{6}', `query_number`='{7}', `chart_id`='{8}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, chart_name, chart_type, chart_title, title_x, title_y, object_name, number_option, query_number, chart_id)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, chart_name, chart_type, chart_title, title_x, title_y, object_name, number_option, query_number, chart_id)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
