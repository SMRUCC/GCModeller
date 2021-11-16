#Region "Microsoft.VisualBasic::38d3e00de88202e57a1350bebbc393b8, DataMySql\Xfam\Rfam\Tables\keywords.vb"

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

    ' Class keywords
    ' 
    '     Properties: clan_info, description, literature, pdb_mappings, rfam_acc
    '                 rfam_general, rfam_id, wiki
    ' 
    '     Function: Clone, GetDeleteSQL, GetDumpInsertValue, (+2 Overloads) GetInsertSQL, (+2 Overloads) GetReplaceSQL
    '               GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2018/5/23 13:13:34


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

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
) ENGINE=MyISAM DEFAULT CHARSET=utf8;")>
Public Class keywords: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="rfam_acc"), XmlAttribute> Public Property rfam_acc As String
    <DatabaseField("rfam_id"), DataType(MySqlDbType.VarChar, "40"), Column(Name:="rfam_id")> Public Property rfam_id As String
    <DatabaseField("description"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="description")> Public Property description As String
    <DatabaseField("rfam_general"), DataType(MySqlDbType.Text), Column(Name:="rfam_general")> Public Property rfam_general As String
    <DatabaseField("literature"), DataType(MySqlDbType.Text), Column(Name:="literature")> Public Property literature As String
    <DatabaseField("wiki"), DataType(MySqlDbType.Text), Column(Name:="wiki")> Public Property wiki As String
    <DatabaseField("pdb_mappings"), DataType(MySqlDbType.Text), Column(Name:="pdb_mappings")> Public Property pdb_mappings As String
    <DatabaseField("clan_info"), DataType(MySqlDbType.Text), Column(Name:="clan_info")> Public Property clan_info As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `keywords` (`rfam_acc`, `rfam_id`, `description`, `rfam_general`, `literature`, `wiki`, `pdb_mappings`, `clan_info`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `keywords` (`rfam_acc`, `rfam_id`, `description`, `rfam_general`, `literature`, `wiki`, `pdb_mappings`, `clan_info`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `keywords` (`rfam_acc`, `rfam_id`, `description`, `rfam_general`, `literature`, `wiki`, `pdb_mappings`, `clan_info`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `keywords` (`rfam_acc`, `rfam_id`, `description`, `rfam_general`, `literature`, `wiki`, `pdb_mappings`, `clan_info`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `keywords` WHERE `rfam_acc` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `keywords` SET `rfam_acc`='{0}', `rfam_id`='{1}', `description`='{2}', `rfam_general`='{3}', `literature`='{4}', `wiki`='{5}', `pdb_mappings`='{6}', `clan_info`='{7}' WHERE `rfam_acc` = '{8}';</SQL>

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
''' ```SQL
''' INSERT INTO `keywords` (`rfam_acc`, `rfam_id`, `description`, `rfam_general`, `literature`, `wiki`, `pdb_mappings`, `clan_info`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, rfam_acc, rfam_id, description, rfam_general, literature, wiki, pdb_mappings, clan_info)
        Else
        Return String.Format(INSERT_SQL, rfam_acc, rfam_id, description, rfam_general, literature, wiki, pdb_mappings, clan_info)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{rfam_acc}', '{rfam_id}', '{description}', '{rfam_general}', '{literature}', '{wiki}', '{pdb_mappings}', '{clan_info}')"
        Else
            Return $"('{rfam_acc}', '{rfam_id}', '{description}', '{rfam_general}', '{literature}', '{wiki}', '{pdb_mappings}', '{clan_info}')"
        End If
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
''' REPLACE INTO `keywords` (`rfam_acc`, `rfam_id`, `description`, `rfam_general`, `literature`, `wiki`, `pdb_mappings`, `clan_info`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, rfam_acc, rfam_id, description, rfam_general, literature, wiki, pdb_mappings, clan_info)
        Else
        Return String.Format(REPLACE_SQL, rfam_acc, rfam_id, description, rfam_general, literature, wiki, pdb_mappings, clan_info)
        End If
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

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As keywords
                         Return DirectCast(MyClass.MemberwiseClone, keywords)
                     End Function
End Class


End Namespace
