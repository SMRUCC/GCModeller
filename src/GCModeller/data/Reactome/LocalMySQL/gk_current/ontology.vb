#Region "Microsoft.VisualBasic::bc20e838a8d62fbad98331ed845b5958, data\Reactome\LocalMySQL\gk_current\ontology.vb"

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

    '   Total Lines: 146
    '    Code Lines: 71 (48.63%)
    ' Comment Lines: 53 (36.30%)
    '    - Xml Docs: 94.34%
    ' 
    '   Blank Lines: 22 (15.07%)
    '     File Size: 4.59 KB


    ' Class ontology
    ' 
    '     Properties: ontology
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

REM  Dump @2018/5/23 13:13:41


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `ontology`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `ontology` (
'''   `ontology` longblob
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("ontology", Database:="gk_current", SchemaSQL:="
CREATE TABLE `ontology` (
  `ontology` longblob
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class ontology: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ontology"), DataType(MySqlDbType.Blob), Column(Name:="ontology")> Public Property ontology As Byte()
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `ontology` (`ontology`) VALUES ('{0}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `ontology` (`ontology`) VALUES ('{0}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `ontology` (`ontology`) VALUES ('{0}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `ontology` (`ontology`) VALUES ('{0}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `ontology` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `ontology` SET `ontology`='{0}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `ontology` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `ontology` (`ontology`) VALUES ('{0}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ontology)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `ontology` (`ontology`) VALUES ('{0}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, ontology)
        Else
        Return String.Format(INSERT_SQL, ontology)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{ontology}')"
        Else
            Return $"('{ontology}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `ontology` (`ontology`) VALUES ('{0}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ontology)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `ontology` (`ontology`) VALUES ('{0}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, ontology)
        Else
        Return String.Format(REPLACE_SQL, ontology)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `ontology` SET `ontology`='{0}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As ontology
                         Return DirectCast(MyClass.MemberwiseClone, ontology)
                     End Function
End Class


End Namespace
