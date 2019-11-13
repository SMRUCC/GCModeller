#Region "Microsoft.VisualBasic::29b1c5685d43d2d8a8a5e5fa162c6122, data\GO_gene-ontology\GO_mysql\kb_go\alt_id.vb"

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

    ' Class alt_id
    ' 
    '     Properties: alt_id, id, name
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

REM  Dump @2018/5/23 13:13:50


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace kb_go

''' <summary>
''' ```SQL
''' GO_term的主编号和次级编号之间的关系
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `alt_id`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `alt_id` (
'''   `id` int(10) unsigned NOT NULL,
'''   `alt_id` int(10) unsigned NOT NULL,
'''   `name` mediumtext COMMENT 'The name field in the go_term',
'''   PRIMARY KEY (`id`,`alt_id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='GO_term的主编号和次级编号之间的关系';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("alt_id", Database:="kb_go", SchemaSQL:="
CREATE TABLE `alt_id` (
  `id` int(10) unsigned NOT NULL,
  `alt_id` int(10) unsigned NOT NULL,
  `name` mediumtext COMMENT 'The name field in the go_term',
  PRIMARY KEY (`id`,`alt_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='GO_term的主编号和次级编号之间的关系';")>
Public Class alt_id: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("alt_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="alt_id"), XmlAttribute> Public Property alt_id As Long
''' <summary>
''' The name field in the go_term
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("name"), DataType(MySqlDbType.Text), Column(Name:="name")> Public Property name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `alt_id` (`id`, `alt_id`, `name`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `alt_id` (`id`, `alt_id`, `name`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `alt_id` (`id`, `alt_id`, `name`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `alt_id` (`id`, `alt_id`, `name`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `alt_id` WHERE `id`='{0}' and `alt_id`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `alt_id` SET `id`='{0}', `alt_id`='{1}', `name`='{2}' WHERE `id`='{3}' and `alt_id`='{4}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `alt_id` WHERE `id`='{0}' and `alt_id`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id, alt_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `alt_id` (`id`, `alt_id`, `name`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, alt_id, name)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `alt_id` (`id`, `alt_id`, `name`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, alt_id, name)
        Else
        Return String.Format(INSERT_SQL, id, alt_id, name)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{alt_id}', '{name}')"
        Else
            Return $"('{id}', '{alt_id}', '{name}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `alt_id` (`id`, `alt_id`, `name`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, alt_id, name)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `alt_id` (`id`, `alt_id`, `name`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, alt_id, name)
        Else
        Return String.Format(REPLACE_SQL, id, alt_id, name)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `alt_id` SET `id`='{0}', `alt_id`='{1}', `name`='{2}' WHERE `id`='{3}' and `alt_id`='{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, alt_id, name, id, alt_id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As alt_id
                         Return DirectCast(MyClass.MemberwiseClone, alt_id)
                     End Function
End Class


End Namespace
