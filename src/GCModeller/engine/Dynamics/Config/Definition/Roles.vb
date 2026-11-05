Imports Microsoft.VisualBasic.ComponentModel.Settings.Inf

Namespace Definitions

    ''' <summary>
    ''' Role of the general compound
    ''' </summary>
    Public Enum GeneralCompound
        Compound
        DNA
        RNA
        Protein
    End Enum

    ''' <summary>
    ''' 
    ''' </summary>
    ''' 
    <ClassName("transmembrane")>
    Public Class Transmembrane

        ''' <summary>
        ''' Defines the compound unique id set as the passive transmembrane in the simulation
        ''' </summary>
        ''' <returns>an array of the target compound unique id</returns>
        Public Property passive As String()

    End Class
End Namespace