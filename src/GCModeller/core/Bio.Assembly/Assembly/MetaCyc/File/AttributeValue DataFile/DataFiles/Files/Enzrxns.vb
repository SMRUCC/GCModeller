Imports LANS.SystemsBiology.Assembly.MetaCyc.Schema.Reflection

Namespace Assembly.MetaCyc.File.DataFiles

    ''' <summary>
    ''' Frames in the class Enzymatic-Reactions describe attributes of an enzyme with respect 
    ''' to a particular reaction. For reactions that are catalyzed by more than one enzyme, 
    ''' or for enzymes that catalyze more than one reaction, multiple Enzymatic-Reactions 
    ''' frames are created, one for each enzyme/reaction pair. For example, Enzymatic-Reactions 
    ''' frames can represent the fact that two enzymes that catalyze the same reaction may be 
    ''' controlled by different activators and inhibitors.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Enzrxns : Inherits DataFile(Of Slots.Enzrxn)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Return {
                    "UNIQUE-ID", "TYPES", "COMMON-NAME", "ALTERNATIVE-COFACTORS", "ALTERNATIVE-SUBSTRATES",
                    "BASIS-FOR-ASSIGNMENT", "CITATIONS", "COFACTOR-BINDING-COMMENT", "COFACTORS",
                    "COFACTORS-OR-PROSTHETIC-GROUPS", "COMMENT", "COMMENT-INTERNAL", "CREDITS", "DATA-SOURCE",
                    "DBLINKS", "DOCUMENTATION", "ENZYME", "HIDE-SLOT?", "INSTANCE-NAME-TEMPLATE", "KCAT",
                    "KM", "MEMBER-SORT-FN", "PH-OPT", "PHYSIOLOGICALLY-RELEVANT?", "PROSTHETIC-GROUPS",
                    "REACTION", "REACTION-DIRECTION", "REGULATED-BY", "REQUIRED-PROTEIN-COMPLEX",
                    "SPECIFIC-ACTIVITY", "SYNONYMS", "TEMPERATURE-OPT", "TEMPLATE-FILE", "VMAX"
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function
    End Class
End Namespace