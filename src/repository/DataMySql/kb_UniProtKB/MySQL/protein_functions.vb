#Region "Microsoft.VisualBasic::0fe28a419f22594abaf57aa47e96c887, DataMySql\kb_UniProtKB\MySQL\protein_functions.vb"

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

    ' Class protein_functions
    ' 
    '     Properties: [function], full_name, hash_code, name, short_name1
    '                 short_name2, short_name3, uniprot_id
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
''' 对蛋白质的名称以及功能方面的字符串描述
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `protein_functions`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `protein_functions` (
'''   `hash_code` int(10) unsigned NOT NULL,
'''   `uniprot_id` varchar(45) DEFAULT NULL,
'''   `function` varchar(45) DEFAULT NULL COMMENT 'comment -> type = function',
'''   `name` varchar(45) DEFAULT NULL,
'''   `full_name` varchar(45) DEFAULT NULL COMMENT 'recommendedName',
'''   `short_name1` varchar(45) DEFAULT NULL COMMENT 'recommendedName',
'''   `short_name2` varchar(45) DEFAULT NULL COMMENT 'recommendedName',
'''   `short_name3` varchar(45) DEFAULT NULL COMMENT 'recommendedName',
'''   PRIMARY KEY (`hash_code`),
'''   UNIQUE KEY `hash_code_UNIQUE` (`hash_code`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='对蛋白质的名称以及功能方面的字符串描述';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("protein_functions", Database:="kb_uniprotkb", SchemaSQL:="
CREATE TABLE `protein_functions` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `function` varchar(45) DEFAULT NULL COMMENT 'comment -> type = function',
  `name` varchar(45) DEFAULT NULL,
  `full_name` varchar(45) DEFAULT NULL COMMENT 'recommendedName',
  `short_name1` varchar(45) DEFAULT NULL COMMENT 'recommendedName',
  `short_name2` varchar(45) DEFAULT NULL COMMENT 'recommendedName',
  `short_name3` varchar(45) DEFAULT NULL COMMENT 'recommendedName',
  PRIMARY KEY (`hash_code`),
  UNIQUE KEY `hash_code_UNIQUE` (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='对蛋白质的名称以及功能方面的字符串描述';")>
Public Class protein_functions: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("hash_code"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="hash_code"), XmlAttribute> Public Property hash_code As Long
    <DatabaseField("uniprot_id"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="uniprot_id")> Public Property uniprot_id As String
''' <summary>
''' comment -> type = function
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("function"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="function")> Public Property [function] As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="name")> Public Property name As String
''' <summary>
''' recommendedName
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("full_name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="full_name")> Public Property full_name As String
''' <summary>
''' recommendedName
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("short_name1"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="short_name1")> Public Property short_name1 As String
''' <summary>
''' recommendedName
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("short_name2"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="short_name2")> Public Property short_name2 As String
''' <summary>
''' recommendedName
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("short_name3"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="short_name3")> Public Property short_name3 As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `protein_functions` (`hash_code`, `uniprot_id`, `function`, `name`, `full_name`, `short_name1`, `short_name2`, `short_name3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `protein_functions` (`hash_code`, `uniprot_id`, `function`, `name`, `full_name`, `short_name1`, `short_name2`, `short_name3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `protein_functions` (`hash_code`, `uniprot_id`, `function`, `name`, `full_name`, `short_name1`, `short_name2`, `short_name3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `protein_functions` (`hash_code`, `uniprot_id`, `function`, `name`, `full_name`, `short_name1`, `short_name2`, `short_name3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `protein_functions` WHERE `hash_code` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `protein_functions` SET `hash_code`='{0}', `uniprot_id`='{1}', `function`='{2}', `name`='{3}', `full_name`='{4}', `short_name1`='{5}', `short_name2`='{6}', `short_name3`='{7}' WHERE `hash_code` = '{8}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `protein_functions` WHERE `hash_code` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, hash_code)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `protein_functions` (`hash_code`, `uniprot_id`, `function`, `name`, `full_name`, `short_name1`, `short_name2`, `short_name3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, hash_code, uniprot_id, [function], name, full_name, short_name1, short_name2, short_name3)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `protein_functions` (`hash_code`, `uniprot_id`, `function`, `name`, `full_name`, `short_name1`, `short_name2`, `short_name3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, hash_code, uniprot_id, [function], name, full_name, short_name1, short_name2, short_name3)
        Else
        Return String.Format(INSERT_SQL, hash_code, uniprot_id, [function], name, full_name, short_name1, short_name2, short_name3)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{hash_code}', '{uniprot_id}', '{[function]}', '{name}', '{full_name}', '{short_name1}', '{short_name2}', '{short_name3}')"
        Else
            Return $"('{hash_code}', '{uniprot_id}', '{[function]}', '{name}', '{full_name}', '{short_name1}', '{short_name2}', '{short_name3}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `protein_functions` (`hash_code`, `uniprot_id`, `function`, `name`, `full_name`, `short_name1`, `short_name2`, `short_name3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, hash_code, uniprot_id, [function], name, full_name, short_name1, short_name2, short_name3)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `protein_functions` (`hash_code`, `uniprot_id`, `function`, `name`, `full_name`, `short_name1`, `short_name2`, `short_name3`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, hash_code, uniprot_id, [function], name, full_name, short_name1, short_name2, short_name3)
        Else
        Return String.Format(REPLACE_SQL, hash_code, uniprot_id, [function], name, full_name, short_name1, short_name2, short_name3)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `protein_functions` SET `hash_code`='{0}', `uniprot_id`='{1}', `function`='{2}', `name`='{3}', `full_name`='{4}', `short_name1`='{5}', `short_name2`='{6}', `short_name3`='{7}' WHERE `hash_code` = '{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, hash_code, uniprot_id, [function], name, full_name, short_name1, short_name2, short_name3, hash_code)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As protein_functions
                         Return DirectCast(MyClass.MemberwiseClone, protein_functions)
                     End Function
End Class


End Namespace
