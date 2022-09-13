#Region "Microsoft.VisualBasic::7f2bd9ee31186f141f4cfa6d8ba0fc16, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\functional_class.vb"

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

    '   Total Lines: 170
    '    Code Lines: 87
    ' Comment Lines: 61
    '   Blank Lines: 22
    '     File Size: 9.67 KB


    ' Class functional_class
    ' 
    '     Properties: color_class, fc_description, fc_internal_comment, fc_label_index, fc_note
    '                 fc_reference, functional_class_id, head_class, key_id_org
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
''' DROP TABLE IF EXISTS `functional_class`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `functional_class` (
'''   `functional_class_id` char(12) NOT NULL,
'''   `fc_description` varchar(500) NOT NULL,
'''   `fc_label_index` varchar(50) NOT NULL,
'''   `head_class` char(12) DEFAULT NULL,
'''   `color_class` varchar(20) DEFAULT NULL,
'''   `fc_reference` varchar(255) NOT NULL,
'''   `fc_note` varchar(2000) DEFAULT NULL,
'''   `fc_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("functional_class", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `functional_class` (
  `functional_class_id` char(12) NOT NULL,
  `fc_description` varchar(500) NOT NULL,
  `fc_label_index` varchar(50) NOT NULL,
  `head_class` char(12) DEFAULT NULL,
  `color_class` varchar(20) DEFAULT NULL,
  `fc_reference` varchar(255) NOT NULL,
  `fc_note` varchar(2000) DEFAULT NULL,
  `fc_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class functional_class: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("functional_class_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="functional_class_id")> Public Property functional_class_id As String
    <DatabaseField("fc_description"), NotNull, DataType(MySqlDbType.VarChar, "500"), Column(Name:="fc_description")> Public Property fc_description As String
    <DatabaseField("fc_label_index"), NotNull, DataType(MySqlDbType.VarChar, "50"), Column(Name:="fc_label_index")> Public Property fc_label_index As String
    <DatabaseField("head_class"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="head_class")> Public Property head_class As String
    <DatabaseField("color_class"), DataType(MySqlDbType.VarChar, "20"), Column(Name:="color_class")> Public Property color_class As String
    <DatabaseField("fc_reference"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="fc_reference")> Public Property fc_reference As String
    <DatabaseField("fc_note"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="fc_note")> Public Property fc_note As String
    <DatabaseField("fc_internal_comment"), DataType(MySqlDbType.Text), Column(Name:="fc_internal_comment")> Public Property fc_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="key_id_org")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `functional_class` (`functional_class_id`, `fc_description`, `fc_label_index`, `head_class`, `color_class`, `fc_reference`, `fc_note`, `fc_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `functional_class` (`functional_class_id`, `fc_description`, `fc_label_index`, `head_class`, `color_class`, `fc_reference`, `fc_note`, `fc_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `functional_class` (`functional_class_id`, `fc_description`, `fc_label_index`, `head_class`, `color_class`, `fc_reference`, `fc_note`, `fc_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `functional_class` (`functional_class_id`, `fc_description`, `fc_label_index`, `head_class`, `color_class`, `fc_reference`, `fc_note`, `fc_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `functional_class` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `functional_class` SET `functional_class_id`='{0}', `fc_description`='{1}', `fc_label_index`='{2}', `head_class`='{3}', `color_class`='{4}', `fc_reference`='{5}', `fc_note`='{6}', `fc_internal_comment`='{7}', `key_id_org`='{8}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `functional_class` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `functional_class` (`functional_class_id`, `fc_description`, `fc_label_index`, `head_class`, `color_class`, `fc_reference`, `fc_note`, `fc_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, functional_class_id, fc_description, fc_label_index, head_class, color_class, fc_reference, fc_note, fc_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `functional_class` (`functional_class_id`, `fc_description`, `fc_label_index`, `head_class`, `color_class`, `fc_reference`, `fc_note`, `fc_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, functional_class_id, fc_description, fc_label_index, head_class, color_class, fc_reference, fc_note, fc_internal_comment, key_id_org)
        Else
        Return String.Format(INSERT_SQL, functional_class_id, fc_description, fc_label_index, head_class, color_class, fc_reference, fc_note, fc_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{functional_class_id}', '{fc_description}', '{fc_label_index}', '{head_class}', '{color_class}', '{fc_reference}', '{fc_note}', '{fc_internal_comment}', '{key_id_org}')"
        Else
            Return $"('{functional_class_id}', '{fc_description}', '{fc_label_index}', '{head_class}', '{color_class}', '{fc_reference}', '{fc_note}', '{fc_internal_comment}', '{key_id_org}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `functional_class` (`functional_class_id`, `fc_description`, `fc_label_index`, `head_class`, `color_class`, `fc_reference`, `fc_note`, `fc_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, functional_class_id, fc_description, fc_label_index, head_class, color_class, fc_reference, fc_note, fc_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `functional_class` (`functional_class_id`, `fc_description`, `fc_label_index`, `head_class`, `color_class`, `fc_reference`, `fc_note`, `fc_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, functional_class_id, fc_description, fc_label_index, head_class, color_class, fc_reference, fc_note, fc_internal_comment, key_id_org)
        Else
        Return String.Format(REPLACE_SQL, functional_class_id, fc_description, fc_label_index, head_class, color_class, fc_reference, fc_note, fc_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `functional_class` SET `functional_class_id`='{0}', `fc_description`='{1}', `fc_label_index`='{2}', `head_class`='{3}', `color_class`='{4}', `fc_reference`='{5}', `fc_note`='{6}', `fc_internal_comment`='{7}', `key_id_org`='{8}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As functional_class
                         Return DirectCast(MyClass.MemberwiseClone, functional_class)
                     End Function
End Class


End Namespace
