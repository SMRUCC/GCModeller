Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel

Namespace DataStorage.FileModel

    Public Class CHUNK_BUFFER_TransitionStates : Inherits DataSerials(Of Boolean)
    End Class

    Public Class CHUNK_BUFFER_EntityQuantities : Inherits DataSerials(Of Double)
    End Class

    ''' <summary>
    ''' Inherits DataSerials(Of Integer)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CHUNK_BUFFER_StateEnumerations : Inherits DataSerials(Of Integer)
        Public Overrides Function ToString() As String
            Return Me.UniqueId & "  " & String.Join("; ", Me.Samples)
        End Function
    End Class

End Namespace