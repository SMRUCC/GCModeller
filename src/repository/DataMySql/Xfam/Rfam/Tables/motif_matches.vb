#Region "Microsoft.VisualBasic::5acde95d172948b6dd74d06f9c1e990b, DataMySql\Xfam\Rfam\Tables\motif_matches.vb"

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

    ' Class motif_matches
    ' 
    '     Properties: bit_score, e_value, motif_acc, motif_start, motif_stop
    '                 query_start, query_stop, rfam_acc, rfamseq_acc, rfamseq_start
    '                 rfamseq_stop
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
''' DROP TABLE IF EXISTS `motif_matches`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `motif_matches` (
'''   `motif_acc` varchar(7) NOT NULL,
'''   `rfam_acc` varchar(7) NOT NULL,
'''   `rfamseq_acc` varchar(20) NOT NULL,
'''   `rfamseq_start` bigint(19) DEFAULT NULL,
'''   `rfamseq_stop` bigint(19) DEFAULT NULL,
'''   `query_start` int(11) DEFAULT NULL,
'''   `query_stop` int(11) DEFAULT NULL,
'''   `motif_start` int(11) DEFAULT NULL,
'''   `motif_stop` int(11) DEFAULT NULL,
'''   `e_value` varchar(15) DEFAULT NULL,
'''   `bit_score` double(7,2) DEFAULT NULL,
'''   KEY `motif_match_motif_acc_idx` (`motif_acc`),
'''   KEY `motif_match_rfam_acc_idx` (`rfam_acc`),
'''   KEY `motif_match_rfamseq_acc_idx` (`rfamseq_acc`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("motif_matches", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `motif_matches` (
  `motif_acc` varchar(7) NOT NULL,
  `rfam_acc` varchar(7) NOT NULL,
  `rfamseq_acc` varchar(20) NOT NULL,
  `rfamseq_start` bigint(19) DEFAULT NULL,
  `rfamseq_stop` bigint(19) DEFAULT NULL,
  `query_start` int(11) DEFAULT NULL,
  `query_stop` int(11) DEFAULT NULL,
  `motif_start` int(11) DEFAULT NULL,
  `motif_stop` int(11) DEFAULT NULL,
  `e_value` varchar(15) DEFAULT NULL,
  `bit_score` double(7,2) DEFAULT NULL,
  KEY `motif_match_motif_acc_idx` (`motif_acc`),
  KEY `motif_match_rfam_acc_idx` (`rfam_acc`),
  KEY `motif_match_rfamseq_acc_idx` (`rfamseq_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class motif_matches: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("motif_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="motif_acc"), XmlAttribute> Public Property motif_acc As String
    <DatabaseField("rfam_acc"), NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="rfam_acc")> Public Property rfam_acc As String
    <DatabaseField("rfamseq_acc"), NotNull, DataType(MySqlDbType.VarChar, "20"), Column(Name:="rfamseq_acc")> Public Property rfamseq_acc As String
    <DatabaseField("rfamseq_start"), DataType(MySqlDbType.Int64, "19"), Column(Name:="rfamseq_start")> Public Property rfamseq_start As Long
    <DatabaseField("rfamseq_stop"), DataType(MySqlDbType.Int64, "19"), Column(Name:="rfamseq_stop")> Public Property rfamseq_stop As Long
    <DatabaseField("query_start"), DataType(MySqlDbType.Int64, "11"), Column(Name:="query_start")> Public Property query_start As Long
    <DatabaseField("query_stop"), DataType(MySqlDbType.Int64, "11"), Column(Name:="query_stop")> Public Property query_stop As Long
    <DatabaseField("motif_start"), DataType(MySqlDbType.Int64, "11"), Column(Name:="motif_start")> Public Property motif_start As Long
    <DatabaseField("motif_stop"), DataType(MySqlDbType.Int64, "11"), Column(Name:="motif_stop")> Public Property motif_stop As Long
    <DatabaseField("e_value"), DataType(MySqlDbType.VarChar, "15"), Column(Name:="e_value")> Public Property e_value As String
    <DatabaseField("bit_score"), DataType(MySqlDbType.Double), Column(Name:="bit_score")> Public Property bit_score As Double
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `motif_matches` (`motif_acc`, `rfam_acc`, `rfamseq_acc`, `rfamseq_start`, `rfamseq_stop`, `query_start`, `query_stop`, `motif_start`, `motif_stop`, `e_value`, `bit_score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `motif_matches` (`motif_acc`, `rfam_acc`, `rfamseq_acc`, `rfamseq_start`, `rfamseq_stop`, `query_start`, `query_stop`, `motif_start`, `motif_stop`, `e_value`, `bit_score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `motif_matches` (`motif_acc`, `rfam_acc`, `rfamseq_acc`, `rfamseq_start`, `rfamseq_stop`, `query_start`, `query_stop`, `motif_start`, `motif_stop`, `e_value`, `bit_score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `motif_matches` (`motif_acc`, `rfam_acc`, `rfamseq_acc`, `rfamseq_start`, `rfamseq_stop`, `query_start`, `query_stop`, `motif_start`, `motif_stop`, `e_value`, `bit_score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `motif_matches` WHERE `motif_acc` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `motif_matches` SET `motif_acc`='{0}', `rfam_acc`='{1}', `rfamseq_acc`='{2}', `rfamseq_start`='{3}', `rfamseq_stop`='{4}', `query_start`='{5}', `query_stop`='{6}', `motif_start`='{7}', `motif_stop`='{8}', `e_value`='{9}', `bit_score`='{10}' WHERE `motif_acc` = '{11}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `motif_matches` WHERE `motif_acc` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, motif_acc)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `motif_matches` (`motif_acc`, `rfam_acc`, `rfamseq_acc`, `rfamseq_start`, `rfamseq_stop`, `query_start`, `query_stop`, `motif_start`, `motif_stop`, `e_value`, `bit_score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, motif_acc, rfam_acc, rfamseq_acc, rfamseq_start, rfamseq_stop, query_start, query_stop, motif_start, motif_stop, e_value, bit_score)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `motif_matches` (`motif_acc`, `rfam_acc`, `rfamseq_acc`, `rfamseq_start`, `rfamseq_stop`, `query_start`, `query_stop`, `motif_start`, `motif_stop`, `e_value`, `bit_score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, motif_acc, rfam_acc, rfamseq_acc, rfamseq_start, rfamseq_stop, query_start, query_stop, motif_start, motif_stop, e_value, bit_score)
        Else
        Return String.Format(INSERT_SQL, motif_acc, rfam_acc, rfamseq_acc, rfamseq_start, rfamseq_stop, query_start, query_stop, motif_start, motif_stop, e_value, bit_score)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{motif_acc}', '{rfam_acc}', '{rfamseq_acc}', '{rfamseq_start}', '{rfamseq_stop}', '{query_start}', '{query_stop}', '{motif_start}', '{motif_stop}', '{e_value}', '{bit_score}')"
        Else
            Return $"('{motif_acc}', '{rfam_acc}', '{rfamseq_acc}', '{rfamseq_start}', '{rfamseq_stop}', '{query_start}', '{query_stop}', '{motif_start}', '{motif_stop}', '{e_value}', '{bit_score}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `motif_matches` (`motif_acc`, `rfam_acc`, `rfamseq_acc`, `rfamseq_start`, `rfamseq_stop`, `query_start`, `query_stop`, `motif_start`, `motif_stop`, `e_value`, `bit_score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, motif_acc, rfam_acc, rfamseq_acc, rfamseq_start, rfamseq_stop, query_start, query_stop, motif_start, motif_stop, e_value, bit_score)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `motif_matches` (`motif_acc`, `rfam_acc`, `rfamseq_acc`, `rfamseq_start`, `rfamseq_stop`, `query_start`, `query_stop`, `motif_start`, `motif_stop`, `e_value`, `bit_score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, motif_acc, rfam_acc, rfamseq_acc, rfamseq_start, rfamseq_stop, query_start, query_stop, motif_start, motif_stop, e_value, bit_score)
        Else
        Return String.Format(REPLACE_SQL, motif_acc, rfam_acc, rfamseq_acc, rfamseq_start, rfamseq_stop, query_start, query_stop, motif_start, motif_stop, e_value, bit_score)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `motif_matches` SET `motif_acc`='{0}', `rfam_acc`='{1}', `rfamseq_acc`='{2}', `rfamseq_start`='{3}', `rfamseq_stop`='{4}', `query_start`='{5}', `query_stop`='{6}', `motif_start`='{7}', `motif_stop`='{8}', `e_value`='{9}', `bit_score`='{10}' WHERE `motif_acc` = '{11}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, motif_acc, rfam_acc, rfamseq_acc, rfamseq_start, rfamseq_stop, query_start, query_stop, motif_start, motif_stop, e_value, bit_score, motif_acc)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As motif_matches
                         Return DirectCast(MyClass.MemberwiseClone, motif_matches)
                     End Function
End Class


End Namespace
