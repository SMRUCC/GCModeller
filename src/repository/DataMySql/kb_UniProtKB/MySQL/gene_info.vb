#Region "Microsoft.VisualBasic::9902c4ef7134db7daf55d41b2af47d2e, DataMySql\kb_UniProtKB\MySQL\gene_info.vb"

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

    ' Class gene_info
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2017/9/3 12:29:35


Imports System.Data.Linq.Mapping
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports System.Xml.Serialization

Namespace kb_UniProtKB.mysql

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `gene_info`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `gene_info` (
'''   `hash_code` int(10) unsigned NOT NULL,
'''   `uniprot_id` varchar(45) DEFAULT NULL,
'''   `gene_name` varchar(45) DEFAULT NULL,
'''   `ORF` varchar(45) DEFAULT NULL,
'''   `synonym1` varchar(45) DEFAULT NULL,
'''   `synonym2` varchar(45) DEFAULT NULL,
'''   `synonym3` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`hash_code`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("gene_info", Database:="kb_uniprotkb", SchemaSQL:="
CREATE TABLE `gene_info` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `gene_name` varchar(45) DEFAULT NULL,
  `ORF` varchar(45) DEFAULT NULL,
  `synonym1` varchar(45) DEFAULT NULL,
  `synonym2` varchar(45) DEFAULT NULL,
  `synonym3` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class gene_info: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("hash_code"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="hash_code"), XmlAttribute> Public Property hash_code As Long
    <DatabaseField("uniprot_id"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="uniprot_id")> Public Property uniprot_id As String
    <DatabaseField("gene_name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="gene_name")> Public Property gene_name As String
    <DatabaseField("ORF"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="ORF")> Public Property ORF As String
    <DatabaseField("synonym1"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="synonym1")> Public Property synonym1 As String
    <DatabaseField("synonym2"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="synonym2")> Public Property synonym2 As String
    <DatabaseField("synonym3"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="synonym3")> Public Property synonym3 As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `gene_info` (`hash_code`, `uniprot_id`, `gene_name`, `ORF`, `synonym1`, `synonym2`, `synonym3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `gene_info` (`hash_code`, `uniprot_id`, `gene_name`, `ORF`, `synonym1`, `synonym2`, `synonym3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `gene_info` WHERE `hash_code` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `gene_info` SET `hash_code`='{0}', `uniprot_id`='{1}', `gene_name`='{2}', `ORF`='{3}', `synonym1`='{4}', `synonym2`='{5}', `synonym3`='{6}' WHERE `hash_code` = '{7}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `gene_info` WHERE `hash_code` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, hash_code)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `gene_info` (`hash_code`, `uniprot_id`, `gene_name`, `ORF`, `synonym1`, `synonym2`, `synonym3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, hash_code, uniprot_id, gene_name, ORF, synonym1, synonym2, synonym3)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{hash_code}', '{uniprot_id}', '{gene_name}', '{ORF}', '{synonym1}', '{synonym2}', '{synonym3}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `gene_info` (`hash_code`, `uniprot_id`, `gene_name`, `ORF`, `synonym1`, `synonym2`, `synonym3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, hash_code, uniprot_id, gene_name, ORF, synonym1, synonym2, synonym3)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `gene_info` SET `hash_code`='{0}', `uniprot_id`='{1}', `gene_name`='{2}', `ORF`='{3}', `synonym1`='{4}', `synonym2`='{5}', `synonym3`='{6}' WHERE `hash_code` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, hash_code, uniprot_id, gene_name, ORF, synonym1, synonym2, synonym3, hash_code)
    End Function
#End Region
Public Function Clone() As gene_info
                  Return DirectCast(MyClass.MemberwiseClone, gene_info)
              End Function
End Class


End Namespace
