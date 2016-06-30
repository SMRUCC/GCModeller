Namespace EngineSystem.ObjectModels.Module

    ''' <summary>
    ''' Comment for the <see cref="EngineSystem.ObjectModels.Module"></see> namespace
    ''' </summary>
    ''' <remarks></remarks>
    Module Comment

        ''' <summary>
        ''' 计算原理的解释
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property InteractionAlgorithm_CommentText As String
            Get
                Return _
<DFL>
The working algorithm of the GCModeller is base on the dynamics fuzzy logic. 
Why chose DFL? here is the reason:

1. The events in the living system is a probability event, the probabilities of a molecule 
interact with another molecule depends on the molecule quantity in a small unit compartment.
As the biochemical can 
</DFL>
            End Get
        End Property
    End Module
End Namespace