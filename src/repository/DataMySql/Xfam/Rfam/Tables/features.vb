#Region "Microsoft.VisualBasic::00e300b9b9416a35a74cbb6164ad9847, ..\repository\DataMySql\Xfam\Rfam\Tables\features.vb"

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
''' DROP TABLE IF EXISTS `features`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `features` (
'''   `rfamseq_acc` varchar(20) NOT NULL,
'''   `database_id` varchar(50) NOT NULL,
'''   `primary_id` varchar(100) NOT NULL,
'''   `secondary_id` varchar(255) DEFAULT NULL,
'''   `feat_orient` tinyint(3) NOT NULL DEFAULT '0',
'''   `feat_start` bigint(19) unsigned NOT NULL DEFAULT '0',
'''   `feat_end` bigint(19) unsigned NOT NULL DEFAULT '0',
'''   `quaternary_id` varchar(150) DEFAULT NULL,
'''   KEY `fk_features_rfamseq1_idx` (`rfamseq_acc`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("features", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `features` (
  `rfamseq_acc` varchar(20) NOT NULL,
  `database_id` varchar(50) NOT NULL,
  `primary_id` varchar(100) NOT NULL,
  `secondary_id` varchar(255) DEFAULT NULL,
  `feat_orient` tinyint(3) NOT NULL DEFAULT '0',
  `feat_start` bigint(19) unsigned NOT NULL DEFAULT '0',
  `feat_end` bigint(19) unsigned NOT NULL DEFAULT '0',
  `quaternary_id` varchar(150) DEFAULT NULL,
  KEY `fk_features_rfamseq1_idx` (`rfamseq_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class features: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfamseq_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property rfamseq_acc As String
    <DatabaseField("database_id"), NotNull, DataType(MySqlDbType.VarChar, "50")> Public Property database_id As String
    <DatabaseField("primary_id"), NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property primary_id As String
    <DatabaseField("secondary_id"), DataType(MySqlDbType.VarChar, "255")> Public Property secondary_id As String
    <DatabaseField("feat_orient"), NotNull, DataType(MySqlDbType.Int64, "3")> Public Property feat_orient As Long
    <DatabaseField("feat_start"), NotNull, DataType(MySqlDbType.Int64, "19")> Public Property feat_start As Long
    <DatabaseField("feat_end"), NotNull, DataType(MySqlDbType.Int64, "19")> Public Property feat_end As Long
    <DatabaseField("quaternary_id"), DataType(MySqlDbType.VarChar, "150")> Public Property quaternary_id As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `features` (`rfamseq_acc`, `database_id`, `primary_id`, `secondary_id`, `feat_orient`, `feat_start`, `feat_end`, `quaternary_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `features` (`rfamseq_acc`, `database_id`, `primary_id`, `secondary_id`, `feat_orient`, `feat_start`, `feat_end`, `quaternary_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `features` WHERE `rfamseq_acc` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `features` SET `rfamseq_acc`='{0}', `database_id`='{1}', `primary_id`='{2}', `secondary_id`='{3}', `feat_orient`='{4}', `feat_start`='{5}', `feat_end`='{6}', `quaternary_id`='{7}' WHERE `rfamseq_acc` = '{8}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `features` WHERE `rfamseq_acc` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfamseq_acc)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `features` (`rfamseq_acc`, `database_id`, `primary_id`, `secondary_id`, `feat_orient`, `feat_start`, `feat_end`, `quaternary_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfamseq_acc, database_id, primary_id, secondary_id, feat_orient, feat_start, feat_end, quaternary_id)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{rfamseq_acc}', '{database_id}', '{primary_id}', '{secondary_id}', '{feat_orient}', '{feat_start}', '{feat_end}', '{quaternary_id}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `features` (`rfamseq_acc`, `database_id`, `primary_id`, `secondary_id`, `feat_orient`, `feat_start`, `feat_end`, `quaternary_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfamseq_acc, database_id, primary_id, secondary_id, feat_orient, feat_start, feat_end, quaternary_id)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `features` SET `rfamseq_acc`='{0}', `database_id`='{1}', `primary_id`='{2}', `secondary_id`='{3}', `feat_orient`='{4}', `feat_start`='{5}', `feat_end`='{6}', `quaternary_id`='{7}' WHERE `rfamseq_acc` = '{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfamseq_acc, database_id, primary_id, secondary_id, feat_orient, feat_start, feat_end, quaternary_id, rfamseq_acc)
    End Function
#End Region
End Class


End Namespace

