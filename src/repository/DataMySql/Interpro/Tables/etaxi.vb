#Region "Microsoft.VisualBasic::b21950a9b8ece399cbe04daeb71dce79, ..\repository\DataMySql\Interpro\Tables\etaxi.vb"

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

REM  Dump @3/29/2017 10:21:21 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Interpro.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `etaxi`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `etaxi` (
'''   `tax_id` bigint(15) NOT NULL,
'''   `parent_id` bigint(15) DEFAULT NULL,
'''   `scientific_name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `complete_genome_flag` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `rank` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `hidden` int(3) NOT NULL,
'''   `left_number` bigint(15) DEFAULT NULL,
'''   `right_number` bigint(15) DEFAULT NULL,
'''   `annotation_source` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `full_name` mediumtext CHARACTER SET latin1 COLLATE latin1_bin
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("etaxi", Database:="interpro", SchemaSQL:="
CREATE TABLE `etaxi` (
  `tax_id` bigint(15) NOT NULL,
  `parent_id` bigint(15) DEFAULT NULL,
  `scientific_name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `complete_genome_flag` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `rank` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `hidden` int(3) NOT NULL,
  `left_number` bigint(15) DEFAULT NULL,
  `right_number` bigint(15) DEFAULT NULL,
  `annotation_source` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `full_name` mediumtext CHARACTER SET latin1 COLLATE latin1_bin
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class etaxi: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("tax_id"), NotNull, DataType(MySqlDbType.Int64, "15")> Public Property tax_id As Long
    <DatabaseField("parent_id"), DataType(MySqlDbType.Int64, "15")> Public Property parent_id As Long
    <DatabaseField("scientific_name"), NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property scientific_name As String
    <DatabaseField("complete_genome_flag"), NotNull, DataType(MySqlDbType.VarChar, "1")> Public Property complete_genome_flag As String
    <DatabaseField("rank"), DataType(MySqlDbType.VarChar, "20")> Public Property rank As String
    <DatabaseField("hidden"), NotNull, DataType(MySqlDbType.Int64, "3")> Public Property hidden As Long
    <DatabaseField("left_number"), DataType(MySqlDbType.Int64, "15")> Public Property left_number As Long
    <DatabaseField("right_number"), DataType(MySqlDbType.Int64, "15")> Public Property right_number As Long
    <DatabaseField("annotation_source"), NotNull, DataType(MySqlDbType.VarChar, "30")> Public Property annotation_source As String
    <DatabaseField("full_name"), DataType(MySqlDbType.Text)> Public Property full_name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `etaxi` (`tax_id`, `parent_id`, `scientific_name`, `complete_genome_flag`, `rank`, `hidden`, `left_number`, `right_number`, `annotation_source`, `full_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `etaxi` (`tax_id`, `parent_id`, `scientific_name`, `complete_genome_flag`, `rank`, `hidden`, `left_number`, `right_number`, `annotation_source`, `full_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `etaxi` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `etaxi` SET `tax_id`='{0}', `parent_id`='{1}', `scientific_name`='{2}', `complete_genome_flag`='{3}', `rank`='{4}', `hidden`='{5}', `left_number`='{6}', `right_number`='{7}', `annotation_source`='{8}', `full_name`='{9}' WHERE ;</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `etaxi` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `etaxi` (`tax_id`, `parent_id`, `scientific_name`, `complete_genome_flag`, `rank`, `hidden`, `left_number`, `right_number`, `annotation_source`, `full_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, tax_id, parent_id, scientific_name, complete_genome_flag, rank, hidden, left_number, right_number, annotation_source, full_name)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{tax_id}', '{parent_id}', '{scientific_name}', '{complete_genome_flag}', '{rank}', '{hidden}', '{left_number}', '{right_number}', '{annotation_source}', '{full_name}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `etaxi` (`tax_id`, `parent_id`, `scientific_name`, `complete_genome_flag`, `rank`, `hidden`, `left_number`, `right_number`, `annotation_source`, `full_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, tax_id, parent_id, scientific_name, complete_genome_flag, rank, hidden, left_number, right_number, annotation_source, full_name)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `etaxi` SET `tax_id`='{0}', `parent_id`='{1}', `scientific_name`='{2}', `complete_genome_flag`='{3}', `rank`='{4}', `hidden`='{5}', `left_number`='{6}', `right_number`='{7}', `annotation_source`='{8}', `full_name`='{9}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace

