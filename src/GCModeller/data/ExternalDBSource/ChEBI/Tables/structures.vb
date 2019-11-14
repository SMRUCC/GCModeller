#Region "Microsoft.VisualBasic::6ebab9e074f6736b67bc565d22acbf5b, data\ExternalDBSource\ChEBI\Tables\structures.vb"

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

    ' Class structures
    ' 
    '     Properties: [structure], compound_id, dimension, id, type
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
''' DROP TABLE IF EXISTS `structures`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `structures` (
'''   `id` int(11) NOT NULL,
'''   `compound_id` int(11) NOT NULL,
'''   `structure` text NOT NULL,
'''   `type` text NOT NULL,
'''   `dimension` text NOT NULL,
'''   PRIMARY KEY (`id`),
'''   KEY `FK_STRUCTURES_TO_COMPOUND` (`compound_id`),
'''   CONSTRAINT `FK_STRUCTURES_TO_COMPOUND` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("structures", Database:="chebi", SchemaSQL:="
CREATE TABLE `structures` (
  `id` int(11) NOT NULL,
  `compound_id` int(11) NOT NULL,
  `structure` text NOT NULL,
  `type` text NOT NULL,
  `dimension` text NOT NULL,
  PRIMARY KEY (`id`),
  KEY `FK_STRUCTURES_TO_COMPOUND` (`compound_id`),
  CONSTRAINT `FK_STRUCTURES_TO_COMPOUND` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class structures: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("compound_id"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="compound_id")> Public Property compound_id As Long
    <DatabaseField("structure"), NotNull, DataType(MySqlDbType.Text), Column(Name:="structure")> Public Property [structure] As String
    <DatabaseField("type"), NotNull, DataType(MySqlDbType.Text), Column(Name:="type")> Public Property type As String
    <DatabaseField("dimension"), NotNull, DataType(MySqlDbType.Text), Column(Name:="dimension")> Public Property dimension As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `structures` (`id`, `compound_id`, `structure`, `type`, `dimension`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `structures` (`id`, `compound_id`, `structure`, `type`, `dimension`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `structures` (`id`, `compound_id`, `structure`, `type`, `dimension`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `structures` (`id`, `compound_id`, `structure`, `type`, `dimension`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `structures` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `structures` SET `id`='{0}', `compound_id`='{1}', `structure`='{2}', `type`='{3}', `dimension`='{4}' WHERE `id` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `structures` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `structures` (`id`, `compound_id`, `structure`, `type`, `dimension`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, compound_id, [structure], type, dimension)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `structures` (`id`, `compound_id`, `structure`, `type`, `dimension`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, compound_id, [structure], type, dimension)
        Else
        Return String.Format(INSERT_SQL, id, compound_id, [structure], type, dimension)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{compound_id}', '{[structure]}', '{type}', '{dimension}')"
        Else
            Return $"('{id}', '{compound_id}', '{[structure]}', '{type}', '{dimension}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `structures` (`id`, `compound_id`, `structure`, `type`, `dimension`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, compound_id, [structure], type, dimension)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `structures` (`id`, `compound_id`, `structure`, `type`, `dimension`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, compound_id, [structure], type, dimension)
        Else
        Return String.Format(REPLACE_SQL, id, compound_id, [structure], type, dimension)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `structures` SET `id`='{0}', `compound_id`='{1}', `structure`='{2}', `type`='{3}', `dimension`='{4}' WHERE `id` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, compound_id, [structure], type, dimension, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As structures
                         Return DirectCast(MyClass.MemberwiseClone, structures)
                     End Function
End Class


End Namespace
