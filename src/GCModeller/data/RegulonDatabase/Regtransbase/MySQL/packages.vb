#Region "Microsoft.VisualBasic::7b6283c03bb0e37b238473da0c1d4267, ..\GCModeller\data\RegulonDatabase\Regtransbase\MySQL\packages.vb"

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

REM  Dump @12/3/2015 8:07:30 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Regtransbase.MySQL

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `packages`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `packages` (
'''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
'''   `title` char(50) DEFAULT NULL,
'''   `annotator_id` int(11) DEFAULT NULL,
'''   `article_num` int(11) DEFAULT NULL,
'''   `pkg_state` int(11) NOT NULL DEFAULT '0',
'''   `pkg_state_date` char(10) DEFAULT NULL,
'''   `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
'''   PRIMARY KEY (`pkg_guid`),
'''   KEY `annotator_id` (`annotator_id`),
'''   CONSTRAINT `packages_ibfk_1` FOREIGN KEY (`annotator_id`) REFERENCES `db_users` (`id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("packages", Database:="dbregulation_update")>
Public Class packages: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pkg_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property pkg_guid As Long
    <DatabaseField("title"), DataType(MySqlDbType.VarChar, "50")> Public Property title As String
    <DatabaseField("annotator_id"), DataType(MySqlDbType.Int64, "11")> Public Property annotator_id As Long
    <DatabaseField("article_num"), DataType(MySqlDbType.Int64, "11")> Public Property article_num As Long
    <DatabaseField("pkg_state"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property pkg_state As Long
    <DatabaseField("pkg_state_date"), DataType(MySqlDbType.VarChar, "10")> Public Property pkg_state_date As String
    <DatabaseField("last_update"), NotNull, DataType(MySqlDbType.DateTime)> Public Property last_update As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `packages` (`pkg_guid`, `title`, `annotator_id`, `article_num`, `pkg_state`, `pkg_state_date`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `packages` (`pkg_guid`, `title`, `annotator_id`, `article_num`, `pkg_state`, `pkg_state_date`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `packages` WHERE `pkg_guid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `packages` SET `pkg_guid`='{0}', `title`='{1}', `annotator_id`='{2}', `article_num`='{3}', `pkg_state`='{4}', `pkg_state_date`='{5}', `last_update`='{6}' WHERE `pkg_guid` = '{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pkg_guid)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pkg_guid, title, annotator_id, article_num, pkg_state, pkg_state_date, DataType.ToMySqlDateTimeString(last_update))
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pkg_guid, title, annotator_id, article_num, pkg_state, pkg_state_date, DataType.ToMySqlDateTimeString(last_update))
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pkg_guid, title, annotator_id, article_num, pkg_state, pkg_state_date, DataType.ToMySqlDateTimeString(last_update), pkg_guid)
    End Function
#End Region
End Class


End Namespace
