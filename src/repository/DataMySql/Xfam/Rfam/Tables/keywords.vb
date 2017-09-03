#Region "Microsoft.VisualBasic::839ec921c675ca8482e083e26b124260, ..\repository\DataMySql\Xfam\Rfam\Tables\keywords.vb"

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

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 11:55:32 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Xfam.Rfam.MySQL.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `keywords`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `keywords` (
'''   `rfam_acc` varchar(7) NOT NULL DEFAULT '',
'''   `rfam_id` varchar(40) DEFAULT NULL,
'''   `description` varchar(100) DEFAULT 'NULL',
'''   `rfam_general` longtext,
'''   `literature` longtext,
'''   `wiki` longtext,
'''   `pdb_mappings` longtext,
'''   `clan_info` longtext,
'''   PRIMARY KEY (`rfam_acc`),
'''   FULLTEXT KEY `rfam_kw_idx` (`description`,`rfam_general`,`literature`,`wiki`,`pdb_mappings`,`clan_info`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("keywords", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `keywords` (
  `rfam_acc` varchar(7) NOT NULL DEFAULT '',
  `rfam_id` varchar(40) DEFAULT NULL,
  `description` varchar(100) DEFAULT 'NULL',
  `rfam_general` longtext,
  `literature` longtext,
  `wiki` longtext,
  `pdb_mappings` longtext,
  `clan_info` longtext,
  PRIMARY KEY (`rfam_acc`),
  FULLTEXT KEY `rfam_kw_idx` (`description`,`rfam_general`,`literature`,`wiki`,`pdb_mappings`,`clan_info`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;")>
Public Class keywords: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property rfam_acc As String
    <DatabaseField("rfam_id"), DataType(MySqlDbType.VarChar, "40")> Public Property rfam_id As String
    <DatabaseField("description"), DataType(MySqlDbType.VarChar, "100")> Public Property description As String
    <DatabaseField("rfam_general"), DataType(MySqlDbType.Text)> Public Property rfam_general As String
    <DatabaseField("literature"), DataType(MySqlDbType.Text)> Public Property literature As String
    <DatabaseField("wiki"), DataType(MySqlDbType.Text)> Public Property wiki As String
    <DatabaseField("pdb_mappings"), DataType(MySqlDbType.Text)> Public Property pdb_mappings As String
    <DatabaseField("clan_info"), DataType(MySqlDbType.Text)> Public Property clan_info As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `keywords` (`rfam_acc`, `rfam_id`, `description`, `rfam_general`, `literature`, `wiki`, `pdb_mappings`, `clan_info`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `keywords` (`rfam_acc`, `rfam_id`, `description`, `rfam_general`, `literature`, `wiki`, `pdb_mappings`, `clan_info`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `keywords` WHERE `rfam_acc` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `keywords` SET `rfam_acc`='{0}', `rfam_id`='{1}', `description`='{2}', `rfam_general`='{3}', `literature`='{4}', `wiki`='{5}', `pdb_mappings`='{6}', `clan_info`='{7}' WHERE `rfam_acc` = '{8}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `keywords` WHERE `rfam_acc` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfam_acc)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `keywords` (`rfam_acc`, `rfam_id`, `description`, `rfam_general`, `literature`, `wiki`, `pdb_mappings`, `clan_info`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_acc, rfam_id, description, rfam_general, literature, wiki, pdb_mappings, clan_info)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{rfam_acc}', '{rfam_id}', '{description}', '{rfam_general}', '{literature}', '{wiki}', '{pdb_mappings}', '{clan_info}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `keywords` (`rfam_acc`, `rfam_id`, `description`, `rfam_general`, `literature`, `wiki`, `pdb_mappings`, `clan_info`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_acc, rfam_id, description, rfam_general, literature, wiki, pdb_mappings, clan_info)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `keywords` SET `rfam_acc`='{0}', `rfam_id`='{1}', `description`='{2}', `rfam_general`='{3}', `literature`='{4}', `wiki`='{5}', `pdb_mappings`='{6}', `clan_info`='{7}' WHERE `rfam_acc` = '{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfam_acc, rfam_id, description, rfam_general, literature, wiki, pdb_mappings, clan_info, rfam_acc)
    End Function
#End Region
End Class


End Namespace

