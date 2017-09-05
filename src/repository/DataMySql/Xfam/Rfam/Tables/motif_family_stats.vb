#Region "Microsoft.VisualBasic::09be803c4b367bafe9eb21ed56b420ed, ..\repository\DataMySql\Xfam\Rfam\Tables\motif_family_stats.vb"

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
''' DROP TABLE IF EXISTS `motif_family_stats`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `motif_family_stats` (
'''   `rfam_acc` varchar(7) NOT NULL,
'''   `motif_acc` varchar(7) NOT NULL,
'''   `num_hits` int(11) DEFAULT NULL,
'''   `frac_hits` decimal(4,3) DEFAULT NULL,
'''   `sum_bits` decimal(12,3) DEFAULT NULL,
'''   `avg_weight_bits` decimal(12,3) DEFAULT NULL,
'''   KEY `motif_family_stats_rfam_acc_idx` (`rfam_acc`),
'''   KEY `motif_family_stats_motif_acc_idx` (`motif_acc`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("motif_family_stats", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `motif_family_stats` (
  `rfam_acc` varchar(7) NOT NULL,
  `motif_acc` varchar(7) NOT NULL,
  `num_hits` int(11) DEFAULT NULL,
  `frac_hits` decimal(4,3) DEFAULT NULL,
  `sum_bits` decimal(12,3) DEFAULT NULL,
  `avg_weight_bits` decimal(12,3) DEFAULT NULL,
  KEY `motif_family_stats_rfam_acc_idx` (`rfam_acc`),
  KEY `motif_family_stats_motif_acc_idx` (`motif_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class motif_family_stats: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property rfam_acc As String
    <DatabaseField("motif_acc"), NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property motif_acc As String
    <DatabaseField("num_hits"), DataType(MySqlDbType.Int64, "11")> Public Property num_hits As Long
    <DatabaseField("frac_hits"), DataType(MySqlDbType.Decimal)> Public Property frac_hits As Decimal
    <DatabaseField("sum_bits"), DataType(MySqlDbType.Decimal)> Public Property sum_bits As Decimal
    <DatabaseField("avg_weight_bits"), DataType(MySqlDbType.Decimal)> Public Property avg_weight_bits As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `motif_family_stats` (`rfam_acc`, `motif_acc`, `num_hits`, `frac_hits`, `sum_bits`, `avg_weight_bits`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `motif_family_stats` (`rfam_acc`, `motif_acc`, `num_hits`, `frac_hits`, `sum_bits`, `avg_weight_bits`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `motif_family_stats` WHERE `rfam_acc` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `motif_family_stats` SET `rfam_acc`='{0}', `motif_acc`='{1}', `num_hits`='{2}', `frac_hits`='{3}', `sum_bits`='{4}', `avg_weight_bits`='{5}' WHERE `rfam_acc` = '{6}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `motif_family_stats` WHERE `rfam_acc` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfam_acc)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `motif_family_stats` (`rfam_acc`, `motif_acc`, `num_hits`, `frac_hits`, `sum_bits`, `avg_weight_bits`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_acc, motif_acc, num_hits, frac_hits, sum_bits, avg_weight_bits)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{rfam_acc}', '{motif_acc}', '{num_hits}', '{frac_hits}', '{sum_bits}', '{avg_weight_bits}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `motif_family_stats` (`rfam_acc`, `motif_acc`, `num_hits`, `frac_hits`, `sum_bits`, `avg_weight_bits`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_acc, motif_acc, num_hits, frac_hits, sum_bits, avg_weight_bits)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `motif_family_stats` SET `rfam_acc`='{0}', `motif_acc`='{1}', `num_hits`='{2}', `frac_hits`='{3}', `sum_bits`='{4}', `avg_weight_bits`='{5}' WHERE `rfam_acc` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfam_acc, motif_acc, num_hits, frac_hits, sum_bits, avg_weight_bits, rfam_acc)
    End Function
#End Region
End Class


End Namespace

