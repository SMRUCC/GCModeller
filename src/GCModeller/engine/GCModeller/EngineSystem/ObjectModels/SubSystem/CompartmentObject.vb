Namespace EngineSystem.ObjectModels.SubSystem

    ''' <summary>
    ''' (表示细胞结构内的一个被生物膜所隔绝的小区间)
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface ICompartmentObject : Inherits ICellComponentContainer

#Region "Public Property"

        <DumpNode>
        Property Metabolites As ObjectModels.Entity.Compound()
        ReadOnly Property CompartmentId As String
        ReadOnly Property EnzymeKinetics As MathematicsModels.EnzymeKinetics.MichaelisMenten
        ReadOnly Property EnvironmentFactors As MathematicsModels.EnzymeKinetics.pH_Tempratures
#End Region

        Function Initialize() As Integer

        ''' <summary>
        ''' 获取当前环境下的PH值
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Get_currentPH() As Double
        Function Get_currentTemperature() As Double
    End Interface
End Namespace