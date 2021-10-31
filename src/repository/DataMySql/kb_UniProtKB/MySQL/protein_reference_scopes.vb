#Region "Microsoft.VisualBasic::9ec1e249d9cda4d49b8f6d11aa0407a7, DataMySql\kb_UniProtKB\MySQL\protein_reference_scopes.vb"

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

    ' Class protein_reference_scopes
    ' 
    '     Properties: scope, scope_id, uid, uniprot_hashcode, uniprot_id
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
''' DROP TABLE IF EXISTS `protein_reference_scopes`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `protein_reference_scopes` (
'''   `uid` int(10) unsigned NOT NULL COMMENT '指向的是protein_reference表之中的uid唯一标识符字段',
'''   `scope_id` int(10) unsigned NOT NULL,
'''   `scope` varchar(45) NOT NULL,
'''   `uniprot_hashcode` int(10) unsigned NOT NULL,
'''   `uniprot_id` varchar(45) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("protein_reference_scopes", Database:="kb_uniprotkb", SchemaSQL:="
CREATE TABLE `protein_reference_scopes` (
  `uid` int(10) unsigned NOT NULL COMMENT '指向的是protein_reference表之中的uid唯一标识符字段',
  `scope_id` int(10) unsigned NOT NULL,
  `scope` varchar(45) NOT NULL,
  `uniprot_hashcode` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class protein_reference_scopes: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
''' <summary>
''' 指向的是protein_reference表之中的uid唯一标识符字段
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("uid"), NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="uid")> Public Property uid As Long
    <DatabaseField("scope_id"), NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="scope_id")> Public Property scope_id As Long
    <DatabaseField("scope"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="scope")> Public Property scope As String
    <DatabaseField("uniprot_hashcode"), NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="uniprot_hashcode")> Public Property uniprot_hashcode As Long
    <DatabaseField("uniprot_id"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="uniprot_id")> Public Property uniprot_id As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `protein_reference_scopes` (`uid`, `scope_id`, `scope`, `uniprot_hashcode`, `uniprot_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `protein_reference_scopes` (`uid`, `scope_id`, `scope`, `uniprot_hashcode`, `uniprot_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `protein_reference_scopes` (`uid`, `scope_id`, `scope`, `uniprot_hashcode`, `uniprot_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `protein_reference_scopes` (`uid`, `scope_id`, `scope`, `uniprot_hashcode`, `uniprot_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `protein_reference_scopes` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `protein_reference_scopes` SET `uid`='{0}', `scope_id`='{1}', `scope`='{2}', `uniprot_hashcode`='{3}', `uniprot_id`='{4}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `protein_reference_scopes` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `protein_reference_scopes` (`uid`, `scope_id`, `scope`, `uniprot_hashcode`, `uniprot_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uid, scope_id, scope, uniprot_hashcode, uniprot_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `protein_reference_scopes` (`uid`, `scope_id`, `scope`, `uniprot_hashcode`, `uniprot_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, uid, scope_id, scope, uniprot_hashcode, uniprot_id)
        Else
        Return String.Format(INSERT_SQL, uid, scope_id, scope, uniprot_hashcode, uniprot_id)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{uid}', '{scope_id}', '{scope}', '{uniprot_hashcode}', '{uniprot_id}')"
        Else
            Return $"('{uid}', '{scope_id}', '{scope}', '{uniprot_hashcode}', '{uniprot_id}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `protein_reference_scopes` (`uid`, `scope_id`, `scope`, `uniprot_hashcode`, `uniprot_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uid, scope_id, scope, uniprot_hashcode, uniprot_id)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `protein_reference_scopes` (`uid`, `scope_id`, `scope`, `uniprot_hashcode`, `uniprot_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, uid, scope_id, scope, uniprot_hashcode, uniprot_id)
        Else
        Return String.Format(REPLACE_SQL, uid, scope_id, scope, uniprot_hashcode, uniprot_id)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `protein_reference_scopes` SET `uid`='{0}', `scope_id`='{1}', `scope`='{2}', `uniprot_hashcode`='{3}', `uniprot_id`='{4}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As protein_reference_scopes
                         Return DirectCast(MyClass.MemberwiseClone, protein_reference_scopes)
                     End Function
End Class


End Namespace
