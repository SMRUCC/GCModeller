Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem

Namespace SequenceModel

    Interface ISequenceModel
        Property CompositionVector As CompositionVector
        ''' <summary>
        ''' 使用本方法生成<see cref="CompositionVector">序列组成成分</see>
        ''' </summary>
        ''' <param name="MetaCyc"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GenerateVector(MetaCyc As DatabaseLoadder) As Integer
    End Interface

    ''' <summary>
    ''' 序列分子的构成的成分的列表，即核酸链分子中的4种碱基，多肽链分子中的20种氨基酸分子
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CompositionVector
        <XmlAttribute> Public Property T As Integer()

        Public Overrides Function ToString() As String
            Return Me.GetXml
        End Function
    End Class
End Namespace