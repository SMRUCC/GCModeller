Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.ProteinModel
Imports LANS.SystemsBiology.SequenceModel
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.MiST2

    Public Class TwoComponent

        ''' <summary>
        ''' Histidine kinase
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlArray("HisK")> Public Property HisK As Transducin()
        ''' <summary>
        ''' Hybrid histidine kinase
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlArray("HybridHisK")> Public Property HHK As Transducin()
        ''' <summary>
        ''' Response regulator
        ''' </summary>
        ''' <remarks></remarks>
        <XmlArray("RespRegulator")> Public Property RR As Transducin()
        ''' <summary>
        ''' Hybrid response regulator
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlArray("HybridRR")> Public Property HRR As Transducin()
        <XmlArray("Others")> Public Property Other As Transducin()

        ''' <summary>
        ''' 获取所有的双组份系统之中的RR蛋白质的基因编号
        ''' </summary>
        ''' <returns></returns>
        Public Function GetRR() As String()
            Dim LQuery = (From transducin As Transducin
                          In {RR, HRR}.MatrixToVector
                          Select transducin.Identifier
                          Distinct).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 获取所有的双组份系统之中的Hisk蛋白质的基因编号
        ''' </summary>
        ''' <returns></returns>
        Public Function get_HisKinase() As String()
            Dim LQuery = (From item In {HisK, HHK}.MatrixToVector Select item.Identifier Distinct).ToArray
            Return LQuery
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("HK: {0}, HHK: {1}, RR: {2}, HRR: {3}, Other: {4}", HisK.Count, HHK.Count, RR.Count, HRR.Count, Other.Count)
        End Function

        Public Shared Narrowing Operator CType(TwoComponent As TwoComponent) As Transducin()
            Dim List As List(Of Transducin) = New List(Of Transducin)
            Call List.AddRange(TwoComponent.HisK)
            Call List.AddRange(TwoComponent.HHK)
            Call List.AddRange(TwoComponent.RR)
            Call List.AddRange(TwoComponent.HRR)
            Call List.AddRange(TwoComponent.Other)

            Return List.ToArray
        End Operator
    End Class
End Namespace