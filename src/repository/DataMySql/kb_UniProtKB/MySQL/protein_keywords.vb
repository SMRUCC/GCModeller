#Region "Microsoft.VisualBasic::dd5c8cca5a1e949c8f0e1b2e45619723, DataMySql\kb_UniProtKB\MySQL\protein_keywords.vb"

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

    ' Class protein_keywords
    ' 
    '     Properties: hash_code, keyword, keyword_id, uniprot_id
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

REM  Dump @2018/5/23 13:13:51


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace kb_UniProtKB.mysql

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `protein_keywords`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `protein_keywords` (
'''   `hash_code` int(10) unsigned NOT NULL,
'''   `uniprot_id` varchar(45) NOT NULL,
'''   `keyword_id` int(10) unsigned NOT NULL,
'''   `keyword` varchar(45) NOT NULL,
'''   PRIMARY KEY (`hash_code`,`keyword_id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("protein_keywords", Database:="kb_uniprotkb", SchemaSQL:="
CREATE TABLE `protein_keywords` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) NOT NULL,
  `keyword_id` int(10) unsigned NOT NULL,
  `keyword` varchar(45) NOT NULL,
  PRIMARY KEY (`hash_code`,`keyword_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class protein_keywords: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("hash_code"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="hash_code"), XmlAttribute> Public Property hash_code As Long
    <DatabaseField("uniprot_id"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="uniprot_id")> Public Property uniprot_id As String
    <DatabaseField("keyword_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="keyword_id"), XmlAttribute> Public Property keyword_id As Long
    <DatabaseField("keyword"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="keyword")> Public Property keyword As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `protein_keywords` (`hash_code`, `uniprot_id`, `keyword_id`, `keyword`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `protein_keywords` (`hash_code`, `uniprot_id`, `keyword_id`, `keyword`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `protein_keywords` (`hash_code`, `uniprot_id`, `keyword_id`, `keyword`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `protein_keywords` (`hash_code`, `uniprot_id`, `keyword_id`, `keyword`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `protein_keywords` WHERE `hash_code`='{0}' and `keyword_id`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `protein_keywords` SET `hash_code`='{0}', `uniprot_id`='{1}', `keyword_id`='{2}', `keyword`='{3}' WHERE `hash_code`='{4}' and `keyword_id`='{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `protein_keywords` WHERE `hash_code`='{0}' and `keyword_id`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, hash_code, keyword_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `protein_keywords` (`hash_code`, `uniprot_id`, `keyword_id`, `keyword`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, hash_code, uniprot_id, keyword_id, keyword)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `protein_keywords` (`hash_code`, `uniprot_id`, `keyword_id`, `keyword`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, hash_code, uniprot_id, keyword_id, keyword)
        Else
        Return String.Format(INSERT_SQL, hash_code, uniprot_id, keyword_id, keyword)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{hash_code}', '{uniprot_id}', '{keyword_id}', '{keyword}')"
        Else
            Return $"('{hash_code}', '{uniprot_id}', '{keyword_id}', '{keyword}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `protein_keywords` (`hash_code`, `uniprot_id`, `keyword_id`, `keyword`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, hash_code, uniprot_id, keyword_id, keyword)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `protein_keywords` (`hash_code`, `uniprot_id`, `keyword_id`, `keyword`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, hash_code, uniprot_id, keyword_id, keyword)
        Else
        Return String.Format(REPLACE_SQL, hash_code, uniprot_id, keyword_id, keyword)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `protein_keywords` SET `hash_code`='{0}', `uniprot_id`='{1}', `keyword_id`='{2}', `keyword`='{3}' WHERE `hash_code`='{4}' and `keyword_id`='{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, hash_code, uniprot_id, keyword_id, keyword, hash_code, keyword_id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As protein_keywords
                         Return DirectCast(MyClass.MemberwiseClone, protein_keywords)
                     End Function
End Class


End Namespace
