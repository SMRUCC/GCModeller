Imports System.Text
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.DataTabular
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.TabularDataFiles

    ''' <summary>
    ''' (pathways.col) For each pathway in the PGDB, the file lists the genes that 
    ''' encode the enzymes in that pathway.
    ''' </summary>
    ''' <remarks>
    ''' Columns (multiple columns are indicated in parentheses; n is the maximum 
    ''' number of genes for all pathways in the PGDB):
    ''' 
    '''   UNIQUE-ID
    '''   NAME
    '''   GENE-NAME (n)
    '''   GENE-ID (n)
    ''' 
    ''' </remarks>
    Public Class Pathways

        Public Property DbProperty As [Property]
        Public Property Objects As Pathway()

        Public Shared Widening Operator CType(Path As String) As Pathways
            Dim File As MetaCyc.File.TabularFile = Path
            Dim NewObj As Generic.IEnumerable(Of Pathway) =
                From e As RecordLine
                In File.Objects
                Select CType(e.Data, Pathway)

            Return New Pathways With {.DbProperty = File.DbProperty, .Objects = NewObj.ToArray}
        End Operator
    End Class
End Namespace