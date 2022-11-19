#Region "Microsoft.VisualBasic::608f8fb50488380d28efd70c5b266a94, GCModeller\data\RegulonDatabase\Regtransbase\StructureObjects\Dictionary.vb"

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

    '   Total Lines: 45
    '    Code Lines: 13
    ' Comment Lines: 27
    '   Blank Lines: 5
    '     File Size: 1.80 KB


    '     Class Dictionary
    ' 
    '         Properties: Name
    ' 
    '         Function: ToString
    ' 
    '     Class Genomes
    ' 
    '         Properties: Guid
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Regtransbase.StructureObjects.Dictionaries

    ''' <summary>
    ''' RegTransBase contains the following dictionaries: 
    ''' 
    '''  '•	Genomes dictionary (Genome)
    '''  '•	Functional types of site dictionary (FuncSiteType)
    '''  '•	Structural types of site dictionary (StructSiteType)
    '''  '•	Types of position dictionary (ObjSideType)
    '''  '•	Dictionary of experimental techniques (ExpType)
    ''' 
    ''' All dictionaries are created by curator. Complete set of dictionaries can be exported 
    ''' as a single file for import by annotators in their annotation programs. Dictionary 
    ''' entry created by curator has property fl_new=FALSE. Annotator also can add entry into 
    ''' any dictionary. Such entry has property fl_new=TRUE and property user_id contains 
    ''' annotator name.
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class Dictionary
        'Elements of all dictionaries contains following common properties:

        <DatabaseField("name")> Public Property Name As String
        Public MustOverride Property Guid As Integer

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' CREATE TABLE `dict_genomes` (
    '''  `genome_guid` int(11) NOT NULL DEFAULT '0',
    '''  `name` varchar(100) DEFAULT NULL,
    ''' PRIMARY KEY (`genome_guid`)
    ''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
    ''' </remarks>
    Public Class Genomes : Inherits Regtransbase.StructureObjects.Dictionaries.Dictionary
        <DatabaseField("genome_guid")> Public Overrides Property Guid As Integer
    End Class
End Namespace
