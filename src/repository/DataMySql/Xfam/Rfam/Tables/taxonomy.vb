#Region "Microsoft.VisualBasic::144b18945b90a150f6941a2e07d7c4a6, DataMySql\Xfam\Rfam\Tables\taxonomy.vb"

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

    ' Class taxonomy
    ' 
    '     Properties: align_display_name, ncbi_id, species, tax_string, tree_display_name
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
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class taxonomy: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ncbi_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="ncbi_id"), XmlAttribute> Public Property ncbi_id As Long
    <DatabaseField("species"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="species")> Public Property species As String
    <DatabaseField("tax_string"), DataType(MySqlDbType.Text), Column(Name:="tax_string")> Public Property tax_string As String
    <DatabaseField("tree_display_name"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="tree_display_name")> Public Property tree_display_name As String
    <DatabaseField("align_display_name"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="align_display_name")> Public Property align_display_name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `taxonomy` (`ncbi_id`, `species`, `tax_string`, `tree_display_name`, `align_display_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `taxonomy` (`ncbi_id`, `species`, `tax_string`, `tree_display_name`, `align_display_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `taxonomy` (`ncbi_id`, `species`, `tax_string`, `tree_display_name`, `align_display_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `taxonomy` (`ncbi_id`, `species`, `tax_string`, `tree_display_name`, `align_display_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `taxonomy` WHERE `ncbi_id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `taxonomy` SET `ncbi_id`='{0}', `species`='{1}', `tax_string`='{2}', `tree_display_name`='{3}', `align_display_name`='{4}' WHERE `ncbi_id` = '{5}';</SQL>

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
''' ```SQL
''' INSERT INTO `taxonomy` (`ncbi_id`, `species`, `tax_string`, `tree_display_name`, `align_display_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, ncbi_id, species, tax_string, tree_display_name, align_display_name)
        Else
        Return String.Format(INSERT_SQL, ncbi_id, species, tax_string, tree_display_name, align_display_name)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{ncbi_id}', '{species}', '{tax_string}', '{tree_display_name}', '{align_display_name}')"
        Else
            Return $"('{ncbi_id}', '{species}', '{tax_string}', '{tree_display_name}', '{align_display_name}')"
        End If
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
''' REPLACE INTO `taxonomy` (`ncbi_id`, `species`, `tax_string`, `tree_display_name`, `align_display_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, ncbi_id, species, tax_string, tree_display_name, align_display_name)
        Else
        Return String.Format(REPLACE_SQL, ncbi_id, species, tax_string, tree_display_name, align_display_name)
        End If
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

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As taxonomy
                         Return DirectCast(MyClass.MemberwiseClone, taxonomy)
                     End Function
End Class


End Namespace
