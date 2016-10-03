#Region "Microsoft.VisualBasic::7b38a80468fe572722adae1ae704ea4f, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\map_tmp.vb"

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
''' DROP TABLE IF EXISTS `map_tmp`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `map_tmp` (
'''   `map_id` varchar(12) DEFAULT NULL,
'''   `map_name` varchar(250) DEFAULT NULL,
'''   `map_description` varchar(4000) DEFAULT NULL,
'''   `map_type` varchar(1) DEFAULT NULL,
'''   `map_component` varchar(2000) DEFAULT NULL,
'''   `map_reaction_name` varchar(2000) DEFAULT NULL,
'''   `map_source` varchar(255) DEFAULT NULL,
'''   `map_status` varchar(255) DEFAULT NULL,
'''   `map_file_name` varchar(250) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("map_tmp", Database:="regulondb_7_5")>
Public Class map_tmp: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("map_id"), DataType(MySqlDbType.VarChar, "12")> Public Property map_id As String
    <DatabaseField("map_name"), DataType(MySqlDbType.VarChar, "250")> Public Property map_name As String
    <DatabaseField("map_description"), DataType(MySqlDbType.VarChar, "4000")> Public Property map_description As String
    <DatabaseField("map_type"), DataType(MySqlDbType.VarChar, "1")> Public Property map_type As String
    <DatabaseField("map_component"), DataType(MySqlDbType.VarChar, "2000")> Public Property map_component As String
    <DatabaseField("map_reaction_name"), DataType(MySqlDbType.VarChar, "2000")> Public Property map_reaction_name As String
    <DatabaseField("map_source"), DataType(MySqlDbType.VarChar, "255")> Public Property map_source As String
    <DatabaseField("map_status"), DataType(MySqlDbType.VarChar, "255")> Public Property map_status As String
    <DatabaseField("map_file_name"), DataType(MySqlDbType.VarChar, "250")> Public Property map_file_name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `map_tmp` (`map_id`, `map_name`, `map_description`, `map_type`, `map_component`, `map_reaction_name`, `map_source`, `map_status`, `map_file_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `map_tmp` (`map_id`, `map_name`, `map_description`, `map_type`, `map_component`, `map_reaction_name`, `map_source`, `map_status`, `map_file_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `map_tmp` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `map_tmp` SET `map_id`='{0}', `map_name`='{1}', `map_description`='{2}', `map_type`='{3}', `map_component`='{4}', `map_reaction_name`='{5}', `map_source`='{6}', `map_status`='{7}', `map_file_name`='{8}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, map_id, map_name, map_description, map_type, map_component, map_reaction_name, map_source, map_status, map_file_name)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, map_id, map_name, map_description, map_type, map_component, map_reaction_name, map_source, map_status, map_file_name)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
