Imports LANS.SystemsBiology.ComponentModel.Loci.Abstract
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace ComponentModel

    ''' <summary>
    ''' The basically information of a gene object.(这个接口对象表示了一个在计算机程序之中的最基本的基因信息的载体对象)
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IGeneBrief : Inherits ICOGDigest, IContig
    End Interface

    ''' <summary>
    ''' The COG annotation data of the genes.(基因对象的COG注释结果)
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface ICOGDigest : Inherits sIdEnumerable

        ''' <summary>
        ''' The gene object COG classification.(COG功能分类)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property COG As String
        ''' <summary>
        ''' The protein function annotation data of the gene coding product.(所编码的蛋白质产物的功能注释)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Product As String
        ''' <summary>
        ''' The nucleotide sequence length.(基因的长度)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Length As Integer

    End Interface
End Namespace