#Region "Microsoft.VisualBasic::b34ffd5dead9b146f06e4d171d7124e9, engine\BootstrapLoader\SyntheticMicrobialNetwork.vb"

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

    '   Total Lines: 48
    '    Code Lines: 35 (72.92%)
    ' Comment Lines: 3 (6.25%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (20.83%)
    '     File Size: 1.89 KB


    ' Module SyntheticMicrobialNetwork
    ' 
    '     Function: CreateNetwork
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.ModelLoader
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular

''' <summary>
''' Synthetic microbial community (SynComs)
''' </summary>
Public Module SyntheticMicrobialNetwork

    <Extension>
    Public Function CreateNetwork(models As IEnumerable(Of CellularModule),
                                  define As Definition,
                                  dynamics As FluxBaseline,
                                  referenceIds As Dictionary(Of String, String)) As (
        mass As MassTable,
        network As Channel(),
        fluxIndex As Dictionary(Of String, List(Of String))
    )

        Dim massTable As New MassTable(referenceIds)
        Dim fluxIndex As New Dictionary(Of String, List(Of String))
        Dim processList As New List(Of Channel)

        For Each modelData As CellularModule In models
            Dim loader As New Loader(define, dynamics, massTable:=massTable)

            Call massTable.SetDefaultCompartmentId(modelData.CellularEnvironmentName)

            With loader.CreateEnvironment(modelData)
                Call processList.AddRange(.processes)
            End With

            Dim modelFluxIndex = loader.GetFluxIndex

            For Each part_key As String In modelFluxIndex.Keys
                If Not fluxIndex.ContainsKey(part_key) Then
                    Call fluxIndex.Add(part_key, New List(Of String))
                End If

                Call fluxIndex(part_key).AddRange(modelFluxIndex(part_key))
            Next
        Next

        Return (massTable, processList.ToArray, fluxIndex)
    End Function
End Module

