#Region "Microsoft.VisualBasic::2d3f99d5d3aaf3842d348e3611aabea3, ..\repository\DataMySql\Xfam\Rfam\Tables\taxonomy_websearch.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
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

REM  Dump @3/29/2017 11:55:32 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Xfam.Rfam.MySQL.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `taxonomy_websearch`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `taxonomy_websearch` (
'''   `ncbi_id` int(10) unsigned DEFAULT '0',
'''   `species` varchar(100) DEFAULT NULL,
'''   `taxonomy` mediumtext,
'''   `lft` int(10) DEFAULT NULL,
'''   `rgt` int(10) DEFAULT NULL,
'''   `parent` int(10) unsigned DEFAULT NULL,
'''   `level` varchar(200) DEFAULT NULL,
'''   `minimal` tinyint(1) NOT NULL DEFAULT '0',
'''   `rank` varchar(100) DEFAULT NULL,
'''   KEY `taxonomy_lft_idx` (`lft`),
'''   KEY `taxonomy_rgt_idx` (`rgt`),
'''   KEY `taxonomy_level_text_idx` (`level`),
'''   KEY `taxonomy_species_text_idx` (`species`),
'''   KEY `minimal_idx` (`minimal`),
'''   KEY `parent` (`parent`),
'''   KEY `ncbi_id_idx` (`ncbi_id`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("taxonomy_websearch", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `taxonomy_websearch` (
  `ncbi_id` int(10) unsigned DEFAULT '0',
  `species` varchar(100) DEFAULT NULL,
  `taxonomy` mediumtext,
  `lft` int(10) DEFAULT NULL,
  `rgt` int(10) DEFAULT NULL,
  `parent` int(10) unsigned DEFAULT NULL,
  `level` varchar(200) DEFAULT NULL,
  `minimal` tinyint(1) NOT NULL DEFAULT '0',
  `rank` varchar(100) DEFAULT NULL,
  KEY `taxonomy_lft_idx` (`lft`),
  KEY `taxonomy_rgt_idx` (`rgt`),
  KEY `taxonomy_level_text_idx` (`level`),
  KEY `taxonomy_species_text_idx` (`species`),
  KEY `minimal_idx` (`minimal`),
  KEY `parent` (`parent`),
  KEY `ncbi_id_idx` (`ncbi_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class taxonomy_websearch: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ncbi_id"), DataType(MySqlDbType.Int64, "10")> Public Property ncbi_id As Long
    <DatabaseField("species"), DataType(MySqlDbType.VarChar, "100")> Public Property species As String
    <DatabaseField("taxonomy"), DataType(MySqlDbType.Text)> Public Property taxonomy As String
    <DatabaseField("lft"), PrimaryKey, DataType(MySqlDbType.Int64, "10")> Public Property lft As Long
    <DatabaseField("rgt"), DataType(MySqlDbType.Int64, "10")> Public Property rgt As Long
    <DatabaseField("parent"), DataType(MySqlDbType.Int64, "10")> Public Property parent As Long
    <DatabaseField("level"), DataType(MySqlDbType.VarChar, "200")> Public Property level As String
    <DatabaseField("minimal"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property minimal As Long
    <DatabaseField("rank"), DataType(MySqlDbType.VarChar, "100")> Public Property rank As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `taxonomy_websearch` (`ncbi_id`, `species`, `taxonomy`, `lft`, `rgt`, `parent`, `level`, `minimal`, `rank`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `taxonomy_websearch` (`ncbi_id`, `species`, `taxonomy`, `lft`, `rgt`, `parent`, `level`, `minimal`, `rank`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `taxonomy_websearch` WHERE `lft` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `taxonomy_websearch` SET `ncbi_id`='{0}', `species`='{1}', `taxonomy`='{2}', `lft`='{3}', `rgt`='{4}', `parent`='{5}', `level`='{6}', `minimal`='{7}', `rank`='{8}' WHERE `lft` = '{9}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `taxonomy_websearch` WHERE `lft` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, lft)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `taxonomy_websearch` (`ncbi_id`, `species`, `taxonomy`, `lft`, `rgt`, `parent`, `level`, `minimal`, `rank`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ncbi_id, species, taxonomy, lft, rgt, parent, level, minimal, rank)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{ncbi_id}', '{species}', '{taxonomy}', '{lft}', '{rgt}', '{parent}', '{level}', '{minimal}', '{rank}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `taxonomy_websearch` (`ncbi_id`, `species`, `taxonomy`, `lft`, `rgt`, `parent`, `level`, `minimal`, `rank`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ncbi_id, species, taxonomy, lft, rgt, parent, level, minimal, rank)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `taxonomy_websearch` SET `ncbi_id`='{0}', `species`='{1}', `taxonomy`='{2}', `lft`='{3}', `rgt`='{4}', `parent`='{5}', `level`='{6}', `minimal`='{7}', `rank`='{8}' WHERE `lft` = '{9}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ncbi_id, species, taxonomy, lft, rgt, parent, level, minimal, rank, lft)
    End Function
#End Region
End Class


End Namespace

