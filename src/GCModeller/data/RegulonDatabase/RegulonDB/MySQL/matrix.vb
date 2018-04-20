#Region "Microsoft.VisualBasic::11992863e3137371cb9ebfe9ac4da902, data\RegulonDatabase\RegulonDB\MySQL\matrix.vb"

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

    ' Class matrix
    ' 
    '     Properties: freq_a, freq_c, freq_g, freq_t, num_col
    '                 tf_matrix_id
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
''' DROP TABLE IF EXISTS `matrix`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `matrix` (
'''   `tf_matrix_id` char(12) NOT NULL,
'''   `num_col` decimal(10,0) NOT NULL,
'''   `freq_a` decimal(10,0) NOT NULL,
'''   `freq_c` decimal(10,0) NOT NULL,
'''   `freq_g` decimal(10,0) NOT NULL,
'''   `freq_t` decimal(10,0) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("matrix", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `matrix` (
  `tf_matrix_id` char(12) NOT NULL,
  `num_col` decimal(10,0) NOT NULL,
  `freq_a` decimal(10,0) NOT NULL,
  `freq_c` decimal(10,0) NOT NULL,
  `freq_g` decimal(10,0) NOT NULL,
  `freq_t` decimal(10,0) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class matrix: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("tf_matrix_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="tf_matrix_id")> Public Property tf_matrix_id As String
    <DatabaseField("num_col"), NotNull, DataType(MySqlDbType.Decimal), Column(Name:="num_col")> Public Property num_col As Decimal
    <DatabaseField("freq_a"), NotNull, DataType(MySqlDbType.Decimal), Column(Name:="freq_a")> Public Property freq_a As Decimal
    <DatabaseField("freq_c"), NotNull, DataType(MySqlDbType.Decimal), Column(Name:="freq_c")> Public Property freq_c As Decimal
    <DatabaseField("freq_g"), NotNull, DataType(MySqlDbType.Decimal), Column(Name:="freq_g")> Public Property freq_g As Decimal
    <DatabaseField("freq_t"), NotNull, DataType(MySqlDbType.Decimal), Column(Name:="freq_t")> Public Property freq_t As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `matrix` (`tf_matrix_id`, `num_col`, `freq_a`, `freq_c`, `freq_g`, `freq_t`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `matrix` (`tf_matrix_id`, `num_col`, `freq_a`, `freq_c`, `freq_g`, `freq_t`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `matrix` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `matrix` SET `tf_matrix_id`='{0}', `num_col`='{1}', `freq_a`='{2}', `freq_c`='{3}', `freq_g`='{4}', `freq_t`='{5}' WHERE ;</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `matrix` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `matrix` (`tf_matrix_id`, `num_col`, `freq_a`, `freq_c`, `freq_g`, `freq_t`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, tf_matrix_id, num_col, freq_a, freq_c, freq_g, freq_t)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{tf_matrix_id}', '{num_col}', '{freq_a}', '{freq_c}', '{freq_g}', '{freq_t}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `matrix` (`tf_matrix_id`, `num_col`, `freq_a`, `freq_c`, `freq_g`, `freq_t`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, tf_matrix_id, num_col, freq_a, freq_c, freq_g, freq_t)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `matrix` SET `tf_matrix_id`='{0}', `num_col`='{1}', `freq_a`='{2}', `freq_c`='{3}', `freq_g`='{4}', `freq_t`='{5}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
Public Function Clone() As matrix
                  Return DirectCast(MyClass.MemberwiseClone, matrix)
              End Function
End Class


End Namespace
