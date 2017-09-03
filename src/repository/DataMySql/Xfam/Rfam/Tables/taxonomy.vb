#Region "Microsoft.VisualBasic::5cdd8611b4e29e9a90596eae7e5c97fe, ..\repository\DataMySql\Xfam\Rfam\Tables\taxonomy.vb"

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
''' DROP TABLE IF EXISTS `taxonomy`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `taxonomy` (
'''   `ncbi_id` int(10) unsigned NOT NULL DEFAULT '0',
'''   `species` varchar(100) NOT NULL DEFAULT '',
'''   `tax_string` mediumtext,
'''   `tree_display_name` varchar(100) DEFAULT NULL,
'''   `align_display_name` varchar(50) DEFAULT NULL,
'''   PRIMARY KEY (`ncbi_id`),
'''   KEY `species` (`species`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("taxonomy", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `taxonomy` (
  `ncbi_id` int(10) unsigned NOT NULL DEFAULT '0',
  `species` varchar(100) NOT NULL DEFAULT '',
  `tax_string` mediumtext,
  `tree_display_name` varchar(100) DEFAULT NULL,
  `align_display_name` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`ncbi_id`),
  KEY `species` (`species`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class taxonomy: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ncbi_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property ncbi_id As Long
    <DatabaseField("species"), NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property species As String
    <DatabaseField("tax_string"), DataType(MySqlDbType.Text)> Public Property tax_string As String
    <DatabaseField("tree_display_name"), DataType(MySqlDbType.VarChar, "100")> Public Property tree_display_name As String
    <DatabaseField("align_display_name"), DataType(MySqlDbType.VarChar, "50")> Public Property align_display_name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `taxonomy` (`ncbi_id`, `species`, `tax_string`, `tree_display_name`, `align_display_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `taxonomy` (`ncbi_id`, `species`, `tax_string`, `tree_display_name`, `align_display_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `taxonomy` WHERE `ncbi_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `taxonomy` SET `ncbi_id`='{0}', `species`='{1}', `tax_string`='{2}', `tree_display_name`='{3}', `align_display_name`='{4}' WHERE `ncbi_id` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `taxonomy` WHERE `ncbi_id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ncbi_id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `taxonomy` (`ncbi_id`, `species`, `tax_string`, `tree_display_name`, `align_display_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ncbi_id, species, tax_string, tree_display_name, align_display_name)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{ncbi_id}', '{species}', '{tax_string}', '{tree_display_name}', '{align_display_name}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `taxonomy` (`ncbi_id`, `species`, `tax_string`, `tree_display_name`, `align_display_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ncbi_id, species, tax_string, tree_display_name, align_display_name)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `taxonomy` SET `ncbi_id`='{0}', `species`='{1}', `tax_string`='{2}', `tree_display_name`='{3}', `align_display_name`='{4}' WHERE `ncbi_id` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ncbi_id, species, tax_string, tree_display_name, align_display_name, ncbi_id)
    End Function
#End Region
End Class


End Namespace

