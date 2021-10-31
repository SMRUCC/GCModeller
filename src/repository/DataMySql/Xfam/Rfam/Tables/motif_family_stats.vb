#Region "Microsoft.VisualBasic::0838db8b05e31d0a040c4e6576533d68, DataMySql\Xfam\Rfam\Tables\motif_family_stats.vb"

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

    ' Class motif_family_stats
    ' 
    '     Properties: avg_weight_bits, frac_hits, motif_acc, num_hits, rfam_acc
    '                 sum_bits
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
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class motif_family_stats: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="rfam_acc"), XmlAttribute> Public Property rfam_acc As String
    <DatabaseField("motif_acc"), NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="motif_acc")> Public Property motif_acc As String
    <DatabaseField("num_hits"), DataType(MySqlDbType.Int64, "11"), Column(Name:="num_hits")> Public Property num_hits As Long
    <DatabaseField("frac_hits"), DataType(MySqlDbType.Decimal), Column(Name:="frac_hits")> Public Property frac_hits As Decimal
    <DatabaseField("sum_bits"), DataType(MySqlDbType.Decimal), Column(Name:="sum_bits")> Public Property sum_bits As Decimal
    <DatabaseField("avg_weight_bits"), DataType(MySqlDbType.Decimal), Column(Name:="avg_weight_bits")> Public Property avg_weight_bits As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `motif_family_stats` (`rfam_acc`, `motif_acc`, `num_hits`, `frac_hits`, `sum_bits`, `avg_weight_bits`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `motif_family_stats` (`rfam_acc`, `motif_acc`, `num_hits`, `frac_hits`, `sum_bits`, `avg_weight_bits`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `motif_family_stats` (`rfam_acc`, `motif_acc`, `num_hits`, `frac_hits`, `sum_bits`, `avg_weight_bits`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `motif_family_stats` (`rfam_acc`, `motif_acc`, `num_hits`, `frac_hits`, `sum_bits`, `avg_weight_bits`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `motif_family_stats` WHERE `rfam_acc` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `motif_family_stats` SET `rfam_acc`='{0}', `motif_acc`='{1}', `num_hits`='{2}', `frac_hits`='{3}', `sum_bits`='{4}', `avg_weight_bits`='{5}' WHERE `rfam_acc` = '{6}';</SQL>

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
''' ```SQL
''' INSERT INTO `motif_family_stats` (`rfam_acc`, `motif_acc`, `num_hits`, `frac_hits`, `sum_bits`, `avg_weight_bits`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, rfam_acc, motif_acc, num_hits, frac_hits, sum_bits, avg_weight_bits)
        Else
        Return String.Format(INSERT_SQL, rfam_acc, motif_acc, num_hits, frac_hits, sum_bits, avg_weight_bits)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{rfam_acc}', '{motif_acc}', '{num_hits}', '{frac_hits}', '{sum_bits}', '{avg_weight_bits}')"
        Else
            Return $"('{rfam_acc}', '{motif_acc}', '{num_hits}', '{frac_hits}', '{sum_bits}', '{avg_weight_bits}')"
        End If
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
''' REPLACE INTO `motif_family_stats` (`rfam_acc`, `motif_acc`, `num_hits`, `frac_hits`, `sum_bits`, `avg_weight_bits`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, rfam_acc, motif_acc, num_hits, frac_hits, sum_bits, avg_weight_bits)
        Else
        Return String.Format(REPLACE_SQL, rfam_acc, motif_acc, num_hits, frac_hits, sum_bits, avg_weight_bits)
        End If
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

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As motif_family_stats
                         Return DirectCast(MyClass.MemberwiseClone, motif_family_stats)
                     End Function
End Class


End Namespace
