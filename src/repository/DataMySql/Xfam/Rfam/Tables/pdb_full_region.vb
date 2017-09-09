#Region "Microsoft.VisualBasic::de9f663efb37800fa049cfe3fe6ce45d, ..\repository\DataMySql\Xfam\Rfam\Tables\pdb_full_region.vb"

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
''' DROP TABLE IF EXISTS `pdb_full_region`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `pdb_full_region` (
'''   `rfam_acc` varchar(7) NOT NULL,
'''   `pdb_id` varchar(4) NOT NULL,
'''   `chain` varchar(4) DEFAULT 'NULL',
'''   `pdb_start` mediumint(8) NOT NULL,
'''   `pdb_end` mediumint(8) NOT NULL,
'''   `bit_score` double(7,2) NOT NULL DEFAULT '0.00',
'''   `evalue_score` varchar(15) NOT NULL DEFAULT '0',
'''   `cm_start` mediumint(8) NOT NULL,
'''   `cm_end` mediumint(8) NOT NULL,
'''   `hex_colour` varchar(6) DEFAULT 'NULL',
'''   KEY `fk_pdb_rfam_reg_family1_idx` (`rfam_acc`),
'''   KEY `fk_pdb_rfam_reg_pdb1_idx` (`pdb_id`),
'''   KEY `rfam_acc` (`rfam_acc`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pdb_full_region", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `pdb_full_region` (
  `rfam_acc` varchar(7) NOT NULL,
  `pdb_id` varchar(4) NOT NULL,
  `chain` varchar(4) DEFAULT 'NULL',
  `pdb_start` mediumint(8) NOT NULL,
  `pdb_end` mediumint(8) NOT NULL,
  `bit_score` double(7,2) NOT NULL DEFAULT '0.00',
  `evalue_score` varchar(15) NOT NULL DEFAULT '0',
  `cm_start` mediumint(8) NOT NULL,
  `cm_end` mediumint(8) NOT NULL,
  `hex_colour` varchar(6) DEFAULT 'NULL',
  KEY `fk_pdb_rfam_reg_family1_idx` (`rfam_acc`),
  KEY `fk_pdb_rfam_reg_pdb1_idx` (`pdb_id`),
  KEY `rfam_acc` (`rfam_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class pdb_full_region: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property rfam_acc As String
    <DatabaseField("pdb_id"), NotNull, DataType(MySqlDbType.VarChar, "4")> Public Property pdb_id As String
    <DatabaseField("chain"), DataType(MySqlDbType.VarChar, "4")> Public Property chain As String
    <DatabaseField("pdb_start"), NotNull, DataType(MySqlDbType.Int64, "8")> Public Property pdb_start As Long
    <DatabaseField("pdb_end"), NotNull, DataType(MySqlDbType.Int64, "8")> Public Property pdb_end As Long
    <DatabaseField("bit_score"), NotNull, DataType(MySqlDbType.Double)> Public Property bit_score As Double
    <DatabaseField("evalue_score"), NotNull, DataType(MySqlDbType.VarChar, "15")> Public Property evalue_score As String
    <DatabaseField("cm_start"), NotNull, DataType(MySqlDbType.Int64, "8")> Public Property cm_start As Long
    <DatabaseField("cm_end"), NotNull, DataType(MySqlDbType.Int64, "8")> Public Property cm_end As Long
    <DatabaseField("hex_colour"), DataType(MySqlDbType.VarChar, "6")> Public Property hex_colour As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pdb_full_region` (`rfam_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `hex_colour`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pdb_full_region` (`rfam_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `hex_colour`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pdb_full_region` WHERE `rfam_acc` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pdb_full_region` SET `rfam_acc`='{0}', `pdb_id`='{1}', `chain`='{2}', `pdb_start`='{3}', `pdb_end`='{4}', `bit_score`='{5}', `evalue_score`='{6}', `cm_start`='{7}', `cm_end`='{8}', `hex_colour`='{9}' WHERE `rfam_acc` = '{10}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `pdb_full_region` WHERE `rfam_acc` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfam_acc)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `pdb_full_region` (`rfam_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `hex_colour`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_acc, pdb_id, chain, pdb_start, pdb_end, bit_score, evalue_score, cm_start, cm_end, hex_colour)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{rfam_acc}', '{pdb_id}', '{chain}', '{pdb_start}', '{pdb_end}', '{bit_score}', '{evalue_score}', '{cm_start}', '{cm_end}', '{hex_colour}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `pdb_full_region` (`rfam_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `hex_colour`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_acc, pdb_id, chain, pdb_start, pdb_end, bit_score, evalue_score, cm_start, cm_end, hex_colour)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `pdb_full_region` SET `rfam_acc`='{0}', `pdb_id`='{1}', `chain`='{2}', `pdb_start`='{3}', `pdb_end`='{4}', `bit_score`='{5}', `evalue_score`='{6}', `cm_start`='{7}', `cm_end`='{8}', `hex_colour`='{9}' WHERE `rfam_acc` = '{10}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfam_acc, pdb_id, chain, pdb_start, pdb_end, bit_score, evalue_score, cm_start, cm_end, hex_colour, rfam_acc)
    End Function
#End Region
End Class


End Namespace

