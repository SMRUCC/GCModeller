#Region "Microsoft.VisualBasic::5093b1ea56ab076f1da44098822ea433, data\ExternalDBSource\MetaCyc\MySQL\nodecontents.vb"

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

    ' Class nodecontents
    ' 
    '     Properties: BioAssayDimension, DataSetWID, DesignElementDimension, Node_NodeContents, QuantitationDimension
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
''' DROP TABLE IF EXISTS `nodecontents`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `nodecontents` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `Node_NodeContents` bigint(20) DEFAULT NULL,
'''   `BioAssayDimension` bigint(20) DEFAULT NULL,
'''   `DesignElementDimension` bigint(20) DEFAULT NULL,
'''   `QuantitationDimension` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_NodeContents1` (`DataSetWID`),
'''   KEY `FK_NodeContents3` (`Node_NodeContents`),
'''   KEY `FK_NodeContents4` (`BioAssayDimension`),
'''   KEY `FK_NodeContents5` (`DesignElementDimension`),
'''   KEY `FK_NodeContents6` (`QuantitationDimension`),
'''   CONSTRAINT `FK_NodeContents1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_NodeContents3` FOREIGN KEY (`Node_NodeContents`) REFERENCES `node` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_NodeContents4` FOREIGN KEY (`BioAssayDimension`) REFERENCES `bioassaydimension` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_NodeContents5` FOREIGN KEY (`DesignElementDimension`) REFERENCES `designelementdimension` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_NodeContents6` FOREIGN KEY (`QuantitationDimension`) REFERENCES `quantitationtypedimension` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("nodecontents", Database:="warehouse", SchemaSQL:="
CREATE TABLE `nodecontents` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Node_NodeContents` bigint(20) DEFAULT NULL,
  `BioAssayDimension` bigint(20) DEFAULT NULL,
  `DesignElementDimension` bigint(20) DEFAULT NULL,
  `QuantitationDimension` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_NodeContents1` (`DataSetWID`),
  KEY `FK_NodeContents3` (`Node_NodeContents`),
  KEY `FK_NodeContents4` (`BioAssayDimension`),
  KEY `FK_NodeContents5` (`DesignElementDimension`),
  KEY `FK_NodeContents6` (`QuantitationDimension`),
  CONSTRAINT `FK_NodeContents1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_NodeContents3` FOREIGN KEY (`Node_NodeContents`) REFERENCES `node` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_NodeContents4` FOREIGN KEY (`BioAssayDimension`) REFERENCES `bioassaydimension` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_NodeContents5` FOREIGN KEY (`DesignElementDimension`) REFERENCES `designelementdimension` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_NodeContents6` FOREIGN KEY (`QuantitationDimension`) REFERENCES `quantitationtypedimension` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class nodecontents: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("Node_NodeContents"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Node_NodeContents")> Public Property Node_NodeContents As Long
    <DatabaseField("BioAssayDimension"), DataType(MySqlDbType.Int64, "20"), Column(Name:="BioAssayDimension")> Public Property BioAssayDimension As Long
    <DatabaseField("DesignElementDimension"), DataType(MySqlDbType.Int64, "20"), Column(Name:="DesignElementDimension")> Public Property DesignElementDimension As Long
    <DatabaseField("QuantitationDimension"), DataType(MySqlDbType.Int64, "20"), Column(Name:="QuantitationDimension")> Public Property QuantitationDimension As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `nodecontents` (`WID`, `DataSetWID`, `Node_NodeContents`, `BioAssayDimension`, `DesignElementDimension`, `QuantitationDimension`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `nodecontents` (`WID`, `DataSetWID`, `Node_NodeContents`, `BioAssayDimension`, `DesignElementDimension`, `QuantitationDimension`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `nodecontents` (`WID`, `DataSetWID`, `Node_NodeContents`, `BioAssayDimension`, `DesignElementDimension`, `QuantitationDimension`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `nodecontents` (`WID`, `DataSetWID`, `Node_NodeContents`, `BioAssayDimension`, `DesignElementDimension`, `QuantitationDimension`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `nodecontents` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `nodecontents` SET `WID`='{0}', `DataSetWID`='{1}', `Node_NodeContents`='{2}', `BioAssayDimension`='{3}', `DesignElementDimension`='{4}', `QuantitationDimension`='{5}' WHERE `WID` = '{6}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `nodecontents` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `nodecontents` (`WID`, `DataSetWID`, `Node_NodeContents`, `BioAssayDimension`, `DesignElementDimension`, `QuantitationDimension`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, Node_NodeContents, BioAssayDimension, DesignElementDimension, QuantitationDimension)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `nodecontents` (`WID`, `DataSetWID`, `Node_NodeContents`, `BioAssayDimension`, `DesignElementDimension`, `QuantitationDimension`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, DataSetWID, Node_NodeContents, BioAssayDimension, DesignElementDimension, QuantitationDimension)
        Else
        Return String.Format(INSERT_SQL, WID, DataSetWID, Node_NodeContents, BioAssayDimension, DesignElementDimension, QuantitationDimension)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{DataSetWID}', '{Node_NodeContents}', '{BioAssayDimension}', '{DesignElementDimension}', '{QuantitationDimension}')"
        Else
            Return $"('{WID}', '{DataSetWID}', '{Node_NodeContents}', '{BioAssayDimension}', '{DesignElementDimension}', '{QuantitationDimension}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `nodecontents` (`WID`, `DataSetWID`, `Node_NodeContents`, `BioAssayDimension`, `DesignElementDimension`, `QuantitationDimension`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Node_NodeContents, BioAssayDimension, DesignElementDimension, QuantitationDimension)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `nodecontents` (`WID`, `DataSetWID`, `Node_NodeContents`, `BioAssayDimension`, `DesignElementDimension`, `QuantitationDimension`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, DataSetWID, Node_NodeContents, BioAssayDimension, DesignElementDimension, QuantitationDimension)
        Else
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Node_NodeContents, BioAssayDimension, DesignElementDimension, QuantitationDimension)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `nodecontents` SET `WID`='{0}', `DataSetWID`='{1}', `Node_NodeContents`='{2}', `BioAssayDimension`='{3}', `DesignElementDimension`='{4}', `QuantitationDimension`='{5}' WHERE `WID` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, Node_NodeContents, BioAssayDimension, DesignElementDimension, QuantitationDimension, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As nodecontents
                         Return DirectCast(MyClass.MemberwiseClone, nodecontents)
                     End Function
End Class


End Namespace
