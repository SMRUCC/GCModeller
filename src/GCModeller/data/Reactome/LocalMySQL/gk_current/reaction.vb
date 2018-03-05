#Region "Microsoft.VisualBasic::f3a818663fb91bb22b6295370b2c17ea, data\Reactome\LocalMySQL\gk_current\reaction.vb"

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

    ' Class reaction
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

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
''' DROP TABLE IF EXISTS `reaction`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `reaction` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `reverseReaction` int(10) unsigned DEFAULT NULL,
'''   `reverseReaction_class` varchar(64) DEFAULT NULL,
'''   `totalProt` text,
'''   `maxHomologues` text,
'''   `inferredProt` text,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `reverseReaction` (`reverseReaction`),
'''   FULLTEXT KEY `totalProt` (`totalProt`),
'''   FULLTEXT KEY `maxHomologues` (`maxHomologues`),
'''   FULLTEXT KEY `inferredProt` (`inferredProt`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("reaction", Database:="gk_current", SchemaSQL:="
CREATE TABLE `reaction` (
  `DB_ID` int(10) unsigned NOT NULL,
  `reverseReaction` int(10) unsigned DEFAULT NULL,
  `reverseReaction_class` varchar(64) DEFAULT NULL,
  `totalProt` text,
  `maxHomologues` text,
  `inferredProt` text,
  PRIMARY KEY (`DB_ID`),
  KEY `reverseReaction` (`reverseReaction`),
  FULLTEXT KEY `totalProt` (`totalProt`),
  FULLTEXT KEY `maxHomologues` (`maxHomologues`),
  FULLTEXT KEY `inferredProt` (`inferredProt`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class reaction: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property DB_ID As Long
    <DatabaseField("reverseReaction"), DataType(MySqlDbType.Int64, "10")> Public Property reverseReaction As Long
    <DatabaseField("reverseReaction_class"), DataType(MySqlDbType.VarChar, "64")> Public Property reverseReaction_class As String
    <DatabaseField("totalProt"), DataType(MySqlDbType.Text)> Public Property totalProt As String
    <DatabaseField("maxHomologues"), DataType(MySqlDbType.Text)> Public Property maxHomologues As String
    <DatabaseField("inferredProt"), DataType(MySqlDbType.Text)> Public Property inferredProt As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `reaction` (`DB_ID`, `reverseReaction`, `reverseReaction_class`, `totalProt`, `maxHomologues`, `inferredProt`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `reaction` (`DB_ID`, `reverseReaction`, `reverseReaction_class`, `totalProt`, `maxHomologues`, `inferredProt`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `reaction` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `reaction` SET `DB_ID`='{0}', `reverseReaction`='{1}', `reverseReaction_class`='{2}', `totalProt`='{3}', `maxHomologues`='{4}', `inferredProt`='{5}' WHERE `DB_ID` = '{6}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `reaction` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `reaction` (`DB_ID`, `reverseReaction`, `reverseReaction_class`, `totalProt`, `maxHomologues`, `inferredProt`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, reverseReaction, reverseReaction_class, totalProt, maxHomologues, inferredProt)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{DB_ID}', '{reverseReaction}', '{reverseReaction_class}', '{totalProt}', '{maxHomologues}', '{inferredProt}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `reaction` (`DB_ID`, `reverseReaction`, `reverseReaction_class`, `totalProt`, `maxHomologues`, `inferredProt`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, reverseReaction, reverseReaction_class, totalProt, maxHomologues, inferredProt)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `reaction` SET `DB_ID`='{0}', `reverseReaction`='{1}', `reverseReaction_class`='{2}', `totalProt`='{3}', `maxHomologues`='{4}', `inferredProt`='{5}' WHERE `DB_ID` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, reverseReaction, reverseReaction_class, totalProt, maxHomologues, inferredProt, DB_ID)
    End Function
#End Region
End Class


End Namespace
