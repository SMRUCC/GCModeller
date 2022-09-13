#Region "Microsoft.VisualBasic::c8b4759b7cb63a4f48426e5f3bc55683, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\rfam.vb"

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
    '     File Size: 9.07 KB


    ' Class rfam
    ' 
    '     Properties: gene_id, rfam_description, rfam_id, rfam_posleft, rfam_posright
    '                 rfam_score, rfam_sequence, rfam_strand, rfam_type
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
''' DROP TABLE IF EXISTS `rfam`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `rfam` (
'''   `rfam_id` varchar(12) NOT NULL,
'''   `gene_id` char(12) DEFAULT NULL,
'''   `rfam_type` varchar(100) DEFAULT NULL,
'''   `rfam_description` varchar(2000) DEFAULT NULL,
'''   `rfam_score` decimal(10,5) DEFAULT NULL,
'''   `rfam_strand` varchar(12) DEFAULT NULL,
'''   `rfam_posleft` decimal(10,0) DEFAULT NULL,
'''   `rfam_posright` decimal(10,0) DEFAULT NULL,
'''   `rfam_sequence` varchar(1000) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("rfam", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `rfam` (
  `rfam_id` varchar(12) NOT NULL,
  `gene_id` char(12) DEFAULT NULL,
  `rfam_type` varchar(100) DEFAULT NULL,
  `rfam_description` varchar(2000) DEFAULT NULL,
  `rfam_score` decimal(10,5) DEFAULT NULL,
  `rfam_strand` varchar(12) DEFAULT NULL,
  `rfam_posleft` decimal(10,0) DEFAULT NULL,
  `rfam_posright` decimal(10,0) DEFAULT NULL,
  `rfam_sequence` varchar(1000) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class rfam: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="rfam_id")> Public Property rfam_id As String
    <DatabaseField("gene_id"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="gene_id")> Public Property gene_id As String
    <DatabaseField("rfam_type"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="rfam_type")> Public Property rfam_type As String
    <DatabaseField("rfam_description"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="rfam_description")> Public Property rfam_description As String
    <DatabaseField("rfam_score"), DataType(MySqlDbType.Decimal), Column(Name:="rfam_score")> Public Property rfam_score As Decimal
    <DatabaseField("rfam_strand"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="rfam_strand")> Public Property rfam_strand As String
    <DatabaseField("rfam_posleft"), DataType(MySqlDbType.Decimal), Column(Name:="rfam_posleft")> Public Property rfam_posleft As Decimal
    <DatabaseField("rfam_posright"), DataType(MySqlDbType.Decimal), Column(Name:="rfam_posright")> Public Property rfam_posright As Decimal
    <DatabaseField("rfam_sequence"), DataType(MySqlDbType.VarChar, "1000"), Column(Name:="rfam_sequence")> Public Property rfam_sequence As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `rfam` (`rfam_id`, `gene_id`, `rfam_type`, `rfam_description`, `rfam_score`, `rfam_strand`, `rfam_posleft`, `rfam_posright`, `rfam_sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `rfam` (`rfam_id`, `gene_id`, `rfam_type`, `rfam_description`, `rfam_score`, `rfam_strand`, `rfam_posleft`, `rfam_posright`, `rfam_sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `rfam` (`rfam_id`, `gene_id`, `rfam_type`, `rfam_description`, `rfam_score`, `rfam_strand`, `rfam_posleft`, `rfam_posright`, `rfam_sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `rfam` (`rfam_id`, `gene_id`, `rfam_type`, `rfam_description`, `rfam_score`, `rfam_strand`, `rfam_posleft`, `rfam_posright`, `rfam_sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `rfam` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `rfam` SET `rfam_id`='{0}', `gene_id`='{1}', `rfam_type`='{2}', `rfam_description`='{3}', `rfam_score`='{4}', `rfam_strand`='{5}', `rfam_posleft`='{6}', `rfam_posright`='{7}', `rfam_sequence`='{8}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `rfam` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `rfam` (`rfam_id`, `gene_id`, `rfam_type`, `rfam_description`, `rfam_score`, `rfam_strand`, `rfam_posleft`, `rfam_posright`, `rfam_sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_id, gene_id, rfam_type, rfam_description, rfam_score, rfam_strand, rfam_posleft, rfam_posright, rfam_sequence)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `rfam` (`rfam_id`, `gene_id`, `rfam_type`, `rfam_description`, `rfam_score`, `rfam_strand`, `rfam_posleft`, `rfam_posright`, `rfam_sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, rfam_id, gene_id, rfam_type, rfam_description, rfam_score, rfam_strand, rfam_posleft, rfam_posright, rfam_sequence)
        Else
        Return String.Format(INSERT_SQL, rfam_id, gene_id, rfam_type, rfam_description, rfam_score, rfam_strand, rfam_posleft, rfam_posright, rfam_sequence)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{rfam_id}', '{gene_id}', '{rfam_type}', '{rfam_description}', '{rfam_score}', '{rfam_strand}', '{rfam_posleft}', '{rfam_posright}', '{rfam_sequence}')"
        Else
            Return $"('{rfam_id}', '{gene_id}', '{rfam_type}', '{rfam_description}', '{rfam_score}', '{rfam_strand}', '{rfam_posleft}', '{rfam_posright}', '{rfam_sequence}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `rfam` (`rfam_id`, `gene_id`, `rfam_type`, `rfam_description`, `rfam_score`, `rfam_strand`, `rfam_posleft`, `rfam_posright`, `rfam_sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_id, gene_id, rfam_type, rfam_description, rfam_score, rfam_strand, rfam_posleft, rfam_posright, rfam_sequence)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `rfam` (`rfam_id`, `gene_id`, `rfam_type`, `rfam_description`, `rfam_score`, `rfam_strand`, `rfam_posleft`, `rfam_posright`, `rfam_sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, rfam_id, gene_id, rfam_type, rfam_description, rfam_score, rfam_strand, rfam_posleft, rfam_posright, rfam_sequence)
        Else
        Return String.Format(REPLACE_SQL, rfam_id, gene_id, rfam_type, rfam_description, rfam_score, rfam_strand, rfam_posleft, rfam_posright, rfam_sequence)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `rfam` SET `rfam_id`='{0}', `gene_id`='{1}', `rfam_type`='{2}', `rfam_description`='{3}', `rfam_score`='{4}', `rfam_strand`='{5}', `rfam_posleft`='{6}', `rfam_posright`='{7}', `rfam_sequence`='{8}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As rfam
                         Return DirectCast(MyClass.MemberwiseClone, rfam)
                     End Function
End Class


End Namespace
