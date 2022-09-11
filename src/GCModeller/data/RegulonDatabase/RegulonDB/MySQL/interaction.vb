#Region "Microsoft.VisualBasic::c09f8c606d9039b4293c865a3d57f396, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\interaction.vb"

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

    '   Total Lines: 179
    '    Code Lines: 93
    ' Comment Lines: 64
    '   Blank Lines: 22
    '     File Size: 12.21 KB


    ' Class interaction
    ' 
    '     Properties: affinity_exp, center_position, interaction_first_gene_id, interaction_function, interaction_id
    '                 interaction_internal_comment, interaction_note, interaction_sequence, key_id_org, promoter_id
    '                 regulator_id, site_id
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
''' DROP TABLE IF EXISTS `interaction`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `interaction` (
'''   `interaction_id` varchar(12) NOT NULL,
'''   `regulator_id` varchar(12) DEFAULT NULL,
'''   `promoter_id` char(12) DEFAULT NULL,
'''   `site_id` char(12) DEFAULT NULL,
'''   `interaction_function` varchar(12) DEFAULT NULL,
'''   `center_position` decimal(20,2) DEFAULT NULL,
'''   `interaction_first_gene_id` varchar(12) DEFAULT NULL,
'''   `affinity_exp` decimal(20,5) DEFAULT NULL,
'''   `interaction_note` varchar(2000) DEFAULT NULL,
'''   `interaction_internal_comment` longtext,
'''   `interaction_sequence` varchar(100) DEFAULT NULL,
'''   `key_id_org` varchar(5) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("interaction", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `interaction` (
  `interaction_id` varchar(12) NOT NULL,
  `regulator_id` varchar(12) DEFAULT NULL,
  `promoter_id` char(12) DEFAULT NULL,
  `site_id` char(12) DEFAULT NULL,
  `interaction_function` varchar(12) DEFAULT NULL,
  `center_position` decimal(20,2) DEFAULT NULL,
  `interaction_first_gene_id` varchar(12) DEFAULT NULL,
  `affinity_exp` decimal(20,5) DEFAULT NULL,
  `interaction_note` varchar(2000) DEFAULT NULL,
  `interaction_internal_comment` longtext,
  `interaction_sequence` varchar(100) DEFAULT NULL,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class interaction: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("interaction_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="interaction_id")> Public Property interaction_id As String
    <DatabaseField("regulator_id"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="regulator_id")> Public Property regulator_id As String
    <DatabaseField("promoter_id"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="promoter_id")> Public Property promoter_id As String
    <DatabaseField("site_id"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="site_id")> Public Property site_id As String
    <DatabaseField("interaction_function"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="interaction_function")> Public Property interaction_function As String
    <DatabaseField("center_position"), DataType(MySqlDbType.Decimal), Column(Name:="center_position")> Public Property center_position As Decimal
    <DatabaseField("interaction_first_gene_id"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="interaction_first_gene_id")> Public Property interaction_first_gene_id As String
    <DatabaseField("affinity_exp"), DataType(MySqlDbType.Decimal), Column(Name:="affinity_exp")> Public Property affinity_exp As Decimal
    <DatabaseField("interaction_note"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="interaction_note")> Public Property interaction_note As String
    <DatabaseField("interaction_internal_comment"), DataType(MySqlDbType.Text), Column(Name:="interaction_internal_comment")> Public Property interaction_internal_comment As String
    <DatabaseField("interaction_sequence"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="interaction_sequence")> Public Property interaction_sequence As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="key_id_org")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `interaction` (`interaction_id`, `regulator_id`, `promoter_id`, `site_id`, `interaction_function`, `center_position`, `interaction_first_gene_id`, `affinity_exp`, `interaction_note`, `interaction_internal_comment`, `interaction_sequence`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `interaction` (`interaction_id`, `regulator_id`, `promoter_id`, `site_id`, `interaction_function`, `center_position`, `interaction_first_gene_id`, `affinity_exp`, `interaction_note`, `interaction_internal_comment`, `interaction_sequence`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `interaction` (`interaction_id`, `regulator_id`, `promoter_id`, `site_id`, `interaction_function`, `center_position`, `interaction_first_gene_id`, `affinity_exp`, `interaction_note`, `interaction_internal_comment`, `interaction_sequence`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `interaction` (`interaction_id`, `regulator_id`, `promoter_id`, `site_id`, `interaction_function`, `center_position`, `interaction_first_gene_id`, `affinity_exp`, `interaction_note`, `interaction_internal_comment`, `interaction_sequence`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `interaction` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `interaction` SET `interaction_id`='{0}', `regulator_id`='{1}', `promoter_id`='{2}', `site_id`='{3}', `interaction_function`='{4}', `center_position`='{5}', `interaction_first_gene_id`='{6}', `affinity_exp`='{7}', `interaction_note`='{8}', `interaction_internal_comment`='{9}', `interaction_sequence`='{10}', `key_id_org`='{11}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `interaction` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `interaction` (`interaction_id`, `regulator_id`, `promoter_id`, `site_id`, `interaction_function`, `center_position`, `interaction_first_gene_id`, `affinity_exp`, `interaction_note`, `interaction_internal_comment`, `interaction_sequence`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, interaction_id, regulator_id, promoter_id, site_id, interaction_function, center_position, interaction_first_gene_id, affinity_exp, interaction_note, interaction_internal_comment, interaction_sequence, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `interaction` (`interaction_id`, `regulator_id`, `promoter_id`, `site_id`, `interaction_function`, `center_position`, `interaction_first_gene_id`, `affinity_exp`, `interaction_note`, `interaction_internal_comment`, `interaction_sequence`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, interaction_id, regulator_id, promoter_id, site_id, interaction_function, center_position, interaction_first_gene_id, affinity_exp, interaction_note, interaction_internal_comment, interaction_sequence, key_id_org)
        Else
        Return String.Format(INSERT_SQL, interaction_id, regulator_id, promoter_id, site_id, interaction_function, center_position, interaction_first_gene_id, affinity_exp, interaction_note, interaction_internal_comment, interaction_sequence, key_id_org)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{interaction_id}', '{regulator_id}', '{promoter_id}', '{site_id}', '{interaction_function}', '{center_position}', '{interaction_first_gene_id}', '{affinity_exp}', '{interaction_note}', '{interaction_internal_comment}', '{interaction_sequence}', '{key_id_org}')"
        Else
            Return $"('{interaction_id}', '{regulator_id}', '{promoter_id}', '{site_id}', '{interaction_function}', '{center_position}', '{interaction_first_gene_id}', '{affinity_exp}', '{interaction_note}', '{interaction_internal_comment}', '{interaction_sequence}', '{key_id_org}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `interaction` (`interaction_id`, `regulator_id`, `promoter_id`, `site_id`, `interaction_function`, `center_position`, `interaction_first_gene_id`, `affinity_exp`, `interaction_note`, `interaction_internal_comment`, `interaction_sequence`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, interaction_id, regulator_id, promoter_id, site_id, interaction_function, center_position, interaction_first_gene_id, affinity_exp, interaction_note, interaction_internal_comment, interaction_sequence, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `interaction` (`interaction_id`, `regulator_id`, `promoter_id`, `site_id`, `interaction_function`, `center_position`, `interaction_first_gene_id`, `affinity_exp`, `interaction_note`, `interaction_internal_comment`, `interaction_sequence`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, interaction_id, regulator_id, promoter_id, site_id, interaction_function, center_position, interaction_first_gene_id, affinity_exp, interaction_note, interaction_internal_comment, interaction_sequence, key_id_org)
        Else
        Return String.Format(REPLACE_SQL, interaction_id, regulator_id, promoter_id, site_id, interaction_function, center_position, interaction_first_gene_id, affinity_exp, interaction_note, interaction_internal_comment, interaction_sequence, key_id_org)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `interaction` SET `interaction_id`='{0}', `regulator_id`='{1}', `promoter_id`='{2}', `site_id`='{3}', `interaction_function`='{4}', `center_position`='{5}', `interaction_first_gene_id`='{6}', `affinity_exp`='{7}', `interaction_note`='{8}', `interaction_internal_comment`='{9}', `interaction_sequence`='{10}', `key_id_org`='{11}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As interaction
                         Return DirectCast(MyClass.MemberwiseClone, interaction)
                     End Function
End Class


End Namespace
