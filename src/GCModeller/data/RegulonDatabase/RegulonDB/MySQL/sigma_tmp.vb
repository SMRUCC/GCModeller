#Region "Microsoft.VisualBasic::4b724992d95f16c96b675d3bf0221b4b, data\RegulonDatabase\RegulonDB\MySQL\sigma_tmp.vb"

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

    ' Class sigma_tmp
    ' 
    '     Properties: key_id_org, sigma_coregulators, sigma_gene_id, sigma_gene_name, sigma_id
    '                 sigma_name, sigma_notes, sigma_sigmulon_genes, sigma_synonyms
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @3/16/2018 10:40:14 PM


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
''' DROP TABLE IF EXISTS `sigma_tmp`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `sigma_tmp` (
'''   `sigma_id` varchar(12) NOT NULL,
'''   `sigma_name` varchar(50) NOT NULL,
'''   `sigma_synonyms` varchar(50) DEFAULT NULL,
'''   `sigma_gene_id` varchar(12) DEFAULT NULL,
'''   `sigma_gene_name` varchar(250) DEFAULT NULL,
'''   `sigma_coregulators` varchar(2000) DEFAULT NULL,
'''   `sigma_notes` varchar(4000) DEFAULT NULL,
'''   `sigma_sigmulon_genes` varchar(4000) DEFAULT NULL,
'''   `key_id_org` varchar(5) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("sigma_tmp", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `sigma_tmp` (
  `sigma_id` varchar(12) NOT NULL,
  `sigma_name` varchar(50) NOT NULL,
  `sigma_synonyms` varchar(50) DEFAULT NULL,
  `sigma_gene_id` varchar(12) DEFAULT NULL,
  `sigma_gene_name` varchar(250) DEFAULT NULL,
  `sigma_coregulators` varchar(2000) DEFAULT NULL,
  `sigma_notes` varchar(4000) DEFAULT NULL,
  `sigma_sigmulon_genes` varchar(4000) DEFAULT NULL,
  `key_id_org` varchar(5) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class sigma_tmp: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("sigma_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="sigma_id")> Public Property sigma_id As String
    <DatabaseField("sigma_name"), NotNull, DataType(MySqlDbType.VarChar, "50"), Column(Name:="sigma_name")> Public Property sigma_name As String
    <DatabaseField("sigma_synonyms"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="sigma_synonyms")> Public Property sigma_synonyms As String
    <DatabaseField("sigma_gene_id"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="sigma_gene_id")> Public Property sigma_gene_id As String
    <DatabaseField("sigma_gene_name"), DataType(MySqlDbType.VarChar, "250"), Column(Name:="sigma_gene_name")> Public Property sigma_gene_name As String
    <DatabaseField("sigma_coregulators"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="sigma_coregulators")> Public Property sigma_coregulators As String
    <DatabaseField("sigma_notes"), DataType(MySqlDbType.VarChar, "4000"), Column(Name:="sigma_notes")> Public Property sigma_notes As String
    <DatabaseField("sigma_sigmulon_genes"), DataType(MySqlDbType.VarChar, "4000"), Column(Name:="sigma_sigmulon_genes")> Public Property sigma_sigmulon_genes As String
    <DatabaseField("key_id_org"), DataType(MySqlDbType.VarChar, "5"), Column(Name:="key_id_org")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `sigma_tmp` (`sigma_id`, `sigma_name`, `sigma_synonyms`, `sigma_gene_id`, `sigma_gene_name`, `sigma_coregulators`, `sigma_notes`, `sigma_sigmulon_genes`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `sigma_tmp` (`sigma_id`, `sigma_name`, `sigma_synonyms`, `sigma_gene_id`, `sigma_gene_name`, `sigma_coregulators`, `sigma_notes`, `sigma_sigmulon_genes`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `sigma_tmp` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `sigma_tmp` SET `sigma_id`='{0}', `sigma_name`='{1}', `sigma_synonyms`='{2}', `sigma_gene_id`='{3}', `sigma_gene_name`='{4}', `sigma_coregulators`='{5}', `sigma_notes`='{6}', `sigma_sigmulon_genes`='{7}', `key_id_org`='{8}' WHERE ;</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `sigma_tmp` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `sigma_tmp` (`sigma_id`, `sigma_name`, `sigma_synonyms`, `sigma_gene_id`, `sigma_gene_name`, `sigma_coregulators`, `sigma_notes`, `sigma_sigmulon_genes`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, sigma_id, sigma_name, sigma_synonyms, sigma_gene_id, sigma_gene_name, sigma_coregulators, sigma_notes, sigma_sigmulon_genes, key_id_org)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{sigma_id}', '{sigma_name}', '{sigma_synonyms}', '{sigma_gene_id}', '{sigma_gene_name}', '{sigma_coregulators}', '{sigma_notes}', '{sigma_sigmulon_genes}', '{key_id_org}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `sigma_tmp` (`sigma_id`, `sigma_name`, `sigma_synonyms`, `sigma_gene_id`, `sigma_gene_name`, `sigma_coregulators`, `sigma_notes`, `sigma_sigmulon_genes`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, sigma_id, sigma_name, sigma_synonyms, sigma_gene_id, sigma_gene_name, sigma_coregulators, sigma_notes, sigma_sigmulon_genes, key_id_org)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `sigma_tmp` SET `sigma_id`='{0}', `sigma_name`='{1}', `sigma_synonyms`='{2}', `sigma_gene_id`='{3}', `sigma_gene_name`='{4}', `sigma_coregulators`='{5}', `sigma_notes`='{6}', `sigma_sigmulon_genes`='{7}', `key_id_org`='{8}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
Public Function Clone() As sigma_tmp
                  Return DirectCast(MyClass.MemberwiseClone, sigma_tmp)
              End Function
End Class


End Namespace

