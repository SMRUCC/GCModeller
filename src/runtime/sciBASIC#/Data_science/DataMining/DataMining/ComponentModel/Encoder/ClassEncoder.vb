Imports stdNum = System.Math

Namespace ComponentModel.Encoder

    Public Class ClassEncoder

        Dim m_colors As New Dictionary(Of String, ColorClass)
        Dim labels As New List(Of String)

        ''' <summary>
        ''' get unique class label list
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
            If Not m_colors.ContainsKey(color.name) Then
                m_colors.Add(color.name, color)
            End If

            labels.Add(color.name)

            Return Me
        End Function

        Public Function AddClass(label As String) As ClassEncoder
            If Not m_colors.ContainsKey(label) Then
                m_colors.Add(label, New ColorClass With {.color = "000000", .enumInt = Colors.Select(Function(a) a.enumInt).Max + 1, .name = label})
            End If

            labels.Add(label)

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

        Public Iterator Function PopulateFactors() As IEnumerable(Of ColorClass)
            For Each label As String In labels
                Dim template As ColorClass = m_colors(label)
                Dim factor As New ColorClass With {
                    .color = template.color,
                    .enumInt = template.enumInt,
                    .name = template.name
                }

                Yield factor
            Next
        End Function

        Public Shared Function Union(classList As IEnumerable(Of ColorClass), newLabels As IEnumerable(Of String)) As ColorClass()
            Dim encoder As New ClassEncoder

            For Each cls In classList
                encoder.AddClass(cls)
            Next
            For Each label As String In newLabels
                encoder.AddClass(label)
            Next

            Return encoder.PopulateFactors.ToArray
        End Function
    End Class
End Namespace