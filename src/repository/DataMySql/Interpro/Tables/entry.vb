#Region "Microsoft.VisualBasic::37396cd36dfe4cc17540b25d3441d746, DataMySql\Interpro\Tables\entry.vb"

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

    ' Class entry
    ' 
    '     Properties: created, entry_ac, entry_type, name, short_name
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

Namespace Interpro.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `entry`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `entry` (
'''   `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `entry_type` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `created` datetime NOT NULL,
'''   `short_name` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   PRIMARY KEY (`entry_ac`),
'''   KEY `i_fk_entry$entry_type` (`entry_type`),
'''   KEY `fk_entry$entry_type` (`entry_type`),
'''   CONSTRAINT `fk_entry$entry_type` FOREIGN KEY (`entry_type`) REFERENCES `cv_entry_type` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("entry", Database:="interpro", SchemaSQL:="
CREATE TABLE `entry` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `entry_type` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `created` datetime NOT NULL,
  `short_name` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`entry_ac`),
  KEY `i_fk_entry$entry_type` (`entry_type`),
  KEY `fk_entry$entry_type` (`entry_type`),
  CONSTRAINT `fk_entry$entry_type` FOREIGN KEY (`entry_type`) REFERENCES `cv_entry_type` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class entry: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("entry_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "9"), Column(Name:="entry_ac"), XmlAttribute> Public Property entry_ac As String
    <DatabaseField("entry_type"), NotNull, DataType(MySqlDbType.VarChar, "1"), Column(Name:="entry_type")> Public Property entry_type As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="name")> Public Property name As String
    <DatabaseField("created"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="created")> Public Property created As Date
    <DatabaseField("short_name"), DataType(MySqlDbType.VarChar, "30"), Column(Name:="short_name")> Public Property short_name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `entry` (`entry_ac`, `entry_type`, `name`, `created`, `short_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `entry` (`entry_ac`, `entry_type`, `name`, `created`, `short_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `entry` (`entry_ac`, `entry_type`, `name`, `created`, `short_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `entry` (`entry_ac`, `entry_type`, `name`, `created`, `short_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `entry` WHERE `entry_ac` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `entry` SET `entry_ac`='{0}', `entry_type`='{1}', `name`='{2}', `created`='{3}', `short_name`='{4}' WHERE `entry_ac` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `entry` WHERE `entry_ac` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, entry_ac)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `entry` (`entry_ac`, `entry_type`, `name`, `created`, `short_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, entry_ac, entry_type, name, MySqlScript.ToMySqlDateTimeString(created), short_name)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `entry` (`entry_ac`, `entry_type`, `name`, `created`, `short_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, entry_ac, entry_type, name, MySqlScript.ToMySqlDateTimeString(created), short_name)
        Else
        Return String.Format(INSERT_SQL, entry_ac, entry_type, name, MySqlScript.ToMySqlDateTimeString(created), short_name)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{entry_ac}', '{entry_type}', '{name}', '{created}', '{short_name}')"
        Else
            Return $"('{entry_ac}', '{entry_type}', '{name}', '{created}', '{short_name}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `entry` (`entry_ac`, `entry_type`, `name`, `created`, `short_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, entry_ac, entry_type, name, MySqlScript.ToMySqlDateTimeString(created), short_name)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `entry` (`entry_ac`, `entry_type`, `name`, `created`, `short_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, entry_ac, entry_type, name, MySqlScript.ToMySqlDateTimeString(created), short_name)
        Else
        Return String.Format(REPLACE_SQL, entry_ac, entry_type, name, MySqlScript.ToMySqlDateTimeString(created), short_name)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `entry` SET `entry_ac`='{0}', `entry_type`='{1}', `name`='{2}', `created`='{3}', `short_name`='{4}' WHERE `entry_ac` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, entry_ac, entry_type, name, MySqlScript.ToMySqlDateTimeString(created), short_name, entry_ac)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As entry
                         Return DirectCast(MyClass.MemberwiseClone, entry)
                     End Function
End Class


End Namespace
