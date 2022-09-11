#Region "Microsoft.VisualBasic::69da3d46da91eb40104bf94bb7440a70, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\reg_phrase.vb"

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


    ' Class reg_phrase
    ' 
    '     Properties: key_id_org, off_half_life, on_half_life, phrase, reg_phrase_description
    '                 reg_phrase_function, reg_phrase_id, reg_phrase_internal_comment, reg_phrase_note, regulation_ratio
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
''' DROP TABLE IF EXISTS `reg_phrase`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `reg_phrase` (
'''   `reg_phrase_id` char(12) NOT NULL,
'''   `reg_phrase_description` varchar(255) DEFAULT NULL,
'''   `regulation_ratio` varchar(20) DEFAULT NULL,
'''   `on_half_life` decimal(20,5) DEFAULT NULL,
'''   `off_half_life` decimal(20,5) DEFAULT NULL,
'''   `phrase` varchar(2000) NOT NULL,
'''   `reg_phrase_function` varchar(25) DEFAULT NULL,
'''   `reg_phrase_note` varchar(2000) DEFAULT NULL,
'''   `reg_phrase_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("reg_phrase", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `reg_phrase` (
  `reg_phrase_id` char(12) NOT NULL,
  `reg_phrase_description` varchar(255) DEFAULT NULL,
  `regulation_ratio` varchar(20) DEFAULT NULL,
  `on_half_life` decimal(20,5) DEFAULT NULL,
  `off_half_life` decimal(20,5) DEFAULT NULL,
  `phrase` varchar(2000) NOT NULL,
  `reg_phrase_function` varchar(25) DEFAULT NULL,
  `reg_phrase_note` varchar(2000) DEFAULT NULL,
  `reg_phrase_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class reg_phrase: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("reg_phrase_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="reg_phrase_id")> Public Property reg_phrase_id As String
    <DatabaseField("reg_phrase_description"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="reg_phrase_description")> Public Property reg_phrase_description As String
    <DatabaseField("regulation_ratio"), DataType(MySqlDbType.VarChar, "20"), Column(Name:="regulation_ratio")> Public Property regulation_ratio As String
    <DatabaseField("on_half_life"), DataType(MySqlDbType.Decimal), Column(Name:="on_half_life")> Public Property on_half_life As Decimal
    <DatabaseField("off_half_life"), DataType(MySqlDbType.Decimal), Column(Name:="off_half_life")> Public Property off_half_life As Decimal
    <DatabaseField("phrase"), NotNull, DataType(MySqlDbType.VarChar, "2000"), Column(Name:="phrase")> Public Property phrase As String
    <DatabaseField("reg_phrase_function"), DataType(MySqlDbType.VarChar, "25"), Column(Name:="reg_phrase_function")> Public Property reg_phrase_function As String
    <DatabaseField("reg_phrase_note"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="reg_phrase_note")> Public Property reg_phrase_note As String
    <DatabaseField("reg_phrase_internal_comment"), DataType(MySqlDbType.Text), Column(Name:="reg_phrase_internal_comment")> Public Property reg_phrase_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="key_id_org")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `reg_phrase` (`reg_phrase_id`, `reg_phrase_description`, `regulation_ratio`, `on_half_life`, `off_half_life`, `phrase`, `reg_phrase_function`, `reg_phrase_note`, `reg_phrase_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `reg_phrase` (`reg_phrase_id`, `reg_phrase_description`, `regulation_ratio`, `on_half_life`, `off_half_life`, `phrase`, `reg_phrase_function`, `reg_phrase_note`, `reg_phrase_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `reg_phrase` (`reg_phrase_id`, `reg_phrase_description`, `regulation_ratio`, `on_half_life`, `off_half_life`, `phrase`, `reg_phrase_function`, `reg_phrase_note`, `reg_phrase_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `reg_phrase` (`reg_phrase_id`, `reg_phrase_description`, `regulation_ratio`, `on_half_life`, `off_half_life`, `phrase`, `reg_phrase_function`, `reg_phrase_note`, `reg_phrase_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `reg_phrase` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `reg_phrase` SET `reg_phrase_id`='{0}', `reg_phrase_description`='{1}', `regulation_ratio`='{2}', `on_half_life`='{3}', `off_half_life`='{4}', `phrase`='{5}', `reg_phrase_function`='{6}', `reg_phrase_note`='{7}', `reg_phrase_internal_comment`='{8}', `key_id_org`='{9}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `reg_phrase` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `reg_phrase` (`reg_phrase_id`, `reg_phrase_description`, `regulation_ratio`, `on_half_life`, `off_half_life`, `phrase`, `reg_phrase_function`, `reg_phrase_note`, `reg_phrase_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, reg_phrase_id, reg_phrase_description, regulation_ratio, on_half_life, off_half_life, phrase, reg_phrase_function, reg_phrase_note, reg_phrase_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `reg_phrase` (`reg_phrase_id`, `reg_phrase_description`, `regulation_ratio`, `on_half_life`, `off_half_life`, `phrase`, `reg_phrase_function`, `reg_phrase_note`, `reg_phrase_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, reg_phrase_id, reg_phrase_description, regulation_ratio, on_half_life, off_half_life, phrase, reg_phrase_function, reg_phrase_note, reg_phrase_internal_comment, key_id_org)
        Else
        Return String.Format(INSERT_SQL, reg_phrase_id, reg_phrase_description, regulation_ratio, on_half_life, off_half_life, phrase, reg_phrase_function, reg_phrase_note, reg_phrase_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{reg_phrase_id}', '{reg_phrase_description}', '{regulation_ratio}', '{on_half_life}', '{off_half_life}', '{phrase}', '{reg_phrase_function}', '{reg_phrase_note}', '{reg_phrase_internal_comment}', '{key_id_org}')"
        Else
            Return $"('{reg_phrase_id}', '{reg_phrase_description}', '{regulation_ratio}', '{on_half_life}', '{off_half_life}', '{phrase}', '{reg_phrase_function}', '{reg_phrase_note}', '{reg_phrase_internal_comment}', '{key_id_org}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `reg_phrase` (`reg_phrase_id`, `reg_phrase_description`, `regulation_ratio`, `on_half_life`, `off_half_life`, `phrase`, `reg_phrase_function`, `reg_phrase_note`, `reg_phrase_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, reg_phrase_id, reg_phrase_description, regulation_ratio, on_half_life, off_half_life, phrase, reg_phrase_function, reg_phrase_note, reg_phrase_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `reg_phrase` (`reg_phrase_id`, `reg_phrase_description`, `regulation_ratio`, `on_half_life`, `off_half_life`, `phrase`, `reg_phrase_function`, `reg_phrase_note`, `reg_phrase_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, reg_phrase_id, reg_phrase_description, regulation_ratio, on_half_life, off_half_life, phrase, reg_phrase_function, reg_phrase_note, reg_phrase_internal_comment, key_id_org)
        Else
        Return String.Format(REPLACE_SQL, reg_phrase_id, reg_phrase_description, regulation_ratio, on_half_life, off_half_life, phrase, reg_phrase_function, reg_phrase_note, reg_phrase_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `reg_phrase` SET `reg_phrase_id`='{0}', `reg_phrase_description`='{1}', `regulation_ratio`='{2}', `on_half_life`='{3}', `off_half_life`='{4}', `phrase`='{5}', `reg_phrase_function`='{6}', `reg_phrase_note`='{7}', `reg_phrase_internal_comment`='{8}', `key_id_org`='{9}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As reg_phrase
                         Return DirectCast(MyClass.MemberwiseClone, reg_phrase)
                     End Function
End Class


End Namespace
