#Region "Microsoft.VisualBasic::3968027bc6e7bdf8bb5a017b80103241, GCModeller\data\MicrobesOnline\MySQL\genomics\compound.vb"

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

    '   Total Lines: 71
    '    Code Lines: 40
    ' Comment Lines: 24
    '   Blank Lines: 7
    '     File Size: 3.54 KB


    ' Class compound
    ' 
    '     Properties: charge, compoundId, mw, name, pka1
    '                 pka2, pka3, sName
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:30:29 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.genomics

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `compound`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `compound` (
'''   `compoundId` varchar(255) NOT NULL,
'''   `charge` int(5) DEFAULT NULL,
'''   `name` varchar(255) DEFAULT NULL,
'''   `mw` float DEFAULT NULL,
'''   `pka1` float DEFAULT NULL,
'''   `pka2` float DEFAULT NULL,
'''   `pka3` float DEFAULT NULL,
'''   `sName` varchar(255) DEFAULT NULL,
'''   PRIMARY KEY (`compoundId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("compound")>
Public Class compound: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("compoundId"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property compoundId As String
    <DatabaseField("charge"), DataType(MySqlDbType.Int64, "5")> Public Property charge As Long
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "255")> Public Property name As String
    <DatabaseField("mw"), DataType(MySqlDbType.Double)> Public Property mw As Double
    <DatabaseField("pka1"), DataType(MySqlDbType.Double)> Public Property pka1 As Double
    <DatabaseField("pka2"), DataType(MySqlDbType.Double)> Public Property pka2 As Double
    <DatabaseField("pka3"), DataType(MySqlDbType.Double)> Public Property pka3 As Double
    <DatabaseField("sName"), DataType(MySqlDbType.VarChar, "255")> Public Property sName As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `compound` (`compoundId`, `charge`, `name`, `mw`, `pka1`, `pka2`, `pka3`, `sName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `compound` (`compoundId`, `charge`, `name`, `mw`, `pka1`, `pka2`, `pka3`, `sName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `compound` WHERE `compoundId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `compound` SET `compoundId`='{0}', `charge`='{1}', `name`='{2}', `mw`='{3}', `pka1`='{4}', `pka2`='{5}', `pka3`='{6}', `sName`='{7}' WHERE `compoundId` = '{8}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, compoundId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, compoundId, charge, name, mw, pka1, pka2, pka3, sName)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, compoundId, charge, name, mw, pka1, pka2, pka3, sName)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, compoundId, charge, name, mw, pka1, pka2, pka3, sName, compoundId)
    End Function
#End Region
End Class


End Namespace
