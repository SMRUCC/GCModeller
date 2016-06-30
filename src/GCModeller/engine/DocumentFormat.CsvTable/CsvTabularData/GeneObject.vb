Imports LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.RpsBLAST
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace FileStream

    ''' <summary>
    ''' Gene Annotiation
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GeneObject : Implements sIdEnumerable

        ''' <summary>
        ''' NCBI gene accession id
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Identifier As String Implements sIdEnumerable.Identifier
        Public Property GeneName As String
        Public Property COG As String

        ''' <summary>
        ''' 这个基因所转录出来的<see cref="Transcript.UniqueId">RNA分子产物的UniqueId属性值</see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TranscriptProduct As String
        ''' <summary>
        ''' 一个基因之上通常是由若干个motif所共同调控的，而每一个motif有可能会被多个调控因子所调控
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MotifSites As String()

        Public Overrides Function ToString() As String
            Return GeneName
        End Function

        Public Shared Function CreateObject(GenesTable As Genes, MyvaCOG As MyvaCOG()) As GeneObject()
            Dim LQuery = (From Gene As Slots.Gene
                          In GenesTable.AsParallel
                          Let CogItem = MyvaCOG.GetItem(Gene.Accession1)
                          Let GeneObject = New GeneObject With {
                              .Identifier = Gene.Accession1,
                              .GeneName = Gene.CommonName,
                              .COG = If(CogItem Is Nothing, "", CogItem.Category)
                          }
                          Select GeneObject).ToArray
            Return LQuery
        End Function

        Public Shared Function CreateObject(GenesBrief As IEnumerable(Of GeneBrief), MyvaCOG As MyvaCOG()) As GeneObject()
            Dim LQuery = (From Gene As GeneBrief In GenesBrief.AsParallel
                          Let CogItem = MyvaCOG.GetItem(Gene.Synonym)
                          Let GeneObject = New GeneObject With {
                              .Identifier = Gene.Synonym,
                              .GeneName = Gene.Product,
                              .COG = If(CogItem Is Nothing, "", CogItem.Category)
                          }
                          Select GeneObject).ToArray
            Return LQuery
        End Function
    End Class
End Namespace