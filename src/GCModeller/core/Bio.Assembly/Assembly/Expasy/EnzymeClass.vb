Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.Expasy.AnnotationsTool

    ''' <summary>
    ''' 这个是最终的酶分类结果的呈现形式
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EnzymeClass : Implements sIdEnumerable

        ''' <summary>
        ''' 一种酶分子是可能同时具备有多个酶分类编号的
        ''' </summary>
        ''' <remarks></remarks>
        Public Property EC_Class As String()
        Public Property ProteinId As String Implements sIdEnumerable.Identifier
        ''' <summary>
        ''' {[EC] Annotation}
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ExpasyAnnotations As String()
        Public Property Catalysts As String()
        Public Property Hits As String()

        ''' <summary>
        ''' KEGG数据库之中的Reaction的编号: {[KEGG_Reaction_Entry] Reaction}
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property KEGG_ENTRIES As String()

        Public Overrides Function ToString() As String
            Return String.Format("{0} -> {1}", ProteinId, String.Join(", ", EC_Class))
        End Function
    End Class

    Public MustInherit Class T_ECPaired : Implements sIdEnumerable
        Implements IKeyValuePairObject(Of String, String)

        Public Property ProteinId As String Implements sIdEnumerable.Identifier, IKeyValuePairObject(Of String, String).Identifier
        Public Property UniprotMatched As String Implements IKeyValuePairObject(Of String, String).Value

        Public Overrides Function ToString() As String
            Return MyClass.GetJson
        End Function
    End Class

    ''' <summary>
    ''' The raw annotation data which was export from the blast output text.(蛋白酶的酶编号分类（这个数据结构是用来表示blast比对结果的）)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class T_EnzymeClass_BLAST_OUT : Inherits T_ECPaired

        ''' <summary>
        ''' 酶分类的EC编号
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property [Class] As String
        Public Property EValue As Double
        Public Property Identity As Double
    End Class
End Namespace