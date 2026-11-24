Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

''' <summary>
''' the transcription start site (promoter) definition of the BioCyc database
''' </summary>
<Xref("promoters.dat")>
Public Class promoters : Inherits Model

    <AttributeField("ABSOLUTE-PLUS-1-POS")> Public Property absolutePlus1Position As Long
    <AttributeField("ALTERNATE-TSS")> Public Property alternateTSS As String()
    <AttributeField("BINDS-SIGMA-FACTOR")> Public Property sigmaFactor As String()
    <AttributeField("COMPONENT-OF")> Public Property componentOf As String()
    <AttributeField("MINUS-10-LEFT")> Public Property minus10Left As Long
    <AttributeField("MINUS-10-RIGHT")> Public Property minus10Right As Long
    <AttributeField("MINUS-35-LEFT")> Public Property minus35Left As Long
    <AttributeField("MINUS-35-RIGHT")> Public Property minus35Right As Long
    <AttributeField("LEFT-END-POSITION")> Public Property left As Long
    <AttributeField("RIGHT-END-POSITION")> Public Property right As Long
    <AttributeField("TRANSCRIPTION-DIRECTION")> Public Property direction As String
    <AttributeField("REGULATED-BY")> Public Property regulatedBy As String()
    <AttributeField("DBLINKS")> Public Property dbLinks As String()

End Class

''' <summary>
''' the transcription terminator definition of the BioCyc database
''' </summary>
<Xref("terminators.dat")>
Public Class terminators : Inherits Model

    <AttributeField("LEFT-END-POSITION")> Public Property left As Long
    <AttributeField("RIGHT-END-POSITION")> Public Property right As Long
    <AttributeField("POSITION-OF-TERMINATION")> Public Property positionOfTermination As Long
    <AttributeField("COMPONENT-OF")> Public Property componentOf As String()
    <AttributeField("UNMAPPED-COMPONENT-OF")> Public Property unmappedComponentOf As String()
    <AttributeField("DBLINKS")> Public Property dbLinks As String()

End Class

''' <summary>
''' the transcription factor binding site definition of the BioCyc database
''' </summary>
<Xref("dnabindsites.dat")>
Public Class dnabindsites : Inherits Model

    <AttributeField("ABS-CENTER-POS")> Public Property centerPosition As Long
    <AttributeField("LEFT-END-POSITION")> Public Property left As Long
    <AttributeField("RIGHT-END-POSITION")> Public Property right As Long
    <AttributeField("SITE-LENGTH")> Public Property length As Integer
    <AttributeField("TRANSCRIPTION-DIRECTION")> Public Property direction As String
    <AttributeField("INVOLVED-IN-REGULATION")> Public Property involvedInRegulation As String()
    <AttributeField("COMPONENT-OF")> Public Property componentOf As String()
    <AttributeField("DBLINKS")> Public Property dbLinks As String()

End Class

''' <summary>
''' the regulon definition of the BioCyc database: one transcription factor and all of
''' its regulated transcription units
''' </summary>
<Xref("regulons.dat")>
Public Class regulons : Inherits Model

    <AttributeField("REGULATOR")> Public Property regulator As String
    <AttributeField("REGULATED-ENTITY")> Public Property regulatedEntity As String()
    <AttributeField("DBLINKS")> Public Property dbLinks As String()
    <AttributeField("SYMMETRY")> Public Property symmetry As String

End Class

''' <summary>
''' the protein feature (domain / active site / modified residue) definition
''' </summary>
<Xref("protein-features.dat")>
Public Class protein_features : Inherits Model

    <AttributeField("FEATURE-OF")> Public Property featureOf As String()
    <AttributeField("ACCESSION-1")> Public Property accession1 As String
    <AttributeField("LEFT-END-POSITION")> Public Property left As Long
    <AttributeField("RIGHT-END-POSITION")> Public Property right As Long
    <AttributeField("RESIDUE-NUMBER")> Public Property residueNumber As Long
    <AttributeField("RESIDUE-TYPE")> Public Property residueType As String
    <AttributeField("ATTACHED-GROUP")> Public Property attachedGroup As String
    <AttributeField("CATALYTIC-ACTIVITY")> Public Property catalyticActivity As String()
    <AttributeField("HOMOLOGY-MOTIF")> Public Property homologyMotif As String
    <AttributeField("LINKAGE-TYPE")> Public Property linkageType As String
    <AttributeField("COMPONENT-OF")> Public Property componentOf As String()
    <AttributeField("POSSIBLE-FEATURE-STATES")> Public Property possibleFeatureStates As String()
    <AttributeField("DBLINKS")> Public Property dbLinks As String()

    ''' <summary>
    ''' the feature type, from the ``TYPES`` slot
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property featureType As String
        Get
            Return types.SafeQuery.FirstOrDefault
        End Get
    End Property

End Class
