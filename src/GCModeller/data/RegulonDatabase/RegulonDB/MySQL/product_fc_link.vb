﻿#Region "Microsoft.VisualBasic::5d00338e4bda31d9b0bc1f3dda52d4bd, data\RegulonDatabase\RegulonDB\MySQL\product_fc_link.vb"

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

    '   Total Lines: 152
    '    Code Lines: 75 (49.34%)
    ' Comment Lines: 55 (36.18%)
    '    - Xml Docs: 94.55%
    ' 
    '   Blank Lines: 22 (14.47%)
    '     File Size: 6.11 KB


    ' Class product_fc_link
    ' 
    '     Properties: functional_class_id, prod_fc_l_id, product_id
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

REM  Dump @2018/5/23 13:13:36


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

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
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("product_fc_link", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `product_fc_link` (
  `product_id` char(12) NOT NULL,
  `functional_class_id` char(12) NOT NULL,
  `prod_fc_l_id` char(12) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class product_fc_link: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("product_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="product_id")> Public Property product_id As String
    <DatabaseField("functional_class_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="functional_class_id")> Public Property functional_class_id As String
    <DatabaseField("prod_fc_l_id"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="prod_fc_l_id")> Public Property prod_fc_l_id As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `product_fc_link` (`product_id`, `functional_class_id`, `prod_fc_l_id`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `product_fc_link` (`product_id`, `functional_class_id`, `prod_fc_l_id`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `product_fc_link` (`product_id`, `functional_class_id`, `prod_fc_l_id`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `product_fc_link` (`product_id`, `functional_class_id`, `prod_fc_l_id`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `product_fc_link` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `product_fc_link` SET `product_id`='{0}', `functional_class_id`='{1}', `prod_fc_l_id`='{2}' WHERE ;</SQL>

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
''' ```SQL
''' INSERT INTO `product_fc_link` (`product_id`, `functional_class_id`, `prod_fc_l_id`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, product_id, functional_class_id, prod_fc_l_id)
        Else
        Return String.Format(INSERT_SQL, product_id, functional_class_id, prod_fc_l_id)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{product_id}', '{functional_class_id}', '{prod_fc_l_id}')"
        Else
            Return $"('{product_id}', '{functional_class_id}', '{prod_fc_l_id}')"
        End If
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
''' REPLACE INTO `product_fc_link` (`product_id`, `functional_class_id`, `prod_fc_l_id`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, product_id, functional_class_id, prod_fc_l_id)
        Else
        Return String.Format(REPLACE_SQL, product_id, functional_class_id, prod_fc_l_id)
        End If
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

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As product_fc_link
                         Return DirectCast(MyClass.MemberwiseClone, product_fc_link)
                     End Function
End Class


End Namespace
