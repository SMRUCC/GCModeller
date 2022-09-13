#Region "Microsoft.VisualBasic::54f424e9b2f75af22b9aecf1c47ee21c, GCModeller\data\RegulonDatabase\Regtransbase\MySQL\obj_sub_types.vb"

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

    '   Total Lines: 149
    '    Code Lines: 73
    ' Comment Lines: 54
    '   Blank Lines: 22
    '     File Size: 5.68 KB


    ' Class obj_sub_types
    ' 
    '     Properties: child_obj_type_id, parent_obj_type_id
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
''' DROP TABLE IF EXISTS `obj_sub_types`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `obj_sub_types` (
'''   `parent_obj_type_id` int(11) NOT NULL DEFAULT '0',
'''   `child_obj_type_id` int(11) NOT NULL DEFAULT '0'
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("obj_sub_types", Database:="dbregulation_update", SchemaSQL:="
CREATE TABLE `obj_sub_types` (
  `parent_obj_type_id` int(11) NOT NULL DEFAULT '0',
  `child_obj_type_id` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class obj_sub_types: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("parent_obj_type_id"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="parent_obj_type_id")> Public Property parent_obj_type_id As Long
    <DatabaseField("child_obj_type_id"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="child_obj_type_id")> Public Property child_obj_type_id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `obj_sub_types` (`parent_obj_type_id`, `child_obj_type_id`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `obj_sub_types` (`parent_obj_type_id`, `child_obj_type_id`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `obj_sub_types` (`parent_obj_type_id`, `child_obj_type_id`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `obj_sub_types` (`parent_obj_type_id`, `child_obj_type_id`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `obj_sub_types` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `obj_sub_types` SET `parent_obj_type_id`='{0}', `child_obj_type_id`='{1}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `obj_sub_types` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `obj_sub_types` (`parent_obj_type_id`, `child_obj_type_id`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, parent_obj_type_id, child_obj_type_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `obj_sub_types` (`parent_obj_type_id`, `child_obj_type_id`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, parent_obj_type_id, child_obj_type_id)
        Else
        Return String.Format(INSERT_SQL, parent_obj_type_id, child_obj_type_id)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{parent_obj_type_id}', '{child_obj_type_id}')"
        Else
            Return $"('{parent_obj_type_id}', '{child_obj_type_id}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `obj_sub_types` (`parent_obj_type_id`, `child_obj_type_id`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, parent_obj_type_id, child_obj_type_id)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `obj_sub_types` (`parent_obj_type_id`, `child_obj_type_id`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, parent_obj_type_id, child_obj_type_id)
        Else
        Return String.Format(REPLACE_SQL, parent_obj_type_id, child_obj_type_id)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `obj_sub_types` SET `parent_obj_type_id`='{0}', `child_obj_type_id`='{1}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As obj_sub_types
                         Return DirectCast(MyClass.MemberwiseClone, obj_sub_types)
                     End Function
End Class


End Namespace
