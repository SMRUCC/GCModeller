#Region "Microsoft.VisualBasic::12cbd7054b1bd8ed87e3fe8d3c05f4c6, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\product_fc_link.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 11:24:24 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace RegulonDB.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `product_fc_link`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `product_fc_link` (
'''   `product_id` char(12) NOT NULL,
'''   `functional_class_id` char(12) NOT NULL,
'''   `prod_fc_l_id` char(12) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("product_fc_link", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `product_fc_link` (
  `product_id` char(12) NOT NULL,
  `functional_class_id` char(12) NOT NULL,
  `prod_fc_l_id` char(12) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class product_fc_link: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("product_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property product_id As String
    <DatabaseField("functional_class_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property functional_class_id As String
    <DatabaseField("prod_fc_l_id"), DataType(MySqlDbType.VarChar, "12")> Public Property prod_fc_l_id As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `product_fc_link` (`product_id`, `functional_class_id`, `prod_fc_l_id`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `product_fc_link` (`product_id`, `functional_class_id`, `prod_fc_l_id`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `product_fc_link` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `product_fc_link` SET `product_id`='{0}', `functional_class_id`='{1}', `prod_fc_l_id`='{2}' WHERE ;</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `product_fc_link` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `product_fc_link` (`product_id`, `functional_class_id`, `prod_fc_l_id`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, product_id, functional_class_id, prod_fc_l_id)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{product_id}', '{functional_class_id}', '{prod_fc_l_id}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `product_fc_link` (`product_id`, `functional_class_id`, `prod_fc_l_id`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, product_id, functional_class_id, prod_fc_l_id)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `product_fc_link` SET `product_id`='{0}', `functional_class_id`='{1}', `prod_fc_l_id`='{2}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
