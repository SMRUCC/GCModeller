Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace FLuxBalanceModel

    ''' <summary>
    ''' FBA计算模型的构建对象类型
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class IFBA

        Public Const LOWER_BOUND As String = "LOWER_BOUND"
        Public Const UPPER_BOUND As String = "UPPER_BOUND"
        Public Const OBJECTIVE_COEFFICIENT As String = "OBJECTIVE_COEFFICIENT"

    End Class

    Public Interface I_FBAC2(Of T_Ref As ICompoundSpecies)
        ReadOnly Property Width As Integer
        ReadOnly Property Height As Integer

        ReadOnly Property MetabolismNetwork As Generic.IEnumerable(Of I_ReactionModel(Of T_Ref))
        ReadOnly Property Metabolites As Generic.IEnumerable(Of IMetabolite)
    End Interface

    Public Interface IMetabolite : Inherits sIdEnumerable
        Property InitializeAmount As Double
    End Interface

    Public Interface I_ReactionModel(Of T_Ref As ICompoundSpecies) : Inherits sIdEnumerable, IEquation(Of T_Ref)

        ''' <summary>
        ''' Query in this reaction object that the specific metabolite is exists in this reaction or not.
        ''' (查询本生化反应对象以了解所制定的代谢物是否被本反应所使用)
        ''' </summary>
        ''' <param name="Metabolite">
        ''' The id of the target metabolite.(目标代谢物的编号)
        ''' </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetStoichiometry(Metabolite As String) As Double

        Property Name As String
        ''' <summary>
        ''' The lower bound of the flux in this reaction object.
        ''' (本生化反应对象的最小流量值)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property LOWER_BOUND As Double

        ''' <summary>
        ''' The upper bound of the flux in this reaction object.
        ''' (本生化反应对象的最大流量值)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property UPPER_BOUND As Double

        ReadOnly Property ObjectiveCoefficient As Integer
    End Interface
End Namespace