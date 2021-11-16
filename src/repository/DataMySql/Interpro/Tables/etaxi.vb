#Region "Microsoft.VisualBasic::646400f8dc1c64159d781a1857855d13, DataMySql\Interpro\Tables\etaxi.vb"

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

    ' Class etaxi
    ' 
    '     Properties: annotation_source, complete_genome_flag, full_name, hidden, left_number
    '                 parent_id, rank, right_number, scientific_name, tax_id
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
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class etaxi: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("tax_id"), NotNull, DataType(MySqlDbType.Int64, "15"), Column(Name:="tax_id")> Public Property tax_id As Long
    <DatabaseField("parent_id"), DataType(MySqlDbType.Int64, "15"), Column(Name:="parent_id")> Public Property parent_id As Long
    <DatabaseField("scientific_name"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="scientific_name")> Public Property scientific_name As String
    <DatabaseField("complete_genome_flag"), NotNull, DataType(MySqlDbType.VarChar, "1"), Column(Name:="complete_genome_flag")> Public Property complete_genome_flag As String
    <DatabaseField("rank"), DataType(MySqlDbType.VarChar, "20"), Column(Name:="rank")> Public Property rank As String
    <DatabaseField("hidden"), NotNull, DataType(MySqlDbType.Int64, "3"), Column(Name:="hidden")> Public Property hidden As Long
    <DatabaseField("left_number"), DataType(MySqlDbType.Int64, "15"), Column(Name:="left_number")> Public Property left_number As Long
    <DatabaseField("right_number"), DataType(MySqlDbType.Int64, "15"), Column(Name:="right_number")> Public Property right_number As Long
    <DatabaseField("annotation_source"), NotNull, DataType(MySqlDbType.VarChar, "30"), Column(Name:="annotation_source")> Public Property annotation_source As String
    <DatabaseField("full_name"), DataType(MySqlDbType.Text), Column(Name:="full_name")> Public Property full_name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `etaxi` (`tax_id`, `parent_id`, `scientific_name`, `complete_genome_flag`, `rank`, `hidden`, `left_number`, `right_number`, `annotation_source`, `full_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `etaxi` (`tax_id`, `parent_id`, `scientific_name`, `complete_genome_flag`, `rank`, `hidden`, `left_number`, `right_number`, `annotation_source`, `full_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `etaxi` (`tax_id`, `parent_id`, `scientific_name`, `complete_genome_flag`, `rank`, `hidden`, `left_number`, `right_number`, `annotation_source`, `full_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `etaxi` (`tax_id`, `parent_id`, `scientific_name`, `complete_genome_flag`, `rank`, `hidden`, `left_number`, `right_number`, `annotation_source`, `full_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `etaxi` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `etaxi` SET `tax_id`='{0}', `parent_id`='{1}', `scientific_name`='{2}', `complete_genome_flag`='{3}', `rank`='{4}', `hidden`='{5}', `left_number`='{6}', `right_number`='{7}', `annotation_source`='{8}', `full_name`='{9}' WHERE ;</SQL>

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
''' ```SQL
''' INSERT INTO `etaxi` (`tax_id`, `parent_id`, `scientific_name`, `complete_genome_flag`, `rank`, `hidden`, `left_number`, `right_number`, `annotation_source`, `full_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, tax_id, parent_id, scientific_name, complete_genome_flag, rank, hidden, left_number, right_number, annotation_source, full_name)
        Else
        Return String.Format(INSERT_SQL, tax_id, parent_id, scientific_name, complete_genome_flag, rank, hidden, left_number, right_number, annotation_source, full_name)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{tax_id}', '{parent_id}', '{scientific_name}', '{complete_genome_flag}', '{rank}', '{hidden}', '{left_number}', '{right_number}', '{annotation_source}', '{full_name}')"
        Else
            Return $"('{tax_id}', '{parent_id}', '{scientific_name}', '{complete_genome_flag}', '{rank}', '{hidden}', '{left_number}', '{right_number}', '{annotation_source}', '{full_name}')"
        End If
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
''' REPLACE INTO `etaxi` (`tax_id`, `parent_id`, `scientific_name`, `complete_genome_flag`, `rank`, `hidden`, `left_number`, `right_number`, `annotation_source`, `full_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, tax_id, parent_id, scientific_name, complete_genome_flag, rank, hidden, left_number, right_number, annotation_source, full_name)
        Else
        Return String.Format(REPLACE_SQL, tax_id, parent_id, scientific_name, complete_genome_flag, rank, hidden, left_number, right_number, annotation_source, full_name)
        End If
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

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As etaxi
                         Return DirectCast(MyClass.MemberwiseClone, etaxi)
                     End Function
End Class


End Namespace
