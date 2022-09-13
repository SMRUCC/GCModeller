#Region "Microsoft.VisualBasic::7c80001618054320267cc6baef1cec9b, GCModeller\data\RegulonDatabase\Regtransbase\MySQL\dict_effectors_superclasses.vb"

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

    '   Total Lines: 163
    '    Code Lines: 82
    ' Comment Lines: 59
    '   Blank Lines: 22
    '     File Size: 7.90 KB


    ' Class dict_effectors_superclasses
    ' 
    '     Properties: effector_superclass_guid, idx, left_idx, name, parent_guid
    '                 right_idx
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
''' DROP TABLE IF EXISTS `dict_effectors_superclasses`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `dict_effectors_superclasses` (
'''   `effector_superclass_guid` int(11) NOT NULL DEFAULT '0',
'''   `name` varchar(100) NOT NULL DEFAULT '',
'''   `parent_guid` int(11) DEFAULT NULL,
'''   `idx` int(11) NOT NULL DEFAULT '0',
'''   `left_idx` int(11) NOT NULL DEFAULT '0',
'''   `right_idx` int(11) NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`effector_superclass_guid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("dict_effectors_superclasses", Database:="dbregulation_update", SchemaSQL:="
CREATE TABLE `dict_effectors_superclasses` (
  `effector_superclass_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(100) NOT NULL DEFAULT '',
  `parent_guid` int(11) DEFAULT NULL,
  `idx` int(11) NOT NULL DEFAULT '0',
  `left_idx` int(11) NOT NULL DEFAULT '0',
  `right_idx` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`effector_superclass_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class dict_effectors_superclasses: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("effector_superclass_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="effector_superclass_guid"), XmlAttribute> Public Property effector_superclass_guid As Long
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="name")> Public Property name As String
    <DatabaseField("parent_guid"), DataType(MySqlDbType.Int64, "11"), Column(Name:="parent_guid")> Public Property parent_guid As Long
    <DatabaseField("idx"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="idx")> Public Property idx As Long
    <DatabaseField("left_idx"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="left_idx")> Public Property left_idx As Long
    <DatabaseField("right_idx"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="right_idx")> Public Property right_idx As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `dict_effectors_superclasses` (`effector_superclass_guid`, `name`, `parent_guid`, `idx`, `left_idx`, `right_idx`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `dict_effectors_superclasses` (`effector_superclass_guid`, `name`, `parent_guid`, `idx`, `left_idx`, `right_idx`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `dict_effectors_superclasses` (`effector_superclass_guid`, `name`, `parent_guid`, `idx`, `left_idx`, `right_idx`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `dict_effectors_superclasses` (`effector_superclass_guid`, `name`, `parent_guid`, `idx`, `left_idx`, `right_idx`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `dict_effectors_superclasses` WHERE `effector_superclass_guid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `dict_effectors_superclasses` SET `effector_superclass_guid`='{0}', `name`='{1}', `parent_guid`='{2}', `idx`='{3}', `left_idx`='{4}', `right_idx`='{5}' WHERE `effector_superclass_guid` = '{6}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `dict_effectors_superclasses` WHERE `effector_superclass_guid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, effector_superclass_guid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `dict_effectors_superclasses` (`effector_superclass_guid`, `name`, `parent_guid`, `idx`, `left_idx`, `right_idx`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, effector_superclass_guid, name, parent_guid, idx, left_idx, right_idx)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `dict_effectors_superclasses` (`effector_superclass_guid`, `name`, `parent_guid`, `idx`, `left_idx`, `right_idx`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, effector_superclass_guid, name, parent_guid, idx, left_idx, right_idx)
        Else
        Return String.Format(INSERT_SQL, effector_superclass_guid, name, parent_guid, idx, left_idx, right_idx)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{effector_superclass_guid}', '{name}', '{parent_guid}', '{idx}', '{left_idx}', '{right_idx}')"
        Else
            Return $"('{effector_superclass_guid}', '{name}', '{parent_guid}', '{idx}', '{left_idx}', '{right_idx}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `dict_effectors_superclasses` (`effector_superclass_guid`, `name`, `parent_guid`, `idx`, `left_idx`, `right_idx`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, effector_superclass_guid, name, parent_guid, idx, left_idx, right_idx)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `dict_effectors_superclasses` (`effector_superclass_guid`, `name`, `parent_guid`, `idx`, `left_idx`, `right_idx`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, effector_superclass_guid, name, parent_guid, idx, left_idx, right_idx)
        Else
        Return String.Format(REPLACE_SQL, effector_superclass_guid, name, parent_guid, idx, left_idx, right_idx)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `dict_effectors_superclasses` SET `effector_superclass_guid`='{0}', `name`='{1}', `parent_guid`='{2}', `idx`='{3}', `left_idx`='{4}', `right_idx`='{5}' WHERE `effector_superclass_guid` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, effector_superclass_guid, name, parent_guid, idx, left_idx, right_idx, effector_superclass_guid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As dict_effectors_superclasses
                         Return DirectCast(MyClass.MemberwiseClone, dict_effectors_superclasses)
                     End Function
End Class


End Namespace
