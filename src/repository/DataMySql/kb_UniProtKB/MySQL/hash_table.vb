#Region "Microsoft.VisualBasic::b05b2f15043ba00fd39e772441a4ae57, DataMySql\kb_UniProtKB\MySQL\hash_table.vb"

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

    ' Class hash_table
    ' 
    '     Properties: hash_code, name, uniprot_id
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
''' 这个表主要是为了加快整个数据库的查询效率而建立的冗余表，在这里为每一个uniport accession编号都赋值了一个唯一编号，然后利用这个唯一编号就可以实现对其他数据表之中的数据的快速查询了
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `hash_table`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `hash_table` (
'''   `hash_code` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT '每一个字符串形式的uniprot数据库编号都有一个唯一的哈希值编号',
'''   `uniprot_id` char(32) NOT NULL COMMENT 'uniprot数据库编号首先会在这个表之中进行查找，得到自己唯一的哈希值结果，然后再根据这个哈希值去快速的查找其他的表之中的结果',
'''   `name` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`uniprot_id`),
'''   UNIQUE KEY `uniprot_id_UNIQUE` (`uniprot_id`),
'''   UNIQUE KEY `hash_code_UNIQUE` (`hash_code`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='这个表主要是为了加快整个数据库的查询效率而建立的冗余表，在这里为每一个uniport accession编号都赋值了一个唯一编号，然后利用这个唯一编号就可以实现对其他数据表之中的数据的快速查询了';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("hash_table", Database:="kb_uniprotkb", SchemaSQL:="
CREATE TABLE `hash_table` (
  `hash_code` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT '每一个字符串形式的uniprot数据库编号都有一个唯一的哈希值编号',
  `uniprot_id` char(32) NOT NULL COMMENT 'uniprot数据库编号首先会在这个表之中进行查找，得到自己唯一的哈希值结果，然后再根据这个哈希值去快速的查找其他的表之中的结果',
  `name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uniprot_id`),
  UNIQUE KEY `uniprot_id_UNIQUE` (`uniprot_id`),
  UNIQUE KEY `hash_code_UNIQUE` (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='这个表主要是为了加快整个数据库的查询效率而建立的冗余表，在这里为每一个uniport accession编号都赋值了一个唯一编号，然后利用这个唯一编号就可以实现对其他数据表之中的数据的快速查询了';")>
Public Class hash_table: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
''' <summary>
''' 每一个字符串形式的uniprot数据库编号都有一个唯一的哈希值编号
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("hash_code"), AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="hash_code")> Public Property hash_code As Long
''' <summary>
''' uniprot数据库编号首先会在这个表之中进行查找，得到自己唯一的哈希值结果，然后再根据这个哈希值去快速的查找其他的表之中的结果
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("uniprot_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "32"), Column(Name:="uniprot_id"), XmlAttribute> Public Property uniprot_id As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="name")> Public Property name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `hash_table` (`uniprot_id`, `name`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `hash_table` (`hash_code`, `uniprot_id`, `name`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `hash_table` (`uniprot_id`, `name`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `hash_table` (`hash_code`, `uniprot_id`, `name`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `hash_table` WHERE `uniprot_id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `hash_table` SET `hash_code`='{0}', `uniprot_id`='{1}', `name`='{2}' WHERE `uniprot_id` = '{3}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `hash_table` WHERE `uniprot_id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uniprot_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `hash_table` (`hash_code`, `uniprot_id`, `name`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uniprot_id, name)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `hash_table` (`hash_code`, `uniprot_id`, `name`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, hash_code, uniprot_id, name)
        Else
        Return String.Format(INSERT_SQL, uniprot_id, name)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{hash_code}', '{uniprot_id}', '{name}')"
        Else
            Return $"('{uniprot_id}', '{name}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `hash_table` (`hash_code`, `uniprot_id`, `name`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uniprot_id, name)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `hash_table` (`hash_code`, `uniprot_id`, `name`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, hash_code, uniprot_id, name)
        Else
        Return String.Format(REPLACE_SQL, uniprot_id, name)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `hash_table` SET `hash_code`='{0}', `uniprot_id`='{1}', `name`='{2}' WHERE `uniprot_id` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, hash_code, uniprot_id, name, uniprot_id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As hash_table
                         Return DirectCast(MyClass.MemberwiseClone, hash_table)
                     End Function
End Class


End Namespace
