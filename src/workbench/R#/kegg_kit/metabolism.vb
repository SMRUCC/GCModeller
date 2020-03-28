#Region "Microsoft.VisualBasic::f0f15bb14bf9e4f666fb692c456364a4, R#\kegg_kit\metabolism.vb"

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

    ' Module metabolism
    ' 
    '     Function: CreateCompoundOriginModel, filterInvalidCompoundIds, GetAllCompounds
    ' 
    ' /********************************************************************************/

#End Region


Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports RDotNET.Extensions.GCModeller
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Data

''' <summary>
''' The kegg metabolism model toolkit
''' </summary>
<Package("kegg.metabolism", Category:=APICategories.ResearchTools)>
Module metabolism

    ''' <summary>
    ''' Get compounds kegg id which is related to the given KO id list
    ''' </summary>
    ''' <param name="enzymes">KO id list</param>
    ''' <param name="reactions"></param>
    ''' <returns></returns>
    <ExportAPI("related.compounds")>
    Public Function GetAllCompounds(enzymes$(), reactions As ReactionRepository) As String()
        Return reactions _
            .GetByKOMatch(KO:=enzymes) _
            .Select(Function(r) r.GetSubstrateCompounds) _
            .IteratesALL _
            .Distinct _
            .ToArray
    End Function

    <ExportAPI("compound.origins")>
    Public Function CreateCompoundOriginModel(repo As String, Optional compoundNames As Dictionary(Of String, String) = Nothing) As OrganismCompounds
        If compoundNames Is Nothing Then
            Return OrganismCompounds.LoadData(repo)
        Else
            Return OrganismCompounds.LoadData(repo, compoundNames)
        End If
    End Function

    ''' <summary>
    ''' Removes invalid kegg compound id
    ''' </summary>
    ''' <param name="identified"></param>
    ''' <returns></returns>
    <ExportAPI("filter.invalid_keggIds")>
    Public Function filterInvalidCompoundIds(identified As String()) As String()
        Return identified _
            .Where(Function(id)
                       Return id.IsPattern(KEGGCompoundIDPatterns)
                   End Function) _
            .ToArray
    End Function
End Module

