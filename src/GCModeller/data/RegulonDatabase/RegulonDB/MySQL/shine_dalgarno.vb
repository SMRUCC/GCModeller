#Region "Microsoft.VisualBasic::1ba33fb98f461f5c336157d33883c1f6, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\shine_dalgarno.vb"

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

    '   Total Lines: 170
    '    Code Lines: 87
    ' Comment Lines: 61
    '   Blank Lines: 22
    '     File Size: 10.73 KB


    ' Class shine_dalgarno
    ' 
    '     Properties: gene_id, key_id_org, sd_internal_comment, shine_dalgarno_dist_gene, shine_dalgarno_id
    '                 shine_dalgarno_note, shine_dalgarno_posleft, shine_dalgarno_posright, shine_dalgarno_sequence
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
''' DROP TABLE IF EXISTS `shine_dalgarno`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `shine_dalgarno` (
'''   `shine_dalgarno_id` char(12) NOT NULL,
'''   `gene_id` char(12) NOT NULL,
'''   `shine_dalgarno_dist_gene` decimal(10,0) NOT NULL,
'''   `shine_dalgarno_posleft` decimal(10,0) DEFAULT NULL,
'''   `shine_dalgarno_posright` decimal(10,0) DEFAULT NULL,
'''   `shine_dalgarno_sequence` varchar(500) DEFAULT NULL,
'''   `shine_dalgarno_note` varchar(2000) DEFAULT NULL,
'''   `sd_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("shine_dalgarno", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `shine_dalgarno` (
  `shine_dalgarno_id` char(12) NOT NULL,
  `gene_id` char(12) NOT NULL,
  `shine_dalgarno_dist_gene` decimal(10,0) NOT NULL,
  `shine_dalgarno_posleft` decimal(10,0) DEFAULT NULL,
  `shine_dalgarno_posright` decimal(10,0) DEFAULT NULL,
  `shine_dalgarno_sequence` varchar(500) DEFAULT NULL,
  `shine_dalgarno_note` varchar(2000) DEFAULT NULL,
  `sd_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class shine_dalgarno: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("shine_dalgarno_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="shine_dalgarno_id")> Public Property shine_dalgarno_id As String
    <DatabaseField("gene_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="gene_id")> Public Property gene_id As String
    <DatabaseField("shine_dalgarno_dist_gene"), NotNull, DataType(MySqlDbType.Decimal), Column(Name:="shine_dalgarno_dist_gene")> Public Property shine_dalgarno_dist_gene As Decimal
    <DatabaseField("shine_dalgarno_posleft"), DataType(MySqlDbType.Decimal), Column(Name:="shine_dalgarno_posleft")> Public Property shine_dalgarno_posleft As Decimal
    <DatabaseField("shine_dalgarno_posright"), DataType(MySqlDbType.Decimal), Column(Name:="shine_dalgarno_posright")> Public Property shine_dalgarno_posright As Decimal
    <DatabaseField("shine_dalgarno_sequence"), DataType(MySqlDbType.VarChar, "500"), Column(Name:="shine_dalgarno_sequence")> Public Property shine_dalgarno_sequence As String
    <DatabaseField("shine_dalgarno_note"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="shine_dalgarno_note")> Public Property shine_dalgarno_note As String
    <DatabaseField("sd_internal_comment"), DataType(MySqlDbType.Text), Column(Name:="sd_internal_comment")> Public Property sd_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="key_id_org")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `shine_dalgarno` (`shine_dalgarno_id`, `gene_id`, `shine_dalgarno_dist_gene`, `shine_dalgarno_posleft`, `shine_dalgarno_posright`, `shine_dalgarno_sequence`, `shine_dalgarno_note`, `sd_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `shine_dalgarno` (`shine_dalgarno_id`, `gene_id`, `shine_dalgarno_dist_gene`, `shine_dalgarno_posleft`, `shine_dalgarno_posright`, `shine_dalgarno_sequence`, `shine_dalgarno_note`, `sd_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `shine_dalgarno` (`shine_dalgarno_id`, `gene_id`, `shine_dalgarno_dist_gene`, `shine_dalgarno_posleft`, `shine_dalgarno_posright`, `shine_dalgarno_sequence`, `shine_dalgarno_note`, `sd_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `shine_dalgarno` (`shine_dalgarno_id`, `gene_id`, `shine_dalgarno_dist_gene`, `shine_dalgarno_posleft`, `shine_dalgarno_posright`, `shine_dalgarno_sequence`, `shine_dalgarno_note`, `sd_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `shine_dalgarno` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `shine_dalgarno` SET `shine_dalgarno_id`='{0}', `gene_id`='{1}', `shine_dalgarno_dist_gene`='{2}', `shine_dalgarno_posleft`='{3}', `shine_dalgarno_posright`='{4}', `shine_dalgarno_sequence`='{5}', `shine_dalgarno_note`='{6}', `sd_internal_comment`='{7}', `key_id_org`='{8}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `shine_dalgarno` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `shine_dalgarno` (`shine_dalgarno_id`, `gene_id`, `shine_dalgarno_dist_gene`, `shine_dalgarno_posleft`, `shine_dalgarno_posright`, `shine_dalgarno_sequence`, `shine_dalgarno_note`, `sd_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, shine_dalgarno_id, gene_id, shine_dalgarno_dist_gene, shine_dalgarno_posleft, shine_dalgarno_posright, shine_dalgarno_sequence, shine_dalgarno_note, sd_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `shine_dalgarno` (`shine_dalgarno_id`, `gene_id`, `shine_dalgarno_dist_gene`, `shine_dalgarno_posleft`, `shine_dalgarno_posright`, `shine_dalgarno_sequence`, `shine_dalgarno_note`, `sd_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, shine_dalgarno_id, gene_id, shine_dalgarno_dist_gene, shine_dalgarno_posleft, shine_dalgarno_posright, shine_dalgarno_sequence, shine_dalgarno_note, sd_internal_comment, key_id_org)
        Else
        Return String.Format(INSERT_SQL, shine_dalgarno_id, gene_id, shine_dalgarno_dist_gene, shine_dalgarno_posleft, shine_dalgarno_posright, shine_dalgarno_sequence, shine_dalgarno_note, sd_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{shine_dalgarno_id}', '{gene_id}', '{shine_dalgarno_dist_gene}', '{shine_dalgarno_posleft}', '{shine_dalgarno_posright}', '{shine_dalgarno_sequence}', '{shine_dalgarno_note}', '{sd_internal_comment}', '{key_id_org}')"
        Else
            Return $"('{shine_dalgarno_id}', '{gene_id}', '{shine_dalgarno_dist_gene}', '{shine_dalgarno_posleft}', '{shine_dalgarno_posright}', '{shine_dalgarno_sequence}', '{shine_dalgarno_note}', '{sd_internal_comment}', '{key_id_org}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `shine_dalgarno` (`shine_dalgarno_id`, `gene_id`, `shine_dalgarno_dist_gene`, `shine_dalgarno_posleft`, `shine_dalgarno_posright`, `shine_dalgarno_sequence`, `shine_dalgarno_note`, `sd_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, shine_dalgarno_id, gene_id, shine_dalgarno_dist_gene, shine_dalgarno_posleft, shine_dalgarno_posright, shine_dalgarno_sequence, shine_dalgarno_note, sd_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `shine_dalgarno` (`shine_dalgarno_id`, `gene_id`, `shine_dalgarno_dist_gene`, `shine_dalgarno_posleft`, `shine_dalgarno_posright`, `shine_dalgarno_sequence`, `shine_dalgarno_note`, `sd_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, shine_dalgarno_id, gene_id, shine_dalgarno_dist_gene, shine_dalgarno_posleft, shine_dalgarno_posright, shine_dalgarno_sequence, shine_dalgarno_note, sd_internal_comment, key_id_org)
        Else
        Return String.Format(REPLACE_SQL, shine_dalgarno_id, gene_id, shine_dalgarno_dist_gene, shine_dalgarno_posleft, shine_dalgarno_posright, shine_dalgarno_sequence, shine_dalgarno_note, sd_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `shine_dalgarno` SET `shine_dalgarno_id`='{0}', `gene_id`='{1}', `shine_dalgarno_dist_gene`='{2}', `shine_dalgarno_posleft`='{3}', `shine_dalgarno_posright`='{4}', `shine_dalgarno_sequence`='{5}', `shine_dalgarno_note`='{6}', `sd_internal_comment`='{7}', `key_id_org`='{8}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As shine_dalgarno
                         Return DirectCast(MyClass.MemberwiseClone, shine_dalgarno)
                     End Function
End Class


End Namespace
