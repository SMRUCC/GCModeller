#Region "Microsoft.VisualBasic::6e6074b231cf96028efa80b6328b9f92, GCModeller\engine\BootstrapLoader\ProteinMatureFluxLoader.vb"

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

    '   Total Lines: 68
    '    Code Lines: 51
    ' Comment Lines: 4
    '   Blank Lines: 13
    '     File Size: 2.76 KB


    '     Class ProteinMatureFluxLoader
    ' 
    '         Properties: polypeptides, proteinComplex
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateFlux
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Molecule

Namespace ModelLoader

    ''' <summary>
    ''' 构建酶成熟的过程
    ''' </summary>
    Public Class ProteinMatureFluxLoader : Inherits FluxLoader

        Public ReadOnly Property polypeptides As String()
        Public ReadOnly Property proteinComplex As String()

        Public Sub New(loader As Loader)
            MyBase.New(loader)

            Call loader.fluxIndex.Add(NameOf(ProteinMatureFluxLoader), New List(Of String))
        End Sub

        Public Overrides Iterator Function CreateFlux(cell As CellularModule) As IEnumerable(Of Channel)
            Dim polypeptides As New List(Of String)
            Dim proteinComplex As New List(Of String)
            Dim flux As Channel

            For Each complex As Protein In cell.Phenotype.proteins
                For Each compound In complex.compounds
                    If Not MassTable.Exists(compound) Then
                        Call MassTable.AddNew(compound, MassRoles.compound)
                    End If
                Next
                For Each peptide In complex.polypeptides
                    If Not MassTable.Exists(peptide) Then
                        Throw New MissingMemberException(peptide)
                    Else
                        polypeptides += peptide
                    End If
                Next

                Dim unformed = MassTable.variables(complex).ToArray
                Dim complexID As String = loader.massLoader.proteinComplex(complex.ProteinID)
                Dim mature As Variable = MassTable.variable(complexID)

                proteinComplex += complexID

                ' 酶的成熟过程也是一个不可逆的过程
                flux = New Channel(unformed, {mature}) With {
                    .ID = complex.DoCall(AddressOf loader.GetProteinMatureId),
                    .reverse = Controls.StaticControl(0),
                    .forward = Controls.StaticControl(loader.dynamics.proteinMatureBaseline),
                    .bounds = New Boundary With {
                        .forward = loader.dynamics.proteinMatureCapacity,
                        .reverse = 0
                    }
                }

                loader.fluxIndex(NameOf(ProteinMatureFluxLoader)).Add(flux.ID)

                Yield flux
            Next

            _polypeptides = polypeptides
            _proteinComplex = proteinComplex
        End Function
    End Class
End Namespace
