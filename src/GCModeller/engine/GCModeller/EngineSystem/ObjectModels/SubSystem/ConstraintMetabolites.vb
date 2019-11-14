#Region "Microsoft.VisualBasic::d0829ca0f84a1b702703bcfa3604d98e, engine\GCModeller\EngineSystem\ObjectModels\SubSystem\ConstraintMetabolites.vb"

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

    '     Class MetabolismCompartment
    ' 
    '         Properties: TransmembraneFluxs
    ' 
    '         Function: GetTransmembraneFluxs, InsertNewMetabolite
    '         Class ConstraintMetabolites
    ' 
    '             Function: GetEnumerator, GetEnumerator1
    ' 
    '             Sub: Initialize, InvokeATP_Compensate, setup_ATP_Compensate
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging

#Const DEBUG = 1

Namespace EngineSystem.ObjectModels.SubSystem

    Partial Class MetabolismCompartment

        ''' <summary>
        ''' 这个不进行计算，仅作为数据观察的接口而是用，对于本部分的反应的计算的驱动，则来自于培养基对象之中
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TransmembraneFluxs As [Module].PassiveTransportationFlux()

        Public Function GetTransmembraneFluxs(UniqueId As String) As [Module].PassiveTransportationFlux()
            Dim LQuery = (From TrFlux In TransmembraneFluxs.AsParallel Where String.Equals(UniqueId, TrFlux._BaseType.Identifier) Select TrFlux).ToArray
            Return LQuery
        End Function

        Public Class ConstraintMetabolites : Implements Generic.IEnumerable(Of ObjectModels.Entity.Compound)

            Public CONSTRAINT_ALA_TRNA_CHARGED As ObjectModels.Entity.Compound ' "ALA-TRNAS+"
            Public CONSTRAINT_ARG_TRNA_CHARGED As ObjectModels.Entity.Compound ' "ARG-TRNAS+"
            Public CONSTRAINT_ASN_TRNA_CHARGED As ObjectModels.Entity.Compound ' "ASN-TRNAS+"
            Public CONSTRAINT_ASP_TRNA_CHARGED As ObjectModels.Entity.Compound ' "ASP-TRNAS+"
            Public CONSTRAINT_CYS_TRNA_CHARGED As ObjectModels.Entity.Compound ' "CYS-TRNAS+"
            Public CONSTRAINT_GLN_TRNA_CHARGED As ObjectModels.Entity.Compound ' "GLN-TRNAS+"
            Public CONSTRAINT_GLT_TRNA_CHARGED As ObjectModels.Entity.Compound ' "GLT-TRNAS+"
            Public CONSTRAINT_GLY_TRNA_CHARGED As ObjectModels.Entity.Compound ' "GLY-TRNAS+"
            Public CONSTRAINT_HIS_TRNA_CHARGED As ObjectModels.Entity.Compound ' "HIS-TRNAS+"
            Public CONSTRAINT_ILE_TRNA_CHARGED As ObjectModels.Entity.Compound ' "ILE-TRNAS+"
            Public CONSTRAINT_LEU_TRNA_CHARGED As ObjectModels.Entity.Compound ' "LEU-TRNAS+"
            Public CONSTRAINT_LYS_TRNA_CHARGED As ObjectModels.Entity.Compound ' "LYS-TRNAS+"
            Public CONSTRAINT_MET_TRNA_CHARGED As ObjectModels.Entity.Compound ' "MET-TRNAS+"
            Public CONSTRAINT_PHE_TRNA_CHARGED As ObjectModels.Entity.Compound ' "PHE-TRNAS+"
            Public CONSTRAINT_PRO_TRNA_CHARGED As ObjectModels.Entity.Compound ' "PRO-TRNAS+"
            Public CONSTRAINT_SER_TRNA_CHARGED As ObjectModels.Entity.Compound ' "SER-TRNAS+"
            Public CONSTRAINT_THR_TRNA_CHARGED As ObjectModels.Entity.Compound ' "THR-TRNAS+"
            Public CONSTRAINT_TRP_TRNA_CHARGED As ObjectModels.Entity.Compound ' "TRP-TRNAS+"
            Public CONSTRAINT_TYR_TRNA_CHARGED As ObjectModels.Entity.Compound ' "TYR-TRNAS+"
            Public CONSTRAINT_VAL_TRNA_CHARGED As ObjectModels.Entity.Compound ' "VAL-TRNAS+"

            Public CONSTRAINT_ALA_TRNA As ObjectModels.Entity.Compound ' "ALA-TRNAS"
            Public CONSTRAINT_ARG_TRNA As ObjectModels.Entity.Compound ' "ARG-TRNAS"
            Public CONSTRAINT_ASN_TRNA As ObjectModels.Entity.Compound ' "ASN-TRNAS"
            Public CONSTRAINT_ASP_TRNA As ObjectModels.Entity.Compound ' "ASP-TRNAS"
            Public CONSTRAINT_CYS_TRNA As ObjectModels.Entity.Compound ' "CYS-TRNAS"
            Public CONSTRAINT_GLN_TRNA As ObjectModels.Entity.Compound ' "GLN-TRNAS"
            Public CONSTRAINT_GLT_TRNA As ObjectModels.Entity.Compound ' "GLT-TRNAS"
            Public CONSTRAINT_GLY_TRNA As ObjectModels.Entity.Compound ' "GLY-TRNAS"
            Public CONSTRAINT_HIS_TRNA As ObjectModels.Entity.Compound ' "HIS-TRNAS"
            Public CONSTRAINT_ILE_TRNA As ObjectModels.Entity.Compound ' "ILE-TRNAS"
            Public CONSTRAINT_LEU_TRNA As ObjectModels.Entity.Compound ' "LEU-TRNAS"
            Public CONSTRAINT_LYS_TRNA As ObjectModels.Entity.Compound ' "LYS-TRNAS"
            Public CONSTRAINT_MET_TRNA As ObjectModels.Entity.Compound ' "MET-TRNAS"
            Public CONSTRAINT_PHE_TRNA As ObjectModels.Entity.Compound ' "PHE-TRNAS"
            Public CONSTRAINT_PRO_TRNA As ObjectModels.Entity.Compound ' "PRO-TRNAS"
            Public CONSTRAINT_SER_TRNA As ObjectModels.Entity.Compound ' "SER-TRNAS"
            Public CONSTRAINT_THR_TRNA As ObjectModels.Entity.Compound ' "THR-TRNAS"
            Public CONSTRAINT_TRP_TRNA As ObjectModels.Entity.Compound ' "TRP-TRNAS"
            Public CONSTRAINT_TYR_TRNA As ObjectModels.Entity.Compound ' "TYR-TRNAS"
            Public CONSTRAINT_VAL_TRNA As ObjectModels.Entity.Compound ' "VAL-TRNAS"

            Public CONSTRAINT_ATP As ObjectModels.Entity.Compound ' "ATP"
            Public CONSTRAINT_ADP As ObjectModels.Entity.Compound ' "ADP"

            Public CONSTRAINT_CTP As ObjectModels.Entity.Compound ' "CTP"
            Public CONSTRAINT_GTP As ObjectModels.Entity.Compound ' "GTP"
            Public CONSTRAINT_UTP As ObjectModels.Entity.Compound ' "UTP"

            Public CONSTRAINT_ALA As ObjectModels.Entity.Compound '= "ALA"
            Public CONSTRAINT_ARG As ObjectModels.Entity.Compound '= "ARG"
            Public CONSTRAINT_ASP As ObjectModels.Entity.Compound '= "ASP"
            Public CONSTRAINT_ASN As ObjectModels.Entity.Compound '= "ASN"
            Public CONSTRAINT_CYS As ObjectModels.Entity.Compound '= "CYS"
            Public CONSTRAINT_GLN As ObjectModels.Entity.Compound '= "GLN"
            Public CONSTRAINT_GLU As ObjectModels.Entity.Compound '= "GLT"
            Public CONSTRAINT_GLY As ObjectModels.Entity.Compound '= "GLY"
            Public CONSTRAINT_HIS As ObjectModels.Entity.Compound '= "HIS"
            Public CONSTRAINT_ILE As ObjectModels.Entity.Compound '= "ILE"
            Public CONSTRAINT_LEU As ObjectModels.Entity.Compound '= "LEU"
            Public CONSTRAINT_LYS As ObjectModels.Entity.Compound '= "LYS"
            Public CONSTRAINT_MET As ObjectModels.Entity.Compound '= "MET"
            Public CONSTRAINT_PHE As ObjectModels.Entity.Compound '= "PHE"
            Public CONSTRAINT_PRO As ObjectModels.Entity.Compound '= "PRO"
            Public CONSTRAINT_SER As ObjectModels.Entity.Compound '= "SER"
            Public CONSTRAINT_THR As ObjectModels.Entity.Compound '= "THR"
            Public CONSTRAINT_TRP As ObjectModels.Entity.Compound '= "TRP"
            Public CONSTRAINT_TYR As ObjectModels.Entity.Compound '= "TYR"
            Public CONSTRAINT_VAL As ObjectModels.Entity.Compound '= "VAL"

            Public CONSTRAINT_WATER_MOLECULE As ObjectModels.Entity.Compound

            Public CONSTRAINT_PI As ObjectModels.Entity.Compound

            Dim DataCollection As Entity.Compound()

            Default Public ReadOnly Property Metabolite(UniqueId As String) As Entity.Compound
                Get
                    Return DataCollection.GetItem(UniqueId)
                End Get
            End Property

            ''' <summary>
            ''' 假若模型中的ATP合成模型不能够完成的，话，可以试试这个对象进行ATP的补偿，这个方法会尝试将ATP合成有关的酶与ATP的量相联系起来
            ''' </summary>
            ''' <remarks></remarks>
            Dim ATP_Compensate As Entity.Compound()

            ''' <summary>
            ''' 筛选出所有与ATP合成有关的酶分子
            ''' </summary>
            ''' <param name="MetabolismSystem"></param>
            ''' <remarks></remarks>
            Public Sub setup_ATP_Compensate(MetabolismSystem As ObjectModels.SubSystem.MetabolismCompartment)
                Dim LQuery = (From Flux In MetabolismSystem.DelegateSystem.NetworkComponents
                              Let Coefficient As Double = Flux.GetCoefficient(CONSTRAINT_ATP.Identifier)
                              Where Flux.TypeId = ObjectModel.TypeIds.EnzymaticFlux AndAlso Coefficient <> 0
                              Let GetEnzymeAssociation = Function() As Feature.MetabolismEnzyme()
                                                             If Flux.Reversible = False Then
                                                                 If Coefficient > 0 Then
                                                                     Return DirectCast(Flux, [Module].EnzymaticFlux).Enzymes
                                                                 End If
                                                             Else
                                                                 Return DirectCast(Flux, [Module].EnzymaticFlux).Enzymes
                                                             End If

                                                             Return Nothing
                                                         End Function Select GetEnzymeAssociation()).ToArray
                Dim ChunkBuffer As List(Of Entity.Compound) = New List(Of Entity.Compound)
                For Each Line In LQuery
                    If Line.IsNullOrEmpty Then
                        Continue For
                    End If

                    Call ChunkBuffer.AddRange((From item In Line Select item.EnzymeMetabolite).ToArray)
                Next

                ATP_Compensate = ChunkBuffer.ToArray
            End Sub

            Public Sub InvokeATP_Compensate()
                Dim LQuery = (From Association In ATP_Compensate Select Association.Quantity).ToArray.Sum
                CONSTRAINT_ATP.Quantity = CONSTRAINT_ATP.DataSource.value + Global.System.Math.Log(LQuery + Global.System.Math.E)
#If DEBUG Then
                CONSTRAINT_ATP.Quantity = CONSTRAINT_ATP.DataSource.value + 100000
#End If
            End Sub

            Public Shared Sub Initialize(MetabolismSystem As ObjectModels.SubSystem.MetabolismCompartment)
                Dim ConstraintMetabolite = New ConstraintMetabolites
                Dim Metabolites = MetabolismSystem.Metabolites
                Dim ConstraintModels = MetabolismSystem._CellSystem.DataModel.Metabolism.ConstraintMetaboliteMaps

                ConstraintMetabolite.ATP_Compensate = New Entity.Compound() {}

                Try
                    ConstraintMetabolite.CONSTRAINT_ALA_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("ALA-TRNAS+").ModelId))
                    ConstraintMetabolite.CONSTRAINT_ARG_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("ARG-TRNAS+").ModelId))
                    ConstraintMetabolite.CONSTRAINT_ASN_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("ASN-TRNAS+").ModelId))
                    ConstraintMetabolite.CONSTRAINT_ASP_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("ASP-TRNAS+").ModelId))
                    ConstraintMetabolite.CONSTRAINT_CYS_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("CYS-TRNAS+").ModelId))
                    ConstraintMetabolite.CONSTRAINT_GLN_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("GLN-TRNAS+").ModelId))
                    ConstraintMetabolite.CONSTRAINT_GLT_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("GLT-TRNAS+").ModelId))
                    ConstraintMetabolite.CONSTRAINT_GLY_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("GLY-TRNAS+").ModelId))
                    ConstraintMetabolite.CONSTRAINT_HIS_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("HIS-TRNAS+").ModelId))
                    ConstraintMetabolite.CONSTRAINT_ILE_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("ILE-TRNAS+").ModelId))
                    ConstraintMetabolite.CONSTRAINT_LEU_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("LEU-TRNAS+").ModelId))
                    ConstraintMetabolite.CONSTRAINT_LYS_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("LYS-TRNAS+").ModelId))
                    ConstraintMetabolite.CONSTRAINT_MET_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("MET-TRNAS+").ModelId))
                    ConstraintMetabolite.CONSTRAINT_PHE_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("PHE-TRNAS+").ModelId))
                    ConstraintMetabolite.CONSTRAINT_PRO_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("PRO-TRNAS+").ModelId))
                    ConstraintMetabolite.CONSTRAINT_SER_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("SER-TRNAS+").ModelId))
                    ConstraintMetabolite.CONSTRAINT_THR_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("THR-TRNAS+").ModelId))
                    ConstraintMetabolite.CONSTRAINT_TRP_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("TRP-TRNAS+").ModelId))
                    ConstraintMetabolite.CONSTRAINT_TYR_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("TYR-TRNAS+").ModelId))
                    ConstraintMetabolite.CONSTRAINT_VAL_TRNA_CHARGED = Metabolites.GetItem((ConstraintModels.GetItem("VAL-TRNAS+").ModelId))

                    ConstraintMetabolite.CONSTRAINT_ALA_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("ALA-TRNAS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_ARG_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("ARG-TRNAS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_ASN_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("ASN-TRNAS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_ASP_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("ASP-TRNAS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_CYS_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("CYS-TRNAS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_GLN_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("GLN-TRNAS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_GLT_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("GLT-TRNAS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_GLY_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("GLY-TRNAS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_HIS_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("HIS-TRNAS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_ILE_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("ILE-TRNAS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_LEU_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("LEU-TRNAS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_LYS_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("LYS-TRNAS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_MET_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("MET-TRNAS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_PHE_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("PHE-TRNAS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_PRO_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("PRO-TRNAS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_SER_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("SER-TRNAS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_THR_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("THR-TRNAS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_TRP_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("TRP-TRNAS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_TYR_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("TYR-TRNAS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_VAL_TRNA = Metabolites.GetItem((ConstraintModels.GetItem("VAL-TRNAS").ModelId))

                    ConstraintMetabolite.CONSTRAINT_ATP = Metabolites.GetItem((ConstraintModels.GetItem("ATP").ModelId))
                    ConstraintMetabolite.CONSTRAINT_ADP = Metabolites.GetItem((ConstraintModels.GetItem("ADP").ModelId))

                    ConstraintMetabolite.CONSTRAINT_GTP = Metabolites.GetItem((ConstraintModels.GetItem("GTP").ModelId))
                    ConstraintMetabolite.CONSTRAINT_CTP = Metabolites.GetItem((ConstraintModels.GetItem("CTP").ModelId))
                    ConstraintMetabolite.CONSTRAINT_UTP = Metabolites.GetItem((ConstraintModels.GetItem("UTP").ModelId))


                    ConstraintMetabolite.CONSTRAINT_ALA = Metabolites.GetItem((ConstraintModels.GetItem("ALA").ModelId))
                    ConstraintMetabolite.CONSTRAINT_ARG = Metabolites.GetItem((ConstraintModels.GetItem("ARG").ModelId))
                    ConstraintMetabolite.CONSTRAINT_ASP = Metabolites.GetItem((ConstraintModels.GetItem("ASP").ModelId))
                    ConstraintMetabolite.CONSTRAINT_ASN = Metabolites.GetItem((ConstraintModels.GetItem("ASN").ModelId))
                    ConstraintMetabolite.CONSTRAINT_CYS = Metabolites.GetItem((ConstraintModels.GetItem("CYS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_GLN = Metabolites.GetItem((ConstraintModels.GetItem("GLN").ModelId))
                    ConstraintMetabolite.CONSTRAINT_GLU = Metabolites.GetItem((ConstraintModels.GetItem("GLT").ModelId))
                    ConstraintMetabolite.CONSTRAINT_GLY = Metabolites.GetItem((ConstraintModels.GetItem("GLY").ModelId))
                    ConstraintMetabolite.CONSTRAINT_HIS = Metabolites.GetItem((ConstraintModels.GetItem("HIS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_ILE = Metabolites.GetItem((ConstraintModels.GetItem("ILE").ModelId))
                    ConstraintMetabolite.CONSTRAINT_LEU = Metabolites.GetItem((ConstraintModels.GetItem("LEU").ModelId))
                    ConstraintMetabolite.CONSTRAINT_LYS = Metabolites.GetItem((ConstraintModels.GetItem("LYS").ModelId))
                    ConstraintMetabolite.CONSTRAINT_MET = Metabolites.GetItem((ConstraintModels.GetItem("MET").ModelId))
                    ConstraintMetabolite.CONSTRAINT_PHE = Metabolites.GetItem((ConstraintModels.GetItem("PHE").ModelId))
                    ConstraintMetabolite.CONSTRAINT_PRO = Metabolites.GetItem((ConstraintModels.GetItem("PRO").ModelId))
                    ConstraintMetabolite.CONSTRAINT_SER = Metabolites.GetItem((ConstraintModels.GetItem("SER").ModelId))
                    ConstraintMetabolite.CONSTRAINT_THR = Metabolites.GetItem((ConstraintModels.GetItem("THR").ModelId))
                    ConstraintMetabolite.CONSTRAINT_TRP = Metabolites.GetItem((ConstraintModels.GetItem("TRP").ModelId))
                    ConstraintMetabolite.CONSTRAINT_TYR = Metabolites.GetItem((ConstraintModels.GetItem("TYR").ModelId))
                    ConstraintMetabolite.CONSTRAINT_VAL = Metabolites.GetItem((ConstraintModels.GetItem("VAL").ModelId))
                    ConstraintMetabolite.CONSTRAINT_PI = Metabolites.GetItem((ConstraintModels.GetItem("PI").ModelId))

                    ConstraintMetabolite.CONSTRAINT_WATER_MOLECULE = Metabolites.GetItem((ConstraintModels.GetItem("H2O").ModelId))

                    MetabolismSystem.ConstraintMetabolite = ConstraintMetabolite

                    ConstraintMetabolite.DataCollection = New Entity.Compound() {
                        ConstraintMetabolite.CONSTRAINT_ALA_TRNA_CHARGED,
                        ConstraintMetabolite.CONSTRAINT_ARG_TRNA_CHARGED,
                        ConstraintMetabolite.CONSTRAINT_ASN_TRNA_CHARGED,
                        ConstraintMetabolite.CONSTRAINT_ASP_TRNA_CHARGED,
                        ConstraintMetabolite.CONSTRAINT_CYS_TRNA_CHARGED,
                        ConstraintMetabolite.CONSTRAINT_GLN_TRNA_CHARGED,
                        ConstraintMetabolite.CONSTRAINT_GLT_TRNA_CHARGED,
                        ConstraintMetabolite.CONSTRAINT_GLY_TRNA_CHARGED,
                        ConstraintMetabolite.CONSTRAINT_HIS_TRNA_CHARGED,
                        ConstraintMetabolite.CONSTRAINT_ILE_TRNA_CHARGED,
                        ConstraintMetabolite.CONSTRAINT_LEU_TRNA_CHARGED,
                        ConstraintMetabolite.CONSTRAINT_LYS_TRNA_CHARGED,
                        ConstraintMetabolite.CONSTRAINT_MET_TRNA_CHARGED,
                        ConstraintMetabolite.CONSTRAINT_PHE_TRNA_CHARGED,
                        ConstraintMetabolite.CONSTRAINT_PRO_TRNA_CHARGED,
                        ConstraintMetabolite.CONSTRAINT_SER_TRNA_CHARGED,
                        ConstraintMetabolite.CONSTRAINT_THR_TRNA_CHARGED,
                        ConstraintMetabolite.CONSTRAINT_TRP_TRNA_CHARGED,
                        ConstraintMetabolite.CONSTRAINT_TYR_TRNA_CHARGED,
                        ConstraintMetabolite.CONSTRAINT_VAL_TRNA_CHARGED, _
  ConstraintMetabolite.CONSTRAINT_ALA_TRNA,
                    ConstraintMetabolite.CONSTRAINT_ARG_TRNA,
                    ConstraintMetabolite.CONSTRAINT_ASN_TRNA,
                    ConstraintMetabolite.CONSTRAINT_ASP_TRNA,
                    ConstraintMetabolite.CONSTRAINT_CYS_TRNA,
                    ConstraintMetabolite.CONSTRAINT_GLN_TRNA,
                    ConstraintMetabolite.CONSTRAINT_GLT_TRNA,
                    ConstraintMetabolite.CONSTRAINT_GLY_TRNA,
                    ConstraintMetabolite.CONSTRAINT_HIS_TRNA,
                    ConstraintMetabolite.CONSTRAINT_ILE_TRNA,
                    ConstraintMetabolite.CONSTRAINT_LEU_TRNA,
                    ConstraintMetabolite.CONSTRAINT_LYS_TRNA,
                    ConstraintMetabolite.CONSTRAINT_MET_TRNA,
                    ConstraintMetabolite.CONSTRAINT_PHE_TRNA,
                    ConstraintMetabolite.CONSTRAINT_PRO_TRNA,
                    ConstraintMetabolite.CONSTRAINT_SER_TRNA,
                    ConstraintMetabolite.CONSTRAINT_THR_TRNA,
                    ConstraintMetabolite.CONSTRAINT_TRP_TRNA,
                    ConstraintMetabolite.CONSTRAINT_TYR_TRNA,
                    ConstraintMetabolite.CONSTRAINT_VAL_TRNA,
 _
                    ConstraintMetabolite.CONSTRAINT_ATP,
                    ConstraintMetabolite.CONSTRAINT_ADP,
 _
                    ConstraintMetabolite.CONSTRAINT_GTP,
                    ConstraintMetabolite.CONSTRAINT_CTP,
                    ConstraintMetabolite.CONSTRAINT_UTP,
 _
                    ConstraintMetabolite.CONSTRAINT_ALA,
                    ConstraintMetabolite.CONSTRAINT_ARG,
                    ConstraintMetabolite.CONSTRAINT_ASP,
                    ConstraintMetabolite.CONSTRAINT_ASN,
                    ConstraintMetabolite.CONSTRAINT_CYS,
                    ConstraintMetabolite.CONSTRAINT_GLN,
                    ConstraintMetabolite.CONSTRAINT_GLU,
                    ConstraintMetabolite.CONSTRAINT_GLY,
                    ConstraintMetabolite.CONSTRAINT_HIS,
                    ConstraintMetabolite.CONSTRAINT_ILE,
                    ConstraintMetabolite.CONSTRAINT_LEU,
                    ConstraintMetabolite.CONSTRAINT_LYS,
                    ConstraintMetabolite.CONSTRAINT_MET,
                    ConstraintMetabolite.CONSTRAINT_PHE,
                    ConstraintMetabolite.CONSTRAINT_PRO,
                    ConstraintMetabolite.CONSTRAINT_SER,
                    ConstraintMetabolite.CONSTRAINT_THR,
                    ConstraintMetabolite.CONSTRAINT_TRP,
                    ConstraintMetabolite.CONSTRAINT_TYR,
                    ConstraintMetabolite.CONSTRAINT_VAL,
 _
                    ConstraintMetabolite.CONSTRAINT_WATER_MOLECULE,
                        ConstraintMetabolite.CONSTRAINT_PI}
                Catch ex As Exception
                    Call MetabolismSystem.SystemLogging.WriteLine("The constraints condition was broken!" & vbCrLf & vbCrLf & "   ----->  " & ex.ToString,
                                                                  "Initialize(MetabolismSystem As ObjectModels.System.MetabolismCompartment)",
                                                                  Type:=MSG_TYPES.ERR)
                    Throw ex
                End Try
            End Sub

            Public Iterator Function GetEnumerator() As Global.System.Collections.Generic.IEnumerator(Of Entity.Compound) Implements Global.System.Collections.Generic.IEnumerable(Of Entity.Compound).GetEnumerator
                For Each Metabolite As Entity.Compound In Me.DataCollection
                    Yield Metabolite
                Next
            End Function

            Public Iterator Function GetEnumerator1() As Global.System.Collections.IEnumerator Implements Global.System.Collections.IEnumerable.GetEnumerator
                Yield GetEnumerator()
            End Function
        End Class

        Public Function InsertNewMetabolite(UniqueId As String) As Entity.Compound
            Dim ObjectModel As Entity.Compound = Metabolites.GetItem(UniqueId)

            If ObjectModel Is Nothing Then
                ObjectModel = Entity.Compound.CreateObject(UniqueId, 10, 0)
                ReDim Preserve Me.Metabolites(Me.Metabolites.Count)
                Me.Metabolites(Me.Metabolites.Count - 1) = ObjectModel
                ObjectModel.Handle = Me.Metabolites.Count - 1
            End If

            Return ObjectModel
        End Function
    End Class
End Namespace
