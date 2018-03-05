#Region "Microsoft.VisualBasic::10e5168a04fa7d6d13bbd4875021459c, data\Reactome\LocalMySQL\gk_current\complex.vb"

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

    ' Class complex
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 9:40:27 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `complex`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `complex` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `isChimeric` enum('TRUE','FALSE') DEFAULT NULL,
'''   `totalProt` text,
'''   `maxHomologues` text,
'''   `inferredProt` text,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `isChimeric` (`isChimeric`),
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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("complex", Database:="gk_current", SchemaSQL:="
CREATE TABLE `complex` (
  `DB_ID` int(10) unsigned NOT NULL,
  `isChimeric` enum('TRUE','FALSE') DEFAULT NULL,
  `totalProt` text,
  `maxHomologues` text,
  `inferredProt` text,
  PRIMARY KEY (`DB_ID`),
  KEY `isChimeric` (`isChimeric`),
  FULLTEXT KEY `totalProt` (`totalProt`),
  FULLTEXT KEY `maxHomologues` (`maxHomologues`),
  FULLTEXT KEY `inferredProt` (`inferredProt`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class complex: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property DB_ID As Long
    <DatabaseField("isChimeric"), DataType(MySqlDbType.String)> Public Property isChimeric As String
    <DatabaseField("totalProt"), DataType(MySqlDbType.Text)> Public Property totalProt As String
    <DatabaseField("maxHomologues"), DataType(MySqlDbType.Text)> Public Property maxHomologues As String
    <DatabaseField("inferredProt"), DataType(MySqlDbType.Text)> Public Property inferredProt As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `complex` (`DB_ID`, `isChimeric`, `totalProt`, `maxHomologues`, `inferredProt`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `complex` (`DB_ID`, `isChimeric`, `totalProt`, `maxHomologues`, `inferredProt`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `complex` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `complex` SET `DB_ID`='{0}', `isChimeric`='{1}', `totalProt`='{2}', `maxHomologues`='{3}', `inferredProt`='{4}' WHERE `DB_ID` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `complex` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `complex` (`DB_ID`, `isChimeric`, `totalProt`, `maxHomologues`, `inferredProt`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, isChimeric, totalProt, maxHomologues, inferredProt)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{DB_ID}', '{isChimeric}', '{totalProt}', '{maxHomologues}', '{inferredProt}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `complex` (`DB_ID`, `isChimeric`, `totalProt`, `maxHomologues`, `inferredProt`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, isChimeric, totalProt, maxHomologues, inferredProt)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `complex` SET `DB_ID`='{0}', `isChimeric`='{1}', `totalProt`='{2}', `maxHomologues`='{3}', `inferredProt`='{4}' WHERE `DB_ID` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, isChimeric, totalProt, maxHomologues, inferredProt, DB_ID)
    End Function
#End Region
End Class


End Namespace
