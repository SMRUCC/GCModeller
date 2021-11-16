#Region "Microsoft.VisualBasic::ad54024de4d5da87a8ae2e890ea0c1c7, DataMySql\Xfam\Rfam\Tables\motif_pdb.vb"

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

    ' Class motif_pdb
    ' 
    '     Properties: chain, motif_acc, pdb_end, pdb_id, pdb_start
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
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class motif_pdb: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("motif_acc"), NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="motif_acc")> Public Property motif_acc As String
    <DatabaseField("pdb_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "4"), Column(Name:="pdb_id"), XmlAttribute> Public Property pdb_id As String
    <DatabaseField("chain"), DataType(MySqlDbType.VarChar, "4"), Column(Name:="chain")> Public Property chain As String
    <DatabaseField("pdb_start"), DataType(MySqlDbType.Int64, "9"), Column(Name:="pdb_start")> Public Property pdb_start As Long
    <DatabaseField("pdb_end"), DataType(MySqlDbType.Int64, "9"), Column(Name:="pdb_end")> Public Property pdb_end As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `motif_pdb` (`motif_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `motif_pdb` (`motif_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `motif_pdb` (`motif_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `motif_pdb` (`motif_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `motif_pdb` WHERE `pdb_id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `motif_pdb` SET `motif_acc`='{0}', `pdb_id`='{1}', `chain`='{2}', `pdb_start`='{3}', `pdb_end`='{4}' WHERE `pdb_id` = '{5}';</SQL>

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
''' ```SQL
''' INSERT INTO `motif_pdb` (`motif_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, motif_acc, pdb_id, chain, pdb_start, pdb_end)
        Else
        Return String.Format(INSERT_SQL, motif_acc, pdb_id, chain, pdb_start, pdb_end)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{motif_acc}', '{pdb_id}', '{chain}', '{pdb_start}', '{pdb_end}')"
        Else
            Return $"('{motif_acc}', '{pdb_id}', '{chain}', '{pdb_start}', '{pdb_end}')"
        End If
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
''' REPLACE INTO `motif_pdb` (`motif_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, motif_acc, pdb_id, chain, pdb_start, pdb_end)
        Else
        Return String.Format(REPLACE_SQL, motif_acc, pdb_id, chain, pdb_start, pdb_end)
        End If
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

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As motif_pdb
                         Return DirectCast(MyClass.MemberwiseClone, motif_pdb)
                     End Function
End Class


End Namespace
