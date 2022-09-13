#Region "Microsoft.VisualBasic::8ecf6e091821957b8be5210fc25bfef5, GCModeller\data\RegulonDatabase\Regtransbase\MySQL\obj_types.vb"

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

    '   Total Lines: 157
    '    Code Lines: 78
    ' Comment Lines: 57
    '   Blank Lines: 22
    '     File Size: 6.08 KB


    ' Class obj_types
    ' 
    '     Properties: cp_order, id, obj_tbname, obj_type_name
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
''' DROP TABLE IF EXISTS `obj_types`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `obj_types` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `obj_type_name` varchar(50) DEFAULT NULL,
'''   `obj_tbname` varchar(50) DEFAULT NULL,
'''   `cp_order` int(11) DEFAULT NULL,
'''   PRIMARY KEY (`id`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("obj_types", Database:="dbregulation_update", SchemaSQL:="
CREATE TABLE `obj_types` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `obj_type_name` varchar(50) DEFAULT NULL,
  `obj_tbname` varchar(50) DEFAULT NULL,
  `cp_order` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin1;")>
Public Class obj_types: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("obj_type_name"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="obj_type_name")> Public Property obj_type_name As String
    <DatabaseField("obj_tbname"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="obj_tbname")> Public Property obj_tbname As String
    <DatabaseField("cp_order"), DataType(MySqlDbType.Int64, "11"), Column(Name:="cp_order")> Public Property cp_order As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `obj_types` (`obj_type_name`, `obj_tbname`, `cp_order`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `obj_types` (`id`, `obj_type_name`, `obj_tbname`, `cp_order`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `obj_types` (`obj_type_name`, `obj_tbname`, `cp_order`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `obj_types` (`id`, `obj_type_name`, `obj_tbname`, `cp_order`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `obj_types` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `obj_types` SET `id`='{0}', `obj_type_name`='{1}', `obj_tbname`='{2}', `cp_order`='{3}' WHERE `id` = '{4}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `obj_types` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `obj_types` (`id`, `obj_type_name`, `obj_tbname`, `cp_order`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, obj_type_name, obj_tbname, cp_order)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `obj_types` (`id`, `obj_type_name`, `obj_tbname`, `cp_order`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, obj_type_name, obj_tbname, cp_order)
        Else
        Return String.Format(INSERT_SQL, obj_type_name, obj_tbname, cp_order)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{obj_type_name}', '{obj_tbname}', '{cp_order}')"
        Else
            Return $"('{obj_type_name}', '{obj_tbname}', '{cp_order}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `obj_types` (`id`, `obj_type_name`, `obj_tbname`, `cp_order`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, obj_type_name, obj_tbname, cp_order)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `obj_types` (`id`, `obj_type_name`, `obj_tbname`, `cp_order`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, obj_type_name, obj_tbname, cp_order)
        Else
        Return String.Format(REPLACE_SQL, obj_type_name, obj_tbname, cp_order)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `obj_types` SET `id`='{0}', `obj_type_name`='{1}', `obj_tbname`='{2}', `cp_order`='{3}' WHERE `id` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, obj_type_name, obj_tbname, cp_order, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As obj_types
                         Return DirectCast(MyClass.MemberwiseClone, obj_types)
                     End Function
End Class


End Namespace
