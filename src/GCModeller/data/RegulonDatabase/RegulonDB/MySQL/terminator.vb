#Region "Microsoft.VisualBasic::0a3b03de474b4f019e52405b1a2ac127, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\terminator.vb"

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
    '     File Size: 10.49 KB


    ' Class terminator
    ' 
    '     Properties: key_id_org, terminator_class, terminator_dist_gene, terminator_id, terminator_internal_comment
    '                 terminator_note, terminator_posleft, terminator_posright, terminator_sequence
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
''' DROP TABLE IF EXISTS `terminator`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `terminator` (
'''   `terminator_id` char(12) NOT NULL,
'''   `terminator_dist_gene` decimal(10,0) DEFAULT NULL,
'''   `terminator_posleft` decimal(10,0) DEFAULT NULL,
'''   `terminator_posright` decimal(10,0) DEFAULT NULL,
'''   `terminator_class` varchar(30) DEFAULT NULL,
'''   `terminator_sequence` varchar(200) DEFAULT NULL,
'''   `terminator_note` varchar(2000) DEFAULT NULL,
'''   `terminator_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("terminator", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `terminator` (
  `terminator_id` char(12) NOT NULL,
  `terminator_dist_gene` decimal(10,0) DEFAULT NULL,
  `terminator_posleft` decimal(10,0) DEFAULT NULL,
  `terminator_posright` decimal(10,0) DEFAULT NULL,
  `terminator_class` varchar(30) DEFAULT NULL,
  `terminator_sequence` varchar(200) DEFAULT NULL,
  `terminator_note` varchar(2000) DEFAULT NULL,
  `terminator_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class terminator: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("terminator_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="terminator_id")> Public Property terminator_id As String
    <DatabaseField("terminator_dist_gene"), DataType(MySqlDbType.Decimal), Column(Name:="terminator_dist_gene")> Public Property terminator_dist_gene As Decimal
    <DatabaseField("terminator_posleft"), DataType(MySqlDbType.Decimal), Column(Name:="terminator_posleft")> Public Property terminator_posleft As Decimal
    <DatabaseField("terminator_posright"), DataType(MySqlDbType.Decimal), Column(Name:="terminator_posright")> Public Property terminator_posright As Decimal
    <DatabaseField("terminator_class"), DataType(MySqlDbType.VarChar, "30"), Column(Name:="terminator_class")> Public Property terminator_class As String
    <DatabaseField("terminator_sequence"), DataType(MySqlDbType.VarChar, "200"), Column(Name:="terminator_sequence")> Public Property terminator_sequence As String
    <DatabaseField("terminator_note"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="terminator_note")> Public Property terminator_note As String
    <DatabaseField("terminator_internal_comment"), DataType(MySqlDbType.Text), Column(Name:="terminator_internal_comment")> Public Property terminator_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="key_id_org")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `terminator` (`terminator_id`, `terminator_dist_gene`, `terminator_posleft`, `terminator_posright`, `terminator_class`, `terminator_sequence`, `terminator_note`, `terminator_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `terminator` (`terminator_id`, `terminator_dist_gene`, `terminator_posleft`, `terminator_posright`, `terminator_class`, `terminator_sequence`, `terminator_note`, `terminator_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `terminator` (`terminator_id`, `terminator_dist_gene`, `terminator_posleft`, `terminator_posright`, `terminator_class`, `terminator_sequence`, `terminator_note`, `terminator_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `terminator` (`terminator_id`, `terminator_dist_gene`, `terminator_posleft`, `terminator_posright`, `terminator_class`, `terminator_sequence`, `terminator_note`, `terminator_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `terminator` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `terminator` SET `terminator_id`='{0}', `terminator_dist_gene`='{1}', `terminator_posleft`='{2}', `terminator_posright`='{3}', `terminator_class`='{4}', `terminator_sequence`='{5}', `terminator_note`='{6}', `terminator_internal_comment`='{7}', `key_id_org`='{8}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `terminator` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `terminator` (`terminator_id`, `terminator_dist_gene`, `terminator_posleft`, `terminator_posright`, `terminator_class`, `terminator_sequence`, `terminator_note`, `terminator_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, terminator_id, terminator_dist_gene, terminator_posleft, terminator_posright, terminator_class, terminator_sequence, terminator_note, terminator_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `terminator` (`terminator_id`, `terminator_dist_gene`, `terminator_posleft`, `terminator_posright`, `terminator_class`, `terminator_sequence`, `terminator_note`, `terminator_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, terminator_id, terminator_dist_gene, terminator_posleft, terminator_posright, terminator_class, terminator_sequence, terminator_note, terminator_internal_comment, key_id_org)
        Else
        Return String.Format(INSERT_SQL, terminator_id, terminator_dist_gene, terminator_posleft, terminator_posright, terminator_class, terminator_sequence, terminator_note, terminator_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{terminator_id}', '{terminator_dist_gene}', '{terminator_posleft}', '{terminator_posright}', '{terminator_class}', '{terminator_sequence}', '{terminator_note}', '{terminator_internal_comment}', '{key_id_org}')"
        Else
            Return $"('{terminator_id}', '{terminator_dist_gene}', '{terminator_posleft}', '{terminator_posright}', '{terminator_class}', '{terminator_sequence}', '{terminator_note}', '{terminator_internal_comment}', '{key_id_org}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `terminator` (`terminator_id`, `terminator_dist_gene`, `terminator_posleft`, `terminator_posright`, `terminator_class`, `terminator_sequence`, `terminator_note`, `terminator_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, terminator_id, terminator_dist_gene, terminator_posleft, terminator_posright, terminator_class, terminator_sequence, terminator_note, terminator_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `terminator` (`terminator_id`, `terminator_dist_gene`, `terminator_posleft`, `terminator_posright`, `terminator_class`, `terminator_sequence`, `terminator_note`, `terminator_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, terminator_id, terminator_dist_gene, terminator_posleft, terminator_posright, terminator_class, terminator_sequence, terminator_note, terminator_internal_comment, key_id_org)
        Else
        Return String.Format(REPLACE_SQL, terminator_id, terminator_dist_gene, terminator_posleft, terminator_posright, terminator_class, terminator_sequence, terminator_note, terminator_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `terminator` SET `terminator_id`='{0}', `terminator_dist_gene`='{1}', `terminator_posleft`='{2}', `terminator_posright`='{3}', `terminator_class`='{4}', `terminator_sequence`='{5}', `terminator_note`='{6}', `terminator_internal_comment`='{7}', `key_id_org`='{8}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As terminator
                         Return DirectCast(MyClass.MemberwiseClone, terminator)
                     End Function
End Class


End Namespace
