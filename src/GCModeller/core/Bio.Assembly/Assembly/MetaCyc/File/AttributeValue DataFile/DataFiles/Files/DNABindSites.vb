Imports LANS.SystemsBiology.Assembly.MetaCyc.Schema.Reflection

Namespace Assembly.MetaCyc.File.DataFiles

    ''' <summary>
    ''' This class describes DNA regions that are binding sites for transcription factors.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DNABindSites : Inherits DataFile(Of Slots.DNABindSite)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Return {
                    "UNIQUE-ID", "TYPES", "COMMON-NAME", "ABS-CENTER-POS", "CITATIONS",
                    "COMMENT", "COMMENT-INTERNAL", "COMPONENT-OF", "CREDITS", "DATA-SOURCE",
                    "DBLINKS", "DOCUMENTATION", "HIDE-SLOT?", "INSTANCE-NAME-TEMPLATE",
                    "INVOLVED-IN-REGULATION", "LEFT-END-POSITION", "MEMBER-SORT-FN",
                    "RIGHT-END-POSITION", "SITE-LENGTH", "SYNONYMS", "TEMPLATE-FILE"
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function
    End Class
End Namespace