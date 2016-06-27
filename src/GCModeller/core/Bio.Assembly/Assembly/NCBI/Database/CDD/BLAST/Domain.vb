Imports System.Xml.Serialization

Namespace Assembly.NCBI.CDD.Blastp

    ''' <summary>
    ''' The protein domain object that parse from the output log.
    ''' (从日志文件之中所解析出来的蛋白质结构域对象)
    ''' </summary>
    ''' <remarks></remarks>
    <XmlType("Domain.Architecture")> Public Structure Domain

        ''' <summary>
        ''' The protein domain id in the CDD database.
        ''' (目标蛋白质结构域在CDD数据库之中的ID编号)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public ID As String
        ''' <summary>
        ''' The left side residue number of this domain.(本结构域的左侧的残基编号)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Left As Integer
        ''' <summary>
        ''' The right side residue number of this domain.(本结构域的右侧的残基编号) 
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Right As Integer

        <XmlElement> Public CDD As CDD.SmpFile

        ''' <summary>
        ''' IDE debug
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ToString() As String
            Return String.Format("{0} <{1}, {2}>", ID, Left, Right)
        End Function

        ''' <summary>
        ''' 比较两个Domain对象的位置的前后关系
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator >(a As Domain, b As Domain) As Boolean
            Return a.Left > b.Left
        End Operator

        Public Shared Operator <(a As Domain, b As Domain) As Boolean
            Return a.Left < b.Left
        End Operator
    End Structure
End Namespace