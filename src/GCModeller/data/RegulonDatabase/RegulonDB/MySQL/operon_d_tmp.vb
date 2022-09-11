#Region "Microsoft.VisualBasic::6b6e94d89136189df820f4e144e6be9d, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\operon_d_tmp.vb"

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
    '     File Size: 10.66 KB


    ' Class operon_d_tmp
    ' 
    '     Properties: op_id, operon_gene_group, operon_id, operon_name, operon_promoter_group
    '                 operon_sf_group, operon_site_group, operon_terminator_group, operon_tf_group, operon_tu_group
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
''' DROP TABLE IF EXISTS `operon_d_tmp`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `operon_d_tmp` (
'''   `op_id` decimal(10,0) NOT NULL,
'''   `operon_id` char(12) DEFAULT NULL,
'''   `operon_name` varchar(255) DEFAULT NULL,
'''   `operon_tu_group` decimal(10,0) DEFAULT NULL,
'''   `operon_gene_group` decimal(10,0) DEFAULT NULL,
'''   `operon_sf_group` decimal(10,0) DEFAULT NULL,
'''   `operon_site_group` decimal(10,0) DEFAULT NULL,
'''   `operon_promoter_group` decimal(10,0) DEFAULT NULL,
'''   `operon_tf_group` decimal(10,0) DEFAULT NULL,
'''   `operon_terminator_group` decimal(10,0) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("operon_d_tmp", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `operon_d_tmp` (
  `op_id` decimal(10,0) NOT NULL,
  `operon_id` char(12) DEFAULT NULL,
  `operon_name` varchar(255) DEFAULT NULL,
  `operon_tu_group` decimal(10,0) DEFAULT NULL,
  `operon_gene_group` decimal(10,0) DEFAULT NULL,
  `operon_sf_group` decimal(10,0) DEFAULT NULL,
  `operon_site_group` decimal(10,0) DEFAULT NULL,
  `operon_promoter_group` decimal(10,0) DEFAULT NULL,
  `operon_tf_group` decimal(10,0) DEFAULT NULL,
  `operon_terminator_group` decimal(10,0) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class operon_d_tmp: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("op_id"), NotNull, DataType(MySqlDbType.Decimal), Column(Name:="op_id")> Public Property op_id As Decimal
    <DatabaseField("operon_id"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="operon_id")> Public Property operon_id As String
    <DatabaseField("operon_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="operon_name")> Public Property operon_name As String
    <DatabaseField("operon_tu_group"), DataType(MySqlDbType.Decimal), Column(Name:="operon_tu_group")> Public Property operon_tu_group As Decimal
    <DatabaseField("operon_gene_group"), DataType(MySqlDbType.Decimal), Column(Name:="operon_gene_group")> Public Property operon_gene_group As Decimal
    <DatabaseField("operon_sf_group"), DataType(MySqlDbType.Decimal), Column(Name:="operon_sf_group")> Public Property operon_sf_group As Decimal
    <DatabaseField("operon_site_group"), DataType(MySqlDbType.Decimal), Column(Name:="operon_site_group")> Public Property operon_site_group As Decimal
    <DatabaseField("operon_promoter_group"), DataType(MySqlDbType.Decimal), Column(Name:="operon_promoter_group")> Public Property operon_promoter_group As Decimal
    <DatabaseField("operon_tf_group"), DataType(MySqlDbType.Decimal), Column(Name:="operon_tf_group")> Public Property operon_tf_group As Decimal
    <DatabaseField("operon_terminator_group"), DataType(MySqlDbType.Decimal), Column(Name:="operon_terminator_group")> Public Property operon_terminator_group As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `operon_d_tmp` (`op_id`, `operon_id`, `operon_name`, `operon_tu_group`, `operon_gene_group`, `operon_sf_group`, `operon_site_group`, `operon_promoter_group`, `operon_tf_group`, `operon_terminator_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `operon_d_tmp` (`op_id`, `operon_id`, `operon_name`, `operon_tu_group`, `operon_gene_group`, `operon_sf_group`, `operon_site_group`, `operon_promoter_group`, `operon_tf_group`, `operon_terminator_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `operon_d_tmp` (`op_id`, `operon_id`, `operon_name`, `operon_tu_group`, `operon_gene_group`, `operon_sf_group`, `operon_site_group`, `operon_promoter_group`, `operon_tf_group`, `operon_terminator_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `operon_d_tmp` (`op_id`, `operon_id`, `operon_name`, `operon_tu_group`, `operon_gene_group`, `operon_sf_group`, `operon_site_group`, `operon_promoter_group`, `operon_tf_group`, `operon_terminator_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `operon_d_tmp` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `operon_d_tmp` SET `op_id`='{0}', `operon_id`='{1}', `operon_name`='{2}', `operon_tu_group`='{3}', `operon_gene_group`='{4}', `operon_sf_group`='{5}', `operon_site_group`='{6}', `operon_promoter_group`='{7}', `operon_tf_group`='{8}', `operon_terminator_group`='{9}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `operon_d_tmp` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `operon_d_tmp` (`op_id`, `operon_id`, `operon_name`, `operon_tu_group`, `operon_gene_group`, `operon_sf_group`, `operon_site_group`, `operon_promoter_group`, `operon_tf_group`, `operon_terminator_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, op_id, operon_id, operon_name, operon_tu_group, operon_gene_group, operon_sf_group, operon_site_group, operon_promoter_group, operon_tf_group, operon_terminator_group)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `operon_d_tmp` (`op_id`, `operon_id`, `operon_name`, `operon_tu_group`, `operon_gene_group`, `operon_sf_group`, `operon_site_group`, `operon_promoter_group`, `operon_tf_group`, `operon_terminator_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, op_id, operon_id, operon_name, operon_tu_group, operon_gene_group, operon_sf_group, operon_site_group, operon_promoter_group, operon_tf_group, operon_terminator_group)
        Else
        Return String.Format(INSERT_SQL, op_id, operon_id, operon_name, operon_tu_group, operon_gene_group, operon_sf_group, operon_site_group, operon_promoter_group, operon_tf_group, operon_terminator_group)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{op_id}', '{operon_id}', '{operon_name}', '{operon_tu_group}', '{operon_gene_group}', '{operon_sf_group}', '{operon_site_group}', '{operon_promoter_group}', '{operon_tf_group}', '{operon_terminator_group}')"
        Else
            Return $"('{op_id}', '{operon_id}', '{operon_name}', '{operon_tu_group}', '{operon_gene_group}', '{operon_sf_group}', '{operon_site_group}', '{operon_promoter_group}', '{operon_tf_group}', '{operon_terminator_group}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `operon_d_tmp` (`op_id`, `operon_id`, `operon_name`, `operon_tu_group`, `operon_gene_group`, `operon_sf_group`, `operon_site_group`, `operon_promoter_group`, `operon_tf_group`, `operon_terminator_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, op_id, operon_id, operon_name, operon_tu_group, operon_gene_group, operon_sf_group, operon_site_group, operon_promoter_group, operon_tf_group, operon_terminator_group)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `operon_d_tmp` (`op_id`, `operon_id`, `operon_name`, `operon_tu_group`, `operon_gene_group`, `operon_sf_group`, `operon_site_group`, `operon_promoter_group`, `operon_tf_group`, `operon_terminator_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, op_id, operon_id, operon_name, operon_tu_group, operon_gene_group, operon_sf_group, operon_site_group, operon_promoter_group, operon_tf_group, operon_terminator_group)
        Else
        Return String.Format(REPLACE_SQL, op_id, operon_id, operon_name, operon_tu_group, operon_gene_group, operon_sf_group, operon_site_group, operon_promoter_group, operon_tf_group, operon_terminator_group)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `operon_d_tmp` SET `op_id`='{0}', `operon_id`='{1}', `operon_name`='{2}', `operon_tu_group`='{3}', `operon_gene_group`='{4}', `operon_sf_group`='{5}', `operon_site_group`='{6}', `operon_promoter_group`='{7}', `operon_tf_group`='{8}', `operon_terminator_group`='{9}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As operon_d_tmp
                         Return DirectCast(MyClass.MemberwiseClone, operon_d_tmp)
                     End Function
End Class


End Namespace
