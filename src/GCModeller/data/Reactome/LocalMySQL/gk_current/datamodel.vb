#Region "Microsoft.VisualBasic::b927d43c55cd6e83fe8c74b8dca9f3cb, data\Reactome\LocalMySQL\gk_current\datamodel.vb"

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

    ' Class datamodel
    ' 
    '     Properties: property_name, property_value, property_value_rank, property_value_type, thing
    '                 thing_class
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @3/16/2018 10:40:21 PM


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' ```SQL
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
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("datamodel", Database:="gk_current", SchemaSQL:="
CREATE TABLE `datamodel` (
  `thing` varchar(255) NOT NULL,
  `thing_class` enum('SchemaClass','SchemaClassAttribute','Schema') DEFAULT NULL,
  `property_name` varchar(255) NOT NULL,
  `property_value` text,
  `property_value_type` enum('INTEGER','SYMBOL','STRING','INSTANCE','SchemaClass','SchemaClassAttribute') DEFAULT NULL,
  `property_value_rank` int(10) unsigned DEFAULT '0'
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class datamodel: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("thing"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="thing")> Public Property thing As String
    <DatabaseField("thing_class"), DataType(MySqlDbType.String), Column(Name:="thing_class")> Public Property thing_class As String
    <DatabaseField("property_name"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="property_name")> Public Property property_name As String
    <DatabaseField("property_value"), DataType(MySqlDbType.Text), Column(Name:="property_value")> Public Property property_value As String
    <DatabaseField("property_value_type"), DataType(MySqlDbType.String), Column(Name:="property_value_type")> Public Property property_value_type As String
    <DatabaseField("property_value_rank"), DataType(MySqlDbType.Int64, "10"), Column(Name:="property_value_rank")> Public Property property_value_rank As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `datamodel` (`thing`, `thing_class`, `property_name`, `property_value`, `property_value_type`, `property_value_rank`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `datamodel` (`thing`, `thing_class`, `property_name`, `property_value`, `property_value_type`, `property_value_rank`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `datamodel` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `datamodel` SET `thing`='{0}', `thing_class`='{1}', `property_name`='{2}', `property_value`='{3}', `property_value_type`='{4}', `property_value_rank`='{5}' WHERE ;</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `datamodel` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `datamodel` (`thing`, `thing_class`, `property_name`, `property_value`, `property_value_type`, `property_value_rank`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, thing, thing_class, property_name, property_value, property_value_type, property_value_rank)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{thing}', '{thing_class}', '{property_name}', '{property_value}', '{property_value_type}', '{property_value_rank}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `datamodel` (`thing`, `thing_class`, `property_name`, `property_value`, `property_value_type`, `property_value_rank`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, thing, thing_class, property_name, property_value, property_value_type, property_value_rank)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `datamodel` SET `thing`='{0}', `thing_class`='{1}', `property_name`='{2}', `property_value`='{3}', `property_value_type`='{4}', `property_value_rank`='{5}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
Public Function Clone() As datamodel
                  Return DirectCast(MyClass.MemberwiseClone, datamodel)
              End Function
End Class


End Namespace
