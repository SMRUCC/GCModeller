#Region "Microsoft.VisualBasic::08e11d9db7b6ba5a5cf80ce554008106, data\Reactome\LocalMySQL\gk_current\databaseobject.vb"

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

    '   Total Lines: 193
    '    Code Lines: 99
    ' Comment Lines: 72
    '   Blank Lines: 22
    '     File Size: 11.03 KB


    ' Class databaseobject
    ' 
    '     Properties: __is_ghost, _class, _displayName, _Protege_id, _timestamp
    '                 created, created_class, DB_ID, stableIdentifier, stableIdentifier_class
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

REM  Dump @2018/5/23 13:13:41


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `databaseobject`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `databaseobject` (
'''   `DB_ID` int(10) NOT NULL AUTO_INCREMENT,
'''   `_Protege_id` varchar(255) DEFAULT NULL,
'''   `__is_ghost` enum('TRUE') DEFAULT NULL,
'''   `_class` varchar(64) DEFAULT NULL,
'''   `_displayName` text,
'''   `_timestamp` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
'''   `created` int(10) unsigned DEFAULT NULL,
'''   `created_class` varchar(64) DEFAULT NULL,
'''   `stableIdentifier` int(10) unsigned DEFAULT NULL,
'''   `stableIdentifier_class` varchar(64) DEFAULT NULL,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `_Protege_id` (`_Protege_id`),
'''   KEY `__is_ghost` (`__is_ghost`),
'''   KEY `_class` (`_class`),
'''   KEY `_timestamp` (`_timestamp`),
'''   KEY `created` (`created`),
'''   KEY `stableIdentifier` (`stableIdentifier`),
'''   FULLTEXT KEY `_Protege_id_fulltext` (`_Protege_id`),
'''   FULLTEXT KEY `_class_fulltext` (`_class`),
'''   FULLTEXT KEY `_displayName` (`_displayName`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=8835475 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("databaseobject", Database:="gk_current", SchemaSQL:="
CREATE TABLE `databaseobject` (
  `DB_ID` int(10) NOT NULL AUTO_INCREMENT,
  `_Protege_id` varchar(255) DEFAULT NULL,
  `__is_ghost` enum('TRUE') DEFAULT NULL,
  `_class` varchar(64) DEFAULT NULL,
  `_displayName` text,
  `_timestamp` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `created` int(10) unsigned DEFAULT NULL,
  `created_class` varchar(64) DEFAULT NULL,
  `stableIdentifier` int(10) unsigned DEFAULT NULL,
  `stableIdentifier_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `_Protege_id` (`_Protege_id`),
  KEY `__is_ghost` (`__is_ghost`),
  KEY `_class` (`_class`),
  KEY `_timestamp` (`_timestamp`),
  KEY `created` (`created`),
  KEY `stableIdentifier` (`stableIdentifier`),
  FULLTEXT KEY `_Protege_id_fulltext` (`_Protege_id`),
  FULLTEXT KEY `_class_fulltext` (`_class`),
  FULLTEXT KEY `_displayName` (`_displayName`)
) ENGINE=MyISAM AUTO_INCREMENT=8835475 DEFAULT CHARSET=latin1;")>
Public Class databaseobject: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("_Protege_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="_Protege_id")> Public Property _Protege_id As String
    <DatabaseField("__is_ghost"), DataType(MySqlDbType.String), Column(Name:="__is_ghost")> Public Property __is_ghost As String
    <DatabaseField("_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="_class")> Public Property _class As String
    <DatabaseField("_displayName"), DataType(MySqlDbType.Text), Column(Name:="_displayName")> Public Property _displayName As String
    <DatabaseField("_timestamp"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="_timestamp")> Public Property _timestamp As Date
    <DatabaseField("created"), DataType(MySqlDbType.Int64, "10"), Column(Name:="created")> Public Property created As Long
    <DatabaseField("created_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="created_class")> Public Property created_class As String
    <DatabaseField("stableIdentifier"), DataType(MySqlDbType.Int64, "10"), Column(Name:="stableIdentifier")> Public Property stableIdentifier As Long
    <DatabaseField("stableIdentifier_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="stableIdentifier_class")> Public Property stableIdentifier_class As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `databaseobject` (`_Protege_id`, `__is_ghost`, `_class`, `_displayName`, `_timestamp`, `created`, `created_class`, `stableIdentifier`, `stableIdentifier_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `databaseobject` (`DB_ID`, `_Protege_id`, `__is_ghost`, `_class`, `_displayName`, `_timestamp`, `created`, `created_class`, `stableIdentifier`, `stableIdentifier_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `databaseobject` (`_Protege_id`, `__is_ghost`, `_class`, `_displayName`, `_timestamp`, `created`, `created_class`, `stableIdentifier`, `stableIdentifier_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `databaseobject` (`DB_ID`, `_Protege_id`, `__is_ghost`, `_class`, `_displayName`, `_timestamp`, `created`, `created_class`, `stableIdentifier`, `stableIdentifier_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `databaseobject` WHERE `DB_ID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `databaseobject` SET `DB_ID`='{0}', `_Protege_id`='{1}', `__is_ghost`='{2}', `_class`='{3}', `_displayName`='{4}', `_timestamp`='{5}', `created`='{6}', `created_class`='{7}', `stableIdentifier`='{8}', `stableIdentifier_class`='{9}' WHERE `DB_ID` = '{10}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `databaseobject` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `databaseobject` (`DB_ID`, `_Protege_id`, `__is_ghost`, `_class`, `_displayName`, `_timestamp`, `created`, `created_class`, `stableIdentifier`, `stableIdentifier_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, _Protege_id, __is_ghost, _class, _displayName, MySqlScript.ToMySqlDateTimeString(_timestamp), created, created_class, stableIdentifier, stableIdentifier_class)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `databaseobject` (`DB_ID`, `_Protege_id`, `__is_ghost`, `_class`, `_displayName`, `_timestamp`, `created`, `created_class`, `stableIdentifier`, `stableIdentifier_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, DB_ID, _Protege_id, __is_ghost, _class, _displayName, MySqlScript.ToMySqlDateTimeString(_timestamp), created, created_class, stableIdentifier, stableIdentifier_class)
        Else
        Return String.Format(INSERT_SQL, _Protege_id, __is_ghost, _class, _displayName, MySqlScript.ToMySqlDateTimeString(_timestamp), created, created_class, stableIdentifier, stableIdentifier_class)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{DB_ID}', '{_Protege_id}', '{__is_ghost}', '{_class}', '{_displayName}', '{_timestamp}', '{created}', '{created_class}', '{stableIdentifier}', '{stableIdentifier_class}')"
        Else
            Return $"('{_Protege_id}', '{__is_ghost}', '{_class}', '{_displayName}', '{_timestamp}', '{created}', '{created_class}', '{stableIdentifier}', '{stableIdentifier_class}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `databaseobject` (`DB_ID`, `_Protege_id`, `__is_ghost`, `_class`, `_displayName`, `_timestamp`, `created`, `created_class`, `stableIdentifier`, `stableIdentifier_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, _Protege_id, __is_ghost, _class, _displayName, MySqlScript.ToMySqlDateTimeString(_timestamp), created, created_class, stableIdentifier, stableIdentifier_class)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `databaseobject` (`DB_ID`, `_Protege_id`, `__is_ghost`, `_class`, `_displayName`, `_timestamp`, `created`, `created_class`, `stableIdentifier`, `stableIdentifier_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, DB_ID, _Protege_id, __is_ghost, _class, _displayName, MySqlScript.ToMySqlDateTimeString(_timestamp), created, created_class, stableIdentifier, stableIdentifier_class)
        Else
        Return String.Format(REPLACE_SQL, _Protege_id, __is_ghost, _class, _displayName, MySqlScript.ToMySqlDateTimeString(_timestamp), created, created_class, stableIdentifier, stableIdentifier_class)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `databaseobject` SET `DB_ID`='{0}', `_Protege_id`='{1}', `__is_ghost`='{2}', `_class`='{3}', `_displayName`='{4}', `_timestamp`='{5}', `created`='{6}', `created_class`='{7}', `stableIdentifier`='{8}', `stableIdentifier_class`='{9}' WHERE `DB_ID` = '{10}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, _Protege_id, __is_ghost, _class, _displayName, MySqlScript.ToMySqlDateTimeString(_timestamp), created, created_class, stableIdentifier, stableIdentifier_class, DB_ID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As databaseobject
                         Return DirectCast(MyClass.MemberwiseClone, databaseobject)
                     End Function
End Class


End Namespace
