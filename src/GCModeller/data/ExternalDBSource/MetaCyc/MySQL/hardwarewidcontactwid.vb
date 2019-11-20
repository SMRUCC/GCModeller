#Region "Microsoft.VisualBasic::983b052d253cdba7e156759d9c65f83e, data\ExternalDBSource\MetaCyc\MySQL\hardwarewidcontactwid.vb"

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

    ' Class hardwarewidcontactwid
    ' 
    '     Properties: ContactWID, HardwareWID
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
''' DROP TABLE IF EXISTS `hardwarewidcontactwid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `hardwarewidcontactwid` (
'''   `HardwareWID` bigint(20) NOT NULL,
'''   `ContactWID` bigint(20) NOT NULL,
'''   KEY `FK_HardwareWIDContactWID1` (`HardwareWID`),
'''   KEY `FK_HardwareWIDContactWID2` (`ContactWID`),
'''   CONSTRAINT `FK_HardwareWIDContactWID1` FOREIGN KEY (`HardwareWID`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_HardwareWIDContactWID2` FOREIGN KEY (`ContactWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("hardwarewidcontactwid", Database:="warehouse", SchemaSQL:="
CREATE TABLE `hardwarewidcontactwid` (
  `HardwareWID` bigint(20) NOT NULL,
  `ContactWID` bigint(20) NOT NULL,
  KEY `FK_HardwareWIDContactWID1` (`HardwareWID`),
  KEY `FK_HardwareWIDContactWID2` (`ContactWID`),
  CONSTRAINT `FK_HardwareWIDContactWID1` FOREIGN KEY (`HardwareWID`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_HardwareWIDContactWID2` FOREIGN KEY (`ContactWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class hardwarewidcontactwid: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("HardwareWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="HardwareWID"), XmlAttribute> Public Property HardwareWID As Long
    <DatabaseField("ContactWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="ContactWID")> Public Property ContactWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `hardwarewidcontactwid` (`HardwareWID`, `ContactWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `hardwarewidcontactwid` (`HardwareWID`, `ContactWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `hardwarewidcontactwid` (`HardwareWID`, `ContactWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `hardwarewidcontactwid` (`HardwareWID`, `ContactWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `hardwarewidcontactwid` WHERE `HardwareWID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `hardwarewidcontactwid` SET `HardwareWID`='{0}', `ContactWID`='{1}' WHERE `HardwareWID` = '{2}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `hardwarewidcontactwid` WHERE `HardwareWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, HardwareWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `hardwarewidcontactwid` (`HardwareWID`, `ContactWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, HardwareWID, ContactWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `hardwarewidcontactwid` (`HardwareWID`, `ContactWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, HardwareWID, ContactWID)
        Else
        Return String.Format(INSERT_SQL, HardwareWID, ContactWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{HardwareWID}', '{ContactWID}')"
        Else
            Return $"('{HardwareWID}', '{ContactWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `hardwarewidcontactwid` (`HardwareWID`, `ContactWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, HardwareWID, ContactWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `hardwarewidcontactwid` (`HardwareWID`, `ContactWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, HardwareWID, ContactWID)
        Else
        Return String.Format(REPLACE_SQL, HardwareWID, ContactWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `hardwarewidcontactwid` SET `HardwareWID`='{0}', `ContactWID`='{1}' WHERE `HardwareWID` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, HardwareWID, ContactWID, HardwareWID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As hardwarewidcontactwid
                         Return DirectCast(MyClass.MemberwiseClone, hardwarewidcontactwid)
                     End Function
End Class


End Namespace
