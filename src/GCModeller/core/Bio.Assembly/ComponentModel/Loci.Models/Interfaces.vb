Imports LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation

Namespace ComponentModel.Loci.Abstract

    Public Interface ILocationSegment
        ''' <summary>
        ''' Tag data on this location sequence segment.(当前的这个序列片段的标签信息)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property UniqueId As String
        ''' <summary>
        ''' The location value of this sequence segment.(这个序列片段的位置信息)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Location As Location
    End Interface

    Public Interface IContig
        Property Location As NucleotideLocation
    End Interface

    Public Interface ILocationNucleotideSegment : Inherits ILocationSegment
        ReadOnly Property Strand As Strands
    End Interface

    ''' <summary>
    ''' This type of the object has the loci location value attribute.
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface ILocationComponent : Inherits ILoci

        ''' <summary>
        ''' Right position of the loci site on sequence.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Right As Integer
    End Interface

    ''' <summary>
    ''' 只有左端起始位点的模型对象
    ''' </summary>
    Public Interface ILoci

        ''' <summary>
        ''' Left position of the loci site on sequence.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Left As Integer
    End Interface
End Namespace