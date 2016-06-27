Namespace Assembly.MetaCyc.File.DataFiles

    Public Class Terminators : Inherits DataFile(Of Slots.Terminator)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Return {
                    "UNIQUE-ID", "TYPES", "COMMON-NAME", "CITATIONS", "COMMENT",
                    "COMMENT-INTERNAL", "COMPONENT-OF", "CREDITS", "DATA-SOURCE",
                    "DBLINKS", "DOCUMENTATION", "HIDE-SLOT?",
                    "INSTANCE-NAME-TEMPLATE", "LEFT-END-POSITION", "MEMBER-SORT-FN",
                    "RIGHT-END-POSITION", "SYNONYMS", "TEMPLATE-FILE"
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function
    End Class
End Namespace