Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Namespace ObjectModels

    Public Class Reaction

        Protected Friend _innerEqur As Equation

        Public Property Equation As String
            Get
                If _innerEqur Is Nothing Then
                    Return ""
                End If
                Return EquationBuilder.ToString(_innerEqur)
            End Get
            Set(value As String)
                If String.IsNullOrEmpty(value) Then
                    Return
                End If
                _innerEqur = EquationBuilder.CreateObject(value)
            End Set
        End Property

        Public Property Regulations As KeyValuePair(Of String, String)()
        Public Property Id As String
        ' Public Property Enzymes As KeyValuePair(Of String, String)()
        Public Property EC As String
        Public Property Comments As String()
        Public Property Names As String()

        Public ReadOnly Property Reversible As Boolean
            Get
                Return _innerEqur.Reversible
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", EC, Comments.FirstOrDefault)
        End Function
    End Class
End Namespace
