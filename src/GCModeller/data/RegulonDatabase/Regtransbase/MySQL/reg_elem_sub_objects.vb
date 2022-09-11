#Region "Microsoft.VisualBasic::ec812a72894c026a0d5027d1f0225ac8, GCModeller\data\RegulonDatabase\Regtransbase\MySQL\reg_elem_sub_objects.vb"

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

    '   Total Lines: 183
    '    Code Lines: 93
    ' Comment Lines: 68
    '   Blank Lines: 22
    '     File Size: 10.04 KB


    ' Class reg_elem_sub_objects
    ' 
    '     Properties: art_guid, child_guid, child_n, child_type_id, parent_guid
    '                 parent_type_id, pkg_guid, strand
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
''' DROP TABLE IF EXISTS `reg_elem_sub_objects`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `reg_elem_sub_objects` (
'''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
'''   `art_guid` int(11) NOT NULL DEFAULT '0',
'''   `parent_guid` int(11) NOT NULL DEFAULT '0',
'''   `parent_type_id` int(11) DEFAULT NULL,
'''   `child_guid` int(11) NOT NULL DEFAULT '0',
'''   `child_type_id` int(11) DEFAULT NULL,
'''   `child_n` int(11) DEFAULT NULL,
'''   `strand` int(1) DEFAULT NULL,
'''   PRIMARY KEY (`parent_guid`,`child_guid`),
'''   KEY `FK_reg_elem_sub_objects-pkg_guid` (`pkg_guid`),
'''   KEY `FK_reg_elem_sub_objects-art_guid` (`art_guid`),
'''   KEY `child_guid` (`child_guid`),
'''   CONSTRAINT `FK_reg_elem_sub_objects-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
'''   CONSTRAINT `FK_reg_elem_sub_objects-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
'''   CONSTRAINT `reg_elem_sub_objects_ibfk_1` FOREIGN KEY (`child_guid`) REFERENCES `obj_name_genomes` (`obj_guid`),
'''   CONSTRAINT `reg_elem_sub_objects_ibfk_2` FOREIGN KEY (`parent_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("reg_elem_sub_objects", Database:="dbregulation_update", SchemaSQL:="
CREATE TABLE `reg_elem_sub_objects` (
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `parent_guid` int(11) NOT NULL DEFAULT '0',
  `parent_type_id` int(11) DEFAULT NULL,
  `child_guid` int(11) NOT NULL DEFAULT '0',
  `child_type_id` int(11) DEFAULT NULL,
  `child_n` int(11) DEFAULT NULL,
  `strand` int(1) DEFAULT NULL,
  PRIMARY KEY (`parent_guid`,`child_guid`),
  KEY `FK_reg_elem_sub_objects-pkg_guid` (`pkg_guid`),
  KEY `FK_reg_elem_sub_objects-art_guid` (`art_guid`),
  KEY `child_guid` (`child_guid`),
  CONSTRAINT `FK_reg_elem_sub_objects-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_reg_elem_sub_objects-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
  CONSTRAINT `reg_elem_sub_objects_ibfk_1` FOREIGN KEY (`child_guid`) REFERENCES `obj_name_genomes` (`obj_guid`),
  CONSTRAINT `reg_elem_sub_objects_ibfk_2` FOREIGN KEY (`parent_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class reg_elem_sub_objects: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pkg_guid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="pkg_guid")> Public Property pkg_guid As Long
    <DatabaseField("art_guid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="art_guid")> Public Property art_guid As Long
    <DatabaseField("parent_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="parent_guid"), XmlAttribute> Public Property parent_guid As Long
    <DatabaseField("parent_type_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="parent_type_id")> Public Property parent_type_id As Long
    <DatabaseField("child_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="child_guid"), XmlAttribute> Public Property child_guid As Long
    <DatabaseField("child_type_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="child_type_id")> Public Property child_type_id As Long
    <DatabaseField("child_n"), DataType(MySqlDbType.Int64, "11"), Column(Name:="child_n")> Public Property child_n As Long
    <DatabaseField("strand"), DataType(MySqlDbType.Int64, "1"), Column(Name:="strand")> Public Property strand As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `reg_elem_sub_objects` (`pkg_guid`, `art_guid`, `parent_guid`, `parent_type_id`, `child_guid`, `child_type_id`, `child_n`, `strand`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `reg_elem_sub_objects` (`pkg_guid`, `art_guid`, `parent_guid`, `parent_type_id`, `child_guid`, `child_type_id`, `child_n`, `strand`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `reg_elem_sub_objects` (`pkg_guid`, `art_guid`, `parent_guid`, `parent_type_id`, `child_guid`, `child_type_id`, `child_n`, `strand`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `reg_elem_sub_objects` (`pkg_guid`, `art_guid`, `parent_guid`, `parent_type_id`, `child_guid`, `child_type_id`, `child_n`, `strand`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `reg_elem_sub_objects` WHERE `parent_guid`='{0}' and `child_guid`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `reg_elem_sub_objects` SET `pkg_guid`='{0}', `art_guid`='{1}', `parent_guid`='{2}', `parent_type_id`='{3}', `child_guid`='{4}', `child_type_id`='{5}', `child_n`='{6}', `strand`='{7}' WHERE `parent_guid`='{8}' and `child_guid`='{9}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `reg_elem_sub_objects` WHERE `parent_guid`='{0}' and `child_guid`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, parent_guid, child_guid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `reg_elem_sub_objects` (`pkg_guid`, `art_guid`, `parent_guid`, `parent_type_id`, `child_guid`, `child_type_id`, `child_n`, `strand`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pkg_guid, art_guid, parent_guid, parent_type_id, child_guid, child_type_id, child_n, strand)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `reg_elem_sub_objects` (`pkg_guid`, `art_guid`, `parent_guid`, `parent_type_id`, `child_guid`, `child_type_id`, `child_n`, `strand`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, pkg_guid, art_guid, parent_guid, parent_type_id, child_guid, child_type_id, child_n, strand)
        Else
        Return String.Format(INSERT_SQL, pkg_guid, art_guid, parent_guid, parent_type_id, child_guid, child_type_id, child_n, strand)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{pkg_guid}', '{art_guid}', '{parent_guid}', '{parent_type_id}', '{child_guid}', '{child_type_id}', '{child_n}', '{strand}')"
        Else
            Return $"('{pkg_guid}', '{art_guid}', '{parent_guid}', '{parent_type_id}', '{child_guid}', '{child_type_id}', '{child_n}', '{strand}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `reg_elem_sub_objects` (`pkg_guid`, `art_guid`, `parent_guid`, `parent_type_id`, `child_guid`, `child_type_id`, `child_n`, `strand`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pkg_guid, art_guid, parent_guid, parent_type_id, child_guid, child_type_id, child_n, strand)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `reg_elem_sub_objects` (`pkg_guid`, `art_guid`, `parent_guid`, `parent_type_id`, `child_guid`, `child_type_id`, `child_n`, `strand`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, pkg_guid, art_guid, parent_guid, parent_type_id, child_guid, child_type_id, child_n, strand)
        Else
        Return String.Format(REPLACE_SQL, pkg_guid, art_guid, parent_guid, parent_type_id, child_guid, child_type_id, child_n, strand)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `reg_elem_sub_objects` SET `pkg_guid`='{0}', `art_guid`='{1}', `parent_guid`='{2}', `parent_type_id`='{3}', `child_guid`='{4}', `child_type_id`='{5}', `child_n`='{6}', `strand`='{7}' WHERE `parent_guid`='{8}' and `child_guid`='{9}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pkg_guid, art_guid, parent_guid, parent_type_id, child_guid, child_type_id, child_n, strand, parent_guid, child_guid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As reg_elem_sub_objects
                         Return DirectCast(MyClass.MemberwiseClone, reg_elem_sub_objects)
                     End Function
End Class


End Namespace
