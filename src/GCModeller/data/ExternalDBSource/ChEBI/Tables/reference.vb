#Region "Microsoft.VisualBasic::f10547802aefb6a355c131570e4f24fc, data\ExternalDBSource\ChEBI\Tables\reference.vb"

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

    ' Class reference
    ' 
    '     Properties: compound_id, id, location_in_ref, reference_db_name, reference_id
    '                 reference_name
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
''' DROP TABLE IF EXISTS `reference`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `reference` (
'''   `id` int(11) NOT NULL,
'''   `compound_id` int(11) NOT NULL,
'''   `reference_id` varchar(60) NOT NULL,
'''   `reference_db_name` varchar(60) NOT NULL,
'''   `location_in_ref` varchar(90) DEFAULT NULL,
'''   `reference_name` varchar(512) DEFAULT NULL,
'''   PRIMARY KEY (`id`),
'''   KEY `compound_id` (`compound_id`),
'''   CONSTRAINT `FK_REFERENCE_TO_COMPOUND` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("reference", Database:="chebi", SchemaSQL:="
CREATE TABLE `reference` (
  `id` int(11) NOT NULL,
  `compound_id` int(11) NOT NULL,
  `reference_id` varchar(60) NOT NULL,
  `reference_db_name` varchar(60) NOT NULL,
  `location_in_ref` varchar(90) DEFAULT NULL,
  `reference_name` varchar(512) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `compound_id` (`compound_id`),
  CONSTRAINT `FK_REFERENCE_TO_COMPOUND` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class reference: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("compound_id"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="compound_id")> Public Property compound_id As Long
    <DatabaseField("reference_id"), NotNull, DataType(MySqlDbType.VarChar, "60"), Column(Name:="reference_id")> Public Property reference_id As String
    <DatabaseField("reference_db_name"), NotNull, DataType(MySqlDbType.VarChar, "60"), Column(Name:="reference_db_name")> Public Property reference_db_name As String
    <DatabaseField("location_in_ref"), DataType(MySqlDbType.VarChar, "90"), Column(Name:="location_in_ref")> Public Property location_in_ref As String
    <DatabaseField("reference_name"), DataType(MySqlDbType.VarChar, "512"), Column(Name:="reference_name")> Public Property reference_name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `reference` (`id`, `compound_id`, `reference_id`, `reference_db_name`, `location_in_ref`, `reference_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `reference` (`id`, `compound_id`, `reference_id`, `reference_db_name`, `location_in_ref`, `reference_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `reference` (`id`, `compound_id`, `reference_id`, `reference_db_name`, `location_in_ref`, `reference_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `reference` (`id`, `compound_id`, `reference_id`, `reference_db_name`, `location_in_ref`, `reference_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `reference` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `reference` SET `id`='{0}', `compound_id`='{1}', `reference_id`='{2}', `reference_db_name`='{3}', `location_in_ref`='{4}', `reference_name`='{5}' WHERE `id` = '{6}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `reference` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `reference` (`id`, `compound_id`, `reference_id`, `reference_db_name`, `location_in_ref`, `reference_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, compound_id, reference_id, reference_db_name, location_in_ref, reference_name)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `reference` (`id`, `compound_id`, `reference_id`, `reference_db_name`, `location_in_ref`, `reference_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, compound_id, reference_id, reference_db_name, location_in_ref, reference_name)
        Else
        Return String.Format(INSERT_SQL, id, compound_id, reference_id, reference_db_name, location_in_ref, reference_name)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{compound_id}', '{reference_id}', '{reference_db_name}', '{location_in_ref}', '{reference_name}')"
        Else
            Return $"('{id}', '{compound_id}', '{reference_id}', '{reference_db_name}', '{location_in_ref}', '{reference_name}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `reference` (`id`, `compound_id`, `reference_id`, `reference_db_name`, `location_in_ref`, `reference_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, compound_id, reference_id, reference_db_name, location_in_ref, reference_name)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `reference` (`id`, `compound_id`, `reference_id`, `reference_db_name`, `location_in_ref`, `reference_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, compound_id, reference_id, reference_db_name, location_in_ref, reference_name)
        Else
        Return String.Format(REPLACE_SQL, id, compound_id, reference_id, reference_db_name, location_in_ref, reference_name)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `reference` SET `id`='{0}', `compound_id`='{1}', `reference_id`='{2}', `reference_db_name`='{3}', `location_in_ref`='{4}', `reference_name`='{5}' WHERE `id` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, compound_id, reference_id, reference_db_name, location_in_ref, reference_name, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As reference
                         Return DirectCast(MyClass.MemberwiseClone, reference)
                     End Function
End Class


End Namespace
