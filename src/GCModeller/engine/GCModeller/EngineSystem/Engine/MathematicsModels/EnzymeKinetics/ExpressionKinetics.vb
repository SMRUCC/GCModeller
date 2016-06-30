Namespace EngineSystem.MathematicsModels.EnzymeKinetics

    ''' <summary>
    ''' Enzyme catalyze like kinetics model for the gene expression events
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ExpressionKinetics : Inherits MichaelisMenten

        Sub New(CompartmentObject As EngineSystem.ObjectModels.SubSystem.ICompartmentObject)
            Call MyBase.New(CompartmentObject)
        End Sub

        Public Overloads Function GetFluxValue(Vmax As Double, Regulation As Double, Enzyme As ObjectModels.Feature.MetabolismEnzyme) As Double
            Dim S = Regulation
            Dim Km = Enzyme.EnzymeKineticLaw.Km
            Dim pH = Get_currentPH()
            Dim T As Double = Get_currentTemperature()

            Dim v = Vmax * S / (Km + S)
            v *= Factor(Enzyme)
            v *= Factor(Enzyme.EnzymeKineticLaw, pH, T)
            Return v
        End Function
    End Class

    ''' <summary>
    ''' DFL model for gene expression regulations (<see cref="GCModeller.ModellingEngine.EngineSystem.ObjectModels.[Module].CentralDogmaInstance.Transcription"></see>, 
    ''' <see cref="GCModeller.ModellingEngine.EngineSystem.ObjectModels.[Module].CentralDogmaInstance.Translation"></see>).(基因表达事件调控的动态模糊逻辑(DFL)模型)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ExpressionRegulationDynamics : Inherits MathematicsModel

        ''' <summary>
        ''' 动态模糊逻辑计算节点
        ''' </summary>
        ''' <remarks></remarks>
        Public Interface IDFL_Node

        End Interface

        ''' <summary>
        ''' 该<see cref="IDFL_Node"></see>节点之中的计算分量
        ''' </summary>
        ''' <remarks></remarks>
        Public Interface IDFL_Dynamics

        End Interface

    End Class
End Namespace