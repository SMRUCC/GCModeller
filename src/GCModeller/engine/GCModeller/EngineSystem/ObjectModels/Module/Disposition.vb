#Region "Microsoft.VisualBasic::ab63ae81bc4860e6d26eb1e60a2fa844, engine\GCModeller\EngineSystem\ObjectModels\Module\Disposition.vb"

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

    '     Class DisposableCompound
    ' 
    '         Properties: DataSource, DisposalSubstrate, FluxValue, Lambda, PretendedSubstrate
    ' 
    '         Function: CreatePeptideDisposalObject, CreateTranscriptDisposalObject, GetModelBase, Initialize, InternalCreateObjects
    '                   InternalCreateReferences, Invoke, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.Data.SabiorkKineticLaws.TabularDump
Imports SMRUCC.genomics.GCModeller.Assembly
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.ObjectModels.Module

    Public Class DisposableCompound(Of MolecularType As Entity.IDisposableCompound) : Inherits EnzymaticFlux

        ''' <summary>
        ''' 介于0-1之间的数，值越大表示越不容易被降解
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> Public Property Lambda As Double

        ''' <summary>
        ''' 将要被降解的目标代谢物的假定产物
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> Public Property PretendedSubstrate As EngineSystem.ObjectModels.Entity.Compound
        <DumpNode> Public Property DisposalSubstrate As Entity.IDisposableCompound

        Public Overrides ReadOnly Property FluxValue As Double
            Get
                Return (From ItemActivityRecord In _CatalystActivity Select ItemActivityRecord.Value).ToArray.Sum
            End Get
        End Property

        Public Overrides ReadOnly Property DataSource As DataSource
            Get
                Return New DataSource(Handle, FluxValue)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return PretendedSubstrate.Identifier
        End Function

        Public Overrides Function Initialize(Metabolites() As Entity.Compound, SystemLogging As LogFile) As Integer
            Return MyBase.Initialize(Metabolites, SystemLogging)
        End Function

        Public Overrides Function Invoke() As Double
            Dim _FluxValue As Double

            Call _CatalystActivity.Clear()

            For Each Enzyme In _Enzymes.Shuffles
                Me.PretendedSubstrate.Quantity = DisposalSubstrate.DataSource.value * (1 - DisposalSubstrate.Lamda)

                _FluxValue = KineticsModel.GetValue
                _FluxValue = _EnzymeKinetics.GetFluxValue(_FluxValue, Enzyme)
                _FluxValue = MyBase.InvokeFluxConstraintsAndFlowing(_FluxValue)
                DisposalSubstrate.Quantity = DisposalSubstrate.DataSource.value - _FluxValue

                Dim log As New EnzymaticFlux.IEnzymaticFlux.EnzymeCatalystActivity With {
                    .EnzymeCatalystId = Enzyme.Identifier,
                    .Value = _FluxValue
                }

                Call _CatalystActivity.Add(log)
            Next

            Return MyBase.FluxValue
        End Function

        ''' <summary>
        ''' [3.4.11.1-RXN] 1 PEPTIDES + 1 WATER --> 1 AMINO-ACIDS-20
        ''' </summary>
        ''' <param name="Compound"></param>
        ''' <param name="Metabolism"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreatePeptideDisposalObject(Compound As EngineSystem.ObjectModels.Entity.IDisposableCompound,
                                                           Metabolism As EngineSystem.ObjectModels.SubSystem.MetabolismCompartment,
                                                           Model As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.DispositionReactant,
                                                           Products As EngineSystem.ObjectModels.Entity.Compound(),
                                                           ID_POLYPEPTIDE_DISPOSE_CATALYST As String()) As DisposableCompound(Of MolecularType)
            Dim CellDataModel = Metabolism._CellSystem.DataModel
            Dim FluxModelBase As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction =
                New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction With {
 _
                    .LOWER_BOUND = 0,
                    .UPPER_BOUND = Model.UPPER_BOUND,
                    .Identifier = Compound.Identifier,
                    .Reversible = False,
                    .Name = Compound.Identifier,
                    .p_Dynamics_K_1 = 1,
                    .p_Dynamics_K_2 = 0.9,
                    .Enzymes = ID_POLYPEPTIDE_DISPOSE_CATALYST
            }
            Dim ConstraintMapping = Metabolism.ConstraintMetabolite

            FluxModelBase.Reactants = {
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With
                    {
                        .StoiChiometry = Global.System.Math.Log10(Compound.CompositionVector.Sum + 10), .Identifier = Metabolism.ConstraintMetabolite.CONSTRAINT_WATER_MOLECULE.Identifier}}

            Dim CompositionVector As Integer() = Compound.CompositionVector
            Dim p As i32 = Scan0
            FluxModelBase.Products = {
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_ALA.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_ARG.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_ASN.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_ASP.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_CYS.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_GLN.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_GLU.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_GLY.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_HIS.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_ILE.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_LEU.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_LYS.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_MET.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_PHE.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_PRO.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_SER.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_THR.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_TRP.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_TYR.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_VAL.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)}}

            Dim Reactants = (From item In FluxModelBase.Reactants Select Metabolism.Metabolites.GetItem(item.Identifier)).ToArray

            Return InternalCreateObjects(FluxModelBase, Compound, Reactants, Products, Metabolism.Metabolites, Metabolism.EnzymeKinetics, Metabolism.SystemLogging)
        End Function

        Public Shared Function GetModelBase(GeneralTypeId As String, Models As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.DispositionReactant()) _
            As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.DispositionReactant

            For i As Integer = 0 To Models.Count - 1
                Dim item = Models(i)
                If String.Equals(item.GeneralType, GeneralTypeId) Then
                    Return item
                End If
            Next

            Return Nothing
        End Function

        Private Shared Function InternalCreateReferences(ModelBase As Generic.IEnumerable(Of GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference),
                                                         Metabolites As EngineSystem.ObjectModels.Entity.Compound()) _
            As EquationModel.CompoundSpecieReference()

            Dim LQuery = (From Compound In ModelBase Select ObjectModels.Module.EquationModel.CompoundSpecieReference.CreateObject(Model:=Compound, Metabolites:=Metabolites)).ToArray
            Return LQuery
        End Function

        Private Shared Function InternalCreateObjects(FluxModelBase As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction,
                                                      Compound As MolecularType,
                                                      Reactants As EngineSystem.ObjectModels.Entity.Compound(),
                                                      Products As EngineSystem.ObjectModels.Entity.Compound(),
                                                      Metabolites As EngineSystem.ObjectModels.Entity.Compound(),
                                                      EnzymeKinetics As MathematicsModels.EnzymeKinetics.MichaelisMenten,
                                                      SystemLogging As LogFile) As DisposableCompound(Of MolecularType)

            Dim DisposableCompound As DisposableCompound(Of MolecularType) = CreateBasicalObject(Of DisposableCompound(Of MolecularType))(FluxModelBase)

            If Not FluxModelBase.Reactants Is Nothing Then DisposableCompound._Reactants = InternalCreateReferences(FluxModelBase.Reactants, Reactants)
            If Not FluxModelBase.Products Is Nothing Then DisposableCompound._Products = InternalCreateReferences(FluxModelBase.Products, Products)

            Call DisposableCompound.FillMetabolites(SystemLogging)

            Dim ChunkTemp = DisposableCompound._Reactants
            ReDim Preserve ChunkTemp(ChunkTemp.Length)

            Dim PretendedSubstrate = New [Module].EquationModel.CompoundSpecieReference With {
                .Stoichiometry = 1,
                .Identifier = Compound.Identifier & "[Pretended]",
                .EntityCompound = Entity.Compound.CreateObject(Compound.Identifier & "[Pretended]", 0, 0)
            }
            DisposableCompound._Reactants = ChunkTemp

            ChunkTemp(ChunkTemp.Length - 1) = PretendedSubstrate

            ReDim Preserve DisposableCompound._Metabolites(DisposableCompound.MetaboliteCounts)
            DisposableCompound._Metabolites(DisposableCompound.MetaboliteCounts - 1) = PretendedSubstrate.EntityCompound
            '    DisposableCompound._Parameters = ObjectModels.Module.MetabolismFlux.ParameterF.DefaultValue(DisposableCompound.MetaboliteCounts)
            DisposableCompound.Initialize(Metabolites, SystemLogging)
            DisposableCompound.DisposalSubstrate = Compound
            DisposableCompound._Enzymes = (From EnzymeId As String In FluxModelBase.Enzymes
                                           Let params = New EnzymeCatalystKineticLaw With {
                                               .Temperature = 28,
                                               .PH = 6,
                                               .Km = 0.9
                                           }
                                           Select New Feature.MetabolismEnzyme With {
                                               .Identifier = EnzymeId,
                                               .EnzymeMetabolite = Metabolites.GetItem(EnzymeId),
                                               .EnzymeKineticLaw = MathematicsModels.EnzymeKinetics.EnzymeCatalystKineticLaw.[New](params)}).ToArray
            DisposableCompound.PretendedSubstrate = PretendedSubstrate.EntityCompound
            DisposableCompound._EnzymeKinetics = EnzymeKinetics

            Return DisposableCompound
        End Function

        ''' <summary>
        ''' [RXN0-7023] 1 RNASE-R-DEGRADATION-SUBSTRATE-RNA + 1 WATER --> 1 NUCLEOSIDE-MONOPHOSPHATES + 1 DIRIBONUCLEOTIDE
        ''' </summary>
        ''' <param name="Compound"></param>
        ''' <param name="Metabolism"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateTranscriptDisposalObject(Compound As EngineSystem.ObjectModels.Entity.IDisposableCompound,
                                                              Metabolism As EngineSystem.ObjectModels.SubSystem.MetabolismCompartment,
                                                              Model As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.DispositionReactant,
                                                              Products As EngineSystem.ObjectModels.Entity.Compound(),
                                                              ID_TRANSCRIPT_DISPOSE_CATALYST As String()) As DisposableCompound(Of MolecularType)

            Dim CellDataModel = Metabolism._CellSystem.DataModel
            Dim FluxModelBase As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction =
                New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction With {
                    .LOWER_BOUND = 0,
                    .UPPER_BOUND = Model.UPPER_BOUND,
                    .Identifier = Compound.Identifier,
                    .Reversible = False,
                    .Name = Compound.Identifier,
                    .p_Dynamics_K_1 = 1,
                    .p_Dynamics_K_2 = 0.9,
                    .Enzymes = ID_TRANSCRIPT_DISPOSE_CATALYST
            }
            Dim ConstraintMapping = Metabolism.ConstraintMetabolite

            FluxModelBase.Reactants = {
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With
                    {
                        .StoiChiometry = Global.System.Math.Log(Compound.CompositionVector.Sum + 2, 2),
                        .Identifier = Metabolism.ConstraintMetabolite.CONSTRAINT_WATER_MOLECULE.Identifier
                    }
                }

            Dim CompositionVector As Integer() = Compound.CompositionVector
            Dim p As i32 = Scan0
            FluxModelBase.Products = {
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_ADP.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_GTP.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_UTP.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_CTP.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)}
                }

            Dim Reactants = (From item In FluxModelBase.Reactants Select Metabolism.Metabolites.GetItem(item.Identifier)).ToArray

            Return InternalCreateObjects(FluxModelBase, Compound, Reactants, Products, Metabolism.Metabolites, Metabolism.EnzymeKinetics, Metabolism.SystemLogging)
        End Function
    End Class
End Namespace
