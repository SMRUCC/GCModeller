#Region "Microsoft.VisualBasic::9220856cfa9c332203b95589444fb740, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\promoter.vb"

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


    ' Code Statistics:

    '   Total Lines: 182
    '    Code Lines: 95
    ' Comment Lines: 65
    '   Blank Lines: 22
    '     File Size: 12.20 KB


    ' Class promoter
    ' 
    '     Properties: basal_trans_val, equilibrium_const, key_id_org, kinetic_const, pos_1
    '                 promoter_id, promoter_internal_comment, promoter_name, promoter_note, promoter_sequence
    '                 promoter_strand, sigma_factor, strength_seq
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

REM  Dump @2018/5/23 13:13:36


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace RegulonDB.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `promoter`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `promoter` (
'''   `promoter_id` char(12) NOT NULL,
'''   `promoter_name` varchar(255) DEFAULT NULL,
'''   `promoter_strand` varchar(10) DEFAULT NULL,
'''   `pos_1` decimal(10,0) DEFAULT NULL,
'''   `sigma_factor` varchar(80) DEFAULT NULL,
'''   `basal_trans_val` decimal(20,5) DEFAULT NULL,
'''   `equilibrium_const` decimal(20,5) DEFAULT NULL,
'''   `kinetic_const` decimal(20,5) DEFAULT NULL,
'''   `strength_seq` decimal(20,5) DEFAULT NULL,
'''   `promoter_sequence` varchar(200) DEFAULT NULL,
'''   `promoter_note` varchar(4000) DEFAULT NULL,
'''   `promoter_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("promoter", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `promoter` (
  `promoter_id` char(12) NOT NULL,
  `promoter_name` varchar(255) DEFAULT NULL,
  `promoter_strand` varchar(10) DEFAULT NULL,
  `pos_1` decimal(10,0) DEFAULT NULL,
  `sigma_factor` varchar(80) DEFAULT NULL,
  `basal_trans_val` decimal(20,5) DEFAULT NULL,
  `equilibrium_const` decimal(20,5) DEFAULT NULL,
  `kinetic_const` decimal(20,5) DEFAULT NULL,
  `strength_seq` decimal(20,5) DEFAULT NULL,
  `promoter_sequence` varchar(200) DEFAULT NULL,
  `promoter_note` varchar(4000) DEFAULT NULL,
  `promoter_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class promoter: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("promoter_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="promoter_id")> Public Property promoter_id As String
    <DatabaseField("promoter_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="promoter_name")> Public Property promoter_name As String
    <DatabaseField("promoter_strand"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="promoter_strand")> Public Property promoter_strand As String
    <DatabaseField("pos_1"), DataType(MySqlDbType.Decimal), Column(Name:="pos_1")> Public Property pos_1 As Decimal
    <DatabaseField("sigma_factor"), DataType(MySqlDbType.VarChar, "80"), Column(Name:="sigma_factor")> Public Property sigma_factor As String
    <DatabaseField("basal_trans_val"), DataType(MySqlDbType.Decimal), Column(Name:="basal_trans_val")> Public Property basal_trans_val As Decimal
    <DatabaseField("equilibrium_const"), DataType(MySqlDbType.Decimal), Column(Name:="equilibrium_const")> Public Property equilibrium_const As Decimal
    <DatabaseField("kinetic_const"), DataType(MySqlDbType.Decimal), Column(Name:="kinetic_const")> Public Property kinetic_const As Decimal
    <DatabaseField("strength_seq"), DataType(MySqlDbType.Decimal), Column(Name:="strength_seq")> Public Property strength_seq As Decimal
    <DatabaseField("promoter_sequence"), DataType(MySqlDbType.VarChar, "200"), Column(Name:="promoter_sequence")> Public Property promoter_sequence As String
    <DatabaseField("promoter_note"), DataType(MySqlDbType.VarChar, "4000"), Column(Name:="promoter_note")> Public Property promoter_note As String
    <DatabaseField("promoter_internal_comment"), DataType(MySqlDbType.Text), Column(Name:="promoter_internal_comment")> Public Property promoter_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="key_id_org")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `promoter` (`promoter_id`, `promoter_name`, `promoter_strand`, `pos_1`, `sigma_factor`, `basal_trans_val`, `equilibrium_const`, `kinetic_const`, `strength_seq`, `promoter_sequence`, `promoter_note`, `promoter_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `promoter` (`promoter_id`, `promoter_name`, `promoter_strand`, `pos_1`, `sigma_factor`, `basal_trans_val`, `equilibrium_const`, `kinetic_const`, `strength_seq`, `promoter_sequence`, `promoter_note`, `promoter_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `promoter` (`promoter_id`, `promoter_name`, `promoter_strand`, `pos_1`, `sigma_factor`, `basal_trans_val`, `equilibrium_const`, `kinetic_const`, `strength_seq`, `promoter_sequence`, `promoter_note`, `promoter_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `promoter` (`promoter_id`, `promoter_name`, `promoter_strand`, `pos_1`, `sigma_factor`, `basal_trans_val`, `equilibrium_const`, `kinetic_const`, `strength_seq`, `promoter_sequence`, `promoter_note`, `promoter_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `promoter` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `promoter` SET `promoter_id`='{0}', `promoter_name`='{1}', `promoter_strand`='{2}', `pos_1`='{3}', `sigma_factor`='{4}', `basal_trans_val`='{5}', `equilibrium_const`='{6}', `kinetic_const`='{7}', `strength_seq`='{8}', `promoter_sequence`='{9}', `promoter_note`='{10}', `promoter_internal_comment`='{11}', `key_id_org`='{12}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `promoter` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `promoter` (`promoter_id`, `promoter_name`, `promoter_strand`, `pos_1`, `sigma_factor`, `basal_trans_val`, `equilibrium_const`, `kinetic_const`, `strength_seq`, `promoter_sequence`, `promoter_note`, `promoter_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, promoter_id, promoter_name, promoter_strand, pos_1, sigma_factor, basal_trans_val, equilibrium_const, kinetic_const, strength_seq, promoter_sequence, promoter_note, promoter_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `promoter` (`promoter_id`, `promoter_name`, `promoter_strand`, `pos_1`, `sigma_factor`, `basal_trans_val`, `equilibrium_const`, `kinetic_const`, `strength_seq`, `promoter_sequence`, `promoter_note`, `promoter_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, promoter_id, promoter_name, promoter_strand, pos_1, sigma_factor, basal_trans_val, equilibrium_const, kinetic_const, strength_seq, promoter_sequence, promoter_note, promoter_internal_comment, key_id_org)
        Else
        Return String.Format(INSERT_SQL, promoter_id, promoter_name, promoter_strand, pos_1, sigma_factor, basal_trans_val, equilibrium_const, kinetic_const, strength_seq, promoter_sequence, promoter_note, promoter_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{promoter_id}', '{promoter_name}', '{promoter_strand}', '{pos_1}', '{sigma_factor}', '{basal_trans_val}', '{equilibrium_const}', '{kinetic_const}', '{strength_seq}', '{promoter_sequence}', '{promoter_note}', '{promoter_internal_comment}', '{key_id_org}')"
        Else
            Return $"('{promoter_id}', '{promoter_name}', '{promoter_strand}', '{pos_1}', '{sigma_factor}', '{basal_trans_val}', '{equilibrium_const}', '{kinetic_const}', '{strength_seq}', '{promoter_sequence}', '{promoter_note}', '{promoter_internal_comment}', '{key_id_org}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `promoter` (`promoter_id`, `promoter_name`, `promoter_strand`, `pos_1`, `sigma_factor`, `basal_trans_val`, `equilibrium_const`, `kinetic_const`, `strength_seq`, `promoter_sequence`, `promoter_note`, `promoter_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, promoter_id, promoter_name, promoter_strand, pos_1, sigma_factor, basal_trans_val, equilibrium_const, kinetic_const, strength_seq, promoter_sequence, promoter_note, promoter_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `promoter` (`promoter_id`, `promoter_name`, `promoter_strand`, `pos_1`, `sigma_factor`, `basal_trans_val`, `equilibrium_const`, `kinetic_const`, `strength_seq`, `promoter_sequence`, `promoter_note`, `promoter_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, promoter_id, promoter_name, promoter_strand, pos_1, sigma_factor, basal_trans_val, equilibrium_const, kinetic_const, strength_seq, promoter_sequence, promoter_note, promoter_internal_comment, key_id_org)
        Else
        Return String.Format(REPLACE_SQL, promoter_id, promoter_name, promoter_strand, pos_1, sigma_factor, basal_trans_val, equilibrium_const, kinetic_const, strength_seq, promoter_sequence, promoter_note, promoter_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `promoter` SET `promoter_id`='{0}', `promoter_name`='{1}', `promoter_strand`='{2}', `pos_1`='{3}', `sigma_factor`='{4}', `basal_trans_val`='{5}', `equilibrium_const`='{6}', `kinetic_const`='{7}', `strength_seq`='{8}', `promoter_sequence`='{9}', `promoter_note`='{10}', `promoter_internal_comment`='{11}', `key_id_org`='{12}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As promoter
                         Return DirectCast(MyClass.MemberwiseClone, promoter)
                     End Function
End Class


End Namespace
