#Region "Microsoft.VisualBasic::bba4c6210f57c27664fe0baee01bfe6e, DataMySql\Xfam\Rfam\Tables\dead_clan.vb"

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

    ' Class dead_clan
    ' 
    '     Properties: clan_acc, clan_id, comment, forward_to, user
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
''' DROP TABLE IF EXISTS `dead_clan`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `dead_clan` (
'''   `clan_acc` varchar(7) NOT NULL DEFAULT '',
'''   `clan_id` varchar(40) NOT NULL COMMENT 'Added. Add author?',
'''   `comment` mediumtext,
'''   `forward_to` varchar(7) DEFAULT NULL,
'''   `user` tinytext NOT NULL,
'''   UNIQUE KEY `rfam_acc` (`clan_acc`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("dead_clan", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `dead_clan` (
  `clan_acc` varchar(7) NOT NULL DEFAULT '',
  `clan_id` varchar(40) NOT NULL COMMENT 'Added. Add author?',
  `comment` mediumtext,
  `forward_to` varchar(7) DEFAULT NULL,
  `user` tinytext NOT NULL,
  UNIQUE KEY `rfam_acc` (`clan_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class dead_clan: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("clan_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="clan_acc"), XmlAttribute> Public Property clan_acc As String
''' <summary>
''' Added. Add author?
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("clan_id"), NotNull, DataType(MySqlDbType.VarChar, "40"), Column(Name:="clan_id")> Public Property clan_id As String
    <DatabaseField("comment"), DataType(MySqlDbType.Text), Column(Name:="comment")> Public Property comment As String
    <DatabaseField("forward_to"), DataType(MySqlDbType.VarChar, "7"), Column(Name:="forward_to")> Public Property forward_to As String
    <DatabaseField("user"), NotNull, DataType(MySqlDbType.Text), Column(Name:="user")> Public Property user As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `dead_clan` (`clan_acc`, `clan_id`, `comment`, `forward_to`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `dead_clan` (`clan_acc`, `clan_id`, `comment`, `forward_to`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `dead_clan` (`clan_acc`, `clan_id`, `comment`, `forward_to`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `dead_clan` (`clan_acc`, `clan_id`, `comment`, `forward_to`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `dead_clan` WHERE `clan_acc` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `dead_clan` SET `clan_acc`='{0}', `clan_id`='{1}', `comment`='{2}', `forward_to`='{3}', `user`='{4}' WHERE `clan_acc` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `dead_clan` WHERE `clan_acc` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, clan_acc)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `dead_clan` (`clan_acc`, `clan_id`, `comment`, `forward_to`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, clan_acc, clan_id, comment, forward_to, user)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `dead_clan` (`clan_acc`, `clan_id`, `comment`, `forward_to`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, clan_acc, clan_id, comment, forward_to, user)
        Else
        Return String.Format(INSERT_SQL, clan_acc, clan_id, comment, forward_to, user)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{clan_acc}', '{clan_id}', '{comment}', '{forward_to}', '{user}')"
        Else
            Return $"('{clan_acc}', '{clan_id}', '{comment}', '{forward_to}', '{user}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `dead_clan` (`clan_acc`, `clan_id`, `comment`, `forward_to`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, clan_acc, clan_id, comment, forward_to, user)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `dead_clan` (`clan_acc`, `clan_id`, `comment`, `forward_to`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, clan_acc, clan_id, comment, forward_to, user)
        Else
        Return String.Format(REPLACE_SQL, clan_acc, clan_id, comment, forward_to, user)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `dead_clan` SET `clan_acc`='{0}', `clan_id`='{1}', `comment`='{2}', `forward_to`='{3}', `user`='{4}' WHERE `clan_acc` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, clan_acc, clan_id, comment, forward_to, user, clan_acc)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As dead_clan
                         Return DirectCast(MyClass.MemberwiseClone, dead_clan)
                     End Function
End Class


End Namespace
