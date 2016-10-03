#Region "Microsoft.VisualBasic::bdb1d711a32f2807cc89cb8f41f5482a, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\repeat.vb"

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
''' DROP TABLE IF EXISTS `repeat`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `repeat` (
'''   `repeat_id` char(12) NOT NULL,
'''   `repeat_posleft` decimal(10,0) NOT NULL,
'''   `repeat_posright` decimal(10,0) NOT NULL,
'''   `repeat_family` varchar(255) DEFAULT NULL,
'''   `repeat_note` varchar(2000) DEFAULT NULL,
'''   `repeat_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("repeat", Database:="regulondb_7_5")>
Public Class repeat: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("repeat_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property repeat_id As String
    <DatabaseField("repeat_posleft"), NotNull, DataType(MySqlDbType.Decimal)> Public Property repeat_posleft As Decimal
    <DatabaseField("repeat_posright"), NotNull, DataType(MySqlDbType.Decimal)> Public Property repeat_posright As Decimal
    <DatabaseField("repeat_family"), DataType(MySqlDbType.VarChar, "255")> Public Property repeat_family As String
    <DatabaseField("repeat_note"), DataType(MySqlDbType.VarChar, "2000")> Public Property repeat_note As String
    <DatabaseField("repeat_internal_comment"), DataType(MySqlDbType.Text)> Public Property repeat_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `repeat` (`repeat_id`, `repeat_posleft`, `repeat_posright`, `repeat_family`, `repeat_note`, `repeat_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `repeat` (`repeat_id`, `repeat_posleft`, `repeat_posright`, `repeat_family`, `repeat_note`, `repeat_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `repeat` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `repeat` SET `repeat_id`='{0}', `repeat_posleft`='{1}', `repeat_posright`='{2}', `repeat_family`='{3}', `repeat_note`='{4}', `repeat_internal_comment`='{5}', `key_id_org`='{6}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, repeat_id, repeat_posleft, repeat_posright, repeat_family, repeat_note, repeat_internal_comment, key_id_org)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, repeat_id, repeat_posleft, repeat_posright, repeat_family, repeat_note, repeat_internal_comment, key_id_org)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
