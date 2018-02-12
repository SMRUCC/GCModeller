#Region "Microsoft.VisualBasic::4e55ef0cc42bbc2ff08726f7bcd33603, data\Reactome\LocalMySQL\gk_current\simpleentity.vb"

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

    ' Class simpleentity
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
''' DROP TABLE IF EXISTS `simpleentity`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `simpleentity` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `referenceEntity` int(10) unsigned DEFAULT NULL,
'''   `referenceEntity_class` varchar(64) DEFAULT NULL,
'''   `species` int(10) unsigned DEFAULT NULL,
'''   `species_class` varchar(64) DEFAULT NULL,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `referenceEntity` (`referenceEntity`),
'''   KEY `species` (`species`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("simpleentity", Database:="gk_current", SchemaSQL:="
CREATE TABLE `simpleentity` (
  `DB_ID` int(10) unsigned NOT NULL,
  `referenceEntity` int(10) unsigned DEFAULT NULL,
  `referenceEntity_class` varchar(64) DEFAULT NULL,
  `species` int(10) unsigned DEFAULT NULL,
  `species_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `referenceEntity` (`referenceEntity`),
  KEY `species` (`species`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class simpleentity: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property DB_ID As Long
    <DatabaseField("referenceEntity"), DataType(MySqlDbType.Int64, "10")> Public Property referenceEntity As Long
    <DatabaseField("referenceEntity_class"), DataType(MySqlDbType.VarChar, "64")> Public Property referenceEntity_class As String
    <DatabaseField("species"), DataType(MySqlDbType.Int64, "10")> Public Property species As Long
    <DatabaseField("species_class"), DataType(MySqlDbType.VarChar, "64")> Public Property species_class As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `simpleentity` (`DB_ID`, `referenceEntity`, `referenceEntity_class`, `species`, `species_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `simpleentity` (`DB_ID`, `referenceEntity`, `referenceEntity_class`, `species`, `species_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `simpleentity` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `simpleentity` SET `DB_ID`='{0}', `referenceEntity`='{1}', `referenceEntity_class`='{2}', `species`='{3}', `species_class`='{4}' WHERE `DB_ID` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `simpleentity` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `simpleentity` (`DB_ID`, `referenceEntity`, `referenceEntity_class`, `species`, `species_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, referenceEntity, referenceEntity_class, species, species_class)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{DB_ID}', '{referenceEntity}', '{referenceEntity_class}', '{species}', '{species_class}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `simpleentity` (`DB_ID`, `referenceEntity`, `referenceEntity_class`, `species`, `species_class`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, referenceEntity, referenceEntity_class, species, species_class)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `simpleentity` SET `DB_ID`='{0}', `referenceEntity`='{1}', `referenceEntity_class`='{2}', `species`='{3}', `species_class`='{4}' WHERE `DB_ID` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, referenceEntity, referenceEntity_class, species, species_class, DB_ID)
    End Function
#End Region
End Class


End Namespace
