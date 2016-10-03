#Region "Microsoft.VisualBasic::4b603ad249deeb875207636d92f36e47, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\condition.vb"

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

REM  Dump @12/3/2015 8:08:18 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace RegulonDB.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `condition`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `condition` (
'''   `condition_id` char(12) NOT NULL,
'''   `control_condition` varchar(2000) NOT NULL,
'''   `control_details` varchar(2000) DEFAULT NULL,
'''   `exp_condition` varchar(2000) NOT NULL,
'''   `exp_details` varchar(2000) DEFAULT NULL,
'''   `condition_global` varchar(2000) DEFAULT NULL,
'''   `condition_notes` varchar(2000) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("condition", Database:="regulondb_7_5")>
Public Class condition: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("condition_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property condition_id As String
    <DatabaseField("control_condition"), NotNull, DataType(MySqlDbType.VarChar, "2000")> Public Property control_condition As String
    <DatabaseField("control_details"), DataType(MySqlDbType.VarChar, "2000")> Public Property control_details As String
    <DatabaseField("exp_condition"), NotNull, DataType(MySqlDbType.VarChar, "2000")> Public Property exp_condition As String
    <DatabaseField("exp_details"), DataType(MySqlDbType.VarChar, "2000")> Public Property exp_details As String
    <DatabaseField("condition_global"), DataType(MySqlDbType.VarChar, "2000")> Public Property condition_global As String
    <DatabaseField("condition_notes"), DataType(MySqlDbType.VarChar, "2000")> Public Property condition_notes As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `condition` (`condition_id`, `control_condition`, `control_details`, `exp_condition`, `exp_details`, `condition_global`, `condition_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `condition` (`condition_id`, `control_condition`, `control_details`, `exp_condition`, `exp_details`, `condition_global`, `condition_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `condition` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `condition` SET `condition_id`='{0}', `control_condition`='{1}', `control_details`='{2}', `exp_condition`='{3}', `exp_details`='{4}', `condition_global`='{5}', `condition_notes`='{6}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, condition_id, control_condition, control_details, exp_condition, exp_details, condition_global, condition_notes)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, condition_id, control_condition, control_details, exp_condition, exp_details, condition_global, condition_notes)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
