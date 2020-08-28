Imports Microsoft.VisualBasic.Math.Scripting
Imports stdNum = System.Math

Namespace ComponentModel.Encoder

    Public Class ClassEncoder : Inherits NamedVector

        Dim m_colors As Dictionary(Of String, ColorClass)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' apply for save to file
        ''' </remarks>
        Public ReadOnly Property Colors As ColorClass()
            Get
                Return m_colors.Values.ToArray
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="color"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' apply for load from file
        ''' </remarks>
        Public Function AddClass(color As ColorClass) As ClassEncoder
            m_colors.Add(color.name, color)
            MyBase.Add(color.name, color.enumInt)

            Return Me
        End Function

        Public Function GetColor(value As Double) As ColorClass
            Dim min = m_colors.Values _
                .Select(Function(cls)
                            Return (ds:=stdNum.Abs(cls.enumInt - value), cls)
                        End Function) _
                .OrderBy(Function(a) a.ds) _
                .First

            Return min.cls
        End Function
    End Class
End Namespace