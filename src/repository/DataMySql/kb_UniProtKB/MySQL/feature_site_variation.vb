#Region "Microsoft.VisualBasic::2e9c075b3462c83d1ae46be45f58f419, DataMySql\kb_UniProtKB\MySQL\feature_site_variation.vb"

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

    ' Class feature_site_variation
    ' 
    '     Properties: hash_code, original, position, uid, uniprot_id
    '                 variation
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
''' 序列的突变位点
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `feature_site_variation`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `feature_site_variation` (
'''   `uid` int(10) unsigned NOT NULL,
'''   `hash_code` int(10) unsigned NOT NULL,
'''   `uniprot_id` varchar(45) DEFAULT NULL,
'''   `original` varchar(45) DEFAULT NULL,
'''   `variation` varchar(45) DEFAULT NULL,
'''   `position` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`uid`,`hash_code`),
'''   UNIQUE KEY `uid_UNIQUE` (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='序列的突变位点';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("feature_site_variation", Database:="kb_uniprotkb", SchemaSQL:="
CREATE TABLE `feature_site_variation` (
  `uid` int(10) unsigned NOT NULL,
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `original` varchar(45) DEFAULT NULL,
  `variation` varchar(45) DEFAULT NULL,
  `position` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`,`hash_code`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='序列的突变位点';")>
Public Class feature_site_variation: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="uid"), XmlAttribute> Public Property uid As Long
    <DatabaseField("hash_code"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="hash_code"), XmlAttribute> Public Property hash_code As Long
    <DatabaseField("uniprot_id"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="uniprot_id")> Public Property uniprot_id As String
    <DatabaseField("original"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="original")> Public Property original As String
    <DatabaseField("variation"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="variation")> Public Property variation As String
    <DatabaseField("position"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="position")> Public Property position As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `feature_site_variation` (`uid`, `hash_code`, `uniprot_id`, `original`, `variation`, `position`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `feature_site_variation` (`uid`, `hash_code`, `uniprot_id`, `original`, `variation`, `position`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `feature_site_variation` (`uid`, `hash_code`, `uniprot_id`, `original`, `variation`, `position`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `feature_site_variation` (`uid`, `hash_code`, `uniprot_id`, `original`, `variation`, `position`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `feature_site_variation` WHERE `uid`='{0}' and `hash_code`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `feature_site_variation` SET `uid`='{0}', `hash_code`='{1}', `uniprot_id`='{2}', `original`='{3}', `variation`='{4}', `position`='{5}' WHERE `uid`='{6}' and `hash_code`='{7}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `feature_site_variation` WHERE `uid`='{0}' and `hash_code`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uid, hash_code)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `feature_site_variation` (`uid`, `hash_code`, `uniprot_id`, `original`, `variation`, `position`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uid, hash_code, uniprot_id, original, variation, position)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `feature_site_variation` (`uid`, `hash_code`, `uniprot_id`, `original`, `variation`, `position`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, uid, hash_code, uniprot_id, original, variation, position)
        Else
        Return String.Format(INSERT_SQL, uid, hash_code, uniprot_id, original, variation, position)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{uid}', '{hash_code}', '{uniprot_id}', '{original}', '{variation}', '{position}')"
        Else
            Return $"('{uid}', '{hash_code}', '{uniprot_id}', '{original}', '{variation}', '{position}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `feature_site_variation` (`uid`, `hash_code`, `uniprot_id`, `original`, `variation`, `position`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uid, hash_code, uniprot_id, original, variation, position)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `feature_site_variation` (`uid`, `hash_code`, `uniprot_id`, `original`, `variation`, `position`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, uid, hash_code, uniprot_id, original, variation, position)
        Else
        Return String.Format(REPLACE_SQL, uid, hash_code, uniprot_id, original, variation, position)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `feature_site_variation` SET `uid`='{0}', `hash_code`='{1}', `uniprot_id`='{2}', `original`='{3}', `variation`='{4}', `position`='{5}' WHERE `uid`='{6}' and `hash_code`='{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, hash_code, uniprot_id, original, variation, position, uid, hash_code)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As feature_site_variation
                         Return DirectCast(MyClass.MemberwiseClone, feature_site_variation)
                     End Function
End Class


End Namespace
