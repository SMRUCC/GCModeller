#Region "Microsoft.VisualBasic::ce8dc1029e6be4b31292a20235a5ddff, DataMySql\Xfam\Rfam\Tables\dead_family.vb"

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

    ' Class dead_family
    ' 
    '     Properties: comment, forward_to, rfam_acc, rfam_id, title
    '                 user
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
''' DROP TABLE IF EXISTS `dead_family`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `dead_family` (
'''   `rfam_acc` varchar(7) NOT NULL DEFAULT '' COMMENT 'record the author???',
'''   `rfam_id` varchar(40) NOT NULL,
'''   `comment` mediumtext,
'''   `forward_to` varchar(7) DEFAULT NULL,
'''   `title` varchar(150) DEFAULT NULL COMMENT 'wikipedia page title\n',
'''   `user` tinytext NOT NULL,
'''   UNIQUE KEY `rfam_acc` (`rfam_acc`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("dead_family", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `dead_family` (
  `rfam_acc` varchar(7) NOT NULL DEFAULT '' COMMENT 'record the author???',
  `rfam_id` varchar(40) NOT NULL,
  `comment` mediumtext,
  `forward_to` varchar(7) DEFAULT NULL,
  `title` varchar(150) DEFAULT NULL COMMENT 'wikipedia page title\n',
  `user` tinytext NOT NULL,
  UNIQUE KEY `rfam_acc` (`rfam_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class dead_family: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
''' <summary>
''' record the author???
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="rfam_acc"), XmlAttribute> Public Property rfam_acc As String
    <DatabaseField("rfam_id"), NotNull, DataType(MySqlDbType.VarChar, "40"), Column(Name:="rfam_id")> Public Property rfam_id As String
    <DatabaseField("comment"), DataType(MySqlDbType.Text), Column(Name:="comment")> Public Property comment As String
    <DatabaseField("forward_to"), DataType(MySqlDbType.VarChar, "7"), Column(Name:="forward_to")> Public Property forward_to As String
''' <summary>
''' wikipedia page title\n
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("title"), DataType(MySqlDbType.VarChar, "150"), Column(Name:="title")> Public Property title As String
    <DatabaseField("user"), NotNull, DataType(MySqlDbType.Text), Column(Name:="user")> Public Property user As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `dead_family` (`rfam_acc`, `rfam_id`, `comment`, `forward_to`, `title`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `dead_family` (`rfam_acc`, `rfam_id`, `comment`, `forward_to`, `title`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `dead_family` (`rfam_acc`, `rfam_id`, `comment`, `forward_to`, `title`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `dead_family` (`rfam_acc`, `rfam_id`, `comment`, `forward_to`, `title`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `dead_family` WHERE `rfam_acc` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `dead_family` SET `rfam_acc`='{0}', `rfam_id`='{1}', `comment`='{2}', `forward_to`='{3}', `title`='{4}', `user`='{5}' WHERE `rfam_acc` = '{6}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `dead_family` WHERE `rfam_acc` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfam_acc)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `dead_family` (`rfam_acc`, `rfam_id`, `comment`, `forward_to`, `title`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_acc, rfam_id, comment, forward_to, title, user)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `dead_family` (`rfam_acc`, `rfam_id`, `comment`, `forward_to`, `title`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, rfam_acc, rfam_id, comment, forward_to, title, user)
        Else
        Return String.Format(INSERT_SQL, rfam_acc, rfam_id, comment, forward_to, title, user)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{rfam_acc}', '{rfam_id}', '{comment}', '{forward_to}', '{title}', '{user}')"
        Else
            Return $"('{rfam_acc}', '{rfam_id}', '{comment}', '{forward_to}', '{title}', '{user}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `dead_family` (`rfam_acc`, `rfam_id`, `comment`, `forward_to`, `title`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_acc, rfam_id, comment, forward_to, title, user)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `dead_family` (`rfam_acc`, `rfam_id`, `comment`, `forward_to`, `title`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, rfam_acc, rfam_id, comment, forward_to, title, user)
        Else
        Return String.Format(REPLACE_SQL, rfam_acc, rfam_id, comment, forward_to, title, user)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `dead_family` SET `rfam_acc`='{0}', `rfam_id`='{1}', `comment`='{2}', `forward_to`='{3}', `title`='{4}', `user`='{5}' WHERE `rfam_acc` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfam_acc, rfam_id, comment, forward_to, title, user, rfam_acc)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As dead_family
                         Return DirectCast(MyClass.MemberwiseClone, dead_family)
                     End Function
End Class


End Namespace
