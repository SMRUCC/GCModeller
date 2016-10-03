#Region "Microsoft.VisualBasic::07c8f2e0f70b4e212d72a1450428e7ce, ..\GCModeller\data\Reactome\LocalMySQL\gk_current\datamodel.vb"

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

REM  Dump @12/3/2015 8:15:49 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `datamodel`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `datamodel` (
'''   `thing` varchar(255) NOT NULL,
'''   `thing_class` enum('SchemaClass','SchemaClassAttribute','Schema') DEFAULT NULL,
'''   `property_name` varchar(255) NOT NULL,
'''   `property_value` text,
'''   `property_value_type` enum('INTEGER','SYMBOL','STRING','INSTANCE','SchemaClass','SchemaClassAttribute') DEFAULT NULL,
'''   `property_value_rank` int(10) unsigned DEFAULT '0'
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("datamodel", Database:="gk_current")>
Public Class datamodel: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("thing"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property thing As String
    <DatabaseField("thing_class"), DataType(MySqlDbType.String)> Public Property thing_class As String
    <DatabaseField("property_name"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property property_name As String
    <DatabaseField("property_value"), DataType(MySqlDbType.Text)> Public Property property_value As String
    <DatabaseField("property_value_type"), DataType(MySqlDbType.String)> Public Property property_value_type As String
    <DatabaseField("property_value_rank"), DataType(MySqlDbType.Int64, "10")> Public Property property_value_rank As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `datamodel` (`thing`, `thing_class`, `property_name`, `property_value`, `property_value_type`, `property_value_rank`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `datamodel` (`thing`, `thing_class`, `property_name`, `property_value`, `property_value_type`, `property_value_rank`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `datamodel` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `datamodel` SET `thing`='{0}', `thing_class`='{1}', `property_name`='{2}', `property_value`='{3}', `property_value_type`='{4}', `property_value_rank`='{5}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, thing, thing_class, property_name, property_value, property_value_type, property_value_rank)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, thing, thing_class, property_name, property_value, property_value_type, property_value_rank)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
