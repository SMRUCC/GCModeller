Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace Core

    Friend Class CompartTable

        Friend ReadOnly compartments As New Dictionary(Of String, Dictionary(Of String, Factor))
        Friend ReadOnly mapping As New Dictionary(Of String, (source_id As String, compart_id As String))

        ReadOnly massTable As MassTable

        Public ReadOnly Property Values As IEnumerable(Of Factor)
            Get
                Return compartments.Values.Select(Function(d) d.Values).IteratesALL
            End Get
        End Property

        ''' <summary>
        ''' get a collection of the cellular compartment id
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Keys As IEnumerable(Of String)
            Get
                Return compartments.Keys
            End Get
        End Property

        Default Public ReadOnly Property compartment(compart_id As String) As Dictionary(Of String, Factor)
            Get
                If Not compartments.ContainsKey(compart_id) Then
                    Call compartments.Add(compart_id, New Dictionary(Of String, Factor))
                End If

                Return compartments(compart_id)
            End Get
        End Property

        Sub New(cache As Dictionary(Of String, Factor), compart_id As String, hook As MassTable)
            massTable = hook
            compartments = New Dictionary(Of String, Dictionary(Of String, Factor)) From {{compart_id, cache}}
        End Sub

        Sub New(hook As MassTable)
            massTable = hook
        End Sub

        Public Sub add(instance_id As String, compart_id As String, source_id As String)
            mapping(instance_id) = (source_id, compart_id)
        End Sub

        Public Sub delete(mass_id As String)
            For Each compart As Dictionary(Of String, Factor) In compartments.Values
                Call compart.Remove(mass_id)
            Next
        End Sub

        Public Function getFactor(instance_id As String) As Factor
            If Not mapping.ContainsKey(instance_id) Then
                Return Nothing
            End If

            Dim ref = mapping(instance_id)
            Dim table = Me(ref.compart_id)

            Return table(instance_id)
        End Function

        Public Function getFactor(compart_id As String, mass_id As String) As Factor
            Dim massTable As Dictionary(Of String, Factor)
            Dim instance_id As String

            If mapping.ContainsKey(mass_id) Then
                instance_id = mass_id
                compart_id = mapping(mass_id).compart_id
            ElseIf mass_id.EndsWith("@" & compart_id) Then
                instance_id = mass_id
            Else
                instance_id = mass_id & "@" & compart_id
            End If

            massTable = Me(compart_id)

            If Not massTable.ContainsKey(instance_id) Then
                If Me.massTable.m_referenceIds.ContainsKey(mass_id) Then
                    Return getFactor(compart_id, Me.massTable.m_referenceIds(mass_id))
                End If

                Throw New InvalidDataException($"missing molecule '{mass_id}' factor data inside compartment '{compart_id}'!")
            Else
                Return massTable(instance_id)
            End If
        End Function

        ''' <summary>
        ''' Create a mass factor link to the current mass environment
        ''' </summary>
        ''' <param name="mass"></param>
        ''' <param name="coefficient"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function variable(mass As String, compart As String, Optional coefficient As Double = 1) As Variable
            If Not compartments.ContainsKey(compart) Then
                Throw New InvalidDataException($"missing compartment '{compart}' for molecule: '{mass}'!")
            End If

            Dim massTable As Dictionary(Of String, Factor) = compartments(compart)
            Dim fi As Factor = massTable(mass)

            Return New Variable(fi, coefficient, False)
        End Function

    End Class

End Namespace