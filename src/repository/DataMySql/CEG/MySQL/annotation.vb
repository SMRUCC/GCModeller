#Region "Microsoft.VisualBasic::4dd6c4b5de17450d6c234f1761069014, ..\repository\DataMySql\CEG\MySQL\annotation.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 11:05:12 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace CEG.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `annotation`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `annotation` (
'''   `gid` varchar(25) DEFAULT NULL,
'''   `Gene_Name` varchar(80) DEFAULT NULL,
'''   `fundescrp` varchar(255) DEFAULT NULL
''' ) ENGINE=MyISAM AUTO_INCREMENT=11862 DEFAULT CHARSET=gb2312;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("annotation", Database:="ceg", SchemaSQL:="
CREATE TABLE `annotation` (
  `gid` varchar(25) DEFAULT NULL,
  `Gene_Name` varchar(80) DEFAULT NULL,
  `fundescrp` varchar(255) DEFAULT NULL
) ENGINE=MyISAM AUTO_INCREMENT=11862 DEFAULT CHARSET=gb2312;")>
Public Class annotation: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("gid"), DataType(MySqlDbType.VarChar, "25")> Public Property gid As String
    <DatabaseField("Gene_Name"), DataType(MySqlDbType.VarChar, "80")> Public Property Gene_Name As String
    <DatabaseField("fundescrp"), DataType(MySqlDbType.VarChar, "255")> Public Property fundescrp As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `annotation` (`gid`, `Gene_Name`, `fundescrp`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `annotation` (`gid`, `Gene_Name`, `fundescrp`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `annotation` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `annotation` SET `gid`='{0}', `Gene_Name`='{1}', `fundescrp`='{2}' WHERE ;</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `annotation` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `annotation` (`gid`, `Gene_Name`, `fundescrp`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, gid, Gene_Name, fundescrp)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{gid}', '{Gene_Name}', '{fundescrp}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `annotation` (`gid`, `Gene_Name`, `fundescrp`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, gid, Gene_Name, fundescrp)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `annotation` SET `gid`='{0}', `Gene_Name`='{1}', `fundescrp`='{2}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace

