#Region "Microsoft.VisualBasic::c174e31c0b3e11d3c8dd346f22cf5717, GCModeller\models\Networks\Microbiome\MetabolicComplementation\CompoundOrigins.vb"

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

    '   Total Lines: 53
    '    Code Lines: 35
    ' Comment Lines: 11
    '   Blank Lines: 7
    '     File Size: 1.98 KB


    ' Class CompoundOrigins
    ' 
    '     Function: CreateEmptyCompoundsProfile, getIndexJson
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy

''' <summary>
''' Organism KEGG CompoundOrigins profiles dataset
''' 
''' ID property of the dataset is the ncbi taxonomy id or kegg organism id 
''' </summary>
Public Class CompoundOrigins : Inherits OTUTable

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="organism">The directory path which contains the kegg organism information files.</param>
    ''' <returns></returns>
    Public Shared Function CreateEmptyCompoundsProfile(taxonomy As NcbiTaxonomyTree, organism As String) As CompoundOrigins
        Dim info = organism.DoCall(AddressOf getIndexJson).LoadJSON(Of OrganismInfo)
        Dim compoundID As String() = $"{organism}/kegg_compounds.txt".ReadAllLines
        Dim empty As New CompoundOrigins With {
            .ID = info.code
        }
        Dim taxonomyEntry As String = Strings.Trim(info.Taxonomy)

        If taxonomyEntry.IsPattern("TAX[:]\s*\d+") Then
            ' is ncbi taxonomy id
            Dim taxid As String = taxonomyEntry.Split(":"c).Last.Trim
            Dim lineage As Metagenomics.Taxonomy = taxonomy _
                .GetAscendantsWithRanksAndNames(Integer.Parse(taxid), only_std_ranks:=True) _
                .DoCall(Function(nodes)
                            Return New Metagenomics.Taxonomy(nodes)
                        End Function)

            empty.taxonomy = lineage
        End If

        For Each id As String In compoundID
            empty(id) = 1
        Next

        Return empty
    End Function

    Private Shared Function getIndexJson(repo As String) As String
        If $"{repo}/kegg.json".FileExists Then
            Return $"{repo}/kegg.json"
        Else
            Return $"{repo}/{repo.BaseName}.json"
        End If
    End Function
End Class
