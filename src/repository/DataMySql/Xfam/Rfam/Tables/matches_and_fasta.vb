#Region "Microsoft.VisualBasic::06378893f2e358140e6b678c5acd7210, DataMySql\Xfam\Rfam\Tables\matches_and_fasta.vb"

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

    ' Class matches_and_fasta
    ' 
    '     Properties: fasta, match_list, rfam_acc, type
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
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class matches_and_fasta: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="rfam_acc"), XmlAttribute> Public Property rfam_acc As String
    <DatabaseField("match_list"), DataType(MySqlDbType.Blob), Column(Name:="match_list")> Public Property match_list As Byte()
    <DatabaseField("fasta"), DataType(MySqlDbType.Blob), Column(Name:="fasta")> Public Property fasta As Byte()
    <DatabaseField("type"), NotNull, DataType(MySqlDbType.String), Column(Name:="type")> Public Property type As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `matches_and_fasta` (`rfam_acc`, `match_list`, `fasta`, `type`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `matches_and_fasta` (`rfam_acc`, `match_list`, `fasta`, `type`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `matches_and_fasta` (`rfam_acc`, `match_list`, `fasta`, `type`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `matches_and_fasta` (`rfam_acc`, `match_list`, `fasta`, `type`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `matches_and_fasta` WHERE `rfam_acc` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `matches_and_fasta` SET `rfam_acc`='{0}', `match_list`='{1}', `fasta`='{2}', `type`='{3}' WHERE `rfam_acc` = '{4}';</SQL>

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
''' ```SQL
''' INSERT INTO `matches_and_fasta` (`rfam_acc`, `match_list`, `fasta`, `type`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, rfam_acc, match_list, fasta, type)
        Else
        Return String.Format(INSERT_SQL, rfam_acc, match_list, fasta, type)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{rfam_acc}', '{match_list}', '{fasta}', '{type}')"
        Else
            Return $"('{rfam_acc}', '{match_list}', '{fasta}', '{type}')"
        End If
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
''' REPLACE INTO `matches_and_fasta` (`rfam_acc`, `match_list`, `fasta`, `type`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, rfam_acc, match_list, fasta, type)
        Else
        Return String.Format(REPLACE_SQL, rfam_acc, match_list, fasta, type)
        End If
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

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As matches_and_fasta
                         Return DirectCast(MyClass.MemberwiseClone, matches_and_fasta)
                     End Function
End Class


End Namespace
