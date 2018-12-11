Imports System.Xml.Serialization

Namespace NeuralNetwork.StoreProcedure

    Public Class Synapse

        <XmlAttribute> Public Property InputNeuron As String
        <XmlAttribute> Public Property OutputNeuron As String
        ''' <summary>
        ''' 两个神经元之间的连接强度
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Weight As Double
        <XmlAttribute> Public Property WeightDelta As Double

    End Class
End Namespace