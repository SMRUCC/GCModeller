#Region "Microsoft.VisualBasic::2113e89ef18beb42179bf58f940ff07f, data\ExternalDBSource\ExplorEnz\MySQL\class.vb"

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

    ' Class [class]
    ' 
    '     Properties: [class], heading, id, last_change, note
    '                 subclass, subsubclass
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

REM  Dump @2018/5/23 13:13:37


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace ExplorEnz.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `class`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `class` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `class` int(11) NOT NULL DEFAULT '0',
'''   `subclass` int(11) DEFAULT NULL,
'''   `subsubclass` int(11) DEFAULT NULL,
'''   `heading` varchar(255) DEFAULT NULL,
'''   `note` text,
'''   `last_change` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
'''   PRIMARY KEY (`id`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=631 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("class", Database:="enzymed", SchemaSQL:="
CREATE TABLE `class` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `class` int(11) NOT NULL DEFAULT '0',
  `subclass` int(11) DEFAULT NULL,
  `subsubclass` int(11) DEFAULT NULL,
  `heading` varchar(255) DEFAULT NULL,
  `note` text,
  `last_change` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=631 DEFAULT CHARSET=latin1;")>
Public Class [class]: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("class"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="class")> Public Property [class] As Long
    <DatabaseField("subclass"), DataType(MySqlDbType.Int64, "11"), Column(Name:="subclass")> Public Property subclass As Long
    <DatabaseField("subsubclass"), DataType(MySqlDbType.Int64, "11"), Column(Name:="subsubclass")> Public Property subsubclass As Long
    <DatabaseField("heading"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="heading")> Public Property heading As String
    <DatabaseField("note"), DataType(MySqlDbType.Text), Column(Name:="note")> Public Property note As String
    <DatabaseField("last_change"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="last_change")> Public Property last_change As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `class` (`class`, `subclass`, `subsubclass`, `heading`, `note`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `class` (`id`, `class`, `subclass`, `subsubclass`, `heading`, `note`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `class` (`class`, `subclass`, `subsubclass`, `heading`, `note`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `class` (`id`, `class`, `subclass`, `subsubclass`, `heading`, `note`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `class` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `class` SET `id`='{0}', `class`='{1}', `subclass`='{2}', `subsubclass`='{3}', `heading`='{4}', `note`='{5}', `last_change`='{6}' WHERE `id` = '{7}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `class` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `class` (`id`, `class`, `subclass`, `subsubclass`, `heading`, `note`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, [class], subclass, subsubclass, heading, note, MySqlScript.ToMySqlDateTimeString(last_change))
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `class` (`id`, `class`, `subclass`, `subsubclass`, `heading`, `note`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, [class], subclass, subsubclass, heading, note, MySqlScript.ToMySqlDateTimeString(last_change))
        Else
        Return String.Format(INSERT_SQL, [class], subclass, subsubclass, heading, note, MySqlScript.ToMySqlDateTimeString(last_change))
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{[class]}', '{subclass}', '{subsubclass}', '{heading}', '{note}', '{last_change}')"
        Else
            Return $"('{[class]}', '{subclass}', '{subsubclass}', '{heading}', '{note}', '{last_change}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `class` (`id`, `class`, `subclass`, `subsubclass`, `heading`, `note`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, [class], subclass, subsubclass, heading, note, MySqlScript.ToMySqlDateTimeString(last_change))
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `class` (`id`, `class`, `subclass`, `subsubclass`, `heading`, `note`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, [class], subclass, subsubclass, heading, note, MySqlScript.ToMySqlDateTimeString(last_change))
        Else
        Return String.Format(REPLACE_SQL, [class], subclass, subsubclass, heading, note, MySqlScript.ToMySqlDateTimeString(last_change))
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `class` SET `id`='{0}', `class`='{1}', `subclass`='{2}', `subsubclass`='{3}', `heading`='{4}', `note`='{5}', `last_change`='{6}' WHERE `id` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, [class], subclass, subsubclass, heading, note, MySqlScript.ToMySqlDateTimeString(last_change), id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As [class]
                         Return DirectCast(MyClass.MemberwiseClone, [class])
                     End Function
End Class


End Namespace
