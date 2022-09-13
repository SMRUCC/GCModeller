#Region "Microsoft.VisualBasic::8267b275f20778141cb710e2a68962a0, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\product.vb"

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

    '   Total Lines: 176
    '    Code Lines: 91
    ' Comment Lines: 63
    '   Blank Lines: 22
    '     File Size: 10.78 KB


    ' Class product
    ' 
    '     Properties: anticodon, isoelectric_point, key_id_org, location, molecular_weigth
    '                 product_id, product_internal_comment, product_name, product_note, product_sequence
    '                 product_type
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
''' DROP TABLE IF EXISTS `product`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `product` (
'''   `product_id` char(12) NOT NULL,
'''   `product_name` varchar(255) DEFAULT NULL,
'''   `molecular_weigth` decimal(20,5) DEFAULT NULL,
'''   `isoelectric_point` decimal(20,10) DEFAULT NULL,
'''   `location` varchar(1000) DEFAULT NULL,
'''   `anticodon` varchar(10) DEFAULT NULL,
'''   `product_sequence` varchar(4000) DEFAULT NULL,
'''   `product_note` longtext,
'''   `product_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL,
'''   `product_type` varchar(25) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("product", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `product` (
  `product_id` char(12) NOT NULL,
  `product_name` varchar(255) DEFAULT NULL,
  `molecular_weigth` decimal(20,5) DEFAULT NULL,
  `isoelectric_point` decimal(20,10) DEFAULT NULL,
  `location` varchar(1000) DEFAULT NULL,
  `anticodon` varchar(10) DEFAULT NULL,
  `product_sequence` varchar(4000) DEFAULT NULL,
  `product_note` longtext,
  `product_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL,
  `product_type` varchar(25) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class product: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("product_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="product_id")> Public Property product_id As String
    <DatabaseField("product_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="product_name")> Public Property product_name As String
    <DatabaseField("molecular_weigth"), DataType(MySqlDbType.Decimal), Column(Name:="molecular_weigth")> Public Property molecular_weigth As Decimal
    <DatabaseField("isoelectric_point"), DataType(MySqlDbType.Decimal), Column(Name:="isoelectric_point")> Public Property isoelectric_point As Decimal
    <DatabaseField("location"), DataType(MySqlDbType.VarChar, "1000"), Column(Name:="location")> Public Property location As String
    <DatabaseField("anticodon"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="anticodon")> Public Property anticodon As String
    <DatabaseField("product_sequence"), DataType(MySqlDbType.VarChar, "4000"), Column(Name:="product_sequence")> Public Property product_sequence As String
    <DatabaseField("product_note"), DataType(MySqlDbType.Text), Column(Name:="product_note")> Public Property product_note As String
    <DatabaseField("product_internal_comment"), DataType(MySqlDbType.Text), Column(Name:="product_internal_comment")> Public Property product_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="key_id_org")> Public Property key_id_org As String
    <DatabaseField("product_type"), DataType(MySqlDbType.VarChar, "25"), Column(Name:="product_type")> Public Property product_type As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `product` (`product_id`, `product_name`, `molecular_weigth`, `isoelectric_point`, `location`, `anticodon`, `product_sequence`, `product_note`, `product_internal_comment`, `key_id_org`, `product_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `product` (`product_id`, `product_name`, `molecular_weigth`, `isoelectric_point`, `location`, `anticodon`, `product_sequence`, `product_note`, `product_internal_comment`, `key_id_org`, `product_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `product` (`product_id`, `product_name`, `molecular_weigth`, `isoelectric_point`, `location`, `anticodon`, `product_sequence`, `product_note`, `product_internal_comment`, `key_id_org`, `product_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `product` (`product_id`, `product_name`, `molecular_weigth`, `isoelectric_point`, `location`, `anticodon`, `product_sequence`, `product_note`, `product_internal_comment`, `key_id_org`, `product_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `product` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `product` SET `product_id`='{0}', `product_name`='{1}', `molecular_weigth`='{2}', `isoelectric_point`='{3}', `location`='{4}', `anticodon`='{5}', `product_sequence`='{6}', `product_note`='{7}', `product_internal_comment`='{8}', `key_id_org`='{9}', `product_type`='{10}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `product` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `product` (`product_id`, `product_name`, `molecular_weigth`, `isoelectric_point`, `location`, `anticodon`, `product_sequence`, `product_note`, `product_internal_comment`, `key_id_org`, `product_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, product_id, product_name, molecular_weigth, isoelectric_point, location, anticodon, product_sequence, product_note, product_internal_comment, key_id_org, product_type)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `product` (`product_id`, `product_name`, `molecular_weigth`, `isoelectric_point`, `location`, `anticodon`, `product_sequence`, `product_note`, `product_internal_comment`, `key_id_org`, `product_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, product_id, product_name, molecular_weigth, isoelectric_point, location, anticodon, product_sequence, product_note, product_internal_comment, key_id_org, product_type)
        Else
        Return String.Format(INSERT_SQL, product_id, product_name, molecular_weigth, isoelectric_point, location, anticodon, product_sequence, product_note, product_internal_comment, key_id_org, product_type)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{product_id}', '{product_name}', '{molecular_weigth}', '{isoelectric_point}', '{location}', '{anticodon}', '{product_sequence}', '{product_note}', '{product_internal_comment}', '{key_id_org}', '{product_type}')"
        Else
            Return $"('{product_id}', '{product_name}', '{molecular_weigth}', '{isoelectric_point}', '{location}', '{anticodon}', '{product_sequence}', '{product_note}', '{product_internal_comment}', '{key_id_org}', '{product_type}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `product` (`product_id`, `product_name`, `molecular_weigth`, `isoelectric_point`, `location`, `anticodon`, `product_sequence`, `product_note`, `product_internal_comment`, `key_id_org`, `product_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, product_id, product_name, molecular_weigth, isoelectric_point, location, anticodon, product_sequence, product_note, product_internal_comment, key_id_org, product_type)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `product` (`product_id`, `product_name`, `molecular_weigth`, `isoelectric_point`, `location`, `anticodon`, `product_sequence`, `product_note`, `product_internal_comment`, `key_id_org`, `product_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, product_id, product_name, molecular_weigth, isoelectric_point, location, anticodon, product_sequence, product_note, product_internal_comment, key_id_org, product_type)
        Else
        Return String.Format(REPLACE_SQL, product_id, product_name, molecular_weigth, isoelectric_point, location, anticodon, product_sequence, product_note, product_internal_comment, key_id_org, product_type)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `product` SET `product_id`='{0}', `product_name`='{1}', `molecular_weigth`='{2}', `isoelectric_point`='{3}', `location`='{4}', `anticodon`='{5}', `product_sequence`='{6}', `product_note`='{7}', `product_internal_comment`='{8}', `key_id_org`='{9}', `product_type`='{10}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As product
                         Return DirectCast(MyClass.MemberwiseClone, product)
                     End Function
End Class


End Namespace
