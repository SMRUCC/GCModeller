#Region "Microsoft.VisualBasic::506812938efc8a59711f133af275bfa3, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\gene_head_fc.vb"

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

    '   Total Lines: 173
    '    Code Lines: 89
    ' Comment Lines: 62
    '   Blank Lines: 22
    '     File Size: 10.72 KB


    ' Class gene_head_fc
    ' 
    '     Properties: child_fc_description, child_fc_id, child_fc_label_index, child_fc_reference, gene_id
    '                 gene_name, head_fc_description, head_fc_id, head_fc_label_index, head_fc_reference
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
''' DROP TABLE IF EXISTS `gene_head_fc`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `gene_head_fc` (
'''   `gene_id` char(12) DEFAULT NULL,
'''   `gene_name` varchar(255) DEFAULT NULL,
'''   `head_fc_id` char(12) DEFAULT NULL,
'''   `head_fc_description` varchar(500) DEFAULT NULL,
'''   `head_fc_label_index` varchar(50) DEFAULT NULL,
'''   `head_fc_reference` varchar(255) DEFAULT NULL,
'''   `child_fc_id` char(12) DEFAULT NULL,
'''   `child_fc_description` varchar(500) DEFAULT NULL,
'''   `child_fc_label_index` varchar(50) DEFAULT NULL,
'''   `child_fc_reference` varchar(255) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("gene_head_fc", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `gene_head_fc` (
  `gene_id` char(12) DEFAULT NULL,
  `gene_name` varchar(255) DEFAULT NULL,
  `head_fc_id` char(12) DEFAULT NULL,
  `head_fc_description` varchar(500) DEFAULT NULL,
  `head_fc_label_index` varchar(50) DEFAULT NULL,
  `head_fc_reference` varchar(255) DEFAULT NULL,
  `child_fc_id` char(12) DEFAULT NULL,
  `child_fc_description` varchar(500) DEFAULT NULL,
  `child_fc_label_index` varchar(50) DEFAULT NULL,
  `child_fc_reference` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class gene_head_fc: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("gene_id"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="gene_id")> Public Property gene_id As String
    <DatabaseField("gene_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="gene_name")> Public Property gene_name As String
    <DatabaseField("head_fc_id"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="head_fc_id")> Public Property head_fc_id As String
    <DatabaseField("head_fc_description"), DataType(MySqlDbType.VarChar, "500"), Column(Name:="head_fc_description")> Public Property head_fc_description As String
    <DatabaseField("head_fc_label_index"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="head_fc_label_index")> Public Property head_fc_label_index As String
    <DatabaseField("head_fc_reference"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="head_fc_reference")> Public Property head_fc_reference As String
    <DatabaseField("child_fc_id"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="child_fc_id")> Public Property child_fc_id As String
    <DatabaseField("child_fc_description"), DataType(MySqlDbType.VarChar, "500"), Column(Name:="child_fc_description")> Public Property child_fc_description As String
    <DatabaseField("child_fc_label_index"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="child_fc_label_index")> Public Property child_fc_label_index As String
    <DatabaseField("child_fc_reference"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="child_fc_reference")> Public Property child_fc_reference As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `gene_head_fc` (`gene_id`, `gene_name`, `head_fc_id`, `head_fc_description`, `head_fc_label_index`, `head_fc_reference`, `child_fc_id`, `child_fc_description`, `child_fc_label_index`, `child_fc_reference`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `gene_head_fc` (`gene_id`, `gene_name`, `head_fc_id`, `head_fc_description`, `head_fc_label_index`, `head_fc_reference`, `child_fc_id`, `child_fc_description`, `child_fc_label_index`, `child_fc_reference`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `gene_head_fc` (`gene_id`, `gene_name`, `head_fc_id`, `head_fc_description`, `head_fc_label_index`, `head_fc_reference`, `child_fc_id`, `child_fc_description`, `child_fc_label_index`, `child_fc_reference`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `gene_head_fc` (`gene_id`, `gene_name`, `head_fc_id`, `head_fc_description`, `head_fc_label_index`, `head_fc_reference`, `child_fc_id`, `child_fc_description`, `child_fc_label_index`, `child_fc_reference`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `gene_head_fc` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `gene_head_fc` SET `gene_id`='{0}', `gene_name`='{1}', `head_fc_id`='{2}', `head_fc_description`='{3}', `head_fc_label_index`='{4}', `head_fc_reference`='{5}', `child_fc_id`='{6}', `child_fc_description`='{7}', `child_fc_label_index`='{8}', `child_fc_reference`='{9}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `gene_head_fc` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `gene_head_fc` (`gene_id`, `gene_name`, `head_fc_id`, `head_fc_description`, `head_fc_label_index`, `head_fc_reference`, `child_fc_id`, `child_fc_description`, `child_fc_label_index`, `child_fc_reference`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, gene_id, gene_name, head_fc_id, head_fc_description, head_fc_label_index, head_fc_reference, child_fc_id, child_fc_description, child_fc_label_index, child_fc_reference)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `gene_head_fc` (`gene_id`, `gene_name`, `head_fc_id`, `head_fc_description`, `head_fc_label_index`, `head_fc_reference`, `child_fc_id`, `child_fc_description`, `child_fc_label_index`, `child_fc_reference`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, gene_id, gene_name, head_fc_id, head_fc_description, head_fc_label_index, head_fc_reference, child_fc_id, child_fc_description, child_fc_label_index, child_fc_reference)
        Else
        Return String.Format(INSERT_SQL, gene_id, gene_name, head_fc_id, head_fc_description, head_fc_label_index, head_fc_reference, child_fc_id, child_fc_description, child_fc_label_index, child_fc_reference)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{gene_id}', '{gene_name}', '{head_fc_id}', '{head_fc_description}', '{head_fc_label_index}', '{head_fc_reference}', '{child_fc_id}', '{child_fc_description}', '{child_fc_label_index}', '{child_fc_reference}')"
        Else
            Return $"('{gene_id}', '{gene_name}', '{head_fc_id}', '{head_fc_description}', '{head_fc_label_index}', '{head_fc_reference}', '{child_fc_id}', '{child_fc_description}', '{child_fc_label_index}', '{child_fc_reference}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `gene_head_fc` (`gene_id`, `gene_name`, `head_fc_id`, `head_fc_description`, `head_fc_label_index`, `head_fc_reference`, `child_fc_id`, `child_fc_description`, `child_fc_label_index`, `child_fc_reference`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, gene_id, gene_name, head_fc_id, head_fc_description, head_fc_label_index, head_fc_reference, child_fc_id, child_fc_description, child_fc_label_index, child_fc_reference)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `gene_head_fc` (`gene_id`, `gene_name`, `head_fc_id`, `head_fc_description`, `head_fc_label_index`, `head_fc_reference`, `child_fc_id`, `child_fc_description`, `child_fc_label_index`, `child_fc_reference`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, gene_id, gene_name, head_fc_id, head_fc_description, head_fc_label_index, head_fc_reference, child_fc_id, child_fc_description, child_fc_label_index, child_fc_reference)
        Else
        Return String.Format(REPLACE_SQL, gene_id, gene_name, head_fc_id, head_fc_description, head_fc_label_index, head_fc_reference, child_fc_id, child_fc_description, child_fc_label_index, child_fc_reference)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `gene_head_fc` SET `gene_id`='{0}', `gene_name`='{1}', `head_fc_id`='{2}', `head_fc_description`='{3}', `head_fc_label_index`='{4}', `head_fc_reference`='{5}', `child_fc_id`='{6}', `child_fc_description`='{7}', `child_fc_label_index`='{8}', `child_fc_reference`='{9}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As gene_head_fc
                         Return DirectCast(MyClass.MemberwiseClone, gene_head_fc)
                     End Function
End Class


End Namespace
