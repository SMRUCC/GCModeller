#Region "Microsoft.VisualBasic::1649426cec1bc504da133a4c3007d0ac, data\RegulonDatabase\RegulonDB\MySQL\attenuator.vb"

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

    ' Class attenuator
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

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
''' DROP TABLE IF EXISTS `attenuator`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `attenuator` (
'''   `attenuator_id` varchar(12) NOT NULL,
'''   `gene_id` char(12) DEFAULT NULL,
'''   `attenuator_type` varchar(16) DEFAULT NULL,
'''   `attenuator_strand` varchar(12) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("attenuator", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `attenuator` (
  `attenuator_id` varchar(12) NOT NULL,
  `gene_id` char(12) DEFAULT NULL,
  `attenuator_type` varchar(16) DEFAULT NULL,
  `attenuator_strand` varchar(12) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class attenuator: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("attenuator_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property attenuator_id As String
    <DatabaseField("gene_id"), DataType(MySqlDbType.VarChar, "12")> Public Property gene_id As String
    <DatabaseField("attenuator_type"), DataType(MySqlDbType.VarChar, "16")> Public Property attenuator_type As String
    <DatabaseField("attenuator_strand"), DataType(MySqlDbType.VarChar, "12")> Public Property attenuator_strand As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `attenuator` (`attenuator_id`, `gene_id`, `attenuator_type`, `attenuator_strand`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `attenuator` (`attenuator_id`, `gene_id`, `attenuator_type`, `attenuator_strand`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `attenuator` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `attenuator` SET `attenuator_id`='{0}', `gene_id`='{1}', `attenuator_type`='{2}', `attenuator_strand`='{3}' WHERE ;</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `attenuator` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `attenuator` (`attenuator_id`, `gene_id`, `attenuator_type`, `attenuator_strand`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, attenuator_id, gene_id, attenuator_type, attenuator_strand)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{attenuator_id}', '{gene_id}', '{attenuator_type}', '{attenuator_strand}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `attenuator` (`attenuator_id`, `gene_id`, `attenuator_type`, `attenuator_strand`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, attenuator_id, gene_id, attenuator_type, attenuator_strand)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `attenuator` SET `attenuator_id`='{0}', `gene_id`='{1}', `attenuator_type`='{2}', `attenuator_strand`='{3}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
