#Region "Microsoft.VisualBasic::d0af80b94ad5d964c0d7977be2343c84, ..\repository\DataMySql\Interpro\Tables\taxonomy2protein.vb"

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
''' DROP TABLE IF EXISTS `taxonomy2protein`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `taxonomy2protein` (
'''   `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `tax_id` bigint(15) NOT NULL,
'''   `hierarchy` varchar(200) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `tax_name_concat` varchar(80) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   PRIMARY KEY (`protein_ac`,`tax_id`),
'''   CONSTRAINT `fk_taxonomy2protein$p` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("taxonomy2protein", Database:="interpro", SchemaSQL:="
CREATE TABLE `taxonomy2protein` (
  `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `tax_id` bigint(15) NOT NULL,
  `hierarchy` varchar(200) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `tax_name_concat` varchar(80) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`protein_ac`,`tax_id`),
  CONSTRAINT `fk_taxonomy2protein$p` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class taxonomy2protein: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("protein_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "6")> Public Property protein_ac As String
    <DatabaseField("tax_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "15")> Public Property tax_id As Long
    <DatabaseField("hierarchy"), DataType(MySqlDbType.VarChar, "200")> Public Property hierarchy As String
    <DatabaseField("tax_name_concat"), DataType(MySqlDbType.VarChar, "80")> Public Property tax_name_concat As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `taxonomy2protein` (`protein_ac`, `tax_id`, `hierarchy`, `tax_name_concat`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `taxonomy2protein` (`protein_ac`, `tax_id`, `hierarchy`, `tax_name_concat`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `taxonomy2protein` WHERE `protein_ac`='{0}' and `tax_id`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `taxonomy2protein` SET `protein_ac`='{0}', `tax_id`='{1}', `hierarchy`='{2}', `tax_name_concat`='{3}' WHERE `protein_ac`='{4}' and `tax_id`='{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `taxonomy2protein` WHERE `protein_ac`='{0}' and `tax_id`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, protein_ac, tax_id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `taxonomy2protein` (`protein_ac`, `tax_id`, `hierarchy`, `tax_name_concat`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, protein_ac, tax_id, hierarchy, tax_name_concat)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{protein_ac}', '{tax_id}', '{hierarchy}', '{tax_name_concat}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `taxonomy2protein` (`protein_ac`, `tax_id`, `hierarchy`, `tax_name_concat`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, protein_ac, tax_id, hierarchy, tax_name_concat)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `taxonomy2protein` SET `protein_ac`='{0}', `tax_id`='{1}', `hierarchy`='{2}', `tax_name_concat`='{3}' WHERE `protein_ac`='{4}' and `tax_id`='{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, protein_ac, tax_id, hierarchy, tax_name_concat, protein_ac, tax_id)
    End Function
#End Region
End Class


End Namespace

