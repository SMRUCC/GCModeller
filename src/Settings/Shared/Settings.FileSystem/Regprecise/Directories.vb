Namespace GCModeller.FileSystem.RegPrecise

    Module Directories

        ''' <summary>
        ''' Directory of  /Regprecise/MEME/Motif_PWM/
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Motif_PWM As String
            Get
                Return FileSystem.GetRepositoryRoot & "/Regprecise/MEME/Motif_PWM/"
            End Get
        End Property

        ''' <summary>
        ''' <see cref="FileSystem.GetRepositoryRoot"/> &amp; "/Regprecise/RegPrecise.Xml"
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property RegPreciseRegulations As String
            Get
                Return FileSystem.GetRepositoryRoot & "/Regprecise/RegPrecise.Xml"
            End Get
        End Property

        ' Public ReadOnly Property RegPreciseDownload
    End Module
End Namespace