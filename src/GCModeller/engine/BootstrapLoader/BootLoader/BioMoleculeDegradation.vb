#Region "Microsoft.VisualBasic::49d17873ae785e79cfd7316673b18746, engine\BootstrapLoader\BioMoleculeDegradation.vb"

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

'   Total Lines: 147
'    Code Lines: 116 (78.91%)
' Comment Lines: 9 (6.12%)
'    - Xml Docs: 33.33%
' 
'   Blank Lines: 22 (14.97%)
'     File Size: 6.70 KB


'     Class BioMoleculeDegradation
' 
'         Properties: proteinMatures
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: CreateFlux, GetMassSet, proteinDegradation, RNADegradation
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Vector

Namespace ModelLoader

    ''' <summary>
    ''' 构建出生物大分子的降解过程
    ''' </summary>
    Public Class BioMoleculeDegradation : Inherits FluxLoader

        Public Property proteinMatures As Channel()

        ReadOnly pull As New List(Of String)

        Public Sub New(loader As Loader)
            MyBase.New(loader)

            Call loader.fluxIndex.Add(NameOf(proteinDegradation), New List(Of String))
            Call loader.fluxIndex.Add(NameOf(RNADegradation), New List(Of String))
            Call loader.fluxIndex.Add("polypeptideDegradation", New List(Of String))
        End Sub

        Protected Overrides Function CreateFlux() As IEnumerable(Of Channel)
            Return proteinDegradation(cell).AsList + RNADegradation(cell)
        End Function

        Private Iterator Function proteinDegradation(cell As CellularModule) As IEnumerable(Of Channel)
            Dim cellular_id As String = cell.CellularEnvironmentName
            ' protein complex -> polypeptide + compounds
            ' polypeptide -> aminoacid
            Dim proteinComplexId$
            Dim peptideId$
            Dim geneIDindex = cell.Genotype.centralDogmas _
                .Where(Function(cd)
                           Return Not cd.polypeptide.StringEmpty
                       End Function) _
                .GroupBy(Function(a) a.polypeptide) _
                .ToDictionary(Function(cd) cd.Key,
                              Function(cd)
                                  ' 20241115 some gene may translate identical sequence
                                  ' protein
                                  Return cd.Select(Function(g) g.translation).Distinct.ToArray
                              End Function)
            Dim proteinMatrix = cell.Genotype.ProteinMatrix _
                .GroupBy(Function(a) a.proteinID) _
                .ToDictionary(Function(r) r.Key,
                              Function(r)
                                  Return r.First
                              End Function)
            Dim composition As ProteinComposition
            Dim aaResidue As Variable()
            Dim geneIDSet As String()
            Dim flux As Channel
            Dim proteinComplex As Variable

            For Each complex As Channel In proteinMatures
                proteinComplex = complex.right _
                    .First(Function(c)
                               Return MassTable.getSource(c.mass.ID).source_id ' .EndsWith(".complex")
                           End Function)
                proteinComplexId = MassTable.getSource(proteinComplex.mass.ID).source_id
                peptideId = proteinComplexId ' .Replace(".complex", "")
                geneIDSet = geneIDindex(peptideId)

                For Each geneId As String In geneIDSet
                    composition = proteinMatrix(geneId)
                    aaResidue = composition _
                        .Where(Function(i) i.Value > 0) _
                        .Select(Function(aa)
                                    Dim aaName = loader.define.AminoAcid(aa.Name)
                                    Return MassTable.variable(aaName, cellular_id, aa.Value)
                                End Function) _
                        .ToArray

                    flux = New Channel(MassTable.variables({proteinComplexId}, 1, cell.CellularEnvironmentName), MassTable.variables({peptideId}, 1, cell.CellularEnvironmentName)) With {
                        .ID = $"proteinComplexDegradationOf{proteinComplexId}",
                        .forward = Controls.StaticControl(10),
                        .reverse = Controls.StaticControl(0),
                        .bounds = New Boundary With {
                            .forward = 1000,
                            .reverse = 0
                        }
                    }

                    Call loader.fluxIndex(NameOf(Me.proteinDegradation)).Add(flux.ID)

                    Yield flux

                    flux = New Channel(MassTable.variables({peptideId}, 1, cell.CellularEnvironmentName), aaResidue) With {
                        .ID = $"polypeptideDegradationOf{peptideId}",
                        .forward = Controls.StaticControl(10),
                        .reverse = Controls.StaticControl(0),
                        .bounds = New Boundary With {
                            .forward = 1000,
                            .reverse = 0
                        }
                    }

                    Call loader.fluxIndex("polypeptideDegradation").Add(flux.ID)

                    Yield flux
                Next
            Next
        End Function

        Private Iterator Function RNADegradation(cell As CellularModule) As IEnumerable(Of Channel)
            Dim centralDogmas As CentralDogmaFluxLoader = loader.GetCentralDogmaFluxLoader
            Dim composition As RNAComposition
            Dim rnaMatrix = cell.Genotype.RNAMatrix _
                .GroupBy(Function(a) a.geneID) _
                .ToDictionary(Function(r) r.Key,
                              Function(r)
                                  Return r.First
                              End Function)
            Dim ntBase As Variable()
            Dim flux As Channel
            Dim cellular_id As String = cell.CellularEnvironmentName

            ' 20250711
            ' the code is the gene id list
            ' not rna id list
            ' will associate to the wrong object to degradation

            ' centralDogmas.componentRNA.AsList + centralDogmas.mRNA

            ' rna -> nt base
            For Each gene As CentralDogma In cell.Genotype.centralDogmas
                composition = rnaMatrix(gene.geneID)
                ntBase = composition _
                    .Where(Function(i) i.Value > 0) _
                    .Select(Function(base)
                                Dim baseName = loader.define.NucleicAcid(base.Name)
                                Return MassTable.variable(baseName, cellular_id, base.Value)
                            End Function) _
                    .ToArray

                ' 降解过程是不可逆的
                flux = New Channel(MassTable.variables({gene.RNAName}, 1, cell.CellularEnvironmentName), ntBase) With {
                    .ID = $"RNADegradationOf{gene.RNAName}",
                    .forward = Controls.StaticControl(loader.dynamics.RNADegradationBaseline),
                    .reverse = Controls.StaticControl(0),
                    .bounds = New Boundary With {
                        .forward = loader.dynamics.RNADegradationCapacity,
                        .reverse = 0
                    }
                }

                Call loader.fluxIndex(NameOf(Me.RNADegradation)).Add(flux.ID)

                Yield flux
            Next
        End Function

        Protected Overrides Function GetMassSet() As IEnumerable(Of String)
            Return pull
        End Function
    End Class
End Namespace
