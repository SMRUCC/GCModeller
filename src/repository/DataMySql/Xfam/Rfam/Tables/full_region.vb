#Region "Microsoft.VisualBasic::79f6742b72f2ff6cd5df62e98d8e3bfd, DataMySql\Xfam\Rfam\Tables\full_region.vb"

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

    ' Class full_region
    ' 
    '     Properties: bit_score, cm_end, cm_start, evalue_score, is_significant
    '                 rfam_acc, rfamseq_acc, seq_end, seq_start, truncated
    '                 type
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
''' DROP TABLE IF EXISTS `full_region`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `full_region` (
'''   `rfam_acc` varchar(7) NOT NULL,
'''   `rfamseq_acc` varchar(20) NOT NULL,
'''   `seq_start` bigint(19) unsigned NOT NULL DEFAULT '0',
'''   `seq_end` bigint(19) unsigned NOT NULL,
'''   `bit_score` double(7,2) NOT NULL DEFAULT '0.00' COMMENT '99999.99 is the approx limit from Infernal.',
'''   `evalue_score` varchar(15) NOT NULL DEFAULT '0',
'''   `cm_start` mediumint(8) unsigned NOT NULL,
'''   `cm_end` mediumint(8) unsigned NOT NULL,
'''   `truncated` enum('0','5','3','53') NOT NULL,
'''   `type` enum('seed','full') NOT NULL DEFAULT 'full',
'''   `is_significant` tinyint(1) unsigned NOT NULL,
'''   KEY `full_region_sign` (`is_significant`),
'''   KEY `full_region_acc_sign` (`rfam_acc`,`is_significant`) USING BTREE,
'''   KEY `fk_full_region_rfamseq1_cascase` (`rfamseq_acc`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("full_region", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `full_region` (
  `rfam_acc` varchar(7) NOT NULL,
  `rfamseq_acc` varchar(20) NOT NULL,
  `seq_start` bigint(19) unsigned NOT NULL DEFAULT '0',
  `seq_end` bigint(19) unsigned NOT NULL,
  `bit_score` double(7,2) NOT NULL DEFAULT '0.00' COMMENT '99999.99 is the approx limit from Infernal.',
  `evalue_score` varchar(15) NOT NULL DEFAULT '0',
  `cm_start` mediumint(8) unsigned NOT NULL,
  `cm_end` mediumint(8) unsigned NOT NULL,
  `truncated` enum('0','5','3','53') NOT NULL,
  `type` enum('seed','full') NOT NULL DEFAULT 'full',
  `is_significant` tinyint(1) unsigned NOT NULL,
  KEY `full_region_sign` (`is_significant`),
  KEY `full_region_acc_sign` (`rfam_acc`,`is_significant`) USING BTREE,
  KEY `fk_full_region_rfamseq1_cascase` (`rfamseq_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class full_region: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="rfam_acc")> Public Property rfam_acc As String
    <DatabaseField("rfamseq_acc"), NotNull, DataType(MySqlDbType.VarChar, "20"), Column(Name:="rfamseq_acc")> Public Property rfamseq_acc As String
    <DatabaseField("seq_start"), NotNull, DataType(MySqlDbType.Int64, "19"), Column(Name:="seq_start")> Public Property seq_start As Long
    <DatabaseField("seq_end"), NotNull, DataType(MySqlDbType.Int64, "19"), Column(Name:="seq_end")> Public Property seq_end As Long
''' <summary>
''' 99999.99 is the approx limit from Infernal.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("bit_score"), NotNull, DataType(MySqlDbType.Double), Column(Name:="bit_score")> Public Property bit_score As Double
    <DatabaseField("evalue_score"), NotNull, DataType(MySqlDbType.VarChar, "15"), Column(Name:="evalue_score")> Public Property evalue_score As String
    <DatabaseField("cm_start"), NotNull, DataType(MySqlDbType.Int64, "8"), Column(Name:="cm_start")> Public Property cm_start As Long
    <DatabaseField("cm_end"), NotNull, DataType(MySqlDbType.Int64, "8"), Column(Name:="cm_end")> Public Property cm_end As Long
    <DatabaseField("truncated"), NotNull, DataType(MySqlDbType.String), Column(Name:="truncated")> Public Property truncated As String
    <DatabaseField("type"), NotNull, DataType(MySqlDbType.String), Column(Name:="type")> Public Property type As String
    <DatabaseField("is_significant"), PrimaryKey, NotNull, DataType(MySqlDbType.Boolean, "1"), Column(Name:="is_significant"), XmlAttribute> Public Property is_significant As Boolean
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `full_region` (`rfam_acc`, `rfamseq_acc`, `seq_start`, `seq_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `truncated`, `type`, `is_significant`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `full_region` (`rfam_acc`, `rfamseq_acc`, `seq_start`, `seq_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `truncated`, `type`, `is_significant`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `full_region` (`rfam_acc`, `rfamseq_acc`, `seq_start`, `seq_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `truncated`, `type`, `is_significant`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `full_region` (`rfam_acc`, `rfamseq_acc`, `seq_start`, `seq_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `truncated`, `type`, `is_significant`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `full_region` WHERE `is_significant` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `full_region` SET `rfam_acc`='{0}', `rfamseq_acc`='{1}', `seq_start`='{2}', `seq_end`='{3}', `bit_score`='{4}', `evalue_score`='{5}', `cm_start`='{6}', `cm_end`='{7}', `truncated`='{8}', `type`='{9}', `is_significant`='{10}' WHERE `is_significant` = '{11}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `full_region` WHERE `is_significant` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, is_significant)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `full_region` (`rfam_acc`, `rfamseq_acc`, `seq_start`, `seq_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `truncated`, `type`, `is_significant`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_acc, rfamseq_acc, seq_start, seq_end, bit_score, evalue_score, cm_start, cm_end, truncated, type, is_significant)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `full_region` (`rfam_acc`, `rfamseq_acc`, `seq_start`, `seq_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `truncated`, `type`, `is_significant`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, rfam_acc, rfamseq_acc, seq_start, seq_end, bit_score, evalue_score, cm_start, cm_end, truncated, type, is_significant)
        Else
        Return String.Format(INSERT_SQL, rfam_acc, rfamseq_acc, seq_start, seq_end, bit_score, evalue_score, cm_start, cm_end, truncated, type, is_significant)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{rfam_acc}', '{rfamseq_acc}', '{seq_start}', '{seq_end}', '{bit_score}', '{evalue_score}', '{cm_start}', '{cm_end}', '{truncated}', '{type}', '{is_significant}')"
        Else
            Return $"('{rfam_acc}', '{rfamseq_acc}', '{seq_start}', '{seq_end}', '{bit_score}', '{evalue_score}', '{cm_start}', '{cm_end}', '{truncated}', '{type}', '{is_significant}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `full_region` (`rfam_acc`, `rfamseq_acc`, `seq_start`, `seq_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `truncated`, `type`, `is_significant`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_acc, rfamseq_acc, seq_start, seq_end, bit_score, evalue_score, cm_start, cm_end, truncated, type, is_significant)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `full_region` (`rfam_acc`, `rfamseq_acc`, `seq_start`, `seq_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `truncated`, `type`, `is_significant`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, rfam_acc, rfamseq_acc, seq_start, seq_end, bit_score, evalue_score, cm_start, cm_end, truncated, type, is_significant)
        Else
        Return String.Format(REPLACE_SQL, rfam_acc, rfamseq_acc, seq_start, seq_end, bit_score, evalue_score, cm_start, cm_end, truncated, type, is_significant)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `full_region` SET `rfam_acc`='{0}', `rfamseq_acc`='{1}', `seq_start`='{2}', `seq_end`='{3}', `bit_score`='{4}', `evalue_score`='{5}', `cm_start`='{6}', `cm_end`='{7}', `truncated`='{8}', `type`='{9}', `is_significant`='{10}' WHERE `is_significant` = '{11}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfam_acc, rfamseq_acc, seq_start, seq_end, bit_score, evalue_score, cm_start, cm_end, truncated, type, is_significant, is_significant)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As full_region
                         Return DirectCast(MyClass.MemberwiseClone, full_region)
                     End Function
End Class


End Namespace
