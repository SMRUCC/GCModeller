#Region "Microsoft.VisualBasic::0a89d14555cc5ac2bd806edbabf98cac, data\RegulonDatabase\Regtransbase\MySQL\obj_types.vb"

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

    ' Class obj_types
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 10:54:58 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

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
''' 
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
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin1;")>
Public Class obj_types: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property id As Long
    <DatabaseField("obj_type_name"), DataType(MySqlDbType.VarChar, "50")> Public Property obj_type_name As String
    <DatabaseField("obj_tbname"), DataType(MySqlDbType.VarChar, "50")> Public Property obj_tbname As String
    <DatabaseField("cp_order"), DataType(MySqlDbType.Int64, "11")> Public Property cp_order As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `obj_types` (`obj_type_name`, `obj_tbname`, `cp_order`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `obj_types` (`obj_type_name`, `obj_tbname`, `cp_order`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `obj_types` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `obj_types` SET `id`='{0}', `obj_type_name`='{1}', `obj_tbname`='{2}', `cp_order`='{3}' WHERE `id` = '{4}';</SQL>
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
''' INSERT INTO `obj_types` (`obj_type_name`, `obj_tbname`, `cp_order`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, obj_type_name, obj_tbname, cp_order)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{obj_type_name}', '{obj_tbname}', '{cp_order}', '{3}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `obj_types` (`obj_type_name`, `obj_tbname`, `cp_order`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, obj_type_name, obj_tbname, cp_order)
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
End Class


End Namespace
