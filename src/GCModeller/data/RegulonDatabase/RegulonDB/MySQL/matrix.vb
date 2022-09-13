#Region "Microsoft.VisualBasic::5c39edb1f652598347b9e059d081ec95, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\matrix.vb"

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

    '   Total Lines: 161
    '    Code Lines: 81
    ' Comment Lines: 58
    '   Blank Lines: 22
    '     File Size: 6.91 KB


    ' Class matrix
    ' 
    '     Properties: freq_a, freq_c, freq_g, freq_t, num_col
    '                 tf_matrix_id
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
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
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `matrix` (`tf_matrix_id`, `num_col`, `freq_a`, `freq_c`, `freq_g`, `freq_t`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `matrix` (`tf_matrix_id`, `num_col`, `freq_a`, `freq_c`, `freq_g`, `freq_t`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `matrix` (`tf_matrix_id`, `num_col`, `freq_a`, `freq_c`, `freq_g`, `freq_t`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `matrix` (`tf_matrix_id`, `num_col`, `freq_a`, `freq_c`, `freq_g`, `freq_t`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `matrix` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `matrix` SET `tf_matrix_id`='{0}', `num_col`='{1}', `freq_a`='{2}', `freq_c`='{3}', `freq_g`='{4}', `freq_t`='{5}' WHERE ;</SQL>

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
''' ```SQL
''' INSERT INTO `matrix` (`tf_matrix_id`, `num_col`, `freq_a`, `freq_c`, `freq_g`, `freq_t`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, tf_matrix_id, num_col, freq_a, freq_c, freq_g, freq_t)
        Else
        Return String.Format(INSERT_SQL, tf_matrix_id, num_col, freq_a, freq_c, freq_g, freq_t)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{tf_matrix_id}', '{num_col}', '{freq_a}', '{freq_c}', '{freq_g}', '{freq_t}')"
        Else
            Return $"('{tf_matrix_id}', '{num_col}', '{freq_a}', '{freq_c}', '{freq_g}', '{freq_t}')"
        End If
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
''' REPLACE INTO `matrix` (`tf_matrix_id`, `num_col`, `freq_a`, `freq_c`, `freq_g`, `freq_t`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, tf_matrix_id, num_col, freq_a, freq_c, freq_g, freq_t)
        Else
        Return String.Format(REPLACE_SQL, tf_matrix_id, num_col, freq_a, freq_c, freq_g, freq_t)
        End If
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

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As matrix
                         Return DirectCast(MyClass.MemberwiseClone, matrix)
                     End Function
End Class


End Namespace
