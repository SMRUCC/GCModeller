#Region "Microsoft.VisualBasic::2d3fd8ef6e1a5f63023b0a8733703d50, data\ExternalDBSource\MetaCyc\MySQL\arraygroupwidarraywid.vb"

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

    ' Class arraygroupwidarraywid
    ' 
    '     Properties: ArrayGroupWID, ArrayWID
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
''' DROP TABLE IF EXISTS `arraygroupwidarraywid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `arraygroupwidarraywid` (
'''   `ArrayGroupWID` bigint(20) NOT NULL,
'''   `ArrayWID` bigint(20) NOT NULL,
'''   KEY `FK_ArrayGroupWIDArrayWID1` (`ArrayGroupWID`),
'''   KEY `FK_ArrayGroupWIDArrayWID2` (`ArrayWID`),
'''   CONSTRAINT `FK_ArrayGroupWIDArrayWID1` FOREIGN KEY (`ArrayGroupWID`) REFERENCES `arraygroup` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ArrayGroupWIDArrayWID2` FOREIGN KEY (`ArrayWID`) REFERENCES `array_` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("arraygroupwidarraywid", Database:="warehouse", SchemaSQL:="
CREATE TABLE `arraygroupwidarraywid` (
  `ArrayGroupWID` bigint(20) NOT NULL,
  `ArrayWID` bigint(20) NOT NULL,
  KEY `FK_ArrayGroupWIDArrayWID1` (`ArrayGroupWID`),
  KEY `FK_ArrayGroupWIDArrayWID2` (`ArrayWID`),
  CONSTRAINT `FK_ArrayGroupWIDArrayWID1` FOREIGN KEY (`ArrayGroupWID`) REFERENCES `arraygroup` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArrayGroupWIDArrayWID2` FOREIGN KEY (`ArrayWID`) REFERENCES `array_` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class arraygroupwidarraywid: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ArrayGroupWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="ArrayGroupWID"), XmlAttribute> Public Property ArrayGroupWID As Long
    <DatabaseField("ArrayWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="ArrayWID")> Public Property ArrayWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `arraygroupwidarraywid` (`ArrayGroupWID`, `ArrayWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `arraygroupwidarraywid` (`ArrayGroupWID`, `ArrayWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `arraygroupwidarraywid` WHERE `ArrayGroupWID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `arraygroupwidarraywid` SET `ArrayGroupWID`='{0}', `ArrayWID`='{1}' WHERE `ArrayGroupWID` = '{2}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `arraygroupwidarraywid` WHERE `ArrayGroupWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ArrayGroupWID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `arraygroupwidarraywid` (`ArrayGroupWID`, `ArrayWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ArrayGroupWID, ArrayWID)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{ArrayGroupWID}', '{ArrayWID}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `arraygroupwidarraywid` (`ArrayGroupWID`, `ArrayWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ArrayGroupWID, ArrayWID)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `arraygroupwidarraywid` SET `ArrayGroupWID`='{0}', `ArrayWID`='{1}' WHERE `ArrayGroupWID` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ArrayGroupWID, ArrayWID, ArrayGroupWID)
    End Function
#End Region
Public Function Clone() As arraygroupwidarraywid
                  Return DirectCast(MyClass.MemberwiseClone, arraygroupwidarraywid)
              End Function
End Class


End Namespace
