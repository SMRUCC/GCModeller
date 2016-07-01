Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat
Imports Microsoft.VisualBasic

Namespace EngineSystem.ObjectModels.Module.CentralDogmaInstance

    Public Class Translation : Inherits ExpressionProcedureEvent
        Implements IEventUnit(Of Translation)

        Dim _FluxValue As Double

        Public Overrides Function Invoke() As Double
            Dim V As Double
            If Not Me.MotifSites.IsNullOrEmpty Then
                V = (1 + ((From regulator In Me.MotifSites Select (From rn In regulator.Regulators Select rn.RegulateValue).ToArray.Sum).ToArray.Sum) / ((From regulator In Me.MotifSites Select (From rn In regulator.Regulators Select rn.RegulateValue).ToArray.Sum).ToArray.Sum + 1))
            Else
                V = 0.5
            End If

            V += 1

            If V <= 0 Then '当出现负调控作用的时候，则不能够进行表达了
                MyBase._Regulations = 0.0R
                Return 0
            End If

            Dim n As Double = Me.Template.Quantity

            V = If(V = 0.0R, 0.5, V)
            V = V * n

            MyBase._Regulations = V
            MyBase.ConstraintFlux._RegulationConstraint.Quantity = V / CompositionDelayEffect
            MyBase.ConstraintFlux.Invoke()

            V = MyBase.ConstraintFlux.FluxValue

            If V > UPPER_BOUND OrElse V = Double.PositiveInfinity Then V = UPPER_BOUND

            Product.Quantity = Product.DataSource.Value + V
            _FluxValue = V

            Return FluxValue
        End Function

        Public Overrides Function ToString() As String
            Return Product.ToString
        End Function

        Public Function set_Regulators(Regulators As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.Regulator(Of Translation)()) As Translation
            '  Me.VEC = Regulators

            For i As Integer = 0 To Regulators.Count - 1
                '    Me.VEC(i).Weight = (From item In Regulators(i).RegulatorBaseType.WeightVectors Where String.Equals(item.Key, Me.Template.UniqueID) Select item.Value).First
            Next

            Return Me
        End Function

        Protected Friend Overloads Overrides Function InitializeConstraints(MetabolismSystem As SubSystem.MetabolismCompartment) As Integer
            Dim ConstraintModel As SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction = New Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction With {
              .Name = MyBase.Identifier, .Identifier = MyBase.Identifier, .Reversible = False, .UPPER_BOUND = New Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction.Parameter With {.Value = 50}}
            Dim ConstraintMapping = MetabolismSystem.ConstraintMetabolite
            Dim p As Integer = 0

            Me.CompositionDelayEffect = Global.System.Math.Log(Me.CompositionVector.Sum + Global.System.Math.E)

            ConstraintModel.p_Dynamics_K_1 = MetabolismSystem._CellSystem.ExpressionRegulationNetwork._TranslationK1
            ConstraintModel.Enzymes = New String() {MetabolismSystem._CellSystem.ExpressionRegulationNetwork.RibosomeAssemblyCompound.Identifier}

            ConstraintModel.Reactants = {
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_ALA_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_ARG_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_ASN_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_ASP_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_CYS_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_GLN_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_GLT_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_GLY_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_HIS_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_ILE_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_LEU_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_LYS_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_MET_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_PHE_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_PRO_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_SER_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_THR_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_TRP_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_TYR_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_VAL_TRNA_CHARGED.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_ATP.Identifier, .StoiChiometry = 1}}
            p = 0

            ConstraintModel.Products = {
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_ALA_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_ARG_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_ASN_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_ASP_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_CYS_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_GLN_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_GLT_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_GLY_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_HIS_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_ILE_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_LEU_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_LYS_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_MET_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_PHE_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_PRO_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_SER_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_THR_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_TRP_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_TYR_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_VAL_TRNA.Identifier, .StoiChiometry = Global.System.Math.Log(MyBase.CompositionVector(p.MoveNext) + 2, 2)},
              New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_ADP.Identifier, .StoiChiometry = 1}}

            ConstraintFlux = MetabolismFlux.CreateObject(Of ExpressionConstraintFlux)(ConstraintModel, MetabolismSystem.Metabolites)

            Dim UniqueId As String = String.Format("{0}.translation_regulation_constraints", Me.Identifier)

            Dim RegulationConstraint As [Module].EquationModel.CompoundSpecieReference = New EquationModel.CompoundSpecieReference With {.Stoichiometry = 1, .Identifier = UniqueId,
                                                                                                                                         .EntityCompound = Entity.Compound.CreateObject(UniqueId, 0, 1)}
            Dim Reactants = ConstraintFlux._Reactants.ToList
            Call Reactants.Add(RegulationConstraint)
            ConstraintFlux._Reactants = Reactants.ToArray

            ConstraintFlux.Initialize(MetabolismSystem.Metabolites, MetabolismSystem.SystemLogging)
            ConstraintFlux._Enzymes = {MetabolismSystem._CellSystem.ExpressionRegulationNetwork.RibosomeAssemblyCompound}
            ConstraintFlux._EnzymeKinetics = MetabolismSystem._CellSystem.ExpressionRegulationNetwork.ExpressionKinetics
            ConstraintFlux._RegulationConstraint = RegulationConstraint.EntityCompound

            UPPER_BOUND = ConstraintFlux._BaseType.UPPER_BOUND.Value

            Return 0
        End Function

        Public Overrides ReadOnly Property TypeId As ObjectModel.TypeIds
            Get
                Return TypeIds.EventTranslation
            End Get
        End Property

        Public Overrides ReadOnly Property FluxValue As Double
            Get
                Return _FluxValue
            End Get
        End Property

        ''' <summary>
        ''' 多顺反子mRNA链上面的调控位点
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MotifSites As Feature.MotifSite(Of Translation)() Implements IEventUnit(Of Translation).MotifSites
    End Class
End Namespace