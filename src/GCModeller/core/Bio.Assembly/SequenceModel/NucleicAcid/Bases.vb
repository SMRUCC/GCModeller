Imports SMRUCC.genomics.ComponentModel

Namespace SequenceModel.NucleotideModels

    Module Bases

        ''' <summary>
        ''' Formula data of each DNA bases
        ''' </summary>
        Public ReadOnly Deoxyribonucleotide As New Dictionary(Of Char, FormulaData) From {
            {"A"c, PeriodicTable.SimpleParser("C10H12N5O6P") + FormulaData.H2O},
            {"T"c, PeriodicTable.SimpleParser("C10H13N2O8P") + FormulaData.H2O},
            {"C"c, PeriodicTable.SimpleParser("C9H12N3O7P") + FormulaData.H2O},
            {"G"c, PeriodicTable.SimpleParser("C10H12N5O7P") + FormulaData.H2O}
        }

        ''' <summary>
        ''' Formula data of each RNA bases
        ''' </summary>
        Public ReadOnly Ribonucleotide As New Dictionary(Of Char, FormulaData) From {
            {"A"c, PeriodicTable.SimpleParser("C10H12N5O7P")},
            {"U"c, PeriodicTable.SimpleParser("C9H11N2O9P")},
            {"C"c, PeriodicTable.SimpleParser("C9H12N3O8P")},
            {"G"c, PeriodicTable.SimpleParser("C10H12N5O8P")}
        }

    End Module
End Namespace