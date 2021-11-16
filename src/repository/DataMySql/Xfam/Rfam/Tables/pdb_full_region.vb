#Region "Microsoft.VisualBasic::83045e3d572847df9b49d76bcd8e1170, DataMySql\Xfam\Rfam\Tables\pdb_full_region.vb"

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

    ' Class pdb_full_region
    ' 
    '     Properties: bit_score, chain, cm_end, cm_start, evalue_score
    '                 hex_colour, pdb_end, pdb_id, pdb_start, rfam_acc
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
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class pdb_full_region: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="rfam_acc"), XmlAttribute> Public Property rfam_acc As String
    <DatabaseField("pdb_id"), NotNull, DataType(MySqlDbType.VarChar, "4"), Column(Name:="pdb_id")> Public Property pdb_id As String
    <DatabaseField("chain"), DataType(MySqlDbType.VarChar, "4"), Column(Name:="chain")> Public Property chain As String
    <DatabaseField("pdb_start"), NotNull, DataType(MySqlDbType.Int64, "8"), Column(Name:="pdb_start")> Public Property pdb_start As Long
    <DatabaseField("pdb_end"), NotNull, DataType(MySqlDbType.Int64, "8"), Column(Name:="pdb_end")> Public Property pdb_end As Long
    <DatabaseField("bit_score"), NotNull, DataType(MySqlDbType.Double), Column(Name:="bit_score")> Public Property bit_score As Double
    <DatabaseField("evalue_score"), NotNull, DataType(MySqlDbType.VarChar, "15"), Column(Name:="evalue_score")> Public Property evalue_score As String
    <DatabaseField("cm_start"), NotNull, DataType(MySqlDbType.Int64, "8"), Column(Name:="cm_start")> Public Property cm_start As Long
    <DatabaseField("cm_end"), NotNull, DataType(MySqlDbType.Int64, "8"), Column(Name:="cm_end")> Public Property cm_end As Long
    <DatabaseField("hex_colour"), DataType(MySqlDbType.VarChar, "6"), Column(Name:="hex_colour")> Public Property hex_colour As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `pdb_full_region` (`rfam_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `hex_colour`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `pdb_full_region` (`rfam_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `hex_colour`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `pdb_full_region` (`rfam_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `hex_colour`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `pdb_full_region` (`rfam_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `hex_colour`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `pdb_full_region` WHERE `rfam_acc` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `pdb_full_region` SET `rfam_acc`='{0}', `pdb_id`='{1}', `chain`='{2}', `pdb_start`='{3}', `pdb_end`='{4}', `bit_score`='{5}', `evalue_score`='{6}', `cm_start`='{7}', `cm_end`='{8}', `hex_colour`='{9}' WHERE `rfam_acc` = '{10}';</SQL>

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
''' ```SQL
''' INSERT INTO `pdb_full_region` (`rfam_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `hex_colour`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, rfam_acc, pdb_id, chain, pdb_start, pdb_end, bit_score, evalue_score, cm_start, cm_end, hex_colour)
        Else
        Return String.Format(INSERT_SQL, rfam_acc, pdb_id, chain, pdb_start, pdb_end, bit_score, evalue_score, cm_start, cm_end, hex_colour)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{rfam_acc}', '{pdb_id}', '{chain}', '{pdb_start}', '{pdb_end}', '{bit_score}', '{evalue_score}', '{cm_start}', '{cm_end}', '{hex_colour}')"
        Else
            Return $"('{rfam_acc}', '{pdb_id}', '{chain}', '{pdb_start}', '{pdb_end}', '{bit_score}', '{evalue_score}', '{cm_start}', '{cm_end}', '{hex_colour}')"
        End If
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
''' REPLACE INTO `pdb_full_region` (`rfam_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `hex_colour`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, rfam_acc, pdb_id, chain, pdb_start, pdb_end, bit_score, evalue_score, cm_start, cm_end, hex_colour)
        Else
        Return String.Format(REPLACE_SQL, rfam_acc, pdb_id, chain, pdb_start, pdb_end, bit_score, evalue_score, cm_start, cm_end, hex_colour)
        End If
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

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As pdb_full_region
                         Return DirectCast(MyClass.MemberwiseClone, pdb_full_region)
                     End Function
End Class


End Namespace
