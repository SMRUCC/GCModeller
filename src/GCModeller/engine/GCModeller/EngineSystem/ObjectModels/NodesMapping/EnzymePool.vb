Imports Microsoft.VisualBasic

Namespace EngineSystem.ObjectModels.PoolMappings

    ''' <summary>
    ''' 管理突变与进化过程之中的映射关系
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EnzymePool : Inherits PoolMappings.MappingPool(Of PoolMappings.EnzymeClass, EngineSystem.ObjectModels.Feature.MetabolismEnzyme)
        Implements IReadOnlyDictionary(Of String, List(Of Feature.MetabolismEnzyme))
        Implements IDisposable

        Sub New(ReactionModels As Generic.IEnumerable(Of LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction),
                Enzymes As Generic.IEnumerable(Of EngineSystem.ObjectModels.Feature.MetabolismEnzyme))

            Me._MappingHandlers = (From strId As String In (From item In ReactionModels Select item.EC Distinct).ToArray.AsParallel Select New PoolMappings.EnzymeClass(strId)).ToArray.AddHandle
            Me._DICT_MappingPool = New Dictionary(Of String, List(Of Feature.MetabolismEnzyme))
            For Each Item As PoolMappings.EnzymeClass In _MappingHandlers
                Call _DICT_MappingPool.Add(Item.ECNumber, New List(Of EngineSystem.ObjectModels.Feature.MetabolismEnzyme))
            Next
            For Each Enzyme In Enzymes
                Call _DICT_MappingPool(Enzyme.ECNumber).Add(Enzyme)
            Next

            Call UpdateCache()
        End Sub
    End Class
End Namespace