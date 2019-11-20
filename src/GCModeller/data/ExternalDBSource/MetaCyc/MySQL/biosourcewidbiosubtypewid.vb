#Region "Microsoft.VisualBasic::92faff59d4725ce2bb4bd381a5b8c11e, data\ExternalDBSource\MetaCyc\MySQL\biosourcewidbiosubtypewid.vb"

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

    ' Class biosourcewidbiosubtypewid
    ' 
    '     Properties: BioSourceWID, BioSubtypeWID
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
''' DROP TABLE IF EXISTS `biosourcewidbiosubtypewid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `biosourcewidbiosubtypewid` (
'''   `BioSourceWID` bigint(20) NOT NULL,
'''   `BioSubtypeWID` bigint(20) NOT NULL,
'''   KEY `FK_BioSourceWIDBioSubtypeWID1` (`BioSourceWID`),
'''   KEY `FK_BioSourceWIDBioSubtypeWID2` (`BioSubtypeWID`),
'''   CONSTRAINT `FK_BioSourceWIDBioSubtypeWID1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioSourceWIDBioSubtypeWID2` FOREIGN KEY (`BioSubtypeWID`) REFERENCES `biosubtype` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("biosourcewidbiosubtypewid", Database:="warehouse", SchemaSQL:="
CREATE TABLE `biosourcewidbiosubtypewid` (
  `BioSourceWID` bigint(20) NOT NULL,
  `BioSubtypeWID` bigint(20) NOT NULL,
  KEY `FK_BioSourceWIDBioSubtypeWID1` (`BioSourceWID`),
  KEY `FK_BioSourceWIDBioSubtypeWID2` (`BioSubtypeWID`),
  CONSTRAINT `FK_BioSourceWIDBioSubtypeWID1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioSourceWIDBioSubtypeWID2` FOREIGN KEY (`BioSubtypeWID`) REFERENCES `biosubtype` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class biosourcewidbiosubtypewid: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("BioSourceWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="BioSourceWID"), XmlAttribute> Public Property BioSourceWID As Long
    <DatabaseField("BioSubtypeWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="BioSubtypeWID")> Public Property BioSubtypeWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `biosourcewidbiosubtypewid` (`BioSourceWID`, `BioSubtypeWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `biosourcewidbiosubtypewid` (`BioSourceWID`, `BioSubtypeWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `biosourcewidbiosubtypewid` (`BioSourceWID`, `BioSubtypeWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `biosourcewidbiosubtypewid` (`BioSourceWID`, `BioSubtypeWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `biosourcewidbiosubtypewid` WHERE `BioSourceWID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `biosourcewidbiosubtypewid` SET `BioSourceWID`='{0}', `BioSubtypeWID`='{1}' WHERE `BioSourceWID` = '{2}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `biosourcewidbiosubtypewid` WHERE `BioSourceWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, BioSourceWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `biosourcewidbiosubtypewid` (`BioSourceWID`, `BioSubtypeWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, BioSourceWID, BioSubtypeWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `biosourcewidbiosubtypewid` (`BioSourceWID`, `BioSubtypeWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, BioSourceWID, BioSubtypeWID)
        Else
        Return String.Format(INSERT_SQL, BioSourceWID, BioSubtypeWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{BioSourceWID}', '{BioSubtypeWID}')"
        Else
            Return $"('{BioSourceWID}', '{BioSubtypeWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `biosourcewidbiosubtypewid` (`BioSourceWID`, `BioSubtypeWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, BioSourceWID, BioSubtypeWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `biosourcewidbiosubtypewid` (`BioSourceWID`, `BioSubtypeWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, BioSourceWID, BioSubtypeWID)
        Else
        Return String.Format(REPLACE_SQL, BioSourceWID, BioSubtypeWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `biosourcewidbiosubtypewid` SET `BioSourceWID`='{0}', `BioSubtypeWID`='{1}' WHERE `BioSourceWID` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, BioSourceWID, BioSubtypeWID, BioSourceWID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As biosourcewidbiosubtypewid
                         Return DirectCast(MyClass.MemberwiseClone, biosourcewidbiosubtypewid)
                     End Function
End Class


End Namespace
