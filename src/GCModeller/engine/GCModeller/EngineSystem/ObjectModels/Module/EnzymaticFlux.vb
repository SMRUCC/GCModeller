Imports System.Text
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.PoolMappings
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services
Imports Microsoft.VisualBasic

Namespace EngineSystem.ObjectModels.Module

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' 基本的生化反应过程和酶促反应之间的区别在于计算模型之中，酶促反应的K常数值为动态调整的，
    ''' 而基本的生化反应过程中的K常数是固定值的
    ''' </remarks>
    Public Class EnzymaticFlux : Inherits EngineSystem.ObjectModels.Module.MetabolismFlux
        Implements EnzymaticFlux.IEnzymaticFlux

        <DumpNode> Protected Friend _Enzymes As EngineSystem.ObjectModels.Feature.MetabolismEnzyme()
        Protected _EnzymeKinetics As MathematicsModels.EnzymeKinetics.MichaelisMenten
        <DumpNode> Protected _CatalystActivity As List(Of EnzymaticFlux.IEnzymaticFlux.EnzymeCatalystActivity) =
            New List(Of IEnzymaticFlux.EnzymeCatalystActivity)
        Protected _EC As EnzymeClass

        Public ReadOnly Property ECNumber As EnzymeClass
            Get
                Return _EC
            End Get
        End Property

        Public Interface IEnzymaticFlux

            Structure EnzymeCatalystActivity
                ''' <summary>
                ''' <see cref="MetabolismFlux.Identifier">ReactionId</see>.<see cref="Feature.MetabolismEnzyme.Identifier">EnzymeId</see>>
                ''' </summary>
                ''' <remarks></remarks>
                Dim EnzymeCatalystId As String
                Dim Value As Double

                Public Overrides Function ToString() As String
                    Return String.Format("{0} -> {1}", EnzymeCatalystId, Value)
                End Function
            End Structure

            ReadOnly Property CatalystActivities As EnzymeCatalystActivity()
            ReadOnly Property Enzymes As EngineSystem.ObjectModels.Feature.MetabolismEnzyme()

            Property KineticsModel As MathematicsModels.GenericKinetic
        End Interface

        Public Overrides ReadOnly Property TypeId As ObjectModel.TypeIds
            Get
                Return TypeIds.EnzymaticFlux
            End Get
        End Property

        Public Overrides ReadOnly Property DataSource As DataSource
            Get
                Return New DataSource(Handle, FluxValue)
            End Get
        End Property

        Public Overrides ReadOnly Property FluxValue As Double
            Get
                Return (From item In _CatalystActivity Select item.Value).Sum
            End Get
        End Property

        Public Overrides Function Invoke() As Double
            Dim FluxValue As Double = 0

            Call _CatalystActivity.Clear()

            For Each Enzyme In _Enzymes.Randomize
DEBUG:          FluxValue = KineticsModel.GetValue
                FluxValue = _EnzymeKinetics.GetFluxValue(FluxValue, Enzyme)

                Call InvokeFluxConstraintsAndFlowing(FluxValue)
                Call _CatalystActivity.Add(New EnzymaticFlux.IEnzymaticFlux.EnzymeCatalystActivity With {.EnzymeCatalystId = Enzyme.Identifier, .Value = FluxValue})
            Next

            Return MyBase.FluxValue
        End Function

        Public Overloads Shared Function CreateObject(DataModel As SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction,
                                                      Enzymes As EngineSystem.ObjectModels.Feature.MetabolismEnzyme(),
                                                      EnzymeKinetics As MathematicsModels.EnzymeKinetics.MichaelisMenten,
                                                      Metabolites As Entity.Compound()) As EnzymaticFlux
            Dim EnzymaticFlux = EngineSystem.ObjectModels.Module.EnzymaticFlux.CreateObject(Of EnzymaticFlux)(DataModel, Metabolites)
            EnzymaticFlux.Identifier = EnzymaticFlux.Identifier
            EnzymaticFlux._Enzymes = Enzymes
            EnzymaticFlux._EnzymeKinetics = EnzymeKinetics
            Return EnzymaticFlux
        End Function

        Public ReadOnly Property CatalystActivities As IEnzymaticFlux.EnzymeCatalystActivity() Implements IEnzymaticFlux.CatalystActivities
            Get
                Return _CatalystActivity.ToArray
            End Get
        End Property

        Public ReadOnly Property Enzymes As Feature.MetabolismEnzyme() Implements IEnzymaticFlux.Enzymes
            Get
                Return _Enzymes
            End Get
        End Property

        Public Shadows Property KineticsModel As MathematicsModels.GenericKinetic Implements IEnzymaticFlux.KineticsModel
            Get
                Return MyBase.KineticsModel
            End Get
            Set(value As MathematicsModels.GenericKinetic)
                MyBase.KineticsModel = value
            End Set
        End Property
    End Class
End Namespace