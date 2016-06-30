Imports System.Xml.Serialization
Imports LANS.SystemsBiology.DatabaseServices.StringDB.StrPNet.TCS
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace StringDB.StrPNet

    ''' <summary>
    ''' 以TF为中心的信号转导网络，即一条信号转导网络可以使用一个输出节点和若干输入节点来表示
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Pathway : Implements sIdEnumerable

        ''' <summary>
        ''' <see cref="Assembler.TF"></see>中的<see cref="Regprecise.RegpreciseMPBBH.QueryName">标识号</see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute("TF_id")> Public Property TF As String Implements sIdEnumerable.Identifier
        <XmlElement> Public Property Effectors As String()
        ''' <summary>
        ''' 当前的这个<see cref="Pathway.TF">转录调控因子</see>是否为OCS类型
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property OCS As KeyValuePair()
        ''' <summary>
        ''' 当前的这个<see cref="Pathway.TF">转录调控因子</see>是否为TCS类型
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TCSSystem As TCS.TCS()

        <XmlAttribute> Public Property TF_MiST2Type As TFSignalTypes

        Public Enum TFSignalTypes
            TF = 0
            OneComponentType
            TwoComponentType
        End Enum

        ''' <summary>
        ''' 本TF不接受任何信号也可以工作
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NotAcceptStrPSignal As Boolean
            Get
                Dim f = (OCS.IsNullOrEmpty AndAlso TCSSystem.IsNullOrEmpty)
                Return f
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return TF
        End Function
    End Class
End Namespace