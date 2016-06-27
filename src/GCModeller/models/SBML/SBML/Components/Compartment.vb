Imports System.Xml.Serialization

Namespace Components

    ''' <summary>
    ''' The space region in a cell.(细胞内部的一个空间区域)
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <XmlType("compartment")>
    Public Class Compartment : Inherits IPartsBase

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", id, name)
        End Function
    End Class
End Namespace