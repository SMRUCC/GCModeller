#Region "Microsoft.VisualBasic::18ed226e8754603864e45c782a6bd651, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\effector.vb"

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
''' DROP TABLE IF EXISTS `effector`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `effector` (
'''   `effector_id` char(12) NOT NULL,
'''   `effector_name` varchar(255) NOT NULL,
'''   `category` varchar(10) DEFAULT NULL,
'''   `effector_type` varchar(100) DEFAULT NULL,
'''   `effector_note` varchar(2000) DEFAULT NULL,
'''   `effector_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("effector", Database:="regulondb_7_5")>
Public Class effector: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("effector_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property effector_id As String
    <DatabaseField("effector_name"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property effector_name As String
    <DatabaseField("category"), DataType(MySqlDbType.VarChar, "10")> Public Property category As String
    <DatabaseField("effector_type"), DataType(MySqlDbType.VarChar, "100")> Public Property effector_type As String
    <DatabaseField("effector_note"), DataType(MySqlDbType.VarChar, "2000")> Public Property effector_note As String
    <DatabaseField("effector_internal_comment"), DataType(MySqlDbType.Text)> Public Property effector_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `effector` (`effector_id`, `effector_name`, `category`, `effector_type`, `effector_note`, `effector_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `effector` (`effector_id`, `effector_name`, `category`, `effector_type`, `effector_note`, `effector_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `effector` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `effector` SET `effector_id`='{0}', `effector_name`='{1}', `category`='{2}', `effector_type`='{3}', `effector_note`='{4}', `effector_internal_comment`='{5}', `key_id_org`='{6}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, effector_id, effector_name, category, effector_type, effector_note, effector_internal_comment, key_id_org)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, effector_id, effector_name, category, effector_type, effector_note, effector_internal_comment, key_id_org)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
