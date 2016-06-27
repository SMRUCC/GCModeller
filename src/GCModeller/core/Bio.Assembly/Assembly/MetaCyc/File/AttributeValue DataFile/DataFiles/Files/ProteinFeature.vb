Imports LANS.SystemsBiology.Assembly.MetaCyc.Schema.Reflection

Namespace Assembly.MetaCyc.File.DataFiles

    ''' <summary>
    ''' Protein features (for example, active sites), This file lists all the protein 
    ''' features (such as active sites) in the PGDB. /* protein-features.dat */
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ProteinFeatures : Inherits DataFile(Of Slots.ProteinFeature)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Return {
                    "UNIQUE-ID", "TYPES", "COMMON-NAME", "ALTERNATE-SEQUENCE",
                    "ATTACHED-GROUP", "CATALYTIC-ACTIVITY", "CITATIONS",
                    "COMMENT", "COMMENT-INTERNAL", "COMPONENT-OF", "CREDITS",
                    "DATA-SOURCE", "DBLINKS", "DOCUMENTATION", "FEATURE-OF",
                    "HIDE-SLOT?", "HOMOLOGY-MOTIF", "INSTANCE-NAME-TEMPLATE",
                    "LEFT-END-POSITION", "LINKAGE-TYPE", "MEMBER-SORT-FN",
                    "POSSIBLE-FEATURE-STATES", "RESIDUE-NUMBER", "RESIDUE-TYPE",
                    "RIGHT-END-POSITION", "SYNONYMS", "TEMPLATE-FILE"
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function
    End Class
End Namespace