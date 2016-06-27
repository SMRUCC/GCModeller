Imports LANS.SystemsBiology.Assembly.MetaCyc.Schema.Reflection

Namespace Assembly.MetaCyc.File.DataFiles

    ''' <summary>
    ''' The file lists all the complexes of proteins with small-molecule ligands in the PGDB.
    ''' (在本文件中列出了本菌种内的所有与小分子配基所形成的蛋白质复合物)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ProtLigandCplxes : Inherits DataFile(Of Slots.ProtLigandCplxe)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Return {
                    "UNIQUE-ID", "TYPES", "COMMON-NAME", "ABBREV-NAME", "AROMATIC-RINGS",
                    "ATOM-CHARGES", "CATALYZES", "CITATIONS", "COFACTORS-OF",
                    "COFACTORS-OR-PROSTHETIC-GROUPS-OF", "COMMENT", "COMMENT-INTERNAL",
                    "COMPONENT-COEFFICIENTS", "COMPONENT-OF", "COMPONENTS", "CONSENSUS-SEQUENCE",
                    "CREDITS", "DATA-SOURCE", "DBLINKS", "DNA-FOOTPRINT-SIZE", "DOCUMENTATION",
                    "ENZYME-NOT-USED-IN", "GO-TERMS", "HAS-NO-STRUCTURE?", "HIDE-SLOT?", "IN-MIXTURE",
                    "ISOZYME-SEQUENCE-SIMILARITY", "LOCATIONS", "MEMBER-SORT-FN", "MODIFIED-FORM",
                    "MOLECULAR-WEIGHT", "MOLECULAR-WEIGHT-EXP", "MOLECULAR-WEIGHT-KD",
                    "MOLECULAR-WEIGHT-SEQ", "N+1-NAME", "N-1-NAME", "N-NAME", "NEIDHARDT-SPOT-NUMBER",
                    "PI", "PROSTHETIC-GROUPS-OF", "RADICAL-ATOMS", "REGULATED-BY", "REGULATES",
                    "SPECIES", "STRUCTURE-BONDS", "SUPERATOMS", "SYMMETRY", "SYNONYMS", "TEMPLATE-FILE"
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function
    End Class
End Namespace