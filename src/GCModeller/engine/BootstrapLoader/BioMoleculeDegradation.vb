#Region "Microsoft.VisualBasic::629dac34ff033276132dbd6df349297d, GCModeller\engine\BootstrapLoader\BioMoleculeDegradation.vb"

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

    '   Total Lines: 123
    '    Code Lines: 97
    ' Comment Lines: 7
    '   Blank Lines: 19
    '     File Size: 5.29 KB


    '     Class BioMoleculeDegradation
    ' 
    '         Properties: proteinMatures
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateFlux, proteinDegradation, RNADegradation
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Vector

Namespace ModelLoader

    ''' <summary>
    ''' 构建出生物大分子的降解过程
    ''' </summary>
    Public Class BioMoleculeDegradation : Inherits FluxLoader

        Public Property proteinMatures As Channel()

        Public Sub New(loader As Loader)
            MyBase.New(loader)

            Call loader.fluxIndex.Add(NameOf(proteinDegradation), New List(Of String))
            Call loader.fluxIndex.Add(NameOf(RNADegradation), New List(Of String))
            Call loader.fluxIndex.Add("polypeptideDegradation", New List(Of String))
        End Sub

        Public Overrides Function CreateFlux(cell As CellularModule) As IEnumerable(Of Channel)
            Return proteinDegradation(cell).AsList + RNADegradation(cell)
        End Function

        Private Iterator Function proteinDegradation(cell As CellularModule) As IEnumerable(Of Channel)
            ' protein complex -> polypeptide + compounds
            ' polypeptide -> aminoacid
            Dim proteinComplex$
            Dim peptideId$
            Dim geneIDindex = cell.Genotype.centralDogmas _
                .Where(Function(cd)
                           Return Not cd.polypeptide.StringEmpty
                       End Function) _
                .ToDictionary(Function(cd) cd.polypeptide,
                              Function(cd)
                                  Return cd.geneID
                              End Function)
            Dim proteinMatrix = cell.Genotype.ProteinMatrix.ToDictionary(Function(r) r.proteinID)
            Dim composition As ProteinComposition
            Dim aaResidue As Variable()
            Dim geneID$
            Dim flux As Channel

            For Each complex As Channel In proteinMatures
                proteinComplex = complex.right.First(Function(c) c.mass.ID.EndsWith(".complex")).mass.ID
                peptideId = proteinComplex.Replace(".complex", "")
                geneID = geneIDindex(peptideId)
                composition = proteinMatrix(geneID)
                aaResidue = composition _
                    .Where(Function(i) i.Value > 0) _
                    .Select(Function(aa)
                                Dim aaName = loader.define.AminoAcid(aa.Name)
                                Return MassTable.variable(aaName, aa.Value)
                            End Function) _
                    .ToArray

                flux = New Channel(MassTable.variables({proteinComplex}, 1), MassTable.variables({peptideId}, 1)) With {
                    .ID = $"proteinComplexDegradationOf{proteinComplex}",
                    .forward = Controls.StaticControl(10),
                    .reverse = Controls.StaticControl(0),
                    .bounds = New Boundary With {
                        .forward = 1000,
                        .reverse = 0
                    }
                }

                Call loader.fluxIndex(NameOf(Me.proteinDegradation)).Add(flux.ID)

                Yield flux

                flux = New Channel(MassTable.variables({peptideId}, 1), aaResidue) With {
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
        End Function

        Private Iterator Function RNADegradation(cell As CellularModule) As IEnumerable(Of Channel)
            Dim centralDogmas = loader.GetCentralDogmaFluxLoader
            Dim composition As RNAComposition
            Dim rnaMatrix = cell.Genotype.RNAMatrix.ToDictionary(Function(r) r.geneID)
            Dim ntBase As Variable()
            Dim flux As Channel

            ' rna -> nt base
            For Each rna As String In centralDogmas.componentRNA.AsList + centralDogmas.mRNA
                composition = rnaMatrix(rna)
                ntBase = composition _
                    .Where(Function(i) i.Value > 0) _
                    .Select(Function(base)
                                Dim baseName = loader.define.NucleicAcid(base.Name)
                                Return MassTable.variable(baseName, base.Value)
                            End Function) _
                    .ToArray

                ' 降解过程是不可逆的
                flux = New Channel(MassTable.variables({rna}, 1), ntBase) With {
                    .ID = $"RNADegradationOf{rna}",
                    .forward = Controls.StaticControl(10),
                    .reverse = Controls.StaticControl(0),
                    .bounds = New Boundary With {
                        .forward = 1000,
                        .reverse = 0
                    }
                }

                Call loader.fluxIndex(NameOf(Me.RNADegradation)).Add(flux.ID)

                Yield flux
            Next
        End Function
    End Class
End Namespace
