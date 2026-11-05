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
        ''' Defines the flux unique id set as the passive transmembrane flux
        ''' </summary>
        ''' <returns></returns>
        Public Property passive As String()

    End Class
End Namespace