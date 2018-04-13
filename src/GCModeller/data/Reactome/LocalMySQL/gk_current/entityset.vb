#Region "Microsoft.VisualBasic::8c9f57a46b5c95f08e033d60ccf32fb1, data\Reactome\LocalMySQL\gk_current\entityset.vb"

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

    ' Class entityset
    ' 
    '     Properties: DB_ID, inferredProt, isOrdered, maxHomologues, totalProt
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @3/16/2018 10:40:21 PM


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
''' DROP TABLE IF EXISTS `entityset`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `entityset` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `isOrdered` enum('TRUE','FALSE') DEFAULT NULL,
'''   `totalProt` text,
'''   `inferredProt` text,
'''   `maxHomologues` text,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `isOrdered` (`isOrdered`),
'''   FULLTEXT KEY `totalProt` (`totalProt`),
'''   FULLTEXT KEY `inferredProt` (`inferredProt`),
'''   FULLTEXT KEY `maxHomologues` (`maxHomologues`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("entityset", Database:="gk_current", SchemaSQL:="
CREATE TABLE `entityset` (
  `DB_ID` int(10) unsigned NOT NULL,
  `isOrdered` enum('TRUE','FALSE') DEFAULT NULL,
  `totalProt` text,
  `inferredProt` text,
  `maxHomologues` text,
  PRIMARY KEY (`DB_ID`),
  KEY `isOrdered` (`isOrdered`),
  FULLTEXT KEY `totalProt` (`totalProt`),
  FULLTEXT KEY `inferredProt` (`inferredProt`),
  FULLTEXT KEY `maxHomologues` (`maxHomologues`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class entityset: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("isOrdered"), DataType(MySqlDbType.String), Column(Name:="isOrdered")> Public Property isOrdered As String
    <DatabaseField("totalProt"), DataType(MySqlDbType.Text), Column(Name:="totalProt")> Public Property totalProt As String
    <DatabaseField("inferredProt"), DataType(MySqlDbType.Text), Column(Name:="inferredProt")> Public Property inferredProt As String
    <DatabaseField("maxHomologues"), DataType(MySqlDbType.Text), Column(Name:="maxHomologues")> Public Property maxHomologues As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `entityset` (`DB_ID`, `isOrdered`, `totalProt`, `inferredProt`, `maxHomologues`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `entityset` (`DB_ID`, `isOrdered`, `totalProt`, `inferredProt`, `maxHomologues`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `entityset` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `entityset` SET `DB_ID`='{0}', `isOrdered`='{1}', `totalProt`='{2}', `inferredProt`='{3}', `maxHomologues`='{4}' WHERE `DB_ID` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `entityset` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `entityset` (`DB_ID`, `isOrdered`, `totalProt`, `inferredProt`, `maxHomologues`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, isOrdered, totalProt, inferredProt, maxHomologues)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{DB_ID}', '{isOrdered}', '{totalProt}', '{inferredProt}', '{maxHomologues}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `entityset` (`DB_ID`, `isOrdered`, `totalProt`, `inferredProt`, `maxHomologues`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, isOrdered, totalProt, inferredProt, maxHomologues)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `entityset` SET `DB_ID`='{0}', `isOrdered`='{1}', `totalProt`='{2}', `inferredProt`='{3}', `maxHomologues`='{4}' WHERE `DB_ID` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, isOrdered, totalProt, inferredProt, maxHomologues, DB_ID)
    End Function
#End Region
Public Function Clone() As entityset
                  Return DirectCast(MyClass.MemberwiseClone, entityset)
              End Function
End Class


End Namespace
