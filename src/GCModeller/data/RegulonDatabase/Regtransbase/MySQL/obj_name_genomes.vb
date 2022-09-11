#Region "Microsoft.VisualBasic::ba8071b5e594869b72b4132d2983aa3d, GCModeller\data\RegulonDatabase\Regtransbase\MySQL\obj_name_genomes.vb"

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


    ' Code Statistics:

    '   Total Lines: 175
    '    Code Lines: 88
    ' Comment Lines: 65
    '   Blank Lines: 22
    '     File Size: 8.33 KB


    ' Class obj_name_genomes
    ' 
    '     Properties: art_guid, genome_guid, name, obj_guid, obj_type_id
    '                 pkg_guid
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

REM  Dump @2018/5/23 13:13:38


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace Regtransbase.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `obj_name_genomes`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `obj_name_genomes` (
'''   `obj_guid` int(11) NOT NULL DEFAULT '0',
'''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
'''   `art_guid` int(11) NOT NULL DEFAULT '0',
'''   `name` varchar(50) DEFAULT NULL,
'''   `genome_guid` int(11) DEFAULT NULL,
'''   `obj_type_id` int(11) NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`obj_guid`),
'''   KEY `FK_obj_name_genomes-pkg_guid` (`pkg_guid`),
'''   KEY `FK_obj_name_genomes-art_guid` (`art_guid`),
'''   KEY `FK_obj_name_genomes-genome_guid` (`genome_guid`),
'''   CONSTRAINT `FK_obj_name_genomes-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
'''   CONSTRAINT `FK_obj_name_genomes-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
'''   CONSTRAINT `FK_obj_name_genomes-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("obj_name_genomes", Database:="dbregulation_update", SchemaSQL:="
CREATE TABLE `obj_name_genomes` (
  `obj_guid` int(11) NOT NULL DEFAULT '0',
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(50) DEFAULT NULL,
  `genome_guid` int(11) DEFAULT NULL,
  `obj_type_id` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`obj_guid`),
  KEY `FK_obj_name_genomes-pkg_guid` (`pkg_guid`),
  KEY `FK_obj_name_genomes-art_guid` (`art_guid`),
  KEY `FK_obj_name_genomes-genome_guid` (`genome_guid`),
  CONSTRAINT `FK_obj_name_genomes-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_obj_name_genomes-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
  CONSTRAINT `FK_obj_name_genomes-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class obj_name_genomes: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("obj_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="obj_guid"), XmlAttribute> Public Property obj_guid As Long
    <DatabaseField("pkg_guid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="pkg_guid")> Public Property pkg_guid As Long
    <DatabaseField("art_guid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="art_guid")> Public Property art_guid As Long
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="name")> Public Property name As String
    <DatabaseField("genome_guid"), DataType(MySqlDbType.Int64, "11"), Column(Name:="genome_guid")> Public Property genome_guid As Long
    <DatabaseField("obj_type_id"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="obj_type_id")> Public Property obj_type_id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `obj_name_genomes` (`obj_guid`, `pkg_guid`, `art_guid`, `name`, `genome_guid`, `obj_type_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `obj_name_genomes` (`obj_guid`, `pkg_guid`, `art_guid`, `name`, `genome_guid`, `obj_type_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `obj_name_genomes` (`obj_guid`, `pkg_guid`, `art_guid`, `name`, `genome_guid`, `obj_type_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `obj_name_genomes` (`obj_guid`, `pkg_guid`, `art_guid`, `name`, `genome_guid`, `obj_type_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `obj_name_genomes` WHERE `obj_guid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `obj_name_genomes` SET `obj_guid`='{0}', `pkg_guid`='{1}', `art_guid`='{2}', `name`='{3}', `genome_guid`='{4}', `obj_type_id`='{5}' WHERE `obj_guid` = '{6}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `obj_name_genomes` WHERE `obj_guid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, obj_guid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `obj_name_genomes` (`obj_guid`, `pkg_guid`, `art_guid`, `name`, `genome_guid`, `obj_type_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, obj_guid, pkg_guid, art_guid, name, genome_guid, obj_type_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `obj_name_genomes` (`obj_guid`, `pkg_guid`, `art_guid`, `name`, `genome_guid`, `obj_type_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, obj_guid, pkg_guid, art_guid, name, genome_guid, obj_type_id)
        Else
        Return String.Format(INSERT_SQL, obj_guid, pkg_guid, art_guid, name, genome_guid, obj_type_id)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{obj_guid}', '{pkg_guid}', '{art_guid}', '{name}', '{genome_guid}', '{obj_type_id}')"
        Else
            Return $"('{obj_guid}', '{pkg_guid}', '{art_guid}', '{name}', '{genome_guid}', '{obj_type_id}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `obj_name_genomes` (`obj_guid`, `pkg_guid`, `art_guid`, `name`, `genome_guid`, `obj_type_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, obj_guid, pkg_guid, art_guid, name, genome_guid, obj_type_id)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `obj_name_genomes` (`obj_guid`, `pkg_guid`, `art_guid`, `name`, `genome_guid`, `obj_type_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, obj_guid, pkg_guid, art_guid, name, genome_guid, obj_type_id)
        Else
        Return String.Format(REPLACE_SQL, obj_guid, pkg_guid, art_guid, name, genome_guid, obj_type_id)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `obj_name_genomes` SET `obj_guid`='{0}', `pkg_guid`='{1}', `art_guid`='{2}', `name`='{3}', `genome_guid`='{4}', `obj_type_id`='{5}' WHERE `obj_guid` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, obj_guid, pkg_guid, art_guid, name, genome_guid, obj_type_id, obj_guid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As obj_name_genomes
                         Return DirectCast(MyClass.MemberwiseClone, obj_name_genomes)
                     End Function
End Class


End Namespace
