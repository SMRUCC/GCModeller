#Region "Microsoft.VisualBasic::a710efc5b9c5f25c4f866b65fd95cbab, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\PathwayMap\PathwayTextParser.vb"

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

    '   Total Lines: 29
    '    Code Lines: 25
    ' Comment Lines: 0
    '   Blank Lines: 4
    '     File Size: 1.21 KB


    '     Module PathwayTextParser
    ' 
    '         Function: (+2 Overloads) ParsePathway
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Module PathwayTextParser

        Public Function ParsePathway(data As String) As Pathway
            Return ParsePathway(New WebForm(data))
        End Function

        <Extension>
        Public Function ParsePathway(form As WebForm) As Pathway
            Return New Pathway With {
                .compound = form.GetXmlTuples("COMPOUND").ToArray,
                .genes = form.GetValue("GENE").Select(AddressOf GeneName.Parse).ToArray,
                .drugs = form.GetXmlTuples("DRUG").ToArray,
                .name = form!NAME,
                .description = form!DESCRIPTION,
                .EntryId = form!ENTRY.Split(" "c).First,
                .organism = form!ORGANISM,
                .references = form.References,
                .modules = form.GetXmlTuples("MODULE").ToArray,
                .[class] = Strings.Trim(form!CLASS).StringSplit(";\s+"),
                .related_pathways = form.GetXmlTuples("REL_PATHWAY").ToArray
            }
        End Function
    End Module
End Namespace
