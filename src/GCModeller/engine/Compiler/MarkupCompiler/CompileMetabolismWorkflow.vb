#Region "Microsoft.VisualBasic::0118c4385691492fcdb3b573f936599a, engine\Compiler\MarkupCompiler\CompileMetabolismWorkflow.vb"

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

    '     Class CompileMetabolismWorkflow
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: createEnzymes, createMaps, getCompounds
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process
Imports XmlReaction = SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2.Reaction

Namespace MarkupCompiler

    Friend Class CompileMetabolismWorkflow : Inherits CompilerWorkflow

        Public Sub New(compiler As v2MarkupCompiler)
            MyBase.New(compiler)
        End Sub

        Friend Iterator Function createMaps(pathwayMaps As bGetObject.PathwayMap(), KOfunc As Dictionary(Of String, CentralDogma())) As IEnumerable(Of FunctionalCategory)
            Dim mapgroups = pathwayMaps _
                .Where(Function(map) Not map.brite Is Nothing) _
                .GroupBy(Function(map)
                             Return map.brite.class
                         End Function)

            For Each category As IGrouping(Of String, bGetObject.PathwayMap) In mapgroups
                Dim maps As New List(Of Pathway)

                For Each map As bGetObject.PathwayMap In category
                    Dim enzymeUnits = map.KEGGOrthology.Terms _
                        .SafeQuery _
                        .Where(Function(term)
                                   Return KOfunc.ContainsKey(term.name)
                               End Function) _
                        .Select(Function(term)
                                    Dim enzymeUnit = KOfunc(term.name) _
                                        .Select(Function(protein)
                                                    Return New [Property] With {
                                                        .name = protein.polypeptide,
                                                        .comment = protein.geneID,
                                                        .value = term.name
                                                    }
                                                End Function) _
                                        .ToArray
                                    Return enzymeUnit
                                End Function) _
                        .IteratesALL _
                        .ToArray

                    If Not enzymeUnits.IsNullOrEmpty Then
                        maps += New Pathway With {
                            .ID = map.KOpathway,
                            .name = map.name,
                            .enzymes = enzymeUnits
                        }
                    End If
                Next

                If Not maps = 0 Then
                    Yield New FunctionalCategory With {
                        .category = category.Key,
                        .pathways = maps
                    }
                End If
            Next
        End Function

        Friend Iterator Function getCompounds(reactions As IEnumerable(Of XmlReaction)) As IEnumerable(Of Compound)
            Dim allCompoundId$() = reactions _
                .Select(Function(r)
                            Return Equation.TryParse(r.Equation) _
                                           .GetMetabolites _
                                           .Select(Function(compound) compound.ID)
                        End Function) _
                .IteratesALL _
                .Distinct _
                .ToArray
            Dim compounds As CompoundRepository '= compiler.KEGG.GetCompounds

            For Each id As String In allCompoundId.Where(Function(cid) compounds.Exists(cid))
                Dim keggModel = compounds.GetByKey(id).Entity

                Yield New Compound With {
                    .ID = id,
                    .name = keggModel.commonNames.ElementAtOrDefault(0, keggModel.formula)
                }
            Next
        End Function

        Friend Iterator Function createEnzymes(metabolicProcess As IEnumerable(Of Regulation), KOgenes As Dictionary(Of String, CentralDogma)) As IEnumerable(Of Enzyme)
            For Each catalysis As IGrouping(Of String, Regulation) In metabolicProcess.GroupBy(Function(c) c.regulator)
                Yield New Enzyme With {
                    .geneID = catalysis.Key,
                    .catalysis = catalysis _
                        .Select(Function(reg)
                                    Return New Catalysis With {
                                        .reaction = reg.process,
                                        .formula = Nothing
                                    }
                                End Function) _
                        .ToArray,
                    .KO = KOgenes.TryGetValue(.geneID).orthology,
                    .ECNumber = ""
                }
            Next
        End Function
    End Class
End Namespace
