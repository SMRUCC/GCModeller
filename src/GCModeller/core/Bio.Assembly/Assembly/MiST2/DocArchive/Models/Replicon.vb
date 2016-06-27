Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.ProteinModel
Imports LANS.SystemsBiology.SequenceModel
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.MiST2

    ''' <summary>
    ''' 基因组之中的一个复制子
    ''' </summary>
    Public Class Replicon

        <XmlAttribute> Public Property Accession As String
        <XmlAttribute> Public Property Replicon As String

        ''' <summary>
        ''' Replicon Size (Mbp)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Size As String

        ''' <summary>
        ''' One-component
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property OneComponent As Transducin()
        Public Property TwoComponent As TwoComponent
        Public Property Chemotaxis As Transducin()
        ''' <summary>
        ''' Extra Cytoplasmic Function
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlArray("ExtraCytoFunc")> Public Property ECF As Transducin()
        Public Property Other As Transducin()

        Public Overrides Function ToString() As String
            Return String.Format("({0}) {1}, {2}Mbp", Replicon, Accession, Size)
        End Function

        ''' <summary>
        ''' 获取在本对象中所存储的所有的蛋白质对象
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SignalProteinCollection() As Transducin()
            Dim List As List(Of Transducin) = New List(Of Transducin)
            Call List.AddRange(OneComponent)
            Call List.AddRange(CType(TwoComponent, Assembly.MiST2.Transducin()))
            Call List.AddRange(Chemotaxis)
            Call List.AddRange(ECF)
            Call List.AddRange(Other)

            Return List.ToArray
        End Function
    End Class
End Namespace