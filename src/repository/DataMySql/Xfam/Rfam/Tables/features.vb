#Region "Microsoft.VisualBasic::750cbe99a85cf7c32f408764a0dd27fa, DataMySql\Xfam\Rfam\Tables\features.vb"

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

    ' Class features
    ' 
    '     Properties: database_id, feat_end, feat_orient, feat_start, primary_id
    '                 quaternary_id, rfamseq_acc, secondary_id
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
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class features: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfamseq_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "20"), Column(Name:="rfamseq_acc"), XmlAttribute> Public Property rfamseq_acc As String
    <DatabaseField("database_id"), NotNull, DataType(MySqlDbType.VarChar, "50"), Column(Name:="database_id")> Public Property database_id As String
    <DatabaseField("primary_id"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="primary_id")> Public Property primary_id As String
    <DatabaseField("secondary_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="secondary_id")> Public Property secondary_id As String
    <DatabaseField("feat_orient"), NotNull, DataType(MySqlDbType.Int32, "3"), Column(Name:="feat_orient")> Public Property feat_orient As Integer
    <DatabaseField("feat_start"), NotNull, DataType(MySqlDbType.Int64, "19"), Column(Name:="feat_start")> Public Property feat_start As Long
    <DatabaseField("feat_end"), NotNull, DataType(MySqlDbType.Int64, "19"), Column(Name:="feat_end")> Public Property feat_end As Long
    <DatabaseField("quaternary_id"), DataType(MySqlDbType.VarChar, "150"), Column(Name:="quaternary_id")> Public Property quaternary_id As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `features` (`rfamseq_acc`, `database_id`, `primary_id`, `secondary_id`, `feat_orient`, `feat_start`, `feat_end`, `quaternary_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `features` (`rfamseq_acc`, `database_id`, `primary_id`, `secondary_id`, `feat_orient`, `feat_start`, `feat_end`, `quaternary_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `features` (`rfamseq_acc`, `database_id`, `primary_id`, `secondary_id`, `feat_orient`, `feat_start`, `feat_end`, `quaternary_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `features` (`rfamseq_acc`, `database_id`, `primary_id`, `secondary_id`, `feat_orient`, `feat_start`, `feat_end`, `quaternary_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `features` WHERE `rfamseq_acc` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `features` SET `rfamseq_acc`='{0}', `database_id`='{1}', `primary_id`='{2}', `secondary_id`='{3}', `feat_orient`='{4}', `feat_start`='{5}', `feat_end`='{6}', `quaternary_id`='{7}' WHERE `rfamseq_acc` = '{8}';</SQL>

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
''' ```SQL
''' INSERT INTO `features` (`rfamseq_acc`, `database_id`, `primary_id`, `secondary_id`, `feat_orient`, `feat_start`, `feat_end`, `quaternary_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, rfamseq_acc, database_id, primary_id, secondary_id, feat_orient, feat_start, feat_end, quaternary_id)
        Else
        Return String.Format(INSERT_SQL, rfamseq_acc, database_id, primary_id, secondary_id, feat_orient, feat_start, feat_end, quaternary_id)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{rfamseq_acc}', '{database_id}', '{primary_id}', '{secondary_id}', '{feat_orient}', '{feat_start}', '{feat_end}', '{quaternary_id}')"
        Else
            Return $"('{rfamseq_acc}', '{database_id}', '{primary_id}', '{secondary_id}', '{feat_orient}', '{feat_start}', '{feat_end}', '{quaternary_id}')"
        End If
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
''' REPLACE INTO `features` (`rfamseq_acc`, `database_id`, `primary_id`, `secondary_id`, `feat_orient`, `feat_start`, `feat_end`, `quaternary_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, rfamseq_acc, database_id, primary_id, secondary_id, feat_orient, feat_start, feat_end, quaternary_id)
        Else
        Return String.Format(REPLACE_SQL, rfamseq_acc, database_id, primary_id, secondary_id, feat_orient, feat_start, feat_end, quaternary_id)
        End If
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

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As features
                         Return DirectCast(MyClass.MemberwiseClone, features)
                     End Function
End Class


End Namespace
