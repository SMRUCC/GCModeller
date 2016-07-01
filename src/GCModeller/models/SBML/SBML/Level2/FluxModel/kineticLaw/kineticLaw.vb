Imports System.Text
Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.SBML.FLuxBalanceModel.IFBA
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic

Namespace Level2.Elements

    Public Class kineticLaw

        Public Property math As MathInfo
        Public Property listOfParameters As parameter()
            Get
                Return __parameters.ToArray
            End Get
            Set(value As parameter())
                __parameters = value.ToList

                If __parameters.IsNullOrEmpty Then
                    __paramHash = New Dictionary(Of parameter)
                Else
                    __paramHash = __parameters.ToDictionary
                End If
            End Set
        End Property

        Dim __paramHash As Dictionary(Of parameter)
        Dim __parameters As List(Of parameter)

        Public Function GetParameter(name As String) As parameter
            If __paramHash.ContainsKey(name) Then
                Return __paramHash(name)
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 本属性用于生成FBA模型的目标函数的时候使用
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore> Public ReadOnly Property ObjectiveCoefficient As Double
            Get
                Dim param = GetParameter(OBJECTIVE_COEFFICIENT)

                If param Is Nothing Then
                    Return 0
                Else
                    Return param.value
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return math.ToString
        End Function
    End Class
End Namespace