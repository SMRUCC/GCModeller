#Region "Microsoft.VisualBasic::aabe38b07b01cb092b7ac0e5cec5201b, ..\repository\DataMySql\Xfam\Rfam\Tables\alignment_and_tree.vb"

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
''' DROP TABLE IF EXISTS `alignment_and_tree`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `alignment_and_tree` (
'''   `rfam_acc` varchar(7) NOT NULL,
'''   `type` enum('seed','seedTax','genome','genomeTax') NOT NULL,
'''   `alignment` longblob,
'''   `tree` longblob,
'''   `treemethod` tinytext,
'''   `average_length` double(7,2) DEFAULT NULL,
'''   `percent_id` double(5,2) DEFAULT NULL,
'''   `number_of_sequences` bigint(20) DEFAULT NULL,
'''   KEY `fk_alignments_and_trees_family1_idx` (`rfam_acc`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("alignment_and_tree", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `alignment_and_tree` (
  `rfam_acc` varchar(7) NOT NULL,
  `type` enum('seed','seedTax','genome','genomeTax') NOT NULL,
  `alignment` longblob,
  `tree` longblob,
  `treemethod` tinytext,
  `average_length` double(7,2) DEFAULT NULL,
  `percent_id` double(5,2) DEFAULT NULL,
  `number_of_sequences` bigint(20) DEFAULT NULL,
  KEY `fk_alignments_and_trees_family1_idx` (`rfam_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class alignment_and_tree: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property rfam_acc As String
    <DatabaseField("type"), NotNull, DataType(MySqlDbType.String)> Public Property type As String
    <DatabaseField("alignment"), DataType(MySqlDbType.Blob)> Public Property alignment As Byte()
    <DatabaseField("tree"), DataType(MySqlDbType.Blob)> Public Property tree As Byte()
    <DatabaseField("treemethod"), DataType(MySqlDbType.Text)> Public Property treemethod As String
    <DatabaseField("average_length"), DataType(MySqlDbType.Double)> Public Property average_length As Double
    <DatabaseField("percent_id"), DataType(MySqlDbType.Double)> Public Property percent_id As Double
    <DatabaseField("number_of_sequences"), DataType(MySqlDbType.Int64, "20")> Public Property number_of_sequences As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `alignment_and_tree` (`rfam_acc`, `type`, `alignment`, `tree`, `treemethod`, `average_length`, `percent_id`, `number_of_sequences`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `alignment_and_tree` (`rfam_acc`, `type`, `alignment`, `tree`, `treemethod`, `average_length`, `percent_id`, `number_of_sequences`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `alignment_and_tree` WHERE `rfam_acc` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `alignment_and_tree` SET `rfam_acc`='{0}', `type`='{1}', `alignment`='{2}', `tree`='{3}', `treemethod`='{4}', `average_length`='{5}', `percent_id`='{6}', `number_of_sequences`='{7}' WHERE `rfam_acc` = '{8}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `alignment_and_tree` WHERE `rfam_acc` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfam_acc)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `alignment_and_tree` (`rfam_acc`, `type`, `alignment`, `tree`, `treemethod`, `average_length`, `percent_id`, `number_of_sequences`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_acc, type, alignment, tree, treemethod, average_length, percent_id, number_of_sequences)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{rfam_acc}', '{type}', '{alignment}', '{tree}', '{treemethod}', '{average_length}', '{percent_id}', '{number_of_sequences}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `alignment_and_tree` (`rfam_acc`, `type`, `alignment`, `tree`, `treemethod`, `average_length`, `percent_id`, `number_of_sequences`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_acc, type, alignment, tree, treemethod, average_length, percent_id, number_of_sequences)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `alignment_and_tree` SET `rfam_acc`='{0}', `type`='{1}', `alignment`='{2}', `tree`='{3}', `treemethod`='{4}', `average_length`='{5}', `percent_id`='{6}', `number_of_sequences`='{7}' WHERE `rfam_acc` = '{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfam_acc, type, alignment, tree, treemethod, average_length, percent_id, number_of_sequences, rfam_acc)
    End Function
#End Region
End Class


End Namespace

