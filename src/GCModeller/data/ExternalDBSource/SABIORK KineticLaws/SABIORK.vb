Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports System.Text
Imports Microsoft.VisualBasic.IEnumerations
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports Microsoft.VisualBasic.ComponentModel
Imports SMRUCC.genomics.DatabaseServices.SabiorkKineticLaws.TabularDump

Namespace SabiorkKineticLaws

    Public Class SABIORK : Inherits Equation

        Public Const URL_SABIORK_KINETIC_LAWS_QUERY As String = "http://sabiork.h-its.org/sabioRestWebServices/kineticLaws?kinlawids="

        Public Property kineticLawID As Long
        Public Property startValuepH As Double
        Public Property startValueTemperature As Double
        Public Property Buffer As String
        Public Property Fast As Boolean

        Public Property CompoundSpecies As SBMLParser.CompoundSpecie()
        Public Property Identifiers As String()
        Public Property LocalParameters As TripleKeyValuesPair()

        Public Overrides Function ToString() As String
            Return kineticLawID
        End Function
    End Class
End Namespace