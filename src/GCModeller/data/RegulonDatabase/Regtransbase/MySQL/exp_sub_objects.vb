﻿#Region "Microsoft.VisualBasic::e03e5acd9d8702a3f288717fa375de13, data\RegulonDatabase\Regtransbase\MySQL\exp_sub_objects.vb"

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

    '   Total Lines: 180
    '    Code Lines: 91 (50.56%)
    ' Comment Lines: 67 (37.22%)
    '    - Xml Docs: 95.52%
    ' 
    '   Blank Lines: 22 (12.22%)
    '     File Size: 9.07 KB


    ' Class exp_sub_objects
    ' 
    '     Properties: art_guid, exp_guid, obj_guid, obj_type_id, order_num
    '                 pkg_guid, strand
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
''' DROP TABLE IF EXISTS `exp_sub_objects`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `exp_sub_objects` (
'''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
'''   `art_guid` int(11) NOT NULL DEFAULT '0',
'''   `exp_guid` int(11) NOT NULL DEFAULT '0',
'''   `obj_guid` int(11) NOT NULL DEFAULT '0',
'''   `obj_type_id` int(11) DEFAULT NULL,
'''   `order_num` int(11) DEFAULT NULL,
'''   `strand` int(1) DEFAULT NULL,
'''   PRIMARY KEY (`exp_guid`,`obj_guid`),
'''   KEY `FK_exp_sub_objects-pkg_guid` (`pkg_guid`),
'''   KEY `FK_exp_sub_objects-art_guid` (`art_guid`),
'''   KEY `obj_guid` (`obj_guid`),
'''   CONSTRAINT `FK_exp_sub_objects-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
'''   CONSTRAINT `FK_exp_sub_objects-exp_guid` FOREIGN KEY (`exp_guid`) REFERENCES `experiments` (`exp_guid`),
'''   CONSTRAINT `FK_exp_sub_objects-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
'''   CONSTRAINT `exp_sub_objects_ibfk_1` FOREIGN KEY (`obj_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("exp_sub_objects", Database:="dbregulation_update", SchemaSQL:="
CREATE TABLE `exp_sub_objects` (
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `exp_guid` int(11) NOT NULL DEFAULT '0',
  `obj_guid` int(11) NOT NULL DEFAULT '0',
  `obj_type_id` int(11) DEFAULT NULL,
  `order_num` int(11) DEFAULT NULL,
  `strand` int(1) DEFAULT NULL,
  PRIMARY KEY (`exp_guid`,`obj_guid`),
  KEY `FK_exp_sub_objects-pkg_guid` (`pkg_guid`),
  KEY `FK_exp_sub_objects-art_guid` (`art_guid`),
  KEY `obj_guid` (`obj_guid`),
  CONSTRAINT `FK_exp_sub_objects-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_exp_sub_objects-exp_guid` FOREIGN KEY (`exp_guid`) REFERENCES `experiments` (`exp_guid`),
  CONSTRAINT `FK_exp_sub_objects-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
  CONSTRAINT `exp_sub_objects_ibfk_1` FOREIGN KEY (`obj_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class exp_sub_objects: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pkg_guid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="pkg_guid")> Public Property pkg_guid As Long
    <DatabaseField("art_guid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="art_guid")> Public Property art_guid As Long
    <DatabaseField("exp_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="exp_guid"), XmlAttribute> Public Property exp_guid As Long
    <DatabaseField("obj_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="obj_guid"), XmlAttribute> Public Property obj_guid As Long
    <DatabaseField("obj_type_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="obj_type_id")> Public Property obj_type_id As Long
    <DatabaseField("order_num"), DataType(MySqlDbType.Int64, "11"), Column(Name:="order_num")> Public Property order_num As Long
    <DatabaseField("strand"), DataType(MySqlDbType.Int64, "1"), Column(Name:="strand")> Public Property strand As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `exp_sub_objects` (`pkg_guid`, `art_guid`, `exp_guid`, `obj_guid`, `obj_type_id`, `order_num`, `strand`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `exp_sub_objects` (`pkg_guid`, `art_guid`, `exp_guid`, `obj_guid`, `obj_type_id`, `order_num`, `strand`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `exp_sub_objects` (`pkg_guid`, `art_guid`, `exp_guid`, `obj_guid`, `obj_type_id`, `order_num`, `strand`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `exp_sub_objects` (`pkg_guid`, `art_guid`, `exp_guid`, `obj_guid`, `obj_type_id`, `order_num`, `strand`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `exp_sub_objects` WHERE `exp_guid`='{0}' and `obj_guid`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `exp_sub_objects` SET `pkg_guid`='{0}', `art_guid`='{1}', `exp_guid`='{2}', `obj_guid`='{3}', `obj_type_id`='{4}', `order_num`='{5}', `strand`='{6}' WHERE `exp_guid`='{7}' and `obj_guid`='{8}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `exp_sub_objects` WHERE `exp_guid`='{0}' and `obj_guid`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, exp_guid, obj_guid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `exp_sub_objects` (`pkg_guid`, `art_guid`, `exp_guid`, `obj_guid`, `obj_type_id`, `order_num`, `strand`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pkg_guid, art_guid, exp_guid, obj_guid, obj_type_id, order_num, strand)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `exp_sub_objects` (`pkg_guid`, `art_guid`, `exp_guid`, `obj_guid`, `obj_type_id`, `order_num`, `strand`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, pkg_guid, art_guid, exp_guid, obj_guid, obj_type_id, order_num, strand)
        Else
        Return String.Format(INSERT_SQL, pkg_guid, art_guid, exp_guid, obj_guid, obj_type_id, order_num, strand)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{pkg_guid}', '{art_guid}', '{exp_guid}', '{obj_guid}', '{obj_type_id}', '{order_num}', '{strand}')"
        Else
            Return $"('{pkg_guid}', '{art_guid}', '{exp_guid}', '{obj_guid}', '{obj_type_id}', '{order_num}', '{strand}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `exp_sub_objects` (`pkg_guid`, `art_guid`, `exp_guid`, `obj_guid`, `obj_type_id`, `order_num`, `strand`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pkg_guid, art_guid, exp_guid, obj_guid, obj_type_id, order_num, strand)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `exp_sub_objects` (`pkg_guid`, `art_guid`, `exp_guid`, `obj_guid`, `obj_type_id`, `order_num`, `strand`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, pkg_guid, art_guid, exp_guid, obj_guid, obj_type_id, order_num, strand)
        Else
        Return String.Format(REPLACE_SQL, pkg_guid, art_guid, exp_guid, obj_guid, obj_type_id, order_num, strand)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `exp_sub_objects` SET `pkg_guid`='{0}', `art_guid`='{1}', `exp_guid`='{2}', `obj_guid`='{3}', `obj_type_id`='{4}', `order_num`='{5}', `strand`='{6}' WHERE `exp_guid`='{7}' and `obj_guid`='{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pkg_guid, art_guid, exp_guid, obj_guid, obj_type_id, order_num, strand, exp_guid, obj_guid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As exp_sub_objects
                         Return DirectCast(MyClass.MemberwiseClone, exp_sub_objects)
                     End Function
End Class


End Namespace
