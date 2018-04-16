#Region "Microsoft.VisualBasic::fec735b14653fad551a22aa92ce4bd8d, data\RegulonDatabase\Regtransbase\MySQL\h_dict_struct_site_types.vb"

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

    ' Class h_dict_struct_site_types
    ' 
    '     Properties: db_name, pkg_name
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @3/16/2018 10:40:17 PM


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
''' DROP TABLE IF EXISTS `h_dict_struct_site_types`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `h_dict_struct_site_types` (
'''   `pkg_name` varchar(100) NOT NULL DEFAULT '',
'''   `db_name` varchar(100) DEFAULT NULL,
'''   PRIMARY KEY (`pkg_name`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("h_dict_struct_site_types", Database:="dbregulation_update", SchemaSQL:="
CREATE TABLE `h_dict_struct_site_types` (
  `pkg_name` varchar(100) NOT NULL DEFAULT '',
  `db_name` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`pkg_name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class h_dict_struct_site_types: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pkg_name"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="pkg_name"), XmlAttribute> Public Property pkg_name As String
    <DatabaseField("db_name"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="db_name")> Public Property db_name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `h_dict_struct_site_types` (`pkg_name`, `db_name`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `h_dict_struct_site_types` (`pkg_name`, `db_name`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `h_dict_struct_site_types` WHERE `pkg_name` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `h_dict_struct_site_types` SET `pkg_name`='{0}', `db_name`='{1}' WHERE `pkg_name` = '{2}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `h_dict_struct_site_types` WHERE `pkg_name` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pkg_name)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `h_dict_struct_site_types` (`pkg_name`, `db_name`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pkg_name, db_name)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{pkg_name}', '{db_name}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `h_dict_struct_site_types` (`pkg_name`, `db_name`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pkg_name, db_name)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `h_dict_struct_site_types` SET `pkg_name`='{0}', `db_name`='{1}' WHERE `pkg_name` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pkg_name, db_name, pkg_name)
    End Function
#End Region
Public Function Clone() As h_dict_struct_site_types
                  Return DirectCast(MyClass.MemberwiseClone, h_dict_struct_site_types)
              End Function
End Class


End Namespace
