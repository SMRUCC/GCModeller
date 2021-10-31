#Region "Microsoft.VisualBasic::eb81a64a8dc81456b1439f40847fc257, DataMySql\Xfam\Rfam\Tables\motif_file.vb"

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

    ' Class motif_file
    ' 
    '     Properties: cm, motif_acc, seed
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

REM  Dump @2018/5/23 13:13:34


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace Xfam.Rfam.MySQL.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `motif_file`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `motif_file` (
'''   `motif_acc` varchar(7) NOT NULL,
'''   `seed` longblob NOT NULL,
'''   `cm` longblob NOT NULL,
'''   KEY `fk_motif_file_motif_idx` (`motif_acc`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("motif_file", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `motif_file` (
  `motif_acc` varchar(7) NOT NULL,
  `seed` longblob NOT NULL,
  `cm` longblob NOT NULL,
  KEY `fk_motif_file_motif_idx` (`motif_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class motif_file: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("motif_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="motif_acc"), XmlAttribute> Public Property motif_acc As String
    <DatabaseField("seed"), NotNull, DataType(MySqlDbType.Blob), Column(Name:="seed")> Public Property seed As Byte()
    <DatabaseField("cm"), NotNull, DataType(MySqlDbType.Blob), Column(Name:="cm")> Public Property cm As Byte()
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `motif_file` (`motif_acc`, `seed`, `cm`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `motif_file` (`motif_acc`, `seed`, `cm`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `motif_file` (`motif_acc`, `seed`, `cm`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `motif_file` (`motif_acc`, `seed`, `cm`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `motif_file` WHERE `motif_acc` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `motif_file` SET `motif_acc`='{0}', `seed`='{1}', `cm`='{2}' WHERE `motif_acc` = '{3}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `motif_file` WHERE `motif_acc` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, motif_acc)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `motif_file` (`motif_acc`, `seed`, `cm`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, motif_acc, seed, cm)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `motif_file` (`motif_acc`, `seed`, `cm`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, motif_acc, seed, cm)
        Else
        Return String.Format(INSERT_SQL, motif_acc, seed, cm)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{motif_acc}', '{seed}', '{cm}')"
        Else
            Return $"('{motif_acc}', '{seed}', '{cm}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `motif_file` (`motif_acc`, `seed`, `cm`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, motif_acc, seed, cm)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `motif_file` (`motif_acc`, `seed`, `cm`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, motif_acc, seed, cm)
        Else
        Return String.Format(REPLACE_SQL, motif_acc, seed, cm)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `motif_file` SET `motif_acc`='{0}', `seed`='{1}', `cm`='{2}' WHERE `motif_acc` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, motif_acc, seed, cm, motif_acc)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As motif_file
                         Return DirectCast(MyClass.MemberwiseClone, motif_file)
                     End Function
End Class


End Namespace
