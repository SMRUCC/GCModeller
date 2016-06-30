Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Public Class I_SubstrateRefx : Implements sIdEnumerable

    <DumpNode> <XmlAttribute>
    Public Property Identifier As String Implements sIdEnumerable.Identifier

    <DumpNode> <XmlAttribute>
    Public Property InitialQuantity As Double

    Public Overrides Function ToString() As String
        Return String.Format("{0} --> {1}", Identifier, InitialQuantity)
    End Function
End Class

''' <summary>
''' 培养基的功能就是源源不断的对细胞提供物质源
''' </summary>
''' <remarks></remarks>
Public Class CultivationMediums

    ''' <summary>
    ''' 培养基在计算机模型中的组成与代谢组一致，只不过在最开始的时候仅有用户所设定的一些代谢物有浓度值，则本属性提供了一个用于设置培养基初始组成的接口
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DumpNode> <XmlElement> Public Property Uptake_Substrates As I_SubstrateRefx()

    Public Shared Function MetaCycDefault() As CultivationMediums
        Dim Defualts As I_SubstrateRefx() = New I_SubstrateRefx() {
            New I_SubstrateRefx With {.Identifier = "OXYGEN-MOLECULE", .InitialQuantity = 20},
            New I_SubstrateRefx With {.Identifier = "GLUCOSE", .InitialQuantity = 20},
            New I_SubstrateRefx With {.Identifier = "AMMONIA", .InitialQuantity = 20}
        }

        Return New CultivationMediums With {
            .Uptake_Substrates = Defualts
        }
    End Function
End Class
