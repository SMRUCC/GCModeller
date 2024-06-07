﻿#Region "Microsoft.VisualBasic::3e5242765f657b880669bf498141f6fe, data\RegulonDatabase\Regtransbase\MySQL\dict_regulator_types.vb"

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

    '   Total Lines: 151
    '    Code Lines: 74 (49.01%)
    ' Comment Lines: 55 (36.42%)
    '    - Xml Docs: 94.55%
    ' 
    '   Blank Lines: 22 (14.57%)
    '     File Size: 5.60 KB


    ' Class dict_regulator_types
    ' 
    '     Properties: name, regulator_type_guid
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
''' DROP TABLE IF EXISTS `dict_regulator_types`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `dict_regulator_types` (
'''   `regulator_type_guid` int(11) NOT NULL DEFAULT '0',
'''   `name` varchar(100) NOT NULL DEFAULT '',
'''   PRIMARY KEY (`regulator_type_guid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("dict_regulator_types", Database:="dbregulation_update", SchemaSQL:="
CREATE TABLE `dict_regulator_types` (
  `regulator_type_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(100) NOT NULL DEFAULT '',
  PRIMARY KEY (`regulator_type_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class dict_regulator_types: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("regulator_type_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="regulator_type_guid"), XmlAttribute> Public Property regulator_type_guid As Long
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="name")> Public Property name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `dict_regulator_types` (`regulator_type_guid`, `name`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `dict_regulator_types` (`regulator_type_guid`, `name`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `dict_regulator_types` (`regulator_type_guid`, `name`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `dict_regulator_types` (`regulator_type_guid`, `name`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `dict_regulator_types` WHERE `regulator_type_guid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `dict_regulator_types` SET `regulator_type_guid`='{0}', `name`='{1}' WHERE `regulator_type_guid` = '{2}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `dict_regulator_types` WHERE `regulator_type_guid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, regulator_type_guid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `dict_regulator_types` (`regulator_type_guid`, `name`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, regulator_type_guid, name)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `dict_regulator_types` (`regulator_type_guid`, `name`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, regulator_type_guid, name)
        Else
        Return String.Format(INSERT_SQL, regulator_type_guid, name)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{regulator_type_guid}', '{name}')"
        Else
            Return $"('{regulator_type_guid}', '{name}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `dict_regulator_types` (`regulator_type_guid`, `name`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, regulator_type_guid, name)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `dict_regulator_types` (`regulator_type_guid`, `name`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, regulator_type_guid, name)
        Else
        Return String.Format(REPLACE_SQL, regulator_type_guid, name)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `dict_regulator_types` SET `regulator_type_guid`='{0}', `name`='{1}' WHERE `regulator_type_guid` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, regulator_type_guid, name, regulator_type_guid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As dict_regulator_types
                         Return DirectCast(MyClass.MemberwiseClone, dict_regulator_types)
                     End Function
End Class


End Namespace
