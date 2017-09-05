#Region "Microsoft.VisualBasic::809a5ef664176fd4d2a07480343f1fe1, ..\repository\DataMySql\Interpro\Tables\uniprot_taxonomy.vb"

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
''' DROP TABLE IF EXISTS `uniprot_taxonomy`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `uniprot_taxonomy` (
'''   `protein_ac` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `tax_id` bigint(15) NOT NULL,
'''   `left_number` bigint(15) NOT NULL,
'''   `right_number` bigint(15) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("uniprot_taxonomy", Database:="interpro", SchemaSQL:="
CREATE TABLE `uniprot_taxonomy` (
  `protein_ac` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `tax_id` bigint(15) NOT NULL,
  `left_number` bigint(15) NOT NULL,
  `right_number` bigint(15) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class uniprot_taxonomy: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("protein_ac"), NotNull, DataType(MySqlDbType.VarChar, "10")> Public Property protein_ac As String
    <DatabaseField("tax_id"), NotNull, DataType(MySqlDbType.Int64, "15")> Public Property tax_id As Long
    <DatabaseField("left_number"), NotNull, DataType(MySqlDbType.Int64, "15")> Public Property left_number As Long
    <DatabaseField("right_number"), NotNull, DataType(MySqlDbType.Int64, "15")> Public Property right_number As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `uniprot_taxonomy` (`protein_ac`, `tax_id`, `left_number`, `right_number`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `uniprot_taxonomy` (`protein_ac`, `tax_id`, `left_number`, `right_number`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `uniprot_taxonomy` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `uniprot_taxonomy` SET `protein_ac`='{0}', `tax_id`='{1}', `left_number`='{2}', `right_number`='{3}' WHERE ;</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `uniprot_taxonomy` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `uniprot_taxonomy` (`protein_ac`, `tax_id`, `left_number`, `right_number`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, protein_ac, tax_id, left_number, right_number)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{protein_ac}', '{tax_id}', '{left_number}', '{right_number}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `uniprot_taxonomy` (`protein_ac`, `tax_id`, `left_number`, `right_number`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, protein_ac, tax_id, left_number, right_number)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `uniprot_taxonomy` SET `protein_ac`='{0}', `tax_id`='{1}', `left_number`='{2}', `right_number`='{3}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace

