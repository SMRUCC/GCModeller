#Region "Microsoft.VisualBasic::6c2dbe319d9ce6fc910838f124c27260, data\ExternalDBSource\MetaCyc\MySQL\synonymtable.vb"

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

    ' Class synonymtable
    ' 
    '     Properties: OtherWID, Syn
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @3/16/2018 10:40:19 PM


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace MetaCyc.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `synonymtable`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `synonymtable` (
'''   `OtherWID` bigint(20) NOT NULL,
'''   `Syn` varchar(255) NOT NULL,
'''   KEY `SYNONYM_OTHERWID_SYN` (`OtherWID`,`Syn`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("synonymtable", Database:="warehouse", SchemaSQL:="
CREATE TABLE `synonymtable` (
  `OtherWID` bigint(20) NOT NULL,
  `Syn` varchar(255) NOT NULL,
  KEY `SYNONYM_OTHERWID_SYN` (`OtherWID`,`Syn`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class synonymtable: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("OtherWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="OtherWID"), XmlAttribute> Public Property OtherWID As Long
    <DatabaseField("Syn"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="Syn"), XmlAttribute> Public Property Syn As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `synonymtable` (`OtherWID`, `Syn`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `synonymtable` (`OtherWID`, `Syn`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `synonymtable` WHERE `OtherWID`='{0}' and `Syn`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `synonymtable` SET `OtherWID`='{0}', `Syn`='{1}' WHERE `OtherWID`='{2}' and `Syn`='{3}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `synonymtable` WHERE `OtherWID`='{0}' and `Syn`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, OtherWID, Syn)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `synonymtable` (`OtherWID`, `Syn`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, OtherWID, Syn)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{OtherWID}', '{Syn}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `synonymtable` (`OtherWID`, `Syn`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, OtherWID, Syn)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `synonymtable` SET `OtherWID`='{0}', `Syn`='{1}' WHERE `OtherWID`='{2}' and `Syn`='{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, OtherWID, Syn, OtherWID, Syn)
    End Function
#End Region
Public Function Clone() As synonymtable
                  Return DirectCast(MyClass.MemberwiseClone, synonymtable)
              End Function
End Class


End Namespace
