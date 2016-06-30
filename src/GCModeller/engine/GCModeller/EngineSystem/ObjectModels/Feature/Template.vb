Namespace EngineSystem.ObjectModels.Feature

    Public MustInherit Class BiomacromoleculeFeature : Inherits EngineSystem.ObjectModels.ObjectModel

        ''' <summary>
        ''' 目标大分子所具备的模板特性
        ''' </summary>
        ''' <remarks></remarks>
        Public Interface ITemplate
            Property locusId As String
            Property Quantity As Double
            ''' <summary>
            ''' 一个<see cref="Entity.Compound.Identifier">唯一标识符属性</see>，用于表示本模板分子的产物对象
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Property Products As String
        End Interface
    End Class

    Public MustInherit Class MappingFeature(Of TMappingHandler As PoolMappings.IPoolHandle) : Inherits BiomacromoleculeFeature
        Public MustOverride ReadOnly Property MappingHandler As TMappingHandler
        Public MustOverride Overrides ReadOnly Property TypeId As ObjectModel.TypeIds
    End Class

    ''' <summary>
    ''' 是mRNA，tRNA，rRNA分子合成的模板，同时也是<see cref="Feature.TransUnit"></see>对象的组成单元
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Gene : Inherits BiomacromoleculeFeature
        Implements EngineSystem.ObjectModels.Feature.BiomacromoleculeFeature.ITemplate

        ''' <summary>
        ''' RNA分子产物的UniqueId
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> Public Property Products As String Implements ITemplate.Products
        <DumpNode> Public Property ExpressionActivity As Double Implements ITemplate.Quantity
        <DumpNode> Public Overrides Property Identifier As String Implements ITemplate.locusId

        Public Overrides ReadOnly Property TypeId As ObjectModel.TypeIds
            Get
                Return TypeIds.FeatureGene
            End Get
        End Property
    End Class
End Namespace