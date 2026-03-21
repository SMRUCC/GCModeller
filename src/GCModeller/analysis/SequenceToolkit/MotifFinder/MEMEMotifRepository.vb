Imports Microsoft.VisualBasic.FileIO
Imports MotifSet = SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.PWMDatabase

''' <summary>
''' save meme motif model data inside a local directory with multiple meme text file
''' </summary>
Public Class MEMEMotifRepository : Inherits MotifSet

    Public Overrides ReadOnly Property FamilyList As String()
        Get
            Return fs.EnumerateFiles("/", "*.meme") _
                .Select(Function(file) file.BaseName) _
                .ToArray
        End Get
    End Property

    Public Sub New(dir As String)
        MyBase.New(Directory.FromLocalFileSystem(dir))
    End Sub

    Public Overrides Sub AddPWM(family As String, pwm As IEnumerable(Of Probability))

    End Sub

    Public Overrides Function LoadFamilyMotifs(family As String) As IEnumerable(Of Probability)
        Return MyBase.LoadFamilyMotifs(family)
    End Function
End Class
