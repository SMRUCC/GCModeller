''' <summary>
''' 在进行计算的时候，是以代谢反应过程为基础的
''' 代谢途径等信息可以在导出结果之后做分类分析
''' 所以在模型之中并没有包含有代谢途径的信息？？？
''' </summary>
Public Structure Phenotype

    ''' <summary>
    ''' enzyme = protein + RNA
    ''' </summary>
    Public enzymes As String()

    ''' <summary>
    ''' 
    ''' </summary>
    Public fluxes As Reaction()

    ''' <summary>
    ''' Some protein is not an enzyme
    ''' </summary>
    Public proteins As Protein()

End Structure
