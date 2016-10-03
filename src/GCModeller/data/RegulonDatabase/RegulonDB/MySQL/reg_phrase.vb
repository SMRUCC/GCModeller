#Region "Microsoft.VisualBasic::ce0ea74562952e84393d71643127d859, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\reg_phrase.vb"

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
''' DROP TABLE IF EXISTS `reg_phrase`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `reg_phrase` (
'''   `reg_phrase_id` char(12) NOT NULL,
'''   `reg_phrase_description` varchar(255) NOT NULL,
'''   `regulation_ratio` varchar(20) DEFAULT NULL,
'''   `on_half_life` decimal(20,5) DEFAULT NULL,
'''   `off_half_life` decimal(20,5) DEFAULT NULL,
'''   `phrase` varchar(2000) NOT NULL,
'''   `reg_phrase_function` varchar(25) NOT NULL,
'''   `reg_phrase_note` varchar(2000) DEFAULT NULL,
'''   `reg_phrase_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("reg_phrase", Database:="regulondb_7_5")>
Public Class reg_phrase: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("reg_phrase_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property reg_phrase_id As String
    <DatabaseField("reg_phrase_description"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property reg_phrase_description As String
    <DatabaseField("regulation_ratio"), DataType(MySqlDbType.VarChar, "20")> Public Property regulation_ratio As String
    <DatabaseField("on_half_life"), DataType(MySqlDbType.Decimal)> Public Property on_half_life As Decimal
    <DatabaseField("off_half_life"), DataType(MySqlDbType.Decimal)> Public Property off_half_life As Decimal
    <DatabaseField("phrase"), NotNull, DataType(MySqlDbType.VarChar, "2000")> Public Property phrase As String
    <DatabaseField("reg_phrase_function"), NotNull, DataType(MySqlDbType.VarChar, "25")> Public Property reg_phrase_function As String
    <DatabaseField("reg_phrase_note"), DataType(MySqlDbType.VarChar, "2000")> Public Property reg_phrase_note As String
    <DatabaseField("reg_phrase_internal_comment"), DataType(MySqlDbType.Text)> Public Property reg_phrase_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `reg_phrase` (`reg_phrase_id`, `reg_phrase_description`, `regulation_ratio`, `on_half_life`, `off_half_life`, `phrase`, `reg_phrase_function`, `reg_phrase_note`, `reg_phrase_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `reg_phrase` (`reg_phrase_id`, `reg_phrase_description`, `regulation_ratio`, `on_half_life`, `off_half_life`, `phrase`, `reg_phrase_function`, `reg_phrase_note`, `reg_phrase_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `reg_phrase` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `reg_phrase` SET `reg_phrase_id`='{0}', `reg_phrase_description`='{1}', `regulation_ratio`='{2}', `on_half_life`='{3}', `off_half_life`='{4}', `phrase`='{5}', `reg_phrase_function`='{6}', `reg_phrase_note`='{7}', `reg_phrase_internal_comment`='{8}', `key_id_org`='{9}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, reg_phrase_id, reg_phrase_description, regulation_ratio, on_half_life, off_half_life, phrase, reg_phrase_function, reg_phrase_note, reg_phrase_internal_comment, key_id_org)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, reg_phrase_id, reg_phrase_description, regulation_ratio, on_half_life, off_half_life, phrase, reg_phrase_function, reg_phrase_note, reg_phrase_internal_comment, key_id_org)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
