Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.SBML
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Net.Protocols

Namespace FBACompatibility

    Public Class Vector : Inherits Streams.Array.Double
        Implements FLuxBalanceModel.IMetabolite

        ''' <summary>
        ''' The Unique ID property for the metabolite.
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Identifier As String Implements FLuxBalanceModel.IMetabolite.Identifier

        Public Overrides Function ToString() As String
            Return Identifier
        End Function

        <XmlAttribute>
        Public Property InitializeAmount As Double Implements FLuxBalanceModel.IMetabolite.InitializeAmount
    End Class
End Namespace