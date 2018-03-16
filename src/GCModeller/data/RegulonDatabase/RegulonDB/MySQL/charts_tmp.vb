#Region "Microsoft.VisualBasic::01dd5eb5ea2b17259f11e9ae7381692c, data\RegulonDatabase\RegulonDB\MySQL\charts_tmp.vb"

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

    ' Class charts_tmp
    ' 
    '     Properties: chart_id, chart_name, chart_title, chart_type, number_option
    '                 object_name, query_number, title_x, title_y
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @3/16/2018 10:40:14 PM


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace RegulonDB.Tables

''' <summary>
''' ```SQL
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
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("charts_tmp", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `charts_tmp` (
  `chart_name` varchar(150) NOT NULL,
  `chart_type` varchar(150) NOT NULL,
  `chart_title` varchar(150) DEFAULT NULL,
  `title_x` varchar(150) DEFAULT NULL,
  `title_y` varchar(150) DEFAULT NULL,
  `object_name` varchar(150) DEFAULT NULL,
  `number_option` decimal(10,0) DEFAULT NULL,
  `query_number` decimal(10,0) DEFAULT NULL,
  `chart_id` decimal(15,5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class charts_tmp: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("chart_name"), NotNull, DataType(MySqlDbType.VarChar, "150"), Column(Name:="chart_name")> Public Property chart_name As String
    <DatabaseField("chart_type"), NotNull, DataType(MySqlDbType.VarChar, "150"), Column(Name:="chart_type")> Public Property chart_type As String
    <DatabaseField("chart_title"), DataType(MySqlDbType.VarChar, "150"), Column(Name:="chart_title")> Public Property chart_title As String
    <DatabaseField("title_x"), DataType(MySqlDbType.VarChar, "150"), Column(Name:="title_x")> Public Property title_x As String
    <DatabaseField("title_y"), DataType(MySqlDbType.VarChar, "150"), Column(Name:="title_y")> Public Property title_y As String
    <DatabaseField("object_name"), DataType(MySqlDbType.VarChar, "150"), Column(Name:="object_name")> Public Property object_name As String
    <DatabaseField("number_option"), DataType(MySqlDbType.Decimal), Column(Name:="number_option")> Public Property number_option As Decimal
    <DatabaseField("query_number"), DataType(MySqlDbType.Decimal), Column(Name:="query_number")> Public Property query_number As Decimal
    <DatabaseField("chart_id"), NotNull, DataType(MySqlDbType.Decimal), Column(Name:="chart_id")> Public Property chart_id As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `charts_tmp` (`chart_name`, `chart_type`, `chart_title`, `title_x`, `title_y`, `object_name`, `number_option`, `query_number`, `chart_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `charts_tmp` (`chart_name`, `chart_type`, `chart_title`, `title_x`, `title_y`, `object_name`, `number_option`, `query_number`, `chart_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `charts_tmp` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `charts_tmp` SET `chart_name`='{0}', `chart_type`='{1}', `chart_title`='{2}', `title_x`='{3}', `title_y`='{4}', `object_name`='{5}', `number_option`='{6}', `query_number`='{7}', `chart_id`='{8}' WHERE ;</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `charts_tmp` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `charts_tmp` (`chart_name`, `chart_type`, `chart_title`, `title_x`, `title_y`, `object_name`, `number_option`, `query_number`, `chart_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, chart_name, chart_type, chart_title, title_x, title_y, object_name, number_option, query_number, chart_id)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{chart_name}', '{chart_type}', '{chart_title}', '{title_x}', '{title_y}', '{object_name}', '{number_option}', '{query_number}', '{chart_id}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `charts_tmp` (`chart_name`, `chart_type`, `chart_title`, `title_x`, `title_y`, `object_name`, `number_option`, `query_number`, `chart_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, chart_name, chart_type, chart_title, title_x, title_y, object_name, number_option, query_number, chart_id)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `charts_tmp` SET `chart_name`='{0}', `chart_type`='{1}', `chart_title`='{2}', `title_x`='{3}', `title_y`='{4}', `object_name`='{5}', `number_option`='{6}', `query_number`='{7}', `chart_id`='{8}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
Public Function Clone() As charts_tmp
                  Return DirectCast(MyClass.MemberwiseClone, charts_tmp)
              End Function
End Class


End Namespace

