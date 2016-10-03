#Region "Microsoft.VisualBasic::18d2eefb0bc91e36cee11039f0ceec83, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\lookup.vb"

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
''' DROP TABLE IF EXISTS `lookup`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `lookup` (
'''   `lookup_id` varchar(16) DEFAULT NULL,
'''   `lookup_name` varchar(2000) DEFAULT NULL,
'''   `lookup_category` varchar(100) NOT NULL,
'''   `lookup_source` varchar(20) NOT NULL,
'''   `lookup_reference` varchar(500) DEFAULT NULL,
'''   `lookup_object_id` varchar(255) DEFAULT NULL,
'''   `lookup_accesion_id` varchar(100) NOT NULL,
'''   `lookup_context` varchar(100) DEFAULT NULL,
'''   `lookup_description` varchar(1000) NOT NULL,
'''   `lookup_lastupdate` varchar(100) DEFAULT NULL,
'''   `lookup_url` varchar(2000) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("lookup", Database:="regulondb_7_5")>
Public Class lookup: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("lookup_id"), DataType(MySqlDbType.VarChar, "16")> Public Property lookup_id As String
    <DatabaseField("lookup_name"), DataType(MySqlDbType.VarChar, "2000")> Public Property lookup_name As String
    <DatabaseField("lookup_category"), NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property lookup_category As String
    <DatabaseField("lookup_source"), NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property lookup_source As String
    <DatabaseField("lookup_reference"), DataType(MySqlDbType.VarChar, "500")> Public Property lookup_reference As String
    <DatabaseField("lookup_object_id"), DataType(MySqlDbType.VarChar, "255")> Public Property lookup_object_id As String
    <DatabaseField("lookup_accesion_id"), NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property lookup_accesion_id As String
    <DatabaseField("lookup_context"), DataType(MySqlDbType.VarChar, "100")> Public Property lookup_context As String
    <DatabaseField("lookup_description"), NotNull, DataType(MySqlDbType.VarChar, "1000")> Public Property lookup_description As String
    <DatabaseField("lookup_lastupdate"), DataType(MySqlDbType.VarChar, "100")> Public Property lookup_lastupdate As String
    <DatabaseField("lookup_url"), NotNull, DataType(MySqlDbType.VarChar, "2000")> Public Property lookup_url As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `lookup` (`lookup_id`, `lookup_name`, `lookup_category`, `lookup_source`, `lookup_reference`, `lookup_object_id`, `lookup_accesion_id`, `lookup_context`, `lookup_description`, `lookup_lastupdate`, `lookup_url`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `lookup` (`lookup_id`, `lookup_name`, `lookup_category`, `lookup_source`, `lookup_reference`, `lookup_object_id`, `lookup_accesion_id`, `lookup_context`, `lookup_description`, `lookup_lastupdate`, `lookup_url`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `lookup` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `lookup` SET `lookup_id`='{0}', `lookup_name`='{1}', `lookup_category`='{2}', `lookup_source`='{3}', `lookup_reference`='{4}', `lookup_object_id`='{5}', `lookup_accesion_id`='{6}', `lookup_context`='{7}', `lookup_description`='{8}', `lookup_lastupdate`='{9}', `lookup_url`='{10}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, lookup_id, lookup_name, lookup_category, lookup_source, lookup_reference, lookup_object_id, lookup_accesion_id, lookup_context, lookup_description, lookup_lastupdate, lookup_url)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, lookup_id, lookup_name, lookup_category, lookup_source, lookup_reference, lookup_object_id, lookup_accesion_id, lookup_context, lookup_description, lookup_lastupdate, lookup_url)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
