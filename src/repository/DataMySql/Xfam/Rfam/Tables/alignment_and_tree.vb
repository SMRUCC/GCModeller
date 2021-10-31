#Region "Microsoft.VisualBasic::3c7ca1567f06f655bded8c14b6fadaae, DataMySql\Xfam\Rfam\Tables\alignment_and_tree.vb"

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

    ' Class alignment_and_tree
    ' 
    '     Properties: alignment, average_length, number_of_sequences, percent_id, rfam_acc
    '                 tree, treemethod, type
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

REM  Dump @2018/5/23 13:13:34


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

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
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class alignment_and_tree: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="rfam_acc"), XmlAttribute> Public Property rfam_acc As String
    <DatabaseField("type"), NotNull, DataType(MySqlDbType.String), Column(Name:="type")> Public Property type As String
    <DatabaseField("alignment"), DataType(MySqlDbType.Blob), Column(Name:="alignment")> Public Property alignment As Byte()
    <DatabaseField("tree"), DataType(MySqlDbType.Blob), Column(Name:="tree")> Public Property tree As Byte()
    <DatabaseField("treemethod"), DataType(MySqlDbType.Text), Column(Name:="treemethod")> Public Property treemethod As String
    <DatabaseField("average_length"), DataType(MySqlDbType.Double), Column(Name:="average_length")> Public Property average_length As Double
    <DatabaseField("percent_id"), DataType(MySqlDbType.Double), Column(Name:="percent_id")> Public Property percent_id As Double
    <DatabaseField("number_of_sequences"), DataType(MySqlDbType.Int64, "20"), Column(Name:="number_of_sequences")> Public Property number_of_sequences As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `alignment_and_tree` (`rfam_acc`, `type`, `alignment`, `tree`, `treemethod`, `average_length`, `percent_id`, `number_of_sequences`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `alignment_and_tree` (`rfam_acc`, `type`, `alignment`, `tree`, `treemethod`, `average_length`, `percent_id`, `number_of_sequences`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `alignment_and_tree` (`rfam_acc`, `type`, `alignment`, `tree`, `treemethod`, `average_length`, `percent_id`, `number_of_sequences`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `alignment_and_tree` (`rfam_acc`, `type`, `alignment`, `tree`, `treemethod`, `average_length`, `percent_id`, `number_of_sequences`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `alignment_and_tree` WHERE `rfam_acc` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `alignment_and_tree` SET `rfam_acc`='{0}', `type`='{1}', `alignment`='{2}', `tree`='{3}', `treemethod`='{4}', `average_length`='{5}', `percent_id`='{6}', `number_of_sequences`='{7}' WHERE `rfam_acc` = '{8}';</SQL>

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
''' ```SQL
''' INSERT INTO `alignment_and_tree` (`rfam_acc`, `type`, `alignment`, `tree`, `treemethod`, `average_length`, `percent_id`, `number_of_sequences`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, rfam_acc, type, alignment, tree, treemethod, average_length, percent_id, number_of_sequences)
        Else
        Return String.Format(INSERT_SQL, rfam_acc, type, alignment, tree, treemethod, average_length, percent_id, number_of_sequences)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{rfam_acc}', '{type}', '{alignment}', '{tree}', '{treemethod}', '{average_length}', '{percent_id}', '{number_of_sequences}')"
        Else
            Return $"('{rfam_acc}', '{type}', '{alignment}', '{tree}', '{treemethod}', '{average_length}', '{percent_id}', '{number_of_sequences}')"
        End If
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
''' REPLACE INTO `alignment_and_tree` (`rfam_acc`, `type`, `alignment`, `tree`, `treemethod`, `average_length`, `percent_id`, `number_of_sequences`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, rfam_acc, type, alignment, tree, treemethod, average_length, percent_id, number_of_sequences)
        Else
        Return String.Format(REPLACE_SQL, rfam_acc, type, alignment, tree, treemethod, average_length, percent_id, number_of_sequences)
        End If
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

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As alignment_and_tree
                         Return DirectCast(MyClass.MemberwiseClone, alignment_and_tree)
                     End Function
End Class


End Namespace
