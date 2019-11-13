#Region "Microsoft.VisualBasic::93b39f50cf0a0d03cb51c69b59d006a2, data\ExternalDBSource\MetaCyc\MySQL\array_.vb"

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

    ' Class array_
    ' 
    '     Properties: ArrayDesign, ArrayGroup, ArrayIdentifier, ArrayXOrigin, ArrayYOrigin
    '                 DataSetWID, Identifier, Information, Name, OriginRelativeTo
    '                 WID
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

REM  Dump @2018/5/23 13:13:40


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
''' DROP TABLE IF EXISTS `array_`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `array_` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `Identifier` varchar(255) DEFAULT NULL,
'''   `Name` varchar(255) DEFAULT NULL,
'''   `ArrayIdentifier` varchar(255) DEFAULT NULL,
'''   `ArrayXOrigin` float DEFAULT NULL,
'''   `ArrayYOrigin` float DEFAULT NULL,
'''   `OriginRelativeTo` varchar(255) DEFAULT NULL,
'''   `ArrayDesign` bigint(20) DEFAULT NULL,
'''   `Information` bigint(20) DEFAULT NULL,
'''   `ArrayGroup` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_Array_1` (`DataSetWID`),
'''   KEY `FK_Array_3` (`ArrayDesign`),
'''   KEY `FK_Array_4` (`Information`),
'''   KEY `FK_Array_5` (`ArrayGroup`),
'''   CONSTRAINT `FK_Array_1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Array_3` FOREIGN KEY (`ArrayDesign`) REFERENCES `arraydesign` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Array_4` FOREIGN KEY (`Information`) REFERENCES `arraymanufacture` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Array_5` FOREIGN KEY (`ArrayGroup`) REFERENCES `arraygroup` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("array_", Database:="warehouse", SchemaSQL:="
CREATE TABLE `array_` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `ArrayIdentifier` varchar(255) DEFAULT NULL,
  `ArrayXOrigin` float DEFAULT NULL,
  `ArrayYOrigin` float DEFAULT NULL,
  `OriginRelativeTo` varchar(255) DEFAULT NULL,
  `ArrayDesign` bigint(20) DEFAULT NULL,
  `Information` bigint(20) DEFAULT NULL,
  `ArrayGroup` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Array_1` (`DataSetWID`),
  KEY `FK_Array_3` (`ArrayDesign`),
  KEY `FK_Array_4` (`Information`),
  KEY `FK_Array_5` (`ArrayGroup`),
  CONSTRAINT `FK_Array_1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Array_3` FOREIGN KEY (`ArrayDesign`) REFERENCES `arraydesign` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Array_4` FOREIGN KEY (`Information`) REFERENCES `arraymanufacture` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Array_5` FOREIGN KEY (`ArrayGroup`) REFERENCES `arraygroup` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class array_: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("Identifier"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Identifier")> Public Property Identifier As String
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Name")> Public Property Name As String
    <DatabaseField("ArrayIdentifier"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="ArrayIdentifier")> Public Property ArrayIdentifier As String
    <DatabaseField("ArrayXOrigin"), DataType(MySqlDbType.Double), Column(Name:="ArrayXOrigin")> Public Property ArrayXOrigin As Double
    <DatabaseField("ArrayYOrigin"), DataType(MySqlDbType.Double), Column(Name:="ArrayYOrigin")> Public Property ArrayYOrigin As Double
    <DatabaseField("OriginRelativeTo"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="OriginRelativeTo")> Public Property OriginRelativeTo As String
    <DatabaseField("ArrayDesign"), DataType(MySqlDbType.Int64, "20"), Column(Name:="ArrayDesign")> Public Property ArrayDesign As Long
    <DatabaseField("Information"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Information")> Public Property Information As Long
    <DatabaseField("ArrayGroup"), DataType(MySqlDbType.Int64, "20"), Column(Name:="ArrayGroup")> Public Property ArrayGroup As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `array_` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ArrayIdentifier`, `ArrayXOrigin`, `ArrayYOrigin`, `OriginRelativeTo`, `ArrayDesign`, `Information`, `ArrayGroup`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `array_` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ArrayIdentifier`, `ArrayXOrigin`, `ArrayYOrigin`, `OriginRelativeTo`, `ArrayDesign`, `Information`, `ArrayGroup`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `array_` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ArrayIdentifier`, `ArrayXOrigin`, `ArrayYOrigin`, `OriginRelativeTo`, `ArrayDesign`, `Information`, `ArrayGroup`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `array_` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ArrayIdentifier`, `ArrayXOrigin`, `ArrayYOrigin`, `OriginRelativeTo`, `ArrayDesign`, `Information`, `ArrayGroup`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `array_` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `array_` SET `WID`='{0}', `DataSetWID`='{1}', `Identifier`='{2}', `Name`='{3}', `ArrayIdentifier`='{4}', `ArrayXOrigin`='{5}', `ArrayYOrigin`='{6}', `OriginRelativeTo`='{7}', `ArrayDesign`='{8}', `Information`='{9}', `ArrayGroup`='{10}' WHERE `WID` = '{11}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `array_` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `array_` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ArrayIdentifier`, `ArrayXOrigin`, `ArrayYOrigin`, `OriginRelativeTo`, `ArrayDesign`, `Information`, `ArrayGroup`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, Identifier, Name, ArrayIdentifier, ArrayXOrigin, ArrayYOrigin, OriginRelativeTo, ArrayDesign, Information, ArrayGroup)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `array_` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ArrayIdentifier`, `ArrayXOrigin`, `ArrayYOrigin`, `OriginRelativeTo`, `ArrayDesign`, `Information`, `ArrayGroup`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, DataSetWID, Identifier, Name, ArrayIdentifier, ArrayXOrigin, ArrayYOrigin, OriginRelativeTo, ArrayDesign, Information, ArrayGroup)
        Else
        Return String.Format(INSERT_SQL, WID, DataSetWID, Identifier, Name, ArrayIdentifier, ArrayXOrigin, ArrayYOrigin, OriginRelativeTo, ArrayDesign, Information, ArrayGroup)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{DataSetWID}', '{Identifier}', '{Name}', '{ArrayIdentifier}', '{ArrayXOrigin}', '{ArrayYOrigin}', '{OriginRelativeTo}', '{ArrayDesign}', '{Information}', '{ArrayGroup}')"
        Else
            Return $"('{WID}', '{DataSetWID}', '{Identifier}', '{Name}', '{ArrayIdentifier}', '{ArrayXOrigin}', '{ArrayYOrigin}', '{OriginRelativeTo}', '{ArrayDesign}', '{Information}', '{ArrayGroup}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `array_` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ArrayIdentifier`, `ArrayXOrigin`, `ArrayYOrigin`, `OriginRelativeTo`, `ArrayDesign`, `Information`, `ArrayGroup`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Identifier, Name, ArrayIdentifier, ArrayXOrigin, ArrayYOrigin, OriginRelativeTo, ArrayDesign, Information, ArrayGroup)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `array_` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ArrayIdentifier`, `ArrayXOrigin`, `ArrayYOrigin`, `OriginRelativeTo`, `ArrayDesign`, `Information`, `ArrayGroup`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, DataSetWID, Identifier, Name, ArrayIdentifier, ArrayXOrigin, ArrayYOrigin, OriginRelativeTo, ArrayDesign, Information, ArrayGroup)
        Else
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Identifier, Name, ArrayIdentifier, ArrayXOrigin, ArrayYOrigin, OriginRelativeTo, ArrayDesign, Information, ArrayGroup)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `array_` SET `WID`='{0}', `DataSetWID`='{1}', `Identifier`='{2}', `Name`='{3}', `ArrayIdentifier`='{4}', `ArrayXOrigin`='{5}', `ArrayYOrigin`='{6}', `OriginRelativeTo`='{7}', `ArrayDesign`='{8}', `Information`='{9}', `ArrayGroup`='{10}' WHERE `WID` = '{11}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, Identifier, Name, ArrayIdentifier, ArrayXOrigin, ArrayYOrigin, OriginRelativeTo, ArrayDesign, Information, ArrayGroup, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As array_
                         Return DirectCast(MyClass.MemberwiseClone, array_)
                     End Function
End Class


End Namespace
