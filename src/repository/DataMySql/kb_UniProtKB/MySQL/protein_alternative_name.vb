#Region "Microsoft.VisualBasic::4b02d04de2ff6672278320d2aa49afeb, DataMySql\kb_UniProtKB\MySQL\protein_alternative_name.vb"

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

    ' Class protein_alternative_name
    ' 
    '     Properties: fullName, hash_code, name, shortName1, shortName2
    '                 shortName3, shortName4, shortName5, uid, uniprot_id
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
''' 一个蛋白质会有多个候选名称
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `protein_alternative_name`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `protein_alternative_name` (
'''   `uid` int(10) unsigned NOT NULL AUTO_INCREMENT,
'''   `hash_code` int(10) unsigned NOT NULL,
'''   `uniprot_id` varchar(45) NOT NULL,
'''   `name` varchar(45) NOT NULL,
'''   `fullName` varchar(45) DEFAULT NULL,
'''   `shortName1` varchar(45) DEFAULT NULL,
'''   `shortName2` varchar(45) DEFAULT NULL,
'''   `shortName3` varchar(45) DEFAULT NULL,
'''   `shortName4` varchar(45) DEFAULT NULL,
'''   `shortName5` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`uid`),
'''   UNIQUE KEY `uid_UNIQUE` (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='一个蛋白质会有多个候选名称';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("protein_alternative_name", Database:="kb_uniprotkb", SchemaSQL:="
CREATE TABLE `protein_alternative_name` (
  `uid` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) NOT NULL,
  `name` varchar(45) NOT NULL,
  `fullName` varchar(45) DEFAULT NULL,
  `shortName1` varchar(45) DEFAULT NULL,
  `shortName2` varchar(45) DEFAULT NULL,
  `shortName3` varchar(45) DEFAULT NULL,
  `shortName4` varchar(45) DEFAULT NULL,
  `shortName5` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='一个蛋白质会有多个候选名称';")>
Public Class protein_alternative_name: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="uid"), XmlAttribute> Public Property uid As Long
    <DatabaseField("hash_code"), NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="hash_code")> Public Property hash_code As Long
    <DatabaseField("uniprot_id"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="uniprot_id")> Public Property uniprot_id As String
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="name")> Public Property name As String
    <DatabaseField("fullName"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="fullName")> Public Property fullName As String
    <DatabaseField("shortName1"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="shortName1")> Public Property shortName1 As String
    <DatabaseField("shortName2"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="shortName2")> Public Property shortName2 As String
    <DatabaseField("shortName3"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="shortName3")> Public Property shortName3 As String
    <DatabaseField("shortName4"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="shortName4")> Public Property shortName4 As String
    <DatabaseField("shortName5"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="shortName5")> Public Property shortName5 As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `protein_alternative_name` (`hash_code`, `uniprot_id`, `name`, `fullName`, `shortName1`, `shortName2`, `shortName3`, `shortName4`, `shortName5`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `protein_alternative_name` (`uid`, `hash_code`, `uniprot_id`, `name`, `fullName`, `shortName1`, `shortName2`, `shortName3`, `shortName4`, `shortName5`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `protein_alternative_name` (`hash_code`, `uniprot_id`, `name`, `fullName`, `shortName1`, `shortName2`, `shortName3`, `shortName4`, `shortName5`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `protein_alternative_name` (`uid`, `hash_code`, `uniprot_id`, `name`, `fullName`, `shortName1`, `shortName2`, `shortName3`, `shortName4`, `shortName5`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `protein_alternative_name` WHERE `uid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `protein_alternative_name` SET `uid`='{0}', `hash_code`='{1}', `uniprot_id`='{2}', `name`='{3}', `fullName`='{4}', `shortName1`='{5}', `shortName2`='{6}', `shortName3`='{7}', `shortName4`='{8}', `shortName5`='{9}' WHERE `uid` = '{10}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `protein_alternative_name` WHERE `uid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `protein_alternative_name` (`uid`, `hash_code`, `uniprot_id`, `name`, `fullName`, `shortName1`, `shortName2`, `shortName3`, `shortName4`, `shortName5`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, hash_code, uniprot_id, name, fullName, shortName1, shortName2, shortName3, shortName4, shortName5)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `protein_alternative_name` (`uid`, `hash_code`, `uniprot_id`, `name`, `fullName`, `shortName1`, `shortName2`, `shortName3`, `shortName4`, `shortName5`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, uid, hash_code, uniprot_id, name, fullName, shortName1, shortName2, shortName3, shortName4, shortName5)
        Else
        Return String.Format(INSERT_SQL, hash_code, uniprot_id, name, fullName, shortName1, shortName2, shortName3, shortName4, shortName5)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{uid}', '{hash_code}', '{uniprot_id}', '{name}', '{fullName}', '{shortName1}', '{shortName2}', '{shortName3}', '{shortName4}', '{shortName5}')"
        Else
            Return $"('{hash_code}', '{uniprot_id}', '{name}', '{fullName}', '{shortName1}', '{shortName2}', '{shortName3}', '{shortName4}', '{shortName5}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `protein_alternative_name` (`uid`, `hash_code`, `uniprot_id`, `name`, `fullName`, `shortName1`, `shortName2`, `shortName3`, `shortName4`, `shortName5`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, hash_code, uniprot_id, name, fullName, shortName1, shortName2, shortName3, shortName4, shortName5)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `protein_alternative_name` (`uid`, `hash_code`, `uniprot_id`, `name`, `fullName`, `shortName1`, `shortName2`, `shortName3`, `shortName4`, `shortName5`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, uid, hash_code, uniprot_id, name, fullName, shortName1, shortName2, shortName3, shortName4, shortName5)
        Else
        Return String.Format(REPLACE_SQL, hash_code, uniprot_id, name, fullName, shortName1, shortName2, shortName3, shortName4, shortName5)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `protein_alternative_name` SET `uid`='{0}', `hash_code`='{1}', `uniprot_id`='{2}', `name`='{3}', `fullName`='{4}', `shortName1`='{5}', `shortName2`='{6}', `shortName3`='{7}', `shortName4`='{8}', `shortName5`='{9}' WHERE `uid` = '{10}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, hash_code, uniprot_id, name, fullName, shortName1, shortName2, shortName3, shortName4, shortName5, uid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As protein_alternative_name
                         Return DirectCast(MyClass.MemberwiseClone, protein_alternative_name)
                     End Function
End Class


End Namespace
