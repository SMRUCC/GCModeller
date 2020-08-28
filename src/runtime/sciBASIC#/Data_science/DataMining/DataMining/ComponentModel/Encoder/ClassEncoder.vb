Imports Microsoft.VisualBasic.Math.Scripting
Imports stdNum = System.Math

Namespace ComponentModel.Encoder

    Public Class ClassEncoder : Inherits NamedVector

        Dim colors As Dictionary(Of String, ColorClass)

        Public Function AddClass(color As ColorClass) As ClassEncoder
            colors.Add(color.name, color)
            MyBase.Add(color.name, color.enumInt)

            Return Me
        End Function

        Public Function GetColor(value As Double) As ColorClass
            Dim min = colors.Values _
                .Select(Function(cls)
                            Return (ds:=stdNum.Abs(cls.enumInt - value), cls)
                        End Function) _
                .OrderBy(Function(a) a.ds) _
                .First

            Return min.cls
        End Function
    End Class
End Namespace