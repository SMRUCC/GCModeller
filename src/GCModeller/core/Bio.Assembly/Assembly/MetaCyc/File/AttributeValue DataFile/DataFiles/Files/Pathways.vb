Imports LANS.SystemsBiology.Assembly.MetaCyc.Schema.Reflection

Namespace Assembly.MetaCyc.File.DataFiles

    ''' <summary>
    ''' Frames in class Pathways encode metabolic and signaling pathways.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Pathways : Inherits DataFile(Of Slots.Pathway)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Return {
                    "UNIQUE-ID", "TYPES", "COMMON-NAME", "CITATIONS", "CLASS-INSTANCE-LINKS",
                    "COMMENT", "CREDITS", "DBLINKS", "ENZYME-USE", "HYPOTHETICAL-REACTIONS",
                    "IN-PATHWAY", "NET-REACTION-EQUATION", "PATHWAY-INTERACTIONS", "PATHWAY-LINKS",
                    "POLYMERIZATION-LINKS", "PREDECESSORS", "PRIMARIES", "REACTION-LAYOUT",
                    "REACTION-LIST", "SPECIES", "SUB-PATHWAYS", "SUPER-PATHWAYS", "SYNONYMS"
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function
    End Class
End Namespace