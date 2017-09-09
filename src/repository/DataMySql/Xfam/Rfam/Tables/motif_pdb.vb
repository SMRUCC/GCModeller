#Region "Microsoft.VisualBasic::db52d150bbc5139060ab58d200910fa6, ..\repository\DataMySql\Xfam\Rfam\Tables\motif_pdb.vb"

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
''' DROP TABLE IF EXISTS `motif_pdb`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `motif_pdb` (
'''   `motif_acc` varchar(7) NOT NULL,
'''   `pdb_id` varchar(4) NOT NULL,
'''   `chain` varchar(4) DEFAULT NULL,
'''   `pdb_start` mediumint(9) DEFAULT NULL,
'''   `pdb_end` mediumint(9) DEFAULT NULL,
'''   KEY `motif_pdb_pdb_idx` (`pdb_id`),
'''   KEY `motif_pdb_motif_acc_idx` (`motif_acc`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("motif_pdb", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `motif_pdb` (
  `motif_acc` varchar(7) NOT NULL,
  `pdb_id` varchar(4) NOT NULL,
  `chain` varchar(4) DEFAULT NULL,
  `pdb_start` mediumint(9) DEFAULT NULL,
  `pdb_end` mediumint(9) DEFAULT NULL,
  KEY `motif_pdb_pdb_idx` (`pdb_id`),
  KEY `motif_pdb_motif_acc_idx` (`motif_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class motif_pdb: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("motif_acc"), NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property motif_acc As String
    <DatabaseField("pdb_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "4")> Public Property pdb_id As String
    <DatabaseField("chain"), DataType(MySqlDbType.VarChar, "4")> Public Property chain As String
    <DatabaseField("pdb_start"), DataType(MySqlDbType.Int64, "9")> Public Property pdb_start As Long
    <DatabaseField("pdb_end"), DataType(MySqlDbType.Int64, "9")> Public Property pdb_end As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `motif_pdb` (`motif_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `motif_pdb` (`motif_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `motif_pdb` WHERE `pdb_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `motif_pdb` SET `motif_acc`='{0}', `pdb_id`='{1}', `chain`='{2}', `pdb_start`='{3}', `pdb_end`='{4}' WHERE `pdb_id` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `motif_pdb` WHERE `pdb_id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pdb_id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `motif_pdb` (`motif_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, motif_acc, pdb_id, chain, pdb_start, pdb_end)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{motif_acc}', '{pdb_id}', '{chain}', '{pdb_start}', '{pdb_end}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `motif_pdb` (`motif_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, motif_acc, pdb_id, chain, pdb_start, pdb_end)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `motif_pdb` SET `motif_acc`='{0}', `pdb_id`='{1}', `chain`='{2}', `pdb_start`='{3}', `pdb_end`='{4}' WHERE `pdb_id` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, motif_acc, pdb_id, chain, pdb_start, pdb_end, pdb_id)
    End Function
#End Region
End Class


End Namespace

