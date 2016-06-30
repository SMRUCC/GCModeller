Namespace EngineSystem.ObjectModels.Entity

    ''' <summary>
    ''' 本类型是为了方便程序在空间之间进行物质交换的程序的编写而设置的一个虚构的代谢物的类型
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CompartmentExchangesVirtualCompound : Inherits Compound

        Protected Friend _CompartmentCompound As Compound
        Protected Friend _CompartmentId As String

        Public Overrides Property Quantity As Double
            Get
                Return _CompartmentCompound.Quantity
            End Get
            Set(value As Double)
                _CompartmentCompound.Quantity = value
            End Set
        End Property
    End Class
End Namespace