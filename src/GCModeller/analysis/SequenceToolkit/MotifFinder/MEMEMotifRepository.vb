Imports System.IO
Imports SMRUCC.genomics.Interops.NBCR
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite
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
        MyBase.New(Microsoft.VisualBasic.FileIO.Directory.FromLocalFileSystem(dir))
    End Sub

    Public Overrides Sub AddPWM(family As String, pwm As IEnumerable(Of Probability))
        Dim writer As New MemeWriter(pwm)
        Dim file As Stream = fs.OpenFile($"/{family.NormalizePathString(False)}.meme", FileMode.OpenOrCreate, FileAccess.Write)

        Using doc As New StreamWriter(file)
            Call writer.WriteDocument(doc)
            Call doc.Flush()
        End Using
    End Sub

    Public Overrides Function LoadFamilyMotifs(family As String) As IEnumerable(Of Probability)
        Return From pwm As MotifPWM
               In MEME_Suite.ParsePWMFile(DirectCast(fs, Microsoft.VisualBasic.FileIO.Directory).GetFullPath($"/{family}.meme"))
               Select CType(pwm, Probability)
    End Function
End Class
