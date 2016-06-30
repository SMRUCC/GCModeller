Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace StringDB.MIF25.Nodes

    Public Class Entry

        <XmlArray("experimentList")> Public Property ExperimentList As ExperimentDescription()
            Get
                Return _experiments.Values.ToArray
            End Get
            Set(value As ExperimentDescription())
                If value.IsNullOrEmpty Then
                    _experiments = New Dictionary(Of Integer, ExperimentDescription)
                Else
                    _experiments = value.ToDictionary(Function(obj) obj.Id)
                End If
            End Set
        End Property
        <XmlArray("interactorList")> Public Property InteractorList As Interactor()
            Get
                Return _interactors.Values.ToArray
            End Get
            Set(value As Interactor())
                If value.IsNullOrEmpty Then
                    _interactors = New Dictionary(Of Integer, Interactor)
                Else
                    _interactors = value.ToDictionary(Function(obj) obj.Id)
                End If
            End Set
        End Property
        <XmlArray("interactionList")> Public Property InteractionList As Interaction()

        Dim _interactors As Dictionary(Of Integer, Interactor)
        Dim _experiments As Dictionary(Of Integer, ExperimentDescription)

        Public Function GetInteractor(handle As Integer) As Interactor
            If _interactors.ContainsKey(handle) Then
                Return _interactors(handle)
            Else
                Return Nothing
            End If
        End Function

        Public Function GetExperiment(handle As Integer) As ExperimentDescription
            If _experiments.ContainsKey(handle) Then
                Return _experiments(handle)
            Else
                Return Nothing
            End If
        End Function

        Public Overrides Function ToString() As String
            Return String.Join(", ", (From obj In _interactors Select obj.Value.Synonym).ToArray)
        End Function
    End Class
End Namespace