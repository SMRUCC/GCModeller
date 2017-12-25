#Region "Microsoft.VisualBasic::ccd5cc2f03dc17d5d4208f16b31d956d, ..\GCModeller\data\Reactome\LocalMySQL\gk_current\reactionlikeevent.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

REM  Dump @3/29/2017 9:40:28 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `reactionlikeevent`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `reactionlikeevent` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `isChimeric` enum('TRUE','FALSE') DEFAULT NULL,
'''   `systematicName` text,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `isChimeric` (`isChimeric`),
'''   FULLTEXT KEY `systematicName` (`systematicName`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("reactionlikeevent", Database:="gk_current", SchemaSQL:="
CREATE TABLE `reactionlikeevent` (
  `DB_ID` int(10) unsigned NOT NULL,
  `isChimeric` enum('TRUE','FALSE') DEFAULT NULL,
  `systematicName` text,
  PRIMARY KEY (`DB_ID`),
  KEY `isChimeric` (`isChimeric`),
  FULLTEXT KEY `systematicName` (`systematicName`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class reactionlikeevent: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property DB_ID As Long
    <DatabaseField("isChimeric"), DataType(MySqlDbType.String)> Public Property isChimeric As String
    <DatabaseField("systematicName"), DataType(MySqlDbType.Text)> Public Property systematicName As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `reactionlikeevent` (`DB_ID`, `isChimeric`, `systematicName`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `reactionlikeevent` (`DB_ID`, `isChimeric`, `systematicName`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `reactionlikeevent` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `reactionlikeevent` SET `DB_ID`='{0}', `isChimeric`='{1}', `systematicName`='{2}' WHERE `DB_ID` = '{3}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `reactionlikeevent` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `reactionlikeevent` (`DB_ID`, `isChimeric`, `systematicName`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, isChimeric, systematicName)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{DB_ID}', '{isChimeric}', '{systematicName}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `reactionlikeevent` (`DB_ID`, `isChimeric`, `systematicName`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, isChimeric, systematicName)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `reactionlikeevent` SET `DB_ID`='{0}', `isChimeric`='{1}', `systematicName`='{2}' WHERE `DB_ID` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, isChimeric, systematicName, DB_ID)
    End Function
#End Region
End Class


End Namespace
