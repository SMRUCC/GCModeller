#Region "Microsoft.VisualBasic::333754ad6604bc19ab1912552966cf86, GCModeller\data\RegulonDatabase\Regtransbase\MySQL\packages.vb"

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


    ' Code Statistics:

    '   Total Lines: 170
    '    Code Lines: 86
    ' Comment Lines: 62
    '   Blank Lines: 22
    '     File Size: 8.52 KB


    ' Class packages
    ' 
    '     Properties: annotator_id, article_num, last_update, pkg_guid, pkg_state
    '                 pkg_state_date, title
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

REM  Dump @2018/5/23 13:13:38


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace Regtransbase.MySQL

''' <summary>
''' ```SQL
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
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("packages", Database:="dbregulation_update", SchemaSQL:="
CREATE TABLE `packages` (
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `title` char(50) DEFAULT NULL,
  `annotator_id` int(11) DEFAULT NULL,
  `article_num` int(11) DEFAULT NULL,
  `pkg_state` int(11) NOT NULL DEFAULT '0',
  `pkg_state_date` char(10) DEFAULT NULL,
  `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`pkg_guid`),
  KEY `annotator_id` (`annotator_id`),
  CONSTRAINT `packages_ibfk_1` FOREIGN KEY (`annotator_id`) REFERENCES `db_users` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class packages: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pkg_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="pkg_guid"), XmlAttribute> Public Property pkg_guid As Long
    <DatabaseField("title"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="title")> Public Property title As String
    <DatabaseField("annotator_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="annotator_id")> Public Property annotator_id As Long
    <DatabaseField("article_num"), DataType(MySqlDbType.Int64, "11"), Column(Name:="article_num")> Public Property article_num As Long
    <DatabaseField("pkg_state"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="pkg_state")> Public Property pkg_state As Long
    <DatabaseField("pkg_state_date"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="pkg_state_date")> Public Property pkg_state_date As String
    <DatabaseField("last_update"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="last_update")> Public Property last_update As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `packages` (`pkg_guid`, `title`, `annotator_id`, `article_num`, `pkg_state`, `pkg_state_date`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `packages` (`pkg_guid`, `title`, `annotator_id`, `article_num`, `pkg_state`, `pkg_state_date`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `packages` (`pkg_guid`, `title`, `annotator_id`, `article_num`, `pkg_state`, `pkg_state_date`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `packages` (`pkg_guid`, `title`, `annotator_id`, `article_num`, `pkg_state`, `pkg_state_date`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `packages` WHERE `pkg_guid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `packages` SET `pkg_guid`='{0}', `title`='{1}', `annotator_id`='{2}', `article_num`='{3}', `pkg_state`='{4}', `pkg_state_date`='{5}', `last_update`='{6}' WHERE `pkg_guid` = '{7}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `packages` WHERE `pkg_guid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pkg_guid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `packages` (`pkg_guid`, `title`, `annotator_id`, `article_num`, `pkg_state`, `pkg_state_date`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pkg_guid, title, annotator_id, article_num, pkg_state, pkg_state_date, MySqlScript.ToMySqlDateTimeString(last_update))
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `packages` (`pkg_guid`, `title`, `annotator_id`, `article_num`, `pkg_state`, `pkg_state_date`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, pkg_guid, title, annotator_id, article_num, pkg_state, pkg_state_date, MySqlScript.ToMySqlDateTimeString(last_update))
        Else
        Return String.Format(INSERT_SQL, pkg_guid, title, annotator_id, article_num, pkg_state, pkg_state_date, MySqlScript.ToMySqlDateTimeString(last_update))
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{pkg_guid}', '{title}', '{annotator_id}', '{article_num}', '{pkg_state}', '{pkg_state_date}', '{last_update}')"
        Else
            Return $"('{pkg_guid}', '{title}', '{annotator_id}', '{article_num}', '{pkg_state}', '{pkg_state_date}', '{last_update}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `packages` (`pkg_guid`, `title`, `annotator_id`, `article_num`, `pkg_state`, `pkg_state_date`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pkg_guid, title, annotator_id, article_num, pkg_state, pkg_state_date, MySqlScript.ToMySqlDateTimeString(last_update))
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `packages` (`pkg_guid`, `title`, `annotator_id`, `article_num`, `pkg_state`, `pkg_state_date`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, pkg_guid, title, annotator_id, article_num, pkg_state, pkg_state_date, MySqlScript.ToMySqlDateTimeString(last_update))
        Else
        Return String.Format(REPLACE_SQL, pkg_guid, title, annotator_id, article_num, pkg_state, pkg_state_date, MySqlScript.ToMySqlDateTimeString(last_update))
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `packages` SET `pkg_guid`='{0}', `title`='{1}', `annotator_id`='{2}', `article_num`='{3}', `pkg_state`='{4}', `pkg_state_date`='{5}', `last_update`='{6}' WHERE `pkg_guid` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pkg_guid, title, annotator_id, article_num, pkg_state, pkg_state_date, MySqlScript.ToMySqlDateTimeString(last_update), pkg_guid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As packages
                         Return DirectCast(MyClass.MemberwiseClone, packages)
                     End Function
End Class


End Namespace
