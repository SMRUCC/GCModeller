Imports LANS.SystemsBiology.Assembly.MetaCyc.Schema.Reflection

Namespace Assembly.MetaCyc.File.DataFiles

    Public Class BindRxns : Inherits DataFile(Of Slots.BindReaction)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Return {
                    "UNIQUE-ID", "TYPES", "COMMON-NAME", "ACTIVATORS",
                    "BALANCE-STATE", "BASAL-TRANSCRIPTION-VALUE",
                    "CITATIONS", "COMMENT", "COMPONENT-OF", "COMPONENTS",
                    "CREDITS", "DBLINKS", "DELTAG0", "DEPRESSORS",
                    "EC-NUMBER", "ENZYMATIC-REACTION",
                    "EQUILIBRIUM-CONSTANT", "IN-PATHWAY", "INHIBITORS",
                    "INSTANCE-NAME-TEMPLATE", "LEFT", "OFFICIAL-EC?",
                    "ORPHAN?", "REACTANTS", "REACTION-PRESENT-IN-E-COLI?",
                    "REQUIREMENTS", "RIGHT", "SIGNAL", "SPECIES",
                    "SPONTANEOUS?", "STIMULATORS", "SYNONYMS"
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function
    End Class
End Namespace