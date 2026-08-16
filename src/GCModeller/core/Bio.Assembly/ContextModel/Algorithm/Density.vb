Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace ContextModel

    ''' <summary>
    ''' Genomics context relative abundance
    ''' </summary>
    Public Class Density : Implements INamedValue

        ''' <summary>
        ''' The gene locus_tag identifier
        ''' </summary>
        ''' <returns></returns>
        Public Property locus_tag As String Implements INamedValue.Key
        Public Property loci As NucleotideLocation
        ''' <summary>
        ''' The specific features on the genome its relative abundance relative to this gene <see cref="locus_tag"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property Abundance As Double
        Public Property Hits As String()
        ''' <summary>
        ''' Current gene object its function annotation.
        ''' </summary>
        ''' <returns></returns>
        Public Property product As String

        <XmlIgnore>
        Public Property location As String
            Get
                Return loci.ToString
            End Get
            Set(value As String)
                loci = LocusExtensions.TryParse(value)
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace