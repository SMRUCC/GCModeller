#Region "Microsoft.VisualBasic::93e2dc654468298a08fbdd4b92501638, R#\annotationKit\Rhea.vb"

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

    '   Total Lines: 35
    '    Code Lines: 14 (40.00%)
    ' Comment Lines: 18 (51.43%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 3 (8.57%)
    '     File Size: 1.24 KB


    ' Module Rhea
    ' 
    '     Function: load_reactions, openRDF
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.Rhea

''' <summary>
''' Rhea is an expert-curated knowledgebase of chemical and transport reactions of biological interest 
''' and the standard for enzyme and transporter annotation in UniProtKB. Rhea uses the chemical 
''' dictionary ChEBI (Chemical Entities of Biological Interest) to describe reaction participants.
''' </summary>
<Package("rhea")>
Module Rhea

    ''' <summary>
    ''' open the rdf data pack of Rhea database
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' https://ftp.expasy.org/databases/rhea/rdf/rhea.rdf.gz
    ''' </remarks>
    <ExportAPI("open.rdf")>
    Public Function openRDF(file As String) As RheaRDF
        Return file.LoadXml(Of RheaRDF)
    End Function

    ''' <summary>
    ''' Load reaction models from rhea rdf database file
    ''' </summary>
    ''' <param name="rhea"></param>
    ''' <returns></returns>
    <ExportAPI("reactions")>
    Public Function load_reactions(rhea As RheaRDF) As Reaction()
        Return rhea.GetReactions.ToArray
    End Function
End Module

