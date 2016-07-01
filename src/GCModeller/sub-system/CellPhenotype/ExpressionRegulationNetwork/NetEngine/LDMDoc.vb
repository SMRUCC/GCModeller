Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Simulation.ExpressionRegulationNetwork

    ''' <summary>
    ''' 模型文件的XML文件对象
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <XmlRoot("BinaryNetwork.NetworkModel", Namespace:="http://code.google.com/p/genome-in-code/simulations/expression_network/binary_network")>
    Public Class NetworkModel
        <XmlElement("GeneObjects")> Public Property GeneObjects As KineticsModel.BinaryExpression()
    End Class

    ''' <summary>
    ''' 网络计算的蒙特卡洛输入
    ''' </summary>
    ''' <remarks></remarks>
    Public Class NetworkInput
        Implements IKeyValuePairObject(Of String, Boolean)
        Implements sIdEnumerable

        Public Property locusId As String Implements IKeyValuePairObject(Of String, Boolean).Identifier, sIdEnumerable.Identifier

        ''' <summary>
        ''' The initialize expression level for the target <see cref="locusId">gene</see>.(初始的表达水平)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Level As Boolean Implements IKeyValuePairObject(Of String, Boolean).Value

        ''' <summary>
        ''' 这个基因是不受任何调控作用的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property NoneRegulation As Boolean = False
        Public Property InitQuantity As Integer

        Public Overrides Function ToString() As String
            Return String.Format("{0}:= {1}", locusId, Level)
        End Function
    End Class
End Namespace