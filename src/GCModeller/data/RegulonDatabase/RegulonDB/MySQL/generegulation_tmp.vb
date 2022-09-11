#Region "Microsoft.VisualBasic::a6b4302800b05f8ab10198dc31a118ae, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\generegulation_tmp.vb"

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
    '     File Size: 10.96 KB


    ' Class generegulation_tmp
    ' 
    '     Properties: conformation_status, gene_id_regulated, gene_id_regulator, gene_name_regulated, gene_name_regulator
    '                 generegulation_function, tf_conformation, tf_id_regulator, transcription_factor_name
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
''' DROP TABLE IF EXISTS `generegulation_tmp`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `generegulation_tmp` (
'''   `gene_id_regulator` char(12) DEFAULT NULL,
'''   `gene_name_regulator` varchar(255) DEFAULT NULL,
'''   `tf_id_regulator` char(12) DEFAULT NULL,
'''   `transcription_factor_name` varchar(255) DEFAULT NULL,
'''   `tf_conformation` varchar(2000) DEFAULT NULL,
'''   `conformation_status` varchar(5) DEFAULT NULL,
'''   `gene_id_regulated` char(12) DEFAULT NULL,
'''   `gene_name_regulated` varchar(255) DEFAULT NULL,
'''   `generegulation_function` varchar(9) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("generegulation_tmp", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `generegulation_tmp` (
  `gene_id_regulator` char(12) DEFAULT NULL,
  `gene_name_regulator` varchar(255) DEFAULT NULL,
  `tf_id_regulator` char(12) DEFAULT NULL,
  `transcription_factor_name` varchar(255) DEFAULT NULL,
  `tf_conformation` varchar(2000) DEFAULT NULL,
  `conformation_status` varchar(5) DEFAULT NULL,
  `gene_id_regulated` char(12) DEFAULT NULL,
  `gene_name_regulated` varchar(255) DEFAULT NULL,
  `generegulation_function` varchar(9) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class generegulation_tmp: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("gene_id_regulator"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="gene_id_regulator")> Public Property gene_id_regulator As String
    <DatabaseField("gene_name_regulator"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="gene_name_regulator")> Public Property gene_name_regulator As String
    <DatabaseField("tf_id_regulator"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="tf_id_regulator")> Public Property tf_id_regulator As String
    <DatabaseField("transcription_factor_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="transcription_factor_name")> Public Property transcription_factor_name As String
    <DatabaseField("tf_conformation"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="tf_conformation")> Public Property tf_conformation As String
    <DatabaseField("conformation_status"), DataType(MySqlDbType.VarChar, "5"), Column(Name:="conformation_status")> Public Property conformation_status As String
    <DatabaseField("gene_id_regulated"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="gene_id_regulated")> Public Property gene_id_regulated As String
    <DatabaseField("gene_name_regulated"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="gene_name_regulated")> Public Property gene_name_regulated As String
    <DatabaseField("generegulation_function"), DataType(MySqlDbType.VarChar, "9"), Column(Name:="generegulation_function")> Public Property generegulation_function As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `generegulation_tmp` (`gene_id_regulator`, `gene_name_regulator`, `tf_id_regulator`, `transcription_factor_name`, `tf_conformation`, `conformation_status`, `gene_id_regulated`, `gene_name_regulated`, `generegulation_function`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `generegulation_tmp` (`gene_id_regulator`, `gene_name_regulator`, `tf_id_regulator`, `transcription_factor_name`, `tf_conformation`, `conformation_status`, `gene_id_regulated`, `gene_name_regulated`, `generegulation_function`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `generegulation_tmp` (`gene_id_regulator`, `gene_name_regulator`, `tf_id_regulator`, `transcription_factor_name`, `tf_conformation`, `conformation_status`, `gene_id_regulated`, `gene_name_regulated`, `generegulation_function`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `generegulation_tmp` (`gene_id_regulator`, `gene_name_regulator`, `tf_id_regulator`, `transcription_factor_name`, `tf_conformation`, `conformation_status`, `gene_id_regulated`, `gene_name_regulated`, `generegulation_function`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `generegulation_tmp` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `generegulation_tmp` SET `gene_id_regulator`='{0}', `gene_name_regulator`='{1}', `tf_id_regulator`='{2}', `transcription_factor_name`='{3}', `tf_conformation`='{4}', `conformation_status`='{5}', `gene_id_regulated`='{6}', `gene_name_regulated`='{7}', `generegulation_function`='{8}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `generegulation_tmp` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `generegulation_tmp` (`gene_id_regulator`, `gene_name_regulator`, `tf_id_regulator`, `transcription_factor_name`, `tf_conformation`, `conformation_status`, `gene_id_regulated`, `gene_name_regulated`, `generegulation_function`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, gene_id_regulator, gene_name_regulator, tf_id_regulator, transcription_factor_name, tf_conformation, conformation_status, gene_id_regulated, gene_name_regulated, generegulation_function)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `generegulation_tmp` (`gene_id_regulator`, `gene_name_regulator`, `tf_id_regulator`, `transcription_factor_name`, `tf_conformation`, `conformation_status`, `gene_id_regulated`, `gene_name_regulated`, `generegulation_function`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, gene_id_regulator, gene_name_regulator, tf_id_regulator, transcription_factor_name, tf_conformation, conformation_status, gene_id_regulated, gene_name_regulated, generegulation_function)
        Else
        Return String.Format(INSERT_SQL, gene_id_regulator, gene_name_regulator, tf_id_regulator, transcription_factor_name, tf_conformation, conformation_status, gene_id_regulated, gene_name_regulated, generegulation_function)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{gene_id_regulator}', '{gene_name_regulator}', '{tf_id_regulator}', '{transcription_factor_name}', '{tf_conformation}', '{conformation_status}', '{gene_id_regulated}', '{gene_name_regulated}', '{generegulation_function}')"
        Else
            Return $"('{gene_id_regulator}', '{gene_name_regulator}', '{tf_id_regulator}', '{transcription_factor_name}', '{tf_conformation}', '{conformation_status}', '{gene_id_regulated}', '{gene_name_regulated}', '{generegulation_function}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `generegulation_tmp` (`gene_id_regulator`, `gene_name_regulator`, `tf_id_regulator`, `transcription_factor_name`, `tf_conformation`, `conformation_status`, `gene_id_regulated`, `gene_name_regulated`, `generegulation_function`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, gene_id_regulator, gene_name_regulator, tf_id_regulator, transcription_factor_name, tf_conformation, conformation_status, gene_id_regulated, gene_name_regulated, generegulation_function)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `generegulation_tmp` (`gene_id_regulator`, `gene_name_regulator`, `tf_id_regulator`, `transcription_factor_name`, `tf_conformation`, `conformation_status`, `gene_id_regulated`, `gene_name_regulated`, `generegulation_function`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, gene_id_regulator, gene_name_regulator, tf_id_regulator, transcription_factor_name, tf_conformation, conformation_status, gene_id_regulated, gene_name_regulated, generegulation_function)
        Else
        Return String.Format(REPLACE_SQL, gene_id_regulator, gene_name_regulator, tf_id_regulator, transcription_factor_name, tf_conformation, conformation_status, gene_id_regulated, gene_name_regulated, generegulation_function)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `generegulation_tmp` SET `gene_id_regulator`='{0}', `gene_name_regulator`='{1}', `tf_id_regulator`='{2}', `transcription_factor_name`='{3}', `tf_conformation`='{4}', `conformation_status`='{5}', `gene_id_regulated`='{6}', `gene_name_regulated`='{7}', `generegulation_function`='{8}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As generegulation_tmp
                         Return DirectCast(MyClass.MemberwiseClone, generegulation_tmp)
                     End Function
End Class


End Namespace
