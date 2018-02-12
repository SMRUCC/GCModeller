#Region "Microsoft.VisualBasic::15591b5f3a473177199034e55e4a2070, data\Reactome\LocalMySQL\gk_current\pathway.vb"

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

    ' Class pathway
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
''' DROP TABLE IF EXISTS `pathway`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `pathway` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `doi` varchar(64) DEFAULT NULL,
'''   `isCanonical` enum('TRUE','FALSE') DEFAULT NULL,
'''   `normalPathway` int(10) unsigned DEFAULT NULL,
'''   `normalPathway_class` varchar(64) DEFAULT NULL,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `doi` (`doi`),
'''   KEY `isCanonical` (`isCanonical`),
'''   KEY `normalPathway` (`normalPathway`),
'''   FULLTEXT KEY `doi_fulltext` (`doi`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pathway", Database:="gk_current", SchemaSQL:="
CREATE TABLE `pathway` (
  `DB_ID` int(10) unsigned NOT NULL,
  `doi` varchar(64) DEFAULT NULL,
  `isCanonical` enum('TRUE','FALSE') DEFAULT NULL,
  `normalPathway` int(10) unsigned DEFAULT NULL,
  `normalPathway_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `doi` (`doi`),
  KEY `isCanonical` (`isCanonical`),
  KEY `normalPathway` (`normalPathway`),
  FULLTEXT KEY `doi_fulltext` (`doi`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class pathway: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property DB_ID As Long
    <DatabaseField("doi"), DataType(MySqlDbType.VarChar, "64")> Public Property doi As String
    <DatabaseField("isCanonical"), DataType(MySqlDbType.String)> Public Property isCanonical As String
    <DatabaseField("normalPathway"), DataType(MySqlDbType.Int64, "10")> Public Property normalPathway As Long
    <DatabaseField("normalPathway_class"), DataType(MySqlDbType.VarChar, "64")> Public Property normalPathway_class As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pathway` (`DB_ID`, `doi`, `isCanonical`, `normalPathway`, `normalPathway_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pathway` (`DB_ID`, `doi`, `isCanonical`, `normalPathway`, `normalPathway_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pathway` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pathway` SET `DB_ID`='{0}', `doi`='{1}', `isCanonical`='{2}', `normalPathway`='{3}', `normalPathway_class`='{4}' WHERE `DB_ID` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `pathway` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `pathway` (`DB_ID`, `doi`, `isCanonical`, `normalPathway`, `normalPathway_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, doi, isCanonical, normalPathway, normalPathway_class)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{DB_ID}', '{doi}', '{isCanonical}', '{normalPathway}', '{normalPathway_class}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `pathway` (`DB_ID`, `doi`, `isCanonical`, `normalPathway`, `normalPathway_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, doi, isCanonical, normalPathway, normalPathway_class)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `pathway` SET `DB_ID`='{0}', `doi`='{1}', `isCanonical`='{2}', `normalPathway`='{3}', `normalPathway_class`='{4}' WHERE `DB_ID` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, doi, isCanonical, normalPathway, normalPathway_class, DB_ID)
    End Function
#End Region
End Class


End Namespace
