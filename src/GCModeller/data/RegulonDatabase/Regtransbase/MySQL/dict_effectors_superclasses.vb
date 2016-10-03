#Region "Microsoft.VisualBasic::df2f46a9a686498f07258b4b219e5e2a, ..\GCModeller\data\RegulonDatabase\Regtransbase\MySQL\dict_effectors_superclasses.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:07:30 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Regtransbase.MySQL

''' <summary>
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
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("dict_effectors_superclasses", Database:="dbregulation_update")>
Public Class dict_effectors_superclasses: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("effector_superclass_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property effector_superclass_guid As Long
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property name As String
    <DatabaseField("parent_guid"), DataType(MySqlDbType.Int64, "11")> Public Property parent_guid As Long
    <DatabaseField("idx"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property idx As Long
    <DatabaseField("left_idx"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property left_idx As Long
    <DatabaseField("right_idx"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property right_idx As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `dict_effectors_superclasses` (`effector_superclass_guid`, `name`, `parent_guid`, `idx`, `left_idx`, `right_idx`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `dict_effectors_superclasses` (`effector_superclass_guid`, `name`, `parent_guid`, `idx`, `left_idx`, `right_idx`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `dict_effectors_superclasses` WHERE `effector_superclass_guid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `dict_effectors_superclasses` SET `effector_superclass_guid`='{0}', `name`='{1}', `parent_guid`='{2}', `idx`='{3}', `left_idx`='{4}', `right_idx`='{5}' WHERE `effector_superclass_guid` = '{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, effector_superclass_guid)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, effector_superclass_guid, name, parent_guid, idx, left_idx, right_idx)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, effector_superclass_guid, name, parent_guid, idx, left_idx, right_idx)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, effector_superclass_guid, name, parent_guid, idx, left_idx, right_idx, effector_superclass_guid)
    End Function
#End Region
End Class


End Namespace
