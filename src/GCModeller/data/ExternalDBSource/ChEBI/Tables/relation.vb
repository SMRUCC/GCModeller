#Region "Microsoft.VisualBasic::b623012da101db99071a7676537c0dfb, data\ExternalDBSource\ChEBI\Tables\relation.vb"

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

    ' Class relation
    ' 
    '     Properties: final_id, id, init_id, status, type
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

REM  Dump @2018/5/23 13:13:39


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace ChEBI.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `relation`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `relation` (
'''   `id` int(11) NOT NULL,
'''   `type` text NOT NULL,
'''   `init_id` int(11) NOT NULL,
'''   `final_id` int(11) NOT NULL,
'''   `status` varchar(1) NOT NULL,
'''   PRIMARY KEY (`id`),
'''   KEY `final_id` (`final_id`),
'''   KEY `init_id` (`init_id`),
'''   CONSTRAINT `FK_RELATION_TO_FINAL_VERTICE` FOREIGN KEY (`final_id`) REFERENCES `vertice` (`id`),
'''   CONSTRAINT `FK_RELATION_TO_INIT_VERTICE` FOREIGN KEY (`init_id`) REFERENCES `vertice` (`id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("relation", Database:="chebi", SchemaSQL:="
CREATE TABLE `relation` (
  `id` int(11) NOT NULL,
  `type` text NOT NULL,
  `init_id` int(11) NOT NULL,
  `final_id` int(11) NOT NULL,
  `status` varchar(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `final_id` (`final_id`),
  KEY `init_id` (`init_id`),
  CONSTRAINT `FK_RELATION_TO_FINAL_VERTICE` FOREIGN KEY (`final_id`) REFERENCES `vertice` (`id`),
  CONSTRAINT `FK_RELATION_TO_INIT_VERTICE` FOREIGN KEY (`init_id`) REFERENCES `vertice` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class relation: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("type"), NotNull, DataType(MySqlDbType.Text), Column(Name:="type")> Public Property type As String
    <DatabaseField("init_id"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="init_id")> Public Property init_id As Long
    <DatabaseField("final_id"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="final_id")> Public Property final_id As Long
    <DatabaseField("status"), NotNull, DataType(MySqlDbType.VarChar, "1"), Column(Name:="status")> Public Property status As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `relation` (`id`, `type`, `init_id`, `final_id`, `status`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `relation` (`id`, `type`, `init_id`, `final_id`, `status`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `relation` (`id`, `type`, `init_id`, `final_id`, `status`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `relation` (`id`, `type`, `init_id`, `final_id`, `status`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `relation` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `relation` SET `id`='{0}', `type`='{1}', `init_id`='{2}', `final_id`='{3}', `status`='{4}' WHERE `id` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `relation` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `relation` (`id`, `type`, `init_id`, `final_id`, `status`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, type, init_id, final_id, status)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `relation` (`id`, `type`, `init_id`, `final_id`, `status`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, type, init_id, final_id, status)
        Else
        Return String.Format(INSERT_SQL, id, type, init_id, final_id, status)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{type}', '{init_id}', '{final_id}', '{status}')"
        Else
            Return $"('{id}', '{type}', '{init_id}', '{final_id}', '{status}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `relation` (`id`, `type`, `init_id`, `final_id`, `status`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, type, init_id, final_id, status)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `relation` (`id`, `type`, `init_id`, `final_id`, `status`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, type, init_id, final_id, status)
        Else
        Return String.Format(REPLACE_SQL, id, type, init_id, final_id, status)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `relation` SET `id`='{0}', `type`='{1}', `init_id`='{2}', `final_id`='{3}', `status`='{4}' WHERE `id` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, type, init_id, final_id, status, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As relation
                         Return DirectCast(MyClass.MemberwiseClone, relation)
                     End Function
End Class


End Namespace
