#Region "Microsoft.VisualBasic::480bc3c491d1d6339bd4b05f438f7df2, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\tu_gene_link.vb"

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

    '   Total Lines: 149
    '    Code Lines: 73
    ' Comment Lines: 54
    '   Blank Lines: 22
    '     File Size: 5.45 KB


    ' Class tu_gene_link
    ' 
    '     Properties: gene_id, transcription_unit_id
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
''' DROP TABLE IF EXISTS `tu_gene_link`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `tu_gene_link` (
'''   `transcription_unit_id` char(12) NOT NULL,
'''   `gene_id` char(12) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("tu_gene_link", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `tu_gene_link` (
  `transcription_unit_id` char(12) NOT NULL,
  `gene_id` char(12) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class tu_gene_link: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("transcription_unit_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="transcription_unit_id")> Public Property transcription_unit_id As String
    <DatabaseField("gene_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="gene_id")> Public Property gene_id As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `tu_gene_link` (`transcription_unit_id`, `gene_id`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `tu_gene_link` (`transcription_unit_id`, `gene_id`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `tu_gene_link` (`transcription_unit_id`, `gene_id`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `tu_gene_link` (`transcription_unit_id`, `gene_id`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `tu_gene_link` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `tu_gene_link` SET `transcription_unit_id`='{0}', `gene_id`='{1}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `tu_gene_link` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `tu_gene_link` (`transcription_unit_id`, `gene_id`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, transcription_unit_id, gene_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `tu_gene_link` (`transcription_unit_id`, `gene_id`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, transcription_unit_id, gene_id)
        Else
        Return String.Format(INSERT_SQL, transcription_unit_id, gene_id)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{transcription_unit_id}', '{gene_id}')"
        Else
            Return $"('{transcription_unit_id}', '{gene_id}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `tu_gene_link` (`transcription_unit_id`, `gene_id`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, transcription_unit_id, gene_id)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `tu_gene_link` (`transcription_unit_id`, `gene_id`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, transcription_unit_id, gene_id)
        Else
        Return String.Format(REPLACE_SQL, transcription_unit_id, gene_id)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `tu_gene_link` SET `transcription_unit_id`='{0}', `gene_id`='{1}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As tu_gene_link
                         Return DirectCast(MyClass.MemberwiseClone, tu_gene_link)
                     End Function
End Class


End Namespace
