Imports LANS.SystemsBiology.ComponentModel.Loci
Imports System.Xml.Serialization
Imports System.Data.Linq.Mapping

Namespace Assembly.Bac_sRNA.org

    ''' <summary>
    ''' <see cref="Interaction.sRNAid"/> --> <see cref="Interaction.TargetName"/>
    ''' </summary>
    Public Structure Interaction

        <Column(Name:="sRNAid")> <XmlAttribute>
        Public Property sRNAid As String

        ''' <summary>
        ''' The bacterial organism species name.
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="organism")> <XmlElement>
        Public Property Organism As String
        ''' <summary>
        ''' srna_name
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="srna_name")> <XmlAttribute>
        Public Property Name As String
        <Column(Name:="regulation")> <XmlElement>
        Public Property Regulation As String

        ''' <summary>
        ''' siRNA所作用的目标基因的基因名或者基因号，这个属性总是不会空的
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="target_name")> <XmlElement>
        Public Property TargetName As String

        ''' <summary>
        ''' Reference (PMID)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column(Name:="Reference (PMID)")> <XmlAttribute>
        Public Property Reference As String

        Public Overrides Function ToString() As String
            Return sRNAid
        End Function
    End Structure
End Namespace