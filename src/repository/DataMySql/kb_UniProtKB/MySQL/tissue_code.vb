#Region "Microsoft.VisualBasic::0971bb095f2aec862ddc31eded7da39a, DataMySql\kb_UniProtKB\MySQL\tissue_code.vb"

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

    ' Class tissue_code
    ' 
    '     Properties: org_id, organism, tissue_name, uid
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
''' 对某一个物种的组织进行编号
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `tissue_code`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `tissue_code` (
'''   `uid` int(10) unsigned NOT NULL AUTO_INCREMENT,
'''   `tissue_name` varchar(45) NOT NULL,
'''   `org_id` int(10) unsigned DEFAULT NULL,
'''   `organism` varchar(45) DEFAULT NULL COMMENT '物种名称',
'''   PRIMARY KEY (`uid`),
'''   UNIQUE KEY `uid_UNIQUE` (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='对某一个物种的组织进行编号';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("tissue_code", Database:="kb_uniprotkb", SchemaSQL:="
CREATE TABLE `tissue_code` (
  `uid` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `tissue_name` varchar(45) NOT NULL,
  `org_id` int(10) unsigned DEFAULT NULL,
  `organism` varchar(45) DEFAULT NULL COMMENT '物种名称',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='对某一个物种的组织进行编号';")>
Public Class tissue_code: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="uid"), XmlAttribute> Public Property uid As Long
    <DatabaseField("tissue_name"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="tissue_name")> Public Property tissue_name As String
    <DatabaseField("org_id"), DataType(MySqlDbType.Int64, "10"), Column(Name:="org_id")> Public Property org_id As Long
''' <summary>
''' 物种名称
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("organism"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="organism")> Public Property organism As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `tissue_code` (`tissue_name`, `org_id`, `organism`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `tissue_code` (`uid`, `tissue_name`, `org_id`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `tissue_code` (`tissue_name`, `org_id`, `organism`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `tissue_code` (`uid`, `tissue_name`, `org_id`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `tissue_code` WHERE `uid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `tissue_code` SET `uid`='{0}', `tissue_name`='{1}', `org_id`='{2}', `organism`='{3}' WHERE `uid` = '{4}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `tissue_code` WHERE `uid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `tissue_code` (`uid`, `tissue_name`, `org_id`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, tissue_name, org_id, organism)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `tissue_code` (`uid`, `tissue_name`, `org_id`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, uid, tissue_name, org_id, organism)
        Else
        Return String.Format(INSERT_SQL, tissue_name, org_id, organism)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{uid}', '{tissue_name}', '{org_id}', '{organism}')"
        Else
            Return $"('{tissue_name}', '{org_id}', '{organism}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `tissue_code` (`uid`, `tissue_name`, `org_id`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, tissue_name, org_id, organism)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `tissue_code` (`uid`, `tissue_name`, `org_id`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, uid, tissue_name, org_id, organism)
        Else
        Return String.Format(REPLACE_SQL, tissue_name, org_id, organism)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `tissue_code` SET `uid`='{0}', `tissue_name`='{1}', `org_id`='{2}', `organism`='{3}' WHERE `uid` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, tissue_name, org_id, organism, uid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As tissue_code
                         Return DirectCast(MyClass.MemberwiseClone, tissue_code)
                     End Function
End Class


End Namespace
