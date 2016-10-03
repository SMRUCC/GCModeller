#Region "Microsoft.VisualBasic::730d4b29eb52b78aadd7fbc134213025, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\objects_properties_tmp.vb"

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
''' DROP TABLE IF EXISTS `objects_properties_tmp`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `objects_properties_tmp` (
'''   `object_id` varchar(12) DEFAULT NULL,
'''   `object_name` varchar(255) DEFAULT NULL,
'''   `object_type` varchar(50) NOT NULL,
'''   `object_strand` varchar(7) DEFAULT NULL,
'''   `object_posleft` decimal(10,0) DEFAULT NULL,
'''   `object_posright` decimal(10,0) DEFAULT NULL,
'''   `object_color` varchar(11) DEFAULT NULL,
'''   `tool_tip` varchar(4000) DEFAULT NULL,
'''   `line_type` decimal(10,0) DEFAULT NULL,
'''   `label_size` decimal(10,0) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("objects_properties_tmp", Database:="regulondb_7_5")>
Public Class objects_properties_tmp: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("object_id"), DataType(MySqlDbType.VarChar, "12")> Public Property object_id As String
    <DatabaseField("object_name"), DataType(MySqlDbType.VarChar, "255")> Public Property object_name As String
    <DatabaseField("object_type"), NotNull, DataType(MySqlDbType.VarChar, "50")> Public Property object_type As String
    <DatabaseField("object_strand"), DataType(MySqlDbType.VarChar, "7")> Public Property object_strand As String
    <DatabaseField("object_posleft"), DataType(MySqlDbType.Decimal)> Public Property object_posleft As Decimal
    <DatabaseField("object_posright"), DataType(MySqlDbType.Decimal)> Public Property object_posright As Decimal
    <DatabaseField("object_color"), DataType(MySqlDbType.VarChar, "11")> Public Property object_color As String
    <DatabaseField("tool_tip"), DataType(MySqlDbType.VarChar, "4000")> Public Property tool_tip As String
    <DatabaseField("line_type"), DataType(MySqlDbType.Decimal)> Public Property line_type As Decimal
    <DatabaseField("label_size"), DataType(MySqlDbType.Decimal)> Public Property label_size As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `objects_properties_tmp` (`object_id`, `object_name`, `object_type`, `object_strand`, `object_posleft`, `object_posright`, `object_color`, `tool_tip`, `line_type`, `label_size`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `objects_properties_tmp` (`object_id`, `object_name`, `object_type`, `object_strand`, `object_posleft`, `object_posright`, `object_color`, `tool_tip`, `line_type`, `label_size`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `objects_properties_tmp` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `objects_properties_tmp` SET `object_id`='{0}', `object_name`='{1}', `object_type`='{2}', `object_strand`='{3}', `object_posleft`='{4}', `object_posright`='{5}', `object_color`='{6}', `tool_tip`='{7}', `line_type`='{8}', `label_size`='{9}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, object_id, object_name, object_type, object_strand, object_posleft, object_posright, object_color, tool_tip, line_type, label_size)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, object_id, object_name, object_type, object_strand, object_posleft, object_posright, object_color, tool_tip, line_type, label_size)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
