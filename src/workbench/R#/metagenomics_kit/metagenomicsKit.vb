#Region "Microsoft.VisualBasic::5ebb14ab9947fa842306517340211958, R#\metagenomics_kit\metagenomicsKit.vb"

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

' Module metagenomicsKit
' 
'     Function: createEmptyCompoundOriginProfile
' 
' /********************************************************************************/

#End Region


Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.Model.Network.Microbiome
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object

<Package("metagenomics_kit")>
Module metagenomicsKit

    <ExportAPI("compounds.origin.profile")>
    Public Function createEmptyCompoundOriginProfile(taxonomy As NcbiTaxonomyTree, organism As String) As CompoundOrigins
        Return CompoundOrigins.CreateEmptyCompoundsProfile(taxonomy, organism)
    End Function

    <ExportAPI("compounds.origin")>
    Public Function CompoundOrigin(annotations As list, tree As NcbiTaxonomyTree, Optional env As Environment = Nothing) As list
        Dim compounds As New Dictionary(Of String, List(Of String))

        For Each organism As KeyValuePair(Of String, Pathway()) In annotations.AsGeneric(Of Pathway())(env)
            For Each map As Pathway In organism.Value
                For Each compound As NamedValue In map.compound.SafeQuery
                    If Not compounds.ContainsKey(compound.name) Then
                        Call compounds.Add(compound.name, New List(Of String))
                    End If

                    Call compounds(compound.name).Add(organism.Key)
                Next
            Next
        Next

        Dim origins As New Dictionary(Of String, Object)
        Dim ncbi_taxid As String()
        Dim taxonomyList As gast.Taxonomy()

        For Each compound In compounds
            ncbi_taxid = compound.Value.ToArray
            taxonomyList = ncbi_taxid _
                .Select(Function(id)
                            Return New gast.Taxonomy(New Metagenomics.Taxonomy(tree.GetAscendantsWithRanksAndNames(Integer.Parse(id), True)))
                        End Function) _
                .ToArray

            origins(compound.Key) = New Dictionary(Of String, Object) From {
                {"kegg_id", compound.Key},
                {"ncbi_taxid", ncbi_taxid},
                {"taxonomy", Nothing}
            }
        Next

        Return New list With {.slots = origins}
    End Function
End Module

