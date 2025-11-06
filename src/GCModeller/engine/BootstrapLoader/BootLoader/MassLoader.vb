#Region "Microsoft.VisualBasic::5c85450bc48094651f2d82520c4303a3, engine\BootstrapLoader\MassLoader.vb"

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

'   Total Lines: 58
'    Code Lines: 35 (60.34%)
' Comment Lines: 12 (20.69%)
'    - Xml Docs: 58.33%
' 
'   Blank Lines: 11 (18.97%)
'     File Size: 2.42 KB


'     Class MassLoader
' 
'         Properties: massTable
' 
'         Constructor: (+1 Overloads) Sub New
'         Sub: doMassLoadingOn
' 
' 
' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Molecule
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process

Namespace ModelLoader

    Public Class MassLoader

        Public ReadOnly Property massTable As MassTable

        ''' <summary>
        ''' link mapping from protein to protein complex
        ''' </summary>
        Public ReadOnly proteinComplex As New Dictionary(Of String, String)
        Public ReadOnly peptideToProteinComplex As New Dictionary(Of String, String())

        Sub New(loader As Loader)
            massTable = loader.massTable
        End Sub

        ''' <summary>
        ''' Create mass table from the virtual cell model
        ''' </summary>
        ''' <param name="cell"></param>
        Public Sub doMassLoadingOn(cell As CellularModule)
            Dim defaultCompartment As String() = cell.GetCompartments.ToArray

            ' 在这里需要首选构建物质列表
            ' 否则下面的转录和翻译过程的构建会出现找不到物质因子对象的问题
            For Each reaction As Reaction In cell.Phenotype.fluxes
                For Each compart_id As String In defaultCompartment
                    For Each compound In reaction.equation.GetMetabolites
                        If Not massTable.Exists(compound.ID, compart_id) Then
                            Call massTable.addNew(compound.ID, MassRoles.compound, compart_id)
                        End If
                    Next
                Next

                If Not reaction.kinetics.IsNullOrEmpty Then
                    For Each law As Kinetics In reaction.kinetics
                        For Each val As Object In law.paramVals
                            If TypeOf val Is String Then
                                Dim cid As String = CStr(val)

                                For Each compart_id As String In defaultCompartment
                                    If Not massTable.Exists(cid, compart_id) Then
                                        Call massTable.addNew(cid, MassRoles.compound, compart_id)
                                    End If
                                Next
                            End If
                        Next
                    Next
                End If
            Next

            Dim complexID As String
            Dim peptideMaps As New Dictionary(Of String, List(Of String))
            Dim duplicated As New List(Of String)

            ' 20241113 protein id maybe duplicated, due to the reason of
            ' some gene translate the protein with identicial protein sequence data
            ' so reference to the identical protein model
            For Each complex As Protein In cell.Phenotype.proteins
                ' If complex.isAutoConstructed Then
                ' complexID = massTable.addNew(complex.ProteinID & ".complex", MassRoles.protein, cell.CellularEnvironmentName)
                ' Else
                complexID = massTable.addNew(complex.ProteinID, MassRoles.protein, cell.CellularEnvironmentName)
                ' End If

                For Each id As String In complex.polypeptides
                    If Not peptideMaps.ContainsKey(id) Then
                        peptideMaps.Add(id, New List(Of String))
                    End If

                    Call peptideMaps(id).Add(complexID)
                Next

                If proteinComplex.ContainsKey(complex.ProteinID) Then
                    Call duplicated.Add(complex.ProteinID)
                Else
                    Call proteinComplex.Add(complex.ProteinID, complexID)
                End If
            Next

            If duplicated.Any Then
                Dim uniq As String() = duplicated.Distinct.ToArray
                Dim warn = $"found {uniq.Length} duplicated protein complex models: {uniq.JoinBy(", ")}!"

                Call warn.warning
                Call warn.debug
            End If

            With proteinComplex.OrderBy(Function(a) a.Key).ToArray
                Call proteinComplex.Clear()

                For Each map In .AsEnumerable
                    Call proteinComplex.Add(map.Key, map.Value)
                Next
            End With

            For Each link In peptideMaps
                peptideToProteinComplex(link.Key) = link.Value.ToArray
            Next
        End Sub

    End Class
End Namespace
