#Region "Microsoft.VisualBasic::a806e3627143f68e32aa18cbd35935fe, ..\repository\DataMySql\Xfam\Rfam\Tables\matches_and_fasta.vb"

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
''' DROP TABLE IF EXISTS `matches_and_fasta`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `matches_and_fasta` (
'''   `rfam_acc` varchar(7) NOT NULL,
'''   `match_list` longblob,
'''   `fasta` longblob,
'''   `type` enum('rfamseq','genome','refseq') NOT NULL,
'''   KEY `fk_matches_and_fasta_family1_idx` (`rfam_acc`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("matches_and_fasta", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `matches_and_fasta` (
  `rfam_acc` varchar(7) NOT NULL,
  `match_list` longblob,
  `fasta` longblob,
  `type` enum('rfamseq','genome','refseq') NOT NULL,
  KEY `fk_matches_and_fasta_family1_idx` (`rfam_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class matches_and_fasta: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property rfam_acc As String
    <DatabaseField("match_list"), DataType(MySqlDbType.Blob)> Public Property match_list As Byte()
    <DatabaseField("fasta"), DataType(MySqlDbType.Blob)> Public Property fasta As Byte()
    <DatabaseField("type"), NotNull, DataType(MySqlDbType.String)> Public Property type As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `matches_and_fasta` (`rfam_acc`, `match_list`, `fasta`, `type`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `matches_and_fasta` (`rfam_acc`, `match_list`, `fasta`, `type`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `matches_and_fasta` WHERE `rfam_acc` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `matches_and_fasta` SET `rfam_acc`='{0}', `match_list`='{1}', `fasta`='{2}', `type`='{3}' WHERE `rfam_acc` = '{4}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `matches_and_fasta` WHERE `rfam_acc` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfam_acc)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `matches_and_fasta` (`rfam_acc`, `match_list`, `fasta`, `type`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_acc, match_list, fasta, type)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{rfam_acc}', '{match_list}', '{fasta}', '{type}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `matches_and_fasta` (`rfam_acc`, `match_list`, `fasta`, `type`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_acc, match_list, fasta, type)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `matches_and_fasta` SET `rfam_acc`='{0}', `match_list`='{1}', `fasta`='{2}', `type`='{3}' WHERE `rfam_acc` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfam_acc, match_list, fasta, type, rfam_acc)
    End Function
#End Region
End Class


End Namespace

