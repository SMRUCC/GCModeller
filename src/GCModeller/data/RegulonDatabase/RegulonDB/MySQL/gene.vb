#Region "Microsoft.VisualBasic::166beff613bbda68fed734de6445b705, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\gene.vb"

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
    '     File Size: 10.73 KB


    ' Class gene
    ' 
    '     Properties: cri_score, gc_content, gene_id, gene_internal_comment, gene_name
    '                 gene_note, gene_posleft, gene_posright, gene_sequence, gene_strand
    '                 gene_type, key_id_org
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
''' DROP TABLE IF EXISTS `gene`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `gene` (
'''   `gene_id` char(12) NOT NULL,
'''   `gene_name` varchar(255) DEFAULT NULL,
'''   `gene_posleft` decimal(10,0) DEFAULT NULL,
'''   `gene_posright` decimal(10,0) DEFAULT NULL,
'''   `gene_strand` varchar(10) DEFAULT NULL,
'''   `gene_sequence` longtext,
'''   `gc_content` decimal(15,10) DEFAULT NULL,
'''   `cri_score` decimal(15,10) DEFAULT NULL,
'''   `gene_note` varchar(2000) DEFAULT NULL,
'''   `gene_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL,
'''   `gene_type` varchar(100) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("gene", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `gene` (
  `gene_id` char(12) NOT NULL,
  `gene_name` varchar(255) DEFAULT NULL,
  `gene_posleft` decimal(10,0) DEFAULT NULL,
  `gene_posright` decimal(10,0) DEFAULT NULL,
  `gene_strand` varchar(10) DEFAULT NULL,
  `gene_sequence` longtext,
  `gc_content` decimal(15,10) DEFAULT NULL,
  `cri_score` decimal(15,10) DEFAULT NULL,
  `gene_note` varchar(2000) DEFAULT NULL,
  `gene_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL,
  `gene_type` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class gene: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("gene_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="gene_id")> Public Property gene_id As String
    <DatabaseField("gene_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="gene_name")> Public Property gene_name As String
    <DatabaseField("gene_posleft"), DataType(MySqlDbType.Decimal), Column(Name:="gene_posleft")> Public Property gene_posleft As Decimal
    <DatabaseField("gene_posright"), DataType(MySqlDbType.Decimal), Column(Name:="gene_posright")> Public Property gene_posright As Decimal
    <DatabaseField("gene_strand"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="gene_strand")> Public Property gene_strand As String
    <DatabaseField("gene_sequence"), DataType(MySqlDbType.Text), Column(Name:="gene_sequence")> Public Property gene_sequence As String
    <DatabaseField("gc_content"), DataType(MySqlDbType.Decimal), Column(Name:="gc_content")> Public Property gc_content As Decimal
    <DatabaseField("cri_score"), DataType(MySqlDbType.Decimal), Column(Name:="cri_score")> Public Property cri_score As Decimal
    <DatabaseField("gene_note"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="gene_note")> Public Property gene_note As String
    <DatabaseField("gene_internal_comment"), DataType(MySqlDbType.Text), Column(Name:="gene_internal_comment")> Public Property gene_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="key_id_org")> Public Property key_id_org As String
    <DatabaseField("gene_type"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="gene_type")> Public Property gene_type As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `gene` (`gene_id`, `gene_name`, `gene_posleft`, `gene_posright`, `gene_strand`, `gene_sequence`, `gc_content`, `cri_score`, `gene_note`, `gene_internal_comment`, `key_id_org`, `gene_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `gene` (`gene_id`, `gene_name`, `gene_posleft`, `gene_posright`, `gene_strand`, `gene_sequence`, `gc_content`, `cri_score`, `gene_note`, `gene_internal_comment`, `key_id_org`, `gene_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `gene` (`gene_id`, `gene_name`, `gene_posleft`, `gene_posright`, `gene_strand`, `gene_sequence`, `gc_content`, `cri_score`, `gene_note`, `gene_internal_comment`, `key_id_org`, `gene_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `gene` (`gene_id`, `gene_name`, `gene_posleft`, `gene_posright`, `gene_strand`, `gene_sequence`, `gc_content`, `cri_score`, `gene_note`, `gene_internal_comment`, `key_id_org`, `gene_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `gene` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `gene` SET `gene_id`='{0}', `gene_name`='{1}', `gene_posleft`='{2}', `gene_posright`='{3}', `gene_strand`='{4}', `gene_sequence`='{5}', `gc_content`='{6}', `cri_score`='{7}', `gene_note`='{8}', `gene_internal_comment`='{9}', `key_id_org`='{10}', `gene_type`='{11}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `gene` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `gene` (`gene_id`, `gene_name`, `gene_posleft`, `gene_posright`, `gene_strand`, `gene_sequence`, `gc_content`, `cri_score`, `gene_note`, `gene_internal_comment`, `key_id_org`, `gene_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, gene_id, gene_name, gene_posleft, gene_posright, gene_strand, gene_sequence, gc_content, cri_score, gene_note, gene_internal_comment, key_id_org, gene_type)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `gene` (`gene_id`, `gene_name`, `gene_posleft`, `gene_posright`, `gene_strand`, `gene_sequence`, `gc_content`, `cri_score`, `gene_note`, `gene_internal_comment`, `key_id_org`, `gene_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, gene_id, gene_name, gene_posleft, gene_posright, gene_strand, gene_sequence, gc_content, cri_score, gene_note, gene_internal_comment, key_id_org, gene_type)
        Else
        Return String.Format(INSERT_SQL, gene_id, gene_name, gene_posleft, gene_posright, gene_strand, gene_sequence, gc_content, cri_score, gene_note, gene_internal_comment, key_id_org, gene_type)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{gene_id}', '{gene_name}', '{gene_posleft}', '{gene_posright}', '{gene_strand}', '{gene_sequence}', '{gc_content}', '{cri_score}', '{gene_note}', '{gene_internal_comment}', '{key_id_org}', '{gene_type}')"
        Else
            Return $"('{gene_id}', '{gene_name}', '{gene_posleft}', '{gene_posright}', '{gene_strand}', '{gene_sequence}', '{gc_content}', '{cri_score}', '{gene_note}', '{gene_internal_comment}', '{key_id_org}', '{gene_type}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `gene` (`gene_id`, `gene_name`, `gene_posleft`, `gene_posright`, `gene_strand`, `gene_sequence`, `gc_content`, `cri_score`, `gene_note`, `gene_internal_comment`, `key_id_org`, `gene_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, gene_id, gene_name, gene_posleft, gene_posright, gene_strand, gene_sequence, gc_content, cri_score, gene_note, gene_internal_comment, key_id_org, gene_type)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `gene` (`gene_id`, `gene_name`, `gene_posleft`, `gene_posright`, `gene_strand`, `gene_sequence`, `gc_content`, `cri_score`, `gene_note`, `gene_internal_comment`, `key_id_org`, `gene_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, gene_id, gene_name, gene_posleft, gene_posright, gene_strand, gene_sequence, gc_content, cri_score, gene_note, gene_internal_comment, key_id_org, gene_type)
        Else
        Return String.Format(REPLACE_SQL, gene_id, gene_name, gene_posleft, gene_posright, gene_strand, gene_sequence, gc_content, cri_score, gene_note, gene_internal_comment, key_id_org, gene_type)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `gene` SET `gene_id`='{0}', `gene_name`='{1}', `gene_posleft`='{2}', `gene_posright`='{3}', `gene_strand`='{4}', `gene_sequence`='{5}', `gc_content`='{6}', `cri_score`='{7}', `gene_note`='{8}', `gene_internal_comment`='{9}', `key_id_org`='{10}', `gene_type`='{11}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As gene
                         Return DirectCast(MyClass.MemberwiseClone, gene)
                     End Function
End Class


End Namespace
